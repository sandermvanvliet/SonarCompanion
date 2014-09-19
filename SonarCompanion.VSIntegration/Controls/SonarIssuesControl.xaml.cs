using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SonarCompanion.API;
using SonarCompanion_VSIntegration.MessageBus;
using SonarCompanion_VSIntegration.MessageBus.Messages;
using SonarCompanion_VSIntegration.ViewModel;

namespace SonarCompanion_VSIntegration.Controls
{
    /// <summary>
    ///     Interaction logic for SonarIssuesControl.xaml
    /// </summary>
    public partial class SonarIssuesControl : UserControl,
        IHandler<SonarProjectsAvailable>,
        IHandler<SonarIssuesAvailable>,
        IHandler<SettingsAvailable>,
        IHandler<SonarIssuesRequested>,
        IHandler<SolutionClosed>
    {
        private readonly IMessageBus _messageBus;

        private string _defaultProjectName;

        public SonarIssuesControl(IMessageBus messageBus)
        {
            _messageBus = messageBus;

            _messageBus.Subscribe(this);

            InitializeComponent();
        }

        public void Handle(SettingsAvailable item)
        {
            if (item.Settings.ContainsKey(SonarCompanionSettingKeys.DefaultProject))
            {
                _defaultProjectName = item.Settings[SonarCompanionSettingKeys.DefaultProject];

                _messageBus.Push(new SonarProjectsRequested());
            }
        }

        public void Handle(SolutionClosed item)
        {
            SetSafely(IssuesGrid, i => i.ItemsSource = null);
            SetSafely(ProjectsComboBox, p => p.ItemsSource = null);
        }

        public void Handle(SonarIssuesAvailable item)
        {
            var issues = item.Issues
                .Select(i => new SonarIssueViewModel(i))
                .OrderByDescending(i => i.Severity)
                .ThenBy(i => i.Project)
                .ThenBy(i => i.FileName)
                .ThenBy(i => i.Line)
                .ToList();

            SetSafely(IssuesGrid, i => i.ItemsSource = issues);

            SetSafely(ProgressIndicator, p => { p.Visibility = Visibility.Collapsed; });
            SetSafely(IssuesGrid, l => { l.Visibility = Visibility.Visible; });
        }

        public void Handle(SonarIssuesRequested item)
        {
            SetSafely(ProgressIndicator, p => p.Visibility = Visibility.Visible);
            SetSafely(IssuesGrid, i => i.Visibility = Visibility.Collapsed);
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
            var item = ((DataGridRow) sender).Item as SonarIssueViewModel;

            if (item == null)
            {
                return;
            }

            _messageBus.Push(new NavigateToSource {Project = item.Project, File = Path.Combine(item.Folder, item.FileName), Line = item.Line});
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
            _messageBus.Push(new SonarIssuesRequested {ProjectKey = selectedProject.Key});
        }

        private void SonarIssuesControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ProjectsComboBox.Items == null || ProjectsComboBox.Items.Count == 0)
            {
                _messageBus.Push(new SonarProjectsRequested());
            }
        }
    }
}