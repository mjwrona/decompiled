// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.ItemLevelFailuresComponentV9
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class ItemLevelFailuresComponentV9 : ItemLevelFailuresComponentV8
  {
    public ItemLevelFailuresComponentV9()
    {
    }

    internal ItemLevelFailuresComponentV9(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override int MergeItemFailureRecords(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IEnumerable<ItemLevelFailureRecord> records,
      int maxAttemptCount)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (records == null || !records.Any<ItemLevelFailureRecord>())
        return 0;
      int num1 = 0;
      if (indexingUnit.EntityType.Name == "Code" || indexingUnit.EntityType.Name == "Wiki")
      {
        List<ItemLevelFailureRecord> list = records.ToList<ItemLevelFailureRecord>();
        int count1 = list.Count;
        int val1 = count1;
        for (int index = 0; index < count1; index += 500)
        {
          Stopwatch stopwatch = Stopwatch.StartNew();
          int count2 = Math.Min(val1, 500);
          IList<ItemLevelFailureRecord> range = (IList<ItemLevelFailureRecord>) list.GetRange(index, count2);
          val1 -= count2;
          if (range.Count > 0)
          {
            this.SanitizeBranchNames(range);
            IDictionary<string, ItemLevelFailureRecord> failureRecordsMap = this.GetFileToFailureRecordsMap((IEnumerable<ItemLevelFailureRecord>) range);
            IDictionary<string, ItemLevelFailureRecord> failedItemsFromSql = this.GetExistingFailedItemsFromSQL(indexingUnit, (IList<string>) range.Select<ItemLevelFailureRecord, string>((System.Func<ItemLevelFailureRecord, string>) (x => x.Item)).ToList<string>());
            this.UpdateRecords(failureRecordsMap, failedItemsFromSql);
            this.PrepareStoredProcedure("Search.prc_UpsertFileLevelFailureRecords");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindFileLevelFailureRecordsParameterContainingListOfBranches("@fileLevelFailureRecords", (IEnumerable<ItemLevelFailureRecord>) failureRecordsMap.Values.ToList<ItemLevelFailureRecord>());
            this.BindInt("@maxFailureThreshold", maxAttemptCount);
            int num2 = (int) this.ExecuteScalar();
            num1 += num2;
            stopwatch.Stop();
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("ItemLevelFailuresComponent.MergeItemFailureRecords took {0}ms", (object) stopwatch.ElapsedMilliseconds));
          }
          else
            break;
        }
      }
      else
      {
        if (!(indexingUnit.EntityType.Name == "WorkItem"))
          throw new NotSupportedException(string.Format("Entity type [{0}] is not supported.", (object) indexingUnit.EntityType));
        List<ItemLevelFailureRecord> list = records.ToList<ItemLevelFailureRecord>();
        int count3 = list.Count;
        int val1 = count3;
        for (int index = 0; index < count3; index += 500)
        {
          int count4 = Math.Min(val1, 500);
          IList<ItemLevelFailureRecord> range = (IList<ItemLevelFailureRecord>) list.GetRange(index, count4);
          val1 -= count4;
          if (range.Count > 0)
          {
            this.PrepareStoredProcedure("Search.prc_MergeWorkItemLevelFailures");
            this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
            this.BindWorkItemLevelFailureRecordsParameter("@workItemLevelFailureRecords", (IEnumerable<ItemLevelFailureRecord>) range);
            this.BindInt("@maxFailureThreshold", maxAttemptCount);
            int num3 = (int) this.ExecuteScalar();
            num1 += num3;
          }
          else
            break;
        }
      }
      return num1;
    }

    public IDictionary<string, ItemLevelFailureRecord> GetExistingFailedItemsFromSQL(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IList<string> recordsBatch)
    {
      List<ItemLevelFailureRecord> levelFailureRecordList = new List<ItemLevelFailureRecord>();
      IDictionary<string, ItemLevelFailureRecord> failedItemsFromSql = (IDictionary<string, ItemLevelFailureRecord>) new Dictionary<string, ItemLevelFailureRecord>((IEqualityComparer<string>) this.FilePathComparer);
      if (recordsBatch == null || recordsBatch.Count == 0)
        return failedItemsFromSql;
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PrepareStoredProcedure("Search.prc_RetrieveFailedItemsBatch");
      this.BindInt("@indexingUnitId", indexingUnit.IndexingUnitId);
      this.BindStringTable("@itemLevelFailureRecords", (IEnumerable<string>) recordsBatch);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ItemLevelFailureRecord>((ObjectBinder<ItemLevelFailureRecord>) new ItemLevelFailuresComponent.FailedItemRecordColumnsV2());
        ObjectBinder<ItemLevelFailureRecord> current = resultCollection.GetCurrent<ItemLevelFailureRecord>();
        if (current != null)
        {
          if (current.Items != null)
          {
            if (current.Items.Count > 0)
              levelFailureRecordList.AddRange((IEnumerable<ItemLevelFailureRecord>) current.Items);
          }
        }
      }
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1082627, "Indexing Pipeline", "FileLevelFailuresComponent", string.Format("{0}.{1} took {2}ms", (object) "ItemLevelFailuresComponent", (object) nameof (GetExistingFailedItemsFromSQL), (object) stopwatch.ElapsedMilliseconds));
      foreach (ItemLevelFailureRecord levelFailureRecord in levelFailureRecordList)
        failedItemsFromSql.Add(levelFailureRecord.Item, levelFailureRecord);
      return failedItemsFromSql;
    }

    internal void SanitizeBranchNames(IList<ItemLevelFailureRecord> recordsBatch)
    {
      foreach (ItemLevelFailureRecord levelFailureRecord in (IEnumerable<ItemLevelFailureRecord>) recordsBatch)
      {
        Branches branches = new Branches();
        HashSet<string> source = new HashSet<string>();
        if (!(levelFailureRecord.Metadata is FileFailureMetadata metadata) || metadata.Branches == null)
          throw new Exception("metadata not found as expected for the record " + levelFailureRecord.ToString() + " ");
        foreach (string branch in (List<string>) metadata.Branches)
        {
          if (string.IsNullOrWhiteSpace(branch))
            source.Add(string.Empty);
          else
            source.Add(branch);
        }
        branches.AddRange((IEnumerable<string>) source.ToList<string>());
        metadata.Branches = branches;
      }
    }

    internal void UpdateRecords(
      IDictionary<string, ItemLevelFailureRecord> fileToFailureRecords,
      IDictionary<string, ItemLevelFailureRecord> fileToFailureRecordsFromSql)
    {
      if (fileToFailureRecords.Count == 0)
        throw new ArgumentException(nameof (fileToFailureRecords));
      foreach (KeyValuePair<string, ItemLevelFailureRecord> fileToFailureRecord in (IEnumerable<KeyValuePair<string, ItemLevelFailureRecord>>) fileToFailureRecords)
      {
        ItemLevelFailureRecord levelFailureRecord;
        if (fileToFailureRecordsFromSql.TryGetValue(fileToFailureRecord.Key, out levelFailureRecord))
        {
          FileFailureMetadata metadata1 = fileToFailureRecord.Value.Metadata as FileFailureMetadata;
          FileFailureMetadata metadata2 = levelFailureRecord.Metadata as FileFailureMetadata;
          if (metadata1 == null || metadata2 == null)
            throw new Exception("metadata not found for the record " + fileToFailureRecord.Key + " ");
          fileToFailureRecord.Value.AttemptCount = metadata1.Branches.Count + levelFailureRecord.AttemptCount;
          HashSet<string> source = new HashSet<string>();
          source.UnionWith((IEnumerable<string>) metadata2.Branches);
          source.UnionWith((IEnumerable<string>) metadata1.Branches);
          Branches branches = new Branches();
          branches.AddRange((IEnumerable<string>) source.ToList<string>());
          metadata1.Branches = branches;
        }
      }
    }
  }
}
