// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.IIndexingUnitDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public interface IIndexingUnitDataAccess
  {
    IndexingUnit AddIndexingUnit(IVssRequestContext requestContext, IndexingUnit indexingUnit);

    List<IndexingUnit> AddOrUpdateIndexingUnits(
      IVssRequestContext requestContext,
      List<IndexingUnit> indexingUnits,
      bool merge);

    IndexingUnit GetIndexingUnit(IVssRequestContext requestContext, int indexingUnitId);

    List<IndexingUnit> GetIndexingUnitsRoutingInfo(
      IVssRequestContext requestContext,
      IEntityType entityType,
      List<string> indexingUnitTypes);

    List<IndexingUnit> GetChildIndexingUnitsRoutingInfo(
      IVssRequestContext requestContext,
      string indexingUnitType,
      int parentUnitId);

    IndexingUnit GetIndexingUnit(
      IVssRequestContext requestContext,
      Guid TFSEntityId,
      string indexingUnitType,
      IEntityType entityType);

    IndexingUnit GetIndexingUnit(
      IVssRequestContext requestContext,
      Guid TFSEntityId,
      string indexingUnitType,
      IEntityType entityType,
      bool isShadow);

    List<IndexingUnit> GetIndexingUnits(
      IVssRequestContext requestContext,
      string indexingUnitType,
      IEntityType entityType,
      int topCount);

    List<IndexingUnit> GetIndexingUnits(
      IVssRequestContext requestContext,
      string indexingUnitType,
      bool isShadow,
      IEntityType entityType,
      int topCount);

    List<IndexingUnit> GetIndexingUnitsWithGivenParent(
      IVssRequestContext requestContext,
      int parentIndexingUnitId,
      int topCount);

    List<IndexingUnit> GetDeletedIndexingUnits(IVssRequestContext requestContext, int topCount);

    IndexingUnit UpdateIndexingUnit(IVssRequestContext requestContext, IndexingUnit indexingUnit);

    IndexingUnit AssociateJobId(IVssRequestContext requestContext, IndexingUnit indexingUnit);

    void PromoteShadowIndexingUnitsToPrimary(
      IVssRequestContext requestContext,
      List<string> indexingUnitTypes,
      IEntityType entityType);

    List<IndexingUnit> UpdateIndexingUnits(
      IVssRequestContext requestContext,
      List<IndexingUnit> indexingUnit);

    IndexingUnit DisassociateJobId(IVssRequestContext requestContext, IndexingUnit indexingUnit);

    void DeleteIndexingUnit(IVssRequestContext requestContext, IndexingUnit indexingUnit);

    void DeleteIndexingUnitsPermanently(
      IVssRequestContext requestContext,
      List<IndexingUnit> indexingUnitList);

    IDictionary<int, IndexingUnit> GetIndexingUnits(
      IVssRequestContext requestContext,
      IEnumerable<int> indexingUnitIds);

    void AddOrUpdateIndexingUnitInformation(
      IVssRequestContext requestContext,
      List<IndexingUnitDetails> iudList,
      List<IndexingUnit> iuList);

    void InsertIndexingUnitDetails(
      IVssRequestContext requestContext,
      List<IndexingUnitDetails> iudList);

    void UpdateIndexingUnitGrowthDetails(
      IVssRequestContext requestContext,
      int iuId,
      string lastIndexedWatermark,
      int changeInActualDocCount,
      long changeInActualSize);

    void UpdateInitialEstimates(
      IVssRequestContext requestContext,
      int indexingUnitId,
      string lastIndexedWatermark,
      int estimatedInitialDocCount,
      int actualInitialDocCount,
      int estimatedDocCountGrowth,
      long estimatedInitialSize,
      long actualInitialSize,
      long estimatedSizeGrowth);

    IndexingUnitDetails GetIndexingUnitDetails(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit);

    IDictionary<int, IndexingUnitDetails> FetchIndexingUnitDetails(
      IVssRequestContext requestContext,
      IEnumerable<int> indexingUnitIds);

    void AddOrResetIndexingUnitDetailsAndUpdateShardDetails(
      IVssRequestContext requestContext,
      IList<IndexingUnitDetails> indexingUnitIndexingInformationList,
      IList<IndexingUnit> indexingUnitList,
      IList<ShardDetails> shardEstimationsBeforeAllocation = null);

    void DeleteIndexingUnitDetailsAndUpdateShardInformation(
      IVssRequestContext requestContext,
      List<int> indexingUnitIds);

    void DeleteIndexingUnitDetails(IVssRequestContext requestContext, List<int> indexingUnitIdList);

    List<IndexingUnitDetails> FetchIndexingUnitDetails(
      IVssRequestContext requestContext,
      IEntityType entityType,
      string indexingUnitType);

    Dictionary<Guid, IndexingUnit> GetAssociatedJobIds(
      IVssRequestContext requestContext,
      List<IEntityType> entityTypes);

    List<IndexingUnit> GetIndexingUnitsOfOrphanRepos(IVssRequestContext requestContext);

    void SoftDeleteIndexingUnits(
      IVssRequestContext requestContext,
      IEnumerable<int> indexingUnitIdList);
  }
}
