// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBatchOperationMessage
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataBatchOperationMessage : ODataMessage
  {
    private readonly IODataStreamListener operationListener;
    private readonly IODataPayloadUriConverter payloadUriConverter;
    private Func<Stream> contentStreamCreatorFunc;
    private ODataBatchOperationHeaders headers;

    internal ODataBatchOperationMessage(
      Func<Stream> contentStreamCreatorFunc,
      ODataBatchOperationHeaders headers,
      IODataStreamListener operationListener,
      IODataPayloadUriConverter payloadUriConverter,
      bool writing)
      : base(writing, false, -1L)
    {
      this.contentStreamCreatorFunc = contentStreamCreatorFunc;
      this.operationListener = operationListener;
      this.headers = headers;
      this.payloadUriConverter = payloadUriConverter;
    }

    public override IEnumerable<KeyValuePair<string, string>> Headers => (IEnumerable<KeyValuePair<string, string>>) this.headers ?? Enumerable.Empty<KeyValuePair<string, string>>();

    public override string GetHeader(string headerName)
    {
      string str;
      return this.headers != null && this.headers.TryGetValue(headerName, out str) ? str : (string) null;
    }

    public override void SetHeader(string headerName, string headerValue)
    {
      this.VerifyNotCompleted();
      this.VerifyCanSetHeader();
      if (headerValue == null)
      {
        if (this.headers == null)
          return;
        this.headers.Remove(headerName);
      }
      else
      {
        if (this.headers == null)
          this.headers = new ODataBatchOperationHeaders();
        this.headers[headerName] = headerValue;
      }
    }

    public override Stream GetStream()
    {
      this.VerifyNotCompleted();
      this.operationListener.StreamRequested();
      Stream stream = this.contentStreamCreatorFunc();
      this.PartHeaderProcessingCompleted();
      return stream;
    }

    public override Task<Stream> GetStreamAsync()
    {
      this.VerifyNotCompleted();
      Task antecedentTask = this.operationListener.StreamRequestedAsync();
      Stream contentStream = this.contentStreamCreatorFunc();
      this.PartHeaderProcessingCompleted();
      return antecedentTask.FollowOnSuccessWith<Stream>((Func<Task, Stream>) (task => contentStream));
    }

    internal override TInterface QueryInterface<TInterface>() => default (TInterface);

    internal Uri ResolveUrl(Uri baseUri, Uri payloadUri)
    {
      ExceptionUtils.CheckArgumentNotNull<Uri>(payloadUri, nameof (payloadUri));
      return this.payloadUriConverter != null ? this.payloadUriConverter.ConvertPayloadUri(baseUri, payloadUri) : (Uri) null;
    }

    internal void PartHeaderProcessingCompleted() => this.contentStreamCreatorFunc = (Func<Stream>) null;

    internal void VerifyNotCompleted()
    {
      if (this.contentStreamCreatorFunc == null)
        throw new ODataException(Strings.ODataBatchOperationMessage_VerifyNotCompleted);
    }
  }
}
