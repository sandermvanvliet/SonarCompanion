using System.Collections.Generic;
using System.ComponentModel.Composition;
using SonarCompanion.API;

namespace Rabobank.SonarCompanion_VSIntegration
{
    [Export(typeof(ISonarIssuesService))]
    public class SonarIssuesService : ISonarIssuesService
    {
        public SonarIssue GetIssueFor(string fileName, int lineNumber)
        {
            if (lineNumber == 10)
            {
                return new SonarIssue
                {
                    Component = "TestProject\\TestClass.cs",
                    Key = "TORO",
                    Line = 10,
                    Message = "Test message",
                    Rule = "Tag test message",
                    Severity = "Critical"
                };
            }

            return null;
        }

        public IEnumerable<SonarIssue> GetIssuesForFile(string fileName)
        {
            return new[]
            {
                new SonarIssue
                {
                    Component = "TestProject\\TestClass.cs",
                    Key = "TORO",
                    Line = 10,
                    Message = "Test message",
                    Rule = "Tag test message",
                    Severity = "Critical"
                }
            };
        }
    }
}