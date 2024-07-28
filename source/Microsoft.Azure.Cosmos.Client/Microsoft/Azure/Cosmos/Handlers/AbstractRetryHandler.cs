// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Handlers.AbstractRetryHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Handlers
{
  internal abstract class AbstractRetryHandler : RequestHandler
  {
    internal abstract Task<IDocumentClientRetryPolicy> GetRetryPolicyAsync(RequestMessage request);

    public override async Task<ResponseMessage> SendAsync(
      RequestMessage request,
      CancellationToken cancellationToken)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = await this.GetRetryPolicyAsync(request);
      request.OnBeforeSendRequestActions += new Action<DocumentServiceRequest>(retryPolicyInstance.OnBeforeSendRequest);
      try
      {
        return await AbstractRetryHandler.ExecuteHttpRequestAsync((Func<Task<ResponseMessage>>) (() => base.SendAsync(request, cancellationToken)), (Func<ResponseMessage, CancellationToken, Task<ShouldRetryResult>>) ((cosmosResponseMessage, token) => retryPolicyInstance.ShouldRetryAsync(cosmosResponseMessage, cancellationToken)), (Func<Exception, CancellationToken, Task<ShouldRetryResult>>) ((exception, token) => retryPolicyInstance.ShouldRetryAsync(exception, cancellationToken)), cancellationToken);
      }
      catch (DocumentClientException ex)
      {
        RequestMessage requestMessage = request;
        return ex.ToCosmosResponseMessage(requestMessage);
      }
      catch (CosmosException ex)
      {
        RequestMessage request1 = request;
        return ex.ToCosmosResponseMessage(request1);
      }
      catch (AggregateException ex)
      {
        Exception exception = ex.Flatten().InnerExceptions.FirstOrDefault<Exception>((Func<Exception, bool>) (innerEx => innerEx is DocumentClientException));
        if (exception != null)
          return ((DocumentClientException) exception).ToCosmosResponseMessage(request);
        throw;
      }
      finally
      {
        request.OnBeforeSendRequestActions -= new Action<DocumentServiceRequest>(retryPolicyInstance.OnBeforeSendRequest);
      }
    }

    private static async Task<ResponseMessage> ExecuteHttpRequestAsync(
      Func<Task<ResponseMessage>> callbackMethod,
      Func<ResponseMessage, CancellationToken, Task<ShouldRetryResult>> callShouldRetry,
      Func<Exception, CancellationToken, Task<ShouldRetryResult>> callShouldRetryException,
      CancellationToken cancellationToken)
    {
      while (true)
      {
        cancellationToken.ThrowIfCancellationRequested();
        ShouldRetryResult result;
        try
        {
          ResponseMessage cosmosResponseMessage = await callbackMethod();
          if (cosmosResponseMessage.IsSuccessStatusCode)
            return cosmosResponseMessage;
          result = await callShouldRetry(cosmosResponseMessage, cancellationToken);
          if (!result.ShouldRetry)
            return cosmosResponseMessage;
          cosmosResponseMessage = (ResponseMessage) null;
        }
        catch (HttpRequestException ex)
        {
          result = await callShouldRetryException((Exception) ex, cancellationToken);
          if (!result.ShouldRetry)
          {
            if (ex is Exception source)
              ExceptionDispatchInfo.Capture(source).Throw();
            else
              break;
          }
        }
        if (result.BackoffTime != TimeSpan.Zero)
          await Task.Delay(result.BackoffTime, cancellationToken);
        result = (ShouldRetryResult) null;
      }
      object obj;
      throw obj;
    }
  }
}
