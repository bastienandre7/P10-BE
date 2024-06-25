using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientWebApi.Context;
using PatientWebApi.Models;
using PatientWebApi.Services;
using System.Data.Entity;

namespace PatientWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;

        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (patient == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _patientService.CreatePatient(patient);
            return Ok("Patient added");
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password")] Patient patient)
        {
            if (id != patient.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _patientService.UpdatePatient(id, patient);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("Invalid patient ID");
            }
            var result = await _patientService.DeletePatient(id);
            return Ok(result);
        }

        [HttpGet("Find/{id}")]
        public async Task<IActionResult> Find(int? id)
        {
            var patient = await _patientService.FindPatientById(id);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            var patients = _patientService.GetAllPatients();
            return Ok(patients);
        }
    }
}
