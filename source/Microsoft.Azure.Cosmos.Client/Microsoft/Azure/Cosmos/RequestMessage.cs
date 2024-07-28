// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.RequestMessage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public class RequestMessage : IDisposable
  {
    private readonly Lazy<Dictionary<string, object>> properties = new Lazy<Dictionary<string, object>>(new Func<Dictionary<string, object>>(RequestMessage.CreateDictionary));
    private readonly Lazy<Headers> headers = new Lazy<Headers>(new Func<Headers>(RequestMessage.CreateHeaders));
    private bool disposed;
    private Stream content;

    public RequestMessage() => this.Trace = (ITrace) NoOpTrace.Singleton;

    public RequestMessage(HttpMethod method, Uri requestUri)
    {
      this.Method = method;
      this.RequestUriString = requestUri?.OriginalString;
      this.InternalRequestUri = requestUri;
      this.Trace = (ITrace) NoOpTrace.Singleton;
    }

    internal RequestMessage(HttpMethod method, string requestUriString, ITrace trace)
    {
      this.Method = method;
      this.RequestUriString = requestUriString;
      this.Trace = trace ?? throw new ArgumentNullException(nameof (trace));
    }

    public virtual HttpMethod Method { get; private set; }

    public virtual Uri RequestUri
    {
      get
      {
        if (this.InternalRequestUri == (Uri) null)
          this.InternalRequestUri = new Uri(this.RequestUriString, UriKind.Relative);
        return this.InternalRequestUri;
      }
    }

    public virtual Headers Headers => this.headers.Value;

    public virtual Stream Content
    {
      get => this.content;
      set
      {
        this.CheckDisposed();
        this.content = value;
      }
    }

    internal string RequestUriString { get; }

    internal Uri InternalRequestUri { get; private set; }

    internal ITrace Trace { get; set; }

    internal RequestOptions RequestOptions { get; set; }

    internal ResourceType ResourceType { get; set; }

    internal OperationType OperationType { get; set; }

    internal PartitionKeyRangeIdentity PartitionKeyRangeId { get; set; }

    internal bool? UseGatewayMode { get; set; }

    internal DocumentServiceRequest DocumentServiceRequest { get; set; }

    internal Action<DocumentServiceRequest> OnBeforeSendRequestActions { get; set; }

    internal bool IsPropertiesInitialized => this.properties.IsValueCreated;

    internal bool IsPartitionKeyRangeHandlerRequired => this.OperationType == OperationType.ReadFeed && this.ResourceType.IsPartitioned() && this.PartitionKeyRangeId == null && this.Headers.PartitionKey == null;

    internal string ContainerId { get; set; }

    internal string DatabaseId { get; set; }

    public virtual Dictionary<string, object> Properties => this.properties.Value;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.disposed)
        return;
      this.disposed = true;
      if (this.Content == null)
        return;
      this.Content.Dispose();
    }

    internal void AddThroughputHeader(int? throughputValue)
    {
      if (!throughputValue.HasValue)
        return;
      this.Headers.OfferThroughput = throughputValue.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    internal void AddThroughputPropertiesHeader(ThroughputProperties throughputProperties)
    {
      if (throughputProperties == null)
        return;
      if (throughputProperties.Throughput.HasValue && (throughputProperties.AutoscaleMaxThroughput.HasValue || throughputProperties.AutoUpgradeMaxThroughputIncrementPercentage.HasValue))
        throw new InvalidOperationException("Autoscale provisioned throughput can not be configured with fixed offer");
      if (throughputProperties.Throughput.HasValue)
      {
        this.AddThroughputHeader(throughputProperties.Throughput);
      }
      else
      {
        if (throughputProperties?.Content?.OfferAutoscaleSettings == null)
          return;
        this.Headers.Add("x-ms-cosmos-offer-autopilot-settings", throughputProperties.Content.OfferAutoscaleSettings.GetJsonString());
      }
    }

    internal async Task AssertPartitioningDetailsAsync(
      CosmosClient client,
      CancellationToken cancellationToken,
      ITrace trace)
    {
      if (this.IsMasterOperation())
        return;
      await Task.CompletedTask;
    }

    internal DocumentServiceRequest ToDocumentServiceRequest()
    {
      if (this.DocumentServiceRequest == null)
      {
        DocumentServiceRequest documentServiceRequest1 = this.OperationType != OperationType.ReadFeed || this.ResourceType != ResourceType.Database ? new DocumentServiceRequest(this.OperationType, this.ResourceType, this.RequestUriString, this.Content, AuthorizationTokenType.PrimaryMasterKey, this.Headers.CosmosMessageHeaders.INameValueCollection) : new DocumentServiceRequest(this.OperationType, (string) null, this.ResourceType, this.Content, this.Headers.CosmosMessageHeaders.INameValueCollection, false, AuthorizationTokenType.PrimaryMasterKey);
        bool? useGatewayMode = this.UseGatewayMode;
        if (useGatewayMode.HasValue)
        {
          DocumentServiceRequest documentServiceRequest2 = documentServiceRequest1;
          useGatewayMode = this.UseGatewayMode;
          int num = useGatewayMode.Value ? 1 : 0;
          documentServiceRequest2.UseGatewayMode = num != 0;
        }
        documentServiceRequest1.UseStatusCodeForFailures = true;
        documentServiceRequest1.UseStatusCodeFor429 = true;
        documentServiceRequest1.Properties = (IDictionary<string, object>) this.Properties;
        this.DocumentServiceRequest = documentServiceRequest1;
      }
      if (this.PartitionKeyRangeId != null)
        this.DocumentServiceRequest.RouteTo(this.PartitionKeyRangeId);
      this.OnBeforeRequestHandler(this.DocumentServiceRequest);
      return this.DocumentServiceRequest;
    }

    private static Dictionary<string, object> CreateDictionary() => new Dictionary<string, object>();

    private static Headers CreateHeaders() => new Headers();

    private void OnBeforeRequestHandler(DocumentServiceRequest serviceRequest)
    {
      Action<DocumentServiceRequest> sendRequestActions = this.OnBeforeSendRequestActions;
      if (sendRequestActions == null)
        return;
      sendRequestActions(serviceRequest);
    }

    private bool AssertPartitioningPropertiesAndHeaders()
    {
      int num = !string.IsNullOrEmpty(this.Headers.PartitionKey) ? 1 : 0;
      bool flag1 = this.Properties.ContainsKey("x-ms-effective-partition-key-string");
      if ((num & (flag1 ? 1 : 0)) != 0)
        throw new ArgumentNullException(RMResources.PartitionKeyAndEffectivePartitionKeyBothSpecified);
      int operationType = (int) this.OperationType;
      if (num == 0 && !flag1 && this.OperationType.IsPointOperation())
        throw new ArgumentNullException(RMResources.MissingPartitionKeyValue);
      bool flag2 = !string.IsNullOrEmpty(this.Headers.PartitionKeyRangeId);
      if (flag2 && this.OperationType != OperationType.Query && this.OperationType != OperationType.ReadFeed && this.OperationType != OperationType.Batch)
        throw new ArgumentOutOfRangeException(RMResources.UnexpectedPartitionKeyRangeId);
      if ((num & (flag2 ? 1 : 0)) != 0)
        throw new ArgumentOutOfRangeException(RMResources.PartitionKeyAndPartitionKeyRangeRangeIdBothSpecified);
      return true;
    }

    private bool IsMasterOperation() => this.ResourceType != ResourceType.Document;

    private void CheckDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().ToString());
    }
  }
}
