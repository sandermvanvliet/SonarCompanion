using System;
using System.ComponentModel.Composition;
using System.Timers;
using SonarCompanion_VSIntegration.MessageBus;
using SonarCompanion_VSIntegration.MessageBus.Messages;

namespace SonarCompanion_VSIntegration.Services
{
    [Export]
    public class AutoRefreshService : IHandler<SettingsAvailable>
    {
        private readonly IMessageBus _messageBus;
        private bool _enabled;
        private int _refreshInterval;
        private Timer _timer;

        [ImportingConstructor]
        public AutoRefreshService(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            _messageBus.Subscribe(this);
        }

        public void Handle(SettingsAvailable item)
        {
            var resetTimer = false;

            if (item.Settings.ContainsKey(SonarCompanionSettingKeys.AutoRefreshEnabled))
            {
                var newValue = string.Equals(item.Settings[SonarCompanionSettingKeys.AutoRefreshEnabled], bool.TrueString,
                    StringComparison.InvariantCultureIgnoreCase);

                if (newValue != _enabled)
                {
                    _enabled = newValue;
                    resetTimer = true;
                }
            }

            if (item.Settings.ContainsKey(SonarCompanionSettingKeys.AutoRefreshInterval))
            {
                var configuredValue = item.Settings[SonarCompanionSettingKeys.AutoRefreshInterval];
                if (!string.IsNullOrEmpty(configuredValue))
                {
                    int configuredInterval;
                    if (int.TryParse(configuredValue, out configuredInterval))
                    {
                        var newValue = Math.Max(5, configuredInterval) * 60 * 1000;

                        if (newValue != _refreshInterval)
                        {
                            _refreshInterval = newValue;
                            resetTimer = true;
                        }
                    }
                }
            }

            if (resetTimer)
            {
                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer(_refreshInterval)
                {
                    AutoReset = false,
                    Enabled = _enabled
                };

                _timer.Elapsed += _timer_Elapsed;
            }
            else
            {
                _timer.Stop();

                _timer.Enabled = _enabled;
                _timer.Interval = _refreshInterval;
            }
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _messageBus.Push(new SonarIssuesRequested());

            ResetTimer();
        }
    }
}