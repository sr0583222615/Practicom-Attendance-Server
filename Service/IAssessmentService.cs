using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IAssessmentService
    {
        public Task<string> Login(int guideID);
        public Task<List<Student>> getAllStudent(int guideID);
        public Task<List<CdPracticomType>> getAllProjects(int guideID);
        public Task<List<cd_assessment_type_code>> getAllAssessment();
        public  Task<string> addAssessments(List<assessments> list);
        public Task<List<assessments>> getAssessmentByAssessmentType(int id, int assessmentType);


    }
}
