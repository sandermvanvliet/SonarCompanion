using System;
using System.Collections.Generic;
using System.Linq;
using SonarCompanion.API;

namespace SonarCompanion_VSIntegration.Services
{
    public class DummySonarIssuesService : ISonarIssuesService
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
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 1,
                    Message = "Test message at 1",
                    Rule = "Rule 1",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 10,
                    Message = "Test message at 10",
                    Rule = "Rule 1",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 20,
                    Message = "Test message at 20",
                    Rule = "Rule 2",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 30,
                    Message = "Test message at 30",
                    Rule = "Rule 2",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 40,
                    Message = "Test message at 40",
                    Rule = "Rule 3",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:AnotherApplication:AnotherClass.cs",
                    Key = "TestApplication",
                    Line = 95,
                    Message = "Test message at 95",
                    Rule = "Rule 4",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:AnotherApplication:AnotherClass.cs",
                    Key = "TestApplication",
                    Line = 65,
                    Message = "Test message at 95",
                    Rule = "Rule 2",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:AnotherApplication:AnotherClass.cs",
                    Key = "TestApplication",
                    Line = 75,
                    Message = "Test message at 95",
                    Rule = "Rule 1",
                    Severity = "Critical"
                },
            };
        }

        public List<SonarProject> GetProjects()
        {
            return new List<SonarProject>
            {
                new SonarProject { id = "1", k = "Test", nm = "Test" }
            };
        }

        public IEnumerable<SonarIssue> GetAllIssues(string key, Action<int> updateProgress)
        {
            return GetIssuesForFile("foo");
        }
    }
}