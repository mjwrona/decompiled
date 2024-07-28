// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.TerrapinHttpClientFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public static class TerrapinHttpClientFacade
  {
    private static readonly IFactory<Uri, System.Net.Http.HttpClient> HttpClient = (IFactory<Uri, System.Net.Http.HttpClient>) new LimitingHttpClientFactory((Func<System.Net.Http.HttpClient>) (() => new System.Net.Http.HttpClient((HttpMessageHandler) new HttpClientHandler()
    {
      AllowAutoRedirect = true,
      MaxAutomaticRedirections = 10,
      UseDefaultCredentials = false
    })
    {
      Timeout = TimeSpan.FromSeconds(5.0)
    }));

    public static IRequestContextAwareHttpClient Get() => (IRequestContextAwareHttpClient) new HttpClientWrapper(TerrapinHttpClientFacade.HttpClient, "Packaging.Terrapin.HttpClient");
  }
}
