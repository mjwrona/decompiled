// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.Search.SearchServiceClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.Search
{
  public sealed class SearchServiceClient
  {
    private readonly HttpClient httpClient;

    public SearchServiceClient(HttpClient httpClient) => this.httpClient = httpClient ?? throw new ArgumentNullException(nameof (httpClient));

    public async Task<ExtensionQueryResult> GetSearchResultsAsync(
      ExtensionQuery extensionQuery,
      Uri uri,
      bool isRequestFromChinaRegion)
    {
      if (extensionQuery == null)
        throw new ArgumentNullException(nameof (extensionQuery));
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      ExtensionQueryResult responseFromStream;
      using (HttpRequestMessage message = new HttpRequestMessage())
      {
        message.RequestUri = uri;
        message.Method = HttpMethod.Post;
        message.Content = HttpContentFactory.CreateJsonStreamContent<ExtensionQuery>(extensionQuery);
        if (isRequestFromChinaRegion)
          message.Headers.Add("X-VSMarketplace-Geo", "CN");
        else
          message.Headers.Add("X-VSMarketplace-Geo", "Global");
        using (HttpResponseMessage response = await this.httpClient.SendAsync(message).ConfigureAwait(false))
        {
          if (!response.IsSuccessStatusCode)
            throw new SearchServiceClientException(response.StatusCode, response.ReasonPhrase);
          responseFromStream = await response.GetResponseFromStream<ExtensionQueryResult>();
        }
      }
      return responseFromStream;
    }
  }
}
