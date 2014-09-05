using System.Collections.Generic;

namespace SonarCompanion_VSIntegration.MessageBus.Messages
{
    public class SettingsAvailable : Message
    {
        public SettingsAvailable(Dictionary<string, string> settings)
        {
            Settings = settings;
        }

        public Dictionary<string, string> Settings { get; private set; }
    }
}