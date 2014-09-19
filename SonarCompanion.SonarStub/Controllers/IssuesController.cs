using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SonarCompanion.API;

namespace SonarCompanion.SonarStub.Controllers
{
    public class IssuesController : ApiController
    {
        private readonly List<SonarIssue> _sonarIssues = new List<SonarIssue>
        {
            new SonarIssue {Component = "TestProject1:TestProject1:Classes/Class1.cs", Message = "Test", Line = 1, Key = Guid.NewGuid().ToString()}, 
            new SonarIssue {Component = "TestProject1:TestProject1:Classes/Class1.cs", Message = "Test", Line = 10, Key = Guid.NewGuid().ToString()},
            new SonarIssue {Component = "TestProject1:TestProject1:Classes/Class1.cs", Message = "Test", Line = 12, Key = Guid.NewGuid().ToString() },
        };

        [HttpGet]
        public SonarIssueList Search(string componentRoots = null, bool? resolved = null, int? pageSize = null, int? pageIndex = null)
        {
            return new SonarIssueList
            {
                MaxResultsReached = true,
                Paging = new Paging { PageIndex = 1, PageSize = 25, Pages = 1, Total = 1 },
                Issues = GetIssuesForComponentRoots(componentRoots),
                Components = new List<SonarComponent>
                {
                    new SonarComponent { Key = "TestProject1:TestProject1:Classes/Class1.cs", Qualifier = "FIL", Name = "Class1.cs", LongName = "Classes/Class1.cs" }
                },
                Projects = new List<SonarProject>
                {
                    new SonarProject { id = "123", k = "TestProject1", nm = "TestProject1", qu = "TRK", sc = "PRJ" }
                }
            };
        }

        private List<SonarIssue> GetIssuesForComponentRoots(string componentRoots)
        {
            if (string.IsNullOrEmpty(componentRoots))
            {
                return _sonarIssues;
            }
            
            return _sonarIssues
                .Where(issue => issue.Component.StartsWith(componentRoots))
                .ToList();
        }
    }
}