// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FileContainerComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[23]
    {
      (IComponentCreator) new ComponentCreator<FileContainerComponent>(1, true),
      (IComponentCreator) new ComponentCreator<FileContainerComponent2>(2),
      (IComponentCreator) new ComponentCreator<FileContainerComponent3>(3),
      (IComponentCreator) new ComponentCreator<FileContainerComponent4>(4),
      (IComponentCreator) new ComponentCreator<FileContainerComponent5>(5),
      (IComponentCreator) new ComponentCreator<FileContainerComponent6>(6),
      (IComponentCreator) new ComponentCreator<FileContainerComponent7>(7),
      (IComponentCreator) new ComponentCreator<FileContainerComponent8>(8),
      (IComponentCreator) new ComponentCreator<FileContainerComponent9>(9),
      (IComponentCreator) new ComponentCreator<FileContainerComponent10>(10),
      (IComponentCreator) new ComponentCreator<FileContainerComponent11>(11),
      (IComponentCreator) new ComponentCreator<FileContainerComponent12>(12),
      (IComponentCreator) new ComponentCreator<FileContainerComponent13>(13),
      (IComponentCreator) new ComponentCreator<FileContainerComponent14>(14),
      (IComponentCreator) new ComponentCreator<FileContainerComponent15>(15),
      (IComponentCreator) new ComponentCreator<FileContainerComponent16>(16),
      (IComponentCreator) new ComponentCreator<FileContainerComponent17>(17),
      (IComponentCreator) new ComponentCreator<FileContainerComponent18>(18),
      (IComponentCreator) new ComponentCreator<FileContainerComponent19>(19),
      (IComponentCreator) new ComponentCreator<FileContainerComponent20>(20),
      (IComponentCreator) new ComponentCreator<FileContainerComponent21>(21),
      (IComponentCreator) new ComponentCreator<FileContainerComponent22>(22),
      (IComponentCreator) new ComponentCreator<FileContainerComponent23>(23)
    }, "FileContainer");
    private static readonly SqlMetaData[] typ_ContainerItemTable = new SqlMetaData[7]
    {
      new SqlMetaData("Path", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ItemType", SqlDbType.TinyInt),
      new SqlMetaData("FileLength", SqlDbType.BigInt),
      new SqlMetaData("FileHash", SqlDbType.Binary, 16L),
      new SqlMetaData("FileEncoding", SqlDbType.Int),
      new SqlMetaData("FileType", SqlDbType.Int),
      new SqlMetaData("StringValue", SqlDbType.NVarChar, 512L)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static FileContainerComponent()
    {
      FileContainerComponent.s_sqlExceptionFactories.Add(800087, new SqlExceptionFactory(typeof (ContainerItemExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ContainerItemExistsException((ContainerItemType) sqEr.ExtractInt("itemType"), DBPath.DatabaseToUserPath(sqEr.ExtractString("existingPath"), true, true)))));
      FileContainerComponent.s_sqlExceptionFactories.Add(800088, new SqlExceptionFactory(typeof (ContainerItemNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ContainerItemNotFoundException((ContainerItemType) sqEr.ExtractInt("itemType"), DBPath.DatabaseToUserPath(sqEr.ExtractString("existingPath"), true, true)))));
      FileContainerComponent.s_sqlExceptionFactories.Add(800089, new SqlExceptionFactory(typeof (ContainerItemCopyTargetChildOfSourceException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ContainerItemCopyTargetChildOfSourceException(DBPath.DatabaseToUserPath(sqEr.ExtractString("targetPath"), true, true), DBPath.DatabaseToUserPath(sqEr.ExtractString("sourcePath"), true, true)))));
      FileContainerComponent.s_sqlExceptionFactories.Add(800090, new SqlExceptionFactory(typeof (ContainerItemCopySourcePendingUploadException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ContainerItemCopySourcePendingUploadException(DBPath.DatabaseToUserPath(sqEr.ExtractString("sourcePath"), true, true)))));
      FileContainerComponent.s_sqlExceptionFactories.Add(800091, new SqlExceptionFactory(typeof (ContainerItemCopyDuplicateTargetsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ContainerItemCopyDuplicateTargetsException(DBPath.DatabaseToUserPath(sqEr.ExtractString("targetPath"), true, true)))));
      FileContainerComponent.s_sqlExceptionFactories.Add(800092, new SqlExceptionFactory(typeof (ContainerAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ContainerAlreadyExistsException(DBPath.DatabaseToUserPath(sqEr.ExtractString("artifactUri"), true, true)))));
      FileContainerComponent.s_sqlExceptionFactories.Add(800109, new SqlExceptionFactory(typeof (ContainerContentIdCollisionException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ContainerContentIdCollisionException(DBPath.DatabaseToUserPath(sqEr.ExtractString("path1"), true, true), sqEr.ExtractString("length1"), DBPath.DatabaseToUserPath(sqEr.ExtractString("path2"), true, true), sqEr.ExtractString("length2")))));
      FileContainerComponent.s_sqlExceptionFactories.Add(2628, new SqlExceptionFactory(typeof (InvalidPathException)));
      FileContainerComponent.s_sqlExceptionFactories.Add(8152, new SqlExceptionFactory(typeof (InvalidPathException)));
    }

    public FileContainerComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) FileContainerComponent.s_sqlExceptionFactories;

    protected override string TraceArea => "FileContainer";

    protected virtual SqlParameter BindContainerItemTable(
      string parameterName,
      IEnumerable<FileContainerItem> rows)
    {
      rows = rows ?? Enumerable.Empty<FileContainerItem>();
      System.Func<FileContainerItem, SqlDataRecord> selector = (System.Func<FileContainerItem, SqlDataRecord>) (item =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(FileContainerComponent.typ_ContainerItemTable);
        sqlDataRecord.SetString(0, DBPath.UserToDatabasePath(item.Path, true, true));
        sqlDataRecord.SetByte(1, (byte) item.ItemType);
        switch (item.ItemType)
        {
          case ContainerItemType.Folder:
            sqlDataRecord.SetDBNull(2);
            sqlDataRecord.SetDBNull(3);
            sqlDataRecord.SetDBNull(4);
            sqlDataRecord.SetDBNull(5);
            break;
          case ContainerItemType.File:
            sqlDataRecord.SetInt64(2, item.FileLength);
            if (item.FileHash != null)
              sqlDataRecord.SetBytes(3, 0L, item.FileHash, 0, item.FileHash.Length);
            else
              sqlDataRecord.SetDBNull(3);
            sqlDataRecord.SetInt32(4, item.FileEncoding);
            sqlDataRecord.SetInt32(5, item.FileType);
            break;
          default:
            throw new ArgumentException("ItemType");
        }
        sqlDataRecord.SetDBNull(6);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ContainerItemTable", rows.Select<FileContainerItem, SqlDataRecord>(selector));
    }

    protected virtual void BindDataspace(Guid dataspaceIdentifier)
    {
    }

    protected virtual void BindDataspace(Guid? dataspaceIdentifier)
    {
    }

    protected virtual void BindIsShallow(bool isShallow)
    {
    }

    internal virtual ContainerItemBinder GetFileContainerItemBinder() => new ContainerItemBinder();

    internal virtual ContainerBinder GetFileContainerBinder() => new ContainerBinder();

    internal virtual FileContainerCleanupStatsBinder GetFileContainerCleanupStatsBinder() => new FileContainerCleanupStatsBinder();

    internal virtual ContainerItemBlobReferenceBinder GetContainerItemBlobReferenceBinder() => new ContainerItemBlobReferenceBinder();

    public virtual long AddContainer(
      Uri artifactUri,
      string securityToken,
      string name,
      string description,
      ContainerOptions options,
      Guid dataspaceIdentifier,
      string locatorPath)
    {
      this.TraceEnter(0, nameof (AddContainer));
      this.PrepareStoredProcedure("prc_CreateContainer");
      this.BindString("@artifactUri", artifactUri.ToString(), 128, false, SqlDbType.NVarChar);
      this.BindString("@securityToken", securityToken, -1, false, SqlDbType.NVarChar);
      this.BindString("@name", name, 260, false, SqlDbType.NVarChar);
      this.BindString("@description", description, 2048, true, SqlDbType.NVarChar);
      this.BindByte("@options", (byte) options);
      this.BindNullableGuid("@signingKeyId", Guid.Empty);
      this.BindGuid("@createdBy", this.Author);
      this.BindDataspace(dataspaceIdentifier);
      long int64 = Convert.ToInt64(this.ExecuteScalar(), (IFormatProvider) CultureInfo.InvariantCulture);
      this.TraceLeave(0, nameof (AddContainer));
      return int64;
    }

    public void DeleteContainer(long containerId, Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (DeleteContainer));
      this.PrepareStoredProcedure("prc_DeleteContainer");
      this.BindLong("@containerId", containerId);
      this.BindDataspace(dataspaceIdentifier);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteContainer));
    }

    public virtual void DeleteContainers(IList<long> containerIds, Guid dataspaceIdentifier)
    {
      foreach (long containerId in (IEnumerable<long>) containerIds)
        this.DeleteContainer(containerId, new Guid?(dataspaceIdentifier));
    }

    public virtual Microsoft.VisualStudio.Services.FileContainer.FileContainer GetContainer(
      long containerId,
      Guid? dataspaceIdentifier,
      bool returnSize = false)
    {
      this.TraceEnter(0, nameof (GetContainer));
      this.PrepareStoredProcedure("prc_GetContainer");
      this.BindLong("@containerId", containerId);
      this.BindBoolean("@returnSize", returnSize);
      this.BindDataspace(dataspaceIdentifier);
      Microsoft.VisualStudio.Services.FileContainer.FileContainer container;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.FileContainer.FileContainer>((ObjectBinder<Microsoft.VisualStudio.Services.FileContainer.FileContainer>) this.GetFileContainerBinder());
        container = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.FileContainer.FileContainer>().Items.FirstOrDefault<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
      }
      this.TraceLeave(0, nameof (GetContainer));
      return container;
    }

    public List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> QueryContainers(
      IList<Uri> artifactUris,
      Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (QueryContainers));
      this.PrepareStoredProcedure("prc_QueryContainers");
      this.BindStringTable("@artifactUriTable", artifactUris.Select<Uri, string>((System.Func<Uri, string>) (x => x.ToString())));
      this.BindDataspace(dataspaceIdentifier);
      List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.FileContainer.FileContainer>((ObjectBinder<Microsoft.VisualStudio.Services.FileContainer.FileContainer>) this.GetFileContainerBinder());
        items = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.FileContainer.FileContainer>().Items;
      }
      this.TraceLeave(0, "prc_QueryContainers");
      return items;
    }

    public virtual List<FileContainerItem> CreateItems(
      long containerId,
      IList<FileContainerItem> items,
      Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (CreateItems));
      this.PrepareStoredProcedure("prc_AddContainerItems");
      this.BindLong("@containerId", containerId);
      this.BindContainerItemTable("@itemsTable", (IEnumerable<FileContainerItem>) items);
      this.BindGuid("@createdBy", this.Author);
      this.BindDataspace(dataspaceIdentifier);
      List<FileContainerItem> items1;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FileContainerItem>((ObjectBinder<FileContainerItem>) this.GetFileContainerItemBinder());
        items1 = resultCollection.GetCurrent<FileContainerItem>().Items;
      }
      this.TraceLeave(0, nameof (CreateItems));
      return items1;
    }

    public virtual List<FileContainerItem> QueryItems(
      long containerId,
      string path,
      Guid? dataspaceIdentifier,
      bool isShallow = false)
    {
      return this.QueryItems((ObjectBinder<FileContainerItem>) this.GetFileContainerItemBinder(), containerId, path, dataspaceIdentifier, isShallow);
    }

    public virtual List<FileContainerItem> QueryItems(
      ObjectBinder<FileContainerItem> binder,
      long containerId,
      string path,
      Guid? dataspaceIdentifier,
      bool isShallow = false)
    {
      this.TraceEnter(0, nameof (QueryItems));
      this.PrepareStoredProcedure("prc_QueryContainerItems");
      string parameterValue = (string) null;
      if (!string.IsNullOrEmpty(path))
        parameterValue = DBPath.UserToDatabasePath(path, true, true);
      this.BindLong("@containerId", containerId);
      this.BindString("@path", parameterValue, -1, true, SqlDbType.NVarChar);
      this.BindDataspace(dataspaceIdentifier);
      this.BindIsShallow(isShallow);
      List<FileContainerItem> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FileContainerItem>(binder);
        items = resultCollection.GetCurrent<FileContainerItem>().Items;
      }
      this.TraceLeave(0, nameof (QueryItems));
      return items;
    }

    public virtual List<FileContainerItem> QuerySpecificItems(
      long containerId,
      IEnumerable<string> paths,
      Guid? dataspaceIdentifier)
    {
      return new List<FileContainerItem>();
    }

    public virtual void UpdateItemStatus(
      long containerId,
      string path,
      int fileId,
      ContainerItemStatus status,
      Guid? dataspaceIdentifier,
      long fileLength = -1,
      byte[] contentId = null,
      long? artifactId = null)
    {
      this.TraceEnter(0, nameof (UpdateItemStatus));
      if (!string.IsNullOrEmpty(path))
      {
        string databasePath = DBPath.UserToDatabasePath(path, true, true);
        this.PrepareStoredProcedure("prc_UpdateContainerItemStatus");
        this.BindLong("@containerId", containerId);
        this.BindString("@path", databasePath, -1, false, SqlDbType.NVarChar);
        this.BindInt("@fileId", fileId);
        this.BindByte("@status", (byte) status);
        this.BindGuid("@createdBy", this.Author);
        this.BindDataspace(dataspaceIdentifier);
        this.ExecuteNonQuery();
      }
      this.TraceLeave(0, nameof (UpdateItemStatus));
    }

    public void DeleteItems(long containerId, IList<string> paths, Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (DeleteItems));
      List<string> rows = new List<string>();
      foreach (string path in (IEnumerable<string>) paths)
      {
        string userPath = FileContainerItem.EnsurePathFormat(path);
        if (!string.IsNullOrWhiteSpace(userPath))
          rows.Add(DBPath.UserToDatabasePath(userPath, true, true));
      }
      if (rows.Count > 0)
      {
        this.PrepareStoredProcedure("prc_DeleteContainerItems");
        this.BindLong("@containerId", containerId);
        this.BindStringTable("@pathsTable", (IEnumerable<string>) rows);
        this.BindDataspace(dataspaceIdentifier);
        this.ExecuteNonQuery();
      }
      this.TraceLeave(0, nameof (DeleteItems));
    }

    public List<FileContainerItem> CopyFolder(
      long containerId,
      string folderSourcePath,
      string folderTargetPath,
      bool deleteSourceFolder,
      Guid dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (CopyFolder));
      this.PrepareStoredProcedure("prc_CopyContainerItemFolder");
      folderSourcePath = DBPath.UserToDatabasePath(folderSourcePath, true, true);
      folderTargetPath = DBPath.UserToDatabasePath(folderTargetPath, true, true);
      this.BindLong("@containerId", containerId);
      this.BindString("@folderSource", folderSourcePath, 400, false, SqlDbType.NVarChar);
      this.BindString("@folderTarget", folderTargetPath, 400, false, SqlDbType.NVarChar);
      this.BindGuid("@createdBy", this.Author);
      this.BindBoolean("@deleteSourceFolder", deleteSourceFolder);
      this.BindDataspace(dataspaceIdentifier);
      List<FileContainerItem> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FileContainerItem>((ObjectBinder<FileContainerItem>) this.GetFileContainerItemBinder());
        items = resultCollection.GetCurrent<FileContainerItem>().Items;
      }
      this.TraceLeave(0, nameof (CopyFolder));
      return items;
    }

    public virtual List<FileContainerItem> CopyFiles(
      long containerId,
      List<KeyValuePair<string, string>> sourcesAndTargets,
      bool deleteSources,
      bool ignoreMissingSources,
      bool overwriteTargets,
      Guid dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (CopyFiles));
      if (ignoreMissingSources | overwriteTargets)
        throw new ServiceVersionNotSupportedException(this.TraceArea, this.Version, 5);
      this.PrepareStoredProcedure("prc_CopyContainerItemFiles");
      this.BindLong("@containerId", containerId);
      this.BindKeyValuePairStringTable("@sourcesAndTargets", sourcesAndTargets.Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((System.Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (x => new KeyValuePair<string, string>(DBPath.UserToDatabasePath(x.Key, true, true), DBPath.UserToDatabasePath(x.Value, true, true)))));
      this.BindGuid("@createdBy", this.Author);
      this.BindBoolean("@deleteSources", deleteSources);
      this.BindDataspace(dataspaceIdentifier);
      List<FileContainerItem> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FileContainerItem>((ObjectBinder<FileContainerItem>) this.GetFileContainerItemBinder());
        items = resultCollection.GetCurrent<FileContainerItem>().Items;
      }
      this.TraceLeave(0, nameof (CopyFiles));
      return items;
    }

    public virtual ContainerItemBlobReference AddBlobReference(
      string artifactHash,
      BlobCompressionType artifactType,
      Guid? dataspaceIdentifier)
    {
      return (ContainerItemBlobReference) null;
    }

    public virtual ContainerItemBlobReference AddPendingBlobReference(
      Guid sessionId,
      BlobCompressionType compressionType,
      Guid? dataspaceIdentifier)
    {
      return (ContainerItemBlobReference) null;
    }

    public virtual ContainerItemBlobReference CompletePendingBlobReference(
      long artifactId,
      string artifactHash,
      Guid? dataspaceIdentifier)
    {
      return (ContainerItemBlobReference) null;
    }

    public virtual ContainerItemBlobReference GetBlobReference(
      long artifactId,
      Guid? dataspaceIdentifier)
    {
      return (ContainerItemBlobReference) null;
    }

    public virtual List<ContainerItemBlobReference> GetBlobReferences(
      IEnumerable<long> artifactIds,
      Guid? dataspaceIdentifier)
    {
      return new List<ContainerItemBlobReference>();
    }

    public virtual List<ContainerItemBlobReference> GetUnusedBlobReferences(
      int maxNumberOfFiles,
      int retentionPeriodInDays)
    {
      return new List<ContainerItemBlobReference>();
    }

    public virtual void HardDeleteBlobReferences(IEnumerable<long> artifactIds)
    {
    }

    public virtual List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> FilterContainers(
      string artifactUriFilter,
      Guid? dataspaceIdentifier)
    {
      return new List<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
    }

    public virtual List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> FilterContainers(
      ObjectBinder<Microsoft.VisualStudio.Services.FileContainer.FileContainer> overrideBinder,
      string artifactUriFilter,
      Guid? dataspaceIdentifier)
    {
      return new List<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
    }

    public virtual FileContainerCleanupStats CleanupDeletedFileContent(
      bool useSecondaryFileIdRange,
      int segmentIndex = 0,
      int segmentCount = 1,
      bool hashJoin = false,
      int batchSelectSize = 0,
      int batchDeleteSize = 0)
    {
      return new FileContainerCleanupStats();
    }

    internal virtual FileContainerCleanupStats CleanupDeletedFileContentByShares(
      bool useSecondaryFileIdRange,
      Decimal startingPoint,
      Decimal endingPoint,
      bool hashJoin = false,
      int batchSelectSize = 0,
      int batchDeleteSize = 0)
    {
      return new FileContainerCleanupStats();
    }

    internal virtual Task<FileCleanupSegmentedContainerStats> FileCleanupContainersAsync(
      bool useSecondaryFileIdRange,
      int deleteBatchSize,
      int selectBatchSize,
      ulong lastRowVersion = 0)
    {
      return Task.FromResult<FileCleanupSegmentedContainerStats>(new FileCleanupSegmentedContainerStats());
    }

    internal virtual Task<FileCleanupSegmentedStats> FileCleanupFilesAsync(
      bool useSecondaryFileIdRange,
      int selectbatchsize,
      bool cleanupHashJoin = false)
    {
      return Task.FromResult<FileCleanupSegmentedStats>(new FileCleanupSegmentedStats());
    }

    internal virtual Task<FileCleanupSegmentedStats> FileCleanupArtifactsAsync(int batchSize) => Task.FromResult<FileCleanupSegmentedStats>(new FileCleanupSegmentedStats());

    internal virtual Task<FileCleanupSegmentedArtifactStats> FileCleanupArtifactsWatermarkedAsync(
      int batchSize,
      DateTime lastArtifactDate)
    {
      return Task.FromResult<FileCleanupSegmentedArtifactStats>(new FileCleanupSegmentedArtifactStats());
    }

    internal virtual Task<FileCleanupSegmentedFileStats> FileCleanupFilesWatermarkedAsync(
      bool useSecondaryFileIdRange,
      int selectbatchsize,
      bool cleanupHashJoin = false,
      int? previousFileId = null,
      int? previousDataspaceId = null)
    {
      return Task.FromResult<FileCleanupSegmentedFileStats>(new FileCleanupSegmentedFileStats());
    }

    internal virtual Task<FileCleanupSegmentedArtifactStats> FileCleanupArtifactsWatermarkedIndexedAsync(
      int batchSize,
      DateTime lastArtifactDate)
    {
      return Task.FromResult<FileCleanupSegmentedArtifactStats>(new FileCleanupSegmentedArtifactStats());
    }

    internal virtual Task<FileCleanupSegmentedContainerStats> FileCleanupContainersAlternateAsync(
      bool useSecondaryFileIdRange,
      int deleteBatchSize,
      int selectBatchSize)
    {
      return Task.FromResult<FileCleanupSegmentedContainerStats>(new FileCleanupSegmentedContainerStats());
    }
  }
}
