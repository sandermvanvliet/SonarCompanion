using System;
using System.Collections.Generic;
using System.Linq;
using SonarCompanion.API;

namespace SonarCompanion_VSIntegration.Services
{
    public class SonarIssuesService : ISonarIssuesService
    {
        private readonly SonarService _sonarService;
        private List<SonarIssue> _sonarIssues;

        public SonarIssuesService()
        {
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
            return _sonarService.GetProjects();
        }

        public IEnumerable<SonarIssue> GetAllIssues(string key, Action<int> updateProgress)
        {
            _sonarIssues = _sonarService.GetAllIssues(key, updateProgress);

            return _sonarIssues;
        }
    }
}