namespace SonarCompanion_VSIntegration.Messagebus.Messages
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