// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.DevSecOpsHttpClient
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [ResourceArea("{7F7E9705-96B8-4DA4-AF41-9E272C98DB69}")]
  public class DevSecOpsHttpClient : DevSecOpsHttpClientBase
  {
    public DevSecOpsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DevSecOpsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DevSecOpsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DevSecOpsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DevSecOpsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<BatchScanResult> ScanBatchAsync(
      ContainerMetadata containerMetadata,
      IEnumerable<ScanTarget> scanTargets,
      BatchScanOptions batchScanOptions = null,
      Guid? correlationId = null,
      CompressionLevel batchScanTargetCompressionLevel = CompressionLevel.NoCompression,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BatchScanResult batchScanResult;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (ZipArchive zipArchive = new ZipArchive((Stream) memoryStream, ZipArchiveMode.Create, true))
        {
          foreach (ScanTarget scanTarget in scanTargets)
          {
            using (Stream destination = zipArchive.CreateEntry(scanTarget.Name, batchScanTargetCompressionLevel).Open())
            {
              scanTarget.Content.CopyTo(destination);
              if (scanTarget.Content.CanRead)
                scanTarget.Content.Close();
            }
          }
        }
        memoryStream.Position = 0L;
        // ISSUE: reference to a compiler-generated method
        batchScanResult = await this.\u003C\u003En__0((Stream) memoryStream, containerMetadata, correlationId, batchScanOptions, userState, cancellationToken);
      }
      return batchScanResult;
    }

    public HttpContent CreateHttpContent(Stream uploadStream)
    {
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      if (uploadStream.Length > 0L)
      {
        httpContent.Headers.ContentRange = new ContentRangeHeaderValue(0L, uploadStream.Length - 1L, uploadStream.Length);
        httpContent.Headers.ContentLength = new long?(uploadStream.Length);
      }
      return httpContent;
    }
  }
}
