using System;
using System.Collections.Generic;
using System.Linq;
using SonarCompanion.API;

namespace Rabobank.SonarCompanion_VSIntegration.Services
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
                    Rule = "Tag test message",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 10,
                    Message = "Test message at 10",
                    Rule = "Tag test message",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 20,
                    Message = "Test message at 20",
                    Rule = "Tag test message",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 30,
                    Message = "Test message at 30",
                    Rule = "Tag test message",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 40,
                    Message = "Test message at 40",
                    Rule = "Tag test message",
                    Severity = "Critical"
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 95,
                    Message = "Test message at 95",
                    Rule = "Tag test message",
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