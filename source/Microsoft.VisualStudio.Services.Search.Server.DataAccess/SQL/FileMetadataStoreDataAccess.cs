// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL.FileMetadataStoreDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL
{
  internal class FileMetadataStoreDataAccess : SqlAzureDataAccess, IFileMetadataStoreDataAccess
  {
    private readonly Microsoft.VisualStudio.Services.Search.Common.IndexingUnit m_indexingUnit;

    public FileMetadataStoreDataAccess(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      this.m_indexingUnit = indexingUnit;
      if (indexingUnit == null || indexingUnit.IndexingUnitType == "ScopedIndexingUnit")
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082622, "Indexing Pipeline", nameof (FileMetadataStoreDataAccess), string.Format("FileMetadataStoreDataAccess initialized with unsupported indexing unit -> {0}", (object) indexingUnit.IndexingUnitType));
    }

    public IEnumerable<FileMetadataRecord> GetNextBatchOfRecords(
      IVssRequestContext requestContext,
      long start,
      int count)
    {
      using (FileMetadataStoreComponent component = requestContext.CreateComponent<FileMetadataStoreComponent>())
        return this.ModifyRecordsWhileReading(component.GetRecords(this.m_indexingUnit, start, count));
    }

    public IEnumerable<FileMetadataRecord> GetRecords(
      IVssRequestContext requestContext,
      IEnumerable<string> filePaths,
      DocumentContractType documentContractType)
    {
      using (FileMetadataStoreComponent component = requestContext.CreateComponent<FileMetadataStoreComponent>())
        return this.ModifyRecordsWhileReading(component.GetRecords(this.m_indexingUnit, this.ModifyFilePathsBeforeLookUp(filePaths, documentContractType)));
    }

    public void AddRecords(
      IVssRequestContext requestContext,
      IEnumerable<FileMetadataRecord> records,
      DocumentContractType documentContractType)
    {
      using (FileMetadataStoreComponent component = requestContext.CreateComponent<FileMetadataStoreComponent>())
        component.AddRecords(this.m_indexingUnit, this.ModifyRecordsWhileWriting(records, documentContractType));
    }

    public void GetMinAndMaxFilePathIds(
      IVssRequestContext requestContext,
      out long startingFilePathId,
      out long endingFilePathId)
    {
      this.GetMinAndMaxFilePathIds(requestContext, this.m_indexingUnit, out startingFilePathId, out endingFilePathId);
    }

    public void GetMinAndMaxFilePathIds(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      out long startingFilePathId,
      out long endingFilePathId)
    {
      if (indexingUnit != null)
      {
        int num = indexingUnit.IndexingUnitType == "ScopedIndexingUnit" ? 1 : 0;
      }
      using (FileMetadataStoreComponent component = requestContext.CreateComponent<FileMetadataStoreComponent>())
        component.GetMinAndMaxFilePathIds(indexingUnit, out startingFilePathId, out endingFilePathId);
    }

    public IEnumerable<FilePathAndContentHash> LookupForDocuments(
      IVssRequestContext requestContext,
      IEnumerable<FilePathAndContentHash> recordsForLookup,
      DocumentContractType documentContractType)
    {
      using (FileMetadataStoreComponent component = requestContext.CreateComponent<FileMetadataStoreComponent>())
        return component.LookupForDocuments(this.m_indexingUnit, recordsForLookup, documentContractType);
    }

    public IDictionary<FilePathBranchInfo, string> LookupForContentHash(
      IVssRequestContext requestContext,
      IEnumerable<FilePathBranchInfo> recordsToLookup,
      DocumentContractType documentContractType)
    {
      using (FileMetadataStoreComponent component = requestContext.CreateComponent<FileMetadataStoreComponent>())
        return component.LookupForContentHashUsingFilePathBranch(this.m_indexingUnit, recordsToLookup, documentContractType);
    }

    public Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>> UpdateRecords(
      IVssRequestContext requestContext,
      IEnumerable<FileMetadataRecord> recordsToUpdate,
      IEnumerable<FileMetadataRecord> recordsToDelete,
      DocumentContractType documentContractType)
    {
      using (FileMetadataStoreComponent component = requestContext.CreateComponent<FileMetadataStoreComponent>())
      {
        Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>> tuple = component.UpdateRecordsUsingMerge(this.m_indexingUnit, this.ModifyRecordsWhileWriting(recordsToUpdate, documentContractType), this.ModifyRecordsWhileWriting(recordsToDelete, documentContractType));
        IEnumerable<FileMetadataRecord> source1 = this.ModifyRecordsWhileReading((IEnumerable<FileMetadataRecord>) tuple?.Item1);
        List<FileMetadataRecord> list1 = source1 != null ? source1.ToList<FileMetadataRecord>() : (List<FileMetadataRecord>) null;
        IEnumerable<FileMetadataRecord> source2 = this.ModifyRecordsWhileReading((IEnumerable<FileMetadataRecord>) tuple?.Item2);
        List<FileMetadataRecord> list2 = source2 != null ? source2.ToList<FileMetadataRecord>() : (List<FileMetadataRecord>) null;
        return new Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>>(list1, list2);
      }
    }

    public virtual void DeleteBranchInfoInRecords(
      IVssRequestContext requestContext,
      List<string> branchNames)
    {
      int configValue = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PatchInMemoryThresholdForMaxDocs");
      using (FileMetadataStoreComponent component = requestContext.CreateComponent<FileMetadataStoreComponent>())
        component.DeleteBranchInfoRecordsUsingMerge(this.m_indexingUnit, branchNames, configValue);
    }

    public void DeleteFileMetadataRecordsByRange(
      IVssRequestContext requestContext,
      long startingId,
      long endingId)
    {
      this.DeleteFileMetadataRecordsByRange(requestContext, this.m_indexingUnit, startingId, endingId);
    }

    public void DeleteFileMetadataRecordsByRange(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      long startingId,
      long endingId)
    {
      if (indexingUnit != null)
      {
        int num = indexingUnit.IndexingUnitType == "ScopedIndexingUnit" ? 1 : 0;
      }
      long endingId1 = startingId - 1L;
      long currentHostConfigValue = (long) requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/BatchSizeForFileMetadataStoreCleanUp", true, 5000);
      if (currentHostConfigValue <= 0L)
        throw new Exception("batchSize is 0");
      do
      {
        long startingId1 = endingId1 + 1L;
        endingId1 = startingId1 + currentHostConfigValue - 1L;
        if (endingId1 > endingId)
          endingId1 = endingId;
        using (FileMetadataStoreComponent component = requestContext.CreateComponent<FileMetadataStoreComponent>())
        {
          if (!(component is FileMetadataStoreComponentV2 storeComponentV2))
          {
            component.DeleteFileMetadataRecords(this.m_indexingUnit);
            break;
          }
          storeComponentV2.DeleteFileMetadataRecordsByRange(this.m_indexingUnit, startingId1, endingId1);
        }
      }
      while (endingId1 < endingId);
    }

    internal IEnumerable<FileMetadataRecord> ModifyRecordsWhileReading(
      IEnumerable<FileMetadataRecord> fileMetadataRecords)
    {
      if (fileMetadataRecords != null)
      {
        foreach (FileMetadataRecord fileMetadataRecord in fileMetadataRecords)
        {
          BranchMetadata branchMetadata = fileMetadataRecord.BranchMetadata;
          if (branchMetadata != null && !string.IsNullOrWhiteSpace(branchMetadata.FilePath))
            fileMetadataRecord.FilePath = branchMetadata.FilePath;
        }
      }
      return fileMetadataRecords;
    }

    internal IEnumerable<FileMetadataRecord> ModifyRecordsWhileWriting(
      IEnumerable<FileMetadataRecord> fileMetadataRecords,
      DocumentContractType documentContractType)
    {
      if ((documentContractType == DocumentContractType.DedupeFileContractV4 || documentContractType == DocumentContractType.DedupeFileContractV5) && fileMetadataRecords != null)
      {
        foreach (FileMetadataRecord fileMetadataRecord in fileMetadataRecords)
        {
          if (fileMetadataRecord.BranchMetadata != null)
          {
            BranchMetadata branchMetadata = fileMetadataRecord.BranchMetadata;
            if (branchMetadata != null)
              branchMetadata.FilePath = fileMetadataRecord.FilePath;
          }
          fileMetadataRecord.FilePath = FileAttributes.GetSHA2Hash(fileMetadataRecord.FilePath).HexHash;
        }
      }
      return fileMetadataRecords;
    }

    internal IEnumerable<string> ModifyFilePathsBeforeLookUp(
      IEnumerable<string> filePaths,
      DocumentContractType documentContractType)
    {
      if (documentContractType != DocumentContractType.DedupeFileContractV4 && documentContractType != DocumentContractType.DedupeFileContractV5 || filePaths == null)
        return filePaths;
      List<string> stringList = new List<string>();
      foreach (string filePath in filePaths)
        stringList.Add(FileAttributes.GetSHA2Hash(filePath).HexHash);
      return (IEnumerable<string>) stringList;
    }
  }
}
