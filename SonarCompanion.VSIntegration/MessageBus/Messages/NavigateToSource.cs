namespace SonarCompanion_VSIntegration.MessageBus.Messages
{
    public class NavigateToSource : Message
    {
        public string Project { get; set; }
        public string File { get; set; }
        public int Line { get; set; }
    }
}