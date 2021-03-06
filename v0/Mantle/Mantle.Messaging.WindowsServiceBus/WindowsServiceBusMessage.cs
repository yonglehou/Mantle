﻿using System;
using Microsoft.ServiceBus.Messaging;

namespace Mantle.Messaging.WindowsServiceBus
{
    public class WindowsServiceBusMessage<T> : Message<T>, ICanBeAbandoned, ICanBeCompleted, ICanBeKilled, ICanRenewLock,
        IHaveADeliveryCount
    {
        private readonly BrokeredMessage sbMessage;

        public WindowsServiceBusMessage(T payload, BrokeredMessage sbMessage)
            : base(payload)
        {
            if (sbMessage == null)
                throw new ArgumentNullException("sbMessage");

            this.sbMessage = sbMessage;
        }

        public void Abandon()
        {
            if (sbMessage != null)
            {
                try
                {
                    sbMessage.Abandon();
                }
                finally
                {
                    sbMessage.Dispose();
                }
            }
        }

        public void Complete()
        {
            if (sbMessage != null)
            {
                try
                {
                    sbMessage.Complete();
                }
                finally
                {
                    sbMessage.Dispose();
                }
            }
        }

        public void Kill()
        {
            if (sbMessage != null)
            {
                try
                {
                    sbMessage.DeadLetter();
                }
                finally
                {
                    sbMessage.Dispose();
                }
            }
        }

        public void RenewLock()
        {
            if (sbMessage != null)
                sbMessage.RenewLock();
        }

        public int GetDeliveryCount()
        {
            if (sbMessage != null)
                return sbMessage.DeliveryCount;

            return 0;
        }
    }
}