// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupManifestArtifactClient
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.DataDeduplication.Interop;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common.Telemetry;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Telemetry;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [CLSCompliant(false)]
  public class DedupManifestArtifactClient : IDedupManifestArtifactClient, IDisposable
  {
    private const string EnvironmentVariablePrefix = "VSO_BUILD_DROP_";
    public const string BypassEnvironmentVariable = "VSO_BUILD_DROP_BYPASS_RELATIVE_PATH_REQUIREMENT";
    private static readonly bool BypassRelativePathRequirement;
    private readonly IBlobStoreClientTelemetry ClientTelemetry;
    private readonly bool disposeTelemetry;
    private readonly IDedupStoreClientWithDataport client;
    private readonly IAppTraceSource tracer;
    private readonly IFileSystem fileSystem;
    private readonly TimeSpan DefaultKeepUntilDuration = TimeSpan.FromDays(2.0);
    private static readonly EdgeCache edgeCache;
    private static readonly bool isWindows;
    private bool disposedValue;

    static DedupManifestArtifactClient()
    {
      bool result;
      DedupManifestArtifactClient.BypassRelativePathRequirement = bool.TryParse(Environment.GetEnvironmentVariable("VSO_BUILD_DROP_BYPASS_RELATIVE_PATH_REQUIREMENT"), out result) & result;
      DedupManifestArtifactClient.isWindows = Helpers.IsWindowsPlatform(Environment.OSVersion);
      DedupManifestArtifactClient.edgeCache = EdgeCacheHelper.GetEdgeCacheEnvVar("VSO_BUILD_DROP_");
    }

    public DedupManifestArtifactClient(IDedupStoreClientWithDataport client, IAppTraceSource tracer)
      : this(client, tracer, (IFileSystem) FileSystem.Instance)
    {
    }

    public DedupManifestArtifactClient(
      IBlobStoreClientTelemetry blobStoreClientTelemetry,
      IDedupStoreClientWithDataport client,
      IAppTraceSource tracer)
      : this(blobStoreClientTelemetry, client, tracer, (IFileSystem) FileSystem.Instance)
    {
    }

    internal DedupManifestArtifactClient(
      IDedupStoreClientWithDataport client,
      IAppTraceSource tracer,
      IFileSystem fileSystem)
      : this((IBlobStoreClientTelemetry) new BlobStoreClientTelemetry(tracer, client.Client.BaseAddress), client, tracer, fileSystem)
    {
      this.disposeTelemetry = true;
    }

    internal DedupManifestArtifactClient(
      IBlobStoreClientTelemetry blobStoreClientTelemetry,
      IDedupStoreClientWithDataport client,
      IAppTraceSource tracer,
      IFileSystem fileSystem)
    {
      this.ClientTelemetry = (IBlobStoreClientTelemetry) ((object) blobStoreClientTelemetry ?? (object) NoOpBlobStoreClientTelemetry.Instance);
      this.client = client;
      this.tracer = tracer;
      this.fileSystem = fileSystem;
      this.HashType = client.HashType;
    }

    public TimeSpan StatsInterval { get; set; } = TimeSpan.FromSeconds(5.0);

    public HashType HashType { get; }

    public Task<PublishResult> PublishAsync(string fullPath, CancellationToken cancellationToken) => this.PublishAsync(fullPath, new ArtifactPublishOptions(), cancellationToken);

    public Task<PublishResult> PublishAsync(
      string fullPath,
      ArtifactPublishOptions artifactPublishOptions,
      CancellationToken cancellationToken)
    {
      return this.PublishAsync(fullPath, artifactPublishOptions, (string) null, cancellationToken);
    }

    public Task<PublishResult> PublishAsync(
      string fullPath,
      ArtifactPublishOptions artifactPublishOptions,
      string manifestFileOutputPath,
      CancellationToken cancellationToken)
    {
      if (!this.fileSystem.DirectoryExists(fullPath) && !this.fileSystem.FileExists(fullPath))
        throw new InvalidPathException(Resources.InvalidPath());
      List<FileInfo> fileInfoList;
      string sourceDirectory;
      if (this.fileSystem.FileExists(fullPath))
      {
        fileInfoList = new List<FileInfo>()
        {
          new FileInfo(fullPath)
        };
        sourceDirectory = Path.GetDirectoryName(fullPath);
      }
      else
      {
        sourceDirectory = fullPath;
        fileInfoList = (List<FileInfo>) null;
      }
      return this.PublishAsync(sourceDirectory, (IReadOnlyCollection<FileInfo>) fileInfoList, (IReadOnlyDictionary<string, FileInfo>) null, artifactPublishOptions, manifestFileOutputPath, cancellationToken);
    }

    public async Task<PublishResult> PublishAsync(
      string sourceDirectory,
      IReadOnlyCollection<FileInfo> fileInfoList,
      IReadOnlyDictionary<string, FileInfo> otherFiles,
      ArtifactPublishOptions artifactPublishOptions,
      string manifestFileOutputPath,
      CancellationToken cancellationToken)
    {
      this.Trace_X_TFS_SessionId();
      TempFile temporaryManifest = (TempFile) null;
      if (manifestFileOutputPath == null)
      {
        temporaryManifest = this.fileSystem.GetTempFileFullPath();
        manifestFileOutputPath = temporaryManifest.Path;
      }
      PublishResult publishResult;
      using (CancellationTokenSource statsCancellationSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
      {
        try
        {
          IDedupUploadSession dedupUploadSession = this.client.CreateUploadSession(new KeepUntilBlobReference(DateTime.UtcNow.Add(this.DefaultKeepUntilDuration)), this.tracer, (IFileSystem) FileSystem.Instance);
          ConcurrentDictionary<DedupIdentifier, DedupManifestArtifactClient.ManifestFile> allFileHashes = new ConcurrentDictionary<DedupIdentifier, DedupManifestArtifactClient.ManifestFile>();
          ConcurrentBag<DedupManifestArtifactClient.ManifestFile> manifestItems = new ConcurrentBag<DedupManifestArtifactClient.ManifestFile>();
          ConcurrentBag<DedupNode> innerNodes = new ConcurrentBag<DedupNode>();
          long bytesHashed = 0;
          GroupingDataflowBlockOptions dataflowBlockOptions1 = new GroupingDataflowBlockOptions();
          dataflowBlockOptions1.CancellationToken = cancellationToken;
          BatchBlock<DedupNode> batcher = new BatchBlock<DedupNode>(512, dataflowBlockOptions1);
          int activeUploads = 0;
          Func<DedupNode, string> func;
          Func<DedupNode[], Task> action1 = (Func<DedupNode[], Task>) (async fileNodes =>
          {
            DedupNode node = new DedupNode((IEnumerable<DedupNode>) fileNodes);
            innerNodes.Add(node);
            try
            {
              Interlocked.Increment(ref activeUploads);
              KeepUntilReceipt keepUntilReceipt = await dedupUploadSession.UploadAsync(node, (IReadOnlyDictionary<DedupIdentifier, string>) ((IEnumerable<DedupNode>) fileNodes).ToDictionaryAnyWins<DedupNode, DedupIdentifier, string>((Func<DedupNode, DedupIdentifier>) (f => f.GetDedupIdentifier()), func ?? (func = (Func<DedupNode, string>) (f => allFileHashes[f.GetDedupIdentifier()].LocalFileSystemAbsolutePath))), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
              this.tracer.Warn(ex, "Upload failure.");
              ex.ReThrow();
              throw new InvalidOperationException("unreachable.");
            }
            finally
            {
              Interlocked.Decrement(ref activeUploads);
            }
          });
          ExecutionDataflowBlockOptions dataflowBlockOptions2 = new ExecutionDataflowBlockOptions();
          dataflowBlockOptions2.CancellationToken = cancellationToken;
          dataflowBlockOptions2.MaxDegreeOfParallelism = this.client.MaxParallelismCount;
          dataflowBlockOptions2.BoundedCapacity = 2 * this.client.MaxParallelismCount;
          ActionBlock<DedupNode[]> uploader = NonSwallowingActionBlock.Create<DedupNode[]>(action1, dataflowBlockOptions2);
          Task.Run((Func<Task>) (async () =>
          {
            try
            {
              while (!statsCancellationSrc.IsCancellationRequested)
              {
                if (bytesHashed > 0L)
                {
                  this.tracer.Info(string.Format("Uploading {0} files from directory {1}.", (object) manifestItems.Count, (object) sourceDirectory));
                  this.tracer.Info(string.Format("Uploaded {0:N0} out of {1:N0} bytes.", (object) dedupUploadSession.UploadStatistics.LogicalContentBytesUploaded, (object) bytesHashed));
                  this.tracer.Verbose(string.Format("Batcher OutputCount: {0}  Uploader queue: {1}  Active Upload sessions: {2}.", (object) batcher.OutputCount, (object) uploader.InputCount, (object) activeUploads));
                }
                await Task.Delay(this.StatsInterval, statsCancellationSrc.Token).ConfigureAwait(false);
              }
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == statsCancellationSrc.Token)
            {
            }
          }));
          batcher.LinkTo((ITargetBlock<DedupNode[]>) uploader, new DataflowLinkOptions()
          {
            PropagateCompletion = true
          });
          PrecomputedHashesGenerator precomputedHashesGenerator = new PrecomputedHashesGenerator(this.tracer, this.fileSystem);
          int emptyDirectories = 0;
          string sourceDirectory1 = sourceDirectory;
          IReadOnlyCollection<FileInfo> filePaths = fileInfoList;
          int hashType = (int) this.client.HashType;
          ArtifactPublishOptions artifactPublishOptions1 = artifactPublishOptions;
          CancellationToken cancellationToken1 = cancellationToken;
          Action<FileBlobDescriptor> hashCompleteCallback = (Action<FileBlobDescriptor>) (file =>
          {
            if (file.AbsolutePath.EndsWith(FileBlobDescriptorConstants.EmptyDirectoryEndingPattern))
              Interlocked.Increment(ref emptyDirectories);
            PostManifestFileToBatcher(new DedupManifestArtifactClient.ManifestFile(file.RelativePath, file.Node, file.AbsolutePath));
          });
          long fileCount = await precomputedHashesGenerator.PaginateAndProcessFiles(sourceDirectory1, (IEnumerable<FileInfo>) filePaths, (HashType) hashType, true, artifactPublishOptions1, false, false, cancellationToken1, hashCompleteCallback).ConfigureAwait(false);
          if (otherFiles != null)
          {
            Func<KeyValuePair<string, FileInfo>, Task> action2 = (Func<KeyValuePair<string, FileInfo>, Task>) (async otherFile =>
            {
              string relativePath = otherFile.Key;
              FileInfo info = otherFile.Value;
              DedupNode fromFileAsync = await ChunkerHelper.CreateFromFileAsync(this.fileSystem, info.FullName, false, this.client.HashType, cancellationToken);
              PostManifestFileToBatcher(new DedupManifestArtifactClient.ManifestFile(relativePath, fromFileAsync, info.FullName));
              relativePath = (string) null;
              info = (FileInfo) null;
            });
            ExecutionDataflowBlockOptions dataflowBlockOptions3 = new ExecutionDataflowBlockOptions();
            dataflowBlockOptions3.CancellationToken = cancellationToken;
            dataflowBlockOptions3.MaxDegreeOfParallelism = this.client.MaxParallelismCount;
            dataflowBlockOptions3.BoundedCapacity = 2 * this.client.MaxParallelismCount;
            await NonSwallowingActionBlock.Create<KeyValuePair<string, FileInfo>>(action2, dataflowBlockOptions3).SendAllAndCompleteSingleBlockNetworkAsync<KeyValuePair<string, FileInfo>>((IEnumerable<KeyValuePair<string, FileInfo>>) otherFiles, cancellationToken);
          }
          fileCount -= (long) emptyDirectories;
          batcher.Complete();
          await uploader.Completion.ConfigureAwait(false);
          List<DedupNode> innerNodeRoots = innerNodes.ToList<DedupNode>();
          while (innerNodeRoots.Count > 1)
            innerNodeRoots = innerNodeRoots.GetPages<DedupNode>(512).Select<List<DedupNode>, DedupNode>((Func<List<DedupNode>, DedupNode>) (children => new DedupNode((IEnumerable<DedupNode>) children))).ToList<DedupNode>();
          this.GenerateManifest((IEnumerable<DedupManifestArtifactClient.ManifestFile>) manifestItems, manifestFileOutputPath);
          FileBlobDescriptor manifest = await FileBlobDescriptor.CalculateAsync(this.fileSystem, Path.GetDirectoryName(manifestFileOutputPath), this.client.HashType, manifestFileOutputPath, FileBlobType.File, false, false, cancellationToken).ConfigureAwait(false);
          DedupNode? nullable = innerNodeRoots.Count > 0 ? new DedupNode?(innerNodeRoots.Single<DedupNode>()) : new DedupNode?();
          DedupNode dedupNode;
          if (!nullable.HasValue)
            dedupNode = new DedupNode((IEnumerable<DedupNode>) new DedupNode[1]
            {
              manifest.Node
            });
          else
            dedupNode = new DedupNode((IEnumerable<DedupNode>) new DedupNode[2]
            {
              nullable.Value,
              manifest.Node
            });
          DedupNode root = dedupNode;
          this.tracer.Verbose("ManifestId: " + DedupStoreClient.MaskDedupId(manifest.Node.GetDedupIdentifier()));
          this.tracer.Verbose("ContentNode: " + (nullable.HasValue ? DedupStoreClient.MaskDedupId(nullable.Value.GetDedupIdentifier()) : "No files"));
          this.tracer.Verbose("RootId: " + DedupStoreClient.MaskDedupId(root.GetDedupIdentifier()));
          FileBlobDescriptor[] source = new FileBlobDescriptor[1]
          {
            manifest
          };
          KeepUntilReceipt keepUntilReceipt1 = await dedupUploadSession.UploadAsync(root, (IReadOnlyDictionary<DedupIdentifier, string>) ((IEnumerable<FileBlobDescriptor>) source).ToDictionary<FileBlobDescriptor, DedupIdentifier, string>((Func<FileBlobDescriptor, DedupIdentifier>) (f => f.Node.GetDedupIdentifier()), (Func<FileBlobDescriptor, string>) (f => f.AbsolutePath)), cancellationToken).ConfigureAwait(false);
          this.tracer.Info(string.Format("Uploaded {0:N0} out of {1:N0} bytes", (object) dedupUploadSession.UploadStatistics.LogicalContentBytesUploaded, (object) root.TransitiveContentBytes));
          this.tracer.Info("Content upload is done!");
          this.tracer.Info("\nContent upload statistics:\n" + dedupUploadSession.UploadStatistics.AsString());
          this.ClientTelemetry.SendRecord((BlobStoreTelemetryRecord) this.ClientTelemetry.CreateRecord<DedupUploadTelemetryRecord>((Func<TelemetryInformationLevel, Uri, string, DedupUploadTelemetryRecord>) ((level, uri, prefix) => new DedupUploadTelemetryRecord(level, uri, prefix, "UploadAsync", dedupUploadSession.UploadStatistics))));
          IEnumerable<string> proofNodes = ProofHelper.CreateProofNodes(dedupUploadSession.AllNodes, dedupUploadSession.ParentLookup, manifestItems.Select<DedupManifestArtifactClient.ManifestFile, DedupIdentifier>((Func<DedupManifestArtifactClient.ManifestFile, DedupIdentifier>) (h => h.Node.GetDedupIdentifier())).Concat<DedupIdentifier>((IEnumerable<DedupIdentifier>) new DedupIdentifier[1]
          {
            manifest.Node.GetDedupIdentifier()
          }).Distinct<DedupIdentifier>()).Select<DedupNode, string>((Func<DedupNode, string>) (n => Convert.ToBase64String(n.Serialize())));
          publishResult = new PublishResult(manifest.Node.GetDedupIdentifier(), root.GetDedupIdentifier(), proofNodes, fileCount, bytesHashed);

          void PostManifestFileToBatcher(
            DedupManifestArtifactClient.ManifestFile manifestFile)
          {
            manifestItems.Add(manifestFile);
            Interlocked.Add(ref bytesHashed, (long) manifestFile.Node.TransitiveContentBytes);
            if (!allFileHashes.TryAdd(manifestFile.Node.GetDedupIdentifier(), manifestFile))
              return;
            batcher.PostOrThrow<DedupNode>(manifestFile.Node, cancellationToken);
          }
        }
        finally
        {
          statsCancellationSrc.Cancel();
          temporaryManifest?.Dispose();
        }
      }
      temporaryManifest = (TempFile) null;
      return publishResult;
    }

    public Task DownloadAsync(
      DedupIdentifier manifestId,
      string targetDirectory,
      CancellationToken cancellationToken)
    {
      return this.DownloadAsync(DownloadDedupManifestArtifactOptions.CreateWithManifestId(manifestId, targetDirectory), cancellationToken);
    }

    public async Task DownloadAsync(
      DownloadDedupManifestArtifactOptions downloadOptions,
      CancellationToken cancellationToken)
    {
      this.Trace_X_TFS_SessionId();
      Uri proxyUri = (Uri) null;
      IDictionary<string, DedupIdentifier> artifactNameAndManifestIds = downloadOptions.ArtifactNameAndManifestIds;
      if ((artifactNameAndManifestIds == null || artifactNameAndManifestIds.Count == 0) && downloadOptions.ManifestId == (DedupIdentifier) null)
        throw new ArgumentNullException("No valid manifest ID provided.");
      if (artifactNameAndManifestIds != null && downloadOptions.ManifestId != (DedupIdentifier) null)
        throw new ArgumentException("Options cannot contain both multi download and single download parameters.");
      if (downloadOptions.ManifestId != (DedupIdentifier) null && artifactNameAndManifestIds == null)
        artifactNameAndManifestIds = (IDictionary<string, DedupIdentifier>) new Dictionary<string, DedupIdentifier>()
        {
          {
            string.Empty,
            downloadOptions.ManifestId
          }
        };
      DedupDownloadStatistics downloadStatistics = new DedupDownloadStatistics(0L, 0L, 0L, 0L, 0L);
      foreach (KeyValuePair<string, DedupIdentifier> keyValuePair in (IEnumerable<KeyValuePair<string, DedupIdentifier>>) artifactNameAndManifestIds)
      {
        if (!string.IsNullOrWhiteSpace(keyValuePair.Key))
          this.tracer.Info("Start downloading artifact - " + keyValuePair.Key);
        string targetDirectory = downloadOptions.ManifestId == (DedupIdentifier) null ? this.RelativePathCombine(downloadOptions.TargetDirectory, keyValuePair.Key) : downloadOptions.TargetDirectory;
        DownloadDedupManifestArtifactOptions withManifestId = DownloadDedupManifestArtifactOptions.CreateWithManifestId(keyValuePair.Value, targetDirectory, proxyUri, downloadOptions.MinimatchPatterns, keyValuePair.Key, downloadOptions.MinimatchFilterWithArtifactName, downloadOptions.CustomMinimatchOptions);
        HashSet<string> emptyExcludedPaths = this.CreateEmptyExcludedPaths();
        downloadStatistics.ConcatenateStatistics(await this.DownloadSingleManifestAsync(withManifestId, true, (ISet<string>) emptyExcludedPaths, cancellationToken));
      }
      this.ClientTelemetry.SendRecord((BlobStoreTelemetryRecord) this.ClientTelemetry.CreateRecord<DedupDownloadTelemetryRecord>((Func<TelemetryInformationLevel, Uri, string, DedupDownloadTelemetryRecord>) ((level, uri, prefix) => new DedupDownloadTelemetryRecord(level, uri, prefix, nameof (DownloadAsync), downloadStatistics))));
      this.tracer.Info("Download completed.");
      if (artifactNameAndManifestIds.Count <= 1)
      {
        proxyUri = (Uri) null;
        artifactNameAndManifestIds = (IDictionary<string, DedupIdentifier>) null;
      }
      else
      {
        this.tracer.Info("\nAll the artifacts were downloaded successfully.\n\nDownload Summary:\n" + this.client.DownloadStatistics.AsString());
        proxyUri = (Uri) null;
        artifactNameAndManifestIds = (IDictionary<string, DedupIdentifier>) null;
      }
    }

    public Task DownloadAsyncWithManifestPath(
      string fullManifestPath,
      string targetDirectory,
      Uri proxyUri,
      CancellationToken cancellationToken)
    {
      return this.DownloadAsyncWithManifestPath(DownloadDedupManifestArtifactOptions.CreateWithManifestPath(fullManifestPath, targetDirectory, proxyUri), cancellationToken);
    }

    public Task DownloadAsyncWithManifestPath(
      DownloadDedupManifestArtifactOptions downloadOptions,
      CancellationToken cancellationToken)
    {
      IEnumerable<Func<string, bool>> minimatchFuncs = MinimatchHelper.GetMinimatchFuncs(downloadOptions.MinimatchPatterns, this.tracer, downloadOptions.CustomMinimatchOptions);
      HashSet<string> emptyExcludedPaths = this.CreateEmptyExcludedPaths();
      return (Task) this.DownloadAsyncWithManifestPath(downloadOptions, minimatchFuncs, true, (ISet<string>) emptyExcludedPaths, cancellationToken);
    }

    private async Task<DedupDownloadStatistics> DownloadAsyncWithManifestPath(
      DownloadDedupManifestArtifactOptions downloadOptions,
      IEnumerable<Func<string, bool>> minimatcherFuncs,
      bool downloadManifestReferences,
      ISet<string> excludedPaths,
      CancellationToken cancellationToken)
    {
      DedupManifestArtifactClient manifestArtifactClient = this;
      Uri proxyUri = downloadOptions.ProxyUri;
      string absoluteManifestPath = downloadOptions.AbsoluteManifestPath;
      Manifest manifest = JsonSerializer.Deserialize<Manifest>(manifestArtifactClient.fileSystem.ReadAllText(absoluteManifestPath));
      string artifactName = downloadOptions.ArtifactName;
      string targetDirectory = downloadOptions.TargetDirectory;
      manifestArtifactClient.FixupEmptyDirectoriesFromBrokenManifest(manifest);
      manifestArtifactClient.client.ResetDownloadStatistics();
      if (minimatcherFuncs != null && minimatcherFuncs.Count<Func<string, bool>>() != 0)
        manifest = manifestArtifactClient.GetFilteredManifest(artifactName, manifest, minimatcherFuncs, downloadOptions.MinimatchFilterWithArtifactName);
      IEnumerable<ManifestItem> filteredManifestItems = manifest.Items.Where<ManifestItem>((Func<ManifestItem, bool>) (i => !excludedPaths.Contains(i.Path)));
      ulong totalContentBytes = 0;
      foreach (ManifestItem manifestItem in filteredManifestItems)
      {
        if (manifestItem.Type == ManifestItemType.File)
          totalContentBytes += manifestItem.Blob.Size;
      }
      IDedupDataPort maybeDataport = (await DedupVolumeChunkStore.GetDataPortAsync(targetDirectory, cancellationToken, (Action<string>) (msg => this.tracer.Verbose(msg))).ConfigureAwait(false)).Match<IDedupDataPort>((Func<IDedupDataPort, IDedupDataPort>) (d => d), (Func<string, IDedupDataPort>) (errorMsg =>
      {
        this.tracer.Verbose("Could not initialize dataport: " + errorMsg);
        return (IDedupDataPort) null;
      }));
      CancellationTokenSource statsCancellationSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      manifestArtifactClient.RunPeriodicTraceDownloadProgress(totalContentBytes, statsCancellationSrc);
      if (maybeDataport == null)
      {
        GroupingDataflowBlockOptions dataflowBlockOptions1 = new GroupingDataflowBlockOptions();
        dataflowBlockOptions1.CancellationToken = cancellationToken;
        BatchBlock<ManifestItem> targetBlock = new BatchBlock<ManifestItem>(1000, dataflowBlockOptions1);
        Func<IEnumerable<ManifestItem>, Task<IEnumerable<(ManifestItem, GetDedupAsyncFunc)>>> transform = (Func<IEnumerable<ManifestItem>, Task<IEnumerable<(ManifestItem, GetDedupAsyncFunc)>>>) (async itemPage =>
        {
          Dictionary<DedupIdentifier, GetDedupAsyncFunc> getters = await this.client.GetDedupGettersAsync((ISet<DedupIdentifier>) new HashSet<DedupIdentifier>(itemPage.Select<ManifestItem, DedupIdentifier>((Func<ManifestItem, DedupIdentifier>) (i => DedupIdentifier.Create(i.Blob.Id)))), (Uri) null, DedupManifestArtifactClient.edgeCache, cancellationToken).ConfigureAwait(false);
          return itemPage.Select<ManifestItem, (ManifestItem, GetDedupAsyncFunc)>((Func<ManifestItem, (ManifestItem, GetDedupAsyncFunc)>) (i => (i, getters[DedupIdentifier.Create(i.Blob.Id)])));
        });
        ExecutionDataflowBlockOptions dataflowBlockOptions2 = new ExecutionDataflowBlockOptions();
        dataflowBlockOptions2.BoundedCapacity = 4 * manifestArtifactClient.client.MaxParallelismCount;
        dataflowBlockOptions2.MaxDegreeOfParallelism = manifestArtifactClient.client.MaxParallelismCount;
        dataflowBlockOptions2.CancellationToken = cancellationToken;
        dataflowBlockOptions2.EnsureOrdered = false;
        TransformManyBlock<IEnumerable<ManifestItem>, (ManifestItem, GetDedupAsyncFunc)> target = NonSwallowingTransformManyBlock.Create<IEnumerable<ManifestItem>, (ManifestItem, GetDedupAsyncFunc)>(transform, dataflowBlockOptions2);
        Func<(ManifestItem, GetDedupAsyncFunc), Task> action = (Func<(ManifestItem, GetDedupAsyncFunc), Task>) (async item =>
        {
          string fullPath = this.RelativePathCombine(targetDirectory, item.entry.Path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
          this.CreateDirectoryIfDoesntExist(fullPath, true);
          DedupIdentifier dedupId = DedupIdentifier.Create(item.entry.Blob.Id);
          ChunkedFileDownloadResult fileDownloadResult = await this.client.DownloadToFileAsync(maybeDataport, dedupId, fullPath, item.entry.Blob.Size, item.getter, proxyUri, DedupManifestArtifactClient.edgeCache, cancellationToken).ConfigureAwait(false);
          this.tracer.Verbose("Downloaded " + item.entry.Path);
        });
        ExecutionDataflowBlockOptions dataflowBlockOptions3 = new ExecutionDataflowBlockOptions();
        dataflowBlockOptions3.BoundedCapacity = 4 * manifestArtifactClient.client.MaxParallelismCount;
        dataflowBlockOptions3.MaxDegreeOfParallelism = manifestArtifactClient.client.MaxParallelismCount;
        dataflowBlockOptions3.CancellationToken = cancellationToken;
        dataflowBlockOptions3.EnsureOrdered = false;
        ActionBlock<(ManifestItem, GetDedupAsyncFunc)> actionBlock = NonSwallowingActionBlock.Create<(ManifestItem, GetDedupAsyncFunc)>(action, dataflowBlockOptions3);
        targetBlock.LinkTo((ITargetBlock<ManifestItem[]>) target, new DataflowLinkOptions()
        {
          PropagateCompletion = true
        });
        target.LinkTo((ITargetBlock<(ManifestItem, GetDedupAsyncFunc)>) actionBlock, new DataflowLinkOptions()
        {
          PropagateCompletion = true
        });
        IEnumerable<ManifestItem> inputs = manifest.Items.Where<ManifestItem>((Func<ManifestItem, bool>) (i => i.Type != ManifestItemType.EmptyDirectory));
        try
        {
          await targetBlock.SendAllAndCompleteAsync<ManifestItem, (ManifestItem, GetDedupAsyncFunc)>(inputs, (ITargetBlock<(ManifestItem, GetDedupAsyncFunc)>) actionBlock, cancellationToken).ConfigureAwait(false);
        }
        catch (VssServiceResponseException ex)
        {
          manifestArtifactClient.tracer.Info("Response exception thrown: " + ex.ToString() + " " + string.Format("\n Status code: {0} ", (object) ex.HttpStatusCode) + "\n Inner exception: " + ex.InnerException?.ToString() + " \n Source: " + ex.Source);
          ex.ReThrow();
          throw new InvalidOperationException("unreachable.");
        }
      }
      else
      {
        IDedupDataPort dataport = maybeDataport;
        IEnumerable<ManifestItem> emptyFiles = filteredManifestItems.Where<ManifestItem>((Func<ManifestItem, bool>) (mapping => mapping.Type == ManifestItemType.File && mapping.Blob.Size == 0UL));
        Task.Run((Action) (() =>
        {
          byte[] bytes = Array.Empty<byte>();
          foreach (ManifestItem manifestItem in emptyFiles)
          {
            string str = this.RelativePathCombine(targetDirectory, manifestItem.Path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            try
            {
              this.fileSystem.WriteAllBytes(str, bytes);
            }
            catch (DirectoryNotFoundException ex)
            {
              this.CreateDirectoryIfDoesntExist(str, true);
              this.fileSystem.WriteAllBytes(str, bytes);
            }
            this.tracer.Verbose("Downloaded " + manifestItem.Path);
          }
        }));
        IEnumerable<ManifestItem> nonZeroFiles = filteredManifestItems.Where<ManifestItem>((Func<ManifestItem, bool>) (entry => entry.Type == ManifestItemType.File && entry.Blob.Size > 0UL));
        manifestArtifactClient.tracer.Info("Walking nodes to enumerate all chunks.");
        ConcurrentDictionary<DedupIdentifier, DedupNode> fileNodes = new ConcurrentDictionary<DedupIdentifier, DedupNode>();
        HashSet<DedupIdentifier> dedupIds = new HashSet<DedupIdentifier>(nonZeroFiles.Where<ManifestItem>((Func<ManifestItem, bool>) (entry => entry.Blob.Id.EndsWith("2"))).Select<ManifestItem, DedupIdentifier>((Func<ManifestItem, DedupIdentifier>) (entry => DedupIdentifier.Create(entry.Blob.Id))));
        Dictionary<DedupIdentifier, GetDedupAsyncFunc> inputs = await manifestArtifactClient.client.Client.GetDedupGettersAsync((ISet<DedupIdentifier>) dedupIds, proxyUri, DedupManifestArtifactClient.edgeCache, cancellationToken).ConfigureAwait(false);
        Func<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>, Task> action1 = (Func<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>, Task>) (async nodeGetter =>
        {
          using (DedupCompressedBuffer nodeBuffer = (DedupCompressedBuffer) await nodeGetter.Value(cancellationToken).ConfigureAwait(false))
          {
            DedupNode dedupNode = await this.client.GetFilledNodesAsync(DedupNode.Deserialize(nodeBuffer.Uncompressed), proxyUri, DedupManifestArtifactClient.edgeCache, cancellationToken).ConfigureAwait(false);
            fileNodes[nodeGetter.Key] = dedupNode;
          }
        });
        ExecutionDataflowBlockOptions dataflowBlockOptions4 = new ExecutionDataflowBlockOptions();
        dataflowBlockOptions4.MaxDegreeOfParallelism = manifestArtifactClient.client.MaxParallelismCount;
        dataflowBlockOptions4.CancellationToken = cancellationToken;
        await NonSwallowingActionBlock.Create<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>>(action1, dataflowBlockOptions4).PostAllToUnboundedAndCompleteAsync<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>>((IEnumerable<KeyValuePair<DedupIdentifier, GetDedupAsyncFunc>>) inputs, cancellationToken).ConfigureAwait(false);
        foreach (ManifestItem manifestItem in nonZeroFiles.Where<ManifestItem>((Func<ManifestItem, bool>) (entry => entry.Blob.Id.EndsWith("1"))))
        {
          DedupNode dedupNode = new DedupNode(new ChunkInfo(0UL, (uint) manifestItem.Blob.Size, DedupIdentifier.Create(manifestItem.Blob.Id).AlgorithmResult));
          fileNodes.TryAdd((DedupIdentifier) new ChunkDedupIdentifier(dedupNode.Hash), dedupNode);
        }
        List<DedupNode> list = fileNodes.Values.SelectMany<DedupNode, DedupNode>((Func<DedupNode, IEnumerable<DedupNode>>) (n => n.EnumerateChunkLeafsInOrder())).Distinct<DedupNode>().ToList<DedupNode>();
        manifestArtifactClient.tracer.Info(string.Format("Ensuring {0} chunks are in the chunk store.", (object) list.Count));
        await manifestArtifactClient.client.EnsureChunksAreLocalAsync(dataport, (IEnumerable<DedupNode>) list, proxyUri, DedupManifestArtifactClient.edgeCache, cancellationToken).ConfigureAwait(false);
        manifestArtifactClient.tracer.Info(string.Format("Creating dedup reparse points for {0} files.", (object) manifest.Items.Count));
        Func<ManifestItem, int> func;
        Func<IReadOnlyList<ManifestItem>, Task> action2 = (Func<IReadOnlyList<ManifestItem>, Task>) (async outerPage =>
        {
          Queue<IReadOnlyList<ManifestItem>> innerPages = new Queue<IReadOnlyList<ManifestItem>>();
          innerPages.Enqueue(outerPage);
          while (innerPages.Any<IReadOnlyList<ManifestItem>>())
          {
            IReadOnlyList<ManifestItem> source = innerPages.Dequeue();
            DedupStream[] streams = new DedupStream[source.Count];
            DedupStreamEntry[] entries = new DedupStreamEntry[source.Sum<ManifestItem>(func ?? (func = (Func<ManifestItem, int>) (entry => fileNodes[DedupIdentifier.Create(entry.Blob.Id)].EnumerateChunkLeafsInOrder().Count<DedupNode>())))];
            int index1 = 0;
            for (int index2 = 0; index2 < source.Count; ++index2)
            {
              ManifestItem manifestItem = source[index2];
              DedupNode dedupNode1 = fileNodes[DedupIdentifier.Create(manifestItem.Blob.Id)];
              uint num1 = 0;
              ulong num2 = 0;
              foreach (DedupNode dedupNode2 in dedupNode1.EnumerateChunkLeafsInOrder())
              {
                entries[index1] = new DedupStreamEntry()
                {
                  Hash = new DedupHash()
                  {
                    Hash = dedupNode2.Hash
                  },
                  LogicalSize = (uint) dedupNode2.TransitiveContentBytes,
                  Offset = num2
                };
                num2 += dedupNode2.TransitiveContentBytes;
                ++index1;
                ++num1;
              }
              string str = this.RelativePathCombine(targetDirectory, manifestItem.Path.Replace('/', '\\').TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
              streams[index2] = new DedupStream()
              {
                ChunkCount = num1,
                Length = manifestItem.Blob.Size,
                Offset = 0UL,
                Path = str.Substring(2)
              };
            }
            Guid requestId;
            try
            {
              dataport.CommitStreams((uint) streams.Length, streams, (uint) entries.Length, entries, out requestId);
            }
            catch (COMException ex) when (ex.ErrorCode == -2141826193)
            {
              innerPages.Enqueue((IReadOnlyList<ManifestItem>) source.Take<ManifestItem>(source.Count / 2).ToList<ManifestItem>());
              innerPages.Enqueue((IReadOnlyList<ManifestItem>) source.Skip<ManifestItem>(source.Count / 2).ToList<ManifestItem>());
              continue;
            }
            DataPortResult dataPortResult = await dataport.GetResultAsync(requestId).ConfigureAwait(false);
            if (dataPortResult.BatchResult != 0 || ((IEnumerable<int>) dataPortResult.ItemResults).Any<int>((Func<int, bool>) (r => r != 0)))
              throw new InvalidOperationException(string.Format("CommitStream failed 0x{0:x} 0x{1:x}", (object) dataPortResult.BatchResult, (object) ((IEnumerable<int>) dataPortResult.ItemResults).First<int>((Func<int, bool>) (r => r != 0))));
            foreach (DedupStream dedupStream in streams)
              this.tracer.Verbose("Downloaded /" + dedupStream.Path);
            streams = (DedupStream[]) null;
          }
          innerPages = (Queue<IReadOnlyList<ManifestItem>>) null;
        });
        ExecutionDataflowBlockOptions dataflowBlockOptions5 = new ExecutionDataflowBlockOptions();
        dataflowBlockOptions5.MaxDegreeOfParallelism = manifestArtifactClient.client.MaxParallelismCount;
        dataflowBlockOptions5.CancellationToken = cancellationToken;
        await NonSwallowingActionBlock.Create<IReadOnlyList<ManifestItem>>(action2, dataflowBlockOptions5).PostAllToUnboundedAndCompleteAsync<IReadOnlyList<ManifestItem>>((IEnumerable<IReadOnlyList<ManifestItem>>) nonZeroFiles.GetPages<ManifestItem>(100), cancellationToken).ConfigureAwait(false);
        nonZeroFiles = (IEnumerable<ManifestItem>) null;
      }
      foreach (ManifestItem manifestItem in filteredManifestItems)
      {
        if (manifestItem.Type == ManifestItemType.EmptyDirectory)
        {
          excludedPaths.Add(manifestItem.Path);
          string fullPath = manifestArtifactClient.RelativePathCombine(targetDirectory, manifestItem.Path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
          manifestArtifactClient.CreateDirectoryIfDoesntExist(fullPath, false);
        }
      }
      manifestArtifactClient.TraceDownloadProgress(totalContentBytes);
      statsCancellationSrc.Cancel();
      DedupDownloadStatistics currentStatistics = manifestArtifactClient.TraceDownloadSummary();
      if (downloadManifestReferences)
      {
        foreach (ManifestReference manifestReference in (IEnumerable<ManifestReference>) manifest.ManifestReferences)
        {
          DownloadDedupManifestArtifactOptions withManifestId = DownloadDedupManifestArtifactOptions.CreateWithManifestId(manifestReference.ManifestId, downloadOptions.TargetDirectory, downloadOptions.ProxyUri, downloadOptions.MinimatchPatterns, artifactName);
          currentStatistics.ConcatenateStatistics(await manifestArtifactClient.DownloadSingleManifestAsync(withManifestId, false, excludedPaths, cancellationToken));
        }
      }
      DedupDownloadStatistics downloadStatistics = currentStatistics;
      manifest = (Manifest) null;
      artifactName = (string) null;
      filteredManifestItems = (IEnumerable<ManifestItem>) null;
      statsCancellationSrc = (CancellationTokenSource) null;
      currentStatistics = (DedupDownloadStatistics) null;
      return downloadStatistics;
    }

    public Task DownloadFileToPathAsync(
      DedupIdentifier dedupId,
      string fullFileOutputPath,
      Uri proxyUri,
      CancellationToken cancellationToken)
    {
      this.CreateDirectoryIfDoesntExist(fullFileOutputPath, true);
      return (Task) this.client.DownloadToFileAsync(dedupId, fullFileOutputPath, (GetDedupAsyncFunc) null, proxyUri, DedupManifestArtifactClient.edgeCache, cancellationToken);
    }

    public async Task DownloadToStreamAsync(
      DedupIdentifier dedupId,
      Stream stream,
      Uri proxyUri,
      CancellationToken cancellationToken)
    {
      CancellationTokenSource statsCancellationSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      Action<ulong> traceDownloadProgressFunc = (Action<ulong>) (size =>
      {
        this.tracer.Info(string.Format("Expected size to be downloaded: {0:N1} MB", (object) NumberConversionHelper.ConvertBytesToMegabytes((long) size)));
        this.RunPeriodicTraceDownloadProgress(size, statsCancellationSrc);
      });
      Action<ulong> traceFinalizeDownloadProgressFunc = (Action<ulong>) (size =>
      {
        this.TraceDownloadProgress(size);
        statsCancellationSrc.Cancel();
        this.TraceDownloadSummary();
      });
      await this.client.DownloadToStreamAsync(dedupId, stream, proxyUri, DedupManifestArtifactClient.edgeCache, traceDownloadProgressFunc, traceFinalizeDownloadProgressFunc, cancellationToken);
    }

    private async Task<DedupDownloadStatistics> DownloadSingleManifestAsync(
      DownloadDedupManifestArtifactOptions downloadOptions,
      bool downloadManifestReferences,
      ISet<string> excludedPaths,
      CancellationToken cancellationToken)
    {
      string manifestPath = this.RelativePathCombine(Path.GetTempPath(), "DedupManifestArtifactClient." + Path.GetRandomFileName() + ".manifest");
      IEnumerable<Func<string, bool>> minimatcherFuncs = MinimatchHelper.GetMinimatchFuncs(downloadOptions.MinimatchPatterns, this.tracer, downloadOptions.CustomMinimatchOptions);
      DedupDownloadStatistics downloadStatistics;
      try
      {
        await this.DownloadFileToPathAsync(downloadOptions.ManifestId, manifestPath, downloadOptions.ProxyUri, cancellationToken).ConfigureAwait(false);
        downloadOptions.SetAbsoluteManifestPathAndRemoveManifestId(manifestPath);
        downloadStatistics = await this.DownloadAsyncWithManifestPath(downloadOptions, minimatcherFuncs, downloadManifestReferences, excludedPaths, cancellationToken).ConfigureAwait(false);
      }
      finally
      {
        if (this.fileSystem.FileExists(manifestPath))
        {
          try
          {
            this.fileSystem.DeleteFile(manifestPath);
          }
          catch
          {
            this.tracer.Info("Failed to delete manifest");
          }
        }
      }
      manifestPath = (string) null;
      minimatcherFuncs = (IEnumerable<Func<string, bool>>) null;
      return downloadStatistics;
    }

    private Manifest GetFilteredManifest(
      string artifactName,
      Manifest manifest,
      IEnumerable<Func<string, bool>> minimatchFuncs,
      bool minimatchFilterWithArtifactName)
    {
      HashSet<ManifestItem> source = new HashSet<ManifestItem>();
      foreach (ManifestItem manifestItem in (IEnumerable<ManifestItem>) manifest.Items)
      {
        string str;
        if (!string.IsNullOrWhiteSpace(artifactName) && minimatchFilterWithArtifactName)
          str = artifactName + manifestItem.Path;
        else
          str = manifestItem.Path.TrimStart('/');
        string path = str;
        if (minimatchFuncs.Any<Func<string, bool>>((Func<Func<string, bool>, bool>) (match => match(path))))
          source.Add(manifestItem);
      }
      this.tracer.Info(string.Format("Filtered {0} files from the Minimatch filters supplied.", (object) source.Count));
      return new Manifest((IList<ManifestItem>) source.ToList<ManifestItem>());
    }

    private void CreateDirectoryIfDoesntExist(string fullPath, bool fullPathIsFile)
    {
      string directoryPath = fullPathIsFile ? Path.GetDirectoryName(fullPath) : fullPath;
      if (this.fileSystem.DirectoryExists(directoryPath))
        return;
      this.fileSystem.CreateDirectory(directoryPath);
    }

    private void GenerateManifest(
      IEnumerable<DedupManifestArtifactClient.ManifestFile> files,
      string manifestFilePath)
    {
      List<ManifestItem> items = new List<ManifestItem>();
      foreach (DedupManifestArtifactClient.ManifestFile file in files)
      {
        ManifestItemType type;
        string path;
        if (file.LocalFileSystemAbsolutePath.EndsWith(FileBlobDescriptorConstants.EmptyDirectoryEndingPattern))
        {
          type = ManifestItemType.EmptyDirectory;
          string str = Locator.Parse(file.ManifestRelativePath).Value;
          path = str.Remove(str.Length - FileBlobDescriptorConstants.EmptyDirectoryEndingPattern.Length);
        }
        else
        {
          type = ManifestItemType.File;
          path = Locator.Parse(file.ManifestRelativePath).Value;
        }
        ManifestItem manifestItem = new ManifestItem(path, new DedupInfo(file.Node.GetDedupIdentifier().ValueString, file.Node.TransitiveContentBytes), type);
        items.Add(manifestItem);
      }
      items.Sort((Comparison<ManifestItem>) ((i1, i2) => StringComparer.Ordinal.Compare(i1.Path, i2.Path)));
      string content = JsonSerializer.Serialize<Manifest>(new Manifest((IList<ManifestItem>) items));
      this.CreateDirectoryIfDoesntExist(Path.GetFullPath(manifestFilePath), true);
      this.fileSystem.WriteAllText(manifestFilePath, content);
    }

    private void RunPeriodicTraceDownloadProgress(
      ulong totalContentBytes,
      CancellationTokenSource statsCancellationSrc)
    {
      Task.Run((Func<Task>) (async () =>
      {
        while (!statsCancellationSrc.IsCancellationRequested)
        {
          try
          {
            this.TraceDownloadProgress(totalContentBytes);
            await Task.Delay(this.StatsInterval, statsCancellationSrc.Token).ConfigureAwait(false);
          }
          catch
          {
          }
        }
      }));
    }

    private void TraceDownloadProgress(ulong totalContentBytes)
    {
      int num = totalContentBytes <= 0UL ? 100 : (int) Math.Round((double) this.client.DownloadStatistics.TotalContentBytes / (double) totalContentBytes * 100.0);
      this.tracer.Info(string.Format("Downloaded {0:N1} MB out of {1:N1} MB ({2}%).", (object) NumberConversionHelper.ConvertBytesToMegabytes(this.client.DownloadStatistics.TotalContentBytes), (object) NumberConversionHelper.ConvertBytesToMegabytes((long) totalContentBytes), (object) num));
    }

    private DedupDownloadStatistics TraceDownloadSummary()
    {
      DedupDownloadStatistics downloadStatistics = this.client.DownloadStatistics;
      this.tracer.Info(Environment.NewLine + "Download statistics:" + Environment.NewLine + downloadStatistics.AsString());
      return downloadStatistics;
    }

    private void FixupEmptyDirectoriesFromBrokenManifest(Manifest m)
    {
      for (int index = 0; index < m.Items.Count; ++index)
      {
        ManifestItem manifestItem = m.Items[index];
        if (manifestItem.Type == ManifestItemType.File && Path.GetFileName(manifestItem.Path).Equals(".", StringComparison.Ordinal) && manifestItem.Blob.Size == 0UL)
          m.Items[index] = new ManifestItem(Path.GetDirectoryName(manifestItem.Path), (DedupInfo) null, ManifestItemType.EmptyDirectory);
      }
    }

    private HashSet<string> CreateEmptyExcludedPaths() => !DedupManifestArtifactClient.isWindows ? new HashSet<string>() : new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    private void Trace_X_TFS_SessionId() => this.tracer.Info("DedupManifestArtifactClient will correlate http requests with X-TFS-Session " + TelemetryRecord.Current_X_TFS_Session.ToString());

    private string RelativePathCombine(string lhs, string rhs) => !Path.IsPathRooted(rhs) || DedupManifestArtifactClient.BypassRelativePathRequirement ? Path.Combine(lhs, rhs) : throw new ArgumentException("The path '" + rhs + "' was expected to be relative, but specified a rooted path on this system.");

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing && this.disposeTelemetry)
        ((IDisposable) this.ClientTelemetry).Dispose();
      this.disposedValue = true;
    }

    public void Dispose() => this.Dispose(true);

    private struct ManifestFile
    {
      public readonly string LocalFileSystemAbsolutePath;
      public readonly string ManifestRelativePath;
      public readonly DedupNode Node;

      public ManifestFile(
        string manifestRelativePath,
        DedupNode node,
        string localFileSystemAbsolutePath)
        : this()
      {
        this.ManifestRelativePath = manifestRelativePath;
        this.Node = node;
        this.LocalFileSystemAbsolutePath = localFileSystemAbsolutePath;
      }
    }
  }
}
