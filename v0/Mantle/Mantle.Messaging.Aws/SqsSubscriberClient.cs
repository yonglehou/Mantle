﻿using System;
using Amazon.SQS.Model;
using Mantle.Aws;

namespace Mantle.Messaging.Aws
{
    public class SqsSubscriberClient : SqsClient, ISubscriberClient
    {
        private readonly SqsSubscriberEndpoint endpoint;

        public SqsSubscriberClient(SqsSubscriberEndpoint endpoint, IAwsConfiguration awsConfiguration)
            : base(awsConfiguration)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            endpoint.Validate();

            this.endpoint = endpoint;
        }

        public Message<T> Receive<T>()
        {
            return Receive<T>(TimeSpan.Zero);
        }

        public Message<T> Receive<T>(TimeSpan timeout)
        {
            try
            {
                var request = new ReceiveMessageRequest
                    {
                        MaxNumberOfMessages = 1,
                        QueueUrl = endpoint.QueueUrl,
                        WaitTimeSeconds = ((int) (timeout.TotalSeconds))
                    };

                ReceiveMessageResponse response = Client.ReceiveMessage(request);

                if ((response.Messages != null) || (response.Messages.Count == 0))
                    return null;

                Message sqsMessage = response.Messages[0];

                T payload;

                try
                {
                    payload = sqsMessage.Body.DeserializeString<T>();
                }
                catch
                {
                    payload = default(T);
                }

                return new SqsMessage<T>(payload, this, sqsMessage.ReceiptHandle);
            }
            catch (Exception ex)
            {
                throw new MessagingException(
                    "An error occurred while attempting to read a message from the specified queue. See inner exception for more details.",
                    ex);
            }
        }

        public void Delete(string receiptHandle)
        {
            if (String.IsNullOrEmpty(receiptHandle))
                throw new ArgumentException("Receipt handle is required.", "receiptHandle");

            Client.DeleteMessage(new DeleteMessageRequest {QueueUrl = endpoint.QueueUrl, ReceiptHandle = receiptHandle});
        }
    }
}