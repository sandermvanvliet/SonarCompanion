namespace SonarCompanion_VSIntegration.MessageBus.Messages
{
    public class SettingChanged : Message
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public SettingChanged(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}