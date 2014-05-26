using System.Linq;

namespace SonarCompanion.API
{
    public class SonarIssue
    {
        public string Key { get; set; }
        public string Rule { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public string Component { get; set; }
        public int Line { get; set; }

        public string FileName
        {
            get
            {
                var parts = Component.Split(':');

                return System.IO.Path.GetFileName(parts.Last());
            }
        }
    }
}