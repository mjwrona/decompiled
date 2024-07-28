// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DownloadState
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class DownloadState : IDownloadState<FileInformation>, IDisposable
  {
    private const string c_area = "FileCache";
    private const string c_layer = "DownloadState";

    public DownloadState(HttpResponseBase response)
    {
      this.Response = response;
      this.NeedsHeaders = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      try
      {
        if (this.Response == null || this.NeedsHeaders || !this.Response.IsClientConnected)
          return;
        this.Response.Flush();
      }
      catch (HttpException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(12223, "FileCache", nameof (DownloadState), (Exception) ex);
      }
    }

    public virtual void Dispose(bool isDisposing)
    {
    }

    private void SetHeaders(FileInformation fileInformation)
    {
      if (!this.NeedsHeaders)
        return;
      this.NeedsHeaders = false;
      if (!this.Response.IsClientConnected)
        return;
      this.Response.BufferOutput = true;
      this.Response.ContentType = fileInformation.ContentType;
      this.Response.CacheControl = "no-cache";
      this.Response.AppendHeader("Content-Length", fileInformation.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fileInformation.Length != 0L)
        this.Response.AppendHeader("Uncompressed-Content-Length", fileInformation.UncompressedLength.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (fileInformation.HashValueString == null)
        return;
      this.Response.AppendHeader("File-MD5", fileInformation.HashValueString);
    }

    public abstract Stream CacheMiss(
      FileCacheService fileCacheService,
      FileInformation fileInfo,
      bool compressOutput);

    public virtual bool TransmitChunk(
      FileInformation fileInformation,
      byte[] chunk,
      long offset,
      long length)
    {
      TeamFoundationTracingService.TraceRaw(12220, TraceLevel.Info, "FileCache", nameof (DownloadState), "Transmitting:\tchunk for file {0}:{1} sending {2} bytes - {3}/{4} bytes sent", (object) fileInformation.RepositoryGuid, (object) fileInformation.FileId, (object) length, (object) this.TransmittedLength, (object) fileInformation.Length);
      try
      {
        this.SetHeaders(fileInformation);
        if (this.Response.IsClientConnected)
        {
          this.TransmittedLength += length;
          this.Response.OutputStream.Write(chunk, (int) offset, (int) length);
          this.Response.Flush();
          return true;
        }
      }
      catch (HttpException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(12225, "FileCache", nameof (DownloadState), (Exception) ex);
      }
      return false;
    }

    public virtual bool TransmitFile(
      FileInformation fileInformation,
      string path,
      long offset,
      long length)
    {
      bool flag = false;
      try
      {
        this.SetHeaders(fileInformation);
        if (this.Response.IsClientConnected)
        {
          this.Response.TransmitFile(path, offset, length);
          this.Response.Flush();
        }
        flag = true;
      }
      catch (HttpException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(12217, "FileCache", nameof (DownloadState), (Exception) ex);
      }
      return flag;
    }

    private HttpResponseBase Response { get; set; }

    private bool NeedsHeaders { get; set; }

    private long TransmittedLength { get; set; }
  }
}
