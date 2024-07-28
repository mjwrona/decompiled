// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.IndexingUnitDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  public class IndexingUnitDataAccess : SqlAzureDataAccess, IIndexingUnitDataAccess
  {
    protected internal int BatchCount = 500;

    public IndexingUnit AddIndexingUnit(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        return component.Insert(indexingUnit);
    }

    public List<IndexingUnit> AddOrUpdateIndexingUnits(
      IVssRequestContext requestContext,
      List<IndexingUnit> indexingUnits,
      bool merge)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnit>(nameof (indexingUnits), (IList<IndexingUnit>) indexingUnits);
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        return component.AddTableEntityBatch(indexingUnits, merge);
    }

    public virtual IndexingUnit GetIndexingUnit(
      IVssRequestContext requestContext,
      int indexingUnitId)
    {
      if (indexingUnitId <= 0)
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("indexingUnitId must be a positive integer"));
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
      {
        if (component is IndexingUnitComponentV2 indexingUnitComponentV2)
          return indexingUnitComponentV2.GetIndexingUnit(indexingUnitId);
        TableEntityFilterList filterList = new TableEntityFilterList();
        filterList.Add(new TableEntityFilter("IndexingUnitId", "eq", indexingUnitId.ToString()));
        return component.RetriveTableEntity(filterList);
      }
    }

    public virtual IndexingUnit GetIndexingUnit(
      IVssRequestContext requestContext,
      Guid TFSEntityId,
      string indexingUnitType,
      IEntityType entityType)
    {
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
      {
        if (component is IndexingUnitComponentV2 indexingUnitComponentV2)
          return indexingUnitComponentV2.GetIndexingUnit(TFSEntityId, indexingUnitType, entityType);
        TableEntityFilterList filterList = new TableEntityFilterList();
        filterList.Add(new TableEntityFilter(nameof (TFSEntityId), "eq", TFSEntityId.ToString()));
        filterList.Add(new TableEntityFilter("IndexingUnitType", "eq", indexingUnitType));
        filterList.Add(new TableEntityFilter("EntityType", "eq", entityType.Name.ToString()));
        return component.RetriveTableEntity(filterList);
      }
    }

    public IndexingUnit GetIndexingUnit(
      IVssRequestContext requestContext,
      Guid TFSEntityId,
      string indexingUnitType,
      IEntityType entityType,
      bool isShadow)
    {
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        return component.GetIndexingUnit(TFSEntityId, indexingUnitType, isShadow, entityType);
    }

    public List<IndexingUnit> GetIndexingUnits(
      IVssRequestContext requestContext,
      string indexingUnitType,
      IEntityType entityType,
      int topCount)
    {
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
      {
        if (component is IndexingUnitComponentV4 indexingUnitComponentV4)
          return indexingUnitComponentV4.GetIndexingUnits(indexingUnitType, entityType, topCount);
        TableEntityFilterList filterList = new TableEntityFilterList();
        filterList.Add(new TableEntityFilter("IndexingUnitType", "eq", indexingUnitType));
        filterList.Add(new TableEntityFilter("EntityType", "eq", entityType.Name.ToString()));
        return component.RetriveTableEntityList(topCount, filterList);
      }
    }

    public List<IndexingUnit> GetIndexingUnits(
      IVssRequestContext requestContext,
      string indexingUnitType,
      bool isShadow,
      IEntityType entityType,
      int topCount)
    {
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        return component.GetIndexingUnits(indexingUnitType, isShadow, entityType, topCount);
    }

    public List<IndexingUnit> GetDeletedIndexingUnits(
      IVssRequestContext requestContext,
      int topCount)
    {
      using (IndexingUnitComponent indexingUnitComponent = requestContext.CreateComponent<IndexingUnitComponent>())
        return this.InvokeTableOperation<List<IndexingUnit>>((Func<List<IndexingUnit>>) (() => indexingUnitComponent.GetDeletedIndexingUnitList(topCount)));
    }

    public virtual IndexingUnit UpdateIndexingUnit(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      using (IndexingUnitComponent indexingUnitComponent = requestContext.CreateComponent<IndexingUnitComponent>())
        return this.InvokeTableOperation<IndexingUnit>((Func<IndexingUnit>) (() => indexingUnitComponent.Update(indexingUnit)));
    }

    public virtual IndexingUnit AssociateJobId(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      this.ValidateNotNull<Guid?>("indexingUnit.AssociatedJobId", indexingUnit.AssociatedJobId);
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        return component is IndexingUnitComponentV5 indexingUnitComponentV5 ? indexingUnitComponentV5.AssociateJobId(indexingUnit) : component.Update(indexingUnit);
    }

    public List<IndexingUnit> UpdateIndexingUnits(
      IVssRequestContext requestContext,
      List<IndexingUnit> indexingUnits)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnit>(nameof (indexingUnits), (IList<IndexingUnit>) indexingUnits);
      using (IndexingUnitComponent indexingUnitComponent = requestContext.CreateComponent<IndexingUnitComponent>())
        return this.InvokeTableOperation<List<IndexingUnit>>((Func<List<IndexingUnit>>) (() => indexingUnitComponent.UpdateIndexingUnitsBatch(indexingUnits)));
    }

    public IndexingUnit DisassociateJobId(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      using (IndexingUnitComponent indexingUnitComponent = requestContext.CreateComponent<IndexingUnitComponent>())
        return this.InvokeTableOperation<IndexingUnit>((Func<IndexingUnit>) (() => indexingUnitComponent.DisassociateJobId(indexingUnit)));
    }

    public Dictionary<Guid, IndexingUnit> GetAssociatedJobIds(
      IVssRequestContext requestContext,
      List<IEntityType> entityTypes)
    {
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        return component is IndexingUnitComponentV7 indexingUnitComponentV7 ? indexingUnitComponentV7.GetAssociatedJobIds(entityTypes) : new Dictionary<Guid, IndexingUnit>();
    }

    public void DeleteIndexingUnit(IVssRequestContext requestContext, IndexingUnit indexingUnit)
    {
      this.ValidateNotNull<IndexingUnit>(nameof (indexingUnit), indexingUnit);
      using (IndexingUnitComponent indexingUnitComponent = requestContext.CreateComponent<IndexingUnitComponent>())
        this.InvokeTableOperation<IndexingUnit>((Func<IndexingUnit>) (() =>
        {
          indexingUnitComponent.Delete(indexingUnit);
          return (IndexingUnit) null;
        }));
    }

    public void DeleteIndexingUnitsPermanently(
      IVssRequestContext requestContext,
      List<IndexingUnit> indexingUnitList)
    {
      this.ValidateNotNullOrEmptyList<IndexingUnit>(nameof (indexingUnitList), (IList<IndexingUnit>) indexingUnitList);
      using (IndexingUnitComponent indexingUnitComponent = requestContext.CreateComponent<IndexingUnitComponent>())
        this.InvokeTableOperation<List<IndexingUnit>>((Func<List<IndexingUnit>>) (() =>
        {
          indexingUnitComponent.DeleteIndexingUnitBatchPermanently(indexingUnitList);
          return (List<IndexingUnit>) null;
        }));
    }

    public List<IndexingUnit> GetIndexingUnitsWithGivenParent(
      IVssRequestContext requestContext,
      int parentIndexingUnitId,
      int topCount)
    {
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
      {
        if (component is IndexingUnitComponentV4 indexingUnitComponentV4)
          return indexingUnitComponentV4.GetIndexingUnitsWithGivenParentId(parentIndexingUnitId, topCount);
        TableEntityFilterList filterList = new TableEntityFilterList();
        filterList.Add(new TableEntityFilter("ParentUnitID", "eq", parentIndexingUnitId.ToString()));
        return component.RetriveTableEntityList(topCount, filterList);
      }
    }

    public IDictionary<int, IndexingUnit> GetIndexingUnits(
      IVssRequestContext requestContext,
      IEnumerable<int> indexingUnitIds)
    {
      IDictionary<int, IndexingUnit> indexingUnits = (IDictionary<int, IndexingUnit>) new Dictionary<int, IndexingUnit>();
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
      {
        if (component is IndexingUnitComponentV2 indexingUnitComponentV2)
          return indexingUnitComponentV2.GetIndexingUnits(indexingUnitIds);
        foreach (int indexingUnitId in indexingUnitIds)
        {
          TableEntityFilterList filterList = new TableEntityFilterList();
          filterList.Add(new TableEntityFilter("IndexingUnitId", "eq", indexingUnitId.ToString()));
          IndexingUnit indexingUnit = component.RetriveTableEntity(filterList);
          indexingUnits.Add(indexingUnit.IndexingUnitId, indexingUnit);
        }
      }
      return indexingUnits;
    }

    public virtual List<IndexingUnit> GetIndexingUnitsRoutingInfo(
      IVssRequestContext requestContext,
      IEntityType entityType,
      List<string> indexingUnitTypes)
    {
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        return component.GetIndexingUnitsRoutingInfo(entityType, indexingUnitTypes);
    }

    public virtual List<IndexingUnit> GetChildIndexingUnitsRoutingInfo(
      IVssRequestContext requestContext,
      string indexingUnitType,
      int parentUnitId)
    {
      if (parentUnitId <= 0)
        throw new DataAccessException(DataAccessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("parentUnitId must be a positive integer"));
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        return component.GetChildIndexingUnitsRoutingInfo(indexingUnitType, parentUnitId);
    }

    public void AddOrUpdateIndexingUnitInformation(
      IVssRequestContext requestContext,
      List<IndexingUnitDetails> indexingUnitDetailsList,
      List<IndexingUnit> indexingUnitList)
    {
      this.ValidateIndexingUnitDetailsAndIndexingUnitCorrelate((IList<IndexingUnitDetails>) indexingUnitDetailsList, (IList<IndexingUnit>) indexingUnitList);
      using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
        component.AddOrUpdateIndexingUnitInformation(indexingUnitList, indexingUnitDetailsList);
    }

    public void InsertIndexingUnitDetails(
      IVssRequestContext requestContext,
      List<IndexingUnitDetails> indexingUnitDetailsList)
    {
      using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
        component.InsertIndexingUnitDetails(indexingUnitDetailsList);
    }

    public void UpdateIndexingUnitGrowthDetails(
      IVssRequestContext requestContext,
      int indexingUnitId,
      string lastIndexedWatermark,
      int changeInActualDocCount,
      long changeInActualSize)
    {
      using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
        component.UpdateIndexingUnitGrowthDetails(indexingUnitId, lastIndexedWatermark, changeInActualDocCount, changeInActualSize);
    }

    public void UpdateInitialEstimates(
      IVssRequestContext requestContext,
      int indexingUnitId,
      string lastIndexedWatermark,
      int estimatedInitialDocCount,
      int actualInitialDocCount,
      int estimatedDocCountGrowth,
      long estimatedInitialSize,
      long actualInitialSize,
      long estimatedSizeGrowth)
    {
      using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
        component.UpdateInitialEstimates(indexingUnitId, lastIndexedWatermark, estimatedInitialDocCount, actualInitialDocCount, estimatedDocCountGrowth, estimatedInitialSize, actualInitialSize, estimatedSizeGrowth);
    }

    public IndexingUnitDetails GetIndexingUnitDetails(
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit)
    {
      using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
        return component.QueryIndexingUnitDetails(indexingUnit.IndexingUnitId);
    }

    public IDictionary<int, IndexingUnitDetails> FetchIndexingUnitDetails(
      IVssRequestContext requestContext,
      IEnumerable<int> indexingUnitIds)
    {
      using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
        return component.GetIndexingUnitDetails((IList<int>) indexingUnitIds.ToList<int>());
    }

    public List<IndexingUnitDetails> FetchIndexingUnitDetails(
      IVssRequestContext requestContext,
      IEntityType entityType,
      string indexingUnitType)
    {
      using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
        return component.GetIndexingUnitDetails(entityType, indexingUnitType);
    }

    public void AddOrResetIndexingUnitDetailsAndUpdateShardDetails(
      IVssRequestContext requestContext,
      IList<IndexingUnitDetails> indexingUnitIndexingInformationList,
      IList<IndexingUnit> indexingUnitList,
      IList<ShardDetails> shardEstimationsBeforeAllocation = null)
    {
      if (indexingUnitIndexingInformationList.IsNullOrEmpty<IndexingUnitDetails>() && indexingUnitList.IsNullOrEmpty<IndexingUnit>())
        return;
      string str = "~|~";
      IDictionary<string, List<ShardDetails>> dictionary1 = (IDictionary<string, List<ShardDetails>>) new FriendlyDictionary<string, List<ShardDetails>>();
      if (shardEstimationsBeforeAllocation != null)
      {
        foreach (ShardDetails shardDetails in (IEnumerable<ShardDetails>) shardEstimationsBeforeAllocation)
        {
          string key = shardDetails.EsClusterName + str + shardDetails.IndexName;
          List<ShardDetails> shardDetailsList;
          if (!dictionary1.TryGetValue(key, out shardDetailsList))
          {
            shardDetailsList = new List<ShardDetails>();
            dictionary1[key] = shardDetailsList;
          }
          shardDetailsList.Add(shardDetails);
        }
      }
      this.ValidateIndexingUnitDetailsAndIndexingUnitCorrelate(indexingUnitIndexingInformationList, indexingUnitList);
      IList<int> indexingUnitIds = (IList<int>) new List<int>();
      foreach (IndexingUnitDetails indexingInformation in (IEnumerable<IndexingUnitDetails>) indexingUnitIndexingInformationList)
      {
        if (indexingInformation.IndexingUnitId > 0)
          indexingUnitIds.Add(indexingInformation.IndexingUnitId);
      }
      List<IndexingUnitDetails> indexingUnitDetailsList1;
      if (indexingUnitIds.Count > 0)
      {
        using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
          indexingUnitDetailsList1 = component.GetIndexingUnitDetails(indexingUnitIds).Values.ToList<IndexingUnitDetails>();
      }
      else
        indexingUnitDetailsList1 = new List<IndexingUnitDetails>();
      Dictionary<string, Tuple<List<IndexingUnitDetails>, List<IndexingUnit>, List<IndexingUnitDetails>>> dictionary2 = new Dictionary<string, Tuple<List<IndexingUnitDetails>, List<IndexingUnit>, List<IndexingUnitDetails>>>();
      for (int index = 0; index < indexingUnitIndexingInformationList.Count; ++index)
      {
        IndexingUnitDetails indexingInformation = indexingUnitIndexingInformationList[index];
        IndexingUnit indexingUnit = indexingUnitList[index];
        string key = indexingInformation.ESClusterName + str + indexingInformation.IndexName;
        Tuple<List<IndexingUnitDetails>, List<IndexingUnit>, List<IndexingUnitDetails>> tuple;
        if (!dictionary2.TryGetValue(key, out tuple))
        {
          tuple = Tuple.Create<List<IndexingUnitDetails>, List<IndexingUnit>, List<IndexingUnitDetails>>(new List<IndexingUnitDetails>(), new List<IndexingUnit>(), new List<IndexingUnitDetails>());
          dictionary2[key] = tuple;
        }
        tuple.Item1.Add(indexingInformation);
        tuple.Item2.Add(indexingUnit);
      }
      foreach (IndexingUnitDetails indexingUnitDetails in indexingUnitDetailsList1)
      {
        string key = indexingUnitDetails.ESClusterName + str + indexingUnitDetails.IndexName;
        Tuple<List<IndexingUnitDetails>, List<IndexingUnit>, List<IndexingUnitDetails>> tuple;
        if (!dictionary2.TryGetValue(key, out tuple))
        {
          tuple = Tuple.Create<List<IndexingUnitDetails>, List<IndexingUnit>, List<IndexingUnitDetails>>(new List<IndexingUnitDetails>(), new List<IndexingUnit>(), new List<IndexingUnitDetails>());
          dictionary2[key] = tuple;
        }
        tuple.Item3.Add(indexingUnitDetails);
      }
      using (Dictionary<string, Tuple<List<IndexingUnitDetails>, List<IndexingUnit>, List<IndexingUnitDetails>>>.Enumerator enumerator = dictionary2.GetEnumerator())
      {
label_69:
        while (enumerator.MoveNext())
        {
          KeyValuePair<string, Tuple<List<IndexingUnitDetails>, List<IndexingUnit>, List<IndexingUnitDetails>>> current = enumerator.Current;
          string key = current.Key;
          Tuple<List<IndexingUnitDetails>, List<IndexingUnit>, List<IndexingUnitDetails>> tuple = current.Value;
          List<IndexingUnitDetails> indexingUnitDetailsList2 = tuple.Item1;
          List<IndexingUnit> indexingUnitList1 = tuple.Item2;
          Dictionary<int, IndexingUnitDetails> dictionary3 = tuple.Item3.ToDictionary<IndexingUnitDetails, int, IndexingUnitDetails>((Func<IndexingUnitDetails, int>) (x => x.IndexingUnitId), (Func<IndexingUnitDetails, IndexingUnitDetails>) (x => x));
          List<ShardDetails> originalShardEstimationsBeforeAllocation;
          dictionary1.TryGetValue(key, out originalShardEstimationsBeforeAllocation);
          originalShardEstimationsBeforeAllocation = originalShardEstimationsBeforeAllocation ?? new List<ShardDetails>();
          while (true)
          {
            List<IndexingUnitDetails> indexingUnitDetailsList3;
            List<IndexingUnit> indexingUnitList2;
            List<IndexingUnitDetails> indexingUnitDetailsList4;
            if (indexingUnitDetailsList2.Count > 0)
            {
              int count = Math.Min(this.BatchCount, indexingUnitDetailsList2.Count);
              indexingUnitDetailsList3 = indexingUnitDetailsList2.GetRange(0, count);
              indexingUnitList2 = indexingUnitList1.GetRange(0, count);
              indexingUnitDetailsList4 = new List<IndexingUnitDetails>();
              foreach (IndexingUnitDetails indexingUnitDetails1 in indexingUnitDetailsList3)
              {
                IndexingUnitDetails indexingUnitDetails2;
                if (dictionary3.TryGetValue(indexingUnitDetails1.IndexingUnitId, out indexingUnitDetails2))
                {
                  indexingUnitDetailsList4.Add(indexingUnitDetails2);
                  dictionary3.Remove(indexingUnitDetails1.IndexingUnitId);
                }
              }
              indexingUnitDetailsList2.RemoveRange(0, count);
              indexingUnitList1.RemoveRange(0, count);
            }
            else if (dictionary3.Count > 0)
            {
              int count = Math.Min(this.BatchCount, dictionary3.Count);
              indexingUnitDetailsList3 = new List<IndexingUnitDetails>();
              indexingUnitList2 = new List<IndexingUnit>();
              indexingUnitDetailsList4 = dictionary3.Values.ToList<IndexingUnitDetails>().GetRange(0, count);
              foreach (IndexingUnitDetails indexingUnitDetails in indexingUnitDetailsList4)
                dictionary3.Remove(indexingUnitDetails.IndexingUnitId);
            }
            else
              goto label_69;
            Dictionary<short, ShardEstimateChange> shardMapping = new Dictionary<short, ShardEstimateChange>();
            this.UpdateShardMappingForIndexingUnits(shardMapping, indexingUnitDetailsList3, originalShardEstimationsBeforeAllocation);
            this.UpdateShardMappingForIndexingUnits(shardMapping, indexingUnitDetailsList4, originalShardEstimationsBeforeAllocation, false);
            using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
              component.UpdateShardEstimations(shardMapping.Values.ToList<ShardEstimateChange>());
            try
            {
              if (indexingUnitDetailsList3.Count > 0)
                this.AddOrUpdateIndexingUnitInformation(requestContext, indexingUnitDetailsList3, indexingUnitList2);
            }
            catch
            {
              List<ShardEstimateChange> shardEstimateChangeList = this.FetchRevertedShardEstimates(shardMapping);
              using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
                component.UpdateShardEstimations(shardEstimateChangeList);
              throw;
            }
          }
        }
      }
    }

    public void DeleteIndexingUnitDetailsAndUpdateShardInformation(
      IVssRequestContext requestContext,
      List<int> iuList)
    {
      this.ValidateNotNull<List<int>>(nameof (iuList), iuList);
      int count1 = iuList.Count;
      for (int index = 0; index < count1; index += this.BatchCount)
      {
        int count2 = Math.Min(this.BatchCount, count1 - index);
        List<int> range = iuList.GetRange(index, count2);
        Dictionary<short, ShardEstimateChange> shardMapping = new Dictionary<short, ShardEstimateChange>();
        List<IndexingUnitDetails> list;
        using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
          list = component.GetIndexingUnitDetails((IList<int>) range).Values.ToList<IndexingUnitDetails>();
        if (list.Count > 0)
        {
          List<int> indexingUnitIdList = new List<int>();
          foreach (IndexingUnitDetails indexingUnitDetails in list)
            indexingUnitIdList.Add(indexingUnitDetails.IndexingUnitId);
          this.UpdateShardMappingForIndexingUnits(shardMapping, list, new List<ShardDetails>(), false);
          using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
            component.UpdateShardEstimations(shardMapping.Values.ToList<ShardEstimateChange>());
          try
          {
            this.DeleteIndexingUnitDetails(requestContext, indexingUnitIdList);
          }
          catch
          {
            List<ShardEstimateChange> shardEstimateChangeList = this.FetchRevertedShardEstimates(shardMapping);
            using (ShardDetailsComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ShardDetailsComponent>())
              component.UpdateShardEstimations(shardEstimateChangeList);
            throw;
          }
        }
      }
    }

    public void DeleteIndexingUnitDetails(
      IVssRequestContext requestContext,
      List<int> indexingUnitIdList)
    {
      using (IndexingUnitDetailsComponent component = requestContext.CreateComponent<IndexingUnitDetailsComponent>())
        component.DeleteIndexingUnitDetailsByIds(indexingUnitIdList);
    }

    private List<ShardEstimateChange> FetchRevertedShardEstimates(
      Dictionary<short, ShardEstimateChange> shardMapping)
    {
      List<ShardEstimateChange> list = shardMapping.Values.ToList<ShardEstimateChange>();
      foreach (ShardEstimateChange shardEstimateChange in list)
      {
        shardEstimateChange.ChangeInEstimatedDocCount = -shardEstimateChange.ChangeInEstimatedDocCount;
        shardEstimateChange.ChangeInEstimatedDocCountGrowth = -shardEstimateChange.ChangeInEstimatedDocCountGrowth;
        shardEstimateChange.ChangeInEstimatedSize = -shardEstimateChange.ChangeInEstimatedSize;
        shardEstimateChange.ChangeInEstimatedSizeGrowth = -shardEstimateChange.ChangeInEstimatedSizeGrowth;
      }
      return list;
    }

    internal void UpdateShardMappingForIndexingUnits(
      Dictionary<short, ShardEstimateChange> shardMapping,
      List<IndexingUnitDetails> indexingUnitDetailsList,
      List<ShardDetails> originalShardEstimationsBeforeAllocation,
      bool isIndexingUnitBeingAdded = true)
    {
      string[] separator = new string[1]{ "," };
      Dictionary<short, ShardDetails> dictionary = originalShardEstimationsBeforeAllocation.ToDictionary<ShardDetails, short, ShardDetails>((Func<ShardDetails, short>) (x => x.ShardId), (Func<ShardDetails, ShardDetails>) (x => x));
      for (int index1 = 0; index1 < indexingUnitDetailsList.Count; ++index1)
      {
        IndexingUnitDetails indexingUnitDetails = indexingUnitDetailsList[index1];
        string[] strArray = indexingUnitDetails.ShardIds.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        int num1 = indexingUnitDetails.EstimatedInitialDocCount / strArray.Length;
        int num2 = indexingUnitDetails.EstimatedDocCountGrowth / strArray.Length;
        long num3 = indexingUnitDetails.EstimatedInitialSize / (long) strArray.Length;
        long num4 = indexingUnitDetails.EstimatedSizeGrowth / (long) strArray.Length;
        for (int index2 = 0; index2 < strArray.Length; ++index2)
        {
          short num5 = short.Parse(strArray[index2]);
          ShardEstimateChange shardEstimateChange;
          if (!shardMapping.TryGetValue(num5, out shardEstimateChange))
          {
            shardEstimateChange = new ShardEstimateChange(indexingUnitDetails.ESClusterName, indexingUnitDetails.EntityType, indexingUnitDetails.IndexName, num5, 0, 0, 0, 0L, 0L, 0L);
            shardMapping.Add(num5, shardEstimateChange);
          }
          if (isIndexingUnitBeingAdded)
          {
            shardEstimateChange.ChangeInEstimatedDocCount += num1;
            shardEstimateChange.ChangeInEstimatedDocCountGrowth += num2;
            shardEstimateChange.ChangeInEstimatedSize += num3;
            shardEstimateChange.ChangeInEstimatedSizeGrowth += num4;
          }
          else
          {
            shardEstimateChange.ChangeInEstimatedDocCount -= num1;
            shardEstimateChange.ChangeInEstimatedDocCountGrowth -= num2;
            shardEstimateChange.ChangeInEstimatedSize -= num3;
            shardEstimateChange.ChangeInEstimatedSizeGrowth -= num4;
          }
        }
      }
      foreach (KeyValuePair<short, ShardEstimateChange> keyValuePair in shardMapping)
      {
        short key = keyValuePair.Key;
        ShardEstimateChange shardEstimateChange = keyValuePair.Value;
        ShardDetails shardDetails;
        if (dictionary.TryGetValue(key, out shardDetails))
        {
          shardEstimateChange.OriginalEstimatedDocCount = shardDetails.EstimatedDocCount;
          shardEstimateChange.OriginalEstimatedDocCountGrowth = shardDetails.EstimatedDocCountGrowth;
          shardEstimateChange.OriginalReservedDocCount = shardDetails.ReservedDocCount;
        }
      }
    }

    private void ValidateSameIndexForIndexingUnitDetailsList(
      List<IndexingUnitDetails> indexingUnitDetailsList)
    {
      string esClusterName = indexingUnitDetailsList[0].ESClusterName;
      string indexName = indexingUnitDetailsList[0].IndexName;
      foreach (IndexingUnitDetails indexingUnitDetails in indexingUnitDetailsList)
      {
        if (indexingUnitDetails.IndexName != indexName || indexingUnitDetails.ESClusterName != esClusterName)
          throw new ArgumentException("The IndexName or Cluster are not same throughout the provided list.");
      }
    }

    private void ValidateIndexingUnitDetailsAndIndexingUnitCorrelate(
      IList<IndexingUnitDetails> indexingUnitDetailsList,
      IList<IndexingUnit> indexingUnitList)
    {
      if (indexingUnitDetailsList.Count != indexingUnitList.Count)
        throw new ArgumentException("Item Count in IndexingUnitList and indexingUnitdetails list mismatch");
      for (int index = 0; index < indexingUnitDetailsList.Count; ++index)
      {
        IndexingUnitDetails indexingUnitDetails = indexingUnitDetailsList[index];
        IndexingUnit indexingUnit = indexingUnitList[index];
        Guid? nullable = indexingUnitDetails.TFSEntityId;
        nullable = nullable.HasValue ? indexingUnitDetails.TFSEntityId : throw new ArgumentException("Required values (TFSEntityId, IUType or EntityType) in IndexingUnitList and indexingUnitDetails list mismatch");
        Guid tfsEntityId = indexingUnit.TFSEntityId;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != tfsEntityId ? 1 : 0) : 0) : 1) == 0 && !(indexingUnitDetails.EntityType.Name != indexingUnit.EntityType.Name) && indexingUnitDetails.IndexingUnitType != null && !(indexingUnitDetails.IndexingUnitType != indexingUnit.IndexingUnitType))
        {
          if (string.IsNullOrWhiteSpace(indexingUnitDetails.ESClusterName) || string.IsNullOrWhiteSpace(indexingUnitDetails.IndexName))
            throw new ArgumentException("Required values (ESClusterName or IndexName) missing in IndexingUnitDetails");
          continue;
        }
      }
    }

    public void PromoteShadowIndexingUnitsToPrimary(
      IVssRequestContext requestContext,
      List<string> indexingUnitTypes,
      IEntityType entityType)
    {
      this.ValidateNotNullOrEmptyList<string>(nameof (indexingUnitTypes), (IList<string>) indexingUnitTypes);
      using (IndexingUnitComponent component = (IndexingUnitComponent) requestContext.CreateComponent<IndexingUnitComponentV7>())
        component.PromoteShadowIndexingUnitsToPrimary(indexingUnitTypes, entityType);
    }

    public List<IndexingUnit> GetIndexingUnitsOfOrphanRepos(IVssRequestContext requestContext)
    {
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        return component.GetOrphanIndexingUnitsOfTypeGitOrTfvcRepository();
    }

    public void SoftDeleteIndexingUnits(
      IVssRequestContext requestContext,
      IEnumerable<int> indexingUnitIdList)
    {
      using (IndexingUnitComponent component = requestContext.CreateComponent<IndexingUnitComponent>())
        component.SoftDeleteIndexingUnitBatch(indexingUnitIdList);
    }
  }
}
