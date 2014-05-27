// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SonarService.cs" company="">
//   
// </copyright>
// <summary>
//   The sonar service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace SonarCompanion.API
{
    /// <summary>
    ///     The sonar service.
    /// </summary>
    public class SonarService
    {
        /// <summary>
        /// The sonar uri.
        /// </summary>
        private readonly Uri sonarUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="SonarService"/> class.
        /// </summary>
        /// <param name="sonarUri">
        /// The sonar uri.
        /// </param>
        public SonarService(Uri sonarUri)
        {
            this.sonarUri = sonarUri;
        }

        public string Url {
            get { return sonarUri.ToString(); }
        }

        /// <summary>
        /// The get issues.
        /// </summary>
        /// <param name="qualifier">
        /// The qualifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<SonarIssue> GetIssues(string qualifier)
        {
            var uri = new Uri(sonarUri, "/api/issues/search");

            var client = new RestClient();

            var restRequest = new RestRequest(uri.ToString());

            restRequest.AddParameter("componentRoots", qualifier);
            restRequest.AddParameter("resolved", "false");

            var data = client.Get<SonarIssueList>(restRequest);

            return data.Data.Issues;
        }

        public List<SonarIssue> GetAllIssues(string qualifier, Action<int> progressCallback = null)
        {
            var uri = new Uri(sonarUri, "/api/issues/search");

            var client = new RestClient();

            var restRequest = new RestRequest(uri.ToString());

            var pageIndex = 1;

            restRequest.AddParameter("componentRoots", qualifier);
            restRequest.AddParameter("resolved", "false");
            restRequest.AddParameter("pageSize", 200);
            restRequest.AddParameter("pageIndex", pageIndex);

            var lastPage = false;
            var issueList = new List<SonarIssue>();

            while (!lastPage)
            {
                restRequest.Parameters.Single(p => p.Name == "pageIndex").Value = pageIndex++;

                var data = client.Get<SonarIssueList>(restRequest);

                issueList.AddRange(data.Data.Issues);

                lastPage = pageIndex > data.Data.Paging.Pages;

                if (progressCallback != null)
                {
                    var percentage = ((double)pageIndex / data.Data.Paging.Pages) * 100;
                    progressCallback((int)percentage);
                }
            }

            return issueList;
        }

        /// <summary>
        ///     The get projects.
        /// </summary>
        /// <param name="sonarUri">
        ///     The sonar uri.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        public List<SonarProject> GetProjects()
        {
            var uri = new Uri(sonarUri, "/api/projects");

            var client = new RestClient();

            var data = client.Get<List<SonarProject>>(new RestRequest(uri.ToString()));

            return data.Data;
        }

        /// <summary>
        ///     The get resources.
        /// </summary>
        /// <param name="sonarUri">
        ///     The sonar uri.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        public List<SonarResource> GetResources()
        {
            var uri = new Uri(sonarUri, "/api/resources");

            var client = new RestClient();

            var data = client.Get<List<SonarResource>>(new RestRequest(uri.ToString()));

            return data.Data;
        }
    }
}