using DiabetesRiskApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DiabetesRiskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssessmentController : Controller
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        [HttpGet("Find/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RiskResult>> AssessRisk(int id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _assessmentService.AssessRisk(id, token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception (if logging is configured)
                return StatusCode(500, ex.Message);
            }
        }
    }
}
