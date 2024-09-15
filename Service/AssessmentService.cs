using DataRepository;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Service
{
    public class AssessmentService:IAssessmentService
    {
        private readonly IAssessmentRepository _assessmentRepository;

        public AssessmentService(IAssessmentRepository assessmentRepository)
        {
            this._assessmentRepository = assessmentRepository;
        }

        public async Task<List<cd_assessment_type_code>> getAllAssessment()
        {
            return await _assessmentRepository.getAllAssessment();
        }

        public async Task<List<CdPracticomType>> getAllProjects(int guideID)
        {
            return await _assessmentRepository.getAllProjects(guideID);
        }

        public async Task<List<Student>> getAllStudent(int guideID)
        {
            return await _assessmentRepository.getAllStudent(guideID);
        }

        public async Task<string> Login(int guideID)
        {
            return await _assessmentRepository.Login(guideID);
        }

        public async Task<string> addAssessments(List<assessments> list)
        {
            return await _assessmentRepository.addAssessments(list);
        }

        public async Task<List<assessments>> getAssessmentByAssessmentType(int id, int assessmentType)
        {
            return await _assessmentRepository.getAssessmentByAssessmentType(id, assessmentType);
        }


    }
}
