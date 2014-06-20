using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Constants = EnvDTE.Constants;

namespace SonarCompanion_VSIntegration.Services
{
    [Export(typeof (IVisualStudioAutomationService))]
    public class VisualStudioAutomationService : IVisualStudioAutomationService
    {
        private readonly DTE _dte;

        public VisualStudioAutomationService()
        {
            _dte = Package.GetGlobalService(typeof (SDTE)) as DTE;
        }

        public List<Project> GetProjectsInSolution()
        {
            var solutionService = (IVsSolution) Package.GetGlobalService(typeof (IVsSolution));
            return VsInteropUtilities.ProjectsInSolution(solutionService)
                .Select(p => VsInteropUtilities.GetEnvDTEProject((IVsHierarchy) p))
                .ToList();
        }

        public void OpenFileAtLine(string fileName, int line)
        {
            var window = _dte.ItemOperations.OpenFile(fileName, Constants.vsViewKindAny);

            if (window != null)
            {
                ((TextSelection) window.Document.Selection).GotoLine(line);
            }
        }

        public Properties GetProperties(string category, string page)
        {
            return _dte.Properties[category, page];
        }
    }
}