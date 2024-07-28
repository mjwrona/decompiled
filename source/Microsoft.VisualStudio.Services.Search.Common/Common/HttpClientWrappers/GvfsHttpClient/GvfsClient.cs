// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.GvfsHttpClient.GvfsClient
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.GvfsHttpClient
{
  internal class GvfsClient : IDisposable
  {
    private HttpClient m_httpClient;
    private bool m_disposedValue;

    public GvfsClient(VssHttpMessageHandler vssHttpMessageHandler) => this.m_httpClient = new HttpClient((HttpMessageHandler) vssHttpMessageHandler, false);

    public Task<GitEndPointResponseData> DownloadObjects(
      IEnumerable<string> objectIds,
      string repoUrl,
      int commitDepth = 0)
    {
      if (!objectIds.Any<string>())
        return (Task<GitEndPointResponseData>) null;
      string objectIdJson = GvfsClient.CreateObjectIdJson(objectIds, commitDepth);
      return GvfsClient.SendRequestAsync(new Uri(repoUrl + "/gvfs/objects"), HttpMethod.Post, objectIdJson, this.m_httpClient);
    }

    public Task<GitEndPointResponseData> GetObjectSizes(
      IEnumerable<string> objectIds,
      string repoUrl)
    {
      string jsonList = GvfsClient.ToJsonList(objectIds);
      return GvfsClient.SendRequestAsync(new Uri(repoUrl + "/gvfs/sizes"), HttpMethod.Post, jsonList, this.m_httpClient);
    }

    private static string ToJsonList(IEnumerable<string> strings) => "[\"" + string.Join("\",\"", strings) + "\"]";

    private static string CreateObjectIdJson(IEnumerable<string> strings, int commitDepth) => "{\"commitDepth\": " + commitDepth.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ", \"objectIds\":" + GvfsClient.ToJsonList(strings) + "}";

    private static async Task<GitEndPointResponseData> SendRequestAsync(
      Uri requestUri,
      HttpMethod httpMethod,
      string requestContent,
      HttpClient httpClient)
    {
      HttpRequestMessage request = new HttpRequestMessage(httpMethod, requestUri);
      if (requestContent != null)
        request.Content = (HttpContent) new StringContent(requestContent, Encoding.UTF8, "application/json");
      HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
      if (response.StatusCode != HttpStatusCode.OK)
        return (GitEndPointResponseData) null;
      string contentType = string.Empty;
      IEnumerable<string> values;
      if (response.Content.Headers.TryGetValues("Content-Type", out values))
        contentType = values.First<string>();
      return new GitEndPointResponseData(response.StatusCode, contentType, await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing && this.m_httpClient != null)
      {
        this.m_httpClient.Dispose();
        this.m_httpClient = (HttpClient) null;
      }
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
