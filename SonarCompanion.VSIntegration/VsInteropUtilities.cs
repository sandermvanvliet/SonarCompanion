// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VsInteropUtilities.cs" company="">
//   
// </copyright>
// <summary>
//   The vs interop utilities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj80;

namespace SonarCompanion_VSIntegration
{
    /// <summary>
    /// The vs interop utilities.
    /// </summary>
    public class VsInteropUtilities
    {
        /// <summary>
        /// The v s 2010 sql database project.
        /// </summary>
        public static Guid VS2010SqlDatabaseProject = new Guid("{c8d11400-126e-41cd-887f-60bd40844f9e}");

        /// <summary>
        /// Obtain the project filename and project type Guid from the given IVsHierarchy
        /// </summary>
        /// <param name="hierarchy">
        /// The IVsHierarchy to examine
        /// </param>
        public static void DumpProjectNameAndType(IVsHierarchy hierarchy)
        {
            var aggProject = hierarchy as IVsAggregatableProject;

            var projectType = string.Empty;
            aggProject.GetAggregateProjectTypeGuids(out projectType);

            var projectName = GetFileNameFromHierarchy(hierarchy);

            Trace.WriteLine("Project filename: " + projectName + " project type: " + projectType);
        }

        /// <summary>
        /// Obtain the filename of the given IVsHierarchy
        /// </summary>
        /// <param name="hierarchy">
        /// The IVsHierarchy to examine
        /// </param>
        /// <returns>
        /// The filename or String.Empty if the filename could not be obtained
        /// </returns>
        public static string GetFileNameFromHierarchy(IVsHierarchy hierarchy)
        {
            var fileName = string.Empty;

            var itemId = GetItemId(hierarchy);

            hierarchy.GetCanonicalName(itemId, out fileName);

            return fileName;
        }

        /// <summary>
        /// Obtain the Guid that identifies the project type of the given IVsHierarchy
        /// </summary>
        /// <param name="hierarchy">
        /// The IVsHierarchy to examine
        /// </param>
        /// <returns>
        /// The project type Guid or Guid.Empty if the project type could not be obtained
        /// </returns>
        public static Guid GetProjectType(IVsHierarchy hierarchy)
        {
            var projectType = string.Empty;

            var aggProject = hierarchy as IVsAggregatableProject;

            if (aggProject != null)
                aggProject.GetAggregateProjectTypeGuids(out projectType);

            return projectType != string.Empty ? new Guid(projectType) : Guid.Empty;
        }

        /// <summary>
        /// Determine if the project specified by hierarchy is a SQL 2008 Database project
        /// </summary>
        /// <param name="hierarchy">
        /// The IVsHierarchy to examine
        /// </param>
        /// <returns>
        /// true if hierarchy is a database project, otherwise false
        /// </returns>
        public static bool IsSqlDatabaseProject(IVsHierarchy hierarchy)
        {
            // TODO: Include other SQL Database project Guids that can be supported
            return GetProjectType(hierarchy) == VS2010SqlDatabaseProject;
        }

        /// <summary>
        /// Obtain the item id of the IVsHierarchy
        /// </summary>
        /// <param name="hierarchy">
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static uint GetItemId(IVsHierarchy hierarchy)
        {
            object extObject;
            uint itemId = 0;
            IVsHierarchy tempHierarchy;

            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_BrowseObject, out extObject);

            var browseObject = extObject as IVsBrowseObject;
            if (browseObject != null)
                browseObject.GetProjectItem(out tempHierarchy, out itemId);

            return itemId;
        }

