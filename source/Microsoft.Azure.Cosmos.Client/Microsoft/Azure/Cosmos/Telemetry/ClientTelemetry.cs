// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.ClientTelemetry
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Handler;
using Microsoft.Azure.Cosmos.Util;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Rntbd;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  internal class ClientTelemetry : IDisposable
  {
    private const int allowedNumberOfFailures = 3;
    private static readonly Uri endpointUrl = ClientTelemetryOptions.GetClientTelemetryEndpoint();
    private static readonly TimeSpan observingWindow = ClientTelemetryOptions.GetScheduledTimeSpan();
    private readonly ClientTelemetryProperties clientTelemetryInfo;
    private readonly DocumentClient documentClient;
    private readonly CosmosHttpClient httpClient;
    private readonly AuthorizationTokenProvider tokenProvider;
    private readonly DiagnosticsHandlerHelper diagnosticsHelper;
    private readonly CancellationTokenSource cancellationTokenSource;
    private Task telemetryTask;
    private ConcurrentDictionary<OperationInfo, (LongConcurrentHistogram latency, LongConcurrentHistogram requestcharge)> operationInfoMap = new ConcurrentDictionary<OperationInfo, (LongConcurrentHistogram, LongConcurrentHistogram)>();
    private int numberOfFailures;

    public static ClientTelemetry CreateAndStartBackgroundTelemetry(
      string clientId,
      DocumentClient documentClient,
      string userAgent,
      ConnectionMode connectionMode,
      AuthorizationTokenProvider authorizationTokenProvider,
      DiagnosticsHandlerHelper diagnosticsHelper,
      IReadOnlyList<string> preferredRegions)
    {
      DefaultTrace.TraceInformation("Initiating telemetry with background task.");
      ClientTelemetry backgroundTelemetry = new ClientTelemetry(clientId, documentClient, userAgent, connectionMode, authorizationTokenProvider, diagnosticsHelper, preferredRegions);
      backgroundTelemetry.StartObserverTask();
      return backgroundTelemetry;
    }

    private ClientTelemetry(
      string clientId,
      DocumentClient documentClient,
      string userAgent,
      ConnectionMode connectionMode,
      AuthorizationTokenProvider authorizationTokenProvider,
      DiagnosticsHandlerHelper diagnosticsHelper,
      IReadOnlyList<string> preferredRegions)
    {
      this.documentClient = documentClient ?? throw new ArgumentNullException(nameof (documentClient));
      this.diagnosticsHelper = diagnosticsHelper ?? throw new ArgumentNullException(nameof (diagnosticsHelper));
      this.tokenProvider = authorizationTokenProvider ?? throw new ArgumentNullException(nameof (authorizationTokenProvider));
      this.clientTelemetryInfo = new ClientTelemetryProperties(clientId, HashingExtension.ComputeHash(Process.GetCurrentProcess().ProcessName), userAgent, connectionMode, preferredRegions, (int) ClientTelemetry.observingWindow.TotalSeconds);
      this.httpClient = documentClient.httpClient;
      this.cancellationTokenSource = new CancellationTokenSource();
    }

    private void StartObserverTask() => this.telemetryTask = Task.Run(new Func<Task>(this.EnrichAndSendAsync), this.cancellationTokenSource.Token);

    private async Task EnrichAndSendAsync()
    {
      DefaultTrace.TraceInformation("Telemetry Job Started with Observing window : {0}", (object) ClientTelemetry.observingWindow);
      try
      {
        while (!this.cancellationTokenSource.IsCancellationRequested)
        {
          if (this.numberOfFailures == 3)
          {
            this.Dispose();
            break;
          }
          if (string.IsNullOrEmpty(this.clientTelemetryInfo.GlobalDatabaseAccountName))
            this.clientTelemetryInfo.GlobalDatabaseAccountName = (await ClientTelemetryHelper.SetAccountNameAsync(this.documentClient))?.Id;
          Compute machineInfo = VmMetadataApiHandler.GetMachineInfo();
          if (machineInfo != null)
          {
            this.clientTelemetryInfo.ApplicationRegion = machineInfo.Location;
            this.clientTelemetryInfo.HostEnvInfo = ClientTelemetryOptions.GetHostInformation(machineInfo);
          }
          this.clientTelemetryInfo.MachineId = VmMetadataApiHandler.GetMachineId();
          await Task.Delay(ClientTelemetry.observingWindow, this.cancellationTokenSource.Token);
          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            DefaultTrace.TraceInformation("Observer Task Cancelled.");
            break;
          }
          this.RecordSystemUtilization();
          this.clientTelemetryInfo.DateTimeUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
          this.clientTelemetryInfo.OperationInfo = ClientTelemetryHelper.ToListWithMetricsInfo((IDictionary<OperationInfo, (LongConcurrentHistogram, LongConcurrentHistogram)>) Interlocked.Exchange<ConcurrentDictionary<OperationInfo, (LongConcurrentHistogram, LongConcurrentHistogram)>>(ref this.operationInfoMap, new ConcurrentDictionary<OperationInfo, (LongConcurrentHistogram, LongConcurrentHistogram)>()));
          await this.SendAsync();
        }
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError("Exception in EnrichAndSendAsync() : {0}", (object) ex.Message);
      }
      DefaultTrace.TraceInformation("Telemetry Job Stopped.");
    }

    internal void Collect(
      CosmosDiagnostics cosmosDiagnostics,
      HttpStatusCode statusCode,
      long responseSizeInBytes,
      string containerId,
      string databaseId,
      OperationType operationType,
      ResourceType resourceType,
      string consistencyLevel,
      double requestCharge,
      string subStatusCode)
    {
      DefaultTrace.TraceVerbose("Collecting Operation data for Telemetry.");
      if (cosmosDiagnostics == null)
        throw new ArgumentNullException(nameof (cosmosDiagnostics));
      (LongConcurrentHistogram latency, LongConcurrentHistogram requestcharge) = this.operationInfoMap.GetOrAdd(new OperationInfo(ClientTelemetryHelper.GetContactedRegions(cosmosDiagnostics)?.ToString(), new long?(responseSizeInBytes), consistencyLevel, databaseId, containerId, new OperationType?(operationType), new ResourceType?(resourceType), new int?((int) statusCode), subStatusCode), (Func<OperationInfo, (LongConcurrentHistogram, LongConcurrentHistogram)>) (x => (new LongConcurrentHistogram(1L, 36000000000L, 4), new LongConcurrentHistogram(1L, 9999900L, 2))));
      try
      {
        latency.RecordValue(cosmosDiagnostics.GetClientElapsedTime().Ticks);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError("Latency Recording Failed by Telemetry. Exception : {0}", (object) ex.Message);
      }
      long num = (long) (requestCharge * 100.0);
      try
      {
        requestcharge.RecordValue(num);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError("Request Charge Recording Failed by Telemetry. Request Charge Value : {0}  Exception : {1} ", (object) num, (object) ex.Message);
      }
    }

    private void RecordSystemUtilization()
    {
      try
      {
        DefaultTrace.TraceVerbose("Started Recording System Usage for telemetry.");
        SystemUsageHistory telemetrySystemHistory = this.diagnosticsHelper.GetClientTelemetrySystemHistory();
        if (telemetrySystemHistory != null)
          ClientTelemetryHelper.RecordSystemUsage(telemetrySystemHistory, this.clientTelemetryInfo.SystemInfo, this.clientTelemetryInfo.IsDirectConnectionMode);
        else
          DefaultTrace.TraceWarning("System Usage History not available");
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceError("System Usage Recording Error : {0}", (object) ex.Message);
      }
    }

    private async Task SendAsync()
    {
      if (ClientTelemetry.endpointUrl == (Uri) null)
      {
        DefaultTrace.TraceError("Telemetry is enabled but endpoint is not configured");
      }
      else
      {
        try
        {
          DefaultTrace.TraceInformation("Sending Telemetry Data to {0}", (object) ClientTelemetry.endpointUrl.AbsoluteUri);
          string content = JsonConvert.SerializeObject((object) this.clientTelemetryInfo, ClientTelemetryOptions.JsonSerializerSettings);
          using (HttpRequestMessage request = new HttpRequestMessage()
          {
            Method = HttpMethod.Post,
            RequestUri = ClientTelemetry.endpointUrl,
            Content = (HttpContent) new StringContent(content, Encoding.UTF8, "application/json")
          })
          {
            using (HttpResponseMessage httpResponseMessage = await this.httpClient.SendHttpAsync(new Func<ValueTask<HttpRequestMessage>>(CreateRequestMessage), ResourceType.Telemetry, HttpTimeoutPolicyNoRetry.Instance, (IClientSideRequestStatistics) null, this.cancellationTokenSource.Token))
            {
              if (!httpResponseMessage.IsSuccessStatusCode)
              {
                ++this.numberOfFailures;
                DefaultTrace.TraceError("Juno API response not successful. Status Code : {0},  Message : {1}", (object) httpResponseMessage.StatusCode, (object) httpResponseMessage.ReasonPhrase);
              }
              else
              {
                this.numberOfFailures = 0;
                DefaultTrace.TraceInformation("Telemetry data sent successfully.");
              }
            }

            async ValueTask<HttpRequestMessage> CreateRequestMessage()
            {
              INameValueCollection headersCollection = (INameValueCollection) new StoreResponseNameValueCollection();
              await this.tokenProvider.AddAuthorizationHeaderAsync(headersCollection, ClientTelemetry.endpointUrl, "POST", AuthorizationTokenType.PrimaryMasterKey);
              foreach (string allKey in headersCollection.AllKeys())
                request.Headers.Add(allKey, headersCollection[allKey]);
              request.Headers.Add("x-ms-databaseaccount-name", this.clientTelemetryInfo.GlobalDatabaseAccountName);
              string environmentName = ClientTelemetryOptions.GetEnvironmentName();
              if (!string.IsNullOrEmpty(environmentName))
                request.Headers.Add("x-ms-environment-name", environmentName);
              HttpRequestMessage requestMessage = request;
              headersCollection = (INameValueCollection) null;
              return requestMessage;
            }
          }
        }
        catch (Exception ex)
        {
          ++this.numberOfFailures;
          DefaultTrace.TraceError("Exception while sending telemetry data : {0}", (object) ex.Message);
        }
        finally
        {
          this.Reset();
        }
      }
    }

    private void Reset() => this.clientTelemetryInfo.SystemInfo.Clear();

    public void Dispose()
    {
      if (!this.cancellationTokenSource.IsCancellationRequested)
      {
        this.cancellationTokenSource.Cancel();
        this.cancellationTokenSource.Dispose();
      }
      this.telemetryTask = (Task) null;
    }
  }
}
