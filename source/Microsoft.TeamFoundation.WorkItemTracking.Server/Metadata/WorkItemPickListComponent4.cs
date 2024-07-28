// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemPickListComponent4
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
  internal class WorkItemPickListComponent4 : WorkItemPickListComponent3
  {
    [Obsolete]
    public override WorkItemPickListRecord GetList(Guid processId, Guid listId) => this.GetList(listId);

    [Obsolete]
    internal override IReadOnlyCollection<WorkItemPickListRecord> GetLists(Guid processId) => this.GetLists(processId);

    [Obsolete]
    public override IReadOnlyCollection<WorkItemPickListMetadataRecord> GetListsMetadata(
      Guid processId)
    {
      return this.GetListsMetadata();
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
      return this.CreateList(name, type, items, changerId, isSuggested);
    }

    [Obsolete]
    public override void DeleteList(Guid processId, Guid listId, Guid changerId) => this.DeleteList(listId, changerId);

    [Obsolete]
    public override WorkItemPickListRecord UpdateList(
      Guid processId,
      Guid listId,
      string listName,
      IReadOnlyList<string> newItems,
      Guid changerId,
      bool isSuggested = false)
    {
      return this.UpdateList(listId, listName, newItems, changerId, isSuggested);
    }

    public override WorkItemPickListRecord GetList(Guid listId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemPickList");
      this.BindGuid("@listId", listId);
      return this.ExecuteUnknown<WorkItemPickListRecord>((System.Func<IDataReader, WorkItemPickListRecord>) (reader =>
      {
        if (!reader.Read())
          return (WorkItemPickListRecord) null;
        WorkItemPickListMetadataRecord listMetadataRecord = new WorkItemPickListComponent4.WorkItemPickListMetadataBinder2().Bind(reader);
        reader.NextResult();
        List<WorkItemPickListMemberRecord> list = new WorkItemPickListComponent.WorkItemPickListMemberBinder().BindAll(reader).ToList<WorkItemPickListMemberRecord>();
        return new WorkItemPickListRecord()
        {
          Id = listMetadataRecord.Id,
          Name = listMetadataRecord.Name,
          Type = listMetadataRecord.Type,
          ConstId = listMetadataRecord.ConstId,
          IsSuggested = listMetadataRecord.IsSuggested,
          Items = (IReadOnlyList<WorkItemPickListMemberRecord>) list
        };
      }));
    }

    internal override IReadOnlyCollection<WorkItemPickListRecord> GetLists()
    {
      this.PrepareStoredProcedure("prc_GetWorkItemPickLists");
      return this.ExecuteUnknown<IReadOnlyCollection<WorkItemPickListRecord>>((System.Func<IDataReader, IReadOnlyCollection<WorkItemPickListRecord>>) (reader =>
      {
        List<WorkItemPickListMetadataRecord> list = new WorkItemPickListComponent4.WorkItemPickListMetadataBinder2().BindAll(reader).ToList<WorkItemPickListMetadataRecord>();
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
            IsSuggested = m.IsSuggested,
            Items = (IReadOnlyList<WorkItemPickListMemberRecord>) listMemberDictionary[m.Id]
          };
        })).ToList<WorkItemPickListRecord>();
      }));
    }

    public override IReadOnlyCollection<WorkItemPickListMetadataRecord> GetListsMetadata()
    {
      this.PrepareStoredProcedure("prc_GetWorkItemPickListMetadata");
      return this.ExecuteUnknown<IReadOnlyCollection<WorkItemPickListMetadataRecord>>((System.Func<IDataReader, IReadOnlyCollection<WorkItemPickListMetadataRecord>>) (reader => (IReadOnlyCollection<WorkItemPickListMetadataRecord>) new WorkItemPickListComponent4.WorkItemPickListMetadataBinder2().BindAll(reader).ToList<WorkItemPickListMetadataRecord>()));
    }

    public override WorkItemPickListRecord CreateList(
      string name,
      WorkItemPickListType type,
      IReadOnlyList<string> items,
      Guid changerId,
      bool isSuggested = false)
    {
      this.PrepareStoredProcedure("prc_CreateWorkItemPickList");
      this.BindString("@name", name, 128, false, SqlDbType.NVarChar);
      this.BindInt("@type", (int) type);
      this.BindStringTable("@items", (IEnumerable<string>) items);
      this.BindBoolean("@isSuggested", isSuggested);
      this.BindGuid("@changerId", changerId);
      return this.ExecuteUnknown<WorkItemPickListRecord>((System.Func<IDataReader, WorkItemPickListRecord>) (reader =>
      {
        if (!reader.Read())
          return (WorkItemPickListRecord) null;
        WorkItemPickListMetadataRecord listMetadataRecord = new WorkItemPickListComponent4.WorkItemPickListMetadataBinder2().Bind(reader);
        reader.NextResult();
        List<WorkItemPickListMemberRecord> list = new WorkItemPickListComponent.WorkItemPickListMemberBinder().BindAll(reader).ToList<WorkItemPickListMemberRecord>();
        return new WorkItemPickListRecord()
        {
          Id = listMetadataRecord.Id,
          Name = listMetadataRecord.Name,
          Type = listMetadataRecord.Type,
          ConstId = listMetadataRecord.ConstId,
          IsSuggested = listMetadataRecord.IsSuggested,
          Items = (IReadOnlyList<WorkItemPickListMemberRecord>) list
        };
      }));
    }

    public override void DeleteList(Guid listId, Guid changerId)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkItemPickLists");
      this.BindGuidTable("@listIds", (IEnumerable<Guid>) new Guid[1]
      {
        listId
      });
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQuery();
    }

    public override WorkItemPickListRecord UpdateList(
      Guid listId,
      string listName,
      IReadOnlyList<string> newItems,
      Guid changerId,
      bool isSuggested = false)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemPickList");
      this.BindGuid("@listId", listId);
      this.BindString("@name", listName, 128, false, SqlDbType.NVarChar);
      this.BindStringTable("@items", (IEnumerable<string>) newItems);
      this.BindBoolean("@isSuggested", isSuggested);
      this.BindGuid("@changerId", changerId);
      return this.ExecuteUnknown<WorkItemPickListRecord>((System.Func<IDataReader, WorkItemPickListRecord>) (reader =>
      {
        if (!reader.Read())
          return (WorkItemPickListRecord) null;
        WorkItemPickListMetadataRecord listMetadataRecord = new WorkItemPickListComponent4.WorkItemPickListMetadataBinder2().Bind(reader);
        reader.NextResult();
        List<WorkItemPickListMemberRecord> list = new WorkItemPickListComponent.WorkItemPickListMemberBinder().BindAll(reader).ToList<WorkItemPickListMemberRecord>();
        return new WorkItemPickListRecord()
        {
          Id = listMetadataRecord.Id,
          Name = listMetadataRecord.Name,
          Type = listMetadataRecord.Type,
          ConstId = listMetadataRecord.ConstId,
          IsSuggested = listMetadataRecord.IsSuggested,
          Items = (IReadOnlyList<WorkItemPickListMemberRecord>) list
        };
      }));
    }

    protected class WorkItemPickListMetadataBinder2 : 
      WorkItemPickListComponent.WorkItemPickListMetadataBinder
    {
      protected SqlColumnBinder IsSuggested = new SqlColumnBinder(nameof (IsSuggested));

      public override WorkItemPickListMetadataRecord Bind(IDataReader reader) => new WorkItemPickListMetadataRecord()
      {
        Id = this.Id.GetGuid(reader),
        Name = this.Name.GetString(reader, false),
        Type = this.Type.GetInt32(reader),
        ConstId = this.ConstId.GetInt32(reader),
        IsSuggested = this.IsSuggested.GetBoolean(reader)
      };
    }
  }
}
