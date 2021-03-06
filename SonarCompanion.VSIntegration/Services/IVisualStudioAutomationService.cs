﻿using System.Collections.Generic;
using EnvDTE;

namespace SonarCompanion_VSIntegration.Services
{
    public interface IVisualStudioAutomationService
    {
        List<Project> GetProjectsInSolution();
        void OpenFileAtLine(string fileName, int line);
        Properties GetProperties(string category, string page);
        string GetSolutionDirectory();
    }
}