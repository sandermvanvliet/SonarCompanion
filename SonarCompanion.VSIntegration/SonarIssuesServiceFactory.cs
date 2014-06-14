using System.ComponentModel.Composition;

namespace Rabobank.SonarCompanion_VSIntegration
{
    [Export]
    public class SonarIssuesServiceFactory
    {
        public ISonarIssuesService Create()
        {
#if DEBUG
            return new DummySonarIssuesService();
#endif
        }
    }
}