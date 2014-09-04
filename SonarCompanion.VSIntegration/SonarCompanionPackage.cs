using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using log4net;
using log4net.Config;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SonarCompanion_VSIntegration.Interop;
using SonarCompanion_VSIntegration.MessageBus;
using SonarCompanion_VSIntegration.Services;

namespace SonarCompanion_VSIntegration
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof (SonarIssuesToolWindow))]
    [Guid(GuidList.guidSonarCompanion_VSIntegrationPkgString)]
    [ProvideOptionPage(typeof (SonarOptionsPage), "Sonar Companion", "General", 0, 0, true)]
    public sealed class SonarCompanionPackage : Package
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private SolutionEventsSink _solutionEventsSink;

        private void ShowToolWindow(object sender, EventArgs e)
        {
            var window = FindToolWindow(typeof (SonarIssuesToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }

            var windowFrame = (IVsWindowFrame) window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        protected override void Initialize()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
            Log.Info("Initializing Sonar Companion.");

            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            base.Initialize();

            var componentModel = GetService(typeof (SComponentModel)) as IComponentModel;
            if (componentModel == null)
            {
                throw new InvalidOperationException("ComponentModel is not available");
            }

            var messageBus = componentModel.GetService<IMessageBus>();

            // Compose services that depend on messagebus
            componentModel.GetService<ISonarIssuesService>();
            componentModel.GetService<IVisualStudioAutomationService>();

            _solutionEventsSink = new SolutionEventsSink(messageBus, GetService(typeof (SVsSolution)) as IVsSolution);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;

            if (null != mcs)
            {
                // Create the command for the menu item.
                var menuCommandId = new CommandID(GuidList.guidSonarCompanion_VSIntegrationCmdSet,
                    (int) PkgCmdIDList.cmdidSonarIssues);
                var menuItem = new MenuCommand(ShowToolWindow, menuCommandId);
                mcs.AddCommand(menuItem);

                // Create the command for the tool window
                var toolwndCommandId = new CommandID(GuidList.guidSonarCompanion_VSIntegrationCmdSet,
                    (int) PkgCmdIDList.cmdidSonarIssuesToolWindow);
                var menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandId);
                mcs.AddCommand(menuToolWin);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_solutionEventsSink != null)
            {
                _solutionEventsSink.Dispose();
                _solutionEventsSink = null;
            }
        }

        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject);
        }
    }
}