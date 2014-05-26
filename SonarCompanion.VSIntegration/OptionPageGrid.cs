using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Rabobank.SonarCompanion_VSIntegration
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class OptionPageGrid : DialogPage
    {
        private string sonarUrl = "http://localhost:9000/";
        private string defaultProject;

        [Category("Sonar")]
        [DisplayName("Sonar repository URL")]
        [Description("URL to the remote Sonar repository")]
        public string SonarUrl
        {
            get { return sonarUrl; }
            set { sonarUrl = value; }
        }

        [Category("Sonar")]
        [DisplayName("Default project")]
        [Description("Name of project to select when opening the list of issues")]
        public string DefaultProject
        {
            get { return defaultProject; }
            set { defaultProject = value; }
        }
    }
}