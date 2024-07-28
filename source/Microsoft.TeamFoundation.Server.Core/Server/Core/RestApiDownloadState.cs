// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.RestApiDownloadState
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class RestApiDownloadState : IDownloadState<FileInformation>, IDisposable
  {
    private long m_transmittedLength;
    private static readonly string s_area = "FileContainer";
    private static readonly string s_layer = "DownloadsController";

    public RestApiDownloadState(IVssRequestContext requestContext, HttpResponseMessage response)
    {
      this.RequestContext = requestContext;
      this.Response = response;
    }

    public void Dispose()
    {
      if (this.PushResponseStream == null)
        return;
      this.PushResponseStream.Dispose();
      this.PushResponseStream = (Stream) null;
    }

    public HttpResponseMessage Response { get; private set; }

    public Stream PushResponseStream { get; set; }

    public IVssRequestContext RequestContext { get; set; }

    public Stream CacheMiss(
      FileCacheService fileCacheService,
      FileInformation fileInfo,
      bool compressOutput)
    {
      IVssRequestContext requestContext = this.RequestContext;
      Stream stream = (Stream) null;
      try
      {
        byte[] hashValue;
        long contentLength;
        CompressionType compressionType;
        stream = requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) fileInfo.FileId, compressOutput, out hashValue, out contentLength, out compressionType);
        this.RequestContext.Trace(0, TraceLevel.Verbose, RestApiDownloadState.s_area, RestApiDownloadState.s_layer, "File info from server: Length: {0} UnCompressed Length: {1} Compression Type: {2}", (object) contentLength, (object) fileInfo.UncompressedLength, (object) compressionType);
        if (fileInfo.HashValue != null && !ArrayUtil.Equals(fileInfo.HashValue, hashValue))
          throw new DownloadTicketValidationException(FrameworkResources.RequestSignatureValidationFailed());
        fileInfo.HashValue = hashValue;
        fileInfo.ContentType = TeamFoundationFileService.ToMimeType(compressionType);
        fileInfo.Length = stream == null ? 0L : stream.Length;
        if (fileInfo.Length == 0L)
          fileInfo.ContentType = TeamFoundationFileService.ToMimeType(CompressionType.None);
        fileInfo.UncompressedLength = contentLength;
        return stream;
      }
      catch (Exception ex)
      {
        stream?.Dispose();
        throw;
      }
    }

    public bool TransmitChunk(FileInformation fileInfo, byte[] chunk, long offset, long length)
    {
      this.RequestContext.Trace(0, TraceLevel.Verbose, RestApiDownloadState.s_area, RestApiDownloadState.s_layer, "Transmitting:\tchunk for file {0}:{1} sending {2} bytes - {3}/{4} bytes sent", (object) fileInfo.RepositoryGuid, (object) fileInfo.FileId, (object) length, (object) this.m_transmittedLength, (object) fileInfo.Length);
      try
      {
        this.m_transmittedLength += length;
        this.PushResponseStream.Write(chunk, (int) offset, (int) length);
        this.PushResponseStream.Flush();
        return true;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, RestApiDownloadState.s_area, RestApiDownloadState.s_layer, ex);
        throw;
      }
    }

    public bool TransmitFile(FileInformation fileInfo, string path, long offset, long length)
    {
      try
      {
        FileStream content = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        content.Position = offset;
        this.Response.Content = (HttpContent) new StreamContent((Stream) content);
        return true;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, RestApiDownloadState.s_area, RestApiDownloadState.s_layer, ex);
        throw;
      }
    }
  }
}
