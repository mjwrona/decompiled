// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.GatewayAccountReader
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.Azure.Cosmos
{
  internal sealed class GatewayAccountReader
  {
    private readonly 
    #nullable disable
    ConnectionPolicy connectionPolicy;
    private readonly AuthorizationTokenProvider cosmosAuthorization;
    private readonly CosmosHttpClient httpClient;
    private readonly Uri serviceEndpoint;
    private readonly CancellationToken cancellationToken;

    public GatewayAccountReader(
      Uri serviceEndpoint,
      AuthorizationTokenProvider cosmosAuthorization,
      ConnectionPolicy connectionPolicy,
      CosmosHttpClient httpClient,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.httpClient = httpClient;
      this.serviceEndpoint = serviceEndpoint;
      this.cosmosAuthorization = cosmosAuthorization ?? throw new ArgumentNullException("AuthorizationTokenProvider");
      this.connectionPolicy = connectionPolicy;
      this.cancellationToken = cancellationToken;
    }

    private async Task<AccountProperties> GetDatabaseAccountAsync(Uri serviceEndpoint)
    {
      INameValueCollection headers = (INameValueCollection) new RequestNameValueCollection();
      await this.cosmosAuthorization.AddAuthorizationHeaderAsync(headers, serviceEndpoint, "GET", AuthorizationTokenType.PrimaryMasterKey);
      AccountProperties databaseAccountAsync;
      using (ITrace trace1 = (ITrace) Microsoft.Azure.Cosmos.Tracing.Trace.GetRootTrace("Account Read", TraceComponent.Transport, Microsoft.Azure.Cosmos.Tracing.TraceLevel.Info))
      {
        IClientSideRequestStatistics stats = (IClientSideRequestStatistics) new ClientSideRequestStatisticsTraceDatum(DateTime.UtcNow, trace1.Summary);
        try
        {
          using (HttpResponseMessage responseMessage = await this.httpClient.GetAsync(serviceEndpoint, headers, ResourceType.DatabaseAccount, HttpTimeoutPolicyControlPlaneRead.Instance, stats, new CancellationToken()))
          {
            using (DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(responseMessage))
              databaseAccountAsync = CosmosResource.FromStream<AccountProperties>(responseAsync);
          }
        }
        catch (ObjectDisposedException ex) when (this.cancellationToken.IsCancellationRequested)
        {
          throw new OperationCanceledException(string.Format("Client is being disposed for {0} at {1}, cancelling further operations.", (object) serviceEndpoint, (object) DateTime.UtcNow));
        }
        catch (OperationCanceledException ex)
        {
          trace1.AddDatum("Client Side Request Stats", (object) stats);
          string message = ex.Data?[(object) "Message"]?.ToString() ?? ex.Message;
          Headers headers1 = new Headers();
          headers1.ActivityId = System.Diagnostics.Trace.CorrelationManager.ActivityId.ToString();
          Exception exception = (Exception) ex;
          ITrace trace2 = trace1;
          Exception innerException = exception;
          throw CosmosExceptionFactory.CreateRequestTimeoutException(message, headers1, trace: trace2, innerException: innerException);
        }
      }
      headers = (INameValueCollection) null;
      return databaseAccountAsync;
    }

    public async Task<AccountProperties> InitializeReaderAsync()
    {
      GatewayAccountReader gatewayAccountReader = this;
      return await GlobalEndpointManager.GetDatabaseAccountFromAnyLocationsAsync(gatewayAccountReader.serviceEndpoint, (IList<string>) gatewayAccountReader.connectionPolicy.PreferredLocations, new Func<Uri, Task<AccountProperties>>(gatewayAccountReader.GetDatabaseAccountAsync), gatewayAccountReader.cancellationToken);
    }
  }
}
