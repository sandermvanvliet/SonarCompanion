using System;
using System.Collections.Generic;

namespace SonarCompanion.API
{
    public interface ISonarService
    {
        /// <summary>
        /// The get issues.
        /// </summary>
        /// <param name="qualifier">
        ///     The qualifier.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        SonarIssue[] GetIssues(string qualifier);

        SonarIssue[] GetAllIssues(string qualifier, Action<int> progressCallback = null);

        /// <summary>
        ///     The get projects.
        /// </summary>
        /// <param name="sonarUri">
        ///     The sonar uri.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}" />.
        /// </returns>
        List<SonarProject> GetProjects();

        /// <summary>
        ///     The get resources.
        /// </summary>
        /// <param name="sonarUri">
        ///     The sonar uri.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}" />.
        /// </returns>
        List<SonarResource> GetResources();
    }
}