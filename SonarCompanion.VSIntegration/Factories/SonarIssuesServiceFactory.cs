using System.ComponentModel.Composition;
using SonarCompanion_VSIntegration.Services;

namespace SonarCompanion_VSIntegration.Factories
{
    [Export]
    public class SonarIssuesServiceFactory
    {
        public ISonarIssuesService Create()
        {
            return new SonarIssuesService();
        }
    }
}