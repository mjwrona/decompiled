// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent13
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
  internal class WorkItemTypeExtensionComponent13 : WorkItemTypeExtensionComponent12
  {
    internal override void DestroyWorkItemTypelets(Guid processId, Guid changedBy, Guid witId)
    {
      this.PrepareStoredProcedure("prc_DestroyWorkItemTypelets");
      this.BindGuid("@processId", processId);
      this.BindGuid("@changedBy", changedBy);
      this.BindGuid("@witId", witId);
      this.ExecuteNonQueryEx();
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
      this.BindString("@behaviorReferenceName", behaviorReferenceName, 386, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQueryEx();
    }

    public override List<WorkItemTypeletRecord> GetWorkItemTypelets(Guid processId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypelets");
      this.BindGuid("@processId", processId);
      return this.ReadTypelets((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder2());
    }

    protected override List<WorkItemTypeletRecord> ReadTypelets(
      ObjectBinder<WorkItemTypeletRecord> binder)
    {
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<WorkItemTypeletRecord>(binder);
      resultCollection.AddBinder<Tuple<Guid, int>>((ObjectBinder<Tuple<Guid, int>>) new TupleBinder<Guid, int>());
      resultCollection.AddBinder<Tuple<Guid, string>>((ObjectBinder<Tuple<Guid, string>>) new TupleBinder<Guid, string>());
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
      foreach (IGrouping<Guid, string> source in resultCollection.GetCurrent<Tuple<Guid, string>>().Items.GroupBy<Tuple<Guid, string>, Guid, string>((System.Func<Tuple<Guid, string>, Guid>) (t => t.Item1), (System.Func<Tuple<Guid, string>, string>) (t => t.Item2)))
      {
        WorkItemTypeletRecord itemTypeletRecord;
        if (dictionary.TryGetValue(source.Key, out itemTypeletRecord))
          itemTypeletRecord.Behaviors = source.Select<string, WorkItemTypeExtensionBehaviorRecord>((System.Func<string, WorkItemTypeExtensionBehaviorRecord>) (bRef => new WorkItemTypeExtensionBehaviorRecord()
          {
            BehaviorReferenceName = bRef
          })).ToArray<WorkItemTypeExtensionBehaviorRecord>();
      }
      return items;
    }
  }
}
