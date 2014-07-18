using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using SonarCompanion_VSIntegration.Factories;
using SonarCompanion_VSIntegration.Services;

namespace SonarCompanion_VSIntegration.Controls
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
            var automationService = componentModel.GetService<IVisualStudioAutomationService>();
            var sonarOptionsService = componentModel.GetService<ISonarOptionsService>();

            base.Content =
                new SonarIssuesControl(sonarIssuesServiceFactory.Create(), sonarOptionsService, automationService);
        }
    }
}