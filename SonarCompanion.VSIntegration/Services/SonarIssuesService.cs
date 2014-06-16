using System;
using System.Collections.Generic;
using System.Linq;
using SonarCompanion.API;

namespace Rabobank.SonarCompanion_VSIntegration.Services
{
    public class SonarIssuesService : ISonarIssuesService
    {
        private readonly SonarService _sonarService;
        private List<SonarIssue> _sonarIssues;

        public SonarIssuesService(ISonarOptionsService sonarOptionsService)
        {
            var sonarUri = new Uri(sonarOptionsService.GetOptions().SonarUrl);

            _sonarService = new SonarService(sonarUri);
        }

        public SonarIssue GetIssueFor(string fileName, int lineNumber)
        {
            return GetIssuesForFile(fileName)
                .FirstOrDefault(issue => issue.Line == lineNumber);
        }

        public IEnumerable<SonarIssue> GetIssuesForFile(string fileName)
        {
            return
                _sonarIssues.Where(
                    issue => string.Equals(issue.FileName, fileName, StringComparison.InvariantCultureIgnoreCase));
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
    }
}