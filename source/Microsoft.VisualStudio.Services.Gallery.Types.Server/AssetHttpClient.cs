// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.AssetHttpClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  public class AssetHttpClient : VssHttpClientBase
  {
    public AssetHttpClient(HttpMessageHandler httpMessageHandler)
      : base((Uri) null, httpMessageHandler, false)
    {
    }

    public AssetHttpClient(Uri baseUrl, Guid sessionId)
      : base(baseUrl, new VssCredentials(), new VssHttpRequestSettings(sessionId))
    {
    }

    public Task<HttpResponseMessage> GetAsset(
      string assetUrl,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
        return this.SendAsync(new HttpRequestMessage(new HttpMethod("GET"), assetUrl));
    }
  }
}
