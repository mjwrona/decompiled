// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataRequestMessage
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataRequestMessage : 
    ODataMessage,
    IODataRequestMessageAsync,
    IODataRequestMessage
  {
    private readonly IODataRequestMessage requestMessage;

    internal ODataRequestMessage(
      IODataRequestMessage requestMessage,
      bool writing,
      bool enableMessageStreamDisposal,
      long maxMessageSize)
      : base(writing, enableMessageStreamDisposal, maxMessageSize)
    {
      this.requestMessage = requestMessage;
    }

    public Uri Url
    {
      get => this.requestMessage.Url;
      set => throw new ODataException(Strings.ODataMessage_MustNotModifyMessage);
    }

    public string Method
    {
      get => this.requestMessage.Method;
      set => throw new ODataException(Strings.ODataMessage_MustNotModifyMessage);
    }

    public override IEnumerable<KeyValuePair<string, string>> Headers => this.requestMessage.Headers;

    public override string GetHeader(string headerName) => this.requestMessage.GetHeader(headerName);

    public override void SetHeader(string headerName, string headerValue)
    {
      this.VerifyCanSetHeader();
      this.requestMessage.SetHeader(headerName, headerValue);
    }

    public override Stream GetStream() => this.GetStream(new Func<Stream>(this.requestMessage.GetStream), true);

    public override Task<Stream> GetStreamAsync() => this.requestMessage is IODataRequestMessageAsync requestMessage ? this.GetStreamAsync(new Func<Task<Stream>>(requestMessage.GetStreamAsync), true) : throw new ODataException(Strings.ODataRequestMessage_AsyncNotAvailable);

    internal override TInterface QueryInterface<TInterface>() => this.requestMessage as TInterface;
  }
}
