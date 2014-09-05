using SonarCompanion.API;

namespace SonarCompanion_VSIntegration.MessageBus.Messages
{
    public class SonarProjectsAvailable : Message
    {
        public SonarProject[] Projects { get; set; }
    }
}