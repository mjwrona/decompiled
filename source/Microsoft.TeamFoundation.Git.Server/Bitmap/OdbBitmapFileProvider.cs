// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.OdbBitmapFileProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class OdbBitmapFileProvider : IOdbBitmapFileProvider
  {
    private readonly IVssRequestContext m_rc;
    private readonly OdbId m_odbId;
    private readonly ITfsGitBlobProvider m_blobPrv;
    private readonly IGitDataFileProvider m_dataFilePrv;
    private readonly IGitKnownFilesProvider m_knownFilesPrv;

    public OdbBitmapFileProvider(
      IVssRequestContext rc,
      OdbId odbId,
      ITfsGitBlobProvider blobPrv,
      IGitDataFileProvider dataFilePrv,
      IGitKnownFilesProvider knownFilesPrv)
    {
      this.m_rc = rc;
      this.m_odbId = odbId;
      this.m_blobPrv = blobPrv;
      this.m_dataFilePrv = dataFilePrv;
      this.m_knownFilesPrv = knownFilesPrv;
    }

    public Stream GetStream(Sha1Id fileId) => this.m_dataFilePrv.GetStream(StorageUtils.GetOdbFileName(fileId, KnownFileType.ReachabilityBitmapCollection));

    public Sha1Id? GetReachabilityBitmapFileId()
    {
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId))
        return gitOdbComponent.ReadPointer(OdbPointerType.ReachabilityBitmap);
    }

    public void SerializeUpdatedCollection(
      ReachabilityBitmapCollection newCollection,
      Sha1Id? existingFileId)
    {
      GitKnownFilesBuilder knownFiles = new GitKnownFilesBuilder();
      Sha1Id sha1Id = GitDataFileUtil.WriteBitmapCollection(this.m_rc, this.m_blobPrv, this.m_odbId, knownFiles, newCollection);
      this.m_knownFilesPrv.Update(knownFiles.GetCreates());
      Sha1Id? nullable;
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId))
        nullable = gitOdbComponent.UpdatePointer(OdbPointerType.ReachabilityBitmap, existingFileId, new Sha1Id?(sha1Id));
      if (nullable.Equals((object) sha1Id))
        return;
      this.m_rc.Trace(1013661, TraceLevel.Error, GitServerUtils.TraceArea, nameof (OdbBitmapFileProvider), "Compare and swap on bitmap pointer failed. {0}: {1}.", (object) "resultFileId", (object) nullable);
    }
  }
}
