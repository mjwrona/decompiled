// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StoreClient
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
      ConnectionStateListener connectionStateListener = null,
      bool enableRequestDiagnostics = false,
      bool enableReadRequestsFallback = false,
      bool useMultipleWriteLocations = false,
      bool detectClientConnectivityIssues = false,
      RetryWithConfiguration retryWithConfiguration = null)
    {
      this.transportClient = transportClient;
      this.serviceConfigurationReader = serviceConfigurationReader;
      this.sessionContainer = sessionContainer;
      this.enableRequestDiagnostics = enableRequestDiagnostics;
      this.replicatedResourceClient = new ReplicatedResourceClient(addressResolver, sessionContainer, protocol, this.transportClient, connectionStateListener, this.serviceConfigurationReader, userTokenProvider, enableReadRequestsFallback, useMultipleWriteLocations, detectClientConnectivityIssues, retryWithConfiguration);
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
        Func<Task<StoreResponse>> callbackMethod = (Func<Task<StoreResponse>>) (() => this.replicatedResourceClient.InvokeAsync(request, prepareRequestAsyncDelegate, cancellationToken));
        StoreResponse storeResponse2;
        if (retryPolicy != null)
          storeResponse2 = await BackoffRetryUtility<StoreResponse>.ExecuteAsync(callbackMethod, retryPolicy, cancellationToken);
        else
          storeResponse2 = await callbackMethod();
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

    private DocumentServiceResponse CompleteResponse(
      StoreResponse storeResponse,
      DocumentServiceRequest request)
    {
      if (storeResponse.ResponseHeaderNames.Length != storeResponse.ResponseHeaderValues.Length)
        throw new InternalServerErrorException(RMResources.InvalidBackendResponse);
      INameValueCollection fromStoreResponse = StoreClient.GetHeadersFromStoreResponse(storeResponse);
      this.UpdateResponseHeader(request, fromStoreResponse);
      this.CaptureSessionToken(new HttpStatusCode?((HttpStatusCode) storeResponse.Status), storeResponse.SubStatusCode, request, fromStoreResponse);
      return new DocumentServiceResponse(storeResponse.ResponseBody, fromStoreResponse, (HttpStatusCode) storeResponse.Status, this.enableRequestDiagnostics ? request.RequestContext.ClientRequestStatistics : (IClientSideRequestStatistics) null, request.SerializerSettings ?? this.SerializerSettings);
    }

    private long GetLSN(INameValueCollection headers)
    {
      long result = -1;
      string header = headers["lsn"];
      return !string.IsNullOrEmpty(header) && long.TryParse(header, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? result : -1L;
    }

    private void UpdateResponseHeader(DocumentServiceRequest request, INameValueCollection headers)
    {
      string header1 = request.Headers["x-ms-consistency-level"];
      int num = this.serviceConfigurationReader.DefaultConsistencyLevel == ConsistencyLevel.Session ? 1 : (string.IsNullOrEmpty(header1) ? 0 : (string.Equals(header1, ConsistencyLevel.Session.ToString(), StringComparison.OrdinalIgnoreCase) ? 1 : 0));
      long lsn = this.GetLSN(headers);
      if (lsn == -1L)
        return;
      string header2 = request.Headers["x-ms-version"];
      string str1 = string.IsNullOrEmpty(header2) ? HttpConstants.Versions.CurrentVersion : header2;
      if (string.Compare(str1, HttpConstants.Versions.v2015_12_16, StringComparison.Ordinal) < 0)
      {
        headers["x-ms-session-token"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) lsn);
      }
      else
      {
        string str2 = headers["x-ms-documentdb-partitionkeyrangeid"];
        if (string.IsNullOrEmpty(str2))
        {
          string header3 = request.Headers["x-ms-session-token"];
          str2 = string.IsNullOrEmpty(header3) || header3.IndexOf(":", StringComparison.Ordinal) < 1 ? "0" : header3.Substring(0, header3.IndexOf(":", StringComparison.Ordinal));
        }
        ISessionToken sessionToken = (ISessionToken) null;
        string header4 = headers["x-ms-session-token"];
        if (!string.IsNullOrEmpty(header4))
          sessionToken = SessionTokenHelper.Parse(header4);
        else if (!VersionUtility.IsLaterThan(str1, HttpConstants.Versions.v2018_06_18))
          sessionToken = (ISessionToken) new SimpleSessionToken(lsn);
        if (sessionToken != null)
          headers["x-ms-session-token"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) str2, (object) sessionToken.ConvertToString());
      }
      headers.Remove("x-ms-documentdb-partitionkeyrangeid");
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

    private static INameValueCollection GetHeadersFromStoreResponse(StoreResponse storeResponse)
    {
      INameValueCollection fromStoreResponse = (INameValueCollection) new NameValueCollectionWrapper(storeResponse.ResponseHeaderNames.Length);
      for (int index = 0; index < storeResponse.ResponseHeaderNames.Length; ++index)
        fromStoreResponse.Add(storeResponse.ResponseHeaderNames[index], storeResponse.ResponseHeaderValues[index]);
      return fromStoreResponse;
    }

    internal void AddDisableRntbdChannelCallback(Action action)
    {
      if (!(this.transportClient is Microsoft.Azure.Documents.Rntbd.TransportClient transportClient))
        return;
      transportClient.OnDisableRntbdChannel += action;
    }
  }
}
