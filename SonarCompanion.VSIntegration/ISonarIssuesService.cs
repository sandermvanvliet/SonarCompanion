using System;
using System.Collections.Generic;
using SonarCompanion.API;

namespace Rabobank.SonarCompanion_VSIntegration
{
    public interface ISonarIssuesService
    {
        SonarIssue GetIssueFor(string fileName, int lineNumber);
        IEnumerable<SonarIssue> GetIssuesForFile(string fileName);
        List<SonarProject> GetProjects();
        IEnumerable<SonarIssue> GetAllIssues(string key, Action<int> updateProgress);
    }
}