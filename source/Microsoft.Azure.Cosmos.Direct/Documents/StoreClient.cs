// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StoreClient
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class StoreClient : IStoreClient
  {
    private readonly ISessionContainer sessionContainer;
    private readonly ReplicatedResourceClient replicatedResourceClient;
    private readonly TransportClient transportClient;
    private readonly IServiceConfigurationReader serviceConfigurationReader;
    private readonly bool enableRequestDiagnostics;

    public StoreClient(
      IAddressResolver addressResolver,
      ISessionContainer sessionContainer,
      IServiceConfigurationReader serviceConfigurationReader,
      IAuthorizationTokenProvider userTokenProvider,
      Protocol protocol,
      TransportClient transportClient,
      bool enableRequestDiagnostics = false,
      bool enableReadRequestsFallback = false,
      bool useMultipleWriteLocations = false,
      bool detectClientConnectivityIssues = false,
      bool disableRetryWithRetryPolicy = false,
      RetryWithConfiguration retryWithConfiguration = null)
    {
      this.transportClient = transportClient;
      this.serviceConfigurationReader = serviceConfigurationReader;
      this.sessionContainer = sessionContainer;
      this.enableRequestDiagnostics = enableRequestDiagnostics;
      this.replicatedResourceClient = new ReplicatedResourceClient(addressResolver, sessionContainer, protocol, this.transportClient, this.serviceConfigurationReader, userTokenProvider, enableReadRequestsFallback, useMultipleWriteLocations, detectClientConnectivityIssues, disableRetryWithRetryPolicy, retryWithConfiguration);
    }

    internal JsonSerializerSettings SerializerSettings { get; set; }

    public string LastReadAddress
    {
      get => this.replicatedResourceClient.LastReadAddress;
      set => this.replicatedResourceClient.LastReadAddress = value;
    }

    public string LastWriteAddress => this.replicatedResourceClient.LastWriteAddress;

    public bool ForceAddressRefresh
    {
      get => this.replicatedResourceClient.ForceAddressRefresh;
      set => this.replicatedResourceClient.ForceAddressRefresh = value;
    }

    public Task<DocumentServiceResponse> ProcessMessageAsync(
      DocumentServiceRequest request,
      IRetryPolicy retryPolicy = null,
      Func<DocumentServiceRequest, Task> prepareRequestAsyncDelegate = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ProcessMessageAsync(request, cancellationToken, retryPolicy, prepareRequestAsyncDelegate);
    }

    public async Task<DocumentServiceResponse> ProcessMessageAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken,
      IRetryPolicy retryPolicy = null,
      Func<DocumentServiceRequest, Task> prepareRequestAsyncDelegate = null)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      await request.EnsureBufferedBodyAsync();
      StoreResponse storeResponse1;
      try
      {
        StoreResponse storeResponse2;
        if (retryPolicy != null)
          storeResponse2 = await BackoffRetryUtility<StoreResponse>.ExecuteAsync((Func<Task<StoreResponse>>) (() => this.replicatedResourceClient.InvokeAsync(request, prepareRequestAsyncDelegate, cancellationToken)), retryPolicy, cancellationToken);
        else
          storeResponse2 = await this.replicatedResourceClient.InvokeAsync(request, prepareRequestAsyncDelegate, cancellationToken);
        storeResponse1 = storeResponse2;
      }
      catch (DocumentClientException ex)
      {
        if (request.RequestContext.ClientRequestStatistics != null)
          ex.RequestStatistics = request.RequestContext.ClientRequestStatistics;
        this.UpdateResponseHeader(request, ex.Headers);
        if (!ReplicatedResourceClient.IsMasterResource(request.ResourceType))
        {
          HttpStatusCode? statusCode = ex.StatusCode;
          HttpStatusCode httpStatusCode1 = HttpStatusCode.PreconditionFailed;
          if (!(statusCode.GetValueOrDefault() == httpStatusCode1 & statusCode.HasValue))
          {
            statusCode = ex.StatusCode;
            HttpStatusCode httpStatusCode2 = HttpStatusCode.Conflict;
            if (!(statusCode.GetValueOrDefault() == httpStatusCode2 & statusCode.HasValue))
            {
              statusCode = ex.StatusCode;
              HttpStatusCode httpStatusCode3 = HttpStatusCode.NotFound;
              if (!(statusCode.GetValueOrDefault() == httpStatusCode3 & statusCode.HasValue) || ex.GetSubStatus() == SubStatusCodes.PartitionKeyRangeGone)
                goto label_14;
            }
          }
          this.CaptureSessionToken(ex.StatusCode, ex.GetSubStatus(), request, ex.Headers);
        }
label_14:
        throw;
      }
      return this.CompleteResponse(storeResponse1, request);
    }

    public async Task OpenConnectionsToAllReplicasAsync(
      string databaseName,
      string containerLinkUri,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      await this.replicatedResourceClient.OpenConnectionsToAllReplicasAsync(databaseName, containerLinkUri, cancellationToken);
    }

    private DocumentServiceResponse CompleteResponse(
      StoreResponse storeResponse,
      DocumentServiceRequest request)
    {
      INameValueCollection fromStoreResponse = StoreClient.GetHeadersFromStoreResponse(storeResponse);
      this.UpdateResponseHeader(request, fromStoreResponse);
      this.CaptureSessionToken(new HttpStatusCode?((HttpStatusCode) storeResponse.Status), storeResponse.SubStatusCode, request, fromStoreResponse);
      return new DocumentServiceResponse(storeResponse.ResponseBody, fromStoreResponse, (HttpStatusCode) storeResponse.Status, this.enableRequestDiagnostics ? request.RequestContext.ClientRequestStatistics : (IClientSideRequestStatistics) null, request.SerializerSettings ?? this.SerializerSettings);
    }

    private long GetLSN(INameValueCollection headers)
    {
      string header = headers["lsn"];
      long result;
      return !string.IsNullOrEmpty(header) && long.TryParse(header, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : -1L;
    }

    private void UpdateResponseHeader(DocumentServiceRequest request, INameValueCollection headers)
    {
      long lsn = this.GetLSN(headers);
      if (lsn == -1L)
        return;
      string header1 = request.Headers["x-ms-version"];
      string str1 = string.IsNullOrEmpty(header1) ? HttpConstants.Versions.CurrentVersion : header1;
      if (string.Compare(str1, HttpConstants.Versions.v2015_12_16, StringComparison.Ordinal) < 0)
      {
        headers["x-ms-session-token"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) lsn);
      }
      else
      {
        string str2 = headers["x-ms-documentdb-partitionkeyrangeid"];
        if (string.IsNullOrEmpty(str2))
        {
          string header2 = request.Headers["x-ms-session-token"];
          str2 = string.IsNullOrEmpty(header2) || header2.IndexOf(":", StringComparison.Ordinal) < 1 ? "0" : header2.Substring(0, header2.IndexOf(":", StringComparison.Ordinal));
        }
        ISessionToken sessionToken = (ISessionToken) null;
        string header3 = headers["x-ms-session-token"];
        if (!string.IsNullOrEmpty(header3))
          sessionToken = SessionTokenHelper.Parse(header3);
        else if (!VersionUtility.IsLaterThan(str1, HttpConstants.VersionDates.v2018_06_18))
          sessionToken = (ISessionToken) new SimpleSessionToken(lsn);
        if (sessionToken == null)
          return;
        headers["x-ms-session-token"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) str2, (object) sessionToken.ConvertToString());
      }
    }

    private void CaptureSessionToken(
      HttpStatusCode? statusCode,
      SubStatusCodes subStatusCode,
      DocumentServiceRequest request,
      INameValueCollection headers)
    {
      if (request.IsValidStatusCodeForExceptionlessRetry((int) statusCode.Value, subStatusCode))
      {
        if (ReplicatedResourceClient.IsMasterResource(request.ResourceType))
          return;
        HttpStatusCode? nullable1 = statusCode;
        HttpStatusCode httpStatusCode1 = HttpStatusCode.PreconditionFailed;
        if (!(nullable1.GetValueOrDefault() == httpStatusCode1 & nullable1.HasValue))
        {
          HttpStatusCode? nullable2 = statusCode;
          HttpStatusCode httpStatusCode2 = HttpStatusCode.Conflict;
          if (!(nullable2.GetValueOrDefault() == httpStatusCode2 & nullable2.HasValue))
          {
            HttpStatusCode? nullable3 = statusCode;
            HttpStatusCode httpStatusCode3 = HttpStatusCode.NotFound;
            if (!(nullable3.GetValueOrDefault() == httpStatusCode3 & nullable3.HasValue) || subStatusCode == SubStatusCodes.PartitionKeyRangeGone)
              return;
          }
        }
      }
      if (request.ResourceType == ResourceType.Collection && request.OperationType == OperationType.Delete)
        this.sessionContainer.ClearTokenByResourceId(!request.IsNameBased ? request.ResourceId : headers["x-ms-content-path"]);
      else
        this.sessionContainer.SetSessionToken(request, headers);
    }

    private static INameValueCollection GetHeadersFromStoreResponse(StoreResponse storeResponse) => storeResponse.Headers;

    internal void AddDisableRntbdChannelCallback(Action action)
    {
      if (!(this.transportClient is Microsoft.Azure.Documents.Rntbd.TransportClient transportClient))
        return;
      transportClient.OnDisableRntbdChannel += action;
    }
  }
}
