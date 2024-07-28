// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemPickListComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemPickListComponent2 : WorkItemPickListComponent
  {
    [Obsolete]
    public override WorkItemPickListRecord GetList(Guid processId, Guid listId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemPickList");
      this.BindGuid("@processId", processId);
      this.BindGuid("@listId", listId);
      return this.ExecuteUnknown<WorkItemPickListRecord>((System.Func<IDataReader, WorkItemPickListRecord>) (reader =>
      {
        if (!reader.Read())
          return (WorkItemPickListRecord) null;
        WorkItemPickListMetadataRecord listMetadataRecord = new WorkItemPickListComponent.WorkItemPickListMetadataBinder().Bind(reader);
        reader.NextResult();
        List<WorkItemPickListMemberRecord> list = new WorkItemPickListComponent.WorkItemPickListMemberBinder().BindAll(reader).ToList<WorkItemPickListMemberRecord>();
        return new WorkItemPickListRecord()
        {
          Id = listMetadataRecord.Id,
          Name = listMetadataRecord.Name,
          Type = listMetadataRecord.Type,
          ConstId = listMetadataRecord.ConstId,
          Items = (IReadOnlyList<WorkItemPickListMemberRecord>) list
        };
      }));
    }

    [Obsolete]
    public override IReadOnlyCollection<WorkItemPickListMetadataRecord> GetListsMetadata(
      Guid processId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemPickListMetadata");
      this.BindGuid("@processId", processId);
      return this.ExecuteUnknown<IReadOnlyCollection<WorkItemPickListMetadataRecord>>((System.Func<IDataReader, IReadOnlyCollection<WorkItemPickListMetadataRecord>>) (reader => (IReadOnlyCollection<WorkItemPickListMetadataRecord>) new WorkItemPickListComponent.WorkItemPickListMetadataBinder().BindAll(reader).ToList<WorkItemPickListMetadataRecord>()));
    }

    [Obsolete]
    public override WorkItemPickListRecord CreateList(
      Guid processId,
      string name,
      WorkItemPickListType type,
      IReadOnlyList<string> items,
      Guid changerId,
      bool isSuggested = false)
    {
      this.PrepareStoredProcedure("prc_CreateWorkItemPickList");
      this.BindGuid("@processId", processId);
      this.BindString("@name", name, 128, false, SqlDbType.NVarChar);
      this.BindInt("@type", (int) type);
      this.BindStringTable("@items", (IEnumerable<string>) items);
      this.BindGuid("@changerId", changerId);
      return this.ExecuteUnknown<WorkItemPickListRecord>((System.Func<IDataReader, WorkItemPickListRecord>) (reader =>
      {
        if (!reader.Read())
          return (WorkItemPickListRecord) null;
        WorkItemPickListMetadataRecord listMetadataRecord = new WorkItemPickListComponent.WorkItemPickListMetadataBinder().Bind(reader);
        reader.NextResult();
        List<WorkItemPickListMemberRecord> list = new WorkItemPickListComponent.WorkItemPickListMemberBinder().BindAll(reader).ToList<WorkItemPickListMemberRecord>();
        return new WorkItemPickListRecord()
        {
          Id = listMetadataRecord.Id,
          Name = listMetadataRecord.Name,
          Type = listMetadataRecord.Type,
          ConstId = listMetadataRecord.ConstId,
          Items = (IReadOnlyList<WorkItemPickListMemberRecord>) list
        };
      }));
    }

    [Obsolete]
    public override void DeleteList(Guid processId, Guid listId, Guid changerId)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkItemPickLists");
      this.BindGuid("@processId", processId);
      this.BindGuidTable("@listIds", (IEnumerable<Guid>) new Guid[1]
      {
        listId
      });
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQuery();
    }

    [Obsolete]
    public override WorkItemPickListRecord UpdateList(
      Guid processId,
      Guid listId,
      string listName,
      IReadOnlyList<string> newItems,
      Guid changerId,
      bool isSuggested = false)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemPickList");
      this.BindGuid("@processId", processId);
      this.BindGuid("@listId", listId);
      this.BindString("@name", listName, 128, false, SqlDbType.NVarChar);
      this.BindStringTable("@items", (IEnumerable<string>) newItems);
      this.BindGuid("@changerId", changerId);
      return this.ExecuteUnknown<WorkItemPickListRecord>((System.Func<IDataReader, WorkItemPickListRecord>) (reader =>
      {
        if (!reader.Read())
          return (WorkItemPickListRecord) null;
        WorkItemPickListMetadataRecord listMetadataRecord = new WorkItemPickListComponent.WorkItemPickListMetadataBinder().Bind(reader);
        reader.NextResult();
        List<WorkItemPickListMemberRecord> list = new WorkItemPickListComponent.WorkItemPickListMemberBinder().BindAll(reader).ToList<WorkItemPickListMemberRecord>();
        return new WorkItemPickListRecord()
        {
          Id = listMetadataRecord.Id,
          Name = listMetadataRecord.Name,
          Type = listMetadataRecord.Type,
          ConstId = listMetadataRecord.ConstId,
          Items = (IReadOnlyList<WorkItemPickListMemberRecord>) list
        };
      }));
    }
  }
}
