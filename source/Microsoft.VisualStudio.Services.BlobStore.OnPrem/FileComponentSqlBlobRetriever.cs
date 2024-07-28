// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.FileComponentSqlBlobRetriever
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  internal class FileComponentSqlBlobRetriever : ISqlBlobRetriever
  {
    private Func<FileComponent> fileComponentFactory;

    public bool ReadOnce => false;

    public FileComponentSqlBlobRetriever(Func<FileComponent> fileComponentFactory) => this.fileComponentFactory = fileComponentFactory;

    public FileStatistics GetFileStatistics(IVssRequestContext reqContext, int fileId)
    {
      FileIdentifier fileId1 = new FileIdentifier((long) fileId);
      using (FileComponent fileComponent = this.fileComponentFactory())
      {
        TeamFoundationFile metadata = fileComponent.RetrieveFile((ObjectBinder<TeamFoundationFile>) new DataspaceAgnosticFileBinder(), fileId1, false).Metadata;
        if (metadata == null)
          return (FileStatistics) null;
        return new FileStatistics()
        {
          FileId = (long) fileId,
          ResourceId = metadata.Metadata.ResourceId,
          ContentId = metadata.Metadata.ContentId,
          ContentType = metadata.Metadata.ContentType,
          CompressionType = metadata.Metadata.CompressionType,
          HashValue = metadata.Metadata.HashValue,
          FileLength = metadata.Metadata.FileLength,
          CompressedLength = metadata.Metadata.CompressedLength
        };
      }
    }

    public StreamFactory RetrieveFileStreamFactory(IVssRequestContext reqContext, int fileId) => new StreamFactory()
    {
      IsCached = false,
      Creator = (StreamCreator) (() =>
      {
        FileIdentifier fileId1 = new FileIdentifier((long) fileId);
        using (FileComponent fileComponent = this.fileComponentFactory())
        {
          TeamFoundationFileSet foundationFileSet = fileComponent.RetrieveFile((ObjectBinder<TeamFoundationFile>) new DataspaceAgnosticFileBinder(), fileId1);
          using (Stream content = foundationFileSet?.FullVersion?.Content)
          {
            Stream tempStream = this.GetTempStream(foundationFileSet.Metadata.Metadata.FileLength);
            content.CopyTo(tempStream);
            tempStream.Position = 0L;
            return tempStream;
          }
        }
      })
    };

    private Stream GetTempStream(long size) => (Stream) new PooledMemoryStream((int) size, BlobStoreBufferPoolsProvider.Instance);
  }
}
