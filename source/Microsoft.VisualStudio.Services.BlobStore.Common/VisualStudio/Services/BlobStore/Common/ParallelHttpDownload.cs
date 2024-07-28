// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ParallelHttpDownload
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class ParallelHttpDownload
  {
    public static async Task<long> Download(
      ParallelHttpDownload.DownloadConfiguration config,
      Uri uri,
      StreamWithRange httpStream,
      ParallelHttpDownload.StreamSegmentFactory segmentFactory,
      string destinationPath,
      FileMode mode,
      ParallelHttpDownload.LogSegmentStart logSegmentStart,
      ParallelHttpDownload.LogSegmentStop logSegmentStop,
      ParallelHttpDownload.LogSegmentFailed logSegmentFailed,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<ParallelHttpDownload.DownloadConfiguration>(config, nameof (config));
      ArgumentUtility.CheckStringForNullOrEmpty(destinationPath, nameof (destinationPath));
      ArgumentUtility.CheckForNull<ParallelHttpDownload.LogSegmentStart>(logSegmentStart, nameof (logSegmentStart));
      ArgumentUtility.CheckForNull<ParallelHttpDownload.LogSegmentStop>(logSegmentStop, nameof (logSegmentStop));
      ArgumentUtility.CheckForNull<ParallelHttpDownload.LogSegmentFailed>(logSegmentFailed, nameof (logSegmentFailed));
      ArgumentUtility.CheckForNull<ParallelHttpDownload.StreamSegmentFactory>(segmentFactory, nameof (segmentFactory));
      bool success = false;
      try
      {
        long length = httpStream.Range.WholeLength;
        using (FileStream wholeFile = FileStreamUtils.OpenFileStreamForAsync(destinationPath, mode, FileAccess.Write, FileShare.Write))
        {
          if (length > config.SegmentSizeInBytes)
          {
            bool sparseFile = false;
            if (config.UseSparseFiles)
            {
              int num;
              if ((num = AsyncFile.TryMarkSparse(wholeFile.SafeFileHandle, true)) != 0)
                throw new IOException("Could not mark file as sparse: " + num.ToString());
              sparseFile = true;
            }
            wholeFile.SetLength(length);
            await ParallelHttpDownload.DownloadInSegmentsAsync(config, uri, httpStream, segmentFactory, wholeFile, destinationPath, length, logSegmentStart, logSegmentStop, logSegmentFailed, cancellationToken).ConfigureAwait(false);
            success = true;
            int num1;
            if (sparseFile && (num1 = AsyncFile.TryMarkSparse(wholeFile.SafeFileHandle, false)) != 0)
              throw new IOException("Could not unmark file as sparse: " + num1.ToString());
            return length;
          }
          long num2 = await ParallelHttpDownload.DownloadSegmentAsync(config, uri, httpStream, segmentFactory, wholeFile, destinationPath, logSegmentStart, logSegmentStop, logSegmentFailed, cancellationToken).ConfigureAwait(false);
          success = true;
          return num2;
        }
      }
      finally
      {
        int num;
        if (num < 0 && !success && File.Exists(destinationPath))
          File.Delete(destinationPath);
      }
    }

    private static async Task DownloadInSegmentsAsync(
      ParallelHttpDownload.DownloadConfiguration config,
      Uri uri,
      StreamWithRange httpSegmentStream,
      ParallelHttpDownload.StreamSegmentFactory segmentStreamFactory,
      FileStream wholeFile,
      string destinationPath,
      long fileLength,
      ParallelHttpDownload.LogSegmentStart logSegmentStart,
      ParallelHttpDownload.LogSegmentStop logSegmentStop,
      ParallelHttpDownload.LogSegmentFailed logSegmentFailed,
      CancellationToken cancellationToken)
    {
      int count = (int) ((fileLength + config.SegmentSizeInBytes - 1L) / config.SegmentSizeInBytes);
      Func<int, Task> action = (Func<int, Task>) (async segmentIndex =>
      {
        long offset = config.SegmentSizeInBytes * (long) segmentIndex;
        long length = Math.Min(config.SegmentSizeInBytes, fileLength - offset);
        ParallelHttpDownload.DownloadConfiguration config1 = config;
        Uri uri1 = uri;
        StreamWithRange httpSegmentStream1;
        if (segmentIndex == 0)
          httpSegmentStream1 = httpSegmentStream;
        else
          httpSegmentStream1 = await segmentStreamFactory(offset, length, cancellationToken);
        long num = await ParallelHttpDownload.DownloadSegmentAsync(config1, uri1, httpSegmentStream1, segmentStreamFactory, wholeFile, destinationPath, logSegmentStart, logSegmentStop, logSegmentFailed, cancellationToken).ConfigureAwait(false);
        config1 = (ParallelHttpDownload.DownloadConfiguration) null;
        uri1 = (Uri) null;
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = config.MaxParallelSegmentDownloadsPerFile;
      dataflowBlockOptions.CancellationToken = cancellationToken;
      await NonSwallowingActionBlock.Create<int>(action, dataflowBlockOptions).SendAllAndCompleteSingleBlockNetworkAsync<int>(Enumerable.Range(0, count), cancellationToken).ConfigureAwait(false);
    }

    private static async Task<long> DownloadSegmentAsync(
      ParallelHttpDownload.DownloadConfiguration config,
      Uri uri,
      StreamWithRange httpSegmentStream,
      ParallelHttpDownload.StreamSegmentFactory segmentStreamFactory,
      FileStream wholeFile,
      string destinationPath,
      ParallelHttpDownload.LogSegmentStart logSegmentStart,
      ParallelHttpDownload.LogSegmentStop logSegmentStop,
      ParallelHttpDownload.LogSegmentFailed logSegmentFailed,
      CancellationToken cancellationToken)
    {
      long totalBytesRead = 0;
      long startPosition = httpSegmentStream.Range.FirstBytePositionInclusive;
      long currentPosition = startPosition;
      long endPosition = httpSegmentStream.Range.LastBytePositionInclusive + 1L;
      StreamWithRange? streamInUse = new StreamWithRange?(httpSegmentStream);
      Func<string> func1;
      Func<string> func2;
      await AsyncHttpRetryHelper.InvokeVoidAsync((Func<Task>) (async () =>
      {
        logSegmentStart(destinationPath, startPosition, new long?(endPosition));
        StreamWithRange? nullable = streamInUse;
        StreamWithRange streamWithRange;
        if (nullable.HasValue)
          streamWithRange = nullable.GetValueOrDefault();
        else
          streamWithRange = await segmentStreamFactory(currentPosition, (long) (int) (endPosition - currentPosition), cancellationToken).ConfigureAwait(false);
        streamInUse = new StreamWithRange?(streamWithRange);
        try
        {
          using (CancellationTokenSource segmentTimeoutCancellation = new CancellationTokenSource(config.SegmentDownloadTimeout))
          {
            using (CancellationTokenSource segmentTimeoutLinked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, segmentTimeoutCancellation.Token))
            {
              Pool<byte[]>.PoolHandle buffer = config.BufferPool.Get();
              try
              {
                while (currentPosition < endPosition)
                {
                  int count = (int) Math.Min((long) buffer.Value.Length, endPosition - currentPosition);
                  int bytesRead;
                  using (CancellationTokenSource readBufferTimeoutCancellation = new CancellationTokenSource(config.ReadBufferTimeout))
                  {
                    using (CancellationTokenSource readBufferTimeoutLinked = CancellationTokenSource.CreateLinkedTokenSource(segmentTimeoutLinked.Token, readBufferTimeoutCancellation.Token))
                      bytesRead = await streamInUse.Value.Stream.ReadToBufferAsync(new ArraySegment<byte>(buffer.Value, 0, count), readBufferTimeoutLinked.Token).EnforceCancellation<int>(readBufferTimeoutLinked.Token, func1 ?? (func1 = (Func<string>) (() => "Timed out reading from '" + uri.AbsoluteUri + "'.")), "D:\\a\\_work\\1\\s\\ArtifactServices\\Shared\\BlobStore.Common\\ParallelHttpDownload.cs", nameof (DownloadSegmentAsync), 256).ConfigureAwait(false);
                  }
                  if (bytesRead != 0)
                  {
                    await AsyncFile.WriteAsync(wholeFile, currentPosition, new ArraySegment<byte>(buffer.Value, 0, bytesRead)).EnforceCancellation(segmentTimeoutLinked.Token, func2 ?? (func2 = (Func<string>) (() => "Timed out writing to '" + wholeFile.Name + "'.")), "D:\\a\\_work\\1\\s\\ArtifactServices\\Shared\\BlobStore.Common\\ParallelHttpDownload.cs", nameof (DownloadSegmentAsync), 266).ConfigureAwait(false);
                    currentPosition += (long) bytesRead;
                    totalBytesRead += (long) bytesRead;
                  }
                  else
                    break;
                }
              }
              finally
              {
                buffer.Dispose();
              }
              buffer = new Pool<byte[]>.PoolHandle();
            }
          }
        }
        finally
        {
          ref StreamWithRange? local = ref streamInUse;
          if (local.HasValue)
            local.GetValueOrDefault().Dispose();
          streamInUse = new StreamWithRange?();
        }
      }), config.MaxSegmentDownloadRetries, (IAppTraceSource) new CallbackAppTraceSource((Action<string>) (message => logSegmentFailed(destinationPath, startPosition, new long?(endPosition), message)), SourceLevels.Information), cancellationToken, false, string.Format("{0} [{1}-{2}]", (object) uri, (object) startPosition, (object) endPosition));
      logSegmentStop(destinationPath, startPosition, startPosition + totalBytesRead);
      long num = endPosition - startPosition;
      return num == totalBytesRead ? totalBytesRead : throw new EndOfStreamException(string.Format("Reached end of stream at {0} bytes of the {1} expected.", (object) totalBytesRead, (object) num));
    }

    [JsonConverter(typeof (DownloadConfigurationConverter))]
    public sealed class DownloadConfiguration
    {
      private const int DefaultSegmentDownloadTimeoutInMinutes = 10;
      private const long DefaultParallelDownloadSegmentSizeInBytes = 8388608;
      private const int DefaultMaxParallelSegmentDownloadsPerFile = 16;
      private const int DefaultMaxSegmentDownloadRetries = 5;
      private const int DefaultReadBufferTimeoutInSeconds = 60;
      private const int DefaultReadSizeInBytes = 65536;
      public const string DefaultEnvironmentVariablePrefix = "VSO_AS_HTTP_";
      public readonly TimeSpan SegmentDownloadTimeout;
      public readonly long SegmentSizeInBytes;
      public readonly int MaxParallelSegmentDownloadsPerFile;
      public readonly int MaxSegmentDownloadRetries;
      public readonly TimeSpan ReadBufferTimeout;
      public readonly int ReadBufferSize;
      [JsonIgnore]
      [JsonProperty(Required = Required.Default)]
      public readonly ByteArrayPool BufferPool;
      public readonly bool UseSparseFiles;

      public static ParallelHttpDownload.DownloadConfiguration ReadFromEnvironment(
        string environmentVariablePrefix = "VSO_AS_HTTP_")
      {
        return ParallelHttpDownload.DownloadConfiguration.ReadFromEnvironment(environmentVariablePrefix, new bool?());
      }

      public static ParallelHttpDownload.DownloadConfiguration ReadFromEnvironment(
        string environmentVariablePrefix,
        bool? useSparseFiles)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(environmentVariablePrefix, nameof (environmentVariablePrefix));
        string environmentVariable1 = Environment.GetEnvironmentVariable(environmentVariablePrefix + "SegmentDownloadTimeoutInMinutes");
        int num;
        if (environmentVariable1 == null)
        {
          num = 10;
          environmentVariable1 = num.ToString();
        }
        TimeSpan segmentDownloadTimeout = TimeSpan.FromMinutes((double) int.Parse(environmentVariable1));
        long segmentSizeInBytes = long.Parse(Environment.GetEnvironmentVariable(environmentVariablePrefix + "ParallelDownloadSegmentSizeInBytes") ?? 8388608L.ToString());
        string environmentVariable2 = Environment.GetEnvironmentVariable(environmentVariablePrefix + "MaxParallelSegmentDownloadsPerFile");
        if (environmentVariable2 == null)
        {
          num = 16;
          environmentVariable2 = num.ToString();
        }
        int maxParallelSegmentDownloadsPerFile = int.Parse(environmentVariable2);
        string environmentVariable3 = Environment.GetEnvironmentVariable(environmentVariablePrefix + "MaxSegmentDownloadRetries");
        if (environmentVariable3 == null)
        {
          num = 5;
          environmentVariable3 = num.ToString();
        }
        int maxSegmentDownloadRetries = int.Parse(environmentVariable3);
        string environmentVariable4 = Environment.GetEnvironmentVariable(environmentVariablePrefix + "ReadBufferTimeoutInSeconds");
        if (environmentVariable4 == null)
        {
          num = 60;
          environmentVariable4 = num.ToString();
        }
        TimeSpan readBufferTimeout = TimeSpan.FromSeconds((double) int.Parse(environmentVariable4));
        string environmentVariable5 = Environment.GetEnvironmentVariable(environmentVariablePrefix + "ReadSizeInBytes");
        if (environmentVariable5 == null)
        {
          num = 65536;
          environmentVariable5 = num.ToString();
        }
        int readBufferSize = int.Parse(environmentVariable5);
        bool? useSparseFiles1 = new bool?(((int) useSparseFiles ?? (Environment.GetEnvironmentVariable("VSTS_SPARSE_FILES") == "1" ? 1 : 0)) != 0);
        return new ParallelHttpDownload.DownloadConfiguration(segmentDownloadTimeout, segmentSizeInBytes, maxParallelSegmentDownloadsPerFile, maxSegmentDownloadRetries, readBufferTimeout, readBufferSize, useSparseFiles1);
      }

      public DownloadConfiguration(
        TimeSpan segmentDownloadTimeout,
        long segmentSizeInBytes,
        int maxParallelSegmentDownloadsPerFile,
        int maxSegmentDownloadRetries,
        TimeSpan readBufferTimeout,
        int readBufferSize)
        : this(segmentDownloadTimeout, segmentSizeInBytes, maxParallelSegmentDownloadsPerFile, maxSegmentDownloadRetries, readBufferTimeout, readBufferSize, new bool?())
      {
      }

      [JsonConstructor]
      public DownloadConfiguration(
        TimeSpan segmentDownloadTimeout,
        long segmentSizeInBytes,
        int maxParallelSegmentDownloadsPerFile,
        int maxSegmentDownloadRetries,
        TimeSpan readBufferTimeout,
        int readBufferSize,
        bool? useSparseFiles)
      {
        this.SegmentDownloadTimeout = segmentDownloadTimeout;
        this.SegmentSizeInBytes = segmentSizeInBytes;
        this.MaxParallelSegmentDownloadsPerFile = maxParallelSegmentDownloadsPerFile;
        this.MaxSegmentDownloadRetries = maxSegmentDownloadRetries;
        this.ReadBufferTimeout = readBufferTimeout;
        this.ReadBufferSize = readBufferSize;
        this.BufferPool = new ByteArrayPool(this.ReadBufferSize, 4 * Environment.ProcessorCount);
        this.UseSparseFiles = ((int) useSparseFiles ?? (Environment.GetEnvironmentVariable("VSTS_SPARSE_FILES") == "1" ? 1 : 0)) != 0;
      }
    }

    public delegate Task<StreamWithRange> StreamSegmentFactory(
      long offset,
      long length,
      CancellationToken cancellationToken);

    public delegate void LogSegmentStart(string destinationPath, long startOffset, long? endOffset);

    public delegate void LogSegmentStop(string destinationPath, long startOffset, long endOffset);

    public delegate void LogSegmentFailed(
      string destinationPath,
      long startOffset,
      long? endOffset,
      string message);
  }
}
