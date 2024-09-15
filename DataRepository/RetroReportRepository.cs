
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.Security.Principal;
using DataRepository;
using Entities;
using PracticomAttendance.Helpers;

namespace DataRepository
{
    public class RetroReportRepository : IRetroReportRepository
    {

        private readonly Practicom_attendanceContext _dbContext;

        public RetroReportRepository(Practicom_attendanceContext dbContext)
        {
            _dbContext = dbContext;
        }

        private const string StudentNotFound = "תעודת הזהות אינה קיימת במאגר.";
        private const string NoPracticomTime = "לא ניתן לדווח נוכחות מחוץ לשעות מערכת הפרקטיקום.";
        private const string CodeExpired = "תוקף קוד הכניסה פג.";
        private const string ReportIn = "דווחה כניסה במערכת.";
        private const string ReportOut = "דווחה יציאה במערכת. סך הכל חושבו {0} שעות בדיווח זה.";
        private const string ReportInOut = "דווחה נוכחות במערכת.";
        private const string NoPreviousReport = "לא דווחה {0} בדיווח קודם.";
        private const string NotFountMatchAttendance = "לא נמצאה שעת {0} התואמת לשעת ה{1} שהוזנה";
        private const string Entry = "כניסה";
        private const string Exit = "יציאה";
        private const string Space = " ";
        private const string OverlapFound = "לא ניתן לדווח רטרואקטיבית מאחר ונמצאו שעות נוכחות חופפות לדיווח.";


        public async Task<string?> retrout(AttendanceInputModel AttendanceReport)
        {
            //SPANTIMEהמרת השעה ל 
            DateTime Date = DateTime.Parse(AttendanceReport.Date);
            TimeSpan ExitTime = TimeSpan.Parse(AttendanceReport.ExitTime);
            int DayOfWeek = (int)Date.DayOfWeek + 1;

            //שליפת פרטי התלמידה 
            Student studentDetalis = _dbContext.Students.FirstOrDefault(s => s.IdentityNumber == AttendanceReport.StudentId);

            if (studentDetalis == null)
            {
                return StudentNotFound;
            }

            //לבקשת י.ח. שמעוני, היות ומערכת השעות משתנה, אין צורך להשוות למערכת השעות בדיווח רטרואקטיבי. המדווחת תבצע ידנית את הבדיקה אם השעות מאופשרות לדיווח.
            // שליפת מערכת שעות

            //Schedule currentSchedule = GetPracticomSchedule(studentDetalis.PracticomTypeId, ExitTime, DayOfWeek);
            //if (currentSchedule == null)
            //{
            //    return NoPracticomTime;
            //}

            try
            {

                Attendance existingAttendance = _dbContext.Attendances
                    .Where(a => a.StudentId == studentDetalis.Id && a.Date == Date && ExitTime > a.EntryTime)
                    .OrderByDescending(a => a.EntryTime) // הכניסה הכי צמודה ליציאה הנתונה
                    .FirstOrDefault();                   
                   // && currentSchedule.ToHour >= ExitTime && currentSchedule.FromHour <= a.EntryTime);

                if (existingAttendance != null)
                {
                    if (existingAttendance.ExitTime.HasValue)
                        return OverlapFound;

                    existingAttendance.ExitTime = ExitTime;
                    existingAttendance.DurationOfAttendance = existingAttendance.ExitTime - existingAttendance.EntryTime;

                    _dbContext.SaveChanges();

                    return string.Format(ReportOut, existingAttendance.DurationOfAttendance);

                }
                else
                {
                    return string.Format(NotFountMatchAttendance, Entry, Exit );
                }

            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }

        }

        public async Task<string?> retroin(AttendanceInputModel AttendanceRetroin)

