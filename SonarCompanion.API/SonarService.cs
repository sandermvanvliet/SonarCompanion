using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace SonarCompanion.API
{
    public class SonarService : ISonarService
    {
        private readonly Uri _sonarUri;

        public SonarService(Uri sonarUri)
        {
            _sonarUri = sonarUri;
        }

        public string Url
        {
            get { return _sonarUri.ToString(); }
        }

        public SonarIssue[] GetIssues(string qualifier)
        {
            var uri = new Uri(_sonarUri, "/api/issues/search");

            var client = new RestClient();

            var restRequest = new RestRequest(uri.ToString());

            restRequest.AddParameter("componentRoots", qualifier);
            restRequest.AddParameter("resolved", "false");

            var data = client.Get<SonarIssueList>(restRequest);

            return data.Data.Issues.ToArray();
        }

        public SonarIssue[] GetAllIssues(string qualifier, Action<int> progressCallback = null)
        {
            var uri = new Uri(_sonarUri, "/api/issues/search");

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

                issueList.AddRange(data.Data.Issues.Select(i => EnrichIssue(i, data.Data.Rules)));

                lastPage = pageIndex > data.Data.Paging.Pages;

                if (progressCallback != null)
                {
                    var percentage = ((double) pageIndex/data.Data.Paging.Pages)*100;
                    progressCallback((int) percentage);
                }
            }

            return issueList.ToArray();
        }

        private static SonarIssue EnrichIssue(SonarIssue sonarIssue, IEnumerable<SonarRule> rules)
        {
            var matchinRule = rules.SingleOrDefault(r => r.Key == sonarIssue.Rule);

            if (matchinRule != null)
            {
                sonarIssue.SonarRule = matchinRule;
            }

            return sonarIssue;
        }

        public List<SonarProject> GetProjects()
        {
            var uri = new Uri(_sonarUri, "/api/projects");

            var client = new RestClient();

            var data = client.Get<List<SonarProject>>(new RestRequest(uri.ToString()));

            return data.Data;
        }

        public List<SonarResource> GetResources()
        {
            var uri = new Uri(_sonarUri, "/api/resources");

            var client = new RestClient();

            var data = client.Get<List<SonarResource>>(new RestRequest(uri.ToString()));

            return data.Data;
        }
    }
}