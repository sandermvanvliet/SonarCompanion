namespace SonarCompanion_VSIntegration.MessageBus
{
    public abstract class Message
    {
    }

    public interface IHandler
    {
    }

    public interface IHandler<in TMessage> : IHandler where TMessage : Message
    {
        void Handle(TMessage item);
    }
}