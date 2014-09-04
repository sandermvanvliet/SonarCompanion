using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SonarCompanion_VSIntegration.Messagebus;
using SonarCompanion_VSIntegration.MessageBus.Messages;
using Constants = EnvDTE.Constants;

namespace SonarCompanion_VSIntegration.Services
{
    [Export(typeof (IVisualStudioAutomationService))]
    public class VisualStudioAutomationService : IVisualStudioAutomationService, IHandler<NavigateToSource>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DTE _dte;

        [ImportingConstructor]
        public VisualStudioAutomationService(IMessageBus messageBus)
        {
            _dte = Package.GetGlobalService(typeof (SDTE)) as DTE;

            messageBus.Subscribe(this);
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

        public void Handle(NavigateToSource item)
        {
            var projectInSolution = GetProjectsInSolution();

            var project = projectInSolution.SingleOrDefault(p => p.Name == item.Project);

            if (project == null)
            {
                log.ErrorFormat("Unable to find project: {0}", item.Project);
                return;
            }

            var projectPath = Path.GetDirectoryName(project.FileName);

            if (projectPath == null)
            {
                log.ErrorFormat("Unable to determine path for file: {0}", project.FileName);
                return;
            }

            var path = Path.Combine(projectPath, item.File);

            if (!File.Exists(path))
            {
                log.ErrorFormat("File not found: {0}", path);
            }

            OpenFileAtLine(path, item.Line);
        }
    }
}