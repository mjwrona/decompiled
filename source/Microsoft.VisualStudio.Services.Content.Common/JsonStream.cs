// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.JsonStream
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public struct JsonStream : IDisposable
  {
    public readonly Stream RawStream;

    public JsonStream(Stream rawStream) => this.RawStream = rawStream;

    public void Dispose() => this.RawStream.Dispose();

    public HttpResponseMessage CreateJsonStreamResponse(HttpRequestMessage request)
    {
      HttpResponseMessage jsonStreamResponse = new HttpResponseMessage(HttpStatusCode.OK)
      {
        RequestMessage = request,
        Content = (HttpContent) new StreamContent(this.RawStream)
      };
      jsonStreamResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json")
      {
        CharSet = Encoding.UTF8.WebName
      };
      return jsonStreamResponse;
    }
  }
}
