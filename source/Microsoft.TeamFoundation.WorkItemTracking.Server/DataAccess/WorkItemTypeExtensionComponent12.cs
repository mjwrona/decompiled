// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent12
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent12 : WorkItemTypeExtensionComponent11
  {
    public override List<WorkItemTypeletRecord> GetWorkItemTypelets(Guid processId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypelets");
      this.BindGuid("@processId", processId);
      return this.ReadTypelets((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder2());
    }

    protected virtual List<WorkItemTypeletRecord> ReadTypelets(
      ObjectBinder<WorkItemTypeletRecord> binder)
    {
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<WorkItemTypeletRecord>(binder);
      resultCollection.AddBinder<Tuple<Guid, int>>((ObjectBinder<Tuple<Guid, int>>) new TupleBinder<Guid, int>());
      resultCollection.AddBinder<Tuple<Guid, Guid>>((ObjectBinder<Tuple<Guid, Guid>>) new TupleBinder<Guid, Guid>());
      List<WorkItemTypeletRecord> items = resultCollection.GetCurrent<WorkItemTypeletRecord>().Items;
      Dictionary<Guid, WorkItemTypeletRecord> dictionary = items.ToDictionary<WorkItemTypeletRecord, Guid, WorkItemTypeletRecord>((Func<WorkItemTypeletRecord, Guid>) (e => e.Id), (Func<WorkItemTypeletRecord, WorkItemTypeletRecord>) (e => e));
      resultCollection.NextResult();
      foreach (IGrouping<Guid, int> source in resultCollection.GetCurrent<Tuple<Guid, int>>().Items.GroupBy<Tuple<Guid, int>, Guid, int>((Func<Tuple<Guid, int>, Guid>) (t => t.Item1), (Func<Tuple<Guid, int>, int>) (t => t.Item2)))
      {
        WorkItemTypeletRecord itemTypeletRecord;
        if (dictionary.TryGetValue(source.Key, out itemTypeletRecord))
          itemTypeletRecord.Fields = source.Select<int, WorkItemTypeletFieldRecord>((Func<int, WorkItemTypeletFieldRecord>) (fieldId => new WorkItemTypeletFieldRecord()
          {
            FieldId = fieldId
          })).ToArray<WorkItemTypeletFieldRecord>();
      }
      return items;
    }
  }
}
