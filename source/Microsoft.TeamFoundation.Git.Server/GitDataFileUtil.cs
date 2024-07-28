// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitDataFileUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Bitmap;
using Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring;
using Microsoft.TeamFoundation.Git.Server.Graph;
using Microsoft.TeamFoundation.Git.Server.PackIndex;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitDataFileUtil
  {
    private const int c_maxLocalFileLoadAttempts = 10;
    private const string c_layer = "GitDataFileUtil";

    public static Sha1Id WriteIndex(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobPrv,
      OdbId odbId,
      GitKnownFilesBuilder knownFiles,
      GitPackIndexer indexer,
      IPackIndexMergeStrategy mergeStrategy,
      IGitPackIndexWriter overrideWriter = null)
    {
      if (overrideWriter == null)
        overrideWriter = GitDataFileUtil.GetPackIndexWriter(rc);
      return GitDataFileUtil.WriteIndexRaw(rc, blobPrv, odbId, knownFiles, (Action<Sha1Id, Stream>) ((id, stream) => overrideWriter.Write(indexer, id, stream, mergeStrategy)));
    }

    internal static Sha1Id WriteIndexRaw(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobPrv,
      OdbId odbId,
      GitKnownFilesBuilder knownFiles,
      Action<Sha1Id, Stream> write)
    {
      return GitDataFileUtil.CreateAndUploadOdbFile(rc, blobPrv, odbId, KnownFileType.Index, knownFiles, write);
    }

    public static Sha1Id WriteGraph(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobPrv,
      OdbId odbId,
      GitKnownFilesBuilder knownFiles,
      GitCommitGraph graph)
    {
      IGitCommitGraphWriter graphWriter = (IGitCommitGraphWriter) new GitCommitGraph.M101Format();
      return GitDataFileUtil.CreateAndUploadOdbFile(rc, blobPrv, odbId, KnownFileType.Graph, knownFiles, (Action<Sha1Id, Stream>) ((id, stream) => graphWriter.Write(graph, stream)));
    }

    public static Sha1Id WriteBitmapCollection(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobPrv,
      OdbId odbId,
      GitKnownFilesBuilder knownFiles,
      ReachabilityBitmapCollection collection)
    {
      IReachabilityBitmapFormat format = !rc.IsFeatureEnabled("Git.Bitmap.UseM161Format") ? (IReachabilityBitmapFormat) new ReachabilityBitmapCollection.M119Format() : (IReachabilityBitmapFormat) new ReachabilityBitmapCollection.M161Format();
      return GitDataFileUtil.CreateAndUploadOdbFile(rc, blobPrv, odbId, KnownFileType.ReachabilityBitmapCollection, knownFiles, (Action<Sha1Id, Stream>) ((id, stream) => format.Write(collection, stream)));
    }

    public static Guid WriteBitmap<T>(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobPrv,
      OdbId odbId,
      GitKnownFilesBuilder knownFiles,
      IsolationBitmapType type,
      RoaringBitmap<T> bitmap)
    {
      RoaringBitmap<T>.M123RiffFormat format = new RoaringBitmap<T>.M123RiffFormat(bitmap.FullObjectList);
      using (ByteArray buffer = new ByteArray(GitStreamUtil.OptimalBufferSize))
        return GitDataFileUtil.CreateAndUploadRepoFile(rc, blobPrv, odbId, type.ToKnownFileType(), knownFiles, (Action<Guid, Stream>) ((id, stream) => format.Write(bitmap, stream, buffer.Bytes)));
    }

    public static Sha1Id WritePackFile(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobPrv,
      OdbId odbId,
      GitKnownFilesBuilder knownFiles,
      bool isDerived,
      Action<Sha1Id, Stream> write)
    {
      return GitDataFileUtil.CreateAndUploadOdbFile(rc, blobPrv, odbId, isDerived ? KnownFileType.DerivedPackFile : KnownFileType.RawPackfile, knownFiles, write);
    }

    public static IGitPackIndexWriter GetPackIndexWriter(IVssRequestContext rc)
    {
      GitPackIndexVersion version = (GitPackIndexVersion) rc.GetService<ITeamFoundationResourceManagementService>().GetServiceVersion(rc, "GitIndex", "Git").Version;
      switch (version)
      {
        case GitPackIndexVersion.M88:
          return (IGitPackIndexWriter) new M44GitPackIndexWriter(GitPackIndexVersion.M88);
        case GitPackIndexVersion.M91:
          return (IGitPackIndexWriter) new M91GitPackIndexWriter(rc);
        default:
          throw new NotSupportedException(string.Format("{0}: {1}.", (object) "version", (object) version));
      }
    }

    private static Sha1Id CreateAndUploadOdbFile(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      KnownFileType fileType,
      GitKnownFilesBuilder knownFiles,
      Action<Sha1Id, Stream> write)
    {
      Sha1Id uniqueId = StorageUtils.CreateUniqueId();
      string odbFileName = StorageUtils.GetOdbFileName(uniqueId, fileType);
      GitDataFileUtil.CreateAndUpload<Sha1Id>(rc, blobProvider, odbId, fileType, knownFiles, write, uniqueId, odbFileName);
      return uniqueId;
    }

    private static Guid CreateAndUploadRepoFile(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      KnownFileType fileType,
      GitKnownFilesBuilder knownFiles,
      Action<Guid, Stream> write)
    {
      Guid andUploadRepoFile = Guid.NewGuid();
      string repoFileName = StorageUtils.GetRepoFileName(andUploadRepoFile, fileType);
      GitDataFileUtil.CreateAndUpload<Guid>(rc, blobProvider, odbId, fileType, knownFiles, write, andUploadRepoFile, repoFileName);
      return andUploadRepoFile;
    }

    private static void CreateAndUpload<T>(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      KnownFileType fileType,
      GitKnownFilesBuilder knownFiles,
      Action<T, Stream> write,
      T id,
      string name)
    {
      string path = Path.Combine(GitServerUtils.GetCacheDirectory(rc, odbId.Value), name);
      bool flag = GitServerUtils.CanUseFileCachingService(rc);
      int optimalBufferSize = GitStreamUtil.OptimalBufferSize;
      int options = flag ? 0 : 67108864;
      using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Read | System.IO.FileShare.Delete, optimalBufferSize, (FileOptions) options))
      {
        try
        {
          write(id, (Stream) fileStream);
          fileStream.Flush(true);
          if (flag)
            rc.GetService<ITeamFoundationFileCacheService>().UpdateCacheSize(rc, rc.ServiceHost.InstanceId.ToString(), fileStream.Length, 1);
          fileStream.Seek(0L, SeekOrigin.Begin);
          rc.Trace(1013062, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "Uploading data file {0}", (object) name);
          blobProvider.PutStream(rc, odbId, name, (Stream) fileStream, fileStream.Length);
          rc.Trace(1013104, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "Upload complete.");
          knownFiles.QueueExtant(name, fileType);
        }
        catch
        {
          Microsoft.TeamFoundation.Common.Internal.NativeMethods.DeleteFile(fileStream.Name);
          throw;
        }
      }
    }

    public static GitDataFile Retrieve(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      string resourceId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<ITfsGitBlobProvider>(blobProvider, nameof (blobProvider));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
      rc.TraceConditionally(1013085, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitDataFileUtil), (Func<string>) (() => string.Format("Entering {0}. {1}: {2} {3}: {4}", (object) nameof (Retrieve), (object) nameof (odbId), (object) odbId, (object) nameof (resourceId), (object) resourceId)));
      FileStream fileStream = (FileStream) null;
      MemoryMappedFile file = (MemoryMappedFile) null;
      try
      {
        fileStream = GitDataFileUtil.GetLocalStream(rc, blobProvider, odbId, resourceId);
        file = MemoryMappedFile.CreateFromFile(fileStream, (string) null, fileStream.Length, MemoryMappedFileAccess.Read, (MemoryMappedFileSecurity) null, HandleInheritability.None, false);
        return new GitDataFile(file, fileStream.Length, fileStream.Name);
      }
      catch
      {
        fileStream?.Dispose();
        file?.Dispose();
        throw;
      }
      finally
      {
        rc.Trace(1013091, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "Exiting Retrieve.");
      }
    }

    private static FileStream GetLocalStream(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      string resourceId)
    {
      using (rc.TraceBlock(1013895, 1013896, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "GetLocalStream." + resourceId))
      {
        FileStream localStream = (FileStream) null;
        if (!GitDataFileUtil.TryGetLocalStreamFromCache(rc, blobProvider, odbId, resourceId, out localStream))
        {
          try
          {
            localStream = new FileStream(Path.Combine(FileSpec.GetTempDirectory(), Guid.NewGuid().ToString("N")), FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Read | System.IO.FileShare.Delete, 65536, FileOptions.DeleteOnClose);
            GitDataFileUtil.DoCopyFromBlobStorage(rc, blobProvider, odbId, resourceId, localStream);
            localStream.Flush();
          }
          catch
          {
            localStream?.Dispose();
            throw;
          }
        }
        return localStream;
      }
    }

    private static bool TryGetLocalStreamFromCache(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      string resourceId,
      out FileStream localStream)
    {
      using (rc.TraceBlock(1013897, 1013898, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "TryGetLocalStreamFromCache." + resourceId))
      {
        localStream = (FileStream) null;
        if (!GitServerUtils.CanUseFileCachingService(rc))
          return false;
        string str = Path.Combine(GitServerUtils.GetCacheDirectory(rc, odbId.Value), resourceId);
        for (int attemptNum = 0; localStream == null && attemptNum < 10; ++attemptNum)
        {
          TraceLevel level = attemptNum > 0 ? TraceLevel.Error : TraceLevel.Info;
          if (File.Exists(str))
          {
            try
            {
              localStream = new FileStream(str, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            }
            catch (IOException ex)
            {
              rc.Trace(1013138, level, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "Cached file {0} got cleaned out from under us, we need to reload it.", (object) resourceId);
              rc.TraceException(1013138, level, GitServerUtils.TraceArea, nameof (GitDataFileUtil), (Exception) ex);
            }
          }
          else
            rc.Trace(1013138, level, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "Cached file {0} not found. Attempt #{1}", (object) resourceId, (object) (attemptNum + 1));
          if (localStream == null)
            GitDataFileUtil.CacheResourceToDisk(rc, blobProvider, odbId, resourceId, str, attemptNum);
        }
        if (localStream == null)
          throw new GitPackDoesNotExistException(resourceId + ": c_maxLocalFileLoadAttempts reached.");
        return true;
      }
    }

    private static void CacheResourceToDisk(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      string resourceId,
      string resourcePath,
      int attemptNum)
    {
      using (rc.TraceBlock(1013899, 1013900, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "CacheResourceToDisk." + resourceId))
      {
        string str = Path.Combine(GitServerUtils.GetCacheDirectory(rc, odbId.Value), Guid.NewGuid().ToString("N"));
        bool flag = false;
        long position;
        try
        {
          using (FileStream destination = new FileStream(str, FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Read | System.IO.FileShare.Delete, GitStreamUtil.OptimalBufferSize))
          {
            GitDataFileUtil.DoCopyFromBlobStorage(rc, blobProvider, odbId, resourceId, destination);
            destination.Flush(true);
            position = destination.Position;
          }
          try
          {
            File.Move(str, resourcePath);
            flag = true;
          }
          catch (IOException ex)
          {
            rc.Trace(1013140, attemptNum > 0 ? TraceLevel.Error : TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "Error in moving temp file -- this can happen in a write after write conflict,  and may be ignored. Attempt #{0} Error: {1}", (object) (attemptNum + 1), (object) ex);
          }
        }
        finally
        {
          if (!flag)
            Microsoft.TeamFoundation.Common.Internal.NativeMethods.DeleteFile(str);
        }
        if (!flag)
          return;
        try
        {
          rc.GetService<ITeamFoundationFileCacheService>().UpdateCacheSize(rc, rc.ServiceHost.InstanceId.ToString(), position, 1);
        }
        catch (Exception ex)
        {
          rc.Trace(1013157, TraceLevel.Error, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "Error in updating the file cache statistics. This may cause the cache to not clean up as quickly. Error: {0}", (object) ex);
        }
      }
    }

    private static void DoCopyFromBlobStorage(
      IVssRequestContext rc,
      ITfsGitBlobProvider blobProvider,
      OdbId odbId,
      string resourceId,
      FileStream destination)
    {
      using (rc.TraceBlock(1013064, 1013065, GitServerUtils.TraceArea, nameof (GitDataFileUtil), "DoCopyFromBlobStorage." + resourceId))
      {
        if (!blobProvider.DownloadToStream(rc, odbId, resourceId, (Stream) destination, false))
          throw new GitPackDoesNotExistException(resourceId);
      }
    }
  }
}
