// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent12
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent12 : WorkItemTrackingMetadataComponent10
  {
    public override void PerformDistinctNameMigration()
    {
      if (this.RequestContext.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      this.PrepareStoredProcedure("prc_DistinctNamesMigration");
      this.BindString("@collectionHostId", this.RequestContext.ServiceHost.InstanceId.ToString(), 256, false, SqlDbType.NVarChar);
      this.BindString("@accountHostId", this.RequestContext.ServiceHost.ParentServiceHost.InstanceId.ToString(), 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override IEnumerable<WorkItemTypeEntry> GetWorkItemTypes(
      IEnumerable<int> projectIds,
      bool populateFormEntries,
      bool disableDataspaceRls = false)
    {
      return this._GetWorkItemTypes(projectIds, populateFormEntries, disableDataspaceRls);
    }

    public override IEnumerable<WorkItemTypeEntry> GetWorkItemTypes(IEnumerable<int> projectIds) => this._GetWorkItemTypes(projectIds, true);

    private IEnumerable<WorkItemTypeEntry> _GetWorkItemTypes(
      IEnumerable<int> projectIds,
      bool populateFormEntries,
      bool disableDataspaceRls = false)
    {
      this.DataspaceRlsEnabled = !disableDataspaceRls;
      this.PrepareStoredProcedure("GetWorkItemTypes");
      this.BindUserSid();
      this.BindInt32Table("@projectIds", projectIds);
      this.BindBoolean("@IgnoreUsageVersion", false);
      IEnumerable<WorkItemTypeRecord> workItemTypeRecords;
      IEnumerable<WorkItemTypeUsageRecord> workItemTypeUsageRecords;
      Dictionary<int?, WorkItemTypeEntry>.ValueCollection typeEntries = this.ExecuteUnknown<Dictionary<int?, WorkItemTypeEntry>.ValueCollection>((System.Func<IDataReader, Dictionary<int?, WorkItemTypeEntry>.ValueCollection>) (reader =>
      {
        workItemTypeRecords = (IEnumerable<WorkItemTypeRecord>) new WorkItemTrackingMetadataComponent.WorkItemTypeRecordBinder().BindAll(reader).ToList<WorkItemTypeRecord>();
        reader.NextResult();
        workItemTypeUsageRecords = (IEnumerable<WorkItemTypeUsageRecord>) new WorkItemTrackingMetadataComponent.WorkItemTypeUsageRecordBinder().BindAll(reader).ToList<WorkItemTypeUsageRecord>();
        Dictionary<int?, WorkItemTypeEntry> dictionary = workItemTypeRecords.Select<WorkItemTypeRecord, WorkItemTypeEntry>((System.Func<WorkItemTypeRecord, WorkItemTypeEntry>) (witr => WorkItemTypeEntry.Create(witr))).ToDictionary<WorkItemTypeEntry, int?>((System.Func<WorkItemTypeEntry, int?>) (wite => wite.Id));
        List<WorkItemTypeEntry> list1 = dictionary.Values.Where<WorkItemTypeEntry>((System.Func<WorkItemTypeEntry, bool>) (x =>
        {
          int? id = x.Id;
          int num = 0;
          return id.GetValueOrDefault() > num & id.HasValue;
        })).ToList<WorkItemTypeEntry>();
        List<WorkItemTypeUsageRecord> list2 = workItemTypeUsageRecords.Where<WorkItemTypeUsageRecord>((System.Func<WorkItemTypeUsageRecord, bool>) (x => x.WorkItemTypeId > 0)).ToList<WorkItemTypeUsageRecord>();
        List<WorkItemTypeUsageRecord> list3 = workItemTypeUsageRecords.Where<WorkItemTypeUsageRecord>((System.Func<WorkItemTypeUsageRecord, bool>) (x => x.WorkItemTypeId < 0)).ToList<WorkItemTypeUsageRecord>();
        List<WorkItemTypeUsageRecord> list4 = workItemTypeUsageRecords.Where<WorkItemTypeUsageRecord>((System.Func<WorkItemTypeUsageRecord, bool>) (x => x.WorkItemTypeId == 0)).ToList<WorkItemTypeUsageRecord>();
        foreach (WorkItemTypeUsageRecord itemTypeUsageRecord in list2)
        {
          WorkItemTypeEntry workItemTypeEntry;
          if (dictionary.TryGetValue(new int?(itemTypeUsageRecord.WorkItemTypeId), out workItemTypeEntry))
            workItemTypeEntry.AddField(itemTypeUsageRecord.FieldId);
        }
        foreach (WorkItemTypeUsageRecord itemTypeUsageRecord in list3)
        {
          WorkItemTypeUsageRecord usageRecord = itemTypeUsageRecord;
          WorkItemTypeEntry workItemTypeEntry1;
          if (dictionary.TryGetValue(new int?(usageRecord.WorkItemTypeId), out workItemTypeEntry1))
            workItemTypeEntry1.AddField(usageRecord.FieldId, FieldSource.ProjectGlobalWorkflow);
          foreach (WorkItemTypeEntry workItemTypeEntry2 in list1.Where<WorkItemTypeEntry>((System.Func<WorkItemTypeEntry, bool>) (x => x.ProjectId == usageRecord.ProjectId)))
            workItemTypeEntry2.AddField(usageRecord.FieldId, FieldSource.ProjectGlobalWorkflow);
        }
        foreach (WorkItemTypeUsageRecord itemTypeUsageRecord in list4)
        {
          WorkItemTypeEntry workItemTypeEntry3;
          if (dictionary.TryGetValue(new int?(itemTypeUsageRecord.WorkItemTypeId), out workItemTypeEntry3))
            workItemTypeEntry3.AddField(itemTypeUsageRecord.FieldId, FieldSource.CollectionGlobalWorkflow);
          foreach (WorkItemTypeEntry workItemTypeEntry4 in list1.Where<WorkItemTypeEntry>((System.Func<WorkItemTypeEntry, bool>) (x =>
          {
            int? id = x.Id;
            int num = 0;
            return id.GetValueOrDefault() > num & id.HasValue;
          })))
            workItemTypeEntry4.AddField(itemTypeUsageRecord.FieldId, FieldSource.CollectionGlobalWorkflow);
        }
        return dictionary.Values;
      }));
      if (populateFormEntries)
        this.PopulateFormEntries(projectIds, (IEnumerable<WorkItemTypeEntry>) typeEntries);
      return (IEnumerable<WorkItemTypeEntry>) typeEntries;
    }

    internal virtual void PopulateFormEntries(
      IEnumerable<int> projectIds,
      IEnumerable<WorkItemTypeEntry> typeEntries)
    {
      this.PrepareDynamicProcedure("dynprc_GetWorkItemTypeFormEntries", "\r\n            SELECT P.Val AS ProjectId\r\n                , WIT.WorkItemTypeID AS TypeId\r\n                , TP.Value AS Form\r\n            FROM dbo.Rules R\r\n            JOIN @projectIds P\r\n                ON P.Val = R.RootTreeID\r\n            JOIN dbo.WorkItemTypes WIT\r\n                ON WIT.PartitionId = R.PartitionID AND WIT.NameConstantID = R.IfConstID AND WIT.ProjectID = R.RootTreeID\r\n            JOIN dbo.TreeProperties TP\r\n                ON TP.PartitionId = R.PartitionId AND TP.PropID = R.ThenConstID\r\n            WHERE   R.[fDeleted] = 0\r\n                AND R.VersionTo = 255 --PS_DB_ClientVersion_Max\r\n                AND R.[fAcl] = 0\r\n                AND R.[Fld1ID] = 0\r\n                AND R.[Fld1IsConstID] = 0\r\n                AND R.[ThenFldID] = -14 --PS_DB_FLD_FormID_ID\r\n                AND R.ThenConstID > 0\r\n                AND R.PartitionId = @partitionId\r\n            OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n            ");
      this.BindInt32Table("@projectIds", projectIds);
      ILookup<Tuple<int, int>, WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord> lookup = this.ExecuteUnknown<List<WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord>>((System.Func<IDataReader, List<WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord>>) (reader => new WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleBinder().BindAll(reader).ToList<WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord>())).ToLookup<WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord, Tuple<int, int>>((System.Func<WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord, Tuple<int, int>>) (fe => new Tuple<int, int>(fe.ProjectId, fe.WorkItemTypeId)));
      foreach (WorkItemTypeEntry workItemTypeEntry in typeEntries.Where<WorkItemTypeEntry>((System.Func<WorkItemTypeEntry, bool>) (t => t.Id.HasValue)))
      {
        IEnumerable<WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord> source = lookup[new Tuple<int, int>(workItemTypeEntry.ProjectId, workItemTypeEntry.Id.Value)];
        WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord typeFormRuleRecord = source != null ? source.FirstOrDefault<WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord>() : (WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord) null;
        if (typeFormRuleRecord != null)
          workItemTypeEntry.Form = typeFormRuleRecord.Form;
      }
    }

    public override IEnumerable<ConstantsSearchRecord> SearchConstantsRecords(
      IEnumerable<string> searchValues,
      IEnumerable<Guid> tfIds,
      bool includeInactiveIdentities,
      bool isHostedDeployment)
    {
      IEnumerable<string> rows = searchValues.Distinct<string>();
      this.PrepareStoredProcedure("prc_SearchConstantsNames");
      this.BindStringTable("@personNames", rows);
      this.BindBoolean("@includeInactiveIdentities", includeInactiveIdentities);
      return this.ExecuteUnknown<IEnumerable<ConstantsSearchRecord>>((System.Func<IDataReader, IEnumerable<ConstantsSearchRecord>>) (reader => (IEnumerable<ConstantsSearchRecord>) this.GetSearchConstantRecordBinder().BindAll(reader).ToList<ConstantsSearchRecord>()));
    }

    public override IEnumerable<IdentityConstantRecord> SearchConstantIdentityRecords(
      string searchTerm,
      SearchIdentityType identityType = SearchIdentityType.All)
    {
      this.PrepareStoredProcedure("prc_SearchConstantIdentityRecords");
      this.BindString("@searchTerm", searchTerm, 256, false, SqlDbType.NVarChar);
      this.BindInt("@identityType", (int) identityType);
      return (IEnumerable<IdentityConstantRecord>) this.ExecuteUnknown<List<IdentityConstantRecord>>((System.Func<IDataReader, List<IdentityConstantRecord>>) (reader => new WorkItemTrackingMetadataComponent.IdentityConstantRecordBinder().BindAll(reader).ToList<IdentityConstantRecord>()));
    }

    internal override IEnumerable<WorkItemTypeCategoryRecord> GetWorkItemTypeCategories(
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeCategories");
      this.BindUserSid();
      this.BindGuid("@projectCssNodeId", projectId);
      this.BindStringTable("@witCategoryRefNames", (IEnumerable<string>) null);
      return (IEnumerable<WorkItemTypeCategoryRecord>) this.ExecuteUnknown<List<WorkItemTypeCategoryRecord>>((System.Func<IDataReader, List<WorkItemTypeCategoryRecord>>) (reader =>
      {
        List<WorkItemTypeCategoryRecord> list1 = new WorkItemTrackingMetadataComponent12.WorkItemTypeCategoryRecordBinder2().BindAll(reader).ToList<WorkItemTypeCategoryRecord>();
        reader.NextResult();
        IEnumerable<WorkItemTypeCategoryMemberRecord> list2 = (IEnumerable<WorkItemTypeCategoryMemberRecord>) new WorkItemTrackingMetadataComponent12.WorkItemTypeCategoryMemberRecordBinder().BindAll(reader).ToList<WorkItemTypeCategoryMemberRecord>();
        foreach (WorkItemTypeCategoryRecord typeCategoryRecord in list1)
        {
          WorkItemTypeCategoryRecord workItemTypeCategoriesRecord = typeCategoryRecord;
          workItemTypeCategoriesRecord.WorkItemTypeMembers = list2.Where<WorkItemTypeCategoryMemberRecord>((System.Func<WorkItemTypeCategoryMemberRecord, bool>) (m => m.WorkItemTypeCategoryId == workItemTypeCategoriesRecord.Id));
        }
        return list1;
      }));
    }

    protected class WorkItemTypeCategoryRecordBinder2 : 
      WorkItemTrackingMetadataComponent.WorkItemTypeCategoryRecordBinder
    {
      private SqlColumnBinder IdColumn = new SqlColumnBinder("WorkItemTypeCategoryID");

      public override WorkItemTypeCategoryRecord Bind(IDataReader reader) => new WorkItemTypeCategoryRecord()
      {
        Id = this.IdColumn.GetInt32(reader),
        Name = this.NameColumn.GetString(reader, false),
        DefaultWorkItemTypeName = this.DefaultWorkItemTypeNameColumn.GetString(reader, false),
        ReferenceName = this.ReferenceNameColumn.GetString(reader, false)
      };
    }

    protected class WorkItemTypeCategoryMemberRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemTypeCategoryMemberRecord>
    {
      protected SqlColumnBinder WorkItemTypeCategoryIDColumn = new SqlColumnBinder("WorkItemTypeCategoryID");
      protected SqlColumnBinder WorkItemTypeCategoryMemberIDColumn = new SqlColumnBinder("WorkItemTypeCategoryMemberID");
      protected SqlColumnBinder WorkItemTypeNameColumn = new SqlColumnBinder("WorkItemTypeName");

      public override WorkItemTypeCategoryMemberRecord Bind(IDataReader reader) => new WorkItemTypeCategoryMemberRecord()
      {
        Id = this.WorkItemTypeCategoryMemberIDColumn.GetInt32(reader),
        WorkItemTypeCategoryId = this.WorkItemTypeCategoryIDColumn.GetInt32(reader),
        WorkItemTypeName = this.WorkItemTypeNameColumn.GetString(reader, false)
      };
    }

    protected class WorkItemTypeFormRuleRecord
    {
      public int ProjectId { get; set; }

      public int WorkItemTypeId { get; set; }

      public string Form { get; set; }
    }

    protected class WorkItemTypeFormRuleBinder : 
      WorkItemTrackingObjectBinder<WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord>
    {
      protected SqlColumnBinder ProjectIdColumn = new SqlColumnBinder("ProjectId");
      protected SqlColumnBinder TypeIdColumn = new SqlColumnBinder("TypeId");
      protected SqlColumnBinder FormColumn = new SqlColumnBinder("Form");

      public override WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord Bind(
        IDataReader reader)
      {
        return new WorkItemTrackingMetadataComponent12.WorkItemTypeFormRuleRecord()
        {
          ProjectId = this.ProjectIdColumn.GetInt32(reader),
          WorkItemTypeId = this.TypeIdColumn.GetInt32(reader),
          Form = this.FormColumn.GetString(reader, false)
        };
      }
    }
  }
}
