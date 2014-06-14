using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SonarCompanion.API;

namespace Rabobank.SonarCompanion_VSIntegration
{
    [Export(typeof(ISonarIssuesService))]
    public class SonarIssuesService : ISonarIssuesService
    {
        public SonarIssue GetIssueFor(string fileName, int lineNumber)
        {
            return GetIssuesForFile(fileName)
                .FirstOrDefault(issue => issue.Line == lineNumber);
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
                    Message = "Test message at 10",
                    Rule = "Tag test message",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestProject\\TestClass.cs",
                    Key = "TORO",
                    Line = 20,
                    Message = "Test message at 20",
                    Rule = "Tag test message",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestProject\\TestClass.cs",
                    Key = "TORO",
                    Line = 30,
                    Message = "Test message at 30",
                    Rule = "Tag test message",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestProject\\TestClass.cs",
                    Key = "TORO",
                    Line = 40,
                    Message = "Test message at 40",
                    Rule = "Tag test message",
                    Severity = "Critical"
                }
            };
        }
    }
}