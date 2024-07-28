// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.RepoBitmapFileProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class RepoBitmapFileProvider : IRepoBitmapFileProvider
  {
    private readonly IVssRequestContext m_rc;
    private readonly RepoKey m_repoKey;
    private readonly ITfsGitBlobProvider m_blobPrv;
    private readonly IGitDataFileProvider m_dataFilePrv;
    private readonly IGitKnownFilesProvider m_knownFilesPrv;
    private readonly ILeaseService m_leaseSvc;
    private const string c_layer = "RepoBitmapFileProvider";
    private static readonly TimeSpan s_leaseTime = new TimeSpan(0, 0, 60);
    private static readonly TimeSpan s_leaseWaitTime = new TimeSpan(0, 5, 0);

    public RepoBitmapFileProvider(
      IVssRequestContext rc,
      RepoKey repoKey,
      ITfsGitBlobProvider blobPrv,
      IGitDataFileProvider dataFilePrv,
      IGitKnownFilesProvider knownFilesPrv,
      ILeaseService leaseSvc)
    {
      this.m_rc = rc;
      this.m_repoKey = repoKey;
      this.m_blobPrv = blobPrv;
      this.m_dataFilePrv = dataFilePrv;
      this.m_knownFilesPrv = knownFilesPrv;
      this.m_leaseSvc = leaseSvc;
    }

    public RoaringBitmap<T> GetBitmap<T>(
      IsolationBitmapType type,
      Guid fileId,
      ITwoWayReadOnlyList<T> objectList)
    {
      KnownFileType knownFileType = type.ToKnownFileType();
      string repoFileName = StorageUtils.GetRepoFileName(fileId, knownFileType);
      using (ByteArray byteArray = new ByteArray(GitStreamUtil.OptimalBufferSize))
      {
        using (Stream stream = this.m_dataFilePrv.GetStream(repoFileName))
          return new RoaringBitmap<T>.M123RiffFormat(objectList).Read(stream, byteArray.Bytes);
      }
    }

    public Guid? GetFileId(IsolationBitmapType type)
    {
      using (GitCoreComponent gitCoreComponent = this.m_rc.CreateGitCoreComponent())
        return gitCoreComponent.ReadPointer(this.m_repoKey, type.ToRepoPointerType());
    }

    public Guid? Update<T>(IsolationBitmapType type, RoaringBitmap<T> bitmap, Guid? existingFileId)
    {
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      Stopwatch stopwatch2 = Stopwatch.StartNew();
      RepoBitmapFileProvider.Statistics statistics = new RepoBitmapFileProvider.Statistics();
      try
      {
        using (this.m_leaseSvc.AcquireLease(this.m_rc, string.Format("{0}.Isolation.{1}", (object) this.m_repoKey, (object) type), RepoBitmapFileProvider.s_leaseTime, RepoBitmapFileProvider.s_leaseWaitTime, true))
        {
          statistics.ObtainLeaseMs = stopwatch2.ReadElapsedMsAndRestart();
          Guid? fileId = this.GetFileId(type);
          Guid? nullable1 = fileId;
          Guid? nullable2 = existingFileId;
          if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
          {
            if (!fileId.HasValue && existingFileId.HasValue)
              throw new ArgumentException(nameof (existingFileId));
            bitmap = RoaringBitmapCombiner<T>.Union(this.GetBitmap<T>(type, fileId.Value, bitmap.FullObjectList), bitmap);
          }
          statistics.LoadBitmapMs = stopwatch2.ReadElapsedMsAndRestart();
          GitKnownFilesBuilder knownFiles = new GitKnownFilesBuilder();
          Guid guid = GitDataFileUtil.WriteBitmap<T>(this.m_rc, this.m_blobPrv, this.m_repoKey.OdbId, knownFiles, type, bitmap);
          statistics.WriteBitmapMs = stopwatch2.ReadElapsedMsAndRestart();
          this.m_knownFilesPrv.Update(knownFiles.GetCreates());
          statistics.InsertKnownFileMs = stopwatch2.ReadElapsedMsAndRestart();
          Guid? nullable3;
          using (GitCoreComponent gitCoreComponent = this.m_rc.CreateGitCoreComponent())
            nullable3 = gitCoreComponent.UpdatePointer(this.m_repoKey, type.ToRepoPointerType(), fileId, new Guid?(guid));
          statistics.UpdatePointerMs = stopwatch2.ReadElapsedMsAndRestart();
          KnownFileType knownFileType = type.ToKnownFileType();
          HashSet<string> filesToDelete = new HashSet<string>();
          if (existingFileId.HasValue)
            filesToDelete.Add(StorageUtils.GetRepoFileName(existingFileId.Value, knownFileType));
          if (fileId.HasValue)
            filesToDelete.Add(StorageUtils.GetRepoFileName(fileId.Value, knownFileType));
          this.m_knownFilesPrv.Delete((IEnumerable<string>) filesToDelete);
          statistics.DeleteOldKnownFileMs = stopwatch2.ReadElapsedMsAndRestart();
          return nullable3.Equals((object) guid) ? nullable3 : throw new InvalidOperationException();
        }
      }
      finally
      {
        TimeSpan timeSpan = stopwatch1.Elapsed;
        double totalSeconds = timeSpan.TotalSeconds;
        timeSpan = RepoBitmapFileProvider.s_leaseTime;
        double num = timeSpan.TotalSeconds / 2.0;
        if (totalSeconds > num)
          this.m_rc.TraceAlways(1013787, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (RepoBitmapFileProvider), JsonConvert.SerializeObject((object) statistics));
      }
    }

    private struct Statistics
    {
      public long ObtainLeaseMs { get; set; }

      public long LoadBitmapMs { get; set; }

      public long WriteBitmapMs { get; set; }

      public long InsertKnownFileMs { get; set; }

      public long DeleteOldKnownFileMs { get; set; }

      public long UpdatePointerMs { get; set; }
    }
  }
}
