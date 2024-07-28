// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataResponseMessage
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataResponseMessage : 
    ODataMessage,
    IODataResponseMessageAsync,
    IODataResponseMessage
  {
    private readonly IODataResponseMessage responseMessage;

    internal ODataResponseMessage(
      IODataResponseMessage responseMessage,
      bool writing,
      bool enableMessageStreamDisposal,
      long maxMessageSize)
      : base(writing, enableMessageStreamDisposal, maxMessageSize)
    {
      this.responseMessage = responseMessage;
    }

    public int StatusCode
    {
      get => this.responseMessage.StatusCode;
      set => throw new ODataException(Strings.ODataMessage_MustNotModifyMessage);
    }

    public override IEnumerable<KeyValuePair<string, string>> Headers => this.responseMessage.Headers;

    public override string GetHeader(string headerName) => this.responseMessage.GetHeader(headerName);

    public override void SetHeader(string headerName, string headerValue)
    {
      this.VerifyCanSetHeader();
      this.responseMessage.SetHeader(headerName, headerValue);
    }

    public override Stream GetStream() => this.GetStream(new Func<Stream>(this.responseMessage.GetStream), false);

    public override Task<Stream> GetStreamAsync() => this.responseMessage is IODataResponseMessageAsync responseMessage ? this.GetStreamAsync(new Func<Task<Stream>>(responseMessage.GetStreamAsync), false) : throw new ODataException(Strings.ODataResponseMessage_AsyncNotAvailable);

    internal override TInterface QueryInterface<TInterface>() => this.responseMessage as TInterface;
  }
}
