using System;
using System.Linq;

namespace SonarCompanion.API
{
    public enum Severity
    {
        Info,
        Minor,
        Major,
        Critical,
        Blocker,
    }

    public enum Status
    {
        Open,
    }

    public class SonarIssue
    {
        public string Key { get; set; }
        public string Rule { get; set; }
        public string Message { get; set; }
        public Severity Severity { get; set; }
        public string Component { get; set; }
        public int Line { get; set; }
        public Status Status { get; set; }
        public SonarIssueTechnicalDebt TechnicalDebt { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public string FileName
        {
            get
            {
                var parts = Component.Split(':');

                return System.IO.Path.GetFileName(parts.Last());
            }
        }

        public SonarRule SonarRule { get; set; }
    }
}