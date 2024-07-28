// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryFileDownloadHttpClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class GalleryFileDownloadHttpClient : IGalleryFileDownloadHttpClient
  {
    private readonly HttpClient httpClient;

    public GalleryFileDownloadHttpClient(HttpClient httpClient) => this.httpClient = httpClient ?? throw new ArgumentNullException(nameof (httpClient));

    public async Task<bool> DownloadFileAsync(
      IVssRequestContext requestContext,
      string fileUrl,
      string filePath)
    {
      requestContext.TraceEnter(12062101, "Gallery", nameof (GalleryFileDownloadHttpClient), nameof (DownloadFileAsync));
      bool isSuccess = false;
      try
      {
        using (HttpResponseMessage response = await this.httpClient.GetAsync(fileUrl).ConfigureAwait(false))
        {
          requestContext.Trace(12062101, TraceLevel.Info, "Gallery", nameof (GalleryFileDownloadHttpClient), string.Format("Response Status Code : {0}", (object) response.StatusCode));
          if (response.IsSuccessStatusCode)
          {
            using (Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
              stream.Seek(0L, SeekOrigin.Begin);
              using (FileStream fileStream = File.Create(filePath))
                await stream.CopyToAsync((Stream) fileStream).ConfigureAwait(false);
            }
            isSuccess = true;
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062101, "Gallery", nameof (DownloadFileAsync), ex);
      }
      requestContext.TraceLeave(12062101, "Gallery", nameof (GalleryFileDownloadHttpClient), nameof (DownloadFileAsync));
      return isSuccess;
    }
  }
}
