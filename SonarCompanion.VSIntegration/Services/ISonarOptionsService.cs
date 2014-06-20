namespace SonarCompanion_VSIntegration.Services
{
    public interface ISonarOptionsService
    {
        SonarOptionsPage GetOptions();

        int Subscribe(ISonarOptionsEventSink eventSink);

        void Unsubscribe(int cookie);
    }
}