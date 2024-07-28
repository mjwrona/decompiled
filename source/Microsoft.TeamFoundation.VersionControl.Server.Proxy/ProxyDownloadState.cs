// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Proxy.ProxyDownloadState
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Proxy
{
  internal class ProxyDownloadState : DownloadState
  {
    public ProxyDownloadState(
      HttpResponseBase response,
      IVssRequestContext requestContext,
      DownloadContext downloadContext)
      : base(response)
    {
      this.RequestContext = requestContext;
      this.DownloadContext = downloadContext;
    }

    public IVssRequestContext RequestContext { get; private set; }

    public DownloadContext DownloadContext { get; private set; }

    public override Stream CacheMiss(
      FileCacheService fileCacheService,
      FileInformation fileInfo,
      bool compressOutput)
    {
      TeamFoundationTrace.Verbose("Cache miss for file: {0}:{1}", (object) fileInfo.RepositoryGuid, (object) fileInfo.FileId);
      HttpWebResponse downloadResponse = this.RequestContext.GetService<TeamFoundationProxyRepositoryService>().GetRepository((IProxyConfiguration) fileCacheService.Configuration, this.DownloadContext.RepositoryInfo).GetDownloadResponse(this.DownloadContext.DownloadUrl);
      string s1 = downloadResponse.Headers.Get("File-MD5");
      if (s1 != null)
      {
        byte[] numArray = Convert.FromBase64String(s1);
        if (numArray.Length == 16)
          fileInfo.HashValue = numArray;
        else
          TeamFoundationTrace.Error(TraceKeywordSets.General, "Hash string {0} is invalid!", (object) s1);
      }
      TeamFoundationTrace.Verbose("Encoding: \"{0}\"", (object) downloadResponse.ContentType);
      fileInfo.Length = downloadResponse.ContentLength;
      fileInfo.ContentType = downloadResponse.ContentType;
      string s2 = downloadResponse.Headers.Get("Uncompressed-Content-Length");
      long result;
      if (s2 != null && long.TryParse(s2, out result))
        fileInfo.UncompressedLength = result;
      Stream stream = downloadResponse.GetResponseStream();
      CompressionType compressionType = TeamFoundationFileService.FromMimeType(fileInfo.ContentType);
      if (!compressOutput && compressionType == CompressionType.GZip)
        stream = (Stream) new GZipStream(stream, CompressionMode.Decompress);
      return stream;
    }
  }
}
