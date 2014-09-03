using SonarCompanion.API;
using SonarCompanion_VSIntegration.Messagebus;

namespace SonarCompanion_VSIntegration.MessageBus.Messages
{
    public class SonarProjectsAvailable : Message
    {
        public SonarProject[] Projects { get; set; }
    }
}