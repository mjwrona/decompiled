// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.FileImportHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure;
using Microsoft.Azure.Storage;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.DataImport;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.DataImport;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class FileImportHelper : IDisposable
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly CancellationToken m_cancellationToken;
    private readonly IServicingContext m_servicingContext;
    private readonly IFileImportSqlProvider m_sqlProvider;
    private readonly IFileImportBlobProvider m_blobProvider;
    private readonly ITFLogger m_logger;
    private readonly IDisposableReadOnlyList<IDataImportFileExtension> m_dataImportFileExtensions;
    private readonly Guid m_hostId;
    private readonly Guid m_importId;
    private readonly IDataImportStepStatusScope m_stepStatusScope;
    private long m_tableContentSizeInBytes;
    private long m_updateFileTransferProgressThresholdInBytes;
    private int m_maxBlobCopies;
    private int m_batchSize;
    private const string c_area = "DataImport";
    private const string c_layer = "FileImportHelper";
    private readonly FileImportWatermark m_watermark;
    private readonly ManualResetEventSlim m_fileImportTrackingManualResetEvent;
    private readonly ConcurrentDictionary<string, FileImportTrackingInfo> m_fileImportTrackingInfo;
    private readonly bool m_isLongRunningTasksEnabled;
    internal int m_msToWaitForUploadBeforeWarning = 60000;

    public FileImportHelper(
      IVssRequestContext requestContext,
      IServicingContext servicingContext,
      ISqlConnectionInfo sourceConnectionInfo,
      ISqlConnectionInfo targetConnectionInfo,
      IDataImportStepStatusScope stepStatusScope,
      int maxBlobCopies = 80,
      int batchSize = 1920)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IServicingContext>(servicingContext, nameof (servicingContext));
      ArgumentUtility.CheckForNull<ITFLogger>(servicingContext.TFLogger, "TFLogger");
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(sourceConnectionInfo, nameof (sourceConnectionInfo));
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(targetConnectionInfo, nameof (targetConnectionInfo));
      ArgumentUtility.CheckForNull<IDataImportStepStatusScope>(stepStatusScope, nameof (stepStatusScope));
      this.m_requestContext = requestContext;
      this.m_cancellationToken = this.m_requestContext.CancellationToken;
      this.m_servicingContext = servicingContext;
      this.m_logger = servicingContext.TFLogger;
      this.m_maxBlobCopies = maxBlobCopies;
      this.m_batchSize = batchSize;
      this.m_hostId = requestContext.ServiceHost.InstanceId;
      this.m_importId = servicingContext.GetDataImportId();
      this.m_stepStatusScope = stepStatusScope;
      this.m_blobProvider = this.CreateBlobProvider(this.m_requestContext);
      this.m_sqlProvider = this.CreateFileProvider(sourceConnectionInfo, targetConnectionInfo);
      this.m_dataImportFileExtensions = this.GetDataImportFileExtensions(sourceConnectionInfo);
      this.m_watermark = FileImportWatermark.Read(this.m_servicingContext);
      this.m_fileImportTrackingManualResetEvent = new ManualResetEventSlim();
      this.m_fileImportTrackingInfo = new ConcurrentDictionary<string, FileImportTrackingInfo>();
      this.m_isLongRunningTasksEnabled = this.m_requestContext.IsFeatureEnabled("VisualStudio.Services.DataImport.FileImportHelperLongRunningTasks");
    }

    public void ImportFiles()
    {
      this.m_logger.Info(string.Format("TargetHostId     {0}", (object) this.m_hostId));
      this.m_logger.Info("Target container " + this.m_blobProvider.ContainerName);
      this.m_logger.Info(string.Format("MaxBlobCopies    {0}", (object) this.m_maxBlobCopies));
      this.m_logger.Info(string.Format("File Batch Size  {0}", (object) this.m_batchSize));
      this.m_logger.Info("Source Database  " + this.m_sqlProvider.SourceSqlConnectionInfo.DataSource + ";" + this.m_sqlProvider.SourceSqlConnectionInfo.InitialCatalog);
      this.m_logger.Info("Target Database  " + this.m_sqlProvider.TargetSqlConnectionInfo.DataSource + ";" + this.m_sqlProvider.TargetSqlConnectionInfo.InitialCatalog);
      this.m_logger.Info(string.Format("Watermark        {0}", (object) this.m_watermark));
      this.m_tableContentSizeInBytes = this.m_sqlProvider.GetContentSize();
      this.m_updateFileTransferProgressThresholdInBytes = Math.Min(524288000L, this.m_tableContentSizeInBytes / 10L);
      this.m_logger.Info(string.Format("Approximate blob bytes {0}", (object) this.m_tableContentSizeInBytes));
      this.UpdateFileTransferProgress();
      IVssRequestContext deploymentContext = this.m_requestContext.To(TeamFoundationHostType.Deployment);
      (int, int) minMaxFileId = this.m_sqlProvider.GetMinMaxFileId();
      this.m_logger.Info(string.Format("Minimum fileId: {0}, maximum fileId: {1}", (object) minMaxFileId.Item1, (object) minMaxFileId.Item2));
      bool blockFileImportTrackingThreadFromExiting = false;
      Task task = (Task) null;
      if (this.m_isLongRunningTasksEnabled)
      {
        blockFileImportTrackingThreadFromExiting = true;
        task = Task.Factory.StartNew(new Action(FileImportTrackingTaskAction), this.m_cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
      }
      int num1 = 0;
      DateTime utcNow = DateTime.UtcNow;
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<FileStatistics> files;
      do
      {
        this.CheckForParallelOptionChanges(deploymentContext);
        files = this.m_sqlProvider.GetFiles(this.m_watermark.LastResourceId, this.m_batchSize);
        this.CheckTotalBytesCopied(this.m_watermark.TotalBytesWritten + files.Sum<FileStatistics>((Func<FileStatistics, long>) (x => x.CompressedLength)), this.m_tableContentSizeInBytes, this.m_watermark.Multiplier);
        this.UploadFiles((IEnumerable<FileStatistics>) files);
        num1 += files.Count;
        this.m_watermark.UpdateLastResourceId(files.LastOrDefault<FileStatistics>()?.ResourceId);
        this.m_watermark.Save(this.m_servicingContext);
        if (this.m_watermark.BytesSinceLastFileTransferProgressUpdate >= this.m_updateFileTransferProgressThresholdInBytes)
        {
          this.UpdateFileTransferProgress();
          this.m_watermark.ResetBytesSinceLastFileTransferProgressUpdate();
        }
        if (num1 % 10000 == 0 || this.m_watermark.NumberOfFilesProcessed % 1000L == 0L)
        {
          double num2 = ByteConverterUtility.ConvertUnits(this.m_watermark.BytesSinceLastLogWritten, ByteConverterUtilityUnit.B, ByteConverterUtilityUnit.MB) / (DateTime.UtcNow - utcNow).TotalSeconds;
          double num3 = ByteConverterUtility.ConvertUnits(this.m_watermark.TotalBytesWritten, ByteConverterUtilityUnit.B, ByteConverterUtilityUnit.MB);
          this.m_logger.Info(string.Format("File Transfer in progress, processed {0} files, Copied {1} files, a total of {2} MB, Elapsed time {3}, current copy speed {4}", (object) num1, (object) this.m_watermark.NumberOfFilesProcessed, (object) num3, (object) stopwatch.Elapsed, (object) num2));
          utcNow = DateTime.UtcNow;
          this.m_watermark.ResetBytesSinceLastLog();
        }
      }
      while (files.Count > 0);
      this.UpdateFileTransferProgress(true);
      stopwatch.Stop();
      if (this.m_isLongRunningTasksEnabled && task != null)
      {
        blockFileImportTrackingThreadFromExiting = false;
        this.m_fileImportTrackingManualResetEvent.Set();
        if (!task.Wait(10000))
          this.m_logger.Warning("Timed out waiting for {0} to finish", (object) "fileImportTrackingTask");
      }
      double num4 = ByteConverterUtility.ConvertUnits(this.m_watermark.TotalBytesWritten, ByteConverterUtilityUnit.B, ByteConverterUtilityUnit.MB);
      double num5 = num4 / (double) stopwatch.ElapsedMilliseconds * 1000.0;
      this.m_logger.Info(string.Format("File Transfer complete. processed {0} files, Copied {1}, a total of {2} MB, Elapsed time {3}, average copy speed {4} MB/s", (object) num1, (object) this.m_watermark.NumberOfFilesProcessed, (object) num4, (object) stopwatch.Elapsed, (object) num5));

      void FileImportTrackingTaskAction()
      {
        Dictionary<FileImportTrackingInfo, int> fileImportTrackingInfoWarnings = new Dictionary<FileImportTrackingInfo, int>();
        int smallesMsUntilNextWarning = this.m_msToWaitForUploadBeforeWarning;
        while (blockFileImportTrackingThreadFromExiting)
        {
          this.m_fileImportTrackingManualResetEvent.Wait(smallesMsUntilNextWarning);
          this.m_fileImportTrackingManualResetEvent.Reset();
          smallesMsUntilNextWarning = this.m_msToWaitForUploadBeforeWarning;
          this.m_fileImportTrackingInfo.ForEach<KeyValuePair<string, FileImportTrackingInfo>>(closure_0 ?? (closure_0 = (Action<KeyValuePair<string, FileImportTrackingInfo>>) (trackingInfo =>
          {
            int totalMilliseconds = (int) TimeSpan.FromTicks(DateTime.UtcNow.Ticks - trackingInfo.Value.StartTime).TotalMilliseconds;
            smallesMsUntilNextWarning = Math.Min(smallesMsUntilNextWarning, this.m_msToWaitForUploadBeforeWarning - totalMilliseconds % this.m_msToWaitForUploadBeforeWarning);
            if (totalMilliseconds <= this.m_msToWaitForUploadBeforeWarning)
              return;
            int num6 = 0;
            int num7;
            if (fileImportTrackingInfoWarnings.TryGetValue(trackingInfo.Value, out num7))
              num6 = num7;
            int addValue = totalMilliseconds / this.m_msToWaitForUploadBeforeWarning;
            if (num6 >= addValue)
              return;
            this.m_logger.Warning("File Transfer is taking longer than {0}ms to upload. Time Elapsed: {1}ms, FileResourceId: {2}, FileId: {3}, FileLength: {4}, FileHashValue: {5}", (object) this.m_msToWaitForUploadBeforeWarning, (object) totalMilliseconds, (object) trackingInfo.Value.FileResourceId, (object) trackingInfo.Value.FileId, (object) trackingInfo.Value.FileLength, (object) trackingInfo.Value.HashValue);
            fileImportTrackingInfoWarnings.AddOrUpdate<FileImportTrackingInfo, int>(trackingInfo.Value, addValue, (Func<int, int, int>) ((oldValue, newValue) => newValue));
          })));
          List<FileImportTrackingInfo> fileImportTrackingInfoWarningsToRemove = new List<FileImportTrackingInfo>();
          fileImportTrackingInfoWarnings.ForEach<KeyValuePair<FileImportTrackingInfo, int>>((Action<KeyValuePair<FileImportTrackingInfo, int>>) (warnedTrackingInfo =>
          {
            double totalMilliseconds = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - warnedTrackingInfo.Key.StartTime).TotalMilliseconds;
            if (this.m_fileImportTrackingInfo.ContainsKey(warnedTrackingInfo.Key.FileResourceId))
              return;
            fileImportTrackingInfoWarningsToRemove.Add(warnedTrackingInfo.Key);
            this.m_logger.Info("Long File Transfer successfully uploaded stream. Time Elapsed: {0}ms, FileResourceId: {1}, FileId: {2}, FileLength: {3}, FileHashValue: {4}", (object) totalMilliseconds, (object) warnedTrackingInfo.Key.FileResourceId, (object) warnedTrackingInfo.Key.FileId, (object) warnedTrackingInfo.Key.FileLength, (object) warnedTrackingInfo.Key.HashValue);
          }));
          fileImportTrackingInfoWarningsToRemove.ForEach(closure_1 ?? (closure_1 = (Action<FileImportTrackingInfo>) (toRemove => fileImportTrackingInfoWarnings.Remove(toRemove))));
        }
      }
    }

    private void UpdateFileTransferProgress(bool isComplete = false) => this.m_stepStatusScope.PublishInfoMessage(this.m_requestContext, string.Format("FileTransferProgress: {0}/{1}", (object) this.m_watermark.TotalBytesWritten, (object) (isComplete ? this.m_watermark.TotalBytesWritten : this.m_tableContentSizeInBytes)));

    private void UploadFiles(IEnumerable<FileStatistics> files)
    {
      if (this.m_isLongRunningTasksEnabled)
      {
        ConcurrentQueue<FileStatistics> queue = new ConcurrentQueue<FileStatistics>(files.Where<FileStatistics>((Func<FileStatistics, bool>) (fs => fs.ContentType == ContentType.Full)));
        Task[] tasks = new Task[this.m_maxBlobCopies];
        for (int index = 0; index < tasks.Length; ++index)
          tasks[index] = Task.Factory.StartNew((Action) (() =>
          {
            FileStatistics result;
            while (queue.TryDequeue(out result))
              this.UploadStreamTaskAction(result);
          }), this.m_cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        Task.WaitAll(tasks, this.m_cancellationToken);
      }
      else
      {
        ParallelOptions parallelOptions = new ParallelOptions()
        {
          MaxDegreeOfParallelism = this.m_maxBlobCopies,
          CancellationToken = this.m_cancellationToken
        };
        Parallel.ForEach<FileStatistics>(files.Where<FileStatistics>((Func<FileStatistics, bool>) (fs => fs.ContentType == ContentType.Full)), parallelOptions, new Action<FileStatistics>(this.UploadStreamTaskAction));
      }
    }

    private void UploadStreamTaskAction(FileStatistics fileStatistics)
    {
      new RetryManager(10, (Action<Exception>) (ex =>
      {
        if (!FileImportHelper.ShouldRetryImport(ex))
          throw new ApplicationException(string.Format("Failed to complete blob copy for ResourceId {0}. Exception: {1}, Message: {2}", (object) fileStatistics.ResourceId, (object) ex.GetType().FullName, (object) ex.Message), ex);
      })).Invoke((Action) (() =>
      {
        using (IFileImportContentProvider contentProvider = this.m_sqlProvider.CreateContentProvider(fileStatistics.ResourceId))
        {
          if (contentProvider.Content == null)
            return;
          this.UploadStream(fileStatistics, contentProvider.Content);
        }
      }));
      this.m_cancellationToken.ThrowIfCancellationRequested();
      this.m_watermark.IncrementCounters(fileStatistics.CompressedLength);
    }

    private static bool ShouldRetryImport(Exception ex)
    {
      switch (ex)
      {
        case StorageException _:
        case SqlException _:
        case RequestFailedException _:
          return true;
        case AggregateException aggregateException:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return aggregateException.InnerExceptions.All<Exception>(FileImportHelper.\u003C\u003EO.\u003C0\u003E__ShouldRetryImport ?? (FileImportHelper.\u003C\u003EO.\u003C0\u003E__ShouldRetryImport = new Func<Exception, bool>(FileImportHelper.ShouldRetryImport)));
        default:
          return false;
      }
    }

    private void UploadStream(FileStatistics fileStatistics, Stream fileContent)
    {
      string base64String = Convert.ToBase64String(fileStatistics.HashValue);
      Dictionary<string, string> blobMetadata = new Dictionary<string, string>()
      {
        [FileServiceMetadataConstants.CompressionType] = fileStatistics.CompressionType.ToString(),
        [FileServiceMetadataConstants.CompressedLength] = fileStatistics.CompressedLength.ToString(),
        [FileServiceMetadataConstants.FileId] = fileStatistics.FileId.ToString(),
        [FileServiceMetadataConstants.HashValue] = base64String,
        [FileServiceMetadataConstants.Length] = fileStatistics.FileLength.ToString()
      };
      using (BufferedStream content = new BufferedStream(fileContent, 32768))
      {
        bool flag = false;
        string str = fileStatistics.ResourceId.ToString("N");
        bool loggedWarning = false;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = cancellationTokenSource.Token;
        Stopwatch stopwatch = Stopwatch.StartNew();
        if (this.m_isLongRunningTasksEnabled)
        {
          FileImportTrackingInfo newTrackingInfo = new FileImportTrackingInfo(DateTime.UtcNow.Ticks, str, fileStatistics.FileId, fileStatistics.FileLength, base64String);
          this.m_fileImportTrackingInfo.AddOrUpdate(str, newTrackingInfo, (Func<string, FileImportTrackingInfo, FileImportTrackingInfo>) ((key, existingValue) => newTrackingInfo));
        }
        else
          Task.Run((Func<Task>) (() =>
          {
            while (true)
            {
              Task.Delay(this.m_msToWaitForUploadBeforeWarning, token).Wait();
              loggedWarning = true;
              this.m_logger.Warning("File Transfer is taking longer than {0}ms to upload. Time Elapsed: {1}ms, FileResourceId: {2}, FileId: {3}, FileLength: {4}, FileHashValue: {5}", (object) this.m_msToWaitForUploadBeforeWarning, (object) stopwatch.ElapsedMilliseconds, (object) fileStatistics.ResourceId.ToString("N"), (object) blobMetadata[FileServiceMetadataConstants.FileId], (object) blobMetadata[FileServiceMetadataConstants.Length], (object) blobMetadata[FileServiceMetadataConstants.HashValue]);
            }
          }), token);
        try
        {
          foreach (IDataImportFileExtension importFileExtension in (IEnumerable<IDataImportFileExtension>) this.m_dataImportFileExtensions)
          {
            flag = importFileExtension.FileProcessedByExtension((Stream) content, (IDictionary<string, string>) blobMetadata);
            if (flag)
              break;
          }
          if (!flag)
            this.m_blobProvider.PutStream(str, (Stream) content, (IDictionary<string, string>) blobMetadata, this.m_cancellationToken);
        }
        finally
        {
          if (this.m_isLongRunningTasksEnabled)
          {
            this.m_fileImportTrackingInfo.TryRemove(str, out FileImportTrackingInfo _);
            this.m_fileImportTrackingManualResetEvent.Set();
          }
          else
          {
            stopwatch.Stop();
            cancellationTokenSource.Cancel();
          }
        }
        if (this.m_isLongRunningTasksEnabled)
          return;
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        if (!loggedWarning && elapsedMilliseconds < (long) this.m_msToWaitForUploadBeforeWarning)
          return;
        this.m_logger.Info("Long File Transfer successfully uploaded stream. Time Elapsed: {0}ms, FileResourceId: {1}, FileId: {2}, FileLength: {3}, FileHashValue: {4}", (object) elapsedMilliseconds, (object) fileStatistics.ResourceId.ToString("N"), (object) blobMetadata[FileServiceMetadataConstants.FileId], (object) blobMetadata[FileServiceMetadataConstants.Length], (object) blobMetadata[FileServiceMetadataConstants.HashValue]);
      }
    }

    private IDisposableReadOnlyList<IDataImportFileExtension> GetDataImportFileExtensions(
      ISqlConnectionInfo sourceConnectionInfo)
    {
      List<IDataImportFileExtension> elements = new List<IDataImportFileExtension>();
      using (IDisposableReadOnlyList<IDataImportFileExtensionFactory> extensions = this.m_requestContext.GetExtensions<IDataImportFileExtensionFactory>())
      {
        foreach (IDataImportFileExtensionFactory extensionFactory in (IEnumerable<IDataImportFileExtensionFactory>) extensions)
          elements.Add(extensionFactory.Initialize(this.m_requestContext, this.m_servicingContext, sourceConnectionInfo, this.m_logger));
      }
      return (IDisposableReadOnlyList<IDataImportFileExtension>) new DisposableCollection<IDataImportFileExtension>((IReadOnlyList<IDataImportFileExtension>) elements);
    }

    internal virtual IFileImportSqlProvider CreateFileProvider(
      ISqlConnectionInfo sourceConnectionInfo,
      ISqlConnectionInfo targetConnectionInfo)
    {
      return (IFileImportSqlProvider) new FileImportSqlProvider(sourceConnectionInfo, targetConnectionInfo, this.m_logger);
    }

    internal virtual IFileImportBlobProvider CreateBlobProvider(IVssRequestContext m_requestContext) => (IFileImportBlobProvider) new FileImportBlobProvider(m_requestContext);

    private void CheckTotalBytesCopied(long totalMBCopied, long blobSizeInMBs, int multiplier)
    {
      long num = blobSizeInMBs * (long) multiplier;
      if (blobSizeInMBs >= 0L && totalMBCopied > num)
        throw new BlobsCopiedExceededThresholdException(HostingResources.DataImportBlobsCopiedExceededThreshold((object) totalMBCopied, (object) num));
    }

    private void CheckForParallelOptionChanges(IVssRequestContext deploymentContext)
    {
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(deploymentContext, (RegistryQuery) "/Configuration/DataImport/FileBatchSize", this.m_batchSize);
      if (num1 != this.m_batchSize)
      {
        this.m_batchSize = num1;
        this.m_logger.Info("File Batch Size changed on the registry, new value: {0}", (object) this.m_batchSize);
      }
      int num2 = service.GetValue<int>(deploymentContext, (RegistryQuery) "/Configuration/DataImport/MaxBlobCopies", this.m_maxBlobCopies);
      if (num2 == this.m_maxBlobCopies)
        return;
      this.m_maxBlobCopies = num2;
      this.m_logger.Info("Max blob copies changed on the registry, new value: {0}", (object) this.m_maxBlobCopies);
    }

    public void Dispose()
    {
      this.m_blobProvider.Dispose();
      this.m_dataImportFileExtensions.Dispose();
      this.m_fileImportTrackingManualResetEvent.Dispose();
    }
  }
}
