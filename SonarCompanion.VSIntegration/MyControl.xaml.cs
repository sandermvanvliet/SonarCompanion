using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using SonarCompanion.API;
using Constants = EnvDTE.Constants;

namespace Rabobank.SonarCompanion_VSIntegration
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        private readonly DTE _dte;
        private readonly SonarService _sonarService;

        public MyControl(DTE dte)
        {
            _dte = dte;
            InitializeComponent();
            _sonarService = new SonarService(new Uri("http://riskitcodemetrics:9000"));
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void ProjectsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
            {
                return;
            }

            var selectedProject = (SonarProject)e.AddedItems[0];

            var issues = _sonarService.GetAllIssues(selectedProject.Key).Select(i => new IssueListViewItem(i));

            SetSafely(IssuesListView, i => i.ItemsSource = issues);
        }

        private void MyControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            var projects = _sonarService.GetProjects();

            SetSafely(ProjectsComboBox, c => c.ItemsSource = projects);
        }

        private void SetSafely<TControl>(TControl control, Action<TControl> action)
            where TControl : Control
        {
            if (control.Dispatcher.CheckAccess())
            {
                action(control);
            }
            else
            {
                control.Dispatcher.Invoke(action);
            }
        }

        private void IssuesListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListView)sender).SelectedItem as IssueListViewItem;

            if (item == null)
            {
                return;
            }

            var project = _dte.Solution.Projects.OfType<Project>().SingleOrDefault(p => p.Name == item.Project);

            var projectPath = System.IO.Path.GetDirectoryName(project.FileName);

            var fileName = System.IO.Path.Combine(projectPath, item.FileName);

            var window = _dte.ItemOperations.OpenFile(fileName, Constants.vsViewKindAny);

            if (window != null)
            {
                ((EnvDTE.TextSelection)window.Document.Selection).GotoLine(item.Line);
            }
        }
    }

    public class IssueListViewItem
    {
        public IssueListViewItem(SonarIssue issue)
        {
            Issue = issue;

            var parts = issue.Component.Split(':');

            Project = parts[1].Trim();
            FileName = parts[2].Trim().Replace("/", "\\");
            Line = issue.Line;
            Message = issue.Message;
        }

        public string Project { get; private set; }
        public string FileName { get; private set; }
        public int Line { get; private set; }
        public string Message { get; private set; }
        public SonarIssue Issue { get; private set; }
    }
}