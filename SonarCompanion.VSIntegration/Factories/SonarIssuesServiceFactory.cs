using System.ComponentModel.Composition;
using Rabobank.SonarCompanion_VSIntegration.Services;

namespace Rabobank.SonarCompanion_VSIntegration.Factories
{
    [Export]
    public class SonarIssuesServiceFactory
    {
        public ISonarIssuesService Create()
        {
            return new DummySonarIssuesService();
        }
    }
}