using System;
using System.Collections.Generic;

namespace SonarCompanion.API
{
    public interface ISonarService
    {
        SonarIssue[] GetIssues(string qualifier);

        SonarIssue[] GetAllIssues(string qualifier, Action<int> progressCallback = null);

        List<SonarProject> GetProjects();

        List<SonarResource> GetResources();
    }
}