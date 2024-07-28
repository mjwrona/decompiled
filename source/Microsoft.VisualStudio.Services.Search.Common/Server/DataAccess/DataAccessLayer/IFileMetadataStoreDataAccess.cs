// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.IFileMetadataStoreDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public interface IFileMetadataStoreDataAccess
  {
    IEnumerable<FileMetadataRecord> GetNextBatchOfRecords(
      IVssRequestContext requestContext,
      long start,
      int batchSize);

    IEnumerable<FileMetadataRecord> GetRecords(
      IVssRequestContext requestContext,
      IEnumerable<string> filePaths,
      DocumentContractType documentContractType);

    void AddRecords(
      IVssRequestContext requestContext,
      IEnumerable<FileMetadataRecord> records,
      DocumentContractType documentContractType);

    Tuple<List<FileMetadataRecord>, List<FileMetadataRecord>> UpdateRecords(
      IVssRequestContext requestContext,
      IEnumerable<FileMetadataRecord> recordsToUpdate,
      IEnumerable<FileMetadataRecord> recordsToDelete,
      DocumentContractType documentContractType);

    void GetMinAndMaxFilePathIds(
      IVssRequestContext requestContext,
      out long startingFilePathId,
      out long endingFilePathId);

    void GetMinAndMaxFilePathIds(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      out long startingFilePathId,
      out long endingFilePathId);

    IEnumerable<FilePathAndContentHash> LookupForDocuments(
      IVssRequestContext requestContext,
      IEnumerable<FilePathAndContentHash> recordsForLookup,
      DocumentContractType documentContractType);

    IDictionary<FilePathBranchInfo, string> LookupForContentHash(
      IVssRequestContext requestContext,
      IEnumerable<FilePathBranchInfo> recordsToLookup,
      DocumentContractType documentContractType);

    void DeleteBranchInfoInRecords(IVssRequestContext requestContext, List<string> branchName);

    void DeleteFileMetadataRecordsByRange(
      IVssRequestContext requestContext,
      long startingId,
      long endingId);

    void DeleteFileMetadataRecordsByRange(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      long startingId,
      long endingId);
  }
}
