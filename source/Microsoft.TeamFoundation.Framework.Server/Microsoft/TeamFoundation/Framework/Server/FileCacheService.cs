// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class FileCacheService : IVssFrameworkService, IDisposable
  {
    private static readonly CommandPropertiesSetter DefaultCommandPropertiesForFileCache = new CommandPropertiesSetter().WithExecutionTimeout(TimeSpan.MaxValue).WithExecutionMaxConcurrentRequests(100);
    private static readonly byte[] s_nullHash = new byte[16];
    private int m_storageFailures;
    private CacheCleanup m_cacheCleanup;
    private string m_cleanupFileFullPath;
    private readonly object m_lock = new object();
    internal const string Downloads = "Downloads";
    internal const string CleanupFileName = "Cleanup.txt";
    internal const string s_md5HashHeader = "File-MD5";
    internal const string s_uncompressedContentLengthHeader = "Uncompressed-Content-Length";
    private const int c_maxChunkSize = 1048576;
    private const int c_errorDiskFull = 70;
    private const int c_maxConcurrentRequests = 100;
    private const string c_area = "FileCache";
    private const string c_layer = "Service";

    internal FileCacheService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12000, "FileCache", "Service", "ServiceStart");
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
        if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
        try
        {
          this.Configuration = new ProxyConfiguration(systemRequestContext);
          if (!this.Configuration.IsValid)
            return;
          this.Statistics = new ProxyStatistics();
          this.Statistics.Initialize(systemRequestContext, (IProxyConfigurationInternal) this.Configuration, new ScanDiskCompleted(this.OnScanDiskCompleted));
          this.DeleteCacheItems = (IDeleteCacheItems) new LastAccessTimeBasedDeletion(systemRequestContext, this.Configuration.DeletionAgeThreshold, this.Configuration.CacheRoot, (IProxyStatistics) this.Statistics);
          this.IsVCCacheEnabled = this.Configuration.VCCacheEnabledState == 2;
          this.IsGitCacheEnabled = this.Configuration.GitCacheEnabledState == 2;
        }
        catch (Exception ex)
        {
          systemRequestContext.TraceException(12008, "FileCache", "Service", ex);
          this.Dispose();
          throw;
        }
      }
      finally
      {
        systemRequestContext.TraceLeave(12009, "FileCache", "Service", "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12010, "FileCache", "Service", "ServiceEnd");
      try
      {
        this.IsShuttingDown = true;
        this.Dispose();
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(12018, "FileCache", "Service", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(12019, "FileCache", "Service", "ServiceEnd");
      }
    }

    public void Dispose()
    {
      if (this.DeleteCacheItems != null)
      {
        this.DeleteCacheItems.Dispose();
        this.DeleteCacheItems = (IDeleteCacheItems) null;
      }
      if (this.Statistics != null)
      {
        this.Statistics.Dispose();
        this.Statistics = (ProxyStatistics) null;
      }
      if (this.Configuration == null)
        return;
      this.Configuration.Dispose();
      this.Configuration = (ProxyConfiguration) null;
    }

    internal void ReloadConfiguration(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      this.Configuration = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? new ProxyConfiguration(systemRequestContext) : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_cacheCleanup = (CacheCleanup) null;
    }

    public bool IsVCCacheEnabled { get; private set; }

    public bool IsGitCacheEnabled { get; private set; }

    public void UpdateCacheSize(
      IVssRequestContext requestContext,
      string serverId,
      long cacheSize,
      int fileCount)
    {
      this.Statistics.UpdateCacheSize(serverId, FileCacheHelper.RoundToNearestClusterSize(cacheSize), fileCount);
      this.CacheCleanup.CheckCacheLimit(requestContext);
    }

    public bool TransmitFile<TFileInformation>(
      IDownloadState<TFileInformation> downloadState,
      TFileInformation fileInformation,
      string path,
      long offset,
      long length)
      where TFileInformation : FileInformationBase
    {
      TeamFoundationTracingService.TraceRaw(12020, TraceLevel.Verbose, "FileCache", "Service", "Transmitting:  file {0}:{1} {2} bytes", (object) fileInformation.RepositoryGuid, (object) fileInformation.FileIdentifier, (object) length);
      return downloadState.TransmitFile(fileInformation, path, offset, length);
    }

    public bool TransmitChunk<TFileInformation>(
      IDownloadState<TFileInformation> downloadState,
      TFileInformation fileInformation,
      byte[] chunk,
      long offset,
      long length)
      where TFileInformation : FileInformationBase
    {
      return downloadState.TransmitChunk(fileInformation, chunk, offset, length);
    }

    public string GetFilePath(FileInformationBase fileInfo) => Path.Combine(this.Configuration.CacheRoot, fileInfo.GetFilePath());

    public bool TryGetCacheCleanupMutex(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12030, "FileCache", "Service", nameof (TryGetCacheCleanupMutex));
      try
      {
        if (this.CleanupFile != null)
          return false;
        SafeFileHandle file = ProxyNativeMethods.CreateFile("\\\\?\\" + this.CleanupFilePath, 3U, 0U, IntPtr.Zero, 4U, 128U, IntPtr.Zero);
        if (file.IsInvalid)
        {
          int lastWin32Error = Marshal.GetLastWin32Error();
          if (lastWin32Error != 32)
          {
            requestContext.Trace(12034, TraceLevel.Error, "FileCache", "Service", "Fail to open cleanup file!");
            throw new Win32Exception(lastWin32Error);
          }
          requestContext.Trace(12036, TraceLevel.Verbose, "FileCache", "Service", "Cleanup file is locked");
          return false;
        }
        lock (this.m_lock)
          this.CleanupFile = new FileStream(file, System.IO.FileAccess.ReadWrite);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12038, "FileCache", "Service", ex);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(12039, "FileCache", "Service", nameof (TryGetCacheCleanupMutex));
      }
    }

    public bool ReleaseCacheCleanupMutex(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12040, "FileCache", "Service", nameof (ReleaseCacheCleanupMutex));
      try
      {
        if (this.CleanupFile != null)
        {
          lock (this.m_lock)
          {
            if (this.CleanupFile != null)
            {
              this.CleanupFile.Close();
              this.CleanupFile = (FileStream) null;
            }
          }
          this.DeleteFile(requestContext, this.CleanupFilePath);
        }
        return false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12048, "FileCache", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12049, "FileCache", "Service", nameof (ReleaseCacheCleanupMutex));
      }
    }

    public bool RetrieveFileFromCache<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      IDownloadState<TFileInformation> downloadState,
      bool compressOutput)
      where TFileInformation : FileInformationBase
    {
      bool fileTransmitted = false;
      try
      {
        requestContext.TraceEnter(12060, "FileCache", "Service", nameof (RetrieveFileFromCache));
        Random r = new Random();
        string fullPath = (string) null;
        Action run = (Action) (() =>
        {
          try
          {
            fullPath = this.GetFilePath((FileInformationBase) fileInfo);
            using (SafeFileHandle file = Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateFile("\\\\?\\" + fullPath, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericRead, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.Read, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreationDisposition.OpenExisting, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAttributes.Normal, IntPtr.Zero))
            {
              if (!file.IsInvalid)
              {
                requestContext.Trace(12062, TraceLevel.Verbose, "FileCache", "Service", "File {0} Exists - attempting to open", (object) fullPath);
                using (FileStream fileStream = new FileStream(file, System.IO.FileAccess.Read))
                {
                  byte[] numArray = new byte[256];
                  fileStream.Read(numArray, 0, numArray.Length);
                  FileInformationBase.FileHeader fileHeader = new FileInformationBase.FileHeader(numArray);
                  fileStream.Position = 256L;
                  if (fileHeader.VersionStamp > (short) 10 && this.Configuration.Crc >= r.Next(0, 101) && (int) Crc32.ComputeHash(requestContext, (Stream) fileStream) != (int) fileHeader.CRC32)
                  {
                    requestContext.Trace(12066, TraceLevel.Error, "FileCache", "Service", "CRC does not match for file {0}", (object) fullPath);
                    throw new CorruptHeadersException("CRC mismatch");
                  }
                  if (!FileCacheService.CompareHash(fileHeader.HashValue, ((TFileInformation) fileInfo).HashValue))
                  {
                    requestContext.Trace(12064, TraceLevel.Error, "FileCache", "Service", "Hash does not match for file {0}", (object) fullPath);
                    throw new DownloadTicketValidationException(FrameworkResources.RequestSignatureValidationFailed());
                  }
                  if (((TFileInformation) fileInfo).HashValue == null)
                    ((TFileInformation) fileInfo).HashValue = fileHeader.HashValue;
                  ((TFileInformation) fileInfo).ContentType = fileHeader.ContentType;
                  ((TFileInformation) fileInfo).Length = fileStream.Length - (long) fileHeader.ContentOffset;
                  if (((TFileInformation) fileInfo).Length == 0L)
                  {
                    ((TFileInformation) fileInfo).ContentType = TeamFoundationFileService.ToMimeType(CompressionType.None);
                    compressOutput = false;
                  }
                  ((TFileInformation) fileInfo).UncompressedLength = fileHeader.UncompressedLength;
                  CompressionType compressionType = TeamFoundationFileService.FromMimeType(((TFileInformation) fileInfo).ContentType);
                  if (!compressOutput)
                  {
                    if (compressionType != CompressionType.None)
                      goto label_17;
                  }
                  requestContext.Trace(12068, TraceLevel.Verbose, "FileCache", "Service", "Calling TransmitFile for file {0}", (object) fullPath);
                  requestContext.TraceEnter(12070, "FileCache", "Service", "TransmitFile");
                  requestContext.RequestTimer.PauseTimeToFirstPageTimer();
                  downloadState.TransmitFile(fileInfo, fullPath, (long) fileHeader.ContentOffset, ((TFileInformation) fileInfo).Length);
                  fileTransmitted = true;
                  requestContext.RequestTimer.ResumeTimeToFirstPageTimer();
                  requestContext.TraceLeave(12072, "FileCache", "Service", "TransmitFile");
                }
              }
label_17:
              if (!fileTransmitted)
                return;
              this.Statistics.IncrementDownloads(((TFileInformation) fileInfo).RepositoryGuid, true);
              if (requestContext.ExecutionEnvironment.IsHostedDeployment)
                return;
              this.UpdateLastAccessTime(requestContext, fullPath);
            }
          }
          catch (InvalidDataException ex)
          {
            requestContext.TraceException(12074, "FileCache", "Service", (Exception) ex);
            TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.UnknownProxyException(), (Exception) ex);
            this.DeleteFile(requestContext, fullPath);
          }
          catch (CorruptHeadersException ex)
          {
            requestContext.TraceException(12076, "FileCache", "Service", (Exception) ex);
            TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.MetadataFormatWrong((object) fullPath), (Exception) ex);
            this.DeleteFile(requestContext, fullPath);
          }
          catch (IOException ex)
          {
            requestContext.TraceException(12082, "FileCache", "Service", (Exception) ex);
          }
          catch (UnauthorizedAccessException ex)
          {
            requestContext.TraceException(12084, "FileCache", "Service", (Exception) ex);
          }
          catch (Exception ex)
          {
            TeamFoundationEventLog.Default.LogException(requestContext, FrameworkResources.UnknownProxyException(), ex);
            requestContext.TraceException(12085, "FileCache", "Service", ex);
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "CloudOrchestrationPlatform.").AndCommandKey((CommandKey) "FileCache.CheckFileCache").AndCommandPropertiesDefaults(FileCacheService.DefaultCommandPropertiesForFileCache);
        new CommandService(requestContext, setter, run, (Action) (() => requestContext.Trace(12086, TraceLevel.Warning, "FileCache", "Service", "The File Cache has been bypassed due to circuit breaker"))).Execute();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12087, "FileCache", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12088, "FileCache", "Service", nameof (RetrieveFileFromCache));
      }
      return fileTransmitted;
    }

    private void CopyStreamToFile<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      out long totalBytes,
      out FileStream fileStream,
      out bool cacheUsed,
      out string tempPath)
      where TFileInformation : FileInformationBase
    {
      totalBytes = 0L;
      fileStream = (FileStream) null;
      cacheUsed = false;
      tempPath = Path.Combine(this.Configuration.CacheRoot, fileInfo.GetTempFilePath());
      requestContext.Trace(12094, TraceLevel.Verbose, "FileCache", "Service", "Creating Temp file {1} to copy stream into for file {0}", (object) fileInfo.FileIdentifier, (object) tempPath);
      try
      {
        int num = 0;
        while (num < 3)
        {
          SafeFileHandle file = Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateFile("\\\\?\\" + tempPath, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericRead | Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericWrite, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.None, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreationDisposition.New, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAttributes.Normal, IntPtr.Zero);
          if (!file.IsInvalid)
          {
            fileStream = new FileStream(file, System.IO.FileAccess.ReadWrite);
            break;
          }
          int lastWin32Error = Marshal.GetLastWin32Error();
          file.Dispose();
          if (lastWin32Error == 3)
          {
            ++num;
            string path = Path.Combine(Path.Combine(this.Configuration.CacheRoot, fileInfo.RepositoryId.ToString()), "Downloads");
            this.CreateDirectory(requestContext, path, false);
          }
          else
          {
            if (lastWin32Error == 70)
            {
              requestContext.Trace(12102, TraceLevel.Info, "FileCache", "Service", "Triggering Cache Cleanup because Disk is full");
              this.CacheCleanup.CleanCache(requestContext, -1L);
            }
            throw new IOException("CreateFile", lastWin32Error);
          }
        }
        if (fileStream == null)
          return;
        requestContext.Trace(12096, TraceLevel.Verbose, "FileCache", "Service", "Writing Header for file {0}", (object) fileInfo.FileIdentifier);
        byte[] bytes = FileInformationBase.FileHeader.GetBytes(fileInfo.HashValue ?? FileCacheService.s_nullHash, fileInfo.ContentType, 0U, fileInfo.UncompressedLength);
        totalBytes = fileInfo.Length + (long) bytes.Length;
        fileStream.SetLength(totalBytes);
        fileStream.Write(bytes, 0, bytes.Length);
        requestContext.Trace(12098, TraceLevel.Verbose, "FileCache", "Service", "Header written for file {0}", (object) tempPath);
        cacheUsed = true;
      }
      catch (IOException ex)
      {
        if (fileStream != null)
        {
          fileStream.Dispose();
          fileStream = (FileStream) null;
        }
        requestContext.TraceException(12100, "FileCache", "Service", (Exception) ex);
        TeamFoundationEventLog.Default.Log(FrameworkResources.ErrorCommitingToCache(), TeamFoundationEventId.ProxyException, EventLogEntryType.Error);
        Interlocked.Increment(ref this.m_storageFailures);
      }
    }

    private void StoreInCache<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      long totalBytes,
      string tempPath)
      where TFileInformation : FileInformationBase
    {
      string filePath = this.GetFilePath((FileInformationBase) fileInfo);
      try
      {
        if (this.ValidateOrComputeHash(requestContext, tempPath, totalBytes))
        {
          requestContext.Trace(12110, TraceLevel.Verbose, "FileCache", "Service", "Temp File {0} validated - moving to permanent location {1}", (object) tempPath, (object) filePath);
          int num = 0;
          while (num < 3)
          {
            if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.MoveFile("\\\\?\\" + tempPath, "\\\\?\\" + filePath))
            {
              this.Statistics.UpdateCacheSize(fileInfo.RepositoryGuid, FileCacheHelper.RoundToNearestClusterSize(fileInfo.Length + 256L), 1);
              this.CacheCleanup.CheckCacheLimit(requestContext);
              break;
            }
            int lastWin32Error = Marshal.GetLastWin32Error();
            switch (lastWin32Error)
            {
              case 3:
                ++num;
                string directoryName = Path.GetDirectoryName(filePath);
                this.CreateDirectory(requestContext, directoryName, false);
                continue;
              case 32:
              case 183:
                this.DeleteFile(requestContext, tempPath);
                return;
              default:
                requestContext.Trace(12114, TraceLevel.Error, "FileCache", "Service", "Error {0} while moving file to permanent repository", (object) lastWin32Error);
                goto case 32;
            }
          }
        }
        else
        {
          requestContext.Trace(12112, TraceLevel.Error, "FileCache", "Service", "Temp File {0} is invalid - deleting", (object) tempPath);
          this.DeleteFile(requestContext, tempPath);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12116, "FileCache", "Service", ex);
        switch (ex)
        {
          case CorruptHeadersException _:
            TeamFoundationEventLog.Default.Log(FrameworkResources.MetadataFormatWrong((object) string.Empty), TeamFoundationEventId.ProxyException, EventLogEntryType.Error);
            break;
          case DownloadTicketValidationException _:
            TeamFoundationEventLog.Default.Log(FrameworkResources.ErrorCommitingToCache(), TeamFoundationEventId.ProxyException, EventLogEntryType.Error);
            break;
          default:
            TeamFoundationEventLog.Default.Log(FrameworkResources.ErrorDownloadingFromAppTier(), TeamFoundationEventId.ProxyException, EventLogEntryType.Error);
            break;
        }
      }
    }

    public void RetrieveFileFromDatabase<TFileInformation>(
      IVssRequestContext requestContext,
      TFileInformation fileInfo,
      IDownloadState<TFileInformation> downloadState,
      bool compressOutput,
      Stream databaseStream,
      bool useDisk = false)
      where TFileInformation : FileInformationBase
    {
      try
      {
        requestContext.TraceEnter(12089, "FileCache", "Service", nameof (RetrieveFileFromDatabase));
        FileStream fileStream = (FileStream) null;
        string tempPath = (string) null;
        Stream stream = (Stream) null;
        long totalBytes = 0;
        CommandService commandService = (CommandService) null;
        try
        {
          requestContext.Trace(12090, TraceLevel.Info, "FileCache", "Service", "Cache Miss for file {0}", (object) fileInfo.FileIdentifier);
          stream = databaseStream != null ? databaseStream : downloadState.CacheMiss(this, fileInfo, compressOutput);
          requestContext.Trace(12092, TraceLevel.Verbose, "FileCache", "Service", "Received File {0} from server", (object) fileInfo.FileIdentifier);
          using (ByteArray byteArray = new ByteArray((int) Math.Min(fileInfo.Length, 1048576L)))
          {
            byte[] bytes = byteArray.Bytes;
            ArgumentUtility.CheckForNull<string>(fileInfo.ContentType, "fileInfo.ContentType");
            bool cacheUsed = false;
            if (useDisk)
            {
              CommandSetter setter1 = CommandSetter.WithGroupKey((CommandGroupKey) "CloudOrchestrationPlatform.").AndCommandKey((CommandKey) "FileCache.StoreTempFileOnDisk").AndCommandPropertiesDefaults(FileCacheService.DefaultCommandPropertiesForFileCache);
              new CommandService(requestContext, setter1, (Action) (() => this.CopyStreamToFile<TFileInformation>(requestContext, fileInfo, out totalBytes, out fileStream, out cacheUsed, out tempPath)), (Action) (() => { })).Execute();
              if (!string.IsNullOrEmpty(tempPath))
              {
                CommandSetter setter2 = CommandSetter.WithGroupKey((CommandGroupKey) "CloudOrchestrationPlatform.").AndCommandKey((CommandKey) "FileCache.MoveFileToCache").AndCommandPropertiesDefaults(FileCacheService.DefaultCommandPropertiesForFileCache);
                commandService = new CommandService(requestContext, setter2, (Action) (() => this.StoreInCache<TFileInformation>(requestContext, fileInfo, totalBytes, tempPath)), (Action) (() => { }));
              }
            }
            bool flag = true;
            using (requestContext.CreateTimeToFirstPageExclusionBlock())
            {
              int num;
              do
              {
                num = stream.Read(bytes, 0, bytes.Length);
                if (fileStream != null)
                {
                  try
                  {
                    requestContext.Trace(12104, TraceLevel.Verbose, "FileCache", "Service", "Writing {0} to file {1}", (object) num, (object) tempPath);
                    fileStream.Write(bytes, 0, num);
                  }
                  catch (IOException ex)
                  {
                    requestContext.TraceException(12106, "FileCache", "Service", (Exception) ex);
                    Interlocked.Increment(ref this.m_storageFailures);
                    fileStream.Dispose();
                    fileStream = (FileStream) null;
                  }
                }
                if (flag)
                {
                  requestContext.Trace(12108, TraceLevel.Verbose, "FileCache", "Service", "Transmitting {0} bytes to client for file {1}", (object) num, (object) fileInfo.FileIdentifier);
                  flag = downloadState.TransmitChunk(fileInfo, bytes, 0L, (long) num);
                }
              }
              while (num > 0);
            }
            if (cacheUsed)
              this.Statistics.IncrementDownloads(fileInfo.RepositoryGuid, false);
          }
        }
        finally
        {
          if (fileStream != null)
          {
            fileStream.Close();
            fileStream.Dispose();
            fileStream = (FileStream) null;
          }
          if (databaseStream == null && stream != null)
            stream.Dispose();
        }
        commandService?.Execute();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12118, "FileCache", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12120, "FileCache", "Service", nameof (RetrieveFileFromDatabase));
      }
    }

    private void UpdateLastAccessTime(IVssRequestContext requestContext, string fullPath)
    {
      requestContext.TraceEnter(12130, "FileCache", "Service", nameof (UpdateLastAccessTime));
      try
      {
        DateTime now = DateTime.Now;
        if (!(File.GetLastAccessTime(fullPath) < now.AddDays(-1.0)))
          return;
        File.SetLastAccessTime(fullPath, now);
      }
      catch (IOException ex) when (FileCacheHelper.IsSharingViolation(ex))
      {
      }
      catch (IOException ex)
      {
        requestContext.TraceException(12137, "FileCache", "Service", (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12138, "FileCache", "Service", ex);
      }
      finally
      {
        requestContext.TraceLeave(12139, "FileCache", "Service", nameof (UpdateLastAccessTime));
      }
    }

    internal ProxyConfiguration Configuration { get; private set; }

    internal ProxyStatistics Statistics { get; private set; }

    internal bool IsShuttingDown { get; private set; }

    internal bool DefaultCacheLimitWarningLogged { get; private set; }

    internal CacheCleanup CacheCleanup
    {
      get
      {
        if (this.m_cacheCleanup == null)
          this.m_cacheCleanup = this.Configuration.CacheLimit <= 0 ? (CacheCleanup) new StoragePercentBasedCacheCleanup(this) : (CacheCleanup) new StorageSizeBasedCacheCleanup(this);
        return this.m_cacheCleanup;
      }
    }

    internal IDeleteCacheItems DeleteCacheItems { get; set; }

    private FileStream CleanupFile { get; set; }

    internal bool CacheCleanupInProgress => this.CleanupFile != null;

    private bool ValidateOrComputeHash(
      IVssRequestContext requestContext,
      string fullPath,
      long fileLength)
    {
      requestContext.TraceEnter(12140, "FileCache", "Service", nameof (ValidateOrComputeHash));
      uint crc32_1 = 0;
      byte[] numArray1 = (byte[]) null;
      try
      {
        using (MD5 md5Provider = MD5Util.TryCreateMD5Provider())
        {
          if (md5Provider == null)
          {
            requestContext.Trace(12142, TraceLevel.Warning, "FileCache", "Service", "Hash was not validated because FIPS is on");
            return true;
          }
          using (SafeFileHandle file = Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreateFile("\\\\?\\" + fullPath, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericRead | Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAccess.GenericWrite, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileShare.None, IntPtr.Zero, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CreationDisposition.OpenExisting, Microsoft.TeamFoundation.Common.Internal.NativeMethods.FileAttributes.SequentialScan, IntPtr.Zero))
          {
            if (!file.IsInvalid)
            {
              using (FileStream fileStream = new FileStream(file, System.IO.FileAccess.ReadWrite, 8))
              {
                byte[] numArray2 = new byte[256];
                int num1 = fileStream.Read(numArray2, 0, numArray2.Length);
                FileInformationBase.FileHeader fileHeader = new FileInformationBase.FileHeader(numArray2);
                Stream stream = string.Compare(fileHeader.ContentType, "application/gzip", StringComparison.OrdinalIgnoreCase) != 0 ? (Stream) fileStream : (Stream) new GZipStream((Stream) fileStream, CompressionMode.Decompress);
                using (ByteArray byteArray = new ByteArray((int) Math.Min(fileLength, 1048576L)))
                {
                  byte[] bytes = byteArray.Bytes;
                  num1 = 0;
                  Crc32 crc32_2 = new Crc32();
                  crc32_2.UpdateCRC32(bytes, 0, 0);
                  int num2;
                  do
                  {
                    num2 = stream.Read(bytes, 0, bytes.Length);
                    if (num2 != 0)
                    {
                      md5Provider.TransformBlock(bytes, 0, num2, (byte[]) null, 0);
                      if (stream == fileStream)
                        crc32_2.UpdateCRC32(bytes, 0, num2);
                    }
                    else
                      md5Provider.TransformFinalBlock(bytes, 0, 0);
                  }
                  while (num2 > 0);
                  numArray1 = md5Provider.Hash;
                  crc32_1 = crc32_2.Crc32Value;
                }
                if (stream != fileStream)
                {
                  fileStream.Position = 256L;
                  crc32_1 = Crc32.ComputeHash(requestContext, (Stream) fileStream);
                }
                if (FileCacheService.CompareHash(fileHeader.HashValue, FileCacheService.s_nullHash))
                {
                  requestContext.Trace(12144, TraceLevel.Warning, "FileCache", "Service", "Found a zero-byte hash in the header for file {0}", (object) fullPath);
                  return this.UpdateHeader(requestContext, (Stream) fileStream, numArray1, fileHeader.ContentType, crc32_1, fileHeader.UncompressedLength);
                }
                if (FileCacheService.CompareHash(fileHeader.HashValue, numArray1))
                {
                  requestContext.Trace(12146, TraceLevel.Verbose, "FileCache", "Service", "Hash value is correct for file {0}", (object) fullPath);
                  return this.UpdateHeader(requestContext, (Stream) fileStream, numArray1, fileHeader.ContentType, crc32_1, fileHeader.UncompressedLength);
                }
                requestContext.Trace(12148, TraceLevel.Error, "FileCache", "Service", "Hash value DOES NOT MATCH for file {0}", (object) fullPath);
                throw new DownloadTicketValidationException();
              }
            }
            else
            {
              requestContext.Trace(12149, TraceLevel.Error, "FileCache", "Service", "Cannot open file {0} to validate MD5 hash", (object) fullPath);
              return false;
            }
          }
        }
      }
      catch (CorruptHeadersException ex)
      {
        requestContext.TraceException(12150, "FileCache", "Service", (Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12152, "FileCache", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12154, "FileCache", "Service", nameof (ValidateOrComputeHash));
      }
    }

    internal long ValidateCacheLimit(long cacheLimit)
    {
      if (cacheLimit > 0L)
        return cacheLimit;
      if (!this.DefaultCacheLimitWarningLogged)
      {
        TeamFoundationEventLog.Default.Log(FrameworkResources.DefaultCacheLimit((object) cacheLimit), TeamFoundationEventId.DefaultEventId, EventLogEntryType.Warning);
        this.DefaultCacheLimitWarningLogged = true;
      }
      return 1073741824;
    }

    private bool UpdateHeader(
      IVssRequestContext requestContext,
      Stream fileStream,
      byte[] hash,
      string contentType,
      uint crc32,
      long uncompressedLength)
    {
      requestContext.TraceEnter(12160, "FileCache", "Service", nameof (UpdateHeader));
      try
      {
        byte[] bytes = FileInformationBase.FileHeader.GetBytes(hash, contentType, crc32, uncompressedLength);
        fileStream.Seek(0L, SeekOrigin.Begin);
        fileStream.Write(bytes, 0, 256);
        return true;
      }
      catch (IOException ex)
      {
        requestContext.TraceException(12168, "FileCache", "Service", (Exception) ex);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(12169, "FileCache", "Service", nameof (UpdateHeader));
      }
    }

    private bool DeleteFile(IVssRequestContext requestContext, string path)
    {
      requestContext.TraceEnter(12180, "FileCache", "Service", nameof (DeleteFile));
      try
      {
        File.Delete(path);
        return true;
      }
      catch (PathTooLongException ex)
      {
        requestContext.TraceException(12182, "FileCache", "Service", (Exception) ex);
        return true;
      }
      catch (DirectoryNotFoundException ex)
      {
        requestContext.TraceException(12182, "FileCache", "Service", (Exception) ex);
        return true;
      }
      catch (IOException ex)
      {
        requestContext.TraceException(12184, "FileCache", "Service", (Exception) ex);
        return false;
      }
      catch (UnauthorizedAccessException ex)
      {
        requestContext.TraceException(12186, "FileCache", "Service", (Exception) ex);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(12188, "FileCache", "Service", nameof (DeleteFile));
      }
    }

    private void CreateDirectory(IVssRequestContext requestContext, string path, bool clean)
    {
      requestContext.TraceEnter(12190, "FileCache", "Service", nameof (CreateDirectory));
      try
      {
        bool flag = Directory.Exists(path);
        if (!(!flag | clean))
          return;
        if (flag & clean)
        {
          try
          {
            Directory.Delete(path, true);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(12192, "FileCache", "Service", ex);
          }
        }
        if (Directory.Exists(path))
          return;
        try
        {
          Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
          if (Directory.Exists(path))
            return;
          throw;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12198, "FileCache", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12199, "FileCache", "Service", nameof (CreateDirectory));
      }
    }

    private static bool CompareHash(byte[] b1, byte[] b2)
    {
      if (b1 == null || b2 == null)
        return true;
      if (b1.Length != b2.Length)
        return false;
      for (int index = 0; index < b1.Length; ++index)
      {
        if ((int) b1[index] != (int) b2[index])
          return false;
      }
      return true;
    }

    private void OnScanDiskCompleted(IVssRequestContext requestContext) => this.CacheCleanup.CheckCacheLimit(requestContext);

    private string CleanupFilePath
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_cleanupFileFullPath))
        {
          lock (this.m_lock)
          {
            if (string.IsNullOrEmpty(this.m_cleanupFileFullPath))
              this.m_cleanupFileFullPath = Path.Combine(this.Configuration.CacheRoot, "Cleanup.txt");
          }
        }
        return this.m_cleanupFileFullPath;
      }
    }
  }
}
