// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Handlers.TransportHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Handlers
{
  internal class TransportHandler : RequestHandler
  {
    private readonly CosmosClient client;

    public TransportHandler(CosmosClient client) => this.client = client ?? throw new ArgumentNullException(nameof (client));

    public override async Task<ResponseMessage> SendAsync(
      RequestMessage request,
      CancellationToken cancellationToken)
    {
      try
      {
        return await this.ProcessMessageAsync(request, cancellationToken);
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
      catch (OperationCanceledException ex)
      {
        if (cancellationToken.IsCancellationRequested)
        {
          throw;
        }
        else
        {
          string message = ex.Data?[(object) "Message"].ToString();
          Microsoft.Azure.Cosmos.Headers headers = new Microsoft.Azure.Cosmos.Headers();
          headers.ActivityId = System.Diagnostics.Trace.CorrelationManager.ActivityId.ToString();
          Exception exception = (Exception) ex;
          ITrace trace = request.Trace;
          Exception innerException = exception;
          return CosmosExceptionFactory.CreateRequestTimeoutException(message, headers, trace: trace, innerException: innerException).ToCosmosResponseMessage(request);
        }
      }
      catch (AggregateException ex)
      {
        RequestMessage request2 = request;
        ResponseMessage responseMessage = TransportHandler.AggregateExceptionConverter(ex, request2);
        if (responseMessage != null)
          return responseMessage;
        throw;
      }
    }

    internal async Task<ResponseMessage> ProcessMessageAsync(
      RequestMessage request,
      CancellationToken cancellationToken)
    {
      DocumentServiceRequest serviceRequest = request != null ? request.ToDocumentServiceRequest() : throw new ArgumentNullException(nameof (request));
      ClientSideRequestStatisticsTraceDatum clientSideRequestStatisticsTraceDatum = new ClientSideRequestStatisticsTraceDatum(DateTime.UtcNow, request.Trace.Summary);
      serviceRequest.RequestContext.ClientRequestStatistics = (IClientSideRequestStatistics) clientSideRequestStatisticsTraceDatum;
      serviceRequest.Headers["authorization"] = await ((ICosmosAuthorizationTokenProvider) this.client.DocumentClient).GetUserAuthorizationTokenAsync(serviceRequest.ResourceAddress, PathsHelper.GetResourcePath(request.ResourceType), request.Method.ToString(), serviceRequest.Headers, AuthorizationTokenType.PrimaryMasterKey, request.Trace);
      IStoreModel storeProxy = this.client.DocumentClient.GetStoreProxy(serviceRequest);
      ResponseMessage responseMessage;
      using (ITrace processMessageAsyncTrace = request.Trace.StartChild(storeProxy.GetType().FullName + " Transport Request", TraceComponent.Transport, Microsoft.Azure.Cosmos.Tracing.TraceLevel.Info))
      {
        request.Trace = processMessageAsyncTrace;
        processMessageAsyncTrace.AddDatum("Client Side Request Stats", (TraceDatum) clientSideRequestStatisticsTraceDatum);
        DocumentServiceResponse documentServiceResponse = (DocumentServiceResponse) null;
        try
        {
          documentServiceResponse = await storeProxy.ProcessMessageAsync(serviceRequest, cancellationToken);
        }
        catch (DocumentClientException ex)
        {
          HttpStatusCode? statusCode = ex.StatusCode;
          HttpStatusCode httpStatusCode1 = HttpStatusCode.Unauthorized;
          if (!(statusCode.GetValueOrDefault() == httpStatusCode1 & statusCode.HasValue))
          {
            statusCode = ex.StatusCode;
            HttpStatusCode httpStatusCode2 = HttpStatusCode.Forbidden;
            if (!(statusCode.GetValueOrDefault() == httpStatusCode2 & statusCode.HasValue))
              goto label_10;
          }
          processMessageAsyncTrace.AddDatum("AuthProvider LifeSpan InSec", (object) this.client.DocumentClient.cosmosAuthorization.GetAge().TotalSeconds);
label_10:
          throw;
        }
        finally
        {
          processMessageAsyncTrace.Summary.UpdateRegionContacted((TraceDatum) clientSideRequestStatisticsTraceDatum);
        }
        ResponseMessage cosmosResponseMessage = documentServiceResponse.ToCosmosResponseMessage(request, serviceRequest.RequestContext.RequestChargeTracker);
        if (cosmosResponseMessage != null && cosmosResponseMessage.StatusCode == HttpStatusCode.Unauthorized || cosmosResponseMessage != null && cosmosResponseMessage.StatusCode == HttpStatusCode.Forbidden)
          processMessageAsyncTrace.AddDatum("AuthProvider LifeSpan InSec", (object) this.client.DocumentClient.cosmosAuthorization.GetAge().TotalSeconds);
        responseMessage = cosmosResponseMessage;
      }
      serviceRequest = (DocumentServiceRequest) null;
      clientSideRequestStatisticsTraceDatum = (ClientSideRequestStatisticsTraceDatum) null;
      return responseMessage;
    }

    internal static ResponseMessage AggregateExceptionConverter(
      AggregateException aggregateException,
      RequestMessage request)
    {
      AggregateException aggregateException1 = aggregateException.Flatten();
      DocumentClientException documentClientException = (DocumentClientException) aggregateException1.InnerExceptions.FirstOrDefault<Exception>((Func<Exception, bool>) (innerEx => innerEx is DocumentClientException));
      if (documentClientException != null)
        return documentClientException.ToCosmosResponseMessage(request);
      return aggregateException1.InnerExceptions.FirstOrDefault<Exception>((Func<Exception, bool>) (innerEx => innerEx is CosmosException)) is CosmosException cosmosException ? cosmosException.ToCosmosResponseMessage(request) : (ResponseMessage) null;
    }
  }
}
