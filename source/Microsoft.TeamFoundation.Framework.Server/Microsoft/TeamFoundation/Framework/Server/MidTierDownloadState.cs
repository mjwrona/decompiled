// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MidTierDownloadState
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MidTierDownloadState : DownloadState
  {
    private static readonly string s_area = "FileCache";
    private static readonly string s_layer = "MidTierFileCache";

    public MidTierDownloadState(HttpResponseBase response, IVssRequestContext requestContext)
      : base(response)
    {
      this.RequestContext = requestContext;
    }

    public override void Dispose(bool isDisposing)
    {
    }

    public IVssRequestContext RequestContext { get; private set; }

    public override bool TransmitChunk(
      FileInformation fileInformation,
      byte[] chunk,
      long offset,
      long length)
    {
      this.RequestContext.UpdateTimeToFirstPage();
      return base.TransmitChunk(fileInformation, chunk, offset, length);
    }

    public override bool TransmitFile(
      FileInformation fileInformation,
      string path,
      long offset,
      long length)
    {
      this.RequestContext.UpdateTimeToFirstPage();
      return base.TransmitFile(fileInformation, path, offset, length);
    }

    public override Stream CacheMiss(
      FileCacheService fileCacheService,
      FileInformation fileInfo,
      bool compressOutput)
    {
      IVssRequestContext requestContext = this.RequestContext;
      Stream stream = (Stream) null;
      ArgumentUtility.CheckForNull<FileInformation>(fileInfo, "Expecting non null fileInfo");
      try
      {
        byte[] hashValue;
        long contentLength;
        CompressionType compressionType;
        stream = requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) fileInfo.FileId, compressOutput, out hashValue, out contentLength, out compressionType);
        this.RequestContext.Trace(0, TraceLevel.Info, MidTierDownloadState.s_area, MidTierDownloadState.s_layer, "File info from server: Length: {0} UnCompressed Length: {1} Compression Type: {2}", (object) contentLength, (object) fileInfo.UncompressedLength, (object) compressionType);
        if (fileInfo.HashValue != null && !ArrayUtil.Equals(fileInfo.HashValue, hashValue))
        {
          this.RequestContext.Trace(0, TraceLevel.Error, MidTierDownloadState.s_area, MidTierDownloadState.s_layer, "Hash in DB:{0} does not match hash provided by client:{1} for file Id:{2}", (object) FileCacheHelper.StringFromByteArray(hashValue), (object) FileCacheHelper.StringFromByteArray(fileInfo.HashValue), (object) fileInfo.FileId);
          throw new DownloadTicketValidationException(FrameworkResources.RequestSignatureValidationFailed());
        }
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
        this.RequestContext.TraceException(0, MidTierDownloadState.s_area, MidTierDownloadState.s_layer, ex);
        stream?.Dispose();
        throw;
      }
    }
  }
}
