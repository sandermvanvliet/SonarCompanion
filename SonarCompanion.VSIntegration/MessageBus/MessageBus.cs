using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SonarCompanion_VSIntegration.Messagebus
{
    public interface IMessageBus
    {
        void Subscribe(IHandler handler);
        void Unsubscribe(IHandler handler);
        void Subscribe<TMessage>(IHandler<TMessage> handler) where TMessage : Message;
        void Unsubscribe<TMessage>(IHandler<TMessage> handler) where TMessage : Message;
        void Push<TMessage>(TMessage item) where TMessage : Message;
    }

    [Export(typeof(IMessageBus))]
    public class MessageBus : IMessageBus
    {
        private readonly Queue<Message> _queue;
        private readonly List<IHandler> _registrations;
        private Task _notifyTask;

        public MessageBus()
        {
            _registrations = new List<IHandler>();
            _queue = new Queue<Message>();
        }

        public void Subscribe(IHandler handler)
        {
            if (handler
                .GetType()
                .GetInterfaces()
                .Any(i => i.IsAssignableFrom(typeof (IHandler<>))))
            {
                _registrations.Add(handler);
            }
        }

        public void Unsubscribe(IHandler handler)
        {
            if (_registrations.Contains(handler))
            {
                _registrations.Remove(handler);
            }
        }

        public void Subscribe<TMessage>(IHandler<TMessage> handler) where TMessage : Message
        {
            _registrations.Add(handler);
        }

        public void Unsubscribe<TMessage>(IHandler<TMessage> handler) where TMessage : Message
        {
            if (_registrations.Contains(handler))
            {
                _registrations.Remove(handler);
            }
        }

        public void Push<TMessage>(TMessage item) where TMessage : Message
        {
            _queue.Enqueue(item);

            if (_notifyTask == null)
            {
                _notifyTask = (new TaskFactory()).StartNew(NotifyAsync);
            }
        }

        private void NotifyAsync()
        {
            while (_queue.Any())
            {
                var item = _queue.Dequeue();

                var toFind = typeof(IHandler<>).MakeGenericType(item.GetType());
                var methodToInvoke = toFind.GetMethod("Handle");

                var handlers = _registrations
                    .Where(toFind.IsInstanceOfType)
                    .ToArray();

                foreach (var handler in handlers)
                {
                    methodToInvoke.Invoke(handler, new object[] { item });
                }
            }

            _notifyTask = null;
        }
    }
}