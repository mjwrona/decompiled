// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.IHttpClientExtenions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class IHttpClientExtenions
  {
    public static async Task<HttpResponseMessage> GetAsync(
      this IHttpClient client,
      Uri requestUri,
      HttpCompletionOption completionOption)
    {
      return await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);
    }

    public static async Task<HttpResponseMessage> PostAsync(
      this IHttpClient client,
      Uri requestUri,
      HttpContent content,
      HttpCompletionOption completionOption)
    {
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
      {
        Content = content
      })
        httpResponseMessage = await client.SendAsync(request, completionOption);
      return httpResponseMessage;
    }
  }
}
