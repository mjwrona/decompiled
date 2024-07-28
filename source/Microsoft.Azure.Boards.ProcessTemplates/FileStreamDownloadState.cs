// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.FileStreamDownloadState
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.IO;
using System.Web;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class FileStreamDownloadState : MidTierDownloadState
  {
    public FileStreamDownloadState(IVssRequestContext requestContext)
      : base((HttpResponseBase) null, requestContext)
    {
      this.FileStream = (FileStream) null;
    }

    public override bool TransmitFile(
      FileInformation fileInformation,
      string path,
      long offset,
      long length)
    {
      this.FileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      this.FileStream.Seek(offset, SeekOrigin.Begin);
      this.HashValue = fileInformation.HashValue;
      this.UncompressedLength = fileInformation.UncompressedLength;
      return true;
    }

    public override bool TransmitChunk(
      FileInformation fileInformation,
      byte[] chunk,
      long offset,
      long length)
    {
      CompressionType compressionType = CompressionType.None;
      if (this.FileStream == null)
      {
        using (MemoryStream memoryStream = new MemoryStream(chunk, (int) offset, (int) length))
          this.FileStream = this.RequestContext.GetService<TeamFoundationFileService>().CopyStreamToTempFile(this.RequestContext, (Stream) memoryStream, ref compressionType, false);
        this.HashValue = fileInformation.HashValue;
        this.UncompressedLength = fileInformation.UncompressedLength;
      }
      else
      {
        this.FileStream.Seek(0L, SeekOrigin.End);
        this.FileStream.Write(chunk, (int) offset, (int) length);
      }
      this.FileStream.Seek(0L, SeekOrigin.Begin);
      return true;
    }

    public FileStream FileStream { get; private set; }

    public byte[] HashValue { get; private set; }

    public long UncompressedLength { get; private set; }
  }
}
