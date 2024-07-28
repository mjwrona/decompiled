// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.PrecomputedHashesGenerator
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public class PrecomputedHashesGenerator
  {
    private const int DefaultMaxParallelDirectoryHashCount = 128;
    private const int DefaultMaxPageSize = 100;
    private const bool DefaultLowercasePaths = false;
    private static readonly SemaphoreSlim MaxParallelComputeHash = new SemaphoreSlim(Environment.ProcessorCount);
    private readonly IAppTraceSource tracer;
    private readonly IFileSystem fileSystem;

    public PrecomputedHashesGenerator(IAppTraceSource tracer)
      : this(tracer, (IFileSystem) FileSystem.Instance)
    {
    }

    public PrecomputedHashesGenerator(IAppTraceSource tracer, IFileSystem fileSystem)
      : this(tracer, fileSystem, 100, 128, false)
    {
    }

    public PrecomputedHashesGenerator(
      IAppTraceSource tracer,
      IFileSystem fileSystem,
      int maxPageSize,
      int maxParallelDirectoryHash,
      bool lowercasePaths)
    {
      if (tracer == null)
        throw new ArgumentNullException(nameof (tracer));
      if (maxPageSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (maxPageSize), "Expected a positive pageSize");
      this.MaxParallelDirectoryPublish = maxParallelDirectoryHash >= 1 || maxParallelDirectoryHash == -1 ? maxParallelDirectoryHash : throw new ArgumentOutOfRangeException(nameof (maxParallelDirectoryHash), "Expected a positive value or -1 to denote maximum parallelism");
      this.MaxPageSize = maxPageSize;
      this.LowercasePaths = lowercasePaths;
      this.tracer = tracer;
      ArgumentUtility.CheckForNull<IFileSystem>(fileSystem, nameof (fileSystem));
      this.fileSystem = fileSystem;
    }

    internal int MaxPageSize { get; }

    internal int MaxParallelDirectoryPublish { get; }

    public bool LowercasePaths { get; }

    public static List<FileBlobDescriptor> LoadPrecomputedHashes(
      string preComputedHashesFile,
      string directory,
      bool lowercasePaths = false)
    {
      directory = directory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
      List<FileBlobDescriptor> fileBlobDescriptorList = (List<FileBlobDescriptor>) null;
      using (Stream stream = (Stream) FileStreamUtils.OpenFileStreamForAsync(preComputedHashesFile, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (StreamReader streamReader = new StreamReader(stream))
        {
          fileBlobDescriptorList = new List<FileBlobDescriptor>();
          string serialized;
          while ((serialized = streamReader.ReadLine()) != null)
            fileBlobDescriptorList.Add(FileBlobDescriptor.Deserialize(directory, serialized, lowercasePaths));
        }
      }
      return fileBlobDescriptorList;
    }

    public async Task<List<FileBlobDescriptor>> GeneratePrecomputedHashesAsync(
      string precomputedHashesFileName,
      string directory,
      string fileListFileName,
      bool chunkDedup,
      bool includeEmptyDirectories,
      ArtifactPublishOptions artifactPublishOptions,
      CancellationToken cancellationToken)
    {
      return await this.GeneratePrecomputedHashesAsync(precomputedHashesFileName, directory, fileListFileName, chunkDedup, includeEmptyDirectories, artifactPublishOptions, false, false, cancellationToken);
    }

    public async Task<List<FileBlobDescriptor>> GeneratePrecomputedHashesAsync(
      string precomputedHashesFileName,
      string directory,
      string fileListFileName,
      bool chunkDedup,
      bool includeEmptyDirectories,
      ArtifactPublishOptions artifactPublishOptions,
      bool shouldPreserveSymbolicLink,
      bool shouldPreservePermissionValue,
      CancellationToken cancellationToken)
    {
      return await this.GeneratePrecomputedHashesAsync(precomputedHashesFileName, directory, fileListFileName, includeEmptyDirectories, artifactPublishOptions, shouldPreserveSymbolicLink, shouldPreservePermissionValue, chunkDedup ? ChunkerHelper.DefaultChunkHashType : HashType.Vso0, cancellationToken);
    }

    public async Task<List<FileBlobDescriptor>> GeneratePrecomputedHashesAsync(
      string precomputedHashesFileName,
      string directory,
      string fileListFileName,
      bool includeEmptyDirectories,
      ArtifactPublishOptions artifactPublishOptions,
      bool shouldPreserveSymbolicLink,
      bool shouldPreservePermissionValue,
      HashType hashType,
      CancellationToken cancellationToken)
    {
      directory = directory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
      List<FileBlobDescriptor> files = new List<FileBlobDescriptor>();
      using (StreamWriter hashFileStream = new StreamWriter(precomputedHashesFileName))
      {
        using (BlockingCollection<FileBlobDescriptor> fileBlobIds = new BlockingCollection<FileBlobDescriptor>())
        {
          Task loggerTask = Task.Run((Func<Task>) (async () =>
          {
            foreach (FileBlobDescriptor consuming in fileBlobIds.GetConsumingEnumerable())
            {
              files.Add(consuming);
              await hashFileStream.WriteLineAsync(consuming.Serialize()).ConfigureAwait(false);
            }
          }), cancellationToken);
          List<FileInfo> filePaths = (List<FileInfo>) null;
          if (!string.IsNullOrEmpty(fileListFileName))
            filePaths = await ListOfFiles.LoadFileListAsync(fileListFileName, directory).ConfigureAwait(false);
          long num = await this.PaginateAndProcessFiles(directory, (IEnumerable<FileInfo>) filePaths, hashType, includeEmptyDirectories, artifactPublishOptions, shouldPreserveSymbolicLink, shouldPreservePermissionValue, cancellationToken, (Action<FileBlobDescriptor>) (fileBlobId => fileBlobIds.Add(fileBlobId, cancellationToken))).ConfigureAwait(false);
          fileBlobIds.CompleteAdding();
          await loggerTask.ConfigureAwait(false);
          loggerTask = (Task) null;
        }
      }
      return files;
    }

    public Task<long> PaginateAndProcessFiles(
      string sourceDirectory,
      IEnumerable<FileInfo> filePaths,
      bool chunkDedup,
      bool includeEmptyDirectories,
      ArtifactPublishOptions artifactPublishOptions,
      CancellationToken cancellationToken,
      Action<FileBlobDescriptor> hashCompleteCallback)
    {
      IEnumerable<IEnumerable<PageItem>> pages = filePaths != null ? this.GetPagesFromPaths(filePaths) : this.GetSegmentedPagesFromSourceDirectory(sourceDirectory, includeEmptyDirectories, artifactPublishOptions);
      return this.PaginateAndProcessFiles(sourceDirectory, chunkDedup ? ChunkerHelper.DefaultChunkHashType : HashType.Vso0, pages, false, false, cancellationToken, hashCompleteCallback);
    }

    public Task<long> PaginateAndProcessFiles(
      string sourceDirectory,
      IEnumerable<FileInfo> filePaths,
      bool chunkDedup,
      bool includeEmptyDirectories,
      ArtifactPublishOptions artifactPublishOptions,
      bool preserveSymbolicLink,
      bool preservePermissionValue,
      CancellationToken cancellationToken,
      Action<FileBlobDescriptor> hashCompleteCallback)
    {
      IEnumerable<IEnumerable<PageItem>> pages = filePaths != null ? this.GetPagesFromPaths(filePaths) : this.GetSegmentedPagesFromSourceDirectory(sourceDirectory, includeEmptyDirectories, artifactPublishOptions, preserveSymbolicLink);
      return this.PaginateAndProcessFiles(sourceDirectory, chunkDedup ? ChunkerHelper.DefaultChunkHashType : HashType.Vso0, pages, preserveSymbolicLink, preservePermissionValue, cancellationToken, hashCompleteCallback);
    }

    public Task<long> PaginateAndProcessFiles(
      string sourceDirectory,
      IEnumerable<FileInfo> filePaths,
      HashType hashType,
      bool includeEmptyDirectories,
      ArtifactPublishOptions artifactPublishOptions,
      bool preserveSymbolicLink,
      bool preservePermissionValue,
      CancellationToken cancellationToken,
      Action<FileBlobDescriptor> hashCompleteCallback)
    {
      IEnumerable<IEnumerable<PageItem>> pages = filePaths != null ? this.GetPagesFromPaths(filePaths) : this.GetSegmentedPagesFromSourceDirectory(sourceDirectory, includeEmptyDirectories, artifactPublishOptions, preserveSymbolicLink);
      return this.PaginateAndProcessFiles(sourceDirectory, hashType, pages, preserveSymbolicLink, preservePermissionValue, cancellationToken, hashCompleteCallback);
    }

    public Task<long> PaginateAndProcessFiles(
      string sourceDirectory,
      bool chunkDedup,
      bool includeEmptyDirectories,
      ArtifactPublishOptions artifactPublishOptions,
      CancellationToken cancellationToken,
      Action<FileBlobDescriptor> hashCompleteCallback)
    {
      IEnumerable<IEnumerable<PageItem>> fromSourceDirectory = this.GetSegmentedPagesFromSourceDirectory(sourceDirectory, includeEmptyDirectories, artifactPublishOptions);
      return this.PaginateAndProcessFiles(sourceDirectory, chunkDedup ? ChunkerHelper.DefaultChunkHashType : HashType.Vso0, fromSourceDirectory, false, false, cancellationToken, hashCompleteCallback);
    }

    private async Task<long> PaginateAndProcessFiles(
      string sourceDirectory,
      HashType hashType,
      IEnumerable<IEnumerable<PageItem>> pages,
      bool shouldPreserveSymbolicLink,
      bool shouldPreservePermissionValue,
      CancellationToken cancellationToken,
      Action<FileBlobDescriptor> hashCompleteCallback)
    {
      long processedTotal = 0;
      int page = 0;
      Func<IEnumerable<PageItem>, Task> action = (Func<IEnumerable<PageItem>, Task>) (async pageOfFiles =>
      {
        try
        {
          Interlocked.Increment(ref page);
          long num = await this.PaginateAndProcessFilesHelperAsync(sourceDirectory, shouldPreserveSymbolicLink, shouldPreservePermissionValue, pageOfFiles, hashCompleteCallback, hashType, cancellationToken).ConfigureAwait(false);
          Interlocked.Add(ref processedTotal, num);
          this.tracer.Info(string.Format("{0} files processed.", (object) processedTotal));
        }
        catch (Exception ex)
        {
          this.tracer.Error("Failed to hash a page of files. Exception listed below...");
          this.tracer.Error(ex);
          ex.ReThrow();
          throw new InvalidOperationException("unreachable.");
        }
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = this.MaxParallelDirectoryPublish;
      dataflowBlockOptions.CancellationToken = cancellationToken;
      await NonSwallowingActionBlock.Create<IEnumerable<PageItem>>(action, dataflowBlockOptions).PostAllToUnboundedAndCompleteAsync<IEnumerable<PageItem>>(pages, cancellationToken).ConfigureAwait(false);
      this.tracer.Info(string.Format("Processed {0} files from {1} successfully.", (object) processedTotal, (object) sourceDirectory));
      return processedTotal;
    }

    internal IEnumerable<IEnumerable<PageItem>> GetPagesFromPaths(IEnumerable<FileInfo> paths) => (IEnumerable<IEnumerable<PageItem>>) paths.Select<FileInfo, PageItem>((Func<FileInfo, PageItem>) (path => new PageItem(path.FullName, PageItemType.File))).OrderBy<PageItem, string>((Func<PageItem, string>) (path => path.Path)).GetPages<PageItem>(this.MaxPageSize);

    internal IEnumerable<string> EnumerateFilesAndSymbolicLinksToDirectories(string directory)
    {
      if (!OSUtilities.IsFileSymbolicLink(new FileInfo(directory)))
        return Directory.EnumerateFiles(directory).Concat<string>(Directory.EnumerateDirectories(directory).SelectMany<string, string>((Func<string, IEnumerable<string>>) (subDir => this.EnumerateFilesAndSymbolicLinksToDirectories(subDir))));
      return (IEnumerable<string>) new string[1]
      {
        directory
      };
    }

    internal IEnumerable<IEnumerable<PageItem>> GetSegmentedPagesFromSourceDirectory(
      string sourceDirectory,
      bool includeEmptyDirectories,
      ArtifactPublishOptions artifactPublishOptions,
      bool preserveSymbolicLinks = false)
    {
      StringComparer defaultStringComparer = Helpers.FileSystemStringComparer(Environment.OSVersion);
      Stopwatch ticktock = new Stopwatch();
      ticktock.Start();
      HashSet<string> ignoreFiles = new HashSet<string>((IEqualityComparer<string>) defaultStringComparer);
      if (artifactPublishOptions.HonorIgnoreOptions)
        ignoreFiles = new GlobFactory(this.fileSystem, this.tracer, includeEmptyDirectories).PerformGlobbing(sourceDirectory).ToHashSet<string, string>((Func<string, string>) (x => x), (IEqualityComparer<string>) defaultStringComparer);
      ticktock.Stop();
      this.tracer.Verbose("Globbing completed in: {0}", (object) ticktock.Elapsed);
      ticktock.Restart();
      IEnumerable<string> source = (IEnumerable<string>) new PrecomputedHashesGenerator.EnumerateOnce<string>((IEnumerable<string>) this.fileSystem.EnumerateFiles(sourceDirectory, true).ToHashSet<string, string>((Func<string, string>) (x => x), (IEqualityComparer<string>) defaultStringComparer));
      HashSet<string> nonEmptyDirsFull = new HashSet<string>();
      if (includeEmptyDirectories)
        source = source.Select<string, string>((Func<string, string>) (f =>
        {
          nonEmptyDirsFull.Add(Path.GetDirectoryName(f));
          return f;
        }));
      if (artifactPublishOptions.HonorIgnoreOptions && ignoreFiles.Any<string>())
        source = source.Where<string>((Func<string, bool>) (f => !ignoreFiles.Contains(f)));
      foreach (IEnumerable<PageItem> page in source.Select<string, PageItem>((Func<string, PageItem>) (f => new PageItem(f, PageItemType.File))).GetPages<PageItem>(this.MaxPageSize))
        yield return page;
      if (includeEmptyDirectories)
      {
        HashSet<string> hashSet1 = this.fileSystem.EnumerateDirectories(sourceDirectory, true).ToHashSet<string, string>((Func<string, string>) (x => x), (IEqualityComparer<string>) defaultStringComparer);
        foreach (string path in hashSet1)
          nonEmptyDirsFull.Add(Path.GetDirectoryName(path));
        HashSet<string> hashSet2 = hashSet1.Where<string>((Func<string, bool>) (d => !nonEmptyDirsFull.Contains(d))).ToHashSet<string, string>((Func<string, string>) (x => x), (IEqualityComparer<string>) defaultStringComparer);
        if (artifactPublishOptions.HonorIgnoreOptions && hashSet2.Any<string>())
          hashSet2.ExceptWith((IEnumerable<string>) ignoreFiles);
        foreach (IEnumerable<PageItem> page in hashSet2.Select<string, PageItem>((Func<string, PageItem>) (d => new PageItem(d + Path.DirectorySeparatorChar.ToString() + ".", PageItemType.EmptyDirectory))).GetPages<PageItem>(this.MaxPageSize))
          yield return page;
      }
      ticktock.Stop();
      this.tracer.Verbose("Pages to upload computed in {0}", (object) ticktock.Elapsed);
    }

    private async Task<long> PaginateAndProcessFilesHelperAsync(
      string sourceDirectory,
      bool shouldPreserveSymbolicLink,
      bool shouldPreservePermissionValue,
      IEnumerable<PageItem> pathsInDir,
      Action<FileBlobDescriptor> hashCompleteCallback,
      HashType hashType,
      CancellationToken cancellationToken)
    {
      ConcurrentBag<FileBlobDescriptor> fileIds = new ConcurrentBag<FileBlobDescriptor>();
      Func<PageItem, Task> action = (Func<PageItem, Task>) (async path =>
      {
        FileBlobDescriptor fileBlobDescriptor = await this.GetFileBlobDescriptorAsync(sourceDirectory, shouldPreserveSymbolicLink, shouldPreservePermissionValue, path, hashType, cancellationToken).ConfigureAwait(false);
        fileIds.Add(fileBlobDescriptor);
        if (hashCompleteCallback == null)
          return;
        hashCompleteCallback(fileBlobDescriptor);
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = 16;
      dataflowBlockOptions.CancellationToken = cancellationToken;
      await NonSwallowingActionBlock.Create<PageItem>(action, dataflowBlockOptions).PostAllToUnboundedAndCompleteAsync<PageItem>(pathsInDir, cancellationToken).ConfigureAwait(false);
      return (long) fileIds.Count<FileBlobDescriptor>();
    }

    private async Task<FileBlobDescriptor> GetFileBlobDescriptorAsync(
      string rootDirectory,
      bool shouldPreserveSymbolicLink,
      bool shouldPreservePermissionValue,
      PageItem pageItem,
      HashType hashType,
      CancellationToken cancellationToken)
    {
      if (!pageItem.Path.StartsWith(rootDirectory, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Path '" + pageItem.Path + "' does not start with '" + rootDirectory + "'.", nameof (pageItem));
      await PrecomputedHashesGenerator.MaxParallelComputeHash.WaitAsync().ConfigureAwait(false);
      FileBlobDescriptor async;
      try
      {
        string relativePath = pageItem.Path.Substring(rootDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (this.LowercasePaths)
          relativePath = relativePath.ToLower();
        async = await FileBlobDescriptor.CalculateAsync(this.fileSystem, rootDirectory, hashType, relativePath, (FileBlobType) Enum.Parse(typeof (FileBlobType), pageItem.Type.ToString(), true), shouldPreserveSymbolicLink, shouldPreservePermissionValue, cancellationToken);
      }
      catch (FileNotFoundException ex1)
      {
        FileLoadException fileLoadException = (FileLoadException) null;
        try
        {
          if (File.GetAttributes(pageItem.Path).HasFlag((Enum) FileAttributes.ReparsePoint))
            fileLoadException = new FileLoadException(Resources.SymLinkExceptionMessage(), (Exception) ex1);
        }
        catch (Exception ex2)
        {
        }
        if (fileLoadException == null)
        {
          this.tracer.Error("Failed to calculate hash for file: " + pageItem.Path);
          ex1.ReThrow();
          throw new InvalidOperationException("unreachable.");
        }
        this.tracer.Error("Failed to calculate hash for the file's reparse point: " + pageItem.Path);
        throw fileLoadException;
      }
      catch (Exception ex)
      {
        this.tracer.Error("Failed to calculate hash for file: " + pageItem.Path);
        ex.ReThrow();
        throw new InvalidOperationException("unreachable.");
      }
      finally
      {
        PrecomputedHashesGenerator.MaxParallelComputeHash.Release();
      }
      return async;
    }

    private class EnumerateOnce<T> : IEnumerable<T>, IEnumerable
    {
      private IEnumerable<T> inner;

      public EnumerateOnce(IEnumerable<T> enumerable)
      {
        ArgumentUtility.CheckForNull<IEnumerable<T>>(enumerable, nameof (enumerable));
        this.inner = enumerable;
      }

      public IEnumerator<T> GetEnumerator()
      {
        IEnumerator<T> enumerator = this.inner != null ? this.inner.GetEnumerator() : throw new InvalidOperationException("This has already been enumerated.");
        this.inner = (IEnumerable<T>) null;
        return enumerator;
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
    }
  }
}
