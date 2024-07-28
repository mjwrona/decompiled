// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.ITempFileMetadataStoreDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public interface ITempFileMetadataStoreDataAccess
  {
    IEnumerable<TempFileMetadataRecord> GetNextBatchOfRecords(
      IVssRequestContext requestContext,
      long start,
      int count,
      string indexingUnitType,
      DocumentContractType contractType);

    IEnumerable<TempFileMetadataRecord> GetRecordsByFilePath(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      IEnumerable<FileAttributes> filePaths,
      long startingId,
      long endingId,
      string indexingUnitType,
      DocumentContractType contractType);

    void DeleteRecords(IVssRequestContext requestContext, long startingId, long endingId);

    void DeleteRecords(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      long startingId,
      long endingId);

    void DeleteRecordsExcludingSome(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      long startingId,
      long endingId,
      IEnumerable<long> idsNotToDelete);

    void GetMinAndMaxIds(IVssRequestContext requestContext, out long startingId, out long endingId);

    int AddFileMetadataRecords(
      IVssRequestContext requestContext,
      IEnumerable<TempFileMetadataRecord> records);

    void UpdateRecords(
      IVssRequestContext requestContext,
      IEnumerable<TempFileMetadataRecord> records);

    void UpdateRecordsByOverwrite(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      IEnumerable<TempFileMetadataRecord> records);

    IDictionary<IndexingUnit, int> GetNumberOfRecords(
      IVssRequestContext requestContext,
      IEnumerable<IndexingUnit> indexingUnits);

    IEnumerable<TempFileMetadataRecord> GetFilesWithMinAttemptCount(
      IVssRequestContext requestContext,
      short minAttemptCount,
      string indexingUnitType,
      DocumentContractType contractType);

    void DeleteTempFileMetadataRecords(
      IVssRequestContext requestContext,
      IEnumerable<long> idsToDelete);

    void DeleteBranchInfoInRecords(IVssRequestContext requestContext, string branch);
  }
}
