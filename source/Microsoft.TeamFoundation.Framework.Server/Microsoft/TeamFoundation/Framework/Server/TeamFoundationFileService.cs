// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationFileService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationFileService : 
    VssBaseService,
    ITeamFoundationFileService,
    IVssFrameworkService,
    IInternalTeamFoundationFileService
  {
    private static readonly string[] s_dataspaceCategoryLookup = new string[24]
    {
      "Default",
      "VersionControl",
      "WorkItem",
      "Build",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default",
      "Default"
    };
    private static readonly byte[] s_nullHash = new byte[16];
    private static readonly byte[] s_emptyHash = new byte[16]
    {
      (byte) 212,
      (byte) 29,
      (byte) 140,
      (byte) 217,
      (byte) 143,
      (byte) 0,
      (byte) 178,
      (byte) 4,
      (byte) 233,
      (byte) 128,
      (byte) 9,
      (byte) 152,
      (byte) 236,
      (byte) 248,
      (byte) 66,
      (byte) 126
    };
    private DateTime m_deltaDisableTime;
    private int m_maxPendingDeltas;
    private int m_maxRetryCount;
    private DateTime m_lastDeltaExceptionTime;
    private static readonly int s_bufferSize = 32768;
    internal static readonly int s_maxContentSize = 1048576;
    private IBlobProvider[] m_blobProviders;
    private int m_storageAccountId;
    private long m_minRemoteBlobSize;
    private long m_maxPatchableSize;
    private long m_tempFileSizeThreshold;
    private int m_externalRetentionPeriod;
    private int m_sqlRetentionPeriod;
    private int m_deletionChunkSize;
    private bool m_alwaysValidateUploads;
    private long m_fileId = 1024;
    private TeamFoundationTask m_updateRegistryTask;
    private int m_delaySecondFileRegistryUpdate;
    internal const string CleanupByPartsContainersFF = "VisualStudio.FrameworkService.FileService.CleanupByPartsContainers";
    internal const int c_deleteBatchDefault = 500;
    internal const int c_selectBatchDefault = 4000;
    internal const int c_selectFilesBatchDefault = 1000;
    internal TimeSpan c_logInterval = TimeSpan.FromSeconds(60.0);
    internal const int c_blobRetriesCount = 3;
    internal const string FileIdSecondaryRangeFF = "VisualStudio.FrameworkService.FileService.FileIdSecondaryRange";
    internal const string ReuseSecondaryRangeFileIdFF = "VisualStudio.FrameworkService.FileService.ReuseSecondaryRangeFileId";
    internal const string FileCleanupHashJoin = "VisualStudio.FrameworkService.FileService.SegmentBatchFileContainerCleanupWithHash";
    internal const string CleanupFilesSelectBatchSize = "Service/TeamFoundationFileService/FilesSelectBatchSize";
    internal const string CleanupSelectBatchSize = "Service/TeamFoundationFileService/SelectBatchSize";
    internal const string CleanupDeleteBatchSize = "Service/TeamFoundationFileService/CleanupBatchSize";
    internal const string FileCleanupRunStatistics = "Service/TeamFoundationFileService/FileCleanupStats";
    internal const string c_NoCleanupDays = "VisualStudio.FrameworkService.FileService.NoCleanupDays";
    internal const string c_NoCleanupDaysRegistryPath = "/Service/FileService/NoCleanupDays";
    internal const string c_LastCleanup = "/Service/FileService/LastCleanup";
    private bool m_reuseSecondaryRangeFileId;
    private Stopwatch m_reuseSecondaryRangeFileIdStopwatch = Stopwatch.StartNew();
    private HashSet<OwnerId> m_useSecondaryFileIdRangeSet;
    private const string c_fileIdLock = "Global\\MS.TF.WriteFileIdToRegistry";
    internal const string c_BlobStorageConnectionStringOverride = "BlobStorageConnectionStringOverride";
    internal const string c_blobStorageUriOverrideKey = "BlobStorageUriOverride";
    internal const string c_blobStorageCredentialsOverrideKey = "BlobStorageCredentialsOverride";
    internal const string c_BlobStorageDrawerNameSetting = "DrawerName";
    internal const string c_BlobStorageLookupKeySetting = "LookupKey";
    internal const string c_FileServiceStorageAccountPrefix = "FileServiceStorageAccount";
    private const string s_Area = "FileService";
    private const string s_Layer = "Service";
    private const int c_FileIdStartsAt = 1024;
    internal const string c_checkFileStartFileId = "/Service/FileService/StartFileIdCorruptionCheck";
    internal const string c_FileIdRegistry = "FileId";
    private TeamFoundationFileService m_deploymentService;
    internal const int HostedCleanupChunkSize = 2000;
    private const int c_remoteBlobPutChunkRetries = 3;
    private readonly TimeSpan m_remoteBlobPutChunkDelay = TimeSpan.FromSeconds(15.0);
    private const int c_remoteBlobGetStreamRetries = 3;
    private readonly TimeSpan m_remoteBlobGetStreamDelay = TimeSpan.FromSeconds(15.0);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        this.m_deploymentService = this;
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
        {
          "FileServiceStorageAccount*"
        });
        service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRemoteBlobProviderChanged), false, FrameworkServerConstants.FileServiceRemoteBlobProvider);
        systemRequestContext.GetService<ICompositeBlobProviderService>().RegisterNotification(systemRequestContext, new SecondaryBlobProvidersChanged(this.OnSecondaryBlobProvidersChanged));
        this.InitializeBlobProviders(systemRequestContext);
      }
      else
        this.m_deploymentService = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationFileService>();
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, FrameworkServerConstants.FileServiceRegistryRootPath + "/...", "FeatureAvailability/Entries/VisualStudio.FrameworkService.FileService.FileIdSecondaryRange/AvailabilityState", "FeatureAvailability/Entries/VisualStudio.FrameworkService.FileService.ReuseSecondaryRangeFileId/AvailabilityState");
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        this.m_fileId = 1024L;
        TeamFoundationFileService.InterlockedMax(ref this.m_fileId, TeamFoundationFileService.GetFileIdFromRegistry(systemRequestContext));
        this.UpdateFileIdOnSQL(systemRequestContext);
      }
      this.LoadSettings(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      service.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      if (systemRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (this.m_updateRegistryTask != null)
          systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().RemoveTask(systemRequestContext, this.m_updateRegistryTask);
        this.WriteFileIdToRegistry(systemRequestContext, Math.Max(5, this.m_delaySecondFileRegistryUpdate));
      }
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      service.UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRemoteBlobProviderChanged));
      systemRequestContext.GetService<ICompositeBlobProviderService>().UnregisterNotification(systemRequestContext, new SecondaryBlobProvidersChanged(this.OnSecondaryBlobProvidersChanged));
      if (this.m_blobProviders == null)
        return;
      foreach (IBlobProvider blobProvider in this.m_blobProviders)
        blobProvider.ServiceEnd(systemRequestContext);
    }

    public FileStatistics GetFileStatistics(
      IVssRequestContext requestContext,
      FileIdentifier fileId)
    {
      using (FileComponent component = requestContext.CreateComponent<FileComponent>())
      {
        TeamFoundationFile teamFoundationFile = component.RetrieveStatistics(fileId);
        if (teamFoundationFile == null)
          throw new FileIdNotFoundException(fileId.FileId);
        return new FileStatistics()
        {
          FileId = teamFoundationFile.Reference.FileId,
          DataspaceIdentifier = teamFoundationFile.Reference.DataspaceIdentifier,
          OwnerId = teamFoundationFile.Reference.OwnerId,
          CreationDate = teamFoundationFile.Reference.CreationDate,
          FileName = teamFoundationFile.Reference.FileName,
          ResourceId = teamFoundationFile.Metadata.ResourceId,
          ContentId = teamFoundationFile.Metadata.ContentId,
          ContentType = teamFoundationFile.Metadata.ContentType,
          CompressionType = teamFoundationFile.Metadata.CompressionType,
          HashValue = teamFoundationFile.Metadata.HashValue,
          FileLength = teamFoundationFile.Metadata.FileLength,
          CompressedLength = teamFoundationFile.Metadata.CompressedLength
        };
      }
    }

    public FileStatistics GetFileStatistics(IVssRequestContext requestContext, long fileId) => this.GetFileStatistics(requestContext, new FileIdentifier(fileId));

    public IDictionary<int, long> QueryFileSizes(
      IVssRequestContext requestContext,
      IEnumerable<long> fileIds,
      Guid dataspaceId,
      OwnerId ownerId)
    {
      return (IDictionary<int, long>) this.QueryFileSizes64(requestContext, fileIds, dataspaceId, ownerId).Select<KeyValuePair<long, long>, (int, long)>((Func<KeyValuePair<long, long>, (int, long)>) (t => ((int) t.Key, t.Value))).ToDictionary<(int, long), int, long>((Func<(int, long), int>) (t => t.Item1), (Func<(int, long), long>) (t => t.Value));
    }

    public IDictionary<long, long> QueryFileSizes64(
      IVssRequestContext requestContext,
      IEnumerable<long> fileIds,
      Guid dataspaceId,
      OwnerId ownerId)
    {
      requestContext.TraceEnter(14450, "FileService", "Service", "QueryFileSizes");
      ArgumentUtility.CheckForNull<IEnumerable<long>>(fileIds, nameof (fileIds));
      try
      {
        if (!fileIds.Any<long>())
          return (IDictionary<long, long>) new Dictionary<long, long>();
        using (FileComponent fileComponent = this.CreateFileComponent(requestContext, new OwnerId?(ownerId)))
          return fileComponent.QueryFileSizes(fileIds, dataspaceId, ownerId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14451, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14452, "FileService", "Service", "QueryFileSizes");
      }
    }

    public virtual Stream RetrieveFile(
      IVssRequestContext requestContext,
      long fileId,
      out CompressionType compressionType)
    {
      return this.RetrieveFile(requestContext, new FileIdentifier(fileId), true, out byte[] _, out long _, out compressionType, out string _, true);
    }

    public virtual Stream RetrieveFile(
      IVssRequestContext requestContext,
      FileIdentifier fileId,
      out CompressionType compressionType)
    {
      return this.RetrieveFile(requestContext, fileId, true, out byte[] _, out long _, out compressionType, out string _, true);
    }

    public virtual Stream RetrieveFile(
      IVssRequestContext requestContext,
      long fileId,
      bool compressOutput,
      out byte[] hashValue,
      out long contentLength,
      out CompressionType compressionType)
    {
      return this.RetrieveFile(requestContext, new FileIdentifier(fileId), compressOutput, out hashValue, out contentLength, out compressionType, out string _, true);
    }

    public Stream RetrieveNamedFile(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      string fileName,
      bool compressOutput,
      out byte[] hashValue,
      out long contentLength,
      out CompressionType compressionType)
    {
      requestContext.TraceEnter(14000, "FileService", "Service", nameof (RetrieveNamedFile));
      try
      {
        hashValue = TeamFoundationFileService.s_nullHash;
        contentLength = 0L;
        compressionType = CompressionType.None;
        int fileIdFromFileName;
        using (FileComponent fileComponent = this.CreateFileComponent(requestContext, new OwnerId?(ownerId)))
        {
          fileIdFromFileName = fileComponent.GetFileIdFromFileName(ownerId, fileName);
          if (fileIdFromFileName == 0)
            return (Stream) null;
        }
        return this.RetrieveFile(requestContext, new FileIdentifier((long) fileIdFromFileName), compressOutput, out hashValue, out contentLength, out compressionType, out string _, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14008, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14009, "FileService", "Service", nameof (RetrieveNamedFile));
      }
    }

    public bool TryGetFileId(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      string fileName,
      out int fileId)
    {
      long fileId1;
      int num = this.TryGetFileId64(requestContext, ownerId, fileName, out fileId1) ? 1 : 0;
      fileId = (int) fileId1;
      return num != 0;
    }

    public bool TryGetFileId64(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      string fileName,
      out long fileId)
    {
      requestContext.TraceEnter(15003, "FileService", "Service", "GetFileId");
      try
      {
        using (FileComponent fileComponent = this.CreateFileComponent(requestContext, new OwnerId?(ownerId)))
        {
          fileId = (long) fileComponent.GetFileIdFromFileName(ownerId, fileName);
          return fileId != 0L;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15004, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15005, "FileService", "Service", "GetFileId");
      }
    }

    public virtual int UploadFile(IVssRequestContext requestContext, byte[] content) => (int) this.UploadFile64(requestContext, content);

    public virtual long UploadFile64(IVssRequestContext requestContext, byte[] content)
    {
      byte[] md5 = MD5Util.CalculateMD5(content);
      return this.UploadFile64(requestContext, content, md5, (long) content.Length, CompressionType.None);
    }

    internal long UploadFile64(IVssRequestContext requestContext, byte[] content, byte[] hashValue) => this.UploadFile64(requestContext, content, hashValue, (long) content.Length, CompressionType.None);

    internal long UploadFile64(
      IVssRequestContext requestContext,
      byte[] content,
      byte[] hashValue,
      long uncompressedLength,
      CompressionType compressionType)
    {
      long fileId = 0;
      this.UploadFile64(requestContext, ref fileId, hashValue, uncompressedLength, (long) content.Length, compressionType, 0L, content, content.Length, OwnerId.Generic, Guid.Empty, (string) null);
      return fileId;
    }

    public virtual int UploadFile(
      IVssRequestContext requestContext,
      Stream content,
      OwnerId ownerId,
      Guid dataspaceIdentifier)
    {
      return (int) this.UploadFile64(requestContext, content, ownerId, dataspaceIdentifier);
    }

    public virtual long UploadFile64(
      IVssRequestContext requestContext,
      Stream content,
      OwnerId ownerId,
      Guid dataspaceIdentifier)
    {
      return this.UploadFile64(requestContext, content, ownerId, dataspaceIdentifier, (string) null);
    }

    public long UploadFile64(
      IVssRequestContext requestContext,
      Stream content,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName)
    {
      long position = content.Position;
      byte[] md5 = MD5Util.CalculateMD5(content, true);
      content.Position = position;
      return this.UploadFile64(requestContext, content, md5, content.Length, CompressionType.None, ownerId, dataspaceIdentifier, fileName, false);
    }

    public int UploadFile(
      IVssRequestContext requestContext,
      Stream content,
      byte[] hashValue,
      OwnerId ownerId,
      Guid dataspaceIdentifier)
    {
      return (int) this.UploadFile64(requestContext, content, hashValue, ownerId, dataspaceIdentifier);
    }

    public long UploadFile64(
      IVssRequestContext requestContext,
      Stream content,
      byte[] hashValue,
      OwnerId ownerId,
      Guid dataspaceIdentifier)
    {
      return this.UploadFile64(requestContext, content, hashValue, content.Length, CompressionType.None, ownerId, dataspaceIdentifier, (string) null, false);
    }

    public int UploadFile(
      IVssRequestContext requestContext,
      Stream content,
      byte[] hashValue,
      long uncompressedLength,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName)
    {
      return (int) this.UploadFile64(requestContext, content, hashValue, uncompressedLength, compressionType, ownerId, dataspaceIdentifier, fileName);
    }

    public long UploadFile64(
      IVssRequestContext requestContext,
      Stream content,
      byte[] hashValue,
      long uncompressedLength,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName)
    {
      long fileId = 0;
      this.UploadFile64(requestContext, ref fileId, content, hashValue, content.Length, uncompressedLength, 0L, compressionType, ownerId, dataspaceIdentifier, fileName, false);
      return fileId;
    }

    internal long UploadFile64(
      IVssRequestContext requestContext,
      Stream content,
      byte[] hashValue,
      long uncompressedLength,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool compressContent)
    {
      long fileId = 0;
      this.UploadFile64(requestContext, ref fileId, content, hashValue, content.Length, uncompressedLength, 0L, compressionType, ownerId, dataspaceIdentifier, fileName, compressContent);
      return fileId;
    }

    public bool UploadFile(
      IVssRequestContext requestContext,
      ref int fileId,
      Stream content,
      byte[] hashValue,
      long compressedLength,
      long uncompressedLength,
      long offsetFrom,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool compressContent = false)
    {
      long fileId1 = (long) fileId;
      int num = this.UploadFile64(requestContext, ref fileId1, content, hashValue, compressedLength, uncompressedLength, offsetFrom, compressionType, ownerId, dataspaceIdentifier, fileName, compressContent) ? 1 : 0;
      fileId = (int) fileId1;
      return num != 0;
    }

    public bool UploadFile64(
      IVssRequestContext requestContext,
      ref long fileId,
      Stream content,
      byte[] hashValue,
      long compressedLength,
      long uncompressedLength,
      long offsetFrom,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool compressContent = false)
    {
      return this.UploadFile64(requestContext, ref fileId, content, hashValue, compressedLength, uncompressedLength, offsetFrom, compressionType, ownerId, dataspaceIdentifier, fileName, compressContent, true);
    }

    internal FileIdentifier UploadFile64(
      IVssRequestContext requestContext,
      Guid dataspaceIdentifier,
      OwnerId ownerId,
      Stream content,
      byte[] hashValue,
      long uncompressedLength,
      CompressionType compressionType,
      string fileName,
      bool compressContent)
    {
      return new FileIdentifier(this.UploadFile64(requestContext, content, hashValue, uncompressedLength, compressionType, ownerId, dataspaceIdentifier, fileName, compressContent), dataspaceIdentifier, ownerId);
    }

    public bool UploadFile(
      IVssRequestContext requestContext,
      ref int fileId,
      Stream content,
      byte[] hashValue,
      long compressedLength,
      long uncompressedLength,
      long offsetFrom,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool compressContent,
      bool useRemoteBlobStoreIfPossible)
    {
      long fileId1 = (long) fileId;
      int num = this.UploadFile64(requestContext, ref fileId1, content, hashValue, compressedLength, uncompressedLength, offsetFrom, compressionType, ownerId, dataspaceIdentifier, fileName, compressContent, useRemoteBlobStoreIfPossible) ? 1 : 0;
      fileId = (int) fileId1;
      return num != 0;
    }

    public bool UploadFile64(
      IVssRequestContext requestContext,
      ref long fileId,
      Stream content,
      byte[] hashValue,
      long compressedLength,
      long uncompressedLength,
      long offsetFrom,
      CompressionType compressionType,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool compressContent,
      bool useRemoteBlobStoreIfPossible)
    {
      requestContext.TraceEnter(14020, "FileService", "Service", "UploadFile");
      if (compressContent && compressionType != CompressionType.None)
        throw new ArgumentException("Cannot set compressContent to true when content is already compressed.", nameof (compressContent));
      if (compressContent && !content.CanSeek)
        throw new ArgumentException("compressContent cannot be set to true when content.CanSeek is false.", nameof (compressContent));
      Stream stream = content;
      try
      {
        bool flag = false;
        long num = !stream.CanSeek ? (compressionType == CompressionType.None ? uncompressedLength : compressedLength) : stream.Length;
        if (compressContent)
        {
          long position = stream.Position;
          stream = this.CopyStreamToTempStream(requestContext, stream, num, ref compressionType, true, false);
          if (compressionType != CompressionType.None && stream != null && stream.Length < num)
          {
            num = stream.Length;
            compressedLength = num;
            requestContext.Trace(14121, TraceLevel.Info, "FileService", "Service", "Successfully compressed file with compression ratio '{0}'.", (object) ((double) stream.Length / (double) num));
          }
          else
          {
            requestContext.Trace(14122, TraceLevel.Info, "FileService", "Service", "Failed to compress file.");
            stream?.Dispose();
            stream = content;
            stream.Position = position;
            compressionType = CompressionType.None;
          }
        }
        using (ByteArray byteArray = new ByteArray((int) Math.Min((long) TeamFoundationFileService.s_maxContentSize, num)))
        {
          byte[] bytes = byteArray.Bytes;
          int contentBlockLength;
          do
          {
            contentBlockLength = stream.Read(bytes, 0, bytes.Length);
            if (contentBlockLength != 0 || bytes.Length == 0)
              flag = this.UploadFile64(requestContext, ref fileId, hashValue, uncompressedLength, compressedLength, compressionType, offsetFrom, bytes, contentBlockLength, ownerId, dataspaceIdentifier, fileName, useRemoteBlobStoreIfPossible);
            offsetFrom += (long) contentBlockLength;
          }
          while (contentBlockLength > 0);
        }
        return flag;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14028, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        if (stream != content)
          stream.Dispose();
        requestContext.TraceLeave(14029, "FileService", "Service", "UploadFile");
      }
    }

    public void RenameFile(IVssRequestContext requestContext, long fileId, string newFileName)
    {
      requestContext.TraceEnter(15000, "FileService", "Service", nameof (RenameFile));
      try
      {
        using (FileComponent component = requestContext.CreateComponent<FileComponent>())
          component.RenameFile(fileId, newFileName);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15001, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15002, "FileService", "Service", nameof (RenameFile));
      }
    }

    public void DeleteFile(IVssRequestContext requestContext, long fileId) => this.DeleteFile(requestContext, new FileIdentifier(fileId));

    public void DeleteFile(IVssRequestContext requestContext, FileIdentifier fileId)
    {
      requestContext.TraceEnter(14040, "FileService", "Service", nameof (DeleteFile));
      try
      {
        using (FileComponent fileComponent = this.CreateFileComponent(requestContext, fileId.OwnerId))
          fileComponent.DeleteFile(fileId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14048, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14049, "FileService", "Service", nameof (DeleteFile));
      }
    }

    public bool SoftDeleteCorruptedFile(
      IVssRequestContext requestContext,
      FileIdentifier fileId,
      bool force = false)
    {
      bool flag = force;
      if (!force)
      {
        try
        {
          this.RetrieveFile(requestContext, fileId, out CompressionType _);
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(14058, TraceLevel.Info, "FileService", "Service", string.Format("File: {0} throw the expected exception: {1}", (object) fileId, (object) ex.ToReadableStackTrace()));
          requestContext.TraceException(14058, "FileService", "Service", ex);
          flag = true;
        }
      }
      if (!flag)
        return false;
      try
      {
        using (FileComponent fileComponent = this.CreateFileComponent(requestContext, fileId.OwnerId))
        {
          fileComponent.SoftDeleteFile(fileId);
          return true;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14059, "FileService", "Service", ex);
        throw;
      }
    }

    public virtual void DeleteFiles(IVssRequestContext requestContext, IEnumerable<int> fileIds) => this.DeleteFiles(requestContext, fileIds.Select<int, long>((Func<int, long>) (id => (long) id)));

    public virtual void DeleteFiles(IVssRequestContext requestContext, IEnumerable<long> fileIds) => this.DeleteFiles(requestContext, fileIds.Select<long, FileIdentifier>((Func<long, FileIdentifier>) (fileId => new FileIdentifier(fileId))));

    public virtual void DeleteFiles(
      IVssRequestContext requestContext,
      IEnumerable<FileIdentifier> fileIds)
    {
      requestContext.TraceEnter(14060, "FileService", "Service", nameof (DeleteFiles));
      try
      {
        if (!fileIds.Any<FileIdentifier>())
          return;
        FileIdentifier var = fileIds.FirstOrDefault<FileIdentifier>();
        ArgumentUtility.CheckForNull<FileIdentifier>(var, "fileId");
        using (FileComponent fileComponent = this.CreateFileComponent(requestContext, var.OwnerId))
          fileComponent.DeleteFiles(fileIds);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14068, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14069, "FileService", "Service", nameof (DeleteFiles));
      }
    }

    public void DeleteNamedFiles(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      IEnumerable<string> fileNames)
    {
      requestContext.TraceEnter(14080, "FileService", "Service", nameof (DeleteNamedFiles));
      try
      {
        using (FileComponent fileComponent = this.CreateFileComponent(requestContext, new OwnerId?(ownerId)))
          fileComponent.DeleteNamedFiles(ownerId, fileNames);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14088, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14089, "FileService", "Service", nameof (DeleteNamedFiles));
      }
    }

    public List<FileStatistics> QueryNamedFiles(IVssRequestContext requestContext, OwnerId ownerId) => this.QueryNamedFiles(requestContext, ownerId, (string) null);

    public List<FileStatistics> QueryNamedFiles(
      IVssRequestContext requestContext,
      OwnerId ownerId,
      string pattern)
    {
      requestContext.TraceEnter(14100, "FileService", "Service", nameof (QueryNamedFiles));
      try
      {
        using (FileComponent fileComponent = this.CreateFileComponent(requestContext, new OwnerId?(ownerId)))
        {
          using (ResultCollection resultCollection = fileComponent.QueryFiles(ownerId, pattern))
            return resultCollection.GetCurrent<FileStatistics>().Items;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14108, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14109, "FileService", "Service", nameof (QueryNamedFiles));
      }
    }

    internal List<FileStatistics> QueryAllFiles(
      IVssRequestContext requestContext,
      RemoteStoreId remoteStoreId,
      Guid lastResourceId,
      int batchSize)
    {
      requestContext.TraceEnter(14105, "FileService", "Service", nameof (QueryAllFiles));
      try
      {
        using (FileComponent component = requestContext.CreateComponent<FileComponent>())
        {
          using (ResultCollection resultCollection = component.QueryAllFiles(remoteStoreId, lastResourceId, batchSize))
            return resultCollection.GetCurrent<FileStatistics>().Items;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14106, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14107, "FileService", "Service", nameof (QueryAllFiles));
      }
    }

    internal void SwapFileResources(
      IVssRequestContext requestContext,
      long fileA,
      long fileB,
      bool deleteB)
    {
      requestContext.TraceEnter(14430, "FileService", "Service", nameof (SwapFileResources));
      try
      {
        using (FileComponent component = requestContext.CreateComponent<FileComponent>())
          component.SwapFileResources(fileA, fileB, deleteB);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14438, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14439, "FileService", "Service", nameof (SwapFileResources));
      }
    }

    private List<Guid> DeleteBlobs(
      IVssRequestContext requestContext,
      List<Guid> filesToDelete,
      ref int numberOfFailures)
    {
      requestContext.Trace(14128, TraceLevel.Info, "FileService", "Service", string.Format("Deleting {0} blobs", (object) filesToDelete.Count));
      BackoffRetryManager backoffRetryManager = new BackoffRetryManager(BackoffRetryManager.ExponentialDelay(10, TimeSpan.FromMinutes(5.0)), (BackoffRetryManager.OnExceptionHandler) (retryContext =>
      {
        requestContext.TraceException(14134, "FileService", "Service", retryContext.Exception);
        return true;
      }));
      List<Guid> filesDeleted = (List<Guid>) null;
      Stopwatch stopwatch = Stopwatch.StartNew();
      Action action = (Action) (() => filesDeleted = this.GetBlobProvider().DeleteBlobs(requestContext, requestContext.ServiceHost.InstanceId, filesToDelete));
      backoffRetryManager.Invoke(action);
      stopwatch.Stop();
      if (filesDeleted != null)
      {
        requestContext.Trace(14132, TraceLevel.Verbose, "FileService", "Service", string.Format("{0} Blobs Deleted in {1} ms", (object) filesDeleted.Count, (object) stopwatch.ElapsedMilliseconds));
      }
      else
      {
        filesDeleted = new List<Guid>(0);
        ++numberOfFailures;
      }
      return filesDeleted;
    }

    [Obsolete("Use GetFileIdUsage instead")]
    public (int, int) GetMinMaxFileId(IVssRequestContext requestContext)
    {
      using (FileComponent component = requestContext.CreateComponent<FileComponent>())
        return component.GetMinMaxFileId();
    }

    public FileIdUsage GetFileIdUsage(IVssRequestContext requestContext)
    {
      using (FileComponent component = requestContext.CreateComponent<FileComponent>())
        return component.GetFileIdUsage();
    }

    public (int numberOfFilesDeleted, int numberOfFailures) DoCleanup(
      IVssRequestContext requestContext)
    {
      return this.DoCleanup(requestContext, requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? 100 : 2000, this.m_sqlRetentionPeriod);
    }

    internal (int numberOfFilesDeleted, int numberOfFailures) DoCleanup(
      IVssRequestContext requestContext,
      int chunkSize,
      int retentionPeriodInDays,
      int? externalRetentionPeriod = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      int num = 0;
      int numberOfFailures = 0;
      requestContext.TraceEnter(14120, "FileService", "Service", nameof (DoCleanup));
      try
      {
        bool secondaryRangeFileId = this.GetReuseSecondaryRangeFileId(requestContext);
        using (FileComponent component = requestContext.CreateComponent<FileComponent>())
        {
          requestContext.Trace(14122, TraceLevel.Info, "FileService", "Service", "Deleting SQL files");
          component.DeleteUnusedFiles(retentionPeriodInDays, secondaryRangeFileId, chunkSize);
        }
        List<Guid> filesToDelete = (List<Guid>) null;
        if (this.GetBlobProvider() != null)
        {
          List<Guid> guidList;
          do
          {
            guidList = (List<Guid>) null;
            List<Tuple<int, int, Guid>> source = (List<Tuple<int, int, Guid>>) null;
            requestContext.Trace(14123, TraceLevel.Info, "FileService", "Service", "Getting list of pending upload files to delete");
            using (FileComponent component = requestContext.CreateComponent<FileComponent>())
            {
              if (component.Version >= 15)
                source = component.QueryOrphanPendingUploadFiles(TimeSpan.FromDays((double) retentionPeriodInDays));
            }
            if (source != null && source.Count > 0)
            {
              guidList = this.DeleteBlobs(requestContext, source.Select<Tuple<int, int, Guid>, Guid>((Func<Tuple<int, int, Guid>, Guid>) (File => File.Item3)).ToList<Guid>(), ref numberOfFailures);
              if (guidList.Count > 0)
              {
                HashSet<Guid> filesDeleteHashSet = new HashSet<Guid>((IEnumerable<Guid>) guidList);
                using (FileComponent component = requestContext.CreateComponent<FileComponent>())
                  component.DeleteOrphanPendingUploadFiles(source.Where<Tuple<int, int, Guid>>((Func<Tuple<int, int, Guid>, bool>) (File => filesDeleteHashSet.Contains(File.Item3))).Select<Tuple<int, int, Guid>, KeyValuePair<int, int>>((Func<Tuple<int, int, Guid>, KeyValuePair<int, int>>) (File => new KeyValuePair<int, int>(File.Item1, File.Item2))), secondaryRangeFileId);
              }
            }
          }
          while (!requestContext.IsCanceled && guidList != null && guidList.Count > 0);
          do
          {
            requestContext.Trace(14124, TraceLevel.Info, "FileService", "Service", "Getting list of blobs to delete");
            using (FileComponent component = requestContext.CreateComponent<FileComponent>())
            {
              if (component.Version >= 9)
                filesToDelete = component.QueryUnusedFiles(this.m_deletionChunkSize, -(externalRetentionPeriod ?? this.m_externalRetentionPeriod));
            }
            if (filesToDelete != null)
            {
              requestContext.Trace(14126, TraceLevel.Info, "FileService", "Service", "Got list of {0} blobs to delete", (object) filesToDelete.Count);
              guidList = this.DeleteBlobs(requestContext, filesToDelete, ref numberOfFailures);
              if (guidList.Count != 0)
              {
                using (FileComponent component = requestContext.CreateComponent<FileComponent>())
                {
                  if (component.Version >= 9)
                  {
                    component.HardDeleteFiles((IEnumerable<Guid>) guidList);
                    num += guidList.Count;
                  }
                }
              }
            }
          }
          while (!requestContext.IsCanceled && guidList != null && guidList.Count != 0);
        }
        return (num, numberOfFailures);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14138, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14139, "FileService", "Service", nameof (DoCleanup));
      }
    }

    public ComputePendingDeltasResult ComputePendingDeltas(
      IVssRequestContext requestContext,
      out string resultMessage)
    {
      requestContext.TraceEnter(14150, "FileService", "Service", nameof (ComputePendingDeltas));
      resultMessage = string.Empty;
      string str1 = "Job was momentarily disabled because there were too many exceptions or because servicing is in progress";
      string str2 = "Job was canceled by Job Agent";
      string str3 = "Job was suspended because a cleanup is in progress";
      try
      {
        if (this.m_deltaDisableTime > DateTime.UtcNow)
        {
          resultMessage = str1;
          return ComputePendingDeltasResult.Blocked;
        }
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        requestContext.Trace(14152, TraceLevel.Verbose, "FileService", "Service", "Starting delta computation run");
        try
        {
          if (requestContext.IsCanceled)
          {
            resultMessage = str2;
            requestContext.Trace(14154, TraceLevel.Info, "FileService", "Service", "Delta computation run is cancelled");
            return ComputePendingDeltasResult.Stopped;
          }
          List<Tuple<FileIdentifier, FileIdentifier, int>> tupleList;
          do
          {
            if (requestContext.GetService<TeamFoundationResourceManagementService>().GetVerifyServiceVersion(requestContext, "Default"))
            {
              this.m_deltaDisableTime = DateTime.UtcNow.AddMinutes(5.0);
              resultMessage = str1;
              requestContext.Trace(14163, TraceLevel.Info, "FileService", "Service", "Disabling Deltafication until {0}, because servicing is in progress.", (object) this.m_deltaDisableTime);
              return ComputePendingDeltasResult.Blocked;
            }
            using (FileComponent component = requestContext.CreateComponent<FileComponent>())
            {
              if (!(component is FileComponent12 fileComponent12))
                return ComputePendingDeltasResult.Blocked;
              tupleList = fileComponent12.QueryPendingDeltas2(this.m_maxPendingDeltas);
            }
            foreach (Tuple<FileIdentifier, FileIdentifier, int> delta in tupleList)
            {
              FileIdentifier newFileId = delta.Item1;
              FileIdentifier oldFileId = delta.Item2;
              int num = delta.Item3;
              if (requestContext.IsCanceled)
              {
                resultMessage = str2;
                requestContext.Trace(14156, TraceLevel.Info, "FileService", "Service", "Delta computation run is cancelled");
                return ComputePendingDeltasResult.Stopped;
              }
              try
              {
                this.ComputePendingDelta(requestContext, newFileId, oldFileId, 16);
              }
              catch (RequestCanceledException ex)
              {
                resultMessage = str2;
                requestContext.Trace(14158, TraceLevel.Info, "FileService", "Service", "Delta computation is cancelled");
                throw;
              }
              catch (CleanupJobInProgressException ex)
              {
                resultMessage = str3;
                requestContext.Trace(14159, TraceLevel.Warning, "FileService", "Service", "Cleanup job in progress - bailing out");
                throw;
              }
              catch (Exception ex)
              {
                requestContext.TraceException(14160, "FileService", "Service", ex);
                requestContext.Trace(14161, TraceLevel.Error, "FileService", "Service", "Error occurred creating delta for {0} <- {1}", (object) newFileId, (object) oldFileId);
                this.LogDeltaException(delta, ex, num < this.m_maxRetryCount);
                if ((DateTime.UtcNow - this.m_lastDeltaExceptionTime).TotalSeconds < 60.0)
                {
                  resultMessage = str1;
                  this.m_deltaDisableTime = DateTime.UtcNow.AddMinutes(5.0);
                  requestContext.Trace(14162, TraceLevel.Warning, "FileService", "Service", "Disabling Deltafication until {0}", (object) this.m_deltaDisableTime);
                  return ComputePendingDeltasResult.Blocked;
                }
                this.m_lastDeltaExceptionTime = DateTime.UtcNow;
              }
            }
            if (requestContext.IsCanceled)
            {
              resultMessage = str2;
              requestContext.Trace(14164, TraceLevel.Info, "FileService", "Service", "Delta computation is cancelled");
              return ComputePendingDeltasResult.Stopped;
            }
          }
          while (tupleList.Count > 0);
        }
        catch (RequestCanceledException ex)
        {
          resultMessage = str2;
          requestContext.Trace(14166, TraceLevel.Warning, "FileService", "Service", "Delta computation is cancelled");
          return ComputePendingDeltasResult.Stopped;
        }
        catch (CleanupJobInProgressException ex)
        {
          resultMessage = str3;
          requestContext.Trace(14168, TraceLevel.Warning, "FileService", "Service", "Delta computation is postponed");
          return ComputePendingDeltasResult.Blocked;
        }
        requestContext.Trace(14170, TraceLevel.Info, "FileService", "Service", "Delta computation is done!");
        return ComputePendingDeltasResult.Succeeded;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14178, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14179, "FileService", "Service", nameof (ComputePendingDeltas));
      }
    }

    internal void ComputePendingDelta(
      IVssRequestContext requestContext,
      FileIdentifier newFileId,
      FileIdentifier oldFileId,
      int maxDeltaChainLength)
    {
      requestContext.TraceEnter(14190, "FileService", "Service", nameof (ComputePendingDelta));
      try
      {
        requestContext.Trace(14192, TraceLevel.Verbose, "FileService", "Service", "Computing delta between file {0} and file {1}", (object) newFileId, (object) oldFileId);
        FileStream fileStream1 = (FileStream) null;
        FileStream fileStream2 = (FileStream) null;
        if (newFileId.FileId == oldFileId.FileId)
        {
          requestContext.Trace(14194, TraceLevel.Warning, "FileService", "Service", "Skipping nonsensical delta between file {0} and file {0}", (object) newFileId);
          this.DeletePendingDelta(requestContext, newFileId, oldFileId);
        }
        else
        {
          int num;
          using (FileComponent component = requestContext.CreateComponent<FileComponent>())
            num = component.DecrementDeltaRetryCount(newFileId, oldFileId, maxDeltaChainLength);
          if (num == 0)
          {
            this.DeletePendingDelta(requestContext, newFileId, oldFileId);
            requestContext.Trace(14196, TraceLevel.Warning, "FileService", "Service", "Abandoning Delta between {0} and {1}", (object) newFileId, (object) oldFileId);
          }
          else
          {
            try
            {
              try
              {
                requestContext.Trace(14198, TraceLevel.Verbose, "FileService", "Service", "Reading Old File {0} to deltafy", (object) oldFileId);
                byte[] hashValue;
                long contentLength;
                CompressionType compressionType;
                string fileName;
                fileStream1 = (FileStream) this.RetrieveFile(requestContext, oldFileId, false, out hashValue, out contentLength, out compressionType, out fileName, true, true, true);
                requestContext.Trace(14200, TraceLevel.Verbose, "FileService", "Service", "Reading New File {0} to deltafy against", (object) newFileId);
                fileStream2 = (FileStream) this.RetrieveFile(requestContext, newFileId, false, out hashValue, out contentLength, out compressionType, out fileName, true, true, true);
              }
              catch (FileIdNotFoundException ex)
              {
                requestContext.Trace(14202, TraceLevel.Warning, "FileService", "Service", "FileIdNotFoundException while retrieving file - abandoning Delta between {0} and {1}", (object) newFileId, (object) oldFileId);
                this.DeletePendingDelta(requestContext, newFileId, oldFileId);
                return;
              }
              if (fileStream1 == null || fileStream2 == null)
              {
                requestContext.Trace(14206, TraceLevel.Info, "FileService", "Service", "beforeFile and/or afterFile is null - abandoning Delta between {0} and {1}", (object) oldFileId, (object) newFileId);
                this.DeletePendingDelta(requestContext, newFileId, oldFileId);
                return;
              }
              using (FileStream fileStream3 = File.Create(TeamFoundationFileService.GetTempFileName(), TeamFoundationFileService.s_bufferSize, FileOptions.DeleteOnClose))
              {
                requestContext.Trace(14208, TraceLevel.Verbose, "FileService", "Service", "Creating Delta");
                int file = DeltaLibrary.CreateFile(fileStream2.SafeFileHandle, fileStream1.SafeFileHandle, fileStream3.SafeFileHandle);
                if (fileStream3.Length < fileStream1.Length && file == 0)
                {
                  requestContext.Trace(14210, TraceLevel.Verbose, "FileService", "Service", "Delta Created Successfully");
                  bool flag = false;
                  using (ByteArray byteArray = new ByteArray((int) fileStream3.Length))
                  {
                    using (FileComponent component = requestContext.CreateComponent<FileComponent>())
                    {
                      fileStream3.Seek(0L, SeekOrigin.Begin);
                      fileStream3.Read(byteArray.Bytes, 0, byteArray.Bytes.Length);
                      requestContext.Trace(14212, TraceLevel.Verbose, "FileService", "Service", "Saving Delta");
                      try
                      {
                        if (component.Version >= 18)
                        {
                          Guid empty = Guid.Empty;
                          ArraySegment<byte> content = new ArraySegment<byte>(byteArray.Bytes, 0, (int) fileStream3.Length);
                          Guid guid = component.StoreDelta(newFileId, oldFileId, content);
                          using (TeamFoundationFileSet files = component.RetrieveFile(guid, failOnDelete: false))
                          {
                            if (files.Metadata == null)
                            {
                              flag = true;
                            }
                            else
                            {
                              try
                              {
                                this.RetrieveFile(requestContext, 1023L, component, files, (object) guid, files.Metadata.Metadata.CompressionType == CompressionType.GZip, false, false, false, out byte[] _, out long _, out CompressionType _, out string _);
                              }
                              catch (Exception ex) when (ex is BadChecksumException || ex is Win32Exception)
                              {
                                requestContext.TraceException(14213, TraceLevel.Error, "FileService", "Service", ex);
                              }
                            }
                            component.SaveDelta(newFileId, oldFileId, guid);
                          }
                        }
                        else
                          component.SaveDelta(newFileId, oldFileId, byteArray.Bytes, (int) fileStream3.Length, RemoteStoreId.SqlServer);
                      }
                      catch (FileIdNotFoundException ex)
                      {
                        requestContext.TraceException(14214, TraceLevel.Warning, "FileService", "Service", (Exception) ex);
                        flag = true;
                      }
                    }
                  }
                  if (flag)
                  {
                    try
                    {
                      requestContext.Trace(14216, TraceLevel.Verbose, "FileService", "Service", "Deleting Pending Delta between {0} and {1}", (object) oldFileId, (object) newFileId);
                      this.DeletePendingDelta(requestContext, newFileId, oldFileId);
                    }
                    catch (Exception ex)
                    {
                      requestContext.TraceException(14218, "FileService", "Service", ex);
                    }
                  }
                }
                else
                {
                  if (file == 0)
                  {
                    requestContext.Trace(14220, TraceLevel.Info, "FileService", "Service", "Skipped delta creation for {0};{1}.  Delta longer than full content - deleting pending delta", (object) newFileId, (object) oldFileId);
                    this.DeletePendingDelta(requestContext, newFileId, oldFileId);
                    return;
                  }
                  requestContext.Trace(14222, TraceLevel.Error, "FileService", "Service", "Unexpected Error code {0} was not mapped to an exception!", (object) file);
                }
              }
            }
            finally
            {
              fileStream1?.Dispose();
              fileStream2?.Dispose();
            }
            requestContext.Trace(14224, TraceLevel.Info, "FileService", "Service", "Successfully created delta for {0};{1}", (object) newFileId, (object) oldFileId);
          }
        }
      }
      catch (CleanupJobInProgressException ex)
      {
        requestContext.TraceException(14226, TraceLevel.Warning, "FileService", "Service", (Exception) ex);
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14228, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14229, "FileService", "Service", nameof (ComputePendingDelta));
      }
    }

    public static CompressionType FromMimeType(string mimeType)
    {
      if (VssStringComparer.ContentType.Equals(mimeType, "application/gzip") || VssStringComparer.ContentType.Equals(mimeType, "application/x-gzip"))
        return CompressionType.GZip;
      if (VssStringComparer.ContentType.Equals(mimeType, "application/octet-stream"))
        return CompressionType.None;
      throw new ArgumentException(FrameworkResources.UnsupportedContentType((object) mimeType));
    }

    public static string ToMimeType(CompressionType compressionType)
    {
      if (compressionType == CompressionType.None)
        return "application/octet-stream";
      return compressionType == CompressionType.GZip ? "application/gzip" : "";
    }

    internal void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(14240, "FileService", "Service", nameof (OnRegistryChanged));
      try
      {
        if (!changedEntries.Any<RegistryEntry>())
          return;
        this.LoadSettings(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14248, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14249, "FileService", "Service", nameof (OnRegistryChanged));
      }
    }

    private void OnSecondaryBlobProvidersChanged(IVssRequestContext requestContext)
    {
      requestContext.TraceAlways(14619, TraceLevel.Info, "FileService", "Service", "Secondary blob providers changed, re-initializing blob providers.");
      this.InitializeBlobProviders(requestContext);
    }

    private void OnRemoteBlobProviderChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceAlways(14619, TraceLevel.Info, "FileService", "Service", "FileServiceRemoteBlobProvider changed, re-initializing blob providers.");
      this.InitializeBlobProviders(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(14270, "FileService", "Service", nameof (LoadSettings));
      try
      {
        RegistryEntryCollection registryEntries = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (FrameworkServerConstants.FileServiceRegistryRootPath + "/*"));
        this.m_maxRetryCount = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.FileServiceMaxRetryCount, 5);
        this.m_maxPendingDeltas = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.FileServiceMaxPendingDeltas, (int) byte.MaxValue);
        this.m_maxPatchableSize = (long) registryEntries.GetValueFromPath<int>(FrameworkServerConstants.FileServiceMaxPatchableSize, 4194304);
        this.m_tempFileSizeThreshold = (long) registryEntries.GetValueFromPath<int>(FrameworkServerConstants.FileServiceTempFileSizeThreshold, 1048576);
        this.m_externalRetentionPeriod = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.FileServiceRemoteRetentionPeriod, 21);
        this.m_sqlRetentionPeriod = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.FileServiceSqlRetentionPeriod, 1);
        this.m_deletionChunkSize = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.FileServiceDeletionChunkSize, 1000);
        this.m_alwaysValidateUploads = registryEntries.GetValueFromPath<bool>(FrameworkServerConstants.FileServiceAlwaysValidateUploads, true);
        this.m_minRemoteBlobSize = registryEntries.GetValueFromPath<long>(FrameworkServerConstants.FileServiceMinRemoteBlobSize, 1L);
        this.m_delaySecondFileRegistryUpdate = registryEntries.GetValueFromPath<int>(FrameworkServerConstants.FileServiceDelayFileIdRegistryUpdate, 300);
        this.m_storageAccountId = requestContext.ServiceHost.ServiceHostInternal().StorageAccountId;
        this.m_useSecondaryFileIdRangeSet = requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.FileService.FileIdSecondaryRange") ? this.OwnerIds(requestContext, registryEntries) : (HashSet<OwnerId>) null;
        this.m_reuseSecondaryRangeFileId = requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.FileService.ReuseSecondaryRangeFileId");
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return;
        ITeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
        if (this.m_updateRegistryTask != null)
          service.RemoveTask(requestContext, this.m_updateRegistryTask);
        this.m_updateRegistryTask = new TeamFoundationTask((TeamFoundationTaskCallback) ((systemContext, args) =>
        {
          systemContext.Trace(14343, TraceLevel.Info, "FileService", "Service", "Saving FileId: {0} in the registry for hostId: {1}", (object) this.m_fileId, (object) systemContext.ServiceHost.InstanceId);
          this.WriteFileIdToRegistry(systemContext, this.m_delaySecondFileRegistryUpdate);
        }), (object) null, 1000 * this.m_delaySecondFileRegistryUpdate);
        service.AddTask(requestContext, this.m_updateRegistryTask);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14278, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14279, "FileService", "Service", nameof (LoadSettings));
      }
    }

    private HashSet<OwnerId> OwnerIds(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      string valueFromPath = registryEntries.GetValueFromPath<string>(FrameworkServerConstants.FileServiceOwnerIdsUsingSecondaryRange, string.Empty);
      return this.ParseOwnerIdsUsingSecondaryRange(requestContext, valueFromPath);
    }

    internal virtual bool IsUsingSecondaryRange(IVssRequestContext requestContext, OwnerId ownerId) => this.m_useSecondaryFileIdRangeSet != null && this.m_useSecondaryFileIdRangeSet.Contains(ownerId);

    private HashSet<OwnerId> ParseOwnerIdsUsingSecondaryRange(
      IVssRequestContext requestContext,
      string ownerIds)
    {
      if (ownerIds == null)
        return (HashSet<OwnerId>) null;
      HashSet<OwnerId> usingSecondaryRange = new HashSet<OwnerId>();
      string str1 = ownerIds;
      char[] chArray = new char[1]{ ';' };
      foreach (string str2 in str1.Split(chArray))
      {
        OwnerId result;
        if (Enum.TryParse<OwnerId>(str2, out result))
        {
          usingSecondaryRange.Add(result);
        }
        else
        {
          requestContext.TraceAlways(14527, TraceLevel.Error, "FileService", "Service", "The registry at " + FrameworkServerConstants.FileServiceOwnerIdsUsingSecondaryRange + " cannot be parsed");
          return (HashSet<OwnerId>) null;
        }
      }
      return usingSecondaryRange;
    }

    private void UpdateFileIdOnSQL(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(14260, "FileService", "Service", nameof (UpdateFileIdOnSQL));
      try
      {
        using (FileComponent component = requestContext.CreateComponent<FileComponent>())
          component.UpdateSequenceFileId(this.m_fileId, false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14268, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14269, "FileService", "Service", nameof (UpdateFileIdOnSQL));
      }
    }

    private void InitializeBlobProviders(IVssRequestContext systemContext)
    {
      systemContext.TraceEnter(14280, "FileService", "Service", nameof (InitializeBlobProviders));
      try
      {
        systemContext.CheckSystemRequestContext();
        string remoteBlobProviderType;
        string remoteBlobProviderAssembly;
        if (!TeamFoundationFileService.TryGetBlobProviderInfo(systemContext, out remoteBlobProviderType, out remoteBlobProviderAssembly))
          return;
        List<string> stringList = new List<string>();
        int storageAccountId = 0;
        while (true)
        {
          string connectionString = TeamFoundationFileService.GetStorageAccountConnectionString(systemContext, storageAccountId, false);
          if (connectionString != null)
          {
            stringList.Add(connectionString);
            ++storageAccountId;
          }
          else
            break;
        }
        if (stringList.Count == 0)
          throw new RemoteBlobConfigurationException("No storage accounts configured for remote blob provider. Check FileServiceStorageAccount keyvault secret.");
        ICompositeBlobProviderService service = systemContext.GetService<ICompositeBlobProviderService>();
        bool flag = service.HasSecondaryProviders(systemContext);
        IBlobProvider[] blobProviderArray = new IBlobProvider[stringList.Count];
        for (int index = 0; index < stringList.Count; ++index)
        {
          try
          {
            IBlobProvider provider = TeamFoundationFileService.CreateBlobProvider<IBlobProvider>(systemContext, remoteBlobProviderType, remoteBlobProviderAssembly);
            if (provider != null)
            {
              Type type = provider.GetType();
              systemContext.Trace(14262, TraceLevel.Info, "FileService", "Service", "Found plugin {0}", (object) type.AssemblyQualifiedName);
              systemContext.Trace(14264, TraceLevel.Info, "FileService", "Service", "Starting plugin {0}", (object) type);
              Dictionary<string, string> settings = new Dictionary<string, string>()
              {
                ["BlobStorageConnectionStringOverride"] = stringList[index],
                ["DrawerName"] = "ConfigurationSecrets",
                ["LookupKey"] = TeamFoundationFileService.GetStorageAccountLookupKey(index)
              };
              provider.ServiceStart(systemContext, (IDictionary<string, string>) settings);
              if (flag)
                provider = service.CreateCompositeBlobProvider(systemContext, provider);
              blobProviderArray[index] = provider;
              systemContext.Trace(14266, TraceLevel.Info, "FileService", "Service", "Plugin {0} started successfully", (object) type);
            }
            else
              systemContext.Trace(14268, TraceLevel.Error, "FileService", "Service", "Found no plugins implementing IBlobProvider");
            if (blobProviderArray[index] == null)
            {
              systemContext.Trace(14272, TraceLevel.Error, "FileService", "Service", "Remote Blob Provider {0}.{1} was configured but was not found or did not successfully start", (object) remoteBlobProviderAssembly, (object) remoteBlobProviderType);
              throw new InvalidOperationException();
            }
          }
          catch (Exception ex)
          {
            systemContext.TraceException(14287, "FileService", "Service", ex);
            throw;
          }
        }
        if (this.m_blobProviders != null)
        {
          foreach (IBlobProvider blobProvider in this.m_blobProviders)
            blobProvider.ServiceEnd(systemContext);
        }
        this.m_blobProviders = blobProviderArray;
      }
      catch (Exception ex)
      {
        systemContext.TraceException(14288, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        systemContext.TraceLeave(14289, "FileService", "Service", nameof (InitializeBlobProviders));
      }
    }

    internal static string GetStorageAccountLookupKey(int storageAccountId) => storageAccountId >= 0 ? string.Format("{0}{1}", (object) "FileServiceStorageAccount", (object) storageAccountId) : throw new ArgumentException("Invalid StorageAccountId! Should be greater or equal than 0");

    internal static string GetStorageAccountConnectionString(
      IVssRequestContext deploymentContext,
      int storageAccountId,
      bool throwIfNotFound = true)
    {
      string accountLookupKey = TeamFoundationFileService.GetStorageAccountLookupKey(storageAccountId);
      if (!deploymentContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        deploymentContext = deploymentContext.To(TeamFoundationHostType.Deployment);
      string connectionString = (string) null;
      ITeamFoundationStrongBoxService service = deploymentContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(deploymentContext, "ConfigurationSecrets", accountLookupKey, false);
      if (itemInfo == null)
      {
        if (throwIfNotFound)
          throw new StrongBoxItemNotFoundException(accountLookupKey);
      }
      else
        connectionString = service.GetString(deploymentContext, itemInfo);
      return connectionString;
    }

    private Stream RetrievePendingFile(
      IVssRequestContext requestContext,
      long fileId,
      bool compressOutput,
      out byte[] hashValue,
      out long contentLength,
      out CompressionType compressionType,
      out string fileName)
    {
      requestContext.TraceEnter(14290, "FileService", "Service", "RetrieveFile");
      try
      {
        using (FileComponent component = requestContext.CreateComponent<FileComponent>())
        {
          using (TeamFoundationFileSet files = component.RetrievePendingFile(fileId))
            return this.RetrieveFile(requestContext, fileId, component, files, (object) fileId, compressOutput, true, true, false, out hashValue, out contentLength, out compressionType, out fileName);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14309, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14310, "FileService", "Service", "RetrieveFile");
      }
    }

    private Stream RetrieveFile(
      IVssRequestContext requestContext,
      FileIdentifier fileId,
      bool compressOutput,
      out byte[] hashValue,
      out long contentLength,
      out CompressionType compressionType,
      out string fileName,
      bool failOnDeletedFile,
      bool returnNullIfDelta = false,
      bool forceFileStream = false)
    {
      requestContext.TraceEnter(14290, "FileService", "Service", nameof (RetrieveFile));
      try
      {
        using (FileComponent component = requestContext.CreateComponent<FileComponent>())
        {
          using (TeamFoundationFileSet files = component.RetrieveFile(fileId, failOnDelete: failOnDeletedFile))
            return this.RetrieveFile(requestContext, fileId.FileId, component, files, (object) fileId, compressOutput, failOnDeletedFile, returnNullIfDelta, forceFileStream, out hashValue, out contentLength, out compressionType, out fileName);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14309, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14310, "FileService", "Service", nameof (RetrieveFile));
      }
    }

    internal Stream RetrieveFile(
      IVssRequestContext requestContext,
      long fileId,
      FileComponent fileComponent,
      TeamFoundationFileSet files,
      object fileLookupKey,
      bool compressOutput,
      bool failOnDeletedFile,
      bool returnNullIfDelta,
      bool forceFileStream,
      out byte[] hashValue,
      out long contentLength,
      out CompressionType compressionType,
      out string fileName)
    {
      compressionType = CompressionType.None;
      hashValue = TeamFoundationFileService.s_nullHash;
      contentLength = 0L;
      fileName = (string) null;
      TeamFoundationFile metadata = files.Metadata;
      if (metadata == null)
      {
        if (failOnDeletedFile)
        {
          requestContext.Trace(14292, TraceLevel.Error, "FileService", "Service", "File {0} does not exist", fileLookupKey);
          throw new FileIdNotFoundException(fileId);
        }
        requestContext.Trace(14294, TraceLevel.Info, "FileService", "Service", "File {0} does not exist", fileLookupKey);
        return (Stream) null;
      }
      if (files.FullVersion == null)
        requestContext.Trace(14295, TraceLevel.Error, "FileService", "Service", "Full version is null. Delta chain is null: {0}. File Id: {1}. Remote Store Id: {2}.", (object) (files.DeltaChain == null), fileLookupKey, (object) metadata.Metadata.RemoteStoreId);
      fileName = metadata.Reference != null ? metadata.Reference.FileName : (string) null;
      contentLength = metadata.Metadata.FileLength;
      hashValue = metadata.Metadata.HashValue;
      compressionType = metadata.Metadata.CompressionType;
      if (metadata.Metadata.ContentType != ContentType.Delta)
      {
        requestContext.Trace(14296, TraceLevel.Verbose, "FileService", "Service", "File {0} is not a delta, returning full version", fileLookupKey);
        if (metadata.Metadata.RemoteStoreId != RemoteStoreId.SqlServer)
        {
          fileComponent.Dispose();
          if (compressOutput || compressionType == CompressionType.None)
          {
            if (metadata.Metadata.CompressedLength > this.m_tempFileSizeThreshold && !forceFileStream)
              return this.GetBlobProvider().GetStream(requestContext, requestContext.ServiceHost.InstanceId, metadata.Metadata.ResourceId.ToString("n"));
            long size = compressionType == CompressionType.GZip ? metadata.Metadata.CompressedLength : metadata.Metadata.FileLength;
            Stream targetStream = this.GetTempStream(requestContext, forceFileStream, size);
            try
            {
              this.GetBlobProvider().DownloadToStream(requestContext, requestContext.ServiceHost.InstanceId, metadata.Metadata.ResourceId.ToString("n"), targetStream);
              if (targetStream.Length != size)
                requestContext.Trace(14297, TraceLevel.Error, "FileService", "Service", "File {0} is corrupted. Expected length: {1}. Actual length: {2}.", fileLookupKey, (object) size, (object) targetStream.Length);
              targetStream.Seek(0L, SeekOrigin.Begin);
              Stream stream = targetStream;
              targetStream = (Stream) null;
              return stream;
            }
            finally
            {
              targetStream?.Dispose();
            }
          }
        }
        using (requestContext.AcquireExemptionLock())
        {
          using (Stream stream = this.GetStream(requestContext, files.FullVersion))
            return this.CopyStreamToTempStream(requestContext, stream, contentLength, ref compressionType, compressOutput, forceFileStream);
        }
      }
      else
      {
        if (returnNullIfDelta)
        {
          requestContext.Trace(14298, TraceLevel.Info, "FileService", "Service", "File {0} is a delta and caller requested nullIfDelta - returning null", fileLookupKey);
          return (Stream) null;
        }
        compressionType = files.FullVersion.Metadata.CompressionType;
        using (requestContext.AcquireExemptionLock())
        {
          using (Stream stream = this.GetStream(requestContext, files.FullVersion))
          {
            using (FileStream tempFile1 = this.CopyStreamToTempFile(requestContext, stream, ref compressionType, false))
            {
              while (files.DeltaChain.MoveNext())
              {
                TeamFoundationFile current = files.DeltaChain.Current;
                CompressionType compressionType1 = CompressionType.None;
                using (FileStream tempFile2 = this.CopyStreamToTempFile(requestContext, this.GetStream(requestContext, current), ref compressionType1, false))
                {
                  using (FileStream source = File.Create(TeamFoundationFileService.GetTempFileName(), TeamFoundationFileService.s_bufferSize, FileOptions.DeleteOnClose))
                  {
                    source.SetLength(current.Metadata.FileLength);
                    requestContext.Trace(14300, TraceLevel.Info, "FileService", "Service", "Applying Delta");
                    try
                    {
                      DeltaLibrary.ApplyHandles(tempFile1.SafeFileHandle, tempFile2.SafeFileHandle, source.SafeFileHandle);
                    }
                    catch
                    {
                      requestContext.Trace(14301, TraceLevel.Error, "FileService", "Service", "Delta threw an exception on the fileId: {0}", (object) fileId);
                      throw;
                    }
                    requestContext.Trace(14302, TraceLevel.Info, "FileService", "Service", "Delta Applied Successfully");
                    source.Seek(0L, SeekOrigin.Begin);
                    TeamFoundationFileService.ValidateLengthAndHash(requestContext, (Stream) source, current.Metadata.FileLength, current.Metadata.HashValue, false);
                    source.Seek(0L, SeekOrigin.Begin);
                    tempFile1.SetLength(0L);
                    this.CopyTo((Stream) source, (Stream) tempFile1, (int) Math.Min(current.Metadata.FileLength, 2097152L));
                    tempFile1.Seek(0L, SeekOrigin.Begin);
                  }
                }
              }
              requestContext.Trace(14308, TraceLevel.Verbose, "FileService", "Service", "Deltas Applied Successfully - returning full file");
              return this.CopyStreamToTempStream(requestContext, (Stream) tempFile1, contentLength, ref compressionType, compressOutput, forceFileStream);
            }
          }
        }
      }
    }

    internal void CopyTo(Stream source, Stream destination, int bufferSize)
    {
      if (bufferSize == 0)
        bufferSize = 1023;
      if (destination == null)
        throw new ArgumentNullException(nameof (destination));
      if (bufferSize < 0)
        throw new ArgumentOutOfRangeException(nameof (bufferSize));
      if (!source.CanRead && !source.CanWrite)
        throw new ObjectDisposedException(nameof (source));
      if (!destination.CanRead && !destination.CanWrite)
        throw new ObjectDisposedException(nameof (destination));
      if (!source.CanRead)
        throw new NotSupportedException();
      if (!destination.CanWrite)
        throw new NotSupportedException();
      this.InternalCopyTo(source, destination, bufferSize);
    }

    private void InternalCopyTo(Stream source, Stream destination, int bufferSize)
    {
      using (ByteArray byteArray = new ByteArray(bufferSize))
      {
        byte[] bytes = byteArray.Bytes;
        int count;
        while ((count = source.Read(bytes, 0, bytes.Length)) != 0)
          destination.Write(bytes, 0, count);
      }
    }

    private Stream GetStream(IVssRequestContext requestContext, TeamFoundationFile file)
    {
      if (file.Metadata.RemoteStoreId == RemoteStoreId.SqlServer)
        return file.Content;
      if (this.GetBlobProvider() == null)
        throw new BlobProviderConfigurationException();
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      Guid resourceId = file.Metadata.ResourceId;
      return this.GetBlobProvider().GetStream(requestContext, instanceId, resourceId.ToString("n"));
    }

    private void LogDeltaException(
      Tuple<FileIdentifier, FileIdentifier, int> delta,
      Exception exception,
      bool hasRemainingRetries)
    {
      TeamFoundationEventLog.Default.LogException("delta creation failed", exception, TeamFoundationEventId.DeltaComputationException, hasRemainingRetries ? EventLogEntryType.Warning : EventLogEntryType.Error);
    }

    internal bool UploadFile(
      IVssRequestContext requestContext,
      FileIdentifier fileIdentifier,
      byte[] hashValue,
      long fileLength,
      long compressedLength,
      CompressionType compressionType,
      long offsetFrom,
      byte[] contentBlock,
      int contentBlockLength,
      string fileName)
    {
      long fileId = fileIdentifier.FileId;
      int num = this.UploadFile64(requestContext, ref fileId, hashValue, fileLength, compressedLength, compressionType, offsetFrom, contentBlock, contentBlockLength, fileIdentifier.OwnerId.Value, fileIdentifier.DataspaceIdentifier.Value, fileName, true) ? 1 : 0;
      fileIdentifier.FileId = fileId;
      return num != 0;
    }

    public bool UploadFile(
      IVssRequestContext requestContext,
      ref int fileId,
      byte[] hashValue,
      long fileLength,
      long compressedLength,
      CompressionType compressionType,
      long offsetFrom,
      byte[] contentBlock,
      int contentBlockLength,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName)
    {
      long fileId1 = (long) fileId;
      int num = this.UploadFile64(requestContext, ref fileId1, hashValue, fileLength, compressedLength, compressionType, offsetFrom, contentBlock, contentBlockLength, ownerId, dataspaceIdentifier, fileName) ? 1 : 0;
      fileId = (int) fileId1;
      return num != 0;
    }

    public bool UploadFile64(
      IVssRequestContext requestContext,
      ref long fileId,
      byte[] hashValue,
      long fileLength,
      long compressedLength,
      CompressionType compressionType,
      long offsetFrom,
      byte[] contentBlock,
      int contentBlockLength,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName)
    {
      return this.UploadFile64(requestContext, ref fileId, hashValue, fileLength, compressedLength, compressionType, offsetFrom, contentBlock, contentBlockLength, ownerId, dataspaceIdentifier, fileName, true);
    }

    internal bool UploadFile64(
      IVssRequestContext requestContext,
      ref long fileId,
      byte[] hashValue,
      long fileLength,
      long compressedLength,
      CompressionType compressionType,
      long offsetFrom,
      byte[] contentBlock,
      int contentBlockLength,
      OwnerId ownerId,
      Guid dataspaceIdentifier,
      string fileName,
      bool useRemoteBlobStoreIfPossible)
    {
      requestContext.TraceEnter(14330, "FileService", "Service", "UploadFile");
      try
      {
        bool useRemoteProvider = false;
        bool verifyChecksum = true;
        TeamFoundationServerMD5 md5Context = (TeamFoundationServerMD5) null;
        requestContext.Trace(14332, TraceLevel.Verbose, "FileService", "Service", "FileService UploadFile: Start - FileLength {0}, compressed length {1}, Content Block length {2}, Compression Type {3}", (object) fileLength, (object) compressedLength, (object) contentBlockLength, (object) compressionType);
        requestContext.GetService<TeamFoundationResourceManagementService>();
        if (hashValue == null || hashValue.Length == 0)
          hashValue = TeamFoundationFileService.s_nullHash;
        if (compressionType != CompressionType.GZip && compressionType != CompressionType.None)
        {
          requestContext.Trace(14334, TraceLevel.Error, "FileService", "Service", "Caller specified {0} for fileId {1} which is not a known compression algorithm", (object) compressionType, (object) fileId);
          throw new IncompatibleCompressionFormatException();
        }
        if (compressionType == CompressionType.GZip && offsetFrom == 0L && compressedLength > 0L && (contentBlock[0] != (byte) 31 || contentBlock[1] != (byte) 139))
        {
          requestContext.Trace(14336, TraceLevel.Error, "FileService", "Service", "Caller specified GZip but file {0} doesn't have magic GZip bits", (object) fileId);
          throw new IncompatibleCompressionFormatException();
        }
        if (compressedLength == (long) contentBlockLength)
        {
          hashValue = TeamFoundationFileService.ValidateLengthAndHash(requestContext, (Stream) new MemoryStream(contentBlock, 0, (int) compressedLength, false), fileLength, hashValue, compressionType == CompressionType.GZip);
          verifyChecksum = false;
          requestContext.Trace(14338, TraceLevel.Info, "FileService", "Service", "Checksum and length validated for {0}", (object) fileId);
        }
        else if (compressionType == CompressionType.None)
        {
          using (FileComponent fileComponent = this.CreateFileComponent(requestContext, new OwnerId?(ownerId)))
          {
            if (fileComponent is FileComponent3 fileComponent3)
            {
              if (offsetFrom == 0L)
              {
                md5Context = new TeamFoundationServerMD5();
                md5Context.TransformBlock(contentBlock, 0, contentBlockLength, (byte[]) null, 0);
              }
              else
              {
                QueryMD5ContextResult md5ContextResult = fileComponent3.QueryMD5Context(ownerId, fileId);
                if (md5ContextResult == null)
                {
                  TeamFoundationFile teamFoundationFile = fileComponent.RetrieveStatistics(new FileIdentifier(fileId, dataspaceIdentifier, ownerId));
                  if (teamFoundationFile == null)
                  {
                    requestContext.Trace(14340, TraceLevel.Error, "FileService", "Service", "Cannot find file {0} {1} in database", (object) fileId, (object) ownerId);
                    throw new FileIdNotFoundException(fileId);
                  }
                  if (!ArrayUtil.Equals(TeamFoundationFileService.s_nullHash, hashValue) && ArrayUtil.Equals(teamFoundationFile.Metadata.HashValue, hashValue))
                  {
                    requestContext.Trace(14353, TraceLevel.Info, "FileService", "Service", "Chunk replay for file {0} at offset {1} with length {2}. File already complete and identical", (object) fileId, (object) offsetFrom, (object) contentBlockLength);
                    return true;
                  }
                  requestContext.Trace(14355, TraceLevel.Error, "FileService", "Service", "Chunk replay for file {0} at offset {1} with length {2}. File already complete, not identical", (object) fileId, (object) offsetFrom, (object) contentBlockLength);
                  throw new FileAlreadyUploadedException(fileId);
                }
                if (md5ContextResult.HasMD5ByteCount)
                {
                  if (offsetFrom == md5ContextResult.MD5ByteCount)
                  {
                    md5Context = new TeamFoundationServerMD5(md5ContextResult.MD5Context);
                    md5Context.TransformBlock(contentBlock, 0, contentBlockLength, (byte[]) null, 0);
                  }
                  else if (offsetFrom + (long) contentBlockLength <= md5ContextResult.MD5ByteCount)
                  {
                    using (TeamFoundationFileSet foundationFileSet = fileComponent3.RetrieveFile(new FileIdentifier(fileId, dataspaceIdentifier, ownerId), false))
                    {
                      TeamFoundationFile metadata = foundationFileSet.Metadata;
                      if (metadata == null)
                      {
                        requestContext.Trace(14352, TraceLevel.Info, "FileService", "Service", "Chunk replay for file {0} at offset {1} with length {2}. File incomplete", (object) fileId, (object) offsetFrom, (object) contentBlockLength);
                        return false;
                      }
                      if (TeamFoundationFileService.s_nullHash != hashValue && ArrayUtil.Equals(metadata.Metadata.HashValue, hashValue))
                      {
                        requestContext.Trace(14353, TraceLevel.Info, "FileService", "Service", "Chunk replay for file {0} at offset {1} with length {2}. File already complete and identical", (object) fileId, (object) offsetFrom, (object) contentBlockLength);
                        return true;
                      }
                      requestContext.Trace(14355, TraceLevel.Error, "FileService", "Service", "Chunk replay for file {0} at offset {1} with length {2}. File already complete, not identical", (object) fileId, (object) offsetFrom, (object) contentBlockLength);
                      throw new FileAlreadyUploadedException(fileId);
                    }
                  }
                  else
                  {
                    requestContext.Trace(14357, TraceLevel.Error, "FileService", "Service", "Cannot scatter upload when streaming MD5 is used (file {0}, offset {1}, length {2})", (object) fileId, (object) offsetFrom, (object) contentBlockLength);
                    throw new NotSupportedException();
                  }
                }
                else
                {
                  md5Context = new TeamFoundationServerMD5(md5ContextResult.MD5Context);
                  md5Context.TransformBlock(contentBlock, 0, contentBlockLength, (byte[]) null, 0);
                }
              }
            }
          }
        }
        if (((this.GetBlobProvider() == null ? 0 : (compressedLength > this.m_minRemoteBlobSize ? 1 : 0)) & (useRemoteBlobStoreIfPossible ? 1 : 0)) != 0)
          useRemoteProvider = true;
        requestContext.Trace(14342, TraceLevel.Info, "FileService", "Service", "FileService UploadFile: CompressedLength = {0}, MinRemoteBlobSize {1}, UseRemoteBlobStorage = {2}", (object) compressedLength, (object) this.m_minRemoteBlobSize, (object) useRemoteProvider);
        bool secondaryRangeFileId = this.GetReuseSecondaryRangeFileId(requestContext);
        Guid resourceId;
        bool isUploadComplete;
        using (FileComponent fileComponent = this.CreateFileComponent(requestContext, new OwnerId?(ownerId)))
        {
          RemoteStoreId remoteStoreId = useRemoteProvider ? this.GetBlobProvider().RemoteStoreId : RemoteStoreId.SqlServer;
          bool useSecondaryFileIdRange = this.IsUsingSecondaryRange(requestContext, ownerId);
          SaveFileResult saveFileResult = fileComponent.SaveFile(fileId, dataspaceIdentifier, ownerId, fileName, (byte[]) null, (byte[]) null, compressionType, remoteStoreId, hashValue, fileLength, compressedLength, md5Context?.GetBytes(), offsetFrom + (long) contentBlockLength, offsetFrom, useRemoteProvider ? (byte[]) null : contentBlock, contentBlockLength, true, useSecondaryFileIdRange, secondaryRangeFileId);
          fileId = saveFileResult.FileId;
          isUploadComplete = saveFileResult.IsUploadComplete;
          resourceId = saveFileResult.ResourceId;
          requestContext.Trace(14342, TraceLevel.Info, "FileService", "Service", string.Format("File {0} saved in the db with resource Id {1}", (object) fileId, (object) resourceId));
        }
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          TeamFoundationFileService.InterlockedMax(ref this.m_fileId, fileId);
        if (useRemoteProvider)
        {
          isUploadComplete = offsetFrom + (long) contentBlockLength == compressedLength;
          requestContext.Trace(14344, TraceLevel.Info, "FileService", "Service", "Putting chunk for file {0} with blob provider {1}", (object) fileId, (object) this.GetBlobProvider());
          Dictionary<string, string> blobMetadata = (Dictionary<string, string>) null;
          if (isUploadComplete)
          {
            blobMetadata = new Dictionary<string, string>();
            blobMetadata[FileServiceMetadataConstants.CompressionType] = compressionType.ToString();
            blobMetadata[FileServiceMetadataConstants.CompressedLength] = compressedLength.ToString();
            blobMetadata[FileServiceMetadataConstants.DataspaceIdentifier] = dataspaceIdentifier.ToString("D");
            blobMetadata[FileServiceMetadataConstants.FileId] = fileId.ToString();
            blobMetadata[FileServiceMetadataConstants.HashValue] = Convert.ToBase64String(hashValue);
            blobMetadata[FileServiceMetadataConstants.Length] = fileLength.ToString();
          }
          new RetryManager(3, this.m_remoteBlobPutChunkDelay).Invoke((Action) (() => this.GetBlobProvider().PutChunk(requestContext, requestContext.ServiceHost.InstanceId, resourceId.ToString("n"), contentBlock, contentBlockLength, compressedLength, offsetFrom, isUploadComplete, (IDictionary<string, string>) blobMetadata)));
        }
        if (isUploadComplete)
        {
          requestContext.Trace(14346, TraceLevel.Verbose, "FileService", "Service", "Committing file {0}", (object) fileId);
          if (TeamFoundationFileService.s_nullHash == hashValue)
            requestContext.Trace(14510, TraceLevel.Info, "FileService", "Service", "File hash was not provided by client on the final block");
          hashValue = this.ValidateUpload(requestContext, useRemoteProvider, fileId, resourceId, fileLength, md5Context, hashValue, compressionType, verifyChecksum);
          using (FileComponent fileComponent = this.CreateFileComponent(requestContext, new OwnerId?(ownerId)))
            fileComponent.CommitSingleFile(fileId, dataspaceIdentifier, hashValue, fileLength);
          requestContext.Trace(14348, TraceLevel.Verbose, "FileService", "Service", "Committed file {0}", (object) fileId);
          this.StoreContentValidationMetadata(requestContext, fileId, offsetFrom, contentBlock, contentBlockLength, ownerId, dataspaceIdentifier);
        }
        requestContext.Trace(14354, TraceLevel.Info, "FileService", "Service", "FileService UploadFile: End - Complete = {0}", (object) isUploadComplete);
        return isUploadComplete;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14356, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14358, "FileService", "Service", "UploadFile");
      }
    }

    private void StoreContentValidationMetadata(
      IVssRequestContext requestContext,
      long fileId,
      long offsetFrom,
      byte[] contentBlock,
      int contentBlockLength,
      OwnerId ownerId,
      Guid dataspaceIdentifier)
    {
      try
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.FileServiceContentValidationMetadataCollection"))
          return;
        ContentValidationScanType? scanType = new ContentValidationScanType?();
        string categoryFromOwnerId = TeamFoundationFileService.GetCategoryFromOwnerId(ownerId);
        int dataspaceId = requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, categoryFromOwnerId, dataspaceIdentifier, true).DataspaceId;
        if (offsetFrom == 0L && contentBlockLength >= MimeMapper.MinMimeDetectHeaderBytes)
          scanType = new ContentValidationScanType?(ContentValidationUtil.DetectScanTypeFromFileHeader((IReadOnlyCollection<byte>) new ArraySegment<byte>(contentBlock, 0, Math.Min(contentBlockLength, MimeMapper.SuggestedMimeDetectHeaderBytes))));
        requestContext.GetService<IFileServiceContentValidationService>().SaveMetadata(requestContext, (int) fileId, dataspaceId, requestContext.GetUserId(), requestContext is IVssWebRequestContext webRequestContext ? webRequestContext.RemoteIPAddress : (string) null, scanType: scanType);
      }
      catch (Exception ex)
      {
        requestContext.Trace(14347, TraceLevel.Error, "FileService", "Service", "Error occurred while storing content validation metadata: {0}", (object) ex);
      }
    }

    private byte[] ValidateUpload(
      IVssRequestContext requestContext,
      bool useRemoteProvider,
      long fileId,
      Guid resourceId,
      long fileLength,
      TeamFoundationServerMD5 md5Context,
      byte[] hashValue,
      CompressionType compressionType,
      bool verifyChecksum)
    {
      if (md5Context != null)
      {
        md5Context.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        if (!ArrayUtil.Equals(md5Context.Hash, hashValue) && !ArrayUtil.Equals(hashValue, TeamFoundationFileService.s_nullHash))
        {
          requestContext.Trace(14350, TraceLevel.Error, "FileService", "Service", "The Hash is wrong and not zero!");
          throw new BadChecksumException();
        }
        return md5Context.Hash;
      }
      if (useRemoteProvider)
      {
        RetryManager retryManager = new RetryManager(3, this.m_remoteBlobGetStreamDelay);
        Stream stream = (Stream) null;
        try
        {
          retryManager.Invoke((Action) (() => stream = this.GetBlobProvider().GetStream(requestContext, requestContext.ServiceHost.InstanceId, resourceId.ToString("n"))));
          return TeamFoundationFileService.ValidateLengthAndHash(requestContext, stream, fileLength, hashValue, compressionType == CompressionType.GZip);
        }
        finally
        {
          stream?.Dispose();
        }
      }
      else if (verifyChecksum)
      {
        CompressionType compressionType1;
        using (Stream stream = this.RetrievePendingFile(requestContext, fileId, false, out byte[] _, out long _, out compressionType1, out string _))
          return TeamFoundationFileService.ValidateLengthAndHash(requestContext, stream, fileLength, hashValue, compressionType1 == CompressionType.GZip);
      }
      else
      {
        requestContext.Trace(14351, TraceLevel.Info, "FileService", "Service", "Skipping MD5 check because file {0} is exactly one chunk and was already checked", (object) fileId);
        return hashValue;
      }
    }

    private Stream GetTempStream(IVssRequestContext requestContext, bool useFileStream, long size)
    {
      if (useFileStream || size > this.m_tempFileSizeThreshold - 20L)
      {
        requestContext.Trace(14372, TraceLevel.Info, "FileService", "Service", "Using a temp file because useFileStream is {0} and length is {1}", (object) useFileStream, (object) size);
        return (Stream) File.Create(TeamFoundationFileService.GetTempFileName(), TeamFoundationFileService.s_bufferSize, FileOptions.DeleteOnClose);
      }
      requestContext.Trace(14378, TraceLevel.Info, "FileService", "Service", "Using a memory stream because useFileStream is {0} and length is {1}", (object) useFileStream, (object) size);
      return (Stream) new PooledMemoryStream((int) size + 20);
    }

    private Stream CopyStreamToTempStream(
      IVssRequestContext requestContext,
      Stream stream,
      long compressedLength,
      ref CompressionType compressionType,
      bool compressOutput,
      bool useFileStream)
    {
      requestContext.TraceEnter(14370, "FileService", "Service", nameof (CopyStreamToTempStream));
      try
      {
        Stream stream1 = (Stream) null;
        if (stream == null)
          return (Stream) null;
        try
        {
          requestContext.TraceEnter(14374, "FileService", "Service", "CreateTempStream");
          stream1 = this.GetTempStream(requestContext, useFileStream, compressedLength);
          requestContext.TraceLeave(14376, "FileService", "Service", "CreateTempStream");
          if (compressOutput && compressionType != CompressionType.GZip)
          {
            requestContext.Trace(14380, TraceLevel.Verbose, "FileService", "Service", "FileService: Compressing file on server -- Compression type is not GZip while output expected is compressed");
            using (Stream destination = (Stream) new GZipStream(stream1, CompressionMode.Compress, true))
            {
              requestContext.TraceEnter(14382, "FileService", "Service", "CopyTo");
              this.CopyTo(stream, destination, (int) Math.Min(compressedLength, 2097152L));
              requestContext.TraceLeave(14384, "FileService", "Service", "CopyTo");
              compressionType = CompressionType.GZip;
            }
          }
          else if (!compressOutput && compressionType == CompressionType.GZip)
          {
            requestContext.Trace(14386, TraceLevel.Verbose, "FileService", "Service", "FileService: DeCompressing file on server -- Compression type is GZip while output expected is not compressed");
            using (Stream source = (Stream) new GZipStream(stream, CompressionMode.Decompress, true))
            {
              requestContext.TraceEnter(14388, "FileService", "Service", "CopyTo");
              this.CopyTo(source, stream1, (int) Math.Min(compressedLength, 2097152L));
              requestContext.TraceLeave(14390, "FileService", "Service", "CopyTo");
              compressionType = CompressionType.None;
            }
          }
          else
          {
            requestContext.TraceEnter(14392, "FileService", "Service", "CopyTo");
            this.CopyTo(stream, stream1, (int) Math.Min(compressedLength, 2097152L));
            requestContext.TraceLeave(14394, "FileService", "Service", "CopyTo");
          }
          stream1.Seek(0L, SeekOrigin.Begin);
          Stream tempStream = stream1;
          stream1 = (Stream) null;
          return tempStream;
        }
        finally
        {
          stream1?.Dispose();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14398, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14399, "FileService", "Service", nameof (CopyStreamToTempStream));
      }
    }

    public FileStream CopyStreamToTempFile(
      IVssRequestContext requestContext,
      Stream stream,
      ref CompressionType compressionType,
      bool compressOutput)
    {
      return (FileStream) this.CopyStreamToTempStream(requestContext, stream, 0L, ref compressionType, compressOutput, true);
    }

    internal Guid GetContainerHash(IVssRequestContext requestContext)
    {
      MD5 md5 = MD5.Create();
      byte[] inputBuffer = Array.Empty<byte>();
      if (this.GetBlobProvider() == null)
        return Guid.Empty;
      int num = 3;
      IEnumerable<string> strings = (IEnumerable<string>) null;
      while (num > 0)
      {
        try
        {
          strings = this.GetBlobProvider().EnumerateBlobs(requestContext, requestContext.ServiceHost.InstanceId);
          break;
        }
        catch (Exception ex) when (ex is TimeoutException || ex is ThreadAbortException)
        {
          if (--num <= 0)
          {
            requestContext.TraceException(14401, "FileService", "Service", ex);
            throw;
          }
          else
            requestContext.Trace(14411, TraceLevel.Info, "FileService", "Service", string.Format("There was Azure Timeout while enumerating container. Running retry. Retries remaining {0}", (object) num));
        }
      }
      foreach (string path in strings)
      {
        Guid guid = Guid.Parse(Path.GetFileNameWithoutExtension(path));
        md5.TransformBlock(guid.ToByteArray(), 0, 16, (byte[]) null, 0);
      }
      md5.TransformFinalBlock(inputBuffer, 0, 0);
      return new Guid(md5.Hash);
    }

    internal bool IsContainerExists(IVssRequestContext requestContext) => this.GetBlobProvider() != null && this.GetBlobProvider().ContainerExists(requestContext, requestContext.ServiceHost.InstanceId);

    internal IEnumerable<string> EnumerateBlobs(IVssRequestContext requestContext) => this.GetBlobProvider() != null ? this.GetBlobProvider().EnumerateBlobs(requestContext, requestContext.ServiceHost.InstanceId) : (IEnumerable<string>) new List<string>();

    internal static void CalculateSharesOfFiles(
      IEnumerable<DayOfWeek> forbiddenDays,
      DateTime currentDate,
      CleanupRecord record,
      out Decimal start,
      out Decimal end)
    {
      start = 0M;
      end = 0M;
      if (record.ForbiddenDays.Count<DayOfWeek>() != 7)
      {
        int num = (int) (record.Occured.DayOfWeek + 1 - record.ForbiddenDays.Where<DayOfWeek>((Func<DayOfWeek, bool>) (item => item < currentDate.ToUniversalTime().DayOfWeek)).Count<DayOfWeek>());
        start = (Decimal) num / (Decimal) (7 - record.ForbiddenDays.Count<DayOfWeek>()) * 100M;
      }
      if (forbiddenDays.Count<DayOfWeek>() == 7)
        return;
      int num1 = forbiddenDays.Where<DayOfWeek>((Func<DayOfWeek, bool>) (item => item > currentDate.ToUniversalTime().DayOfWeek)).Count<DayOfWeek>();
      int num2 = (int) (6 - currentDate.ToUniversalTime().DayOfWeek - num1);
      end = (1M - (Decimal) num2 / (Decimal) (7 - forbiddenDays.Count<DayOfWeek>())) * 100M;
    }

    private static IEnumerable<DayOfWeek> ParseDayOfWeekKey(
      IVssRequestContext requestContext,
      string keytoParse)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(keytoParse))
          return Enumerable.Empty<DayOfWeek>();
        return (IEnumerable<DayOfWeek>) ((IEnumerable<string>) keytoParse.Split(',')).Where<string>((Func<string, bool>) (item => !string.IsNullOrWhiteSpace(item))).ToList<string>().ConvertAll<DayOfWeek>((Converter<string, DayOfWeek>) (item => (DayOfWeek) Enum.Parse(typeof (DayOfWeek), item)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14550, TraceLevel.Error, "FileService", "Service", ex);
        return Enumerable.Empty<DayOfWeek>();
      }
    }

    private static void RecordCleanup(
      IVssRequestContext requestContext,
      IEnumerable<DayOfWeek> forbidden)
    {
      string str = new CleanupRecord()
      {
        Occured = DateTime.UtcNow,
        ForbiddenDays = forbidden
      }.Serialize<CleanupRecord>(true);
      requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, "/Service/FileService/LastCleanup", str);
    }

    private static byte[] ValidateLengthAndHash(
      IVssRequestContext requestContext,
      Stream stream,
      long suppliedLength,
      byte[] suppliedHash,
      bool isCompressed)
    {
      requestContext.TraceEnter(14400, "FileService", "Service", nameof (ValidateLengthAndHash));
      try
      {
        long num = 0;
        ArgumentUtility.CheckForNull<byte[]>(suppliedHash, nameof (suppliedHash));
        byte[] numArray = suppliedHash;
        if (suppliedLength == 0L && (stream == null || stream.Length == 0L))
        {
          requestContext.Trace(14402, TraceLevel.Warning, "FileService", "Service", "File is empty or null - returning null hash");
          return TeamFoundationFileService.s_emptyHash;
        }
        if (isCompressed)
          stream = (Stream) new GZipStream(stream, CompressionMode.Decompress);
        using (MD5 md5Provider = MD5Util.TryCreateMD5Provider())
        {
          using (ByteArray byteArray = new ByteArray((int) Math.Min(1048576L, suppliedLength)))
          {
            byte[] bytes = byteArray.Bytes;
            int inputCount;
            while ((inputCount = stream.Read(bytes, 0, bytes.Length)) > 0)
            {
              num += (long) inputCount;
              if (num > suppliedLength)
              {
                requestContext.Trace(14403, TraceLevel.Error, "FileService", "Service", string.Format("File is larger than client claimed - breaking out of hash check loop. SuppliedLength: {0} and we read at least: {1}", (object) suppliedLength, (object) num));
                break;
              }
              md5Provider?.TransformBlock(bytes, 0, inputCount, bytes, 0);
            }
            if (num != suppliedLength)
            {
              requestContext.Trace(14404, TraceLevel.Error, "FileService", "Service", string.Format("Length provided by the client does not match stream length. SuppliedLength: {0} and we read at least: {1}", (object) suppliedLength, (object) num));
              throw new IncorrectSizeException();
            }
            if (md5Provider != null)
            {
              md5Provider.TransformFinalBlock(bytes, 0, 0);
              if (!ArrayUtil.Equals(md5Provider.Hash, suppliedHash))
              {
                if (ArrayUtil.Equals(suppliedHash, TeamFoundationFileService.s_nullHash))
                {
                  requestContext.Trace(14405, TraceLevel.Warning, "FileService", "Service", "Client provided null hash - resetting to actual hash");
                  numArray = md5Provider.Hash;
                }
                else
                {
                  requestContext.Trace(14406, TraceLevel.Error, "FileService", "Service", "The Hash is wrong and not zero!");
                  throw new BadChecksumException();
                }
              }
            }
            else
              requestContext.Trace(14407, TraceLevel.Warning, "FileService", "Service", "MD5 provider is disabled - unable to verify checksum");
          }
        }
        return numArray;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14408, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14409, "FileService", "Service", nameof (ValidateLengthAndHash));
      }
    }

    private void DeletePendingDelta(
      IVssRequestContext requestContext,
      FileIdentifier newFileId,
      FileIdentifier oldFileId)
    {
      requestContext.TraceEnter(14420, "FileService", "Service", nameof (DeletePendingDelta));
      try
      {
        using (FileComponent component = requestContext.CreateComponent<FileComponent>())
          component.DeletePendingDelta(newFileId, oldFileId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14428, "FileService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(14429, "FileService", "Service", nameof (DeletePendingDelta));
      }
    }

    private static string GetTempFileName() => Path.Combine(FileSpec.GetTempDirectory(), Guid.NewGuid().ToString("N"));

    private FileComponent CreateFileComponent(IVssRequestContext requestContext, OwnerId? ownerId)
    {
      string databaseCategory = ownerId.HasValue ? TeamFoundationFileService.s_dataspaceCategoryLookup[(int) ownerId.Value] : "Default";
      return requestContext.CreateComponent<FileComponent>(databaseCategory);
    }

    private void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      if (!itemNames.Any<StrongBoxItemName>((Func<StrongBoxItemName, bool>) (i => !i.LookupKey.EndsWith("-previous"))))
        return;
      this.InitializeBlobProviders(requestContext);
    }

    private IBlobProvider GetBlobProvider()
    {
      if (this.m_deploymentService == null)
        return (IBlobProvider) null;
      if (this.m_deploymentService.m_blobProviders == null)
        return (IBlobProvider) null;
      if (this.m_storageAccountId < 0)
        throw new ArgumentException("Invalid StorageAccountId! Should be greater or equal than 0");
      return this.m_deploymentService.m_blobProviders[this.m_storageAccountId];
    }

    internal static string GetCategoryFromOwnerId(OwnerId ownerId) => TeamFoundationFileService.s_dataspaceCategoryLookup[(int) ownerId];

    internal static OwnerId GetOwnerIdFromCategory(string category)
    {
      switch (category)
      {
        case "VersionControl":
          return OwnerId.VersionControl;
        case "WorkItem":
          return OwnerId.WorkItemTracking;
        case "Build":
          return OwnerId.TeamBuild;
        default:
          return OwnerId.Generic;
      }
    }

    internal static long GetFileIdFromRegistry(IVssRequestContext requestContext)
    {
      try
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(Path.Combine("Software\\Microsoft\\TeamFoundationServer\\19.0", "FileId")))
        {
          switch (registryKey?.GetValue(requestContext.ServiceHost.InstanceId.ToString("n", (IFormatProvider) CultureInfo.InvariantCulture)))
          {
            case int fileIdFromRegistry1:
              return (long) fileIdFromRegistry1;
            case long fileIdFromRegistry2:
              return fileIdFromRegistry2;
          }
        }
      }
      catch (SecurityException ex)
      {
        requestContext.TraceException(14443, "FileService", "Service", (Exception) ex);
      }
      return 1024;
    }

    internal static bool TryGetBlobProviderInfo(
      IVssRequestContext requestContext,
      out string remoteBlobProviderType,
      out string remoteBlobProviderAssembly)
    {
      string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) FrameworkServerConstants.FileServiceRemoteBlobProvider, string.Empty);
      if (!string.IsNullOrEmpty(str))
      {
        string[] array = ((IEnumerable<string>) str.Split(',')).Select<string, string>((Func<string, string>) (part => part.Trim())).ToArray<string>();
        remoteBlobProviderType = array[0];
        remoteBlobProviderAssembly = array.Length >= 2 ? array[1] : (string) null;
        return true;
      }
      remoteBlobProviderType = (string) null;
      remoteBlobProviderAssembly = (string) null;
      return false;
    }

    internal static bool TryCreateBlobProvider<T>(
      IVssRequestContext requestContext,
      out T blobProvider)
      where T : class, IBlobProvider
    {
      string remoteBlobProviderType;
      string remoteBlobProviderAssembly;
      if (TeamFoundationFileService.TryGetBlobProviderInfo(requestContext, out remoteBlobProviderType, out remoteBlobProviderAssembly))
      {
        blobProvider = TeamFoundationFileService.CreateBlobProvider<T>(requestContext, remoteBlobProviderType, remoteBlobProviderAssembly);
        return (object) blobProvider != null;
      }
      blobProvider = default (T);
      return false;
    }

    internal static T CreateBlobProvider<T>(
      IVssRequestContext requestContext,
      string blobProviderType,
      string blobProviderAssembly)
      where T : class, IBlobProvider
    {
      blobProvider = default (T);
      string typeName = blobProviderType + "," + blobProviderAssembly;
      Type type1 = Type.GetType(typeName);
      if (type1 != (Type) null)
      {
        if (!(Activator.CreateInstance(type1) is T blobProvider))
          requestContext.Trace(14449, TraceLevel.Warning, "FileService", "Service", "Type '" + typeName + "' is not assignable to " + typeof (T).FullName);
      }
      else
        requestContext.Trace(14448, TraceLevel.Warning, "FileService", "Service", "Could not find type: " + typeName);
      if ((object) blobProvider == null)
      {
        blobProvider = (T) requestContext.GetExtension<IBlobProvider>((Func<IBlobProvider, bool>) (x =>
        {
          Type type2 = x.GetType();
          if (!typeof (T).IsAssignableFrom(type2) || !type2.FullName.Equals(blobProviderType, StringComparison.Ordinal))
            return false;
          return string.IsNullOrEmpty(blobProviderAssembly) || blobProviderAssembly.Equals(type2.Assembly.GetName().Name, StringComparison.Ordinal);
        }));
        if ((object) blobProvider == null)
          requestContext.Trace(14450, TraceLevel.Warning, "FileService", "Service", "Could not find type IBlobProvider. Type: " + typeName);
      }
      return blobProvider;
    }

    private void WriteFileIdToRegistry(IVssRequestContext requestContext, int timeoutDelayInSecond)
    {
      string name = "Global\\MS.TF.WriteFileIdToRegistry-" + requestContext.ServiceHost.InstanceId.ToString("n", (IFormatProvider) CultureInfo.InvariantCulture);
      Mutex mutex = (Mutex) null;
      bool flag = false;
      try
      {
        MutexSecurity mutexSecurity = new MutexSecurity();
        mutexSecurity.AddAccessRule(new MutexAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.WorldSid, (SecurityIdentifier) null), MutexRights.FullControl, AccessControlType.Allow));
        bool createdNew;
        mutex = new Mutex(false, name, out createdNew, mutexSecurity);
        try
        {
          flag = mutex.WaitOne(1000 * timeoutDelayInSecond);
        }
        catch (AbandonedMutexException ex)
        {
          requestContext.TraceException(14440, "FileService", "Service", (Exception) ex);
          flag = true;
        }
        if (flag)
        {
          if (this.m_fileId <= TeamFoundationFileService.GetFileIdFromRegistry(requestContext))
            return;
          try
          {
            using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey(Path.Combine("Software\\Microsoft\\TeamFoundationServer\\19.0", "FileId"), true))
              subKey.SetValue(requestContext.ServiceHost.InstanceId.ToString("n", (IFormatProvider) CultureInfo.InvariantCulture), (object) this.m_fileId, RegistryValueKind.QWord);
          }
          catch (SecurityException ex)
          {
            requestContext.TraceException(14444, TraceLevel.Warning, "FileService", "Service", (Exception) ex);
          }
        }
        else
          requestContext.Trace(14442, TraceLevel.Warning, "FileService", "Service", string.Format("Cannot obtain the mutex {0} in {1} seconds", (object) name, (object) timeoutDelayInSecond));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14447, TraceLevel.Error, "FileService", "Service", ex);
      }
      finally
      {
        if (flag)
          mutex.ReleaseMutex();
        mutex?.Dispose();
      }
    }

    private static void InterlockedMax(ref long location, long newValue)
    {
      long comparand;
      do
      {
        comparand = location;
      }
      while (comparand < newValue && Interlocked.CompareExchange(ref location, newValue, comparand) != comparand);
    }

    public void CheckFiles(
      IVssRequestContext requestContext,
      DateTime startDate,
      DateTime endDate,
      out CheckFileStats stats,
      bool forceRestart = false,
      int batchSize = 1000)
    {
      long lastFileId = (long) int.MaxValue;
      long num1 = 0;
      long num2 = 0;
      long num3 = 0;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (!forceRestart)
        lastFileId = service.GetValue<long>(requestContext, (RegistryQuery) "/Service/FileService/StartFileIdCorruptionCheck", (long) int.MaxValue);
      List<FileIdentifier> source = new List<FileIdentifier>();
      try
      {
        do
        {
          using (FileComponent component = requestContext.CreateComponent<FileComponent>())
          {
            using (ResultCollection resultCollection = component.QueryAllFilesInInterval(lastFileId, batchSize, startDate, endDate))
            {
              source = resultCollection.GetCurrent<FileIdentifier>().Items;
              lastFileId = source.Count <= 0 ? -1L : source.LastOrDefault<FileIdentifier>().FileId - 1L;
            }
          }
          foreach (FileIdentifier file in source)
          {
            bool wasFixed;
            bool flag = this.CheckFile(requestContext, file, out wasFixed);
            ++num1;
            num2 += (long) !flag;
            num3 += (long) wasFixed;
          }
          service.SetValue<long>(requestContext, "/Service/FileService/StartFileIdCorruptionCheck", lastFileId);
        }
        while (lastFileId > 1024L);
        service.DeleteEntries(requestContext, "/Service/FileService/StartFileIdCorruptionCheck");
      }
      finally
      {
        stats = new CheckFileStats()
        {
          NumberOfFiles = num1,
          NumberOfFilesWithIssue = num2,
          NumberOfFilesFixed = num3
        };
        string format = string.Format("We touched {0} files. {1} were fixed. {2} are still corrupted.", (object) num1, (object) num3, (object) num2);
        requestContext.TraceAlways(14446, TraceLevel.Info, "FileService", "Service", format);
      }
    }

    public bool CheckFile(
      IVssRequestContext requestContext,
      FileIdentifier file,
      out bool wasFixed)
    {
      wasFixed = false;
      try
      {
        byte[] hashValue;
        long contentLength;
        using (Stream stream = this.RetrieveFile(requestContext, file, false, out hashValue, out contentLength, out CompressionType _, out string _, true))
          TeamFoundationFileService.ValidateLengthAndHash(requestContext, stream, contentLength, hashValue, false);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.Trace(14445, TraceLevel.Error, "FileService", "Service", string.Format("The file {0} has an issue", (object) file) + ex.Message);
        switch (ex)
        {
          case BadChecksumException _:
          case IncorrectSizeException _:
          case Win32Exception _:
            if (this.FixDeltaCorruptionFromAzureFullFile(requestContext, file))
            {
              wasFixed = true;
              return true;
            }
            break;
        }
        return false;
      }
    }

    public FileContainerCleanupStats CleanupDeletedFileContentByParts(
      IVssRequestContext requestContext)
    {
      bool useSecondaryFileIdRange = requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.FileService.FileIdSecondaryRange");
      bool flag1 = requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.FileService.CleanupByPartsContainers");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int containerSelectBatchSize = service.GetValue<int>(requestContext, (RegistryQuery) "Service/TeamFoundationFileService/SelectBatchSize", 4000);
      int filesSelectBatchSize = service.GetValue<int>(requestContext, (RegistryQuery) "Service/TeamFoundationFileService/SelectBatchSize", 1000);
      int deleteBatchSize = service.GetValue<int>(requestContext, (RegistryQuery) "Service/TeamFoundationFileService/CleanupBatchSize", 500);
      requestContext.Trace(14581, TraceLevel.Info, "FileService", "Service", "Initiating container cleanup.");
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      bool flag2 = false;
      if (deleteBatchSize < 1)
      {
        requestContext.Trace(14551, TraceLevel.Error, "FileService", "Service", string.Format("Delete batch is set to 0 which makes batch job impossible. Setting batch to {0}", (object) 500));
        deleteBatchSize = 500;
      }
      ulong lastRowVersion = 0;
      Stopwatch stopwatch = Stopwatch.StartNew();
      List<FileCleanupSegmentedContainerStats> source = new List<FileCleanupSegmentedContainerStats>();
      int num6 = 0;
      requestContext.Trace(14579, TraceLevel.Info, "FileService", "Service", string.Format("FileCleanupContainersAsync params useSecondaryFileIdRange were:{0}, deleteBatchSize:{1}, selectBatchSize:{2}", (object) useSecondaryFileIdRange, (object) deleteBatchSize, (object) containerSelectBatchSize));
      while (!flag2 && CheckRegistry(service))
      {
        ++num6;
        FileCleanupSegmentedContainerStats segmentedContainerStats = (FileCleanupSegmentedContainerStats) null;
        if (flag1)
        {
          using (FileContainerComponent fileContainerComponent = requestContext.CreateComponent<FileContainerComponent>())
          {
            segmentedContainerStats = requestContext.RunSynchronously<FileCleanupSegmentedContainerStats>((Func<Task<FileCleanupSegmentedContainerStats>>) (() => fileContainerComponent.FileCleanupContainersAsync(useSecondaryFileIdRange, deleteBatchSize, containerSelectBatchSize, lastRowVersion)));
            lastRowVersion = lastRowVersion == 0UL ? segmentedContainerStats.LastRowVersion : lastRowVersion;
            flag2 = segmentedContainerStats.Containers < deleteBatchSize && segmentedContainerStats.ContainerItems < deleteBatchSize || segmentedContainerStats.Containers <= 0;
            if (((segmentedContainerStats.ContainerItems == -1 ? 0 : (stopwatch.Elapsed > this.c_logInterval ? 1 : 0)) | (flag2 ? 1 : 0)) != 0)
            {
              long num7 = (long) source.Sum<FileCleanupSegmentedContainerStats>((Func<FileCleanupSegmentedContainerStats, int>) (item => item.Containers));
              long num8 = (long) source.Sum<FileCleanupSegmentedContainerStats>((Func<FileCleanupSegmentedContainerStats, int>) (item => item.ContainerItems));
              requestContext.Trace(14584, TraceLevel.Info, "FileService", "Service", string.Format("containerTask, last {0} runs lasted {1}, deleting {2}", (object) num6, (object) stopwatch.Elapsed, (object) num8) + string.Format(" items and {0} containers, resetting it. LastRowVersion was {1}", (object) num7, (object) lastRowVersion));
              stopwatch.Restart();
              num6 = 0;
              source.Clear();
            }
          }
        }
        if (segmentedContainerStats == null || segmentedContainerStats.ContainerItems == -1)
        {
          requestContext.Trace(14578, TraceLevel.Info, "FileService", "Service", string.Format("Starting FileCleanupContainersAlternateAsync with params useSecondaryFileIdRange:{0}, deleteBatchSize:{1}, selectBatchSize:{2}", (object) useSecondaryFileIdRange, (object) deleteBatchSize, (object) containerSelectBatchSize));
          using (FileContainerComponent fileContainerComponent = requestContext.CreateComponent<FileContainerComponent>())
          {
            segmentedContainerStats = requestContext.RunSynchronously<FileCleanupSegmentedContainerStats>((Func<Task<FileCleanupSegmentedContainerStats>>) (() => fileContainerComponent.FileCleanupContainersAlternateAsync(useSecondaryFileIdRange, deleteBatchSize, containerSelectBatchSize)));
            source.Add(segmentedContainerStats);
            flag2 = segmentedContainerStats.Containers < deleteBatchSize && segmentedContainerStats.ContainerItems < deleteBatchSize || segmentedContainerStats.Containers <= 0;
            if (stopwatch.Elapsed > this.c_logInterval | flag2)
            {
              long num9 = (long) source.Sum<FileCleanupSegmentedContainerStats>((Func<FileCleanupSegmentedContainerStats, int>) (item => item.Containers));
              long num10 = (long) source.Sum<FileCleanupSegmentedContainerStats>((Func<FileCleanupSegmentedContainerStats, int>) (item => item.ContainerItems));
              requestContext.Trace(14582, TraceLevel.Info, "FileService", "Service", string.Format("containerTask, last {0} runs lasted {1}, deleting {2} ", (object) num6, (object) stopwatch.Elapsed, (object) num10) + string.Format("items and {0} containers.", (object) num9));
              stopwatch.Restart();
              num6 = 0;
              source.Clear();
            }
          }
        }
        num2 += segmentedContainerStats.Containers;
        num3 += segmentedContainerStats.ContainerItems;
      }
      requestContext.Trace(14585, TraceLevel.Info, "FileService", "Service", string.Format("containerTask Reached END, last cycle deleting {0} items.", (object) num1));
      bool flag3 = false;
      int? nullable1 = new int?();
      int? nullable2 = new int?();
      int num11 = 0;
      long num12 = 0;
      requestContext.Trace(14858, TraceLevel.Info, "FileService", "Service", string.Format("Starting FileCleanupFilesWatermarkedAsync with params useSecondaryFileIdRange:{0}, selectBatchSize:{1}", (object) useSecondaryFileIdRange, (object) filesSelectBatchSize));
      stopwatch.Restart();
      while (!flag3 && CheckRegistry(service))
      {
        ++num11;
        num1 = -1;
        int? fileIdParameter = nullable1;
        int? dataspaceIdParameter = nullable2;
        FileCleanupSegmentedFileStats segmentedFileStats;
        using (FileContainerComponent fileContainerComponent = requestContext.CreateComponent<FileContainerComponent>())
        {
          segmentedFileStats = requestContext.RunSynchronously<FileCleanupSegmentedFileStats>((Func<Task<FileCleanupSegmentedFileStats>>) (() => fileContainerComponent.FileCleanupFilesWatermarkedAsync(useSecondaryFileIdRange, filesSelectBatchSize, previousFileId: fileIdParameter, previousDataspaceId: dataspaceIdParameter)));
          num12 += (long) segmentedFileStats.TotalDeleted;
          num1 = segmentedFileStats.TotalDeleted;
          nullable1 = new int?(segmentedFileStats.LastFileId);
          nullable2 = new int?(segmentedFileStats.LastDataspaceId);
          flag3 = num1 < deleteBatchSize || num1 <= 0;
        }
        if (stopwatch.Elapsed > this.c_logInterval | flag3)
        {
          requestContext.Trace(14589, TraceLevel.Info, "FileService", "Service", string.Format("Files cleanup WITH watermark just finished.Input parameters, fileId:{0}, dataspaceId:{1}, LastFileId: {2},", (object) fileIdParameter, (object) dataspaceIdParameter, (object) segmentedFileStats.LastFileId) + string.Format(" LastDataspaceId: {0}, this run deleted: {1}, last period lasted {2}, it had {3} runs, deleting {4} files", (object) segmentedFileStats.LastDataspaceId, (object) segmentedFileStats.TotalDeleted, (object) stopwatch.Elapsed, (object) num11, (object) num12));
          stopwatch.Restart();
          num11 = 0;
          num12 = 0L;
        }
        num4 += num1;
      }
      requestContext.Trace(14588, TraceLevel.Info, "FileService", "Service", string.Format("fileRefferenceTask Reached END, last cycle deleting {0} items.", (object) num1));
      bool flag4 = false;
      DateTime dateTime = SqlDateTime.MinValue.Value;
      int num13 = 0;
      stopwatch.Restart();
      List<FileCleanupSegmentedArtifactStats> segmentedArtifactStatsList = new List<FileCleanupSegmentedArtifactStats>();
      long num14 = 0;
      requestContext.Trace(14591, TraceLevel.Info, "FileService", "Service", string.Format("Starting FileCleanupArtifactsAsync with params deleteBatchSize:{0}", (object) deleteBatchSize));
      while (!flag4 && CheckRegistry(service))
      {
        ++num13;
        num1 = -1;
        FileCleanupSegmentedArtifactStats segmentedArtifactStats = (FileCleanupSegmentedArtifactStats) null;
        DateTime artifactParameter = dateTime;
        using (FileContainerComponent fileContainerComponent = requestContext.CreateComponent<FileContainerComponent>())
        {
          segmentedArtifactStats = requestContext.RunSynchronously<FileCleanupSegmentedArtifactStats>((Func<Task<FileCleanupSegmentedArtifactStats>>) (() => fileContainerComponent.FileCleanupArtifactsWatermarkedIndexedAsync(deleteBatchSize, artifactParameter)));
          num1 = segmentedArtifactStats.TotalDeleted;
          num14 += (long) segmentedArtifactStats.TotalDeleted;
          int year = segmentedArtifactStats.ArtifactCreationDate.Year;
          DateTime artifactCreationDate1 = segmentedArtifactStats.ArtifactCreationDate;
          int month = artifactCreationDate1.Month;
          artifactCreationDate1 = segmentedArtifactStats.ArtifactCreationDate;
          int day = artifactCreationDate1.Day;
          DateTime artifactCreationDate2 = segmentedArtifactStats.ArtifactCreationDate;
          int hour = artifactCreationDate2.Hour;
          artifactCreationDate2 = segmentedArtifactStats.ArtifactCreationDate;
          int minute = artifactCreationDate2.Minute;
          artifactCreationDate2 = segmentedArtifactStats.ArtifactCreationDate;
          int second = artifactCreationDate2.Second;
          dateTime = new DateTime(year, month, day, hour, minute, second);
          flag4 = num1 < deleteBatchSize || num1 <= 0;
        }
        if (stopwatch.Elapsed > this.c_logInterval | flag4)
        {
          requestContext.Trace(14587, TraceLevel.Info, "FileService", "Service", string.Format("ArtifactCleanup finished, this period lasted {0} had {1} runs and deleted {2} artifacts. Input artifact parameter:{3}, LastArtifact:{4}", (object) stopwatch.Elapsed, (object) num13, (object) num14, (object) artifactParameter, (object) segmentedArtifactStats.ArtifactCreationDate));
          stopwatch.Restart();
          num13 = 0;
          num14 = 0L;
        }
        num5 += num1;
      }
      requestContext.Trace(14590, TraceLevel.Info, "FileService", "Service", string.Format("Artifact task Reached END, last cycle deleting  {0} items, resetting it.", (object) num1));
      requestContext.Trace(14592, TraceLevel.Info, "FileService", "Service", string.Format("The parts separation debugging ends, results: container: {0}, containerItems {1}, files: {2}, artifacts: {3}", (object) num2, (object) num3, (object) num4, (object) num5));
      return new FileContainerCleanupStats()
      {
        ContainerItemsDeleted = num3,
        FileReferencesDeleted = num4,
        BlobReferencesDeleted = num5
      };

      bool CheckRegistry(IVssRegistryService registryService)
      {
        containerSelectBatchSize = registryService.GetValue<int>(requestContext, (RegistryQuery) "Service/TeamFoundationFileService/SelectBatchSize", 4000);
        filesSelectBatchSize = registryService.GetValue<int>(requestContext, (RegistryQuery) "Service/TeamFoundationFileService/SelectBatchSize", 1000);
        deleteBatchSize = registryService.GetValue<int>(requestContext, (RegistryQuery) "Service/TeamFoundationFileService/CleanupBatchSize", 500);
        if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.FileService.NoCleanupDays"))
        {
          IEnumerable<DayOfWeek> daysOff = this.GetDaysOff(requestContext, registryService);
          if (daysOff.Count<DayOfWeek>() == 7)
          {
            requestContext.Trace(14595, TraceLevel.Error, "FileService", "Service", "Incorrect settings of forbidden days for FileCleanupService, job cannot run");
            return false;
          }
          if (daysOff.Contains<DayOfWeek>(DateTime.UtcNow.DayOfWeek))
          {
            requestContext.Trace(14596, TraceLevel.Info, "FileService", "Service", "Today, the cleanup job should not run.");
            return false;
          }
        }
        return true;
      }
    }

    private IEnumerable<DayOfWeek> GetDaysOff(
      IVssRequestContext requestContext,
      IVssRegistryService registryService)
    {
      Enumerable.Empty<DayOfWeek>();
      if (registryService == null)
        registryService = requestContext.GetService<IVssRegistryService>();
      string keytoParse = registryService.GetValue(requestContext, (RegistryQuery) "/Service/FileService/NoCleanupDays", string.Empty);
      return TeamFoundationFileService.ParseDayOfWeekKey(requestContext, keytoParse);
    }

    private bool FixDeltaCorruptionFromAzureFullFile(
      IVssRequestContext requestContext,
      FileIdentifier fileIdentifier)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return false;
      try
      {
        TeamFoundationFile metadata;
        using (FileComponent component = requestContext.CreateComponent<FileComponent>())
        {
          using (TeamFoundationFileSet foundationFileSet = component.RetrieveFile(fileIdentifier, false))
          {
            metadata = foundationFileSet.Metadata;
            if (metadata == null)
              return true;
            if (metadata.Metadata.ContentType != ContentType.Delta)
              return false;
          }
        }
        if (this.GetBlobProvider().BlobExists(requestContext, requestContext.ServiceHost.InstanceId, metadata.Metadata.ResourceId.ToString("n")))
        {
          long length = this.GetBlobProvider().ReadBlobProperties(requestContext, requestContext.ServiceHost.InstanceId, metadata.Metadata.ResourceId.ToString("n")).Length;
          Stream tempStream = this.GetTempStream(requestContext, false, length);
          this.GetBlobProvider().DownloadToStream(requestContext, requestContext.ServiceHost.InstanceId, metadata.Metadata.ResourceId.ToString("n"), tempStream);
          tempStream.Seek(0L, SeekOrigin.Begin);
          TeamFoundationFileService.ValidateLengthAndHash(requestContext, tempStream, metadata.Metadata.FileLength, metadata.Metadata.HashValue, metadata.Metadata.CompressionType == CompressionType.GZip);
          using (FileComponent component = requestContext.CreateComponent<FileComponent>())
            component.RevertDelta(metadata.Metadata.ResourceId, length, RemoteStoreId.AzureBlob);
          return true;
        }
      }
      catch (Exception ex) when (
      {
        // ISSUE: unable to correctly present filter
        int num;
        switch (ex)
        {
          case BadChecksumException _:
          case IncorrectSizeException _:
            num = 1;
            break;
          default:
            num = ex is Win32Exception ? 1 : 0;
            break;
        }
        if ((uint) num > 0U)
        {
          SuccessfulFiltering;
        }
        else
          throw;
      }
      )
      {
      }
      catch (Exception ex)
      {
        requestContext.TraceException(14010, "FileService", "Service", ex);
      }
      return false;
    }

    private bool GetReuseSecondaryRangeFileId(IVssRequestContext requestContext)
    {
      if (this.m_reuseSecondaryRangeFileIdStopwatch.ElapsedMilliseconds > 60000L)
      {
        this.m_reuseSecondaryRangeFileId = requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.FileService.ReuseSecondaryRangeFileId");
        this.m_reuseSecondaryRangeFileIdStopwatch.Restart();
      }
      return this.m_reuseSecondaryRangeFileId;
    }
  }
}
