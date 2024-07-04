using PatientNoteApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace PatientNoteApi.Services
{
    public class PatientNotesService
    {
        private readonly IMongoCollection<PatientNote> _patientNoteCollection;

        public PatientNotesService(
            IOptions<PatientNoteStoreDatabaseSettings> patientNoteStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                patientNoteStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                patientNoteStoreDatabaseSettings.Value.DatabaseName);

            _patientNoteCollection = mongoDatabase.GetCollection<PatientNote>(
                patientNoteStoreDatabaseSettings.Value.PatientNoteCollectionName);
        }

        public async Task<List<PatientNote>> GetAsync() =>
            await _patientNoteCollection.Find(_ => true).ToListAsync();

        public async Task<PatientNote?> GetAsync(string id) =>
            await _patientNoteCollection.Find(x => x.patId == id).FirstOrDefaultAsync();

        public async Task CreateAsync(PatientNote newNote) =>
            await _patientNoteCollection.InsertOneAsync(newNote);

        public async Task UpdateAsync(string id, PatientNote updatedNote) =>
            await _patientNoteCollection.ReplaceOneAsync(x => x.patId == id, updatedNote);

        public async Task RemoveAsync(string id) =>
            await _patientNoteCollection.DeleteOneAsync(x => x.patId == id);

        public async Task<List<PatientNote>> GetNotesByPatIdAsync(string PatId)
        {
            return await _patientNoteCollection.Find(note => note.patId == PatId).ToListAsync();
        }
    }
}
