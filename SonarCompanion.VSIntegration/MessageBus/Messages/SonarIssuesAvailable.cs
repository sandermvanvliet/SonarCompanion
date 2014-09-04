using System.Collections.Generic;
using SonarCompanion.API;

namespace SonarCompanion_VSIntegration.Messagebus.Messages
{
    public class SonarIssuesAvailable : Message
    {
        public IEnumerable<SonarIssue> Issues { get; set; }
        public string ProjectKey { get; set; }
    }
}