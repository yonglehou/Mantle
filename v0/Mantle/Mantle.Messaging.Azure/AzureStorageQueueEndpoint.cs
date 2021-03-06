﻿using System;
using Mantle.Azure;
using Mantle.Configuration;

namespace Mantle.Messaging.Azure
{
    public abstract class AzureStorageQueueEndpoint : Endpoint, IConfigurable
    {
        protected readonly IAzureStorageConfiguration StorageConfiguration;

        protected AzureStorageQueueEndpoint(IAzureStorageConfiguration storageConfiguration)
        {
            if (storageConfiguration == null)
                throw new ArgumentNullException("storageConfiguration");

            storageConfiguration.Validate();

            StorageConfiguration = storageConfiguration;
        }

        public string QueueName { get; set; }

        public void Configure(IConfigurationMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            Name = metadata.Name;

            if (metadata.Properties.ContainsKey(ConfigurationProperties.QueueName))
                QueueName = metadata.Properties[ConfigurationProperties.QueueName];

            Validate();
        }

        public void Configure(string name, string queueName)
        {
            Name = name;
            QueueName = queueName;

            Validate();
        }

        public override void Validate()
        {
            base.Validate();

            if (String.IsNullOrEmpty(QueueName))
                throw new MessagingException("Azure service bus queue name is required.");
        }

        public static class ConfigurationProperties
        {
            public const string QueueName = "QueueName";
        }
    }
}