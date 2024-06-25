using PatientWebApi.Models;

namespace PatientWebApi.Services
{
    public interface IPatientService
    {
        Task<string> CreatePatient(Patient patient);
        Task<string> DeletePatient(int? id);
        Task<Patient> FindPatientById(int? id);
        List<Patient> GetAllPatients();
        Task<string> UpdatePatient(int id, Patient patient);
    }
}