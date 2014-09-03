namespace SonarCompanion_VSIntegration.Messagebus.Messages
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