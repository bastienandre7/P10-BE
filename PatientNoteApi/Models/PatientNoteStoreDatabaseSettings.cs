namespace PatientNoteApi.Models
{
    public class PatientNoteStoreDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string PatientNoteCollectionName { get; set; } = null!;
    }
}
