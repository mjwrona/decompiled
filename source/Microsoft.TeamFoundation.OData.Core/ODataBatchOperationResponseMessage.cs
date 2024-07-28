// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBatchOperationResponseMessage
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public sealed class ODataBatchOperationResponseMessage : 
    IODataResponseMessageAsync,
    IODataResponseMessage,
    IODataPayloadUriConverter,
    IContainerProvider
  {
    public readonly string ContentId;
    private readonly ODataBatchOperationMessage message;
    private int statusCode;

    internal ODataBatchOperationResponseMessage(
      Func<Stream> contentStreamCreatorFunc,
      ODataBatchOperationHeaders headers,
      IODataStreamListener operationListener,
      string contentId,
      IODataPayloadUriConverter payloadUriConverter,
      bool writing,
      IServiceProvider container,
      string groupId)
    {
      this.message = new ODataBatchOperationMessage(contentStreamCreatorFunc, headers, operationListener, payloadUriConverter, writing);
      this.ContentId = contentId;
      this.Container = container;
      this.GroupId = groupId;
    }

    public int StatusCode
    {
      get => this.statusCode;
      set
      {
        this.message.VerifyNotCompleted();
        this.statusCode = value;
      }
    }

    public IEnumerable<KeyValuePair<string, string>> Headers => this.message.Headers;

    public IServiceProvider Container { get; private set; }

    public string GroupId { get; private set; }

    internal ODataBatchOperationMessage OperationMessage => this.message;

    public string GetHeader(string headerName) => this.message.GetHeader(headerName);

    public void SetHeader(string headerName, string headerValue) => this.message.SetHeader(headerName, headerValue);

    public Stream GetStream() => this.message.GetStream();

    public Task<Stream> GetStreamAsync() => this.message.GetStreamAsync();

    Uri IODataPayloadUriConverter.ConvertPayloadUri(Uri baseUri, Uri payloadUri) => this.message.ResolveUrl(baseUri, payloadUri);
  }
}
