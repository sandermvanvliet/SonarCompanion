using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EnvDTE;
using SonarCompanion.API;
using SonarCompanion_VSIntegration.Messagebus;
using SonarCompanion_VSIntegration.MessageBus.Messages;
using SonarCompanion_VSIntegration.Messagebus.Messages;
using SonarCompanion_VSIntegration.Services;

namespace SonarCompanion_VSIntegration.Controls
{
    /// <summary>
    ///     Interaction logic for SonarIssuesControl.xaml
    /// </summary>
    public partial class SonarIssuesControl : UserControl, IHandler<SolutionLoaded>, IHandler<SonarProjectsAvailable>,
        IHandler<SonarIssuesAvailable>
    {
        private readonly IMessageBus _messageBus;
        private readonly ISonarIssuesService _sonarIssuesService;

        private string _defaultProjectName;

        public SonarIssuesControl(
            ISonarIssuesService sonarIssuesService,
            IMessageBus messageBus)
        {
            _sonarIssuesService = sonarIssuesService;
            _messageBus = messageBus;
            
            _messageBus.Subscribe(this);

            InitializeComponent();
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

        private void IssuesGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((DataGrid) sender).SelectedItem as IssueListViewItem;

            if (item == null)
            {
                return;
            }

            _messageBus.Push(new NavigateToSource { Project = item.Project, File = item.FileName, Line = item.Line });
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

            _messageBus.Push(new SonarIssuesRequested { ProjectKey = selectedProject.Key });
        }

        public void Handle(SolutionLoaded item)
        {
            _messageBus.Push(new SonarProjectsRequested());
        }

        public void Handle(SonarProjectsAvailable item)
        {
            SetSafely(ProjectsComboBox, c =>
            {
                // Reset list of issues (might have changed)
                IssuesGrid.ItemsSource = null;

                c.ItemsSource = item.Projects;
                if (item.Projects != null)
                {
                    c.SelectedItem = item.Projects.SingleOrDefault(p => p.Name == _defaultProjectName);
                }
            });
        }

        public void Handle(SonarIssuesAvailable item)
        {
            var issues = item.Issues
                    .Select(i => new IssueListViewItem(i))
                    .OrderByDescending(i => i.Severity)
                    .ThenBy(i => i.Project)
                    .ThenBy(i => i.FileName)
                    .ThenBy(i => i.Line)
                    .ToList();

            SetSafely(IssuesGrid, i => i.ItemsSource = issues);

            SetSafely(ProgressIndicator, p => { p.Visibility = Visibility.Collapsed; });
            SetSafely(IssuesGrid, l => { l.Visibility = Visibility.Visible; });
        }
    }
}