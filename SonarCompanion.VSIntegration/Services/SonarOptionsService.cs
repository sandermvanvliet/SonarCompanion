using System.Collections.Generic;
using System.ComponentModel.Composition;
using EnvDTE;

namespace SonarCompanion_VSIntegration.Services
{
    [Export(typeof (ISonarOptionsService))]
    public class SonarOptionsService : ISonarOptionsService
    {
        private readonly Dictionary<int, ISonarOptionsEventSink> _eventSinks;
        private readonly object _syncRoot = new object();
        private readonly IVisualStudioAutomationService _visualStudioAutomationService;

        [ImportingConstructor]
        public SonarOptionsService(IVisualStudioAutomationService visualStudioAutomationService)
        {
            _visualStudioAutomationService = visualStudioAutomationService;
            _eventSinks = new Dictionary<int, ISonarOptionsEventSink>();
        }

        public SonarOptionsPage GetOptions()
        {
            Properties properties = _visualStudioAutomationService.GetProperties("Sonar Companion", "General");

            return new SonarOptionsPage
            {
                SonarUrl = (string) properties.Item("SonarUrl").Value,
                DefaultProject = (string) properties.Item("DefaultProject").Value
            };
        }

        public int Subscribe(ISonarOptionsEventSink eventSink)
        {
            int cookie;

            lock (_syncRoot)
            {
                cookie = _eventSinks.Count + 1;
                _eventSinks.Add(cookie, eventSink);
            }

            return cookie;
        }

        public void Unsubscribe(int cookie)
        {
            lock (_syncRoot)
            {
                if (!_eventSinks.ContainsKey(cookie))
                {
                    _eventSinks.Remove(cookie);
                }
            }
        }
    }
}