        /// <summary>
        /// Obtain the IVsHierarchy specified by the document cookie from the RunningDocumentTable
        /// </summary>
        /// <param name="table">
        /// The IVsRunningDocumentTable to search
        /// </param>
        /// <param name="cookie">
        /// The document cookie
        /// </param>
        /// <returns>
        /// A IVsHierarchy that points to the document
        /// </returns>
        public static IVsHierarchy GetHierarchyFromDocCookie(IVsRunningDocumentTable table, uint cookie)
        {
            uint pgrfRDTFlags;
            uint pdwReadLocks;
            uint pdwEditLocks;
            string pbstrMkDocument;
            IVsHierarchy hierarchy;
            uint pitemid;
            IntPtr ppunkDocData;

            table.GetDocumentInfo(
                cookie,
                out pgrfRDTFlags,
                out pdwReadLocks,
                out pdwEditLocks,
                out pbstrMkDocument,
                out hierarchy,
                out pitemid,
                out ppunkDocData
                );

            return hierarchy;
        }

        /// <summary>
        /// Obtain the EnvDTE.Project of a given IVsHierarchy
        /// </summary>
        /// <param name="hierarchy">
        /// The IVsHierarchy to examine
        /// </param>
        /// <returns>
        /// A EnvDTE.Project instance that is the parent of the IVsHierarchy
        /// </returns>
        public static Project GetEnvDTEProject(IVsHierarchy hierarchy)
        {
            object prjObject = null;

            hierarchy.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out prjObject);

