// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL.TempFileMetadataStoreDataAccess
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
  internal class TempFileMetadataStoreDataAccess : 
    SqlAzureDataAccess,
    ITempFileMetadataStoreDataAccess
  {
    private readonly IndexingUnit m_indexingUnit;

    internal TempFileMetadataStoreDataAccess(IndexingUnit indexingUnit) => this.m_indexingUnit = indexingUnit;

    public IEnumerable<TempFileMetadataRecord> GetNextBatchOfRecords(
      IVssRequestContext requestContext,
      long start,
      int count,
      string indexingUnitType,
      DocumentContractType contractType)
    {
      using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
        return component.GetTempFileMetadataRecords(this.m_indexingUnit, start, count, indexingUnitType, contractType);
    }

    public void DeleteRecords(IVssRequestContext requestContext, long startingId, long endingId) => this.DeleteRecords(requestContext, this.m_indexingUnit, startingId, endingId);

    public void DeleteRecords(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      long startingId,
      long endingId)
    {
      long endingId1 = startingId - 1L;
      long currentHostConfigValue = (long) requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/BatchSizeForTempFileMetadataStoreCleanUp", true, 5000);
      if (currentHostConfigValue == 0L)
        throw new Exception("batchSize is 0");
      do
      {
        long startingId1 = endingId1 + 1L;
        endingId1 = startingId1 + currentHostConfigValue - 1L;
        if (endingId1 > endingId)
          endingId1 = endingId;
        using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
          component.DeleteRecords(indexingUnit, startingId1, endingId1);
      }
      while (endingId1 < endingId);
    }

    public void DeleteRecordsExcludingSome(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      long startingId,
      long endingId,
      IEnumerable<long> idsNotToDelete)
    {
      long currEndingId = startingId - 1L;
      long currentHostConfigValue = (long) requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/BatchSizeForTempFileMetadataStoreCleanUp", true, 5000);
      List<long> list1 = idsNotToDelete.ToList<long>();
      list1.Sort();
      if (currentHostConfigValue == 0L)
        throw new Exception("batchSize is 0");
      do
      {
        long startingId1 = currEndingId + 1L;
        currEndingId = startingId1 + currentHostConfigValue - 1L;
        if (currEndingId > endingId)
          currEndingId = endingId;
        List<long> list2 = list1.TakeWhile<long>((Func<long, bool>) (x => x < currEndingId + 1L)).ToList<long>();
        list1.RemoveAll((Predicate<long>) (x => x < currEndingId + 1L));
        using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
          component.DeleteRecordsExcludingSome(indexingUnit, startingId1, currEndingId, (IEnumerable<long>) list2);
      }
      while (currEndingId < endingId);
    }

    public IEnumerable<TempFileMetadataRecord> GetRecordsByFilePath(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      IEnumerable<FileAttributes> fileAttributes,
      long startingId,
      long endingId,
      string indexingUnitType,
      DocumentContractType contractType)
    {
      using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
        return component.GetRecordsByFilePath(indexingUnit, fileAttributes, startingId, endingId, indexingUnitType, contractType);
    }

    public void UpdateRecordsByOverwrite(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      IEnumerable<TempFileMetadataRecord> records)
    {
      List<TempFileMetadataRecord> list = records.ToList<TempFileMetadataRecord>();
      int num = list.Count<TempFileMetadataRecord>();
      int val1 = num;
      int currentHostConfigValue = requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/BatchSizeForTempFileMetadataStoreUpdate", true, 500);
      for (int index = 0; index < num; index += currentHostConfigValue)
      {
        int count = Math.Min(val1, currentHostConfigValue);
        IList<TempFileMetadataRecord> range = (IList<TempFileMetadataRecord>) list.GetRange(index, count);
        val1 -= count;
        using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
          component.UpdateRecordsByOverwrite(indexingUnit, this.ModifyRecordsWhileWriting((IEnumerable<TempFileMetadataRecord>) range));
        if (val1 <= 0)
          break;
      }
    }

    public void GetMinAndMaxIds(
      IVssRequestContext requestContext,
      out long startingId,
      out long endingId)
    {
      using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
        component.GetMinAndMaxIds(this.m_indexingUnit, out startingId, out endingId);
    }

    public int AddFileMetadataRecords(
      IVssRequestContext requestContext,
      IEnumerable<TempFileMetadataRecord> records)
    {
      using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
        return component.AddFileMetadataRecords(this.m_indexingUnit, this.ModifyRecordsWhileWriting(records));
    }

    public void UpdateRecords(
      IVssRequestContext requestContext,
      IEnumerable<TempFileMetadataRecord> records)
    {
      using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
        component.UpdateRecords(this.m_indexingUnit, this.ModifyRecordsWhileWriting(records));
    }

    public IDictionary<IndexingUnit, int> GetNumberOfRecords(
      IVssRequestContext requestContext,
      IEnumerable<IndexingUnit> indexingUnits)
    {
      using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
        return component.GetNumberOfRecords(indexingUnits);
    }

    public IEnumerable<TempFileMetadataRecord> GetFilesWithMinAttemptCount(
      IVssRequestContext requestContext,
      short minAttemptCount,
      string indexingUnitType,
      DocumentContractType contractType)
    {
      int configValue = requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/LargeRepositoryFileLevelIndexingExpiryTimeInHours");
      using (TempFileMetadataStoreComponent component = requestContext.CreateComponent<TempFileMetadataStoreComponent>())
        return component.GetFilesWithMinAttemptCount(this.m_indexingUnit, minAttemptCount, indexingUnitType, contractType, configValue);
    }

    public void DeleteTempFileMetadataRecords(
      IVssRequestContext requestContext,
      IEnumerable<long> idsToDelete)
    {
      using (TempFileMetadataStoreComponentV2 component = (TempFileMetadataStoreComponentV2) requestContext.CreateComponent<TempFileMetadataStoreComponent>())
        component.DeleteRecords(this.m_indexingUnit, idsToDelete);
    }

    public virtual void DeleteBranchInfoInRecords(IVssRequestContext requestContext, string branch)
    {
      using (TempFileMetadataStoreComponentV2 component = (TempFileMetadataStoreComponentV2) requestContext.CreateComponent<TempFileMetadataStoreComponent>())
        component.DeleteBranchInfoInRecords(this.m_indexingUnit.IndexingUnitId, branch);
    }

    internal IEnumerable<TempFileMetadataRecord> ModifyRecordsWhileWriting(
      IEnumerable<TempFileMetadataRecord> tempFileMetadataRecords)
    {
      if (tempFileMetadataRecords != null)
      {
        foreach (TempFileMetadataRecord fileMetadataRecord in tempFileMetadataRecords)
        {
          if (fileMetadataRecord.TemporaryBranchMetadata != null)
          {
            TemporaryBranchMetadata temporaryBranchMetadata = fileMetadataRecord.TemporaryBranchMetadata;
            if (temporaryBranchMetadata != null)
              temporaryBranchMetadata.FilePath = fileMetadataRecord.FileAttributes.OriginalFilePath;
          }
        }
      }
      return tempFileMetadataRecords;
    }
  }
}
