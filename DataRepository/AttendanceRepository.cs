using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataRepository;
using Entities;
using Microsoft.EntityFrameworkCore;
using PracticomAttendance.Helpers;

namespace DataRepository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly Practicom_attendanceContext _dbContext;

        public AttendanceRepository(Practicom_attendanceContext dbContext)
        {
            _dbContext = dbContext;
        }

        private const string StudentNotFound = "תעודת הזהות אינה קיימת במאגר.";
        private const string NoPracticomTime = "לא ניתן לדווח נוכחות מחוץ לשעות מערכת הפרקטיקום.";
        private const string CodeExpired = "תוקף קוד הכניסה פג.";
        private const string CodeIncorrect = "הכניסה נכשלה.קוד אינו תקין";
        private const string ReportIn = "דווחה כניסה במערכת. ניתן לדווח יציאה עד לשעה {0}.";
        private const string ReportOut = "דווחה יציאה במערכת. סך הכל חושבו {0} שעות בדיווח זה.";
        private const string ReportInOut = "דווחה נוכחות במערכת.";
        private const string NoPreviousReport = "לא דווחה {0} בדיווח קודם.";
        private const string NotFountMatchAttendance = "לא נמצאה שעת {0} התואמת לשעת ה{1} שהוזנה";
        private const string Entry = "כניסה";
        private const string Exit = "יציאה";
        private const string Space = " ";

        public async Task<string?> SubscribeAsync(int identityNumber)
        {
            try
            {
                //שליפת פרטי התלמידה 
                var studentDetalis = StudentDetalis(identityNumber);

                if (studentDetalis == null)
                {
                    return StudentNotFound;
                }

                //בדיקה מול מערכת השעות
                Schedule isPracticom = GetPracticomSchedule(studentDetalis.PracticomTypeId);

                if (isPracticom == null)
                {
                    return NoPracticomTime;
                }

                TempCode newTempCode = new TempCode
                {
                    StudentId = studentDetalis.Id,
                    DateAndTime = DateTime.Now
                };

                _dbContext.TempCodes.Add(newTempCode);
                await _dbContext.SaveChangesAsync();

                return newTempCode.Code.ToString();


            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> CheckinHome(int identityNumber)
        {
            try
            {
                var studentDetalis = StudentDetalis(identityNumber);

                if (studentDetalis == null)
                {
                    return StudentNotFound;
                }

                //קבלת בלוק המערכת הנוכחי להתראה עד מתי ניתן לדווח יציאה
                Schedule? schedule = GetPracticomSchedule(studentDetalis.PracticomTypeId);
                if (schedule == null)
                {
                    return NoPracticomTime;
                }

                //בדיקה האם היציאה האחרונה מלאה 
                Attendance? lastRecordedDate = _dbContext.Attendances
                .Where(a => a.StudentId == identityNumber)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

                try
                {
                    //הכנסת חדש 
                    Attendance newAttendanceRecord = new Attendance
                    {
                        StudentId = identityNumber,
                        Date = DateTime.Now.Date,
                        EntryTime = DateTime.Now.TimeOfDay,
                    };

                    _dbContext.Attendances.Add(newAttendanceRecord);
                    await _dbContext.SaveChangesAsync();

                    string message = string.Format(ReportIn, schedule.ToHour);

                    //הודעהת שגיאה אם לא בוצעה יציאה
                    if (lastRecordedDate != null && !lastRecordedDate.ExitTime.HasValue)
                        message += Space + string.Format(NoPreviousReport, Exit);

                    return message;
                }
                catch
                {
                    return "הכניסה נכשלה";

                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> CheckinAsync(int temporaryCode)
        {
            try
            {

                //בדיקה האם הקוד זהה לקוד שנמצא בטבלה 
                var tempCodeWithStudent = _dbContext.TempCodes.Include("Student")
                    .Where(tc => tc.Code == temporaryCode)
                    .Select(x => new
                    {
                        TempCode = x,
                        Student = x.Student
                    })
                    .FirstOrDefault();

                if (tempCodeWithStudent != null)
                {
                    // בדיקה האם עבר 10 דקות
                    DateTime tenMinutesLater = tempCodeWithStudent.TempCode.DateAndTime.AddMinutes(10);
                    bool tenMinutesPassed = DateTime.Now > tenMinutesLater;
                    if (tenMinutesPassed)
                    {
                        return CodeExpired;
                    }

                    //קבלת בלוק המערכת הנוכחי להתראה עד מתי ניתן לדווח יציאה
                    Schedule? schedule = GetPracticomSchedule(tempCodeWithStudent.Student.PracticomTypeId);
                    if (schedule == null)
                    {
                        return NoPracticomTime;
                    }

                    //בדיקה האם היציאה האחרונה מלאה 
                    Attendance? lastRecordedDate = _dbContext.Attendances
                    .Where(a => a.StudentId == tempCodeWithStudent.TempCode.StudentId)
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault();

                    try
                    {
                        //הכנסת חדש 
                        Attendance newAttendanceRecord = new Attendance
                        {
                            StudentId = tempCodeWithStudent.TempCode.StudentId,
                            Date = DateTime.Now.Date,
                            EntryTime = DateTime.Now.TimeOfDay,
                        };

                        _dbContext.Attendances.Add(newAttendanceRecord);
                        await _dbContext.SaveChangesAsync();

                        string message = string.Format(ReportIn, schedule.ToHour);

                        //הודעהת שגיאה אם לא בוצעה יציאה
                        if (lastRecordedDate != null && !lastRecordedDate.ExitTime.HasValue)
                            message += Space + string.Format(NoPreviousReport, Exit);

                        return message;
                    }
                    catch
                    {
                        return "הכניסה נכשלה";

                    }

                }

                return CodeIncorrect;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> CheckOutAsync(int tz)
        {
            //(שליפת פרטי התלמידה (בדיקת תז
            var studentDetalis = StudentDetalis(tz);

            //Student? studentDetalis = _dbContext.Students.FirstOrDefault(s => s.IdentityNumber == tz);

            if (studentDetalis == null)
            {
                return StudentNotFound;
            }

            //בדיקה מול מערכת השעות
            Schedule isPracticom = GetPracticomSchedule(studentDetalis.PracticomTypeId);

            if (isPracticom == null)
            {
                return NoPracticomTime;
            }

            // שליפת הנכחות האחרונה
            Attendance? lastAttendance = _dbContext.Attendances
                 .Where(a => a.StudentId == studentDetalis.Id)
                 .OrderByDescending(a => a.Id)
                 .FirstOrDefault();

            //בדיקה שיש בעבר נוכחות וזה של היום ושזה עומד בשעת פרקטיקום 
            if (lastAttendance != null
                && lastAttendance.Date == DateTime.Now.Date && lastAttendance.EntryTime >= isPracticom.FromHour && lastAttendance.EntryTime <= isPracticom.ToHour
                && !lastAttendance.ExitTime.HasValue)
            {
                //הכנסת יציאה וסיכום הנוכחות של אותו היום 
                lastAttendance.ExitTime = DateTime.Now.TimeOfDay;
                lastAttendance.DurationOfAttendance = lastAttendance.ExitTime - lastAttendance.EntryTime;

                await _dbContext.SaveChangesAsync();
                return string.Format(ReportOut, lastAttendance.DurationOfAttendance);
            }
            else
            {
                //(הוספת חדש (ללא כניסה  
                var newAttendance = new Attendance
                {
                    StudentId = studentDetalis.Id,
                    Date = DateTime.Now.Date,
                    ExitTime = DateTime.Now.TimeOfDay,
                };

                _dbContext.Attendances.Add(newAttendance);
                await _dbContext.SaveChangesAsync();

                return string.Format(ReportOut, 0) + Space + string.Format(NoPreviousReport, Entry);
            }

        }

        public async Task<string?> GetUserNameAsync()
        {
            return Space;
            ////return this.HttpContext.User.Identities.FirstOrDefault()?.Name;
            ////WindowsIdentity identity = WindowsIdentity.GetCurrent();
            ////if (identity == null)
            ////{
            ////    return "machine: " + name;
            ////}
            ////return "windows: " + identity.Nam
        }

        private Schedule? GetPracticomSchedule(int? practicomTypeId, TimeSpan? time = null, int? dow = null)
        {
            if (!dow.HasValue)
                dow = (int)DateTime.Now.DayOfWeek + 1;

            if (!time.HasValue)
                time = DateTime.Now.TimeOfDay;

            Schedule? currentSchedule = _dbContext.Schedules
                .Include(s => s.ScheduleGroupType).ThenInclude(c => c.CdPracticomTypes)
                .FirstOrDefault(s => s.ScheduleGroupTypeId.HasValue && s.ScheduleGroupType.CdPracticomTypes.Any(pt => pt.Id == practicomTypeId)
                                     && s.Day == dow
                                     && time >= s.FromHour
                                     && time <= s.ToHour);

            return currentSchedule;
        }

        private Student? StudentDetalis(int? identityNumber)
        {

            var studentDetalis = _dbContext.Students.FirstOrDefault(s => s.IdentityNumber == identityNumber);

            return studentDetalis;
        }


    }
}
