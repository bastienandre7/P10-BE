using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Security.Claims;
using System.Text.Json;
using System.Security.Principal;
using Blazored.LocalStorage;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Amazon.Runtime;

public class PatientService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _sessionStorage;
    private readonly IAuthService _authService;

    public PatientService(HttpClient httpClient, ILocalStorageService sessionStorage, IAuthService authService)
    {
        _httpClient = httpClient;
        _sessionStorage = sessionStorage;
        _authService = authService;
    }

    public async Task<Patient[]> GetPatientsAsync()
    {
        try
        {
            // Récupération du token
            string token = await _sessionStorage.GetItemAsStringAsync("authToken");

            if (token == null)
            {
                throw new UnauthorizedAccessException("No token found. Please authenticate.");
            }

            // Supprimer les guillemets doubles si présents
            token = token.Trim('"');
            Console.WriteLine($"Token récupéré : {token}");

            // Création du client HTTP
            _httpClient.DefaultRequestHeaders.Clear(); // Clear existing headers to avoid duplication
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            Console.WriteLine($"Authorization header : {_httpClient.DefaultRequestHeaders.Authorization}");


            // Envoi de la requête GET
            var response = await _httpClient.GetAsync("api/Patient/Index");
            Console.WriteLine($"Statut de la réponse : {response.StatusCode}");

            // Vérification du succès de la requête
            response.EnsureSuccessStatusCode();

            // Lecture de la réponse en tant que tableau de patients
            return await response.Content.ReadFromJsonAsync<Patient[]>();
        }
        catch (Exception ex)
        {
            // Gestion des exceptions
            Console.WriteLine($"Erreur : {ex.Message}");
            throw; // Optionnel : re-lancer l'exception pour une gestion plus avancée si nécessaire
        }
    }


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