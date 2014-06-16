using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Rabobank.SonarCompanion_VSIntegration
{
    [Guid("70bd26a2-c867-4f03-b1a3-b60814283b2b")]
    public class SonarIssuesToolWindow : ToolWindowPane
    {
        public SonarIssuesToolWindow() :
            base(null)
        {
            Caption = Resources.ToolWindowTitle;
            BitmapResourceID = 301;
            BitmapIndex = 1;

            var componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.
        GetGlobalService(typeof(SComponentModel));

            var sonarIssuesServiceFactory = componentModel.GetService<SonarIssuesServiceFactory>();

            base.Content =
                new SonarIssuesControl(Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof (SDTE)) as DTE,
                    sonarIssuesServiceFactory.Create());
        }
    }
}