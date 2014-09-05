namespace SonarCompanion_VSIntegration.MessageBus.Messages
{
    public class SolutionLoaded : Message
    {
        public string Name { get; private set; }

        public SolutionLoaded(string name)
        {
            Name = name;
        }
    }
}