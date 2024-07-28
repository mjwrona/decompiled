// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.OnPrem.FileServiceSqlBlobRetriever
// Assembly: Microsoft.VisualStudio.Services.BlobStore.OnPrem, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EA52CF3A-8E8F-49A1-8A12-783B16F9478A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.OnPrem.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.IO;

namespace Microsoft.VisualStudio.Services.BlobStore.OnPrem
{
  internal class FileServiceSqlBlobRetriever : ISqlBlobRetriever
  {
    private ITeamFoundationFileService fileService;

    public bool ReadOnce => true;

    public FileServiceSqlBlobRetriever(IVssRequestContext reqContext) => this.fileService = reqContext.GetService<ITeamFoundationFileService>();

    public FileStatistics GetFileStatistics(IVssRequestContext reqContext, int fileId) => this.fileService.GetFileStatistics(reqContext, (long) fileId);

    public StreamFactory RetrieveFileStreamFactory(IVssRequestContext reqContext, int fileId)
    {
      Stream stream = SqlBlobProvider.RetrieveFileStream(reqContext, this.fileService, fileId);
      return new StreamFactory()
      {
        IsCached = true,
        Creator = (StreamCreator) (() => stream)
      };
    }
  }
}
