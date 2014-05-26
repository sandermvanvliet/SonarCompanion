﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SonarCompanion.API;
using Constants = EnvDTE.Constants;

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
            _sonarService = new SonarService(new Uri("http://riskitcodemetrics:9000"));
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

            var issues = _sonarService.GetAllIssues(selectedProject.Key).Select(i => new IssueListViewItem(i));

            SetSafely(IssuesListView, i => i.ItemsSource = issues);
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
        private void MyControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            var projects = _sonarService.GetProjects();

            SetSafely(ProjectsComboBox, c => c.ItemsSource = projects);
        }

        /// <summary>
        /// The set safely.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="TControl">
        /// </typeparam>
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
    }

    /// <summary>
    /// The issue list view item.
    /// </summary>
    public class IssueListViewItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IssueListViewItem"/> class.
        /// </summary>
        /// <param name="issue">
        /// The issue.
        /// </param>
        public IssueListViewItem(SonarIssue issue)
        {
            Issue = issue;

            var parts = issue.Component.Split(':');

            Project = parts[1].Trim();
            FileName = parts[2].Trim().Replace("/", "\\");
            Line = issue.Line;
            Message = issue.Message;
        }

        /// <summary>
        /// Gets the project.
        /// </summary>
        public string Project { get; private set; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the line.
        /// </summary>
        public int Line { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the issue.
        /// </summary>
        public SonarIssue Issue { get; private set; }
    }
}