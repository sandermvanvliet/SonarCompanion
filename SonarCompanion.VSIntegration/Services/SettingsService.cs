using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SonarCompanion_VSIntegration.MessageBus;
using SonarCompanion_VSIntegration.MessageBus.Messages;

namespace SonarCompanion_VSIntegration.Services
{
    [Export]
    public class SettingsService : IHandler<SettingsRequested>, IHandler<SolutionLoaded>
    {
        private readonly IMessageBus _messageBus;
        private readonly IVisualStudioAutomationService _automationService;
        private readonly Dictionary<string, string> _settings;
        private readonly object _settingsSyncRoot = new object();

        public const string SettingsFileName = "sonarcompanion.properties";

        [ImportingConstructor]
        public SettingsService(IMessageBus messageBus, IVisualStudioAutomationService automationService)
        {
            _messageBus = messageBus;
            _messageBus.Subscribe(this);
            _automationService = automationService;
            _settings = new Dictionary<string, string>();
        }

        public void Handle(SettingsRequested item)
        {
            _messageBus.Push(new SettingsAvailable(_settings));
        }

        public void Handle(SolutionLoaded item)
        {
            Task.Factory.StartNew(ReadSettingsFromSolution);
        }

        private void ReadSettingsFromSolution()
        {
            var solutionDirectory = _automationService.GetSolutionDirectory();

            if (!string.IsNullOrEmpty(solutionDirectory))
            {
                var solutionSettingsPath = Path.Combine(solutionDirectory, SettingsFileName);

                if (File.Exists(solutionSettingsPath))
                {
                    var settings = ReadSettingsFromFile(solutionSettingsPath);

                    lock (_settingsSyncRoot)
                    {
                        _settings.Clear();
                        foreach (var kv in settings)
                        {
                            _settings.Add(kv.Key, kv.Value);
                        }
                    }

                    _messageBus.Push(new SettingsAvailable(_settings));
                }
            }
        }

        private static Dictionary<string, string> ReadSettingsFromFile(string solutionSettingsPath)
        {
            var lines = File.ReadAllLines(solutionSettingsPath);

            return lines
                .Where(line => !line.StartsWith("#") && !string.IsNullOrEmpty(line.Trim()))
                .Select(line => line.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(res => res[0], res => res[1]);
        }
    }
}