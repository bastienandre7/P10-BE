using static System.Net.WebRequestMethods;
using System.Net.Http;
using System.Net.Http.Json;
using System.IO;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace DiabetesRiskApi.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly List<string> _triggers = new List<string>
        {
        "Hémoglobine A1C", "Microalbumine", "Taille", "Poids",
        "Fumeur", "Fumeuse","Fumer", "Anormal","Anormale", "Cholestérol",
        "Vertiges","Vertige", "Rechute", "Réaction", "Réactions", "Anticorps"
        };

        public AssessmentService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<RiskResult> AssessRisk(int Id, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Token is null or empty");
            }

            token = token.Trim('"'); // Remove extra quotes

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/Patient/Find/{Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var patientResponse = await _httpClient.SendAsync(request);

            if (!patientResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to retrieve patient data");
            }

            var patient = await patientResponse.Content.ReadFromJsonAsync<Patient>();

            request = new HttpRequestMessage(HttpMethod.Get, $"api/PatientNotes/GetByPatId/{Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var notesResponse = await _httpClient.SendAsync(request);

            if (!notesResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Failed to retrieve patient notes");
            }

            var patientNotes = await notesResponse.Content.ReadFromJsonAsync<PatientNotes[]>();



            var lowerCaseTriggers = _triggers.Select(t => t.ToLower()).ToList();

            var uniqueTriggersFound = new HashSet<string>();

            foreach (var note in patientNotes)
            {
                var words = note.Note.Split(new[] { ' ', ',', '.', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    var lowerCaseWord = word.ToLower();
                    if (lowerCaseTriggers.Contains(lowerCaseWord))
                    {
                        uniqueTriggersFound.Add(lowerCaseWord);
                    }
                }
            }

            int triggerCount = uniqueTriggersFound.Count;

            var age = CalculateAge(patient.DateOfBirth);

            if (triggerCount == 0)
            {
                return new RiskResult { RiskLevel = "None" };
            }

            if (triggerCount >= 2 && triggerCount <= 5 && age > 30)
            {
                return new RiskResult { RiskLevel = "Borderline" };
            }

            if ((patient.Gender == "M" && age < 30 && triggerCount == 3) ||
                (patient.Gender == "F" && age < 30 && triggerCount == 4) ||
                (age > 30 && (triggerCount == 6 || triggerCount == 7)))
            {
                return new RiskResult { RiskLevel = "In Danger" };
            }

            if ((patient.Gender == "M" && age < 30 && triggerCount >= 5) ||
                (patient.Gender == "F" && age < 30 && triggerCount >= 7) ||
                (age > 30 && triggerCount >= 8))
            {
                return new RiskResult { RiskLevel = "Early onset" };
            }

            return new RiskResult { RiskLevel = "None" };
        }

        public static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
    public class RiskResult
    {
        public string RiskLevel { get; set; }
    }
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
    public class PatientNotes
    {
        
        public string Id { get; set; } 
        public string patId { get; set; }
        public string Note { get; set; }
    }

}
