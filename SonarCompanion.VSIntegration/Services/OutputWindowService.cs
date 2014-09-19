using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell.Interop;
using SonarCompanion_VSIntegration.Interop;
using SonarCompanion_VSIntegration.MessageBus;
using SonarCompanion_VSIntegration.MessageBus.Messages;

namespace SonarCompanion_VSIntegration.Services
{
    [Export]
    public class OutputWindowService : IHandler<OutputMessage>, IHandler<SettingsAvailable>
    {
        private IVsOutputWindowPane _outputWindowPane;
        private readonly IMessageBus _messageBus;

        [ImportingConstructor]
        public OutputWindowService(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            messageBus.Subscribe(this);
        }

        public void Handle(OutputMessage item)
        {
            WriteMessage(item.Message);
        }

        private void WriteMessage(string message)
        {
            if (_outputWindowPane == null)
            {
                _outputWindowPane = VsInteropUtilities.GetOutputWindowPane();
            }

            if (_outputWindowPane != null)
            {
                _outputWindowPane.OutputStringThreadSafe(message + Environment.NewLine);
            }
        }

        public void Handle(SettingsAvailable item)
        {
            if (item.Settings.ContainsKey(SonarCompanionSettingKeys.EnableMessageTracing))
            {
                if (bool.Parse(item.Settings[SonarCompanionSettingKeys.EnableMessageTracing]))
                {
                    _messageBus.Subscribe(new DelegateHandler<Message>(m => WriteMessage("Message handled: " + m)));
                }
            }
        }
    }

    internal class DelegateHandler<TMessage> : IHandler<TMessage> where TMessage : Message
    {
        private readonly Action<TMessage> _action;

        public DelegateHandler(Action<TMessage> action)
        {
            _action = action;
        }

        public void Handle(TMessage item)
        {
            _action(item);
        }
    }
}