﻿using System;
using System.Linq;
using Mantle.Extensions;
using Mantle.Messaging.Contexts;
using Mantle.Messaging.Interfaces;
using Mantle.Messaging.Messages;

namespace Mantle.Messaging.Subscriptions
{
    public class DefaultSubscription<T> : ISubscription<T>
        where T : class
    {
        public DefaultSubscription(ISubscriptionConfiguration<T> configuration)
        {
            configuration.Require("configuration");
            configuration.Validate();

            Configuration = configuration;
        }

        public ISubscriptionConfiguration<T> Configuration { get; private set; }

        public bool HandleMessage(IMessageContext<MessageEnvelope> messageContext)
        {
            messageContext.Require("messageContext");

            T body = Configuration.Serializer.Deserialize(messageContext.Message.Body);
            var context = new SubscriptionMessageContext<T>(messageContext, body, this);

            if (Configuration.Constraints.Any(c => (c.Match(context) == false)))
                return false;

            if (Configuration.AutoDeadLetter)
            {
                if ((messageContext.DeliveryCount.HasValue) && (Configuration.DeadLetterDeliveryLimit.HasValue) &&
                    (messageContext.DeliveryCount.Value >= Configuration.DeadLetterDeliveryLimit.Value))
                {
                    Configuration.DeadLetterStrategy.HandleDeadLetterMessage(context);
                    return true;
                }
            }

            try
            {
                bool doContinue = ExecutePreFilters(context);

                if (doContinue)
                {
                    Configuration.Subscriber.HandleMessage(context);
                    ExecutePostFilters(context);
                }

                if (Configuration.AutoComplete)
                    context.TryToComplete();
            }
            catch
            {
                if (Configuration.AutoAbandon)
                    context.TryToAbandon();

                throw;
            }

            return true;
        }

        private bool ExecutePreFilters(IMessageContext<T> messageContext) 
        {
            foreach (var filter in Configuration.Filters)
            {
                var doContinue = filter.OnHandlingMessage(messageContext);

                if (doContinue == false)
                    return false;
            }

            return true;
        }

        private bool ExecutePostFilters(IMessageContext<T> messageContext)
        {
            foreach (var filter in Configuration.Filters)
            {
                var doContinue = filter.OnHandledMessage(messageContext);

                if (doContinue == false)
                    return false;
            }

            return true;
        }
    }
}