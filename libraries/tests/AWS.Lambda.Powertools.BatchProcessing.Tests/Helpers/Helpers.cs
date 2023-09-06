/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.SQSEvents;
using Xunit;
using Xunit.Abstractions;

namespace AWS.Lambda.Powertools.BatchProcessing.Tests.Helpers;

internal static class Helpers
{
    internal static List<SQSEvent.SQSMessage> SqsMessages => new()
    {
        new SQSEvent.SQSMessage
        {
            MessageId = "1",
            Body = "{\"Id\":1,\"Name\":\"product-4\",\"Price\":14}",
            EventSourceArn = "arn:aws:sqs:us-east-2:123456789012:my-queue"
        },
        new SQSEvent.SQSMessage
        {
            MessageId = "2",
            Body = "fail",
            EventSourceArn = "arn:aws:sqs:us-east-2:123456789012:my-queue"
        },
        new SQSEvent.SQSMessage
        {
            MessageId = "3",
            Body = "{\"Id\":3,\"Name\":\"product-4\",\"Price\":14}",
            EventSourceArn = "arn:aws:sqs:us-east-2:123456789012:my-queue"
        },
        new SQSEvent.SQSMessage
        {
            MessageId = "4",
            Body = "{\"Id\":4,\"Name\":\"product-4\",\"Price\":14}",
            EventSourceArn = "arn:aws:sqs:us-east-2:123456789012:my-queue"
        },
        new SQSEvent.SQSMessage
        {
            MessageId = "5",
            Body = "{\"Id\":5,\"Name\":\"product-4\",\"Price\":14}",
            EventSourceArn = "arn:aws:sqs:us-east-2:123456789012:my-queue"
        },
    };
}

public class DisplayNameOrderer : ITestCollectionOrderer
{
    public IEnumerable<ITestCollection> OrderTestCollections(
        IEnumerable<ITestCollection> testCollections) =>
        testCollections.OrderBy(collection => collection.DisplayName);
}