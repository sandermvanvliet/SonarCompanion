using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SonarCompanion.API;
using Constants = EnvDTE.Constants;
using Task = System.Threading.Tasks.Task;

namespace Rabobank.SonarCompanion_VSIntegration
{
    /// <summary>
    ///     Interaction logic for SonarIssuesControl.xaml
    /// </summary>
    public partial class SonarIssuesControl : UserControl
    {
        /// <summary>
        /// The _dte.
        /// </summary>
        private readonly DTE _dte;

        /// <summary>
        /// The _sonar service.
        /// </summary>
        private readonly SonarService _sonarService;

        private List<Project> _projectInSolution;
        private OptionPageGrid _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="SonarIssuesControl"/> class.
        /// </summary>
        /// <param name="dte">
        /// The dte.
        /// </param>
        public SonarIssuesControl(DTE dte)
        {
            _dte = dte;
            InitializeComponent();

            var properties = _dte.Properties["Sonar Companion", "General"];

            _properties = new OptionPageGrid()
            {
                SonarUrl = (string) properties.Item("SonarUrl").Value,
                DefaultProject = (string) properties.Item("DefaultProject").Value
            };

            _sonarService = new SonarService(new Uri(_properties.SonarUrl));
        }

        /// <summary>
        /// The refresh button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ProjectsComboBox.SelectedItem != null)
            {
                var selectedSonarProject = ProjectsComboBox.SelectedItem as SonarProject;

                if (selectedSonarProject != null)
                {
                    LoadIssuesForAsync(selectedSonarProject);
                }
            }
        }

        /// <summary>
        /// The projects combo box_ on selection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ProjectsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
            {
                return;
            }

            var selectedProject = (SonarProject) e.AddedItems[0];

            LoadIssuesForAsync(selectedProject);
        }

        /// <summary>
        /// The my control_ on loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleOnLoaded(object sender, RoutedEventArgs e)
        {
            var projects = _sonarService.GetProjects();

            SetSafely(ProjectsComboBox, c =>
            {
                c.ItemsSource = projects;
                c.SelectedItem = projects.SingleOrDefault(p => p.Name == _properties.DefaultProject);
            });
        }

        /// <summary>
        /// The issues list view_ on mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void IssuesListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListView) sender).SelectedItem as IssueListViewItem;

            if (item == null)
            {
                return;
            }

            if (_projectInSolution == null || !_projectInSolution.Any())
            {
                var solutionService = (IVsSolution) Package.GetGlobalService(typeof (IVsSolution));
                _projectInSolution = VsInteropUtilities.ProjectsInSolution(solutionService)
                    .Select(p => VsInteropUtilities.GetEnvDTEProject((IVsHierarchy) p))
                    .ToList();
            }

            var project = _projectInSolution.SingleOrDefault(p => p.Name == item.Project);

            var projectPath = System.IO.Path.GetDirectoryName(project.FileName);

            var fileName = System.IO.Path.Combine(projectPath, item.FileName);

            var window = _dte.ItemOperations.OpenFile(fileName, Constants.vsViewKindAny);

            if (window != null)
            {
                ((EnvDTE.TextSelection) window.Document.Selection).GotoLine(item.Line);
            }
        }
        private void SetSafely<TControl>(TControl control, Action<TControl> action)
            where TControl : FrameworkElement
        {
            if (control.Dispatcher.CheckAccess())
            {
                action(control);
            }
            else
            {
                control.Dispatcher.Invoke(() => SetSafely(control, action));
            }
        }

        private void LoadIssuesForAsync(SonarProject selectedProject)
        {
            ProgressIndicator.Visibility = Visibility.Visible;
            IssuesListView.Visibility = Visibility.Collapsed;

            Task.Run(() =>
            {
                var issues = _sonarService
                    .GetAllIssues(selectedProject.Key, UpdateProgress)
                    .Select(i => new IssueListViewItem(i));

                SetSafely(IssuesListView, i => i.ItemsSource = issues);

                SetSafely(ProgressIndicator, p =>
                {
                    p.Visibility = Visibility.Collapsed;
                });
                SetSafely(IssuesListView, l =>
                {
                    l.Visibility = Visibility.Visible;
                });
            });
        }

        private void UpdateProgress(int percentage)
        {
            SetSafely(IssueLoadProgressBar, pb => pb.Value = percentage);
        }
    }
}