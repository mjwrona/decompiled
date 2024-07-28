// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Storage.Odb
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.PackIndex;
using Microsoft.TeamFoundation.Git.Server.Repair;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Git.Server.Storage
{
  internal class Odb : IDisposable
  {
    private readonly OdbId m_id;
    private readonly Lazy<PersistentBitmapProvider> m_bitmapPrv;
    private readonly ITfsGitBlobProvider m_blobProvider;
    private readonly ContentDB m_contentDB;
    private readonly IGitKnownFilesProvider m_knownFilePrv;
    private readonly GitGraphProvider m_graphPrv;
    private readonly ITeamFoundationHostManagementService m_hostManagementSvc;
    private readonly IObjectMetadata m_objectMetadata;
    private readonly GitPackIndexLoader m_packIndexLoader;
    private readonly IGitPackIndexPointerProvider m_packIndexPtrPrv;
    private readonly Func<GitPackIndexTransaction> m_packIndexTranFactory;
    private readonly IVssRegistryService m_regSvc;
    private readonly IVssRequestContext m_requestContext;
    private readonly Lazy<GitOdbSettings> m_settings;
    private bool m_disposed;
    private GitObjectSet m_objectSet;
    private const string s_Layer = "TfsGitRepositoryStorage";

    internal Odb(
      OdbId id,
      Func<PersistentBitmapProvider> bitmapPrvFactory,
      ITfsGitBlobProvider blobProvider,
      ContentDB contentDB,
      GitGraphProvider graphPrv,
      ITeamFoundationHostManagementService hostManagementSvc,
      IObjectMetadata objectMetadata,
      GitObjectSet objectSet,
      GitPackIndexLoader packIndexLoader,
      IGitPackIndexPointerProvider packIndexPointerProvider,
      Func<GitPackIndexTransaction> packIndexTranFactory,
      IVssRegistryService regSvc,
      IVssRequestContext requestContext,
      Lazy<GitOdbSettings> odbSettings,
      IGitKnownFilesProvider knownFilePrv = null)
    {
      id.CheckValid();
      ArgumentUtility.CheckForNull<Func<PersistentBitmapProvider>>(bitmapPrvFactory, nameof (bitmapPrvFactory));
      ArgumentUtility.CheckForNull<ITfsGitBlobProvider>(blobProvider, nameof (blobProvider));
      ArgumentUtility.CheckForNull<ContentDB>(contentDB, nameof (contentDB));
      ArgumentUtility.CheckForNull<ITeamFoundationHostManagementService>(hostManagementSvc, nameof (hostManagementSvc));
      ArgumentUtility.CheckForNull<IObjectMetadata>(objectMetadata, nameof (objectMetadata));
      ArgumentUtility.CheckForNull<GitObjectSet>(objectSet, nameof (objectSet));
      ArgumentUtility.CheckForNull<GitPackIndexLoader>(packIndexLoader, nameof (packIndexLoader));
      ArgumentUtility.CheckForNull<IGitPackIndexPointerProvider>(packIndexPointerProvider, nameof (packIndexPointerProvider));
      ArgumentUtility.CheckForNull<Func<GitPackIndexTransaction>>(packIndexTranFactory, nameof (packIndexTranFactory));
      ArgumentUtility.CheckForNull<IVssRegistryService>(regSvc, nameof (regSvc));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Lazy<GitOdbSettings>>(odbSettings, nameof (odbSettings));
      this.m_id = id;
      this.m_bitmapPrv = new Lazy<PersistentBitmapProvider>(bitmapPrvFactory, LazyThreadSafetyMode.None);
      this.m_blobProvider = blobProvider;
      this.m_contentDB = contentDB;
      this.m_knownFilePrv = knownFilePrv;
      this.m_graphPrv = graphPrv;
      this.m_hostManagementSvc = hostManagementSvc;
      this.m_objectMetadata = objectMetadata;
      this.m_objectSet = objectSet;
      this.m_packIndexLoader = packIndexLoader;
      this.m_packIndexPtrPrv = packIndexPointerProvider;
      this.m_packIndexTranFactory = packIndexTranFactory;
      this.m_regSvc = regSvc;
      this.m_requestContext = requestContext;
      this.m_settings = odbSettings;
    }

    internal IReachabilityBitmapProvider ReachabilityBitmapProvider
    {
      get
      {
        this.EnsureNotDisposed();
        return (IReachabilityBitmapProvider) this.m_bitmapPrv.Value;
      }
    }

    internal GitGraphProvider GraphProvider
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_graphPrv;
      }
    }

    internal OdbId Id => this.m_id;

    internal ICommitReachabilityProvider ReachabilityProvider
    {
      get
      {
        this.EnsureNotDisposed();
        return (ICommitReachabilityProvider) this.m_bitmapPrv.Value;
      }
    }

    public IObjectOrderer ObjectOrdererFactory(bool useExtraResources) => (IObjectOrderer) new BatchedObjectOrderer(this.m_requestContext, this.m_id, useExtraResources);

    public IPackIndexMergeStrategy GetAggressivePackIndexMergeStrategy() => (IPackIndexMergeStrategy) new AggressivePackIndexMergeStrategy(this.m_settings.Value.TipIndexSize, this.m_settings.Value.StagingIndexSize);

    public IPackIndexMergeStrategy GetFastPackIndexMergeStrategy() => (IPackIndexMergeStrategy) new FastPackIndexMergeStrategy(this.m_settings.Value.TipIndexSize);

    public IObjectMetadata ObjectMetadata
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_objectMetadata;
      }
    }

    public GitObjectSet ObjectSet
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_objectSet;
      }
    }

    public GitOdbSettings Settings
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_settings.Value;
      }
    }

    internal IGitPackIndexPointerProvider PackIndexPointerProvider
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_packIndexPtrPrv;
      }
    }

    internal Func<GitPackIndexTransaction> PackIndexTranFactory
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_packIndexTranFactory;
      }
    }

    internal ContentDB ContentDB
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_contentDB;
      }
    }

    internal ITfsGitBlobProvider BlobProvider
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_blobProvider;
      }
    }

    internal GitPackIndexLoader PackIndexLoader
    {
      get
      {
        this.EnsureNotDisposed();
        return this.m_packIndexLoader;
      }
    }

    public void Dispose()
    {
      if (!this.m_disposed)
      {
        if (this.m_bitmapPrv.IsValueCreated)
          this.m_bitmapPrv.Value?.Dispose();
        if (this.m_objectSet != null)
        {
          this.m_objectSet.Dispose();
          this.m_objectSet = (GitObjectSet) null;
        }
      }
      this.m_disposed = true;
    }

    internal void StorePack(
      GitKnownFilesBuilder knownFiles,
      IReadOnlyList<PackRange> splitPacks,
      Stream packSource,
      string packSourceFileName)
    {
      this.EnsureNotDisposed();
      int val1 = this.m_regSvc.GetValue<int>(this.m_requestContext, (RegistryQuery) "/Service/Git/Settings/NumberOfParallelPushThreads", true, 0);
      int parallelPushThreads = val1 != 0 ? Math.Min(val1, splitPacks.Count) : Math.Min(4, splitPacks.Count);
      if (parallelPushThreads == 1)
        this.StorePackSerialPush(knownFiles, splitPacks, packSource);
      else
        this.StorePackParallelPush(knownFiles, splitPacks, parallelPushThreads, packSource, packSourceFileName);
    }

    private void StorePackSerialPush(
      GitKnownFilesBuilder knownFiles,
      IReadOnlyList<PackRange> splitPacks,
      Stream packSource)
    {
      this.EnsureNotDisposed();
      using (this.m_requestContext.TraceBlock(1013102, 1013103, GitServerUtils.TraceArea, "TfsGitRepositoryStorage", nameof (StorePackSerialPush)))
      {
        foreach (PackRange splitPack in (IEnumerable<PackRange>) splitPacks)
        {
          PackRange packData = splitPack;
          knownFiles.QueueExtant(packData.PackName, KnownFileType.RawPackfile);
          this.m_requestContext.TraceConditionally(1013061, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitRepositoryStorage", (Func<string>) (() => string.Format("Storing split pack {0} with length {1}", (object) packData.PackName, (object) packData.Length)));
          this.UploadPack(this.m_requestContext, packData, packSource);
        }
      }
    }

    private void StorePackParallelPush(
      GitKnownFilesBuilder knownFiles,
      IReadOnlyList<PackRange> splitPacks,
      int parallelPushThreads,
      Stream packSource,
      string packSourceFileName)
    {
      this.EnsureNotDisposed();
      int bufferSize = GitStreamUtil.GetBufferSize(packSource);
      this.m_requestContext.Trace(1013577, TraceLevel.Verbose, GitServerUtils.TraceArea, "TfsGitRepositoryStorage", string.Format(" Pushing {0} packs with {1} threads.", (object) splitPacks.Count, (object) parallelPushThreads));
      using (this.m_requestContext.TraceBlock(1013102, 1013103, GitServerUtils.TraceArea, "TfsGitRepositoryStorage", nameof (StorePackParallelPush)))
      {
        ParallelOptions parallelOptions = new ParallelOptions()
        {
          MaxDegreeOfParallelism = parallelPushThreads
        };
        object knownfilesLock = new object();
        ConcurrentQueue<PackRange> splitQueue = new ConcurrentQueue<PackRange>((IEnumerable<PackRange>) splitPacks);
        Parallel.ForEach<int>(Enumerable.Range(0, parallelOptions.MaxDegreeOfParallelism), parallelOptions, (Action<int>) (forThread =>
        {
          using (IVssRequestContext rc = this.m_hostManagementSvc.BeginUserRequest(this.m_requestContext, this.m_requestContext.ServiceHost.InstanceId, this.m_requestContext.UserContext, true))
          {
            using (Stream packSource1 = (Stream) new FileStream(packSourceFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, bufferSize))
            {
              PackRange result;
              while (splitQueue.TryDequeue(out result))
              {
                lock (knownfilesLock)
                  knownFiles.QueueExtant(result.PackName, KnownFileType.RawPackfile);
                this.UploadPack(rc, result, packSource1);
              }
            }
          }
        }));
      }
    }

    private void UploadPack(IVssRequestContext rc, PackRange packData, Stream packSource)
    {
      packSource.Seek(packData.Offset, SeekOrigin.Begin);
      using (BlobProviderChunkingWriterStream chunkingWriterStream = new BlobProviderChunkingWriterStream(rc, this.m_blobProvider, this.m_id, packData.PackName))
      {
        using (WriteBufferStream writeBufferStream = new WriteBufferStream((Stream) chunkingWriterStream, (int) Math.Min(4194304L, GitPackSerializer.CalculatePackSize(packData.Length))))
        {
          using (GitPackSerializer gitPackSerializer = new GitPackSerializer((Stream) writeBufferStream, packData.ObjectCount, true))
          {
            gitPackSerializer.AddMultipleRaw(packSource, 0L, packData.Length, packData.ObjectCount, true);
            gitPackSerializer.Complete();
          }
        }
      }
    }

    internal Sha1Id StoreIndex(
      GitPackIndexTransaction tran,
      GitPackIndexer indexer,
      List<Sha1Id> stableObjectOrder)
    {
      this.EnsureNotDisposed();
      ConcatGitPackIndex newIndex = (ConcatGitPackIndex) null;
      try
      {
        using (this.m_requestContext.TraceBlock(1013101, 1013199, GitServerUtils.TraceArea, "TfsGitRepositoryStorage", nameof (StoreIndex)))
        {
          tran.EnsureIndexLease();
          Sha1Id sha1Id;
          using (ConcatGitPackIndex concatGitPackIndex = this.m_packIndexLoader.LoadIndex(this.m_packIndexPtrPrv.GetIndex()))
          {
            indexer.SetBaseIndex(concatGitPackIndex);
            if (concatGitPackIndex.StableObjectOrderEpoch.HasValue)
              indexer.SetStableObjectOrder(stableObjectOrder);
            else if (concatGitPackIndex.Subindexes.Count == 0)
            {
              indexer.StartStableObjectOrderEpoch(StorageUtils.CreateUniqueId());
              indexer.SetStableObjectOrder(stableObjectOrder);
            }
            sha1Id = GitDataFileUtil.WriteIndex(this.m_requestContext, this.m_blobProvider, this.m_id, tran.KnownFilesBuilder, indexer, this.GetFastPackIndexMergeStrategy());
            newIndex = this.m_packIndexLoader.LoadIndex(new Sha1Id?(sha1Id));
            tran.CommitAndDispose(concatGitPackIndex, newIndex);
          }
          this.m_contentDB.Index = newIndex;
          newIndex = (ConcatGitPackIndex) null;
          return sha1Id;
        }
      }
      finally
      {
        newIndex?.Dispose();
      }
    }

    internal void RepackContent(bool noReuseIndexes, bool noReusePacks)
    {
      this.EnsureNotDisposed();
      new GitRepacker(this.m_requestContext, this, this.m_contentDB.Index, this.m_blobProvider, this.m_contentDB.DataFileProvider, this.m_packIndexLoader, this.m_packIndexPtrPrv, this.m_packIndexTranFactory, noReuseIndexes, noReusePacks, new int?()).Execute();
    }

    internal bool RepackContentBatch(int batchSize)
    {
      this.EnsureNotDisposed();
      return new GitRepacker(this.m_requestContext, this, this.m_contentDB.Index, this.m_blobProvider, this.m_contentDB.DataFileProvider, this.m_packIndexLoader, this.m_packIndexPtrPrv, this.m_packIndexTranFactory, true, false, new int?(batchSize)).Execute();
    }

    internal void FsckContent(out Sha1Id? currentGraphId)
    {
      this.EnsureNotDisposed();
      ClientTraceService service = this.m_requestContext.GetService<ClientTraceService>();
      GitCommitGraph graph;
      this.GraphProvider.TryLoadFromStorage(out graph, out currentGraphId);
      IGitKnownFilesProvider knownFilesProvider = (IGitKnownFilesProvider) new SqlGitKnownFilesProvider(this.m_requestContext, this.Id);
      new GitOdbConsistencyChecker(this.m_requestContext, this, service, (IGitCommitGraph) graph, this.m_contentDB.DataFileProvider, knownFilesProvider).CheckOdb();
    }

    internal void RedeltifyContent()
    {
      this.EnsureNotDisposed();
      if (this.m_contentDB.Index.Entries.Count == 0)
        return;
      new GitRedeltifier(this.m_requestContext, this.m_blobProvider, this.m_id, this, this.m_contentDB, (IGitPackIndex) this.m_contentDB.Index, (DeltaListProvider<TfsGitObjectLocation>) new TfsDeltaListProvider((ITfsGitContentDB<TfsGitObjectLocation>) this.m_contentDB, (IGitPackIndex) this.m_contentDB.Index), this.m_packIndexPtrPrv, this.m_packIndexLoader, this.m_packIndexTranFactory()).Execute();
    }

    public override string ToString() => "ODB: " + this.m_id.ToString();

    internal bool TryRepairContent()
    {
      this.EnsureNotDisposed();
      return this.m_contentDB.Index.Entries.Count != 0 && new GitRepairer(this.m_requestContext, this, this.m_contentDB.Index, this.m_blobProvider, this.m_contentDB.DataFileProvider, this.m_knownFilePrv, this.m_packIndexLoader, this.m_packIndexPtrPrv, this.m_packIndexTranFactory).TryRepair();
    }

    private void EnsureNotDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(nameof (Odb));
    }
  }
}
