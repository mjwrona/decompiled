// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.HttpWebResponse
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  [ExcludeFromCodeCoverage]
  internal class HttpWebResponse : IHttpWebResponse
  {
    public ErrorCode ErrorCode { get; set; }

    public WebExceptionStatus ExceptionCode { get; set; }

    public System.Net.HttpWebResponse Response { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public WebHeaderCollection Headers { get; set; }

    public Stream GetResponseStream()
    {
      Stream responseStream = (Stream) null;
      if (this.ErrorCode == ErrorCode.NoError)
        responseStream = this.Response.GetResponseStream();
      return responseStream;
    }
  }
}
