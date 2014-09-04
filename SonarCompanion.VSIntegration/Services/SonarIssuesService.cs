using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using SonarCompanion.API;
using SonarCompanion_VSIntegration.MessageBus;
using SonarCompanion_VSIntegration.MessageBus.Messages;

namespace SonarCompanion_VSIntegration.Services
{
    [Export(typeof(ISonarIssuesService))]
    public class SonarIssuesService : ISonarIssuesService,
        IHandler<SonarProjectsRequested>,
        IHandler<SonarIssuesRequested>,
        IHandler<SettingsAvailable>
    {
        private readonly IMessageBus _messageBus;
        private ISonarService _sonarService;
        private SonarIssue[] _sonarIssues;

        [ImportingConstructor]
        public SonarIssuesService(IMessageBus messageBus)
        {
            _messageBus = messageBus;

            _messageBus.Subscribe(this);

            //var sonarUri = new Uri("http://tempuri.org/");

            _sonarService = new SonarServiceDouble(); // new SonarService(sonarUri);
        }

        public SonarIssue GetIssueFor(string fileName, int lineNumber)
        {
            return GetIssuesForFile(fileName)
                .FirstOrDefault(issue => issue.Line == lineNumber);
        }

        public IEnumerable<SonarIssue> GetIssuesForFile(string fileName)
        {
            if (_sonarIssues != null)
            {
                return
                _sonarIssues.Where(
                    issue => string.Equals(issue.FileName, fileName, StringComparison.InvariantCultureIgnoreCase));
            }

            return new SonarIssue[0];
        }

        public List<SonarProject> GetProjects()
        {
            return _sonarService.GetProjects();
        }

        public IEnumerable<SonarIssue> GetAllIssues(string key, Action<int> updateProgress)
        {
            _sonarIssues = _sonarService.GetAllIssues(key, updateProgress);

            return _sonarIssues;
        }

        public void Handle(SonarProjectsRequested item)
        {
            new TaskFactory().StartNew(() =>
            {
                var projects = GetProjects();

                _messageBus.Push(new SonarProjectsAvailable { Projects = projects.ToArray() });
            });
        }

        public void Handle(SonarIssuesRequested item)
        {
            new TaskFactory().StartNew(() =>
            {
                var issues = GetAllIssues(item.ProjectKey, p => { });

                _messageBus.Push(new SonarIssuesAvailable { ProjectKey = item.ProjectKey, Issues = issues });
            });
        }

        public void Handle(SettingsAvailable item)
        {
            if (item.Settings.ContainsKey(SonarCompanionSettingKeys.SonarUri))
            {
                _sonarService = new SonarService(new Uri(item.Settings[SonarCompanionSettingKeys.SonarUri]));
            }
        }
    }

    public class SonarCompanionSettingKeys
    {
        public const string SonarUri = "sonarUri";
        public const string DefaultProject = "defaultProjectName";
        public const string AutoRefreshEnabled = "autoRefresh.enabled";
        public const string AutoRefreshInterval = "autoRefresh.interval";
    }
}