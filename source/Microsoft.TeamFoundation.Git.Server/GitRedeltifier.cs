// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRedeltifier
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Native;
using Microsoft.TeamFoundation.Git.Server.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitRedeltifier
  {
    private readonly IVssRequestContext m_rc;
    private readonly ITfsGitBlobProvider m_blobProvider;
    private readonly OdbId m_odbId;
    private readonly Odb m_repoStorage;
    private readonly ContentDB m_contentDB;
    private readonly IGitPackIndex m_oldIndex;
    private readonly DeltaListProvider<TfsGitObjectLocation> m_baseAndDeltaProvider;
    private readonly IGitPackIndexPointerProvider m_packIndexPtrPrv;
    private readonly GitPackIndexLoader m_packIndexLoader;
    private readonly GitPackIndexTransaction m_packIndexTran;
    private readonly string m_tempDir;
    private const string c_layer = "GitRedeltifier";

    public GitRedeltifier(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      Odb repoStorage,
      ContentDB contentDB,
      IGitPackIndex index,
      DeltaListProvider<TfsGitObjectLocation> baseAndDeltaProvider,
      IGitPackIndexPointerProvider packIndexPointerProvider,
      GitPackIndexLoader packIndexLoader,
      GitPackIndexTransaction packIndexTran)
    {
      this.m_rc = rc;
      this.m_blobProvider = blobProvider;
      this.m_odbId = odbId;
      this.m_repoStorage = repoStorage;
      this.m_contentDB = contentDB;
      this.m_oldIndex = index;
      this.m_baseAndDeltaProvider = baseAndDeltaProvider;
      this.m_packIndexPtrPrv = packIndexPointerProvider;
      this.m_packIndexLoader = packIndexLoader;
      this.m_packIndexTran = packIndexTran;
      this.m_tempDir = GitServerUtils.GetCacheDirectory(this.m_rc, this.m_odbId.Value);
    }

    public void Execute()
    {
      string str = Path.Combine(this.m_tempDir, "redelt");
      if (Directory.Exists(str))
        GitRedeltifier.DeleteReadOnlyFolder(str);
      DirectoryInfo directory = Directory.CreateDirectory(str);
      try
      {
        Repository.Init(directory.FullName, true);
        string packDir = Path.Combine(directory.FullName, "objects", "pack");
        string redeltifiedPack;
        using (TfsGitOdbBackend backend = new TfsGitOdbBackend(this.m_rc, directory, this.m_contentDB))
        {
          using (Repository repo = new Repository(directory.FullName))
          {
            repo.ObjectDatabase.AddBackend((OdbBackend) backend, int.MaxValue);
            this.Trace(1020000, "Starting redeltification of ODB {0}", (object) this.m_odbId);
            IReadOnlyDictionary<Sha1Id, IList<Sha1Id>> baseAndDeltas = this.m_baseAndDeltaProvider.BuildBasesAndDeltas((IEnumerable<Sha1Id>) this.m_oldIndex.ObjectIds);
            this.Trace(1020001, "Completed index traversal.");
            redeltifiedPack = this.BuildNewPackfile(repo, packDir, baseAndDeltas);
            this.Trace(1020004, "Wrote new packfile {0}", (object) redeltifiedPack);
          }
        }
        this.ReplacePackIndexedObjects(this.StoreRedeltifiedPack(redeltifiedPack));
      }
      finally
      {
        try
        {
          GitRedeltifier.DeleteReadOnlyFolder(directory.FullName);
        }
        catch (Exception ex)
        {
          this.m_rc.TraceCatch(1020007, GitServerUtils.TraceArea, nameof (GitRedeltifier), ex);
        }
      }
    }

    private static void DeleteReadOnlyFolder(string folderPath)
    {
      foreach (string enumerateFile in Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories))
      {
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.SetFileAttributes(enumerateFile, 128U);
        Microsoft.TeamFoundation.Common.Internal.NativeMethods.DeleteFile(enumerateFile);
      }
      Directory.Delete(folderPath, true);
    }

    private int GetParallePackBuilderThreads(int numEntries)
    {
      int val1 = this.m_rc.GetService<IVssRegistryService>().GetValue<int>(this.m_rc, (RegistryQuery) "/Service/Git/Settings/NumParallelGitPackerThreads", true, 0);
      return val1 != 0 ? Math.Min(val1, numEntries) : Math.Min(4, numEntries);
    }

    private string BuildNewPackfile(
      Repository repo,
      string packDir,
      IReadOnlyDictionary<Sha1Id, IList<Sha1Id>> baseAndDeltas)
    {
      int insertedObjectsCount = 0;
      int num = 1;
      if (this.m_rc.IsFeatureEnabled("Git.EnableParallelPackBuilder"))
        num = this.GetParallePackBuilderThreads(baseAndDeltas.Count);
      ObjectDatabase objectDatabase = repo.ObjectDatabase;
      PackBuilderOptions options = new PackBuilderOptions(packDir);
      options.MaximumNumberOfThreads = num;
      Action<PackBuilder> packDelegate = (Action<PackBuilder>) (builder =>
      {
        foreach (KeyValuePair<Sha1Id, IList<Sha1Id>> baseAndDelta in (IEnumerable<KeyValuePair<Sha1Id, IList<Sha1Id>>>) baseAndDeltas)
        {
          builder.Add(new ObjectId(baseAndDelta.Key.ToByteArray()));
          foreach (Sha1Id sha1Id in (IEnumerable<Sha1Id>) baseAndDelta.Value)
            builder.Add(new ObjectId(sha1Id.ToByteArray()));
          if (++insertedObjectsCount % 10000 == 0)
            this.Trace(1020002, "Inserted {0} objects into the packbuilder.", (object) insertedObjectsCount);
        }
        this.Trace(1020003, "Inserted all {0} objects into packbuilder.", (object) insertedObjectsCount);
      });
      objectDatabase.Pack(options, packDelegate);
      return Directory.EnumerateFileSystemEntries(packDir, "*.pack", SearchOption.TopDirectoryOnly).Single<string>();
    }

    private GitPackIndexer StoreRedeltifiedPack(string redeltifiedPack)
    {
      using (FileStream packSource = new FileStream(redeltifiedPack, FileMode.Open, System.IO.FileAccess.Read))
      {
        GitPackDeserializer forOdbFsck1 = GitPackDeserializer.CreateForOdbFsck(this.m_rc, this.m_repoStorage, (Stream) packSource, true);
        GitPackDeserializerObjectParserTrait forOdbFsck2 = GitPackDeserializerObjectParserTrait.CreateForOdbFsck();
        forOdbFsck1.AddTrait((IGitPackDeserializerTrait) forOdbFsck2);
        GitPackDeserializerSplitterTrait trait = new GitPackDeserializerSplitterTrait(this.m_repoStorage.Settings.StablePackfileCapSize);
        forOdbFsck1.AddTrait((IGitPackDeserializerTrait) trait);
        forOdbFsck1.Deserialize();
        if (forOdbFsck2.TotalObjectCount != this.m_oldIndex.Entries.Count)
          throw new InvalidDataException("Deserializer did not see as many objects as the old index contained.");
        this.Trace(1020005, "Deserialized packfile and index.");
        SplitPackResult result = trait.Result;
        this.m_repoStorage.StorePack(this.m_packIndexTran.KnownFilesBuilder, result.SplitPacks, (Stream) packSource, packSource.Name);
        this.Trace(1020006, "Stored redeltified packfile and index.");
        return result.Indexer;
      }
    }

    private void ReplacePackIndexedObjects(GitPackIndexer packIndexer)
    {
      try
      {
        this.m_packIndexTran.EnsureIndexLease();
        Sha1Id sha1Id = this.m_packIndexPtrPrv.GetIndex().Value;
        using (this.TraceBlock(1020008, 1020009, nameof (ReplacePackIndexedObjects)))
        {
          using (ConcatGitPackIndex concatGitPackIndex = this.m_packIndexLoader.LoadIndex(new Sha1Id?(sha1Id)))
          {
            packIndexer.SetBaseIndex(concatGitPackIndex.GetRange(0, 0));
            packIndexer.AddFromIndex(concatGitPackIndex);
            packIndexer.PreserveStableObjectOrderIfCompatible((IGitPackIndex) concatGitPackIndex);
            using (ConcatGitPackIndex newIndex = this.m_packIndexLoader.LoadIndex(new Sha1Id?(GitDataFileUtil.WriteIndex(this.m_rc, this.m_blobProvider, this.m_odbId, this.m_packIndexTran.KnownFilesBuilder, packIndexer, PackIndexMergeStrategy.ForceFull))))
              this.m_packIndexTran.CommitAndDispose(concatGitPackIndex, newIndex);
          }
        }
      }
      finally
      {
        this.m_packIndexTran.TryExpirePendingExtantAndDispose();
      }
    }

    private void Trace(int tracepoint, string message, params object[] args) => VssRequestContextExtensions.Trace(this.m_rc, tracepoint, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitRedeltifier), message, args);

    private IDisposable TraceBlock(int begin, int end, [CallerMemberName] string method = null) => this.m_rc.TraceBlock(begin, end, GitServerUtils.TraceArea, nameof (GitRedeltifier), method);
  }
}
