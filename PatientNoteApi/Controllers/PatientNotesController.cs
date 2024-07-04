using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientNoteApi.Models;
using PatientNoteApi.Services;

namespace PatientNoteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PatientNotesController : Controller
    {
        private readonly PatientNotesService _patientNotesService;

        public PatientNotesController(PatientNotesService patientNotesService) =>
            _patientNotesService = patientNotesService;

        [HttpGet("Index")]
        public async Task<List<PatientNote>> Get() =>
            await _patientNotesService.GetAsync();

        [HttpGet("Get/{id}")]
        public async Task<ActionResult<PatientNote>> Get(string id)
        {
            var patientNote = await _patientNotesService.GetAsync(id);

            if (patientNote is null)
            {
                return NotFound();
            }

            return patientNote;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Post(PatientNote patientNote)
        {
            await _patientNotesService.CreateAsync(patientNote);

            return CreatedAtAction(nameof(Get), new { id = patientNote.patId }, patientNote);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, PatientNote newPatientNote)
        {
            var patientNote = await _patientNotesService.GetAsync(id);

            if (patientNote is null)
            {
                return NotFound();
            }

            newPatientNote.patId = patientNote.patId;

            await _patientNotesService.UpdateAsync(id, newPatientNote);

            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var patientNote = await _patientNotesService.GetAsync(id);

            if (patientNote is null)
            {
                return NotFound();
            }

            await _patientNotesService.RemoveAsync(id);

            return NoContent();
        }

        [HttpGet("GetByPatId/{patId}")]
        public async Task<ActionResult<List<PatientNote>>> GetNotesByPatId(string patId)
        {
            var notes = await _patientNotesService.GetNotesByPatIdAsync(patId);
            if (notes == null || notes.Count == 0)
            {
                return NotFound();
            }
            return notes;
        }
    }
}