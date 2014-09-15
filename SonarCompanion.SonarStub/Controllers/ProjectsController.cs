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
                new SonarProject { id = "project1", k = "projectKey1", nm = "Project 1"},
                new SonarProject { id = "project2", k = "projectKey2", nm = "Project 2"}
            };
        }
    }
}