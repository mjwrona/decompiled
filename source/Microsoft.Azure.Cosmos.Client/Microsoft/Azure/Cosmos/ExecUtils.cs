// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ExecUtils
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Handlers;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal static class ExecUtils
  {
    internal static Task<T> ProcessResourceOperationAsync<T>(
      CosmosClient client,
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      Action<RequestMessage> requestEnricher,
      Func<ResponseMessage, T> responseCreator,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return ExecUtils.ProcessResourceOperationAsync<T>(client, resourceUri, resourceType, operationType, requestOptions, (ContainerInternal) null, (FeedRange) null, (Stream) null, requestEnricher, responseCreator, trace, cancellationToken);
    }

    internal static Task<T> ProcessResourceOperationAsync<T>(
      CosmosClient client,
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      Func<ResponseMessage, T> responseCreator,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return ExecUtils.ProcessResourceOperationAsync<T>(client, resourceUri, resourceType, operationType, requestOptions, (ContainerInternal) null, (FeedRange) null, (Stream) null, (Action<RequestMessage>) null, responseCreator, trace, cancellationToken);
    }

    internal static Task<T> ProcessResourceOperationAsync<T>(
      CosmosClient client,
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      Stream streamPayload,
      Func<ResponseMessage, T> responseCreator,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return ExecUtils.ProcessResourceOperationAsync<T>(client, resourceUri, resourceType, operationType, requestOptions, (ContainerInternal) null, (FeedRange) null, streamPayload, (Action<RequestMessage>) null, responseCreator, trace, cancellationToken);
    }

    internal static Task<T> ProcessResourceOperationAsync<T>(
      CosmosClient client,
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      FeedRange feedRange,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      Func<ResponseMessage, T> responseCreator,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (client == null)
        throw new ArgumentNullException(nameof (client));
      return ExecUtils.ProcessResourceOperationAsync<T>(client.RequestHandler, resourceUri, resourceType, operationType, requestOptions, cosmosContainerCore, feedRange, streamPayload, requestEnricher, responseCreator, trace, cancellationToken);
    }

    internal static async Task<T> ProcessResourceOperationAsync<T>(
      RequestInvokerHandler requestHandler,
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      RequestOptions requestOptions,
      ContainerInternal cosmosContainerCore,
      FeedRange feedRange,
      Stream streamPayload,
      Action<RequestMessage> requestEnricher,
      Func<ResponseMessage, T> responseCreator,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (requestHandler == null)
        throw new ArgumentException(nameof (requestHandler));
      if (resourceUri == null)
        throw new ArgumentNullException(nameof (resourceUri));
      if (responseCreator == null)
        throw new ArgumentNullException(nameof (responseCreator));
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      return responseCreator(await requestHandler.SendAsync(resourceUri, resourceType, operationType, requestOptions, cosmosContainerCore, feedRange, streamPayload, requestEnricher, trace, cancellationToken));
    }
  }
}
