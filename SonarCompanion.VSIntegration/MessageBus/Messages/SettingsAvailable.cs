using System.Collections.Generic;
using SonarCompanion_VSIntegration.MessageBus;

namespace SonarCompanion_VSIntegration.Services
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