// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.LazyStreamContent
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Batch
{
  internal class LazyStreamContent : HttpContent
  {
    private Func<Stream> _getStream;
    private StreamContent _streamContent;

    public LazyStreamContent(Func<Stream> getStream) => this._getStream = getStream;

    private StreamContent StreamContent
    {
      get
      {
        if (this._streamContent == null)
          this._streamContent = new StreamContent(this._getStream());
        return this._streamContent;
      }
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) => this.StreamContent.CopyToAsync(stream, context);

    protected override bool TryComputeLength(out long length)
    {
      length = -1L;
      return false;
    }
  }
}
