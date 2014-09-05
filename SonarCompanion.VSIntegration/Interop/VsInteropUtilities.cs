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

namespace SonarCompanion_VSIntegration.Interop
{
    public class VsInteropUtilities
    {
        public static void DumpProjectNameAndType(IVsHierarchy hierarchy)
        {
            var aggProject = hierarchy as IVsAggregatableProject;
            if (aggProject == null)
            {
                return;
            }

            string projectType;
            aggProject.GetAggregateProjectTypeGuids(out projectType);

            var projectName = GetFileNameFromHierarchy(hierarchy);

            Trace.WriteLine("Project filename: " + projectName + " project type: " + projectType);
        }

        public static string GetFileNameFromHierarchy(IVsHierarchy hierarchy)
        {
            string fileName;

            var itemId = GetItemId(hierarchy);

            hierarchy.GetCanonicalName(itemId, out fileName);

            return fileName;
        }

        public static Guid GetProjectType(IVsHierarchy hierarchy)
        {
            var projectType = string.Empty;

            var aggProject = hierarchy as IVsAggregatableProject;

            if (aggProject != null)
                aggProject.GetAggregateProjectTypeGuids(out projectType);

            return projectType != string.Empty ? new Guid(projectType) : Guid.Empty;
        }

        public static uint GetItemId(IVsHierarchy hierarchy)
        {
            object extObject;
            uint itemId = 0;
            IVsHierarchy tempHierarchy;

            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int) __VSHPROPID.VSHPROPID_BrowseObject, out extObject);

            var browseObject = extObject as IVsBrowseObject;
            if (browseObject != null)
                browseObject.GetProjectItem(out tempHierarchy, out itemId);

            return itemId;
        }

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

        public static Project GetEnvDTEProject(IVsHierarchy hierarchy)
        {
            object prjObject;

            hierarchy.GetProperty((uint) VSConstants.VSITEMID.Root, (int) __VSHPROPID.VSHPROPID_ExtObject, out prjObject);

            return (Project) prjObject;
        }

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
            }

            return retval;
        }

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
            }

            return retval;
        }

        public static IEnumerable<IVsProject> ProjectsInSolution(IVsSolution solution)
        {
            IEnumHierarchies enumerator;
            var guid = Guid.Empty;
            solution.GetProjectEnum((uint) __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref guid, out enumerator);
            var hierarchy = new IVsHierarchy[] {null};
            uint fetched;
            for (enumerator.Reset();
                enumerator.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1;
                /*nothing*/)
            {
                yield return (IVsProject) hierarchy[0];
            }
        }

        public static bool IsVsLangProject(IVsHierarchy hierarchy)
        {
            return GetEnvDTEProject(hierarchy).Object is VSProject2;
        }

        public static VSProject2 GetVsLangProject(IVsProject projectHierarchy)
        {
            return GetEnvDTEProject((IVsHierarchy) projectHierarchy).Object as VSProject2;
        }

        public static IEnumerable<string> GetProjectsInSolution()
        {
            var solutionService = Package.GetGlobalService(typeof (IVsSolution)) as IVsSolution;

            if (solutionService != null)
            {
                var projects = ProjectsInSolution(solutionService);
                return projects
                    .Select(p => GetEnvDTEProject((IVsHierarchy) p))
                    .Select(p => p.Name)
                    .ToList();
            }

            return new List<string>();
        }

        public static IEnumerable<string> GetFilesInDirectory(
            string projectName,
            string relativePath,
            Func<string, bool> filter)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("projectName");
            }

            var solutionService = Package.GetGlobalService(typeof (IVsSolution)) as IVsSolution;

            if (solutionService == null)
            {
                return new List<string>();
            }

            var projects = ProjectsInSolution(solutionService);
            Project project = projects
                .Select(p => GetEnvDTEProject((IVsHierarchy) p)).FirstOrDefault(p => p.Name == projectName);

            if (project == null)
            {
                return new List<string>();
            }

            var folder = GetFolderProjectItem(project, relativePath);

            return folder
                .ProjectItems
                .OfType<ProjectItem>()
                .Where(pi => filter(pi.Name))
                .Select(pi => pi.Name)
                .ToList();
        }

        public static string GetActiveProject()
        {
            var dte = Package.GetGlobalService(typeof (SDTE)) as DTE;
            if (dte == null)
            {
                return null;
            }

            var activeSolutionProjects = dte.ActiveSolutionProjects as Array;

            Project activeProject = null;

            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }

            return activeProject == null ? null : activeProject.Name;
        }

        private static ProjectItem GetFolderProjectItem(Project project, string relativePath)
        {
            var pathParts = relativePath.Split(
                new[] {Path.DirectorySeparatorChar},
                StringSplitOptions.RemoveEmptyEntries
                );

            // Check if the target folders exist, starting at the root of the project
            var targetFolder = project.ProjectItems.Item(pathParts[0]);

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