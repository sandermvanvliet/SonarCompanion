using System;
using System.Collections.Generic;
using SonarCompanion.API;

namespace SonarCompanion_VSIntegration.Services
{
    public class SonarServiceDouble : ISonarService
    {
        public SonarIssue[] GetIssues(string qualifier)
        {
            return GetAllIssues(qualifier);
        }

        public SonarIssue[] GetAllIssues(string qualifier, Action<int> progressCallback = null)
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
                    Severity = Severity.Critical
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 10,
                    Message = "Test message at 10",
                    Rule = "Rule 1",
                    Severity = Severity.Critical
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 20,
                    Message = "Test message at 20",
                    Rule = "Rule 2",
                    Severity = Severity.Critical
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 30,
                    Message = "Test message at 30",
                    Rule = "Rule 2",
                    Severity = Severity.Critical
                },
                new SonarIssue
                {
                    Component = "TestApplication:TestApplication:TestClass.cs",
                    Key = "TestApplication",
                    Line = 40,
                    Message = "Test message at 40",
                    Rule = "Rule 3",
                    Severity = Severity.Critical
                },
                new SonarIssue
                {
                    Component = "TestApplication:AnotherApplication:AnotherClass.cs",
                    Key = "TestApplication",
                    Line = 95,
                    Message = "Test message at 95",
                    Rule = "Rule 4",
                    Severity = Severity.Critical
                },
                new SonarIssue
                {
                    Component = "TestApplication:AnotherApplication:AnotherClass.cs",
                    Key = "TestApplication",
                    Line = 65,
                    Message = "Test message at 95",
                    Rule = "Rule 2",
                    Severity = Severity.Critical
                },
                new SonarIssue
                {
                    Component = "TestApplication:AnotherApplication:AnotherClass.cs",
                    Key = "TestApplication",
                    Line = 75,
                    Message = "Test message at 95",
                    Rule = "Rule 1",
                    Severity = Severity.Critical
                },
            };
        }

        public List<SonarProject> GetProjects()
        {
            return new List<SonarProject>
            {
                new SonarProject { id = "p1", k = "TestProject1", nm = "Test Project 1"},
                new SonarProject { id = "p2", k = "TestProject2", nm = "Test Project 2"},
                new SonarProject { id = "p3", k = "TestProject3", nm = "Test Project 3"},
            };
        }

        public List<SonarResource> GetResources()
        {
            throw new NotImplementedException();
        }
    }
}