            return (EnvDTE.Project)prjObject;
        }

        /// <summary>
        /// Get the string representation of the given tagVSQueryEditResult
        /// </summary>
        /// <param name="result">
        /// A tagVSQueryEditResult value
        /// </param>
        /// <returns>
        /// The string representation of result or String.Empty
        /// </returns>
        public static string QueryEditResultToString(tagVSQueryEditResult result)
        {
            var retval = string.Empty;

            switch (result)
            {
                case tagVSQueryEditResult.QER_EditNotOK:
                    retval = "Edit has been disallowed";
                    break;
                case tagVSQueryEditResult.QER_EditOK:
                    retval = "Edit is allowed";
                    break;
                default:
                    break;
            }

            return retval;
        }

        /// <summary>
        /// Get the string representation of the given tagVSQueryEditResultFlags
        /// </summary>
        /// <param name="flags">
        /// A tagVSQueryEditResultFlags value
        /// </param>
        /// <returns>
        /// The string representation of flags or String.Empty
        /// </returns>
        public static string QueryEditResultFlagsToString(tagVSQueryEditResultFlags flags)
        {
            var retval = string.Empty;

            switch (flags)
            {
                case tagVSQueryEditResultFlags.QER_MaybeCheckedout:
                    retval = "Files checked-out to edit";
                    break;
                case tagVSQueryEditResultFlags.QER_MaybeChanged:
                    retval = "Files changed on checkout";
                    break;
                case tagVSQueryEditResultFlags.QER_InMemoryEdit:
                    retval = "Safe to edit files in memory";
                    break;
                case tagVSQueryEditResultFlags.QER_InMemoryEditNotAllowed:
                    retval = "Edit denied because in-memory edit not allowed";
                    break;
                case tagVSQueryEditResultFlags.QER_NoisyCheckoutRequired:
                    retval = "Silent mode operation does not permit UI";
                    break;

                // QER_NoisyPromptRequired seems to have a value that is shared
                // by another tagVSQueryEditResultFlag
                // case tagVSQueryEditResultFlags.QER_NoisyPromptRequired:
                // retval = "Silent mode operation does not permit UI";
                // break;
                case tagVSQueryEditResultFlags.QER_CheckoutCanceledOrFailed:
                    retval = "Edit not allowed because checkout failed";
                    break;
                case tagVSQueryEditResultFlags.QER_EditNotPossible:
                    retval = "Edit will never be allowed because of current option settings or external conditions";
                    break;
                case tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc:
                    retval = "Edit not allowed because file is read-only on disk";
                    break;
                case tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc:
                    retval = "Edit not allowed because file is read-only and under source control (probably checked in)";
                    break;
                default:
                    break;
            }

            return retval;
        }

        /// <summary>
        /// The projects in solution.
        /// </summary>
        /// <param name="solution">
        /// The solution.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<IVsProject> ProjectsInSolution(IVsSolution solution)
        {
            IEnumHierarchies enumerator = null;
            var guid = Guid.Empty;
            solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref guid, out enumerator);
            var hierarchy = new IVsHierarchy[1] { null };
            uint fetched = 0;
            for (enumerator.Reset();
                enumerator.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1;
                /*nothing*/)
            {
                yield return (IVsProject)hierarchy[0];
            }
        }

        /// <summary>
        /// The is vs lang project.
        /// </summary>
        /// <param name="hierarchy">
        /// The hierarchy.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsVsLangProject(IVsHierarchy hierarchy)
        {
            return GetEnvDTEProject(hierarchy).Object is VSProject2;
        }

        /// <summary>
        /// The get vs lang project.
        /// </summary>
        /// <param name="projectHierarchy">
        /// The project hierarchy.
        /// </param>
        /// <returns>
        /// The <see cref="VSProject2"/>.
        /// </returns>
        public static VSProject2 GetVsLangProject(IVsProject projectHierarchy)
        {
            return GetEnvDTEProject((IVsHierarchy)projectHierarchy).Object as VSProject2;
        }

        /// <summary>
        /// The get projects in solution.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<string> GetProjectsInSolution()
        {
            var solutionService = Package.GetGlobalService(typeof(IVsSolution)) as IVsSolution;

            if (solutionService != null)
            {
                var projects = ProjectsInSolution(solutionService);
                return projects
                    .Select(p => GetEnvDTEProject((IVsHierarchy)p))
                    .Select(p => p.Name)
                    .ToList();
            }

            return new List<string>();
        }

        /// <summary>
        /// The get files in directory.
        /// </summary>
        /// <param name="projectName">
        /// The project name.
        /// </param>
        /// <param name="relativePath">
        /// The relative path.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static IEnumerable<string> GetFilesInDirectory(
            string projectName,
            string relativePath,
            Func<string, bool> filter)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("projectName");
            }

            var solutionService = Package.GetGlobalService(typeof(IVsSolution)) as IVsSolution;

            if (solutionService == null)
            {
                return new List<string>();
            }

            var projects = ProjectsInSolution(solutionService);
            Project project = projects
                .Select(p => GetEnvDTEProject((IVsHierarchy)p)).FirstOrDefault(p => p.Name == projectName);

            if (project == null)
            {
                return new List<string>();
            }

            var folder = VsInteropUtilities.GetFolderProjectItem(project, relativePath);

            return folder
                .ProjectItems
                .OfType<ProjectItem>()
                .Where(pi => filter(pi.Name))
                .Select(pi => pi.Name)
                .ToList();
        }

        /// <summary>
        /// The get active project.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetActiveProject()
        {
            var dte = Package.GetGlobalService(typeof(SDTE)) as DTE;

            var activeSolutionProjects = dte.ActiveSolutionProjects as Array;

            Project activeProject = null;

            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }

            return activeProject.Name;
        }

        /// <summary>
        /// The get folder project item.
        /// </summary>
        /// <param name="project">
        /// The project.
        /// </param>
        /// <param name="relativePath">
        /// The relative path.
        /// </param>
        /// <returns>
        /// The <see cref="ProjectItem"/>.
        /// </returns>
        private static ProjectItem GetFolderProjectItem(Project project, string relativePath)
        {
            var pathParts = relativePath.Split(
                new [] { Path.DirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries
                );

            // Check if the target folders exist, starting at the root of the project
            var targetFolder = project.ProjectItems.Item(pathParts[0]) as EnvDTE.ProjectItem;

            // Index of ProjectItems starts at 1 not 0
            for (var i = 1; i < pathParts.Length; i++)
            {
                targetFolder = targetFolder.ProjectItems.Item(pathParts[i]);

                if (targetFolder == null)
                {
                    return null;
                }
            }

            return targetFolder;
        }
    }
}