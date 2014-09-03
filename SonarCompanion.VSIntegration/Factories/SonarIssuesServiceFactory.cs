using System.ComponentModel.Composition;
using SonarCompanion_VSIntegration.Messagebus;
using SonarCompanion_VSIntegration.Services;

namespace SonarCompanion_VSIntegration.Factories
{
    [Export]
    public class SonarIssuesServiceFactory
    {
        private readonly IMessageBus _messageBus;

        [ImportingConstructor]
        public SonarIssuesServiceFactory(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public ISonarIssuesService Create()
        {
            return new SonarIssuesService(_messageBus);
        }
    }
}