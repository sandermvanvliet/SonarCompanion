using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SonarCompanion.API;
using Constants = EnvDTE.Constants;
using Task = System.Threading.Tasks.Task;
using Window = EnvDTE.Window;

namespace Rabobank.SonarCompanion_VSIntegration
{
    /// <summary>
    ///     Interaction logic for SonarIssuesControl.xaml
    /// </summary>
    public partial class SonarIssuesControl : UserControl
    {
        /// <summary>
        ///     The _dte.
        /// </summary>
        private readonly DTE _dte;

        private bool _initialized;

        private List<Project> _projectInSolution;
        private OptionPageGrid _properties;

        /// <summary>
        ///     The _sonar service.
        /// </summary>
        private SonarService _sonarService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SonarIssuesControl" /> class.
        /// </summary>
        /// <param name="dte">
        ///     The dte.
        /// </param>
        public SonarIssuesControl(DTE dte)
        {
            _dte = dte;
            InitializeComponent();

            LoadOptions();

            _sonarService = new SonarService(new Uri(_properties.SonarUrl));
        }

        private void LoadOptions()
        {
            Properties properties = _dte.Properties["Sonar Companion", "General"];

            _properties = new OptionPageGrid
            {
                SonarUrl = (string) properties.Item("SonarUrl").Value,
                DefaultProject = (string) properties.Item("DefaultProject").Value
            };
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

            var selectedProject = (SonarProject) e.AddedItems[0];

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

            if (!string.Equals(_properties.SonarUrl, _sonarService.Url))
            {
                _sonarService = new SonarService(new Uri(_properties.SonarUrl));

                InitializeProjects();
            }
        }

        private void InitializeProjects()
        {
            List<SonarProject> projects = _sonarService.GetProjects();

            SetSafely(ProjectsComboBox, c =>
            {
                // Reset list of issues (might have changed)
                IssuesListView.ItemsSource = null;

                c.ItemsSource = projects;
                if (projects != null)
                {
                    c.SelectedItem = projects.SingleOrDefault(p => p.Name == _properties.DefaultProject);
                }
            });
        }

        /// <summary>
        ///     The issues list view_ on mouse double click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
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

            Project project = _projectInSolution.SingleOrDefault(p => p.Name == item.Project);

            string projectPath = Path.GetDirectoryName(project.FileName);

            string fileName = Path.Combine(projectPath, item.FileName);

            Window window = _dte.ItemOperations.OpenFile(fileName, Constants.vsViewKindAny);

            if (window != null)
            {
                ((TextSelection) window.Document.Selection).GotoLine(item.Line);
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
            SetSafely(IssueLoadProgressBar, pb => pb.Value = 0);

            Task.Run(() =>
            {
                IEnumerable<IssueListViewItem> issues = _sonarService
                    .GetAllIssues(selectedProject.Key, UpdateProgress)
                    .Select(i => new IssueListViewItem(i));

                SetSafely(IssuesListView, i => i.ItemsSource = issues);

                SetSafely(ProgressIndicator, p => { p.Visibility = Visibility.Collapsed; });
                SetSafely(IssuesListView, l => { l.Visibility = Visibility.Visible; });
            });
        }

        private void UpdateProgress(int percentage)
        {
            SetSafely(IssueLoadProgressBar, pb => pb.Value = percentage);
        }
    }
}