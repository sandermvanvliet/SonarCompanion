// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SonarCompanion.API;

namespace SonarCompanion.Console
{
    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void Main(string[] args)
        {
            var sonarUri = new Uri("http://riskitcodemetrics:9000");

            var service = new SonarService(sonarUri);

            var sonarResources = service.GetResources();
            
            var marketRiskResource = sonarResources.Single(r => r.Name == "RiskEngines.LEA2");

            var issues = service.GetAllIssues(marketRiskResource.Key);

            foreach (var issue in issues)
            {
                System.Console.WriteLine("[{0}] {1} - {1}", issue.Component, issue.Severity, issue.Message);
            }

            System.Console.WriteLine("Done. Press enter to exit");
            System.Console.ReadLine();
        }
    }
}