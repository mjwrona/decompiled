// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBatchOperationRequestMessage
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public sealed class ODataBatchOperationRequestMessage : 
    IODataRequestMessageAsync,
    IODataRequestMessage,
    IODataPayloadUriConverter,
    IContainerProvider
  {
    public readonly string ContentId;
    private readonly string groupId;
    private readonly ODataBatchOperationMessage message;
    private readonly List<string> dependsOnIds;

    internal ODataBatchOperationRequestMessage(
      Func<Stream> contentStreamCreatorFunc,
      string method,
      Uri requestUrl,
      ODataBatchOperationHeaders headers,
      IODataStreamListener operationListener,
      string contentId,
      IODataPayloadUriConverter payloadUriConverter,
      bool writing,
      IServiceProvider container,
      IEnumerable<string> dependsOnIds,
      string groupId)
    {
      this.Method = method;
      this.Url = requestUrl;
      this.ContentId = contentId;
      this.groupId = groupId;
      this.message = new ODataBatchOperationMessage(contentStreamCreatorFunc, headers, operationListener, payloadUriConverter, writing);
      this.Container = container;
      this.dependsOnIds = dependsOnIds == null ? new List<string>() : new List<string>(dependsOnIds);
    }

    public IEnumerable<KeyValuePair<string, string>> Headers => this.message.Headers;

    public Uri Url { get; set; }

    public string Method { get; set; }

    public IServiceProvider Container { get; private set; }

    public string GroupId => this.groupId;

    public IEnumerable<string> DependsOnIds => (IEnumerable<string>) this.dependsOnIds;

    internal ODataBatchOperationMessage OperationMessage => this.message;

    public string GetHeader(string headerName) => this.message.GetHeader(headerName);

    public void SetHeader(string headerName, string headerValue) => this.message.SetHeader(headerName, headerValue);

    public Stream GetStream() => this.message.GetStream();

    public Task<Stream> GetStreamAsync() => this.message.GetStreamAsync();

    Uri IODataPayloadUriConverter.ConvertPayloadUri(Uri baseUri, Uri payloadUri) => this.message.ResolveUrl(baseUri, payloadUri);
  }
}
