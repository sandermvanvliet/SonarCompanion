using System;

namespace SonarCompanion_VSIntegration.MessageBus.Messages
{
    public abstract class Message
    {
        protected Message()
        {
            Timestamp = DateTime.Now;
        }

        public DateTime Timestamp { get; private set; }

        private string _originator;

        public string Originator
        {
            get { return _originator; }
            set
            {
                if (_originator != null)
                {
                    throw new InvalidOperationException("Can only set originator once");
                }
                _originator = value;
            }
        }
    }
}