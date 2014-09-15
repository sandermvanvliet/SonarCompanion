using System.Collections.Generic;
using System.Web.Http;
using SonarCompanion.API;

namespace SonarCompanion.SonarStub.Controllers
{
    public class IssuesController : ApiController
    {
        [HttpGet]
        public SonarIssueList Search(string componentRoots = null, bool? resolved = null, int? pageSize = null, int? pageIndex = null)
        {
            return new SonarIssueList
            {
                Components =
                    new List<SonarComponent>
                    {
                        new SonarComponent {Key = "component1", LongName = "This is component 1", Name = "Component1"}
                    },
                MaxResultsReached = true,
                Paging = new Paging {PageIndex = 1, PageSize = 25, Pages = 1, Total = 1},
                Issues = new List<SonarIssue> {new SonarIssue {Component = "Component1", Message = "Test"}}
            };
        }
    }
}