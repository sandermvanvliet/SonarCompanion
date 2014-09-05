namespace SonarCompanion_VSIntegration.MessageBus.Messages
{
    public interface IHandler
    {
    }

    public interface IHandler<in TMessage> : IHandler where TMessage : Message
    {
        void Handle(TMessage item);
    }
}