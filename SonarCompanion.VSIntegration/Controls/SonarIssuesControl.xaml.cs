using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EnvDTE;
using SonarCompanion.API;
using SonarCompanion_VSIntegration.Services;

namespace SonarCompanion_VSIntegration.Controls
{
    /// <summary>
    ///     Interaction logic for SonarIssuesControl.xaml
    /// </summary>
    public partial class SonarIssuesControl : UserControl, ISonarOptionsEventSink
    {
        private readonly ISonarIssuesService _sonarIssuesService;
        private readonly ISonarOptionsService _sonarOptionsService;
        private readonly IVisualStudioAutomationService _visualStudioAutomationService;

        private bool _initialized;

        private List<Project> _projectInSolution;
        private SonarOptionsPage _properties;

        public SonarIssuesControl(
            ISonarIssuesService sonarIssuesService,
            ISonarOptionsService sonarOptionsService,
            IVisualStudioAutomationService visualStudioAutomationService)
        {
            _sonarIssuesService = sonarIssuesService;
            _sonarOptionsService = sonarOptionsService;
            _visualStudioAutomationService = visualStudioAutomationService;
            _sonarOptionsService.Subscribe(this);

            InitializeComponent();

            LoadOptions();
        }

        public void ReloadOptions()
        {
            LoadOptions();
        }

        private void LoadOptions()
        {
            _properties = _sonarOptionsService.GetOptions();
        }

        /// <summary>
        ///     The refresh button_ on click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
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
        ///     The projects combo box_ on selection changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ProjectsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
            {
                return;
            }

            var selectedProject = (SonarProject)e.AddedItems[0];

            LoadIssuesForAsync(selectedProject);
        }

        /// <summary>
        ///     The my control_ on loaded.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void HandleOnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
            {
                InitializeProjects();
                _initialized = true;
            }

            LoadOptions();
        }

        private void InitializeProjects()
        {
            var projects = _sonarIssuesService.GetProjects();

            SetSafely(ProjectsComboBox, c =>
            {
                // Reset list of issues (might have changed)
                IssuesGrid.ItemsSource = null;

                c.ItemsSource = projects;
                if (projects != null)
                {
                    c.SelectedItem = projects.SingleOrDefault(p => p.Name == _properties.DefaultProject);
                }
            });
        }

        private void IssuesGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((DataGrid)sender).SelectedItem as IssueListViewItem;

            if (item == null)
            {
                return;
            }

            OpenFileAtLine(item.Project, item.FileName, item.Line);
        }

        private void OpenFileAtLine(string projectName, string fileName, int lineNumber)
        {
            if (_projectInSolution == null || !_projectInSolution.Any())
            {
                _projectInSolution = _visualStudioAutomationService.GetProjectsInSolution();
            }

            var project = _projectInSolution.SingleOrDefault(p => p.Name == projectName);

            if (project == null)
            {
                return;
            }

            var projectPath = Path.GetDirectoryName(project.FileName);

            var path = Path.Combine(projectPath, fileName);

            _visualStudioAutomationService.OpenFileAtLine(path, lineNumber);
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
            IssuesGrid.Visibility = Visibility.Collapsed;
            SetSafely(IssueLoadProgressBar, pb => pb.Value = 0);

            Task.Run(() =>
            {
                var issues = _sonarIssuesService
                    .GetAllIssues(selectedProject.Key, UpdateProgress)
                    .Select(i => new IssueListViewItem(i))
                    .OrderBy(i => i.Project)
                    .ThenBy(i => i.FileName)
                    .ThenBy(i => i.Line)
                    .ToList();

                SetSafely(IssuesGrid, i => i.ItemsSource = issues);

                SetSafely(ProgressIndicator, p => { p.Visibility = Visibility.Collapsed; });
                SetSafely(IssuesGrid, l => { l.Visibility = Visibility.Visible; });
            });
        }

        private void UpdateProgress(int percentage)
        {
            SetSafely(IssueLoadProgressBar, pb => pb.Value = percentage);
        }
    }
}