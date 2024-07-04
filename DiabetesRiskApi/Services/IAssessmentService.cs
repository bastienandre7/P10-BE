namespace DiabetesRiskApi.Services
{
    public interface IAssessmentService
    {
        Task<RiskResult> AssessRisk(int Id, string token);
    }
}