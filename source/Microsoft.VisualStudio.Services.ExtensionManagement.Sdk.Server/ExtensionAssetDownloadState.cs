// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionAssetDownloadState
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Web;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ExtensionAssetDownloadState : MidTierDownloadState
  {
    private const string c_area = "ExtensionAssetDownloadState";
    private const string c_layer = "Extensions";

    public ExtensionAssetDownloadState(IVssRequestContext requestContext)
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
      try
      {
        this.FileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        this.FileStream.Seek(offset, SeekOrigin.Begin);
        this.HashValue = fileInformation.HashValue;
        this.UncompressedLength = fileInformation.UncompressedLength;
        return true;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(10013670, nameof (ExtensionAssetDownloadState), "Extensions", ex);
        if (this.FileStream != null)
          this.FileStream.Dispose();
        throw;
      }
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
