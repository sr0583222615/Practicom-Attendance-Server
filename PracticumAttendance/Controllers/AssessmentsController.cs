using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Service;



namespace PracticomAttendance.Controllers
{
    [Route("assessments/[action]")]
    [ApiController]
    public class AssessmentsController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentsController(IAssessmentService assessmentService)
        {
            this._assessmentService = assessmentService;
        }


        [HttpGet("{guideID}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> login(int guideID)
        {
            var loginResult = _assessmentService.Login(guideID).Result;
            return Ok(new { message = loginResult });
        }


        [HttpGet("{guideID}")]
        public async Task<ActionResult<List<Student>>> getAllStudent(int guideID)
        {
            Task<List<Student>> getAllStudentResult = _assessmentService.getAllStudent(guideID);
            return Ok(new { message = getAllStudentResult });
        }

        [HttpGet("{guideID}")]
        public async Task<ActionResult<List<CdPracticomType>>> getAllProjects(int guideID)
        {
            Task<List<CdPracticomType>> getAllProjectsResult = _assessmentService.getAllProjects(guideID);
            return Ok(new { message = getAllProjectsResult });
        }

        [HttpGet]
        public async Task<ActionResult<List<cd_assessment_type_code>>> getAllAssessment()
        {
            Task<List<cd_assessment_type_code>> getAllAssesmentsResult = _assessmentService.getAllAssessment();
            return Ok(new { message = getAllAssesmentsResult });
        }


        [HttpPost]
        public async Task<ActionResult<string>> addAssessments(List<assessments> list)
        {
            Task<string> getAllAssesments =  _assessmentService.addAssessments(list); 
            return Ok(new { message = getAllAssesments });

        }
        [HttpGet]
      public async Task<ActionResult<List<assessments>>> getAssessmentByAssessmentType(int id, int assessmentType)
        {
            Task<List<assessments>> assessments=  _assessmentService.getAssessmentByAssessmentType(id,assessmentType);    
            return Ok(new { message= assessments });
        }
      


    }
}
