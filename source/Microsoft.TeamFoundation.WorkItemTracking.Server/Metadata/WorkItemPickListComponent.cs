// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemPickListComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemPickListComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<WorkItemPickListComponent>(1),
      (IComponentCreator) new ComponentCreator<WorkItemPickListComponent2>(2),
      (IComponentCreator) new ComponentCreator<WorkItemPickListComponent3>(3),
      (IComponentCreator) new ComponentCreator<WorkItemPickListComponent4>(4)
    }, "ProcessPickList", "WorkItem");
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => WorkItemPickListComponent.s_sqlExceptionFactories;

    static WorkItemPickListComponent()
    {
      WorkItemPickListComponent.s_sqlExceptionFactories[600183] = WorkItemTrackingResourceComponent.CreateFactory<WorkItemPickListItemNameAlreadyInUseException>();
      WorkItemPickListComponent.s_sqlExceptionFactories[600187] = WorkItemTrackingResourceComponent.CreateFactory<CannotDeletePicklistReferencedByFieldException>();
      WorkItemPickListComponent.s_sqlExceptionFactories[600177] = new SqlExceptionFactory(typeof (ArgumentException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemStringInvalidException())));
    }

    [Obsolete]
    public virtual WorkItemPickListRecord GetList(Guid processId, Guid listId)
    {
      this.PrepareStoredProcedure("prc_GetProcessList");
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
    internal virtual IReadOnlyCollection<WorkItemPickListRecord> GetLists(Guid processId)
    {
      IReadOnlyCollection<WorkItemPickListMetadataRecord> listsMetadata = this.GetListsMetadata(processId);
      List<WorkItemPickListRecord> lists = new List<WorkItemPickListRecord>();
      foreach (WorkItemPickListMetadataRecord listMetadataRecord in (IEnumerable<WorkItemPickListMetadataRecord>) listsMetadata)
      {
        WorkItemPickListRecord list = this.GetList(processId, listMetadataRecord.Id);
        if (list != null)
          lists.Add(list);
      }
      return (IReadOnlyCollection<WorkItemPickListRecord>) lists;
    }

    [Obsolete]
    public virtual IReadOnlyCollection<WorkItemPickListMetadataRecord> GetListsMetadata(
      Guid processId)
    {
      this.PrepareStoredProcedure("prc_GetProcessListMetadata");
      this.BindGuid("@processId", processId);
      return this.ExecuteUnknown<IReadOnlyCollection<WorkItemPickListMetadataRecord>>((System.Func<IDataReader, IReadOnlyCollection<WorkItemPickListMetadataRecord>>) (reader => (IReadOnlyCollection<WorkItemPickListMetadataRecord>) new WorkItemPickListComponent.WorkItemPickListMetadataBinder().BindAll(reader).ToList<WorkItemPickListMetadataRecord>()));
    }

    [Obsolete]
    public virtual WorkItemPickListRecord CreateList(
      Guid processId,
      string name,
      WorkItemPickListType type,
      IReadOnlyList<string> items,
      Guid changerId,
      bool isSuggested = false)
    {
      this.PrepareStoredProcedure("prc_CreateProcessList");
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
    public virtual void DeleteList(Guid processId, Guid listId, Guid changerId)
    {
      this.PrepareStoredProcedure("prc_DeleteProcessLists");
      this.BindGuid("@processId", processId);
      this.BindGuidTable("@listIds", (IEnumerable<Guid>) new Guid[1]
      {
        listId
      });
      this.BindGuid("@changerId", changerId);
      this.ExecuteNonQuery();
    }

    [Obsolete]
    public virtual WorkItemPickListRecord UpdateList(
      Guid processId,
      Guid listId,
      string listName,
      IReadOnlyList<string> newItems,
      Guid changerId,
      bool isSuggested = false)
    {
      throw new NotImplementedException();
    }

    public virtual WorkItemPickListRecord GetList(Guid listId) => throw new NotImplementedException();

    internal virtual IReadOnlyCollection<WorkItemPickListRecord> GetLists() => throw new NotImplementedException();

    public virtual IReadOnlyCollection<WorkItemPickListMetadataRecord> GetListsMetadata() => throw new NotImplementedException();

    public virtual WorkItemPickListRecord CreateList(
      string name,
      WorkItemPickListType type,
      IReadOnlyList<string> items,
      Guid changerId,
      bool isSuggested = false)
    {
      throw new NotImplementedException();
    }

    public virtual void DeleteList(Guid listId, Guid changerId) => throw new NotImplementedException();

    public virtual WorkItemPickListRecord UpdateList(
      Guid listId,
      string listName,
      IReadOnlyList<string> newItems,
      Guid changerId,
      bool isSuggested = false)
    {
      throw new NotImplementedException();
    }

    protected class WorkItemPickListMetadataBinder : 
      WorkItemTrackingObjectBinder<WorkItemPickListMetadataRecord>
    {
      protected SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      protected SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      protected SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));
      protected SqlColumnBinder ConstId = new SqlColumnBinder(nameof (ConstId));

      public override WorkItemPickListMetadataRecord Bind(IDataReader reader) => new WorkItemPickListMetadataRecord()
      {
        Id = this.Id.GetGuid(reader),
        Name = this.Name.GetString(reader, false),
        Type = this.Type.GetInt32(reader),
        ConstId = this.ConstId.GetInt32(reader)
      };
    }

    protected class WorkItemPickListMemberBinder : 
      WorkItemTrackingObjectBinder<WorkItemPickListMemberRecord>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder ListId = new SqlColumnBinder(nameof (ListId));
      private SqlColumnBinder Value = new SqlColumnBinder(nameof (Value));
      private SqlColumnBinder ConstId = new SqlColumnBinder(nameof (ConstId));

      public override WorkItemPickListMemberRecord Bind(IDataReader reader) => new WorkItemPickListMemberRecord()
      {
        Id = this.Id.GetGuid(reader),
        ListId = this.ListId.GetGuid(reader),
        Value = this.Value.GetString(reader, false),
        ConstId = this.ConstId.GetInt32(reader)
      };
    }
  }
}
