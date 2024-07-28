// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemPickListComponent3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemPickListComponent3 : WorkItemPickListComponent2
  {
    [Obsolete]
    internal override IReadOnlyCollection<WorkItemPickListRecord> GetLists(Guid processId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemPickLists");
      this.BindGuid("@processId", processId);
      return this.ExecuteUnknown<IReadOnlyCollection<WorkItemPickListRecord>>((System.Func<IDataReader, IReadOnlyCollection<WorkItemPickListRecord>>) (reader =>
      {
        List<WorkItemPickListMetadataRecord> list = new WorkItemPickListComponent.WorkItemPickListMetadataBinder().BindAll(reader).ToList<WorkItemPickListMetadataRecord>();
        Dictionary<Guid, List<WorkItemPickListMemberRecord>> listMemberDictionary = list.ToDictionary<WorkItemPickListMetadataRecord, Guid, List<WorkItemPickListMemberRecord>>((System.Func<WorkItemPickListMetadataRecord, Guid>) (r => r.Id), (System.Func<WorkItemPickListMetadataRecord, List<WorkItemPickListMemberRecord>>) (r => new List<WorkItemPickListMemberRecord>()));
        if (!reader.NextResult())
          return (IReadOnlyCollection<WorkItemPickListRecord>) null;
        foreach (WorkItemPickListMemberRecord listMemberRecord in new WorkItemPickListComponent.WorkItemPickListMemberBinder().BindAll(reader).ToList<WorkItemPickListMemberRecord>())
        {
          List<WorkItemPickListMemberRecord> listMemberRecordList;
          if (listMemberDictionary.TryGetValue(listMemberRecord.ListId, out listMemberRecordList))
            listMemberRecordList.Add(listMemberRecord);
        }
        return (IReadOnlyCollection<WorkItemPickListRecord>) list.Select<WorkItemPickListMetadataRecord, WorkItemPickListRecord>((System.Func<WorkItemPickListMetadataRecord, WorkItemPickListRecord>) (m =>
        {
          return new WorkItemPickListRecord()
          {
            Id = m.Id,
            Name = m.Name,
            Type = m.Type,
            ConstId = m.ConstId,
            Items = (IReadOnlyList<WorkItemPickListMemberRecord>) listMemberDictionary[m.Id]
          };
        })).ToList<WorkItemPickListRecord>();
      }));
    }
  }
}
