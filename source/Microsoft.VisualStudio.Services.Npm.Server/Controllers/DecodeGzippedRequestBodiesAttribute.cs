// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.DecodeGzippedRequestBodiesAttribute
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
  public class DecodeGzippedRequestBodiesAttribute : AuthorizationFilterAttribute
  {
    public override async Task OnAuthorizationAsync(
      HttpActionContext actionContext,
      CancellationToken cancellationToken)
    {
      HttpRequestMessage request = actionContext.Request;
      if (request.Content == null)
        request = (HttpRequestMessage) null;
      else if (!request.Content.Headers.ContentEncoding.Contains("gzip"))
      {
        request = (HttpRequestMessage) null;
      }
      else
      {
        StreamContent streamContent = new StreamContent((Stream) new GZipStream(await request.Content.ReadAsStreamAsync(), CompressionMode.Decompress));
        foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Content.Headers)
          streamContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
        streamContent.Headers.ContentEncoding.Clear();
        request.Content = (HttpContent) streamContent;
        request = (HttpRequestMessage) null;
      }
    }
  }
}
