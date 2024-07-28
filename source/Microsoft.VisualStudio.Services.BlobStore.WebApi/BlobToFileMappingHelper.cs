// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.BlobToFileMappingHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public static class BlobToFileMappingHelper
  {
    public static async Task<IEnumerable<BlobToFileMapping>> GetMissingFileMappingsInParallel(
      this IEnumerable<BlobToFileMapping> mappings,
      IAppTraceSource tracer,
      CancellationToken cancellationToken)
    {
      List<BlobToFileMapping> missingFiles = new List<BlobToFileMapping>();
      Action<BlobToFileMapping> action = (Action<BlobToFileMapping>) (mapping =>
      {
        if (mapping.SymbolicLinkTargetPath != null || string.IsNullOrEmpty(mapping.FilePath) || File.Exists(Path.GetFullPath(mapping.FilePath)))
          return;
        lock (missingFiles)
          missingFiles.Add(mapping);
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions.CancellationToken = cancellationToken;
      ActionBlock<BlobToFileMapping> targetBlock = NonSwallowingActionBlock.Create<BlobToFileMapping>(action, dataflowBlockOptions);
      tracer.Info("Validating existence of all non-empty BlobToFileMapping.FilePaths");
      Stopwatch timer = Stopwatch.StartNew();
      IEnumerable<BlobToFileMapping> inputs = mappings;
      CancellationToken cancellationToken1 = cancellationToken;
      await targetBlock.PostAllToUnboundedAndCompleteAsync<BlobToFileMapping>(inputs, cancellationToken1).ConfigureAwait(false);
      timer.Stop();
      tracer.Info(string.Format("Validation of file mappings finished in {0} ms.", (object) timer.ElapsedMilliseconds));
      IEnumerable<BlobToFileMapping> mappingsInParallel = (IEnumerable<BlobToFileMapping>) missingFiles;
      timer = (Stopwatch) null;
      return mappingsInParallel;
    }

    public static void RetryMissingMappings(
      this IEnumerable<BlobToFileMapping> missingFileMappings,
      IAppTraceSource tracer,
      CancellationToken cancellationToken)
    {
      if (missingFileMappings.Count<BlobToFileMapping>() > 100)
        throw new BlobMappingsMissingFilesException(string.Format("We were not able to verify all file exist on the disk as expected.  ${0} files were determined to be missing.  This may indicate disk problems, an issue during downloading, or a problem with the cache.  Please check your logs for additional errors or warnings.", (object) missingFileMappings.Count<BlobToFileMapping>()));
      int num1 = 0;
      foreach (BlobToFileMapping missingFileMapping in missingFileMappings)
      {
        string fullPath = Path.GetFullPath(missingFileMapping.FilePath);
        try
        {
          using (File.OpenRead(fullPath))
            tracer.Warn("The system returned inconsistent results during the existence check for " + fullPath + " (original path " + missingFileMapping.FilePath + ").  This may indicate the disk drive is under pressure.  We were able to recover but recommend you reduce disk activity if possible.");
          ++num1;
        }
        catch (FileNotFoundException ex)
        {
          tracer.Error("We could not find the file at " + fullPath + " (original path " + missingFileMapping.FilePath + ").  This may indicate a problem downloading or pulling a file from the cache.", (object) ex);
        }
        catch (DirectoryNotFoundException ex)
        {
          tracer.Error("We could not find the directory at " + fullPath + " (original path " + missingFileMapping.FilePath + ").  This may indicate a problem downloading or pulling a file from the cache.", (object) ex);
        }
        catch (Exception ex)
        {
          tracer.Error("The system returned inconsistent results during the existence check for " + fullPath + " (original path " + missingFileMapping.FilePath + ").  The check returned the following error: " + ex.Message + ", but we could not determine if the file exists or not.", (object) ex);
        }
      }
      if (num1 > 0)
        Trace.TraceWarning(string.Format("We recovered from ${0} possible disk failures.  Please check the logs for additional warnings and errors.", (object) num1));
      int num2 = missingFileMappings.Count<BlobToFileMapping>() - num1;
      if (num2 > 0)
        throw new BlobMappingsMissingFilesException(string.Format("We were not able to verify all file exist on the disk as expected.  ${0} files were determined to be missing.  This may indicate disk problems, an issue during downloading, or a problem with the cache.  Please check your logs for additional errors or warnings.", (object) num2));
    }

    public static async Task<bool> HashMatchesFileContent(
      this BlobToFileMapping mapping,
      bool chunkDedup,
      IList<ChunkInfo> fileChunkedInfo,
      IAppTraceSource tracer,
      CancellationToken cancellationToken)
    {
      if (!File.Exists(Path.GetFullPath(mapping.FilePath)))
        return false;
      if (chunkDedup && fileChunkedInfo == null)
        throw new ArgumentNullException(nameof (fileChunkedInfo));
      string expectedHash = mapping.BlobId.AlgorithmResultString;
      tracer.Verbose("Validating the expected hash of the file " + expectedHash + " matches the actual hash of the file at: " + mapping.FilePath);
      Stopwatch timer = Stopwatch.StartNew();
      bool flag;
      if (chunkDedup)
      {
        flag = await ChunkDedupedFileContentHashVerifier.VerifyFileAsync(mapping.FilePath, fileChunkedInfo, new ChunkDedupedFileContentHash(expectedHash), cancellationToken).ConfigureAwait(false);
      }
      else
      {
        using (FileStream stream = FileStreamUtils.OpenFileStreamForAsync(mapping.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete))
          flag = expectedHash == (await Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.CalculateBlobIdentifierWithBlocksAsync((Stream) stream).ConfigureAwait(false)).BlobId.AlgorithmResultString;
      }
      timer.Stop();
      string str = string.Format("Validation of file hash finished {0} ms. The file path is: {1}. ", (object) timer.ElapsedMilliseconds, (object) mapping.FilePath);
      if (!flag)
      {
        tracer.Verbose(str + "The file hash does not match.");
        return false;
      }
      tracer.Verbose(str + "The file hash does match.");
      return true;
    }
  }
}
