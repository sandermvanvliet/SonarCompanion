using System;
using System.Collections.Generic;
using System.Web.Http;
using SonarCompanion.API;

namespace SonarCompanion.SonarStub.Controllers
{
    public class ResourcesController : ApiController
    {
        [HttpGet]
        public IEnumerable<SonarResource> Index()
        {
            return new SonarResource[]
            {
                new SonarResource { Id  = "123", Key = "TestProject1", Name = "TestProject1", lname = "TestProject1", Lang = "cs", Scope = "PRJ", CreationDate = DateTime.Today, Qualifier = "TRK", Version = "1.0" },
                new SonarResource { Id  = "234", Key = "TestProject2", Name = "TestProject2", lname = "TestProject2", Lang = "cs", Scope = "PRJ", CreationDate = DateTime.Today, Qualifier = "TRK", Version = "1.0" },
                new SonarResource { Id  = "456", Key = "TestProject3", Name = "TestProject3", lname = "TestProject3", Lang = "cs", Scope = "PRJ", CreationDate = DateTime.Today, Qualifier = "TRK", Version = "1.0" }
            };
        }
    }
}