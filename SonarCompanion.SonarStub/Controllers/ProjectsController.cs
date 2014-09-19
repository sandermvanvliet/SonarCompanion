using System.Collections.Generic;
using System.Web.Http;
using SonarCompanion.API;

namespace SonarCompanion.SonarStub.Controllers
{
    public class ProjectsController : ApiController
    {
        [HttpGet]
        public List<SonarProject> Index()
        {
            return new List<SonarProject>
            {
                new SonarProject { id = "123", k = "TestProject1", nm = "TestProject1", sc = "PRJ", qu = "TRK"},
                new SonarProject { id = "234", k = "TestProject2", nm = "TestProject2", sc = "PRJ", qu = "TRK"},
                new SonarProject { id = "345", k = "TestProject3", nm = "TestProject3", sc = "PRJ", qu = "TRK"},
            };
        }
    }
}