using System.ComponentModel.Composition;
using SonarCompanion_VSIntegration.Services;

namespace SonarCompanion_VSIntegration.Factories
{
    [Export]
    public class SonarIssuesServiceFactory
    {
        private readonly ISonarOptionsService _sonarOptionsService;

        [ImportingConstructor]
        public SonarIssuesServiceFactory(ISonarOptionsService sonarOptionsService)
        {
            _sonarOptionsService = sonarOptionsService;
        }

        public ISonarIssuesService Create()
        {
            return new SonarIssuesService(_sonarOptionsService);
        }
    }
}