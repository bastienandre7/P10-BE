using Microsoft.AspNetCore.Mvc;
using PatientWebApi.Context;
using PatientWebApi.Models;
using System.Data.Entity;

namespace PatientWebApi.Services
{
    public class PatientService : IPatientService
    {
        private readonly PatientDbContext _context;

        public PatientService(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreatePatient(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient));
            }

            _context.Add(patient);
            await _context.SaveChangesAsync();
            return "Patient added";
        }

        public async Task<string> UpdatePatient(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                throw new ArgumentException("ID mismatch");
            }
            _context.Update(patient);
            await _context.SaveChangesAsync();
            return "Patient infos updated";
        }

        public async Task<string> DeletePatient(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var patient =  _context.Patients.FirstOrDefault(m => m.Id == id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }

            return "Patient Deleted";
        }

        public async Task<Patient> FindPatientById(int? id)
        {
            if (id == null)
            {
                return null;
            }
            var result = _context.Patients.FirstOrDefault(m => m.Id == id);

            return result;
        }

        public List<Patient> GetAllPatients()
        {
            return _context.Patients.ToList();
        }
    }
}