        {
            //SPANTIMEהמרת השעה ל 
            TimeSpan EntryTime = TimeSpan.Parse(AttendanceRetroin.EntryTime);
            DateTime Date = DateTime.Parse(AttendanceRetroin.Date);
            int DayOfWeek = (int)Date.DayOfWeek + 1;

            //שליפת פרטי התלמידה 
            Student? studentDetalis = _dbContext.Students.FirstOrDefault(s => s.IdentityNumber == AttendanceRetroin.StudentId);

            if (studentDetalis == null)
            {
                return StudentNotFound;
            }

            //לבקשת י.ח. שמעוני, היות ומערכת השעות משתנה, אין צורך להשוות למערכת השעות בדיווח רטרואקטיבי. המדווחת תבצע ידנית את הבדיקה אם השעות מאופשרות לדיווח.
            //Schedule? currentSchedule = GetPracticomSchedule(studentDetalis.PracticomTypeId, EntryTime, DayOfWeek);

            //if (currentSchedule == null)
            //{
            //    return NoPracticomTime;
            //}

            try
            {
                //(בדיקה אם חסר כניסה ותואם למערכת(יציאה  

                Attendance? existingAttendance = _dbContext.Attendances
                    .Where(a => a.StudentId == studentDetalis.Id && a.Date == Date && EntryTime <= a.ExitTime)
                    .OrderBy(a => a.ExitTime)
                    .FirstOrDefault();
                    //&& currentSchedule.FromHour <= EntryTime && currentSchedule.ToHour >= a.ExitTime);

                if (existingAttendance != null)
                {
                    if (existingAttendance.EntryTime.HasValue)
                        return OverlapFound;

                    // הכנסת כניסה וחישוב הזמן
                    existingAttendance.EntryTime = EntryTime;

                    existingAttendance.DurationOfAttendance = existingAttendance.ExitTime - existingAttendance.EntryTime;

                    _dbContext.SaveChanges();

                    return string.Format(ReportIn, existingAttendance.ExitTime);


                }
                else
                {
                    return string.Format(NotFountMatchAttendance, Exit, Entry);
                }

            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }

        }


        public async Task<string> retroall(AttendanceInputModel AttendanceRetro)
        {

            TimeSpan EntryTime = TimeSpan.Parse(AttendanceRetro.EntryTime);
            TimeSpan ExitTime = TimeSpan.Parse(AttendanceRetro.ExitTime);
            DateTime Date = DateTime.Parse(AttendanceRetro.Date);
            int DayOfWeek = (int)Date.DayOfWeek + 1;

            //שליפת פרטי התלמידה 
            var studentDetalis = StudentDetalis(AttendanceRetro.StudentId);

            if (studentDetalis == null)
            {
                return StudentNotFound;
            }

            if(CheckOverlapAttendance(AttendanceRetro.StudentId, Date, EntryTime, ExitTime))
                return OverlapFound;

            //לבקשת י.ח. שמעוני, היות ומערכת השעות משתנה, אין צורך להשוות למערכת השעות בדיווח רטרואקטיבי. המדווחת תבצע ידנית את הבדיקה אם השעות מאופשרות לדיווח.
            // שליפת המערכת שעות
            //int currentDayOfWeek = (int)DateTime.Now.DayOfWeek + 1;

            //Schedule currentSchedule = _dbContext.Schedules
            //.Include(s => s.ScheduleGroupType.CdPracticomTypes)
            //.FirstOrDefault(s => s.ScheduleGroupType.CdPracticomTypes.Any(pt => pt.Id == studentDetalis.PracticomTypeId)
            //                     && s.Day == DayOfWeek
            //                     && ExitTime <= s.ToHour
            //                     && EntryTime >= s.FromHour);


            //if (currentSchedule == null)
            //{
            //    return NoPracticomTime;
            //}

            try
            {
                var newAttendance = new Attendance
                {
                    StudentId = studentDetalis.Id,
                    Date = Date,
                    ExitTime = ExitTime,
                    EntryTime = EntryTime,
                    DurationOfAttendance = ExitTime - EntryTime,

                };

                _dbContext.Attendances.Add(newAttendance);
                await _dbContext.SaveChangesAsync();

                return ReportInOut;

            }
            catch
            {
                return "הדווח נכשל";
            }

        }

        private Student? StudentDetalis(int? identityNumber)
        {

            var studentDetalis = _dbContext.Students.FirstOrDefault(s => s.IdentityNumber == identityNumber);

            return studentDetalis;
        }

        private bool CheckOverlapAttendance(int studentId, DateTime date, TimeSpan enry, TimeSpan exit)
        {
            return _dbContext.Attendances.Any(a => a.StudentId == studentId && a.Date == date && a.ExitTime >= enry && a.EntryTime <= exit);
        }

    }
}








