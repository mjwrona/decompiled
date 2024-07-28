// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.HttpTransportClient
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class HttpTransportClient : TransportClient
  {
    private readonly HttpClient httpClient;
    private readonly ICommunicationEventSource eventSource;
    public const string Match = "Match";

    public HttpTransportClient(
      int requestTimeout,
      ICommunicationEventSource eventSource,
      UserAgentContainer userAgent = null,
      int idleTimeoutInSeconds = -1,
      HttpMessageHandler messageHandler = null)
    {
      this.httpClient = messageHandler == null ? new HttpClient() : new HttpClient(messageHandler);
      this.httpClient.Timeout = TimeSpan.FromSeconds((double) requestTimeout);
      this.httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
      {
        NoCache = true
      };
      this.httpClient.DefaultRequestHeaders.Add("x-ms-version", HttpConstants.Versions.CurrentVersion);
      if (userAgent == null)
        userAgent = new UserAgentContainer();
      this.httpClient.AddUserAgentHeader(userAgent);
      this.httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
      this.eventSource = eventSource;
    }

    public override void Dispose()
    {
      base.Dispose();
      if (this.httpClient == null)
        return;
      this.httpClient.Dispose();
    }

    private void BeforeRequest(
      Guid activityId,
      Uri uri,
      ResourceType resourceType,
      HttpRequestHeaders requestHeaders)
    {
      this.eventSource.Request(activityId, Guid.Empty, uri.ToString(), resourceType.ToResourceTypeString(), requestHeaders);
    }

    private void AfterRequest(
      Guid activityId,
      HttpStatusCode statusCode,
      double durationInMilliSeconds,
      HttpResponseHeaders responseHeaders)
    {
      this.eventSource.Response(activityId, Guid.Empty, (short) statusCode, durationInMilliSeconds, responseHeaders);
    }

    internal override async Task<StoreResponse> InvokeStoreAsync(
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      DocumentServiceRequest request)
    {
      Guid activityId = System.Diagnostics.Trace.CorrelationManager.ActivityId;
      INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
      headers.Add("x-ms-request-validation-failure", "1");
      if (!request.IsBodySeekableClonableAndCountable)
        throw new InternalServerErrorException(RMResources.InternalServerError, headers);
      StoreResponse storeResponse;
      using (HttpRequestMessage requestMessage = this.PrepareHttpMessage(activityId, physicalAddress, resourceOperation, request))
      {
        HttpResponseMessage responseMessage = (HttpResponseMessage) null;
        DateTime sendTimeUtc = DateTime.UtcNow;
        try
        {
          this.BeforeRequest(activityId, requestMessage.RequestUri, request.ResourceType, requestMessage.Headers);
          responseMessage = await this.httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        }
        catch (Exception ex)
        {
          System.Diagnostics.Trace.CorrelationManager.ActivityId = activityId;
          if (WebExceptionUtility.IsWebExceptionRetriable(ex))
          {
            DefaultTrace.TraceInformation("Received retriable exception {0} sending the request to {1}, will reresolve the address send time UTC: {2}", (object) ex, (object) physicalAddress, (object) sendTimeUtc);
            throw new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), ex, SubStatusCodes.TransportGenerated410, localIpAddress: physicalAddress.ToString());
          }
          if (request.IsReadOnlyRequest)
          {
            DefaultTrace.TraceInformation("Received exception {0} on readonly requestsending the request to {1}, will reresolve the address send time UTC: {2}", (object) ex, (object) physicalAddress, (object) sendTimeUtc);
            throw new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), ex, SubStatusCodes.TransportGenerated410, localIpAddress: physicalAddress.ToString());
          }
          ServiceUnavailableException unavailableException = ServiceUnavailableException.Create(new SubStatusCodes?(SubStatusCodes.Unknown), ex, requestUri: physicalAddress);
          unavailableException.Headers.Add("x-ms-request-validation-failure", "1");
          unavailableException.Headers.Add("x-ms-write-request-trigger-refresh", "1");
          throw unavailableException;
        }
        finally
        {
          double totalMilliseconds = (DateTime.UtcNow - sendTimeUtc).TotalMilliseconds;
          this.AfterRequest(activityId, responseMessage != null ? responseMessage.StatusCode : (HttpStatusCode) 0, totalMilliseconds, responseMessage != null ? responseMessage.Headers : (HttpResponseHeaders) null);
        }
        using (responseMessage)
          storeResponse = await HttpTransportClient.ProcessHttpResponse(request.ResourceAddress, activityId.ToString(), responseMessage, physicalAddress, request);
      }
      return storeResponse;
    }

    private static void AddHeader(
      HttpRequestHeaders requestHeaders,
      string headerName,
      DocumentServiceRequest request)
    {
      string header = request.Headers[headerName];
      if (string.IsNullOrEmpty(header))
        return;
      requestHeaders.Add(headerName, header);
    }

    private static void AddHeader(
      HttpContentHeaders requestHeaders,
      string headerName,
      DocumentServiceRequest request)
    {
      string header = request.Headers[headerName];
      if (string.IsNullOrEmpty(header))
        return;
      requestHeaders.Add(headerName, header);
    }

    private static void AddHeader(
      HttpRequestHeaders requestHeaders,
      string headerName,
      string headerValue)
    {
      if (string.IsNullOrEmpty(headerValue))
        return;
      requestHeaders.Add(headerName, headerValue);
    }

    private string GetMatch(DocumentServiceRequest request, ResourceOperation resourceOperation)
    {
      switch (resourceOperation.operationType)
      {
        case OperationType.ExecuteJavaScript:
        case OperationType.Patch:
        case OperationType.Delete:
        case OperationType.Replace:
        case OperationType.Upsert:
          return request.Headers["If-Match"];
        case OperationType.Read:
        case OperationType.ReadFeed:
          return request.Headers["If-None-Match"];
        default:
          return (string) null;
      }
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000: DisposeObjectsBeforeLosingScope", Justification = "Disposable object returned by method")]
    private HttpRequestMessage PrepareHttpMessage(
      Guid activityId,
      Uri physicalAddress,
      ResourceOperation resourceOperation,
      DocumentServiceRequest request)
    {
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-version", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "User-Agent", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-max-item-count", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-pre-trigger-include", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-pre-trigger-exclude", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-post-trigger-include", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-post-trigger-exclude", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "authorization", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-indexing-directive", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-migratecollection-directive", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-consistency-level", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-session-token", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "Prefer", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-expiry-seconds", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query-enable-scan", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query-emit-traces", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cancharge", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-canthrottle", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query-enable-low-precision-order-by", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-script-enable-logging", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-is-readonly-script", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-content-serialization-format", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-continuation", request.Continuation);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-activity-id", activityId.ToString());
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-partitionkey", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-partitionkeyrangeid", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query-enablecrosspartition", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-sdk-supportedcapabilities", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-read-key-type", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-start-epk", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-end-epk", request);
      string dateHeader = Helpers.GetDateHeader(request.Headers);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-date", dateHeader);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "Match", this.GetMatch(request, resourceOperation));
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "If-Modified-Since", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "A-IM", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-should-return-current-server-datetime", request);
      if (!request.IsNameBased)
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-docdb-resource-id", request.ResourceId);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-docdb-entity-id", request.EntityId);
      string header = request.Headers["x-ms-is-fanout-request"];
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-is-fanout-request", header);
      if (request.ResourceType == ResourceType.Collection)
      {
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "collection-partition-index", request.Headers["collection-partition-index"]);
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "collection-service-index", request.Headers["collection-service-index"]);
      }
      if (request.Headers["x-ms-cosmos-collectiontype"] != null)
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-collectiontype", request.Headers["x-ms-cosmos-collectiontype"]);
      if (request.Headers["x-ms-bind-replica"] != null)
      {
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-bind-replica", request.Headers["x-ms-bind-replica"]);
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-primary-master-key", request.Headers["x-ms-primary-master-key"]);
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-secondary-master-key", request.Headers["x-ms-secondary-master-key"]);
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-primary-readonly-key", request.Headers["x-ms-primary-readonly-key"]);
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-secondary-readonly-key", request.Headers["x-ms-secondary-readonly-key"]);
      }
      if (request.Headers["x-ms-can-offer-replace-complete"] != null)
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-can-offer-replace-complete", request.Headers["x-ms-can-offer-replace-complete"]);
      if (request.Headers["x-ms-cosmos-internal-is-throughputcap-request"] != null)
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-is-throughputcap-request", request.Headers["x-ms-cosmos-internal-is-throughputcap-request"]);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-is-auto-scale", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-isquery", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-query", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-is-query-plan-request", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-is-upsert", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-supportspatiallegacycoordinates", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-partitioncount", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-collection-rid", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-filterby-schema-rid", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-usepolygonssmallerthanahemisphere", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-gateway-signature", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-populatequotainfo", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-disable-ru-per-minute-usage", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-populatequerymetrics", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populateindexmetrics", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-correlated-activityid", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-force-query-scan", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-responsecontinuationtokenlimitinkb", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-remote-storage-type", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-share-throughput", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-populatepartitionstatistics", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-documentdb-populatecollectionthroughputinfo", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-get-all-partition-key-stats", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-remaining-time-in-ms-on-client", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-client-retry-attempt-count", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-target-lsn", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-target-global-committed-lsn", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-federation-for-auth", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-exclude-system-properties", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-fanout-operation-state", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-allow-tentative-writes", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-include-tentative-writes", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-preserve-full-content", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-max-polling-interval", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-start-full-fidelity-if-none-match", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-is-materialized-view-build", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-use-archival-partition", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-changefeed-wire-format-version", request);
      if (resourceOperation.operationType == OperationType.Batch)
      {
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-is-batch-request", request);
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-batch-continue-on-error", request);
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-batch-ordered", request);
        HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-batch-atomic", request);
      }
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-force-sidebyside-indexmigration", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-is-client-encrypted", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-migrate-offer-to-autopilot", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-migrate-offer-to-manual-throughput", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-is-offer-storage-refresh-request", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-serverless-offer-storage-refresh-request", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-serverless-request", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-update-max-throughput-ever-provisioned", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-truncate-merge-log", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-allow-without-instance-id", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populate-analytical-migration-progress", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populate-byok-encryption-progress", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-include-physical-partition-throughput-info", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-update-offer-state-to-pending", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-populate-oldest-active-schema", request);
      HttpTransportClient.AddHeader(httpRequestMessage.Headers, "x-ms-cosmos-internal-offer-replace-ru-redistribution", request);
      Stream content = (Stream) null;
      if (request.Body != null)
        content = (Stream) request.CloneableBody.Clone();
      switch (resourceOperation.operationType)
      {
        case OperationType.ExecuteJavaScript:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Post;
          httpRequestMessage.Content = (HttpContent) new StreamContent(content);
          break;
        case OperationType.Create:
        case OperationType.Batch:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Post;
          httpRequestMessage.Content = (HttpContent) new StreamContent(content);
          break;
        case OperationType.Patch:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = new HttpMethod("PATCH");
          httpRequestMessage.Content = (HttpContent) new StreamContent(content);
          break;
        case OperationType.Read:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Get;
          break;
        case OperationType.ReadFeed:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
          if (content != null)
          {
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.Content = (HttpContent) new StreamContent(content);
            HttpTransportClient.AddHeader(httpRequestMessage.Content.Headers, "Content-Type", request);
            break;
          }
          httpRequestMessage.Method = HttpMethod.Get;
          break;
        case OperationType.Delete:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Delete;
          break;
        case OperationType.Replace:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Put;
          httpRequestMessage.Content = (HttpContent) new StreamContent(content);
          break;
        case OperationType.SqlQuery:
        case OperationType.Query:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Post;
          httpRequestMessage.Content = (HttpContent) new StreamContent(content);
          HttpTransportClient.AddHeader(httpRequestMessage.Content.Headers, "Content-Type", request);
          break;
        case OperationType.Head:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Head;
          break;
        case OperationType.HeadFeed:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Head;
          break;
        case OperationType.Upsert:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceFeedUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Post;
          httpRequestMessage.Content = (HttpContent) new StreamContent(content);
          break;
        case OperationType.QueryPlan:
          httpRequestMessage.RequestUri = HttpTransportClient.GetResourceEntryUri(resourceOperation.resourceType, physicalAddress, request);
          httpRequestMessage.Method = HttpMethod.Post;
          httpRequestMessage.Content = (HttpContent) new StreamContent(content);
          HttpTransportClient.AddHeader(httpRequestMessage.Content.Headers, "Content-Type", request);
          break;
        case OperationType.MetadataCheckAccess:
          httpRequestMessage.RequestUri = HttpTransportClient.GetRootOperationUri(physicalAddress, resourceOperation.operationType);
          httpRequestMessage.Method = HttpMethod.Post;
          httpRequestMessage.Content = (HttpContent) new StreamContent(content);
          break;
        default:
          DefaultTrace.TraceError("Operation type {0} not found", (object) resourceOperation.operationType);
          throw new NotFoundException();
      }
      return httpRequestMessage;
    }

    internal static Uri GetSystemResourceUri(
      ResourceType resourceType,
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      throw new NotFoundException();
    }

    internal static Uri GetResourceFeedUri(
      ResourceType resourceType,
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      switch (resourceType)
      {
        case ResourceType.Database:
          return HttpTransportClient.GetDatabaseFeedUri(physicalAddress);
        case ResourceType.Collection:
          return HttpTransportClient.GetCollectionFeedUri(physicalAddress, request);
        case ResourceType.Document:
          return HttpTransportClient.GetDocumentFeedUri(physicalAddress, request);
        case ResourceType.Attachment:
          return HttpTransportClient.GetAttachmentFeedUri(physicalAddress, request);
        case ResourceType.User:
          return HttpTransportClient.GetUserFeedUri(physicalAddress, request);
        case ResourceType.Permission:
          return HttpTransportClient.GetPermissionFeedUri(physicalAddress, request);
        case ResourceType.Conflict:
          return HttpTransportClient.GetConflictFeedUri(physicalAddress, request);
        case ResourceType.StoredProcedure:
          return HttpTransportClient.GetStoredProcedureFeedUri(physicalAddress, request);
        case ResourceType.Trigger:
          return HttpTransportClient.GetTriggerFeedUri(physicalAddress, request);
        case ResourceType.UserDefinedFunction:
          return HttpTransportClient.GetUserDefinedFunctionFeedUri(physicalAddress, request);
        case ResourceType.Offer:
          return HttpTransportClient.GetOfferFeedUri(physicalAddress, request);
        case ResourceType.Schema:
          return HttpTransportClient.GetSchemaFeedUri(physicalAddress, request);
        case ResourceType.PartitionKeyRange:
          return HttpTransportClient.GetPartitionedKeyRangeFeedUri(physicalAddress, request);
        case ResourceType.UserDefinedType:
          return HttpTransportClient.GetUserDefinedTypeFeedUri(physicalAddress, request);
        case ResourceType.PartitionKey:
          return HttpTransportClient.GetPartitionKeyFeedUri(physicalAddress, request);
        case ResourceType.Snapshot:
          return HttpTransportClient.GetSnapshotFeedUri(physicalAddress, request);
        case ResourceType.PartitionedSystemDocument:
          return HttpTransportClient.GetPartitionedSystemDocumentFeedUri(physicalAddress, request);
        case ResourceType.ClientEncryptionKey:
          return HttpTransportClient.GetClientEncryptionKeyFeedUri(physicalAddress, request);
        case ResourceType.RoleDefinition:
          return HttpTransportClient.GetRoleDefinitionFeedUri(physicalAddress, request);
        case ResourceType.RoleAssignment:
          return HttpTransportClient.GetRoleAssignmentFeedUri(physicalAddress, request);
        case ResourceType.SystemDocument:
          return HttpTransportClient.GetSystemDocumentFeedUri(physicalAddress, request);
        case ResourceType.AuthPolicyElement:
          return HttpTransportClient.GetAuthPolicyElementFeedUri(physicalAddress, request);
        default:
          throw new NotFoundException();
      }
    }

    internal static Uri GetResourceEntryUri(
      ResourceType resourceType,
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      switch (resourceType)
      {
        case ResourceType.Database:
          return HttpTransportClient.GetDatabaseEntryUri(physicalAddress, request);
        case ResourceType.Collection:
          return HttpTransportClient.GetCollectionEntryUri(physicalAddress, request);
        case ResourceType.Document:
          return HttpTransportClient.GetDocumentEntryUri(physicalAddress, request);
        case ResourceType.Attachment:
          return HttpTransportClient.GetAttachmentEntryUri(physicalAddress, request);
        case ResourceType.User:
          return HttpTransportClient.GetUserEntryUri(physicalAddress, request);
        case ResourceType.Permission:
          return HttpTransportClient.GetPermissionEntryUri(physicalAddress, request);
        case ResourceType.Conflict:
          return HttpTransportClient.GetConflictEntryUri(physicalAddress, request);
        case ResourceType.Record:
          throw new NotFoundException();
        case ResourceType.StoredProcedure:
          return HttpTransportClient.GetStoredProcedureEntryUri(physicalAddress, request);
        case ResourceType.Trigger:
          return HttpTransportClient.GetTriggerEntryUri(physicalAddress, request);
        case ResourceType.UserDefinedFunction:
          return HttpTransportClient.GetUserDefinedFunctionEntryUri(physicalAddress, request);
        case ResourceType.Offer:
          return HttpTransportClient.GetOfferEntryUri(physicalAddress, request);
        case ResourceType.Schema:
          return HttpTransportClient.GetSchemaEntryUri(physicalAddress, request);
        case ResourceType.PartitionKeyRange:
          return HttpTransportClient.GetPartitionedKeyRangeEntryUri(physicalAddress, request);
        case ResourceType.UserDefinedType:
          return HttpTransportClient.GetUserDefinedTypeEntryUri(physicalAddress, request);
        case ResourceType.PartitionKey:
          return HttpTransportClient.GetPartitioKeyEntryUri(physicalAddress, request);
        case ResourceType.Snapshot:
          return HttpTransportClient.GetSnapshotEntryUri(physicalAddress, request);
        case ResourceType.PartitionedSystemDocument:
          return HttpTransportClient.GetPartitionedSystemDocumentEntryUri(physicalAddress, request);
        case ResourceType.ClientEncryptionKey:
          return HttpTransportClient.GetClientEncryptionKeyEntryUri(physicalAddress, request);
        case ResourceType.RoleDefinition:
          return HttpTransportClient.GetRoleDefinitionEntryUri(physicalAddress, request);
        case ResourceType.RoleAssignment:
          return HttpTransportClient.GetRoleAssignmentEntryUri(physicalAddress, request);
        case ResourceType.SystemDocument:
          return HttpTransportClient.GetSystemDocumentEntryUri(physicalAddress, request);
        case ResourceType.InteropUser:
          return HttpTransportClient.GetInteropUserEntryUri(physicalAddress, request);
        case ResourceType.AuthPolicyElement:
          return HttpTransportClient.GetAuthPolicyElementEntryUri(physicalAddress, request);
        default:
          throw new NotFoundException();
      }
    }

    private static Uri GetRootFeedUri(Uri baseAddress) => baseAddress;

    internal static Uri GetRootOperationUri(Uri baseAddress, OperationType operationType) => new Uri(baseAddress, PathsHelper.GenerateRootOperationPath(operationType));

    private static Uri GetDatabaseFeedUri(Uri baseAddress) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Database, string.Empty, true));

    private static Uri GetDatabaseEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Database, request, false));

    private static Uri GetCollectionFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Collection, request, true));

    private static Uri GetStoredProcedureFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.StoredProcedure, request, true));

    private static Uri GetTriggerFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Trigger, request, true));

    private static Uri GetUserDefinedFunctionFeedUri(
      Uri baseAddress,
      DocumentServiceRequest request)
    {
      return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.UserDefinedFunction, request, true));
    }

    private static Uri GetCollectionEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Collection, request, false));

    private static Uri GetStoredProcedureEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.StoredProcedure, request, false));

    private static Uri GetTriggerEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Trigger, request, false));

    private static Uri GetUserDefinedFunctionEntryUri(
      Uri baseAddress,
      DocumentServiceRequest request)
    {
      return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.UserDefinedFunction, request, false));
    }

    private static Uri GetDocumentFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Document, request, true));

    private static Uri GetDocumentEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Document, request, false));

    private static Uri GetConflictFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Conflict, request, true));

    private static Uri GetConflictEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Conflict, request, false));

    private static Uri GetAttachmentFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Attachment, request, true));

    private static Uri GetAttachmentEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Attachment, request, false));

    private static Uri GetUserFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.User, request, true));

    private static Uri GetUserEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.User, request, false));

    private static Uri GetClientEncryptionKeyFeedUri(
      Uri baseAddress,
      DocumentServiceRequest request)
    {
      return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.ClientEncryptionKey, request, true));
    }

    private static Uri GetClientEncryptionKeyEntryUri(
      Uri baseAddress,
      DocumentServiceRequest request)
    {
      return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.ClientEncryptionKey, request, false));
    }

    private static Uri GetUserDefinedTypeFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.UserDefinedType, request, true));

    private static Uri GetUserDefinedTypeEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.UserDefinedType, request, false));

    private static Uri GetPermissionFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Permission, request, true));

    private static Uri GetPermissionEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Permission, request, false));

    private static Uri GetOfferFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Offer, request, true));

    private static Uri GetOfferEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Offer, request, false));

    private static Uri GetSchemaFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Schema, request, true));

    private static Uri GetSchemaEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Schema, request, false));

    private static Uri GetSnapshotFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Snapshot, request, true));

    private static Uri GetSnapshotEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.Snapshot, request, false));

    private static Uri GetRoleDefinitionFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.RoleDefinition, request, true));

    private static Uri GetRoleDefinitionEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.RoleDefinition, request, false));

    private static Uri GetRoleAssignmentFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.RoleAssignment, request, true));

    private static Uri GetRoleAssignmentEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.RoleAssignment, request, false));

    private static Uri GetAuthPolicyElementFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.AuthPolicyElement, request, true));

    private static Uri GetAuthPolicyElementEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.AuthPolicyElement, request, false));

    private static Uri GetInteropUserEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.InteropUser, request, false));

    private static Uri GetSystemDocumentFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.SystemDocument, request, true));

    private static Uri GetSystemDocumentEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.SystemDocument, request, false));

    private static Uri GetPartitionedSystemDocumentFeedUri(
      Uri baseAddress,
      DocumentServiceRequest request)
    {
      return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionedSystemDocument, request, true));
    }

    private static Uri GetPartitionedKeyRangeFeedUri(
      Uri baseAddress,
      DocumentServiceRequest request)
    {
      return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionKeyRange, request, true));
    }

    private static Uri GetPartitionedKeyRangeEntryUri(
      Uri baseAddress,
      DocumentServiceRequest request)
    {
      return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionKeyRange, request, false));
    }

    private static Uri GetPartitioKeyEntryUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionKey, request, false));

    private static Uri GetPartitionedSystemDocumentEntryUri(
      Uri baseAddress,
      DocumentServiceRequest request)
    {
      return new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionedSystemDocument, request, false));
    }

    private static Uri GetPartitionKeyFeedUri(Uri baseAddress, DocumentServiceRequest request) => new Uri(baseAddress, PathsHelper.GeneratePath(ResourceType.PartitionKey, request, true));

    public static Task<StoreResponse> ProcessHttpResponse(
      string resourceAddress,
      string activityId,
      HttpResponseMessage response,
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      if (response == null)
      {
        InternalServerErrorException serverErrorException = new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.InvalidBackendResponse), physicalAddress);
        serverErrorException.Headers.Set("x-ms-activity-id", activityId);
        serverErrorException.Headers.Add("x-ms-request-validation-failure", "1");
        throw serverErrorException;
      }
      return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotModified ? HttpTransportClient.CreateStoreResponseFromHttpResponse(response) : HttpTransportClient.CreateErrorResponseFromHttpResponse(resourceAddress, activityId, response, request);
    }

    private static async Task<StoreResponse> CreateErrorResponseFromHttpResponse(
      string resourceAddress,
      string activityId,
      HttpResponseMessage response,
      DocumentServiceRequest request)
    {
      using (response)
      {
        HttpStatusCode statusCode = response.StatusCode;
        string errorResponseAsync = await TransportClient.GetErrorResponseAsync(response);
        long result = -1;
        IEnumerable<string> values1;
        if (response.Headers.TryGetValues("lsn", out values1))
          long.TryParse(values1.FirstOrDefault<string>(), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result);
        string str = (string) null;
        IEnumerable<string> values2;
        if (response.Headers.TryGetValues("x-ms-documentdb-partitionkeyrangeid", out values2))
          str = values2.FirstOrDefault<string>();
        DocumentClientException documentClientException;
        switch (statusCode)
        {
          case HttpStatusCode.BadRequest:
            documentClientException = (DocumentClientException) new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.BadRequest : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.Unauthorized:
            documentClientException = (DocumentClientException) new UnauthorizedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.Unauthorized : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.Forbidden:
            documentClientException = (DocumentClientException) new ForbiddenException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.Forbidden : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.NotFound:
            if (response.Content != null && response.Content.Headers != null && response.Content.Headers.ContentType != null && !string.IsNullOrEmpty(response.Content.Headers.ContentType.MediaType) && response.Content.Headers.ContentType.MediaType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase))
            {
              GoneException goneException = new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), SubStatusCodes.Unknown, response.RequestMessage.RequestUri);
              goneException.LSN = result;
              goneException.PartitionKeyRangeId = str;
              documentClientException = (DocumentClientException) goneException;
              documentClientException.Headers.Set("x-ms-activity-id", activityId);
              break;
            }
            if (request.IsValidStatusCodeForExceptionlessRetry((int) statusCode))
              return await HttpTransportClient.CreateStoreResponseFromHttpResponse(response, false);
            documentClientException = (DocumentClientException) new NotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.NotFound : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.MethodNotAllowed:
            documentClientException = (DocumentClientException) new MethodNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.MethodNotAllowed : (object) errorResponseAsync), (Exception) null, response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.RequestTimeout:
            documentClientException = (DocumentClientException) new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.RequestTimeout : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.Conflict:
            if (request.IsValidStatusCodeForExceptionlessRetry((int) statusCode))
              return await HttpTransportClient.CreateStoreResponseFromHttpResponse(response, false);
            documentClientException = (DocumentClientException) new ConflictException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.EntityAlreadyExists : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.Gone:
            TransportClient.LogGoneException(response.RequestMessage.RequestUri, activityId);
            uint statusFromHeader = HttpTransportClient.GetSubsStatusFromHeader(response);
            switch (statusFromHeader)
            {
              case 1000:
                documentClientException = (DocumentClientException) new InvalidPartitionException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.Gone : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
                break;
              case 1002:
                documentClientException = (DocumentClientException) new PartitionKeyRangeGoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.Gone : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
                break;
              case 1007:
                documentClientException = (DocumentClientException) new PartitionKeyRangeIsSplittingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.Gone : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
                break;
              case 1008:
                documentClientException = (DocumentClientException) new PartitionIsMigratingException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.Gone : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
                break;
              default:
                documentClientException = (DocumentClientException) new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), response.Headers, statusFromHeader == 0U ? new SubStatusCodes?(SubStatusCodes.ServerGenerated410) : new SubStatusCodes?(), response.RequestMessage.RequestUri);
                documentClientException.Headers.Set("x-ms-activity-id", activityId);
                break;
            }
            break;
          case HttpStatusCode.PreconditionFailed:
            if (request.IsValidStatusCodeForExceptionlessRetry((int) statusCode))
              return await HttpTransportClient.CreateStoreResponseFromHttpResponse(response, false);
            documentClientException = (DocumentClientException) new PreconditionFailedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.PreconditionFailed : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.RequestEntityTooLarge:
            documentClientException = (DocumentClientException) new RequestEntityTooLargeException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.RequestEntityTooLarge, (object) "x-ms-max-item-count")), response.Headers, response.RequestMessage.RequestUri);
            break;
          case (HttpStatusCode) 423:
            documentClientException = (DocumentClientException) new LockedException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.Locked : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case (HttpStatusCode) 429:
            if (request.IsValidStatusCodeForExceptionlessRetry((int) statusCode))
              return await HttpTransportClient.CreateStoreResponseFromHttpResponse(response, false);
            documentClientException = (DocumentClientException) new RequestRateTooLargeException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.TooManyRequests : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            IEnumerable<string> source = (IEnumerable<string>) null;
            try
            {
              source = response.Headers.GetValues("x-ms-retry-after-ms");
            }
            catch (InvalidOperationException ex)
            {
              DefaultTrace.TraceWarning("RequestRateTooLargeException being thrown without RetryAfter.");
            }
            if (source != null && source.Any<string>())
            {
              documentClientException.Headers.Set("x-ms-retry-after-ms", source.First<string>());
              break;
            }
            break;
          case (HttpStatusCode) 449:
            documentClientException = (DocumentClientException) new RetryWithException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.RetryWith : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.InternalServerError:
            documentClientException = (DocumentClientException) new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, string.IsNullOrEmpty(errorResponseAsync) ? (object) RMResources.InternalServerError : (object) errorResponseAsync), response.Headers, response.RequestMessage.RequestUri);
            break;
          case HttpStatusCode.ServiceUnavailable:
            documentClientException = (DocumentClientException) ServiceUnavailableException.Create(HttpTransportClient.GetSubsStatusFromHeader(response) == 0U ? new SubStatusCodes?(SubStatusCodes.ServerGenerated503) : new SubStatusCodes?(), headers: response.Headers, requestUri: response.RequestMessage.RequestUri);
            break;
          default:
            DefaultTrace.TraceCritical("Unrecognized status code {0} returned by backend. ActivityId {1}", (object) statusCode, (object) activityId);
            TransportClient.LogException(response.RequestMessage.RequestUri, activityId);
            documentClientException = (DocumentClientException) new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.InvalidBackendResponse), response.Headers, response.RequestMessage.RequestUri);
            break;
        }
        documentClientException.LSN = result;
        documentClientException.PartitionKeyRangeId = str;
        documentClientException.ResourceAddress = resourceAddress;
        throw documentClientException;
      }
    }

    private static uint GetSubsStatusFromHeader(HttpResponseMessage response)
    {
      uint result = 0;
      try
      {
        IEnumerable<string> values = response.Headers.GetValues("x-ms-substatus");
        if (values != null)
        {
          if (values.Any<string>())
          {
            if (!uint.TryParse(values.First<string>(), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
              throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.InvalidBackendResponse), response.Headers, response.RequestMessage.RequestUri);
          }
        }
      }
      catch (InvalidOperationException ex)
      {
        DefaultTrace.TraceInformation("SubStatus doesn't exist in the header");
      }
      return result;
    }

    internal static string GetHeader(string[] names, string[] values, string name)
    {
      for (int index = 0; index < names.Length; ++index)
      {
        if (string.Equals(names[index], name, StringComparison.Ordinal))
          return values[index];
      }
      return (string) null;
    }

    public static async Task<StoreResponse> CreateStoreResponseFromHttpResponse(
      HttpResponseMessage responseMessage,
      bool includeContent = true)
    {
      StoreResponse response = new StoreResponse()
      {
        Headers = (INameValueCollection) new DictionaryNameValueCollection(StringComparer.OrdinalIgnoreCase)
      };
      StoreResponse fromHttpResponse;
      using (responseMessage)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> header in (System.Net.Http.Headers.HttpHeaders) responseMessage.Headers)
          response.Headers[header.Key] = string.Compare(header.Key, "x-ms-alt-content-path", StringComparison.Ordinal) != 0 ? header.Value.SingleOrDefault<string>() : Uri.UnescapeDataString(header.Value.SingleOrDefault<string>());
        response.Status = (int) responseMessage.StatusCode;
        if (includeContent && responseMessage.Content != null)
        {
          Stream bufferredStream = (Stream) new MemoryStream();
          await responseMessage.Content.CopyToAsync(bufferredStream);
          bufferredStream.Position = 0L;
          response.ResponseBody = bufferredStream;
          bufferredStream = (Stream) null;
        }
        fromHttpResponse = response;
      }
      response = (StoreResponse) null;
      return fromHttpResponse;
    }
  }
}
