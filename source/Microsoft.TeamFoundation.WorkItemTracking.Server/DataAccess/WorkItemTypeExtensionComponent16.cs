// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent16
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent16 : WorkItemTypeExtensionComponent15
  {
    public override WorkItemTypeletRecord CreateBehavior(
      Guid extensionId,
      Guid processId,
      string name,
      string refName,
      string parentTypeRefName,
      Guid changedBy,
      string color,
      int rank,
      bool isAbstract,
      int limitCount)
    {
      this.PrepareStoredProcedure("prc_CreateBehavior");
      this.BindGuid("@id", extensionId);
      this.BindGuid("@processId", processId);
      this.BindInt("@typeletType", 2);
      this.BindString("@name", name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@refName", refName, 386, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@parentTypeRefName", parentTypeRefName, 386, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@changedBy", changedBy);
      this.BindString("@color", color, 10, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@rank", rank);
      this.BindBoolean("@abstract", isAbstract);
      this.BindInt("@limitCount", limitCount);
      return this.ReadExtensions((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder3()).FirstOrDefault<WorkItemTypeletRecord>();
    }

    public override List<WorkItemTypeletRecord> GetWorkItemTypelets(Guid processId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypelets");
      this.BindGuid("@processId", processId);
      return this.ReadTypelets((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder3(), (ObjectBinder<WorkItemTypeExtensionBehaviorRecord>) new WorkItemTypeExtensionBehaviorRecordBinder());
    }

    internal override void CreateWorkItemTypeBehaviorReference(
      Guid processId,
      Guid witId,
      string behaviorReferenceName,
      Guid changerId,
      bool isDefault)
    {
      this.PrepareStoredProcedure("prc_CreateWorkItemTypeBehaviorReference");
      this.BindGuid("@processId", processId);
      this.BindGuid("@workItemTypeId", witId);
      this.BindBoolean("@isDefault", isDefault);
      this.BindString("@behaviorReferenceName", behaviorReferenceName, 386, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQueryEx();
    }

    internal override List<WorkItemTypeletRecord> ReadTypelets(
      ObjectBinder<WorkItemTypeletRecord> binder,
      ObjectBinder<WorkItemTypeExtensionBehaviorRecord> behaviorRecordBinder)
    {
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<WorkItemTypeletRecord>(binder);
      resultCollection.AddBinder<Tuple<Guid, int>>((ObjectBinder<Tuple<Guid, int>>) new TupleBinder<Guid, int>());
      resultCollection.AddBinder<WorkItemTypeExtensionBehaviorRecord>(behaviorRecordBinder);
      List<WorkItemTypeletRecord> items = resultCollection.GetCurrent<WorkItemTypeletRecord>().Items;
      Dictionary<Guid, WorkItemTypeletRecord> dictionary = items.ToDictionary<WorkItemTypeletRecord, Guid, WorkItemTypeletRecord>((System.Func<WorkItemTypeletRecord, Guid>) (e => e.Id), (System.Func<WorkItemTypeletRecord, WorkItemTypeletRecord>) (e => e));
      resultCollection.NextResult();
      foreach (IGrouping<Guid, int> source in resultCollection.GetCurrent<Tuple<Guid, int>>().Items.GroupBy<Tuple<Guid, int>, Guid, int>((System.Func<Tuple<Guid, int>, Guid>) (t => t.Item1), (System.Func<Tuple<Guid, int>, int>) (t => t.Item2)))
      {
        WorkItemTypeletRecord itemTypeletRecord;
        if (dictionary.TryGetValue(source.Key, out itemTypeletRecord))
          itemTypeletRecord.Fields = source.Select<int, WorkItemTypeletFieldRecord>((System.Func<int, WorkItemTypeletFieldRecord>) (fieldId => new WorkItemTypeletFieldRecord()
          {
            FieldId = fieldId
          })).ToArray<WorkItemTypeletFieldRecord>();
      }
      resultCollection.NextResult();
      foreach (IGrouping<Guid, WorkItemTypeExtensionBehaviorRecord> source in resultCollection.GetCurrent<WorkItemTypeExtensionBehaviorRecord>().Items.GroupBy<WorkItemTypeExtensionBehaviorRecord, Guid, WorkItemTypeExtensionBehaviorRecord>((System.Func<WorkItemTypeExtensionBehaviorRecord, Guid>) (t => t.WorkItemTypeId), (System.Func<WorkItemTypeExtensionBehaviorRecord, WorkItemTypeExtensionBehaviorRecord>) (t => t)))
      {
        WorkItemTypeletRecord itemTypeletRecord;
        if (dictionary.TryGetValue(source.Key, out itemTypeletRecord))
          itemTypeletRecord.Behaviors = source.ToArray<WorkItemTypeExtensionBehaviorRecord>();
      }
      return items;
    }

    internal override void UpdateDefaultWorkItemTypeForBehavior(
      Guid processId,
      Guid witId,
      string behaviorReferenceName,
      Guid changerId,
      bool isDefault)
    {
      this.PrepareStoredProcedure("prc_UpdateDefaultWorkItemTypeForBehavior");
      this.BindGuid("@processId", processId);
      this.BindGuid("@workItemTypeId", witId);
      this.BindString("@behaviorReferenceName", behaviorReferenceName, 386, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQueryEx();
    }
  }
}
