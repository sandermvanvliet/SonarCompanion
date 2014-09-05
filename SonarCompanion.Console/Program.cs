// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using SonarCompanion.API;

namespace SonarCompanion.Console
{
    /// <summary>
    ///     The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     The main.
        /// </summary>
        private static void Main()
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