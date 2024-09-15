using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataRepository
{
    public class AssessmentRepository:IAssessmentRepository
    {
        private readonly Practicom_attendanceContext _dbContext;


        private const string found = "משתמש רשום";
        private const string notFound = "משתמש לא רשום";

        public AssessmentRepository(Practicom_attendanceContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<string> Login(int guideID)
        {
            try {
                guide guide = await _dbContext.Guides.FirstOrDefaultAsync(g => g.Identity_number == guideID);
                if (guide == null)
                {
                    return notFound;   
                }
                else
                {
                    return found;
                }
            }
            catch(Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<List<Student>> getAllStudent(int guideID)
        {
            try
            {
                guide guide = await _dbContext.Guides.FirstOrDefaultAsync(g => g.Identity_number == guideID);
                practicom_guides practicomType = await _dbContext.practicom_guides.FirstOrDefaultAsync(p => p.Guide_id == guide.Id);
                List<Student>students= await _dbContext.Students.Where(s=>s.PracticomTypeId==practicomType.Practicom_type_id).ToListAsync();
                return students;
            }
            catch (Exception ex)
            {
                throw new Exception("can't get"+ex);
            }
        }

        public async Task<List<CdPracticomType>> getAllProjects(int guideID)
        {
            try
            {
                guide guide = await _dbContext.Guides.FirstOrDefaultAsync(g => g.Identity_number == guideID);
                int[] Practicom_type_id = await _dbContext.practicom_guides.Where(p => p.Guide_id == guide.Id).Select(p => p.Practicom_type_id).ToArrayAsync();
                List<CdPracticomType> cdPracticomType = new List<CdPracticomType>();
                for(int i = 0; i < Practicom_type_id.Length; i++)
                {
                    cdPracticomType.Add(await _dbContext.CdPracticomTypes.FirstOrDefaultAsync(c => c.Id == Practicom_type_id[i]));
                }
                return cdPracticomType;
            }
            catch (Exception ex)
            {
                throw new Exception("can't get");
            }
        }


        public async Task<List<cd_assessment_type_code>> getAllAssessment()
        {
            try
            {
                List<cd_assessment_type_code> assessment_Type_Codes = await _dbContext.cd_assessment_type_code.ToListAsync();
                return assessment_Type_Codes;
            }
            catch (Exception ex)
            {
                throw new Exception("can't get");
            }
        }

        public async Task<string> addAssessments(List<assessments> list)
        {
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    await _dbContext.assessments.AddAsync(list[i]);     
                    await _dbContext.SaveChangesAsync(); 
                }
                return "Added successfully";
            }
            catch (Exception ex)
            {
                throw new Exception("can't get");
            }
        }
  

        public async Task<List<assessments>> getAssessmentByAssessmentType(int id, int assessmentType)
        {
            try
            {
                List<assessments> assessments = await _dbContext.assessments
                    .Where(x => x.Student_id == id && x.Assessment_type_id == assessmentType)
                    .ToListAsync();

                List<assessments> newList = new List<assessments>();
                if (assessments.Count > 15)
                {
                    int startIndex = assessments.Count -15;
                    for (int i = startIndex; i < assessments.Count; i++)
                    {
                        newList.Add(assessments[i]);
                    }
                    return newList;
                }
                return assessments;
            }
            catch (Exception ex)
            {
                throw new Exception("can't get", ex);
            }
        }

    }
}
