// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.HttpClientFactory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade
{
  public static class HttpClientFactory
  {
    private const string DefaultUserAgent = "GalleryService";

    public static HttpClient New()
    {
      HttpClient httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("GalleryService");
      return httpClient;
    }
  }
}
