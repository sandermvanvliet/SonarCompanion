using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using SonarCompanion.API;
using SonarCompanion_VSIntegration.Messagebus;
using SonarCompanion_VSIntegration.MessageBus.Messages;

namespace SonarCompanion_VSIntegration.Services
{
    public class SonarIssuesService : ISonarIssuesService, IHandler<SonarProjectsRequested>
    {
        private readonly IMessageBus _messageBus;
        private readonly SonarService _sonarService;
        private List<SonarIssue> _sonarIssues;

        [ImportingConstructor]
        public SonarIssuesService(IMessageBus messageBus)
        {
            _messageBus = messageBus;

            _messageBus.Subscribe(this);

            var sonarUri = new Uri("http://tempuri.org/");

            _sonarService = new SonarService(sonarUri);
        }

        public SonarIssue GetIssueFor(string fileName, int lineNumber)
        {
            return GetIssuesForFile(fileName)
                .FirstOrDefault(issue => issue.Line == lineNumber);
        }

        public IEnumerable<SonarIssue> GetIssuesForFile(string fileName)
        {
            if(_sonarIssues != null)
            {
                return
                _sonarIssues.Where(
                    issue => string.Equals(issue.FileName, fileName, StringComparison.InvariantCultureIgnoreCase));
            }

            return new SonarIssue[0];
        }

        public List<SonarProject> GetProjects()
        {
            return new List<SonarProject>
            {
                new SonarProject { id = "test1", nm = "Test Project 1"}
            };
            //return _sonarService.GetProjects();
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

                _messageBus.Push(new SonarProjectsAvailable {Projects = projects.ToArray()});
            });
        }
    }
}