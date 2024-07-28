// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DocumentServiceRequest
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class DocumentServiceRequest : IDisposable
  {
    private bool isDisposed;
    private const char PreferHeadersSeparator = ';';
    private const string PreferHeaderValueFormat = "{0}={1}";
    private ServiceIdentity serviceIdentity;
    private PartitionKeyRangeIdentity partitionKeyRangeIdentity;

    private DocumentServiceRequest()
    {
    }

    internal DocumentServiceRequest(
      OperationType operationType,
      string resourceIdOrFullName,
      ResourceType resourceType,
      Stream body,
      INameValueCollection headers,
      bool isNameBased,
      AuthorizationTokenType authorizationTokenType)
    {
      this.OperationType = operationType;
      this.ForceNameCacheRefresh = false;
      this.ResourceType = resourceType;
      this.Body = body;
      this.Headers = headers ?? (INameValueCollection) new DictionaryNameValueCollection();
      this.IsFeed = false;
      this.IsNameBased = isNameBased;
      if (isNameBased)
      {
        this.ResourceAddress = resourceIdOrFullName;
      }
      else
      {
        this.ResourceId = resourceIdOrFullName;
        this.ResourceAddress = resourceIdOrFullName;
      }
      this.RequestAuthorizationTokenType = authorizationTokenType;
      this.RequestContext = new DocumentServiceRequestContext();
      if (string.IsNullOrEmpty(this.Headers["x-ms-documentdb-partitionkeyrangeid"]))
        return;
      this.PartitionKeyRangeIdentity = PartitionKeyRangeIdentity.FromHeader(this.Headers["x-ms-documentdb-partitionkeyrangeid"]);
    }

    internal DocumentServiceRequest(
      OperationType operationType,
      ResourceType resourceType,
      string path,
      Stream body,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers)
    {
      this.OperationType = operationType;
      this.ForceNameCacheRefresh = false;
      this.ResourceType = resourceType;
      this.Body = body;
      this.Headers = headers ?? (INameValueCollection) new DictionaryNameValueCollection();
      this.RequestAuthorizationTokenType = authorizationTokenType;
      this.RequestContext = new DocumentServiceRequestContext();
      if (resourceType == ResourceType.Address)
        return;
      bool isFeed;
      string resourcePath;
      string resourceIdOrFullName;
      bool isNameBased;
      string databaseName;
      string collectionName;
      string documentName;
      if (!PathsHelper.TryParsePathSegmentsWithDatabaseAndCollectionAndDocumentNames(path, out isFeed, out resourcePath, out resourceIdOrFullName, out isNameBased, out databaseName, out collectionName, out documentName, parseDatabaseAndCollectionNames: true))
        throw new NotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidResourceUrlQuery, (object) path, (object) "$resolveFor"));
      this.IsNameBased = isNameBased;
      this.IsResourceNameParsedFromUri = true;
      this.IsFeed = isFeed;
      if (this.ResourceType == ResourceType.Unknown)
        this.ResourceType = PathsHelper.GetResourcePathSegment(resourcePath);
      if (isNameBased)
      {
        this.ResourceAddress = resourceIdOrFullName;
        this.DatabaseName = databaseName;
        this.CollectionName = collectionName;
        this.DocumentName = documentName;
      }
      else
      {
        this.ResourceId = resourceIdOrFullName;
        this.ResourceAddress = resourceIdOrFullName;
        Microsoft.Azure.Documents.ResourceId rid = (Microsoft.Azure.Documents.ResourceId) null;
        if (!string.IsNullOrEmpty(this.ResourceId) && !Microsoft.Azure.Documents.ResourceId.TryParse(this.ResourceId, out rid) && this.ResourceType != ResourceType.Offer && this.ResourceType != ResourceType.Media && this.ResourceType != ResourceType.DatabaseAccount && this.ResourceType != ResourceType.Snapshot && this.ResourceType != ResourceType.EncryptionScope && this.ResourceType != ResourceType.RoleDefinition && this.ResourceType != ResourceType.RoleAssignment && this.ResourceType != ResourceType.InteropUser && this.ResourceType != ResourceType.AuthPolicyElement)
          throw new NotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidResourceUrlQuery, (object) path, (object) "$resolveFor"));
      }
      if (string.IsNullOrEmpty(this.Headers["x-ms-documentdb-partitionkeyrangeid"]))
        return;
      this.PartitionKeyRangeIdentity = PartitionKeyRangeIdentity.FromHeader(this.Headers["x-ms-documentdb-partitionkeyrangeid"]);
    }

    public bool IsNameBased { get; private set; }

    public string DatabaseName { get; private set; }

    public string CollectionName { get; private set; }

    public string DocumentName { get; private set; }

    public bool IsResourceNameParsedFromUri { get; private set; }

    public bool UseGatewayMode { get; set; }

    public bool UseStatusCodeForFailures { get; set; }

    public bool UseStatusCodeFor429 { get; set; }

    public bool UseStatusCodeForBadRequest { get; set; }

    public bool DisableRetryWithPolicy { get; set; }

    public ServiceIdentity ServiceIdentity
    {
      get => this.serviceIdentity;
      private set => this.serviceIdentity = value;
    }

    public DocumentServiceRequest.SystemAuthorizationParameters SystemAuthorizationParams { get; set; }

    public PartitionKeyRangeIdentity PartitionKeyRangeIdentity
    {
      get => this.partitionKeyRangeIdentity;
      private set
      {
        this.partitionKeyRangeIdentity = value;
        if (value != null)
          this.Headers["x-ms-documentdb-partitionkeyrangeid"] = value.ToHeader();
        else
          this.Headers.Remove("x-ms-documentdb-partitionkeyrangeid");
      }
    }

    public void RouteTo(ServiceIdentity serviceIdentity)
    {
      if (this.PartitionKeyRangeIdentity != null)
      {
        DefaultTrace.TraceCritical("This request was going to be routed to partition key range");
        throw new InternalServerErrorException();
      }
      this.ServiceIdentity = serviceIdentity;
    }

    public void RouteTo(
      PartitionKeyRangeIdentity partitionKeyRangeIdentity)
    {
      if (this.ServiceIdentity != null)
      {
        DefaultTrace.TraceCritical("This request was going to be routed to service identity");
        throw new InternalServerErrorException();
      }
      this.PartitionKeyRangeIdentity = partitionKeyRangeIdentity;
    }

    public string ResourceId { get; set; }

    public DocumentServiceRequestContext RequestContext { get; set; }

    public string ResourceAddress { get; private set; }

    public bool IsFeed { get; set; }

    public string EntityId { get; set; }

    public INameValueCollection Headers { get; private set; }

    public IDictionary<string, object> Properties { get; set; }

    public Stream Body { get; set; }

    public CloneableStream CloneableBody { get; private set; }

    public AuthorizationTokenType RequestAuthorizationTokenType { get; set; }

    public bool IsBodySeekableClonableAndCountable => this.Body == null || this.CloneableBody != null;

    public OperationType OperationType { get; private set; }

    public ResourceType ResourceType { get; private set; }

    public string QueryString { get; set; }

    public string Continuation
    {
      get => this.Headers["x-ms-continuation"];
      set => this.Headers["x-ms-continuation"] = value;
    }

    internal string ApiVersion => this.Headers["x-ms-version"];

    public bool ForceNameCacheRefresh { get; set; }

    public int LastCollectionRoutingMapHashCode { get; set; }

    public bool ForcePartitionKeyRangeRefresh { get; set; }

    public bool ForceCollectionRoutingMapRefresh { get; set; }

    public bool ForceMasterRefresh { get; set; }

    public bool IsReadOnlyRequest => this.OperationType == OperationType.Read || this.OperationType == OperationType.ReadFeed || this.OperationType == OperationType.Head || this.OperationType == OperationType.HeadFeed || this.OperationType == OperationType.Query || this.OperationType == OperationType.SqlQuery || this.OperationType == OperationType.QueryPlan;

    public bool IsReadOnlyScript
    {
      get
      {
        string str = this.Headers.Get("x-ms-is-readonly-script");
        return !string.IsNullOrEmpty(str) && this.OperationType == OperationType.ExecuteJavaScript && str.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
      }
    }

    public bool IsChangeFeedRequest => !string.IsNullOrWhiteSpace(this.Headers.Get("A-IM"));

    public string HttpMethod
    {
      get
      {
        switch (this.OperationType)
        {
          case OperationType.ExecuteJavaScript:
          case OperationType.Create:
          case OperationType.BatchApply:
          case OperationType.SqlQuery:
          case OperationType.Query:
          case OperationType.Upsert:
          case OperationType.Batch:
          case OperationType.QueryPlan:
          case OperationType.CompleteUserTransaction:
            return "POST";
          case OperationType.Patch:
            return "PATCH";
          case OperationType.Read:
            return "GET";
          case OperationType.ReadFeed:
            return this.Body == null ? "GET" : "POST";
          case OperationType.Delete:
            return "DELETE";
          case OperationType.Replace:
          case OperationType.CollectionTruncate:
            return "PUT";
          case OperationType.Head:
          case OperationType.HeadFeed:
            return "HEAD";
          default:
            throw new NotImplementedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unsupported operation type: {0}.", (object) this.OperationType));
        }
      }
    }

    public JsonSerializerSettings SerializerSettings { get; set; }

    public uint? DefaultReplicaIndex { get; set; }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      if (this.Body != null)
      {
        this.Body.Dispose();
        this.Body = (Stream) null;
      }
      if (this.CloneableBody != null)
      {
        this.CloneableBody.Dispose();
        this.CloneableBody = (CloneableStream) null;
      }
      this.isDisposed = true;
    }

    public bool IsValidAddress(ResourceType resourceType = ResourceType.Unknown)
    {
      ResourceType resourceType1;
      if (resourceType != ResourceType.Unknown)
        resourceType1 = resourceType;
      else if (!this.IsFeed)
      {
        resourceType1 = this.ResourceType;
      }
      else
      {
        if (this.ResourceType == ResourceType.Database)
          return true;
        if (this.ResourceType == ResourceType.Collection || this.ResourceType == ResourceType.User || this.ResourceType == ResourceType.ClientEncryptionKey || this.ResourceType == ResourceType.UserDefinedType)
          resourceType1 = ResourceType.Database;
        else if (this.ResourceType == ResourceType.Permission)
          resourceType1 = ResourceType.User;
        else if (this.ResourceType == ResourceType.Document || this.ResourceType == ResourceType.StoredProcedure || this.ResourceType == ResourceType.UserDefinedFunction || this.ResourceType == ResourceType.Trigger || this.ResourceType == ResourceType.Conflict || this.ResourceType == ResourceType.StoredProcedure || this.ResourceType == ResourceType.PartitionKeyRange || this.ResourceType == ResourceType.Schema || this.ResourceType == ResourceType.PartitionedSystemDocument || this.ResourceType == ResourceType.SystemDocument)
          resourceType1 = ResourceType.Collection;
        else if (this.ResourceType == ResourceType.Attachment)
          resourceType1 = ResourceType.Document;
        else
          return this.ResourceType == ResourceType.Snapshot || this.ResourceType == ResourceType.RoleDefinition || this.ResourceType == ResourceType.RoleAssignment || this.ResourceType == ResourceType.InteropUser || this.ResourceType == ResourceType.AuthPolicyElement;
      }
      return this.IsNameBased ? PathsHelper.ValidateResourceFullName(resourceType != ResourceType.Unknown ? resourceType : resourceType1, this.ResourceAddress) : PathsHelper.ValidateResourceId(resourceType1, this.ResourceId);
    }

    public void AddPreferHeader(string preferHeaderName, string preferHeaderValue)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) preferHeaderName, (object) preferHeaderValue);
      string header = this.Headers["Prefer"];
      this.Headers["Prefer"] = string.IsNullOrEmpty(header) ? str : header + ";" + str;
    }

    public static DocumentServiceRequest CreateFromResource(
      DocumentServiceRequest request,
      Resource modifiedResource)
    {
      return request.IsNameBased ? DocumentServiceRequest.CreateFromName(request.OperationType, modifiedResource, request.ResourceType, request.Headers, request.ResourceAddress, request.RequestAuthorizationTokenType) : DocumentServiceRequest.Create(request.OperationType, modifiedResource, request.ResourceType, request.RequestAuthorizationTokenType, request.Headers, request.ResourceId);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
    public static DocumentServiceRequest Create(
      OperationType operationType,
      Resource resource,
      ResourceType resourceType,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null,
      string ownerResourceId = null,
      SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
    {
      MemoryStream memoryStream = new MemoryStream();
      resource?.SaveTo((Stream) memoryStream, formattingPolicy);
      memoryStream.Position = 0L;
      return new DocumentServiceRequest(operationType, ownerResourceId != null ? ownerResourceId : resource.ResourceId, resourceType, (Stream) memoryStream, headers, false, authorizationTokenType)
      {
        CloneableBody = new CloneableStream(memoryStream)
      };
    }

    public static DocumentServiceRequest Create(
      OperationType operationType,
      ResourceType resourceType,
      MemoryStream stream,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null)
    {
      return new DocumentServiceRequest(operationType, (string) null, resourceType, (Stream) stream, headers, false, authorizationTokenType)
      {
        CloneableBody = new CloneableStream(stream)
      };
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
    public static DocumentServiceRequest Create(
      OperationType operationType,
      string ownerResourceId,
      byte[] seralizedResource,
      ResourceType resourceType,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null,
      SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
    {
      MemoryStream body = new MemoryStream(seralizedResource);
      return new DocumentServiceRequest(operationType, ownerResourceId, resourceType, (Stream) body, headers, false, authorizationTokenType);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
    public static DocumentServiceRequest Create(
      OperationType operationType,
      string ownerResourceId,
      ResourceType resourceType,
      bool isNameBased,
      AuthorizationTokenType authorizationTokenType,
      byte[] seralizedResource = null,
      INameValueCollection headers = null,
      SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
    {
      MemoryStream body = seralizedResource == null ? (MemoryStream) null : new MemoryStream(seralizedResource);
      return new DocumentServiceRequest(operationType, ownerResourceId, resourceType, (Stream) body, headers, isNameBased, authorizationTokenType);
    }

    public static DocumentServiceRequest Create(
      OperationType operationType,
      string resourceId,
      ResourceType resourceType,
      Stream body,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null)
    {
      return new DocumentServiceRequest(operationType, resourceId, resourceType, body, headers, false, authorizationTokenType);
    }

    public static DocumentServiceRequest Create(
      OperationType operationType,
      string resourceId,
      ResourceType resourceType,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null)
    {
      return new DocumentServiceRequest(operationType, resourceId, resourceType, (Stream) null, headers, false, authorizationTokenType);
    }

    public static DocumentServiceRequest CreateFromName(
      OperationType operationType,
      string resourceFullName,
      ResourceType resourceType,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null)
    {
      return new DocumentServiceRequest(operationType, resourceFullName, resourceType, (Stream) null, headers, true, authorizationTokenType);
    }

    public static DocumentServiceRequest CreateFromName(
      OperationType operationType,
      Resource resource,
      ResourceType resourceType,
      INameValueCollection headers,
      string resourceFullName,
      AuthorizationTokenType authorizationTokenType,
      SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
    {
      MemoryStream body = new MemoryStream();
      resource.SaveTo((Stream) body, formattingPolicy);
      body.Position = 0L;
      return new DocumentServiceRequest(operationType, resourceFullName, resourceType, (Stream) body, headers, true, authorizationTokenType);
    }

    public static DocumentServiceRequest Create(
      OperationType operationType,
      ResourceType resourceType,
      AuthorizationTokenType authorizationTokenType)
    {
      return new DocumentServiceRequest(operationType, (string) null, resourceType, (Stream) null, (INameValueCollection) null, false, authorizationTokenType);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
    public static DocumentServiceRequest Create(
      OperationType operationType,
      string relativePath,
      Resource resource,
      ResourceType resourceType,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null,
      SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None,
      JsonSerializerSettings settings = null)
    {
      MemoryStream memoryStream = new MemoryStream();
      resource.SaveTo((Stream) memoryStream, formattingPolicy, settings);
      memoryStream.Position = 0L;
      return new DocumentServiceRequest(operationType, resourceType, relativePath, (Stream) memoryStream, authorizationTokenType, headers)
      {
        SerializerSettings = settings,
        CloneableBody = new CloneableStream(memoryStream)
      };
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Stream is disposed with request instance")]
    public static DocumentServiceRequest Create(
      OperationType operationType,
      Uri requestUri,
      Resource resource,
      ResourceType resourceType,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null,
      SerializationFormattingPolicy formattingPolicy = SerializationFormattingPolicy.None)
    {
      MemoryStream memoryStream = new MemoryStream();
      resource.SaveTo((Stream) memoryStream, formattingPolicy);
      memoryStream.Position = 0L;
      return new DocumentServiceRequest(operationType, resourceType, requestUri.PathAndQuery, (Stream) memoryStream, authorizationTokenType, headers)
      {
        CloneableBody = new CloneableStream(memoryStream)
      };
    }

    public static DocumentServiceRequest Create(
      OperationType operationType,
      ResourceType resourceType,
      string relativePath,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null)
    {
      return new DocumentServiceRequest(operationType, resourceType, relativePath, (Stream) null, authorizationTokenType, headers);
    }

    public static DocumentServiceRequest Create(
      OperationType operationType,
      ResourceType resourceType,
      Uri requestUri,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null)
    {
      return new DocumentServiceRequest(operationType, resourceType, requestUri.PathAndQuery, (Stream) null, authorizationTokenType, headers);
    }

    public static DocumentServiceRequest Create(
      OperationType operationType,
      ResourceType resourceType,
      string relativePath,
      Stream resourceStream,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers = null)
    {
      return new DocumentServiceRequest(operationType, resourceType, relativePath, resourceStream, authorizationTokenType, headers);
    }

    public static DocumentServiceRequest Create(
      OperationType operationType,
      ResourceType resourceType,
      Uri requestUri,
      Stream resourceStream,
      AuthorizationTokenType authorizationTokenType,
      INameValueCollection headers)
    {
      return new DocumentServiceRequest(operationType, resourceType, requestUri.PathAndQuery, resourceStream, authorizationTokenType, headers);
    }

    public async Task EnsureBufferedBodyAsync()
    {
      if (this.Body == null || this.CloneableBody != null)
        return;
      this.CloneableBody = await StreamExtension.AsClonableStreamAsync(this.Body);
    }

    public void ClearRoutingHints()
    {
      this.PartitionKeyRangeIdentity = (PartitionKeyRangeIdentity) null;
      this.ServiceIdentity = (ServiceIdentity) null;
      this.RequestContext.TargetIdentity = (ServiceIdentity) null;
      this.RequestContext.ResolvedPartitionKeyRange = (PartitionKeyRange) null;
    }

    public DocumentServiceRequest Clone()
    {
      if (!this.IsBodySeekableClonableAndCountable)
        throw new InvalidOperationException();
      return new DocumentServiceRequest()
      {
        OperationType = this.OperationType,
        ForceNameCacheRefresh = this.ForceNameCacheRefresh,
        ResourceType = this.ResourceType,
        ServiceIdentity = this.ServiceIdentity,
        SystemAuthorizationParams = this.SystemAuthorizationParams == null ? (DocumentServiceRequest.SystemAuthorizationParameters) null : this.SystemAuthorizationParams.Clone(),
        CloneableBody = this.CloneableBody != null ? this.CloneableBody.Clone() : (CloneableStream) null,
        Headers = this.Headers.Clone(),
        IsFeed = this.IsFeed,
        IsNameBased = this.IsNameBased,
        ResourceAddress = this.ResourceAddress,
        ResourceId = this.ResourceId,
        RequestAuthorizationTokenType = this.RequestAuthorizationTokenType,
        RequestContext = this.RequestContext.Clone(),
        PartitionKeyRangeIdentity = this.PartitionKeyRangeIdentity,
        UseGatewayMode = this.UseGatewayMode,
        QueryString = this.QueryString,
        Continuation = this.Continuation,
        ForcePartitionKeyRangeRefresh = this.ForcePartitionKeyRangeRefresh,
        LastCollectionRoutingMapHashCode = this.LastCollectionRoutingMapHashCode,
        ForceCollectionRoutingMapRefresh = this.ForceCollectionRoutingMapRefresh,
        ForceMasterRefresh = this.ForceMasterRefresh,
        DefaultReplicaIndex = this.DefaultReplicaIndex,
        Properties = this.Properties,
        UseStatusCodeForFailures = this.UseStatusCodeForFailures,
        UseStatusCodeFor429 = this.UseStatusCodeFor429,
        DatabaseName = this.DatabaseName,
        CollectionName = this.CollectionName
      };
    }

    public sealed class SystemAuthorizationParameters
    {
      public string FederationId { get; set; }

      public string Verb { get; set; }

      public string ResourceId { get; set; }

      public DocumentServiceRequest.SystemAuthorizationParameters Clone() => new DocumentServiceRequest.SystemAuthorizationParameters()
      {
        FederationId = this.FederationId,
        Verb = this.Verb,
        ResourceId = this.ResourceId
      };
    }
  }
}
