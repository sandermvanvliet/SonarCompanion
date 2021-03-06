using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SonarCompanion_VSIntegration.MessageBus.Messages;

namespace SonarCompanion_VSIntegration.MessageBus
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
        private readonly CancellationTokenSource _tokenSource;

        public MessageBus()
        {
            _registrations = new List<IHandler>();
            _queue = new Queue<Message>();
            _tokenSource = new CancellationTokenSource();
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
            if (item == null)
            {
                return;
            }

            var methodInfo = new StackTrace(1).GetFrame(0).GetMethod();

            item.Originator = string.Format("{0}: {1}", methodInfo.DeclaringType.Name, methodInfo.Name);

            _queue.Enqueue(item);

            if (item is SolutionClosed && _notifyTask != null)
            {
                _tokenSource.Cancel();
                return;
            }

            if (_notifyTask == null)
            {
                _notifyTask = (new TaskFactory()).StartNew(() => NotifyAsync(_tokenSource.Token), _tokenSource.Token);
            }
        }

        private void NotifyAsync(CancellationToken token)
        {
            while (_queue.Any())
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var item = _queue.Dequeue();

                if (item == null)
                {
                    continue;
                }

                var toFind = typeof(IHandler<>).MakeGenericType(item.GetType());
                var methodToInvoke = toFind.GetMethod("Handle");

                var handlers = _registrations
                    .Where(toFind.IsInstanceOfType)
                    .ToArray();

                foreach (var handler in handlers)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    methodToInvoke.Invoke(handler, new object[] { item });
                }
            }

            _notifyTask = null;
        }
    }
}