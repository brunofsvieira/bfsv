namespace bfsv.Models
{
    /// <summary>
    /// Database settings.
    /// </summary>
    public class ReavenDBSettings
    {
        public string[] Urls { get; set; }
        public string DatabaseName { get; set; }
        public string CertPath { get; set; }
        public string CertPass { get; set; }
    }
}
