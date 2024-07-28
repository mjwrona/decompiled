// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent : WorkItemTrackingResourceComponent
  {
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[50]
    {
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent>(7),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent8>(8),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent9>(9),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent10>(10),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent10>(11),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent12>(12),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent13>(13),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent14>(14),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent15>(15),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent16>(16),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent17>(17),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent18>(18),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent19>(19),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent20>(20),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent21>(21),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent22>(22),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent23>(23),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent24>(24),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent25>(25),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent26>(26),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent27>(27),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent28>(28),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent29>(29),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent30>(30),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent31>(31),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent32>(32),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent33>(33),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent34>(34),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent35>(35),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent36>(36),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent37>(37),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent38>(38),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent39>(39),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent40>(40),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent41>(41),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent42>(42),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent43>(43),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent44>(44),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent45>(45),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent46>(46),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent47>(47),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent48>(48),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent49>(49),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent50>(50),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent51>(51),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent52>(52),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent53>(53),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent54>(54),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent55>(55),
      (IComponentCreator) new ComponentCreator<WorkItemTrackingMetadataComponent56>(56)
    }, "WorkItemMetadata", "WorkItem");

    internal virtual SqlParameter BindConstantSetReferenceTable(
      string parameterName,
      IEnumerable<ConstantSetReference> rows)
    {
      return this.BindBasicTvp<ConstantSetReference>((WorkItemTrackingResourceComponent.TvpRecordBinder<ConstantSetReference>) new WorkItemTrackingMetadataComponent.ConstantSetReferenceTableRecordBinder(), parameterName, rows);
    }

    internal virtual SqlParameter BindWitSyncIdentityTable(
      string parameterName,
      IEnumerable<IVssIdentity> identities)
    {
      return this.BindBasicTvp<IVssIdentity>((WorkItemTrackingResourceComponent.TvpRecordBinder<IVssIdentity>) new WorkItemTrackingMetadataComponent.WitSyncIdentityTable2RecordBinder(), parameterName, identities);
    }

    internal virtual SqlParameter BindWitSyncMembershipTable(
      string parameterName,
      IEnumerable<(Guid UserVsid, string GroupSid)> memberships)
    {
      return this.BindBasicTvp<(Guid, string)>((WorkItemTrackingResourceComponent.TvpRecordBinder<(Guid, string)>) new WorkItemTrackingMetadataComponent.WitSyncMembershipTable2RecordBinder(), parameterName, memberships);
    }

    internal SqlParameter BindRuleSetRecordTable(
      string parameterName,
      IEnumerable<RuleSetRecord> ruleSetRecords)
    {
      return this.BindBasicTvp<RuleSetRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<RuleSetRecord>) new WorkItemTrackingMetadataComponent.RuleSetRecordTableRecordBinder(), parameterName, ruleSetRecords);
    }

    internal SqlParameter BindTempIdMapTable(
      string parameterName,
      IEnumerable<TempIdRecord> tempIdRecords)
    {
      return this.BindBasicTvp<TempIdRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<TempIdRecord>) new WorkItemTrackingMetadataComponent.TempIdMapTableRecordBinder(), parameterName, tempIdRecords);
    }

    internal virtual SqlParameter BindWitOrderedStringTable(
      string parameterName,
      IEnumerable<OrderedString> witOrderedStrings)
    {
      return this.BindBasicTvp<OrderedString>((WorkItemTrackingResourceComponent.TvpRecordBinder<OrderedString>) new WorkItemTrackingMetadataComponent.OrderedStringTableRecordBinder(), parameterName, witOrderedStrings);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => WorkItemTrackingMetadataComponent.s_sqlExceptionFactories;

    static WorkItemTrackingMetadataComponent()
    {
      WorkItemTrackingMetadataComponent.s_sqlExceptionFactories[600305] = WorkItemTrackingResourceComponent.CreateFactory<WorkItemTypeNameAlreadyExistsException>();
      WorkItemTrackingMetadataComponent.s_sqlExceptionFactories[600158] = WorkItemTrackingResourceComponent.CreateFactory<ProcessFieldAlreadyExistsException>();
      WorkItemTrackingMetadataComponent.s_sqlExceptionFactories[600159] = WorkItemTrackingResourceComponent.CreateFactory<ProcessFieldAlreadyExistsException>();
      WorkItemTrackingMetadataComponent.s_sqlExceptionFactories[600177] = new SqlExceptionFactory(typeof (ArgumentException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemStringInvalidException())));
      WorkItemTrackingMetadataComponent.s_sqlExceptionFactories[600031] = new SqlExceptionFactory(typeof (ArgumentException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ObjectDoesNotExistOrAccessIsDenied())));
      WorkItemTrackingMetadataComponent.s_sqlExceptionFactories[600129] = new SqlExceptionFactory(typeof (ProcessFieldUsedByProjectException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new ProcessFieldUsedByProjectException()));
    }

    protected virtual void BindUserSid()
    {
      string empty = string.Empty;
      this.BindString("@userSID", !this.RequestContext.IsUserContext ? GroupWellKnownSidConstants.NamespaceAdministratorsGroupSid : this.RequestContext.UserContext.Identifier, 256, false, SqlDbType.NVarChar);
    }

    protected virtual WorkItemTrackingMetadataComponent.ConstantRecordBinder GetConstantRecordBinder(
      bool backCompat = false)
    {
      return new WorkItemTrackingMetadataComponent.ConstantRecordBinder(backCompat);
    }

    protected virtual WorkItemTrackingMetadataComponent.SearchConstantRecordBinder GetSearchConstantRecordBinder() => new WorkItemTrackingMetadataComponent.SearchConstantRecordBinder();

    protected virtual WorkItemTrackingMetadataComponent.FieldRecordBinder GetFieldRecordBinder() => new WorkItemTrackingMetadataComponent.FieldRecordBinder();

    protected virtual WorkItemTrackingMetadataComponent.RuleRecordBinder GetRuleRecordBinder() => new WorkItemTrackingMetadataComponent.RuleRecordBinder();

    public virtual List<FieldRecord> GetFieldRecords(bool disableDataspaceRls = false)
    {
      this.DataspaceRlsEnabled = !disableDataspaceRls;
      this.PrepareStoredProcedure("GetFieldEntries");
      return this.ExecuteUnknown<List<FieldRecord>>((System.Func<IDataReader, List<FieldRecord>>) (reader => this.GetFieldRecordBinder().BindAll(reader).ToList<FieldRecord>()));
    }

    public virtual List<WorkItemLinkTypeRecord> GetLinkTypes(bool disableDataspaceRls = false)
    {
      this.DataspaceRlsEnabled = !disableDataspaceRls;
      this.PrepareStoredProcedure("GetWorkItemLinkTypesForCache");
      return this.ExecuteUnknown<List<WorkItemLinkTypeRecord>>((System.Func<IDataReader, List<WorkItemLinkTypeRecord>>) (reader => new WorkItemTrackingMetadataComponent.LinkTypeBinder().BindAll(reader).ToList<WorkItemLinkTypeRecord>()));
    }

    protected virtual bool BindPartitionIdForInstallWorkItemWordsContains => false;

    public virtual bool InstallWorkItemWordsContains()
    {
      try
      {
        this.PrepareStoredProcedure(nameof (InstallWorkItemWordsContains), this.BindPartitionIdForInstallWorkItemWordsContains);
        return (bool) this.ExecuteScalarEx();
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(908515, "ResourceComponent", "WorkItemTrackingResourceComponent", ex);
        return false;
      }
    }

    public DatabaseCollationInfo GetCollationInfo()
    {
      this.PrepareDynamicProcedure("dynprc_GetCollationInfo", "\r\n            DECLARE @collation AS SYSNAME\r\n            SET @collation = CAST(DATABASEPROPERTYEX(DB_NAME(), 'collation') AS SYSNAME)\r\n\r\n            SELECT CAST(collationproperty(@collation,'LCID') AS INT) AS LocaleId,\r\n                   CAST(collationproperty(@collation,'ComparisonStyle') AS INT) AS ComparisonStyle,\r\n                   CAST(collationproperty(@collation,'CodePage') AS INT) AS CodePage,\r\n                   CAST(collationproperty(@collation,'Version') AS INT) AS Version\r\n            ", false);
      return this.ExecuteUnknown<DatabaseCollationInfo>((System.Func<IDataReader, DatabaseCollationInfo>) (reader =>
      {
        reader.Read();
        return new DatabaseCollationInfo()
        {
          LocaleId = reader.GetInt32(0),
          ComparisonStyle = reader.GetInt32(1),
          CodePage = reader.GetInt32(2),
          Version = reader.GetInt32(3)
        };
      }));
    }

    protected virtual void BindTypeletParameter(ISet<MetadataTable> tableNames)
    {
    }

    protected virtual void BindSplitConstantsAndSets(ISet<MetadataTable> tableNames)
    {
    }

    protected virtual void BindIncludeDeletedForFields(bool includeDeleted)
    {
    }

    public virtual IDictionary<MetadataTable, long> GetMetadataTableTimestamps(
      ISet<MetadataTable> tableNames,
      int projectId)
    {
      this.DataspaceRlsEnabled = false;
      this.PrepareStoredProcedure("GetMetadataTimestamps");
      this.BindUserSid();
      this.BindInt("@projectID", projectId);
      this.BindBoolean("@Hierarchy", tableNames.Contains(MetadataTable.Hierarchy));
      this.BindBoolean("@Fields", tableNames.Contains(MetadataTable.Fields));
      this.BindBoolean("@HierarchyProperties", tableNames.Contains(MetadataTable.HierarchyProperties));
      this.BindBoolean("@Constants", tableNames.Contains(MetadataTable.Constants));
      this.BindBoolean("@Rules", tableNames.Contains(MetadataTable.Rules));
      this.BindBoolean("@ConstantSets", tableNames.Contains(MetadataTable.ConstantSets));
      this.BindBoolean("@FieldUsages", tableNames.Contains(MetadataTable.FieldUsages));
      this.BindBoolean("@WorkItemTypes", tableNames.Contains(MetadataTable.WorkItemTypes));
      this.BindBoolean("@WorkItemTypeUsages", tableNames.Contains(MetadataTable.WorkItemTypeUsages));
      this.BindBoolean("@Actions", tableNames.Contains(MetadataTable.Actions));
      this.BindBoolean("@LinkTypes", tableNames.Contains(MetadataTable.LinkTypes));
      this.BindBoolean("@WorkItemTypeCategories", tableNames.Contains(MetadataTable.WorkItemTypeCategories));
      this.BindBoolean("@WorkItemTypeCategoryMembers", tableNames.Contains(MetadataTable.WorkItemTypeCategoryMembers));
      this.BindTypeletParameter(tableNames);
      this.BindSplitConstantsAndSets(tableNames);
      return (IDictionary<MetadataTable, long>) this.ExecuteUnknown<Dictionary<MetadataTable, long>>((System.Func<IDataReader, Dictionary<MetadataTable, long>>) (reader =>
      {
        Dictionary<MetadataTable, long> metadataTableTimestamps = new Dictionary<MetadataTable, long>(16);
        do
        {
          while (reader.Read())
          {
            string str = reader.GetString(0);
            long int64 = !reader.IsDBNull(1) ? reader.GetInt64(1) : 0L;
            MetadataTable key = (MetadataTable) Enum.Parse(typeof (MetadataTable), str, true);
            metadataTableTimestamps[key] = int64;
          }
        }
        while (reader.NextResult());
        return metadataTableTimestamps;
      }));
    }

    public virtual IEnumerable<RuleRecord> GetRules(int projectId, string workItemTypeName)
    {
      this.PrepareStoredProcedure(nameof (GetRules));
      this.BindUserSid();
      this.BindInt("@projectId", projectId);
      this.BindString("@workItemTypeName", workItemTypeName, 256, false, SqlDbType.NVarChar);
      return (IEnumerable<RuleRecord>) this.ExecuteUnknown<List<RuleRecord>>((System.Func<IDataReader, List<RuleRecord>>) (reader => this.GetRuleRecordBinder().BindAll(reader).ToList<RuleRecord>()));
    }

    protected static IDictionary<ConstantSetReference, SetRecord[]> GroupSetRecords(
      IEnumerable<SetRecord> sets)
    {
      return (IDictionary<ConstantSetReference, SetRecord[]>) sets.GroupBy<SetRecord, long>((System.Func<SetRecord, long>) (sr => sr.SetHandle)).Select(g =>
      {
        SetRecord setRecord = g.First<SetRecord>();
        return new
        {
          Key = new ConstantSetReference()
          {
            Id = setRecord.ParentId,
            Direct = setRecord.Direct,
            IncludeTop = setRecord.IncludeTop,
            ExcludeGroups = !setRecord.IncludeGroups
          },
          Values = g.OrderBy<SetRecord, string>((System.Func<SetRecord, string>) (sr => sr.Item), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToArray<SetRecord>()
        };
      }).ToDictionary(r => r.Key, r => r.Values);
    }

    internal virtual IDictionary<ConstantSetReference, SetRecord[]> GetConstantSets(
      IEnumerable<ConstantSetReference> setReferences)
    {
      this.PrepareStoredProcedure(nameof (GetConstantSets));
      this.BindUserSid();
      this.BindConstantSetReferenceTable("@sets", setReferences.Distinct<ConstantSetReference>());
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<SetRecord>((ObjectBinder<SetRecord>) new WorkItemTrackingMetadataComponent.SetRecordBinder());
      return WorkItemTrackingMetadataComponent.GroupSetRecords((IEnumerable<SetRecord>) resultCollection.GetCurrent<SetRecord>().Items);
    }

    internal virtual IDictionary<ConstantSetReference, SetRecord[]> GetDirectConstantSets(
      IEnumerable<ConstantSetReference> setReferences)
    {
      return this.GetConstantSets(setReferences);
    }

    public virtual IEnumerable<ConstantRecord> GetConstantRecords(
      IEnumerable<int> constantIds,
      bool includeInactiveConstants = false)
    {
      IEnumerable<int> rows = constantIds.Distinct<int>();
      this.PrepareDynamicProcedure("dynprc_GetConstantRecords", "\r\n            SELECT  C.ConstID,\r\n                    C.DomainPart,\r\n                    C.NamePart,\r\n                    C.DisplayPart,\r\n                    C.TeamFoundationId\r\n            FROM dbo.Constants C\r\n            JOIN @constantIds Ids\r\n                ON Ids.Val = C.ConstID\r\n            WHERE C.PartitionId = @partitionId\r\n                AND (C.RemovedDate = CONVERT(DATETIME, '9999', 126) OR C.RemovedDate = CONVERT(DATETIME, '1900', 126))\r\n            OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n            ");
      this.BindInt32Table("@constantIds", rows);
      return (IEnumerable<ConstantRecord>) this.ExecuteUnknown<List<ConstantRecord>>((System.Func<IDataReader, List<ConstantRecord>>) (reader => this.GetConstantRecordBinder().BindAll(reader).ToList<ConstantRecord>()));
    }

    public virtual IEnumerable<IdentityConstantRecord> SearchConstantIdentityRecords(
      string searchTerm,
      SearchIdentityType identityType = SearchIdentityType.All)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<PersonNameConstantRecord> GetConstantRecordsFromPersonNames(
      IEnumerable<string> personNames)
    {
      IEnumerable<string> rows = personNames.Distinct<string>();
      this.PrepareStoredProcedure("prc_GetPersonNameConstantRecords");
      this.BindStringTable("@personNames", rows);
      return this.ExecuteUnknown<IEnumerable<PersonNameConstantRecord>>((System.Func<IDataReader, IEnumerable<PersonNameConstantRecord>>) (reader => (IEnumerable<PersonNameConstantRecord>) new WorkItemTrackingMetadataComponent.PersonNameConstantRecordBinder().BindAll(reader).ToList<PersonNameConstantRecord>()));
    }

    public virtual IEnumerable<ConstantsSearchRecord> SearchConstantsRecords(
      IEnumerable<string> searchValues,
      IEnumerable<Guid> tfIds,
      bool includeInactiveIdentities,
      bool isHostedDeployment)
    {
      throw new NotImplementedException();
    }

    internal virtual IEnumerable<ConstantRecord> GetConstantRecords(
      IEnumerable<Guid> teamFoundationIds,
      bool includeInactiveIdentities = false)
    {
      this.PrepareStoredProcedure(nameof (GetConstantRecords));
      List<OrderedString> witOrderedStrings = new List<OrderedString>(teamFoundationIds.Count<Guid>());
      int num = 0;
      foreach (Guid teamFoundationId in teamFoundationIds)
      {
        witOrderedStrings.Add(new OrderedString()
        {
          Value = teamFoundationId.ToString("D"),
          Order = num
        });
        ++num;
      }
      this.BindUserSid();
      this.BindWitOrderedStringTable("@values", (IEnumerable<OrderedString>) witOrderedStrings);
      this.BindInt("@searchFactor", 4);
      this.BindGetInactiveIdentities(includeInactiveIdentities);
      return (IEnumerable<ConstantRecord>) this.ExecuteUnknown<List<ConstantRecord>>((System.Func<IDataReader, List<ConstantRecord>>) (reader => this.GetConstantRecordBinder(true).BindAll(reader).ToList<ConstantRecord>()));
    }

    internal virtual void BindGetInactiveIdentities(bool includeInactiveIdentities)
    {
    }

    internal virtual void BindIncludeInactiveConstants(bool returnInactiveNonIdentityConstants)
    {
    }

    internal virtual IEnumerable<ConstantRecord> GetConstantRecords(
      IEnumerable<string> searchValues,
      bool includeInactiveIdentities = false,
      bool includeInactiveNonIdentityConstants = true)
    {
      this.PrepareStoredProcedure(nameof (GetConstantRecords));
      List<OrderedString> witOrderedStrings = new List<OrderedString>(searchValues.Count<string>());
      int num = 0;
      foreach (string searchValue in searchValues)
        witOrderedStrings.Add(new OrderedString()
        {
          Value = searchValue,
          Order = num++
        });
      this.BindUserSid();
      this.BindWitOrderedStringTable("@values", (IEnumerable<OrderedString>) witOrderedStrings);
      this.BindInt("@searchFactor", 2);
      this.BindGetInactiveIdentities(includeInactiveIdentities);
      this.BindIncludeInactiveConstants(includeInactiveNonIdentityConstants);
      return (IEnumerable<ConstantRecord>) this.ExecuteUnknown<List<ConstantRecord>>((System.Func<IDataReader, List<ConstantRecord>>) (reader => this.GetConstantRecordBinder(true).BindAll(reader).ToList<ConstantRecord>()));
    }

    internal virtual bool HasAnyWorkItemsOfTypeForProcess(Guid processType, string workItemTypeName) => false;

    internal virtual bool HasAnyWorkItemsOfTypeForProject(Guid projectId, string workItemTypeName) => false;

    public virtual IEnumerable<WorkItemTypeEntry> GetWorkItemTypes(
      IEnumerable<int> projectIds,
      bool populateFormEntries,
      bool disableDataspaceRls = false)
    {
      return this.GetWorkItemTypes(projectIds);
    }

    public virtual IEnumerable<WorkItemTypeEntry> GetWorkItemTypes(IEnumerable<int> projectIds)
    {
      this.PrepareStoredProcedure(nameof (GetWorkItemTypes));
      this.BindUserSid();
      this.BindInt32Table("@projectIds", projectIds);
      IEnumerable<WorkItemTypeRecord> workItemTypeRecords;
      IEnumerable<WorkItemTypeUsageRecord> workItemTypeUsageRecords;
      return (IEnumerable<WorkItemTypeEntry>) this.ExecuteUnknown<Dictionary<int?, WorkItemTypeEntry>.ValueCollection>((System.Func<IDataReader, Dictionary<int?, WorkItemTypeEntry>.ValueCollection>) (reader =>
      {
        workItemTypeRecords = (IEnumerable<WorkItemTypeRecord>) new WorkItemTrackingMetadataComponent.WorkItemTypeRecordBinder().BindAll(reader).ToList<WorkItemTypeRecord>();
        reader.NextResult();
        workItemTypeUsageRecords = (IEnumerable<WorkItemTypeUsageRecord>) new WorkItemTrackingMetadataComponent.WorkItemTypeUsageRecordBinder().BindAll(reader).ToList<WorkItemTypeUsageRecord>();
        Dictionary<int?, WorkItemTypeEntry> dictionary = workItemTypeRecords.Select<WorkItemTypeRecord, WorkItemTypeEntry>((System.Func<WorkItemTypeRecord, WorkItemTypeEntry>) (witr => WorkItemTypeEntry.Create(witr))).ToDictionary<WorkItemTypeEntry, int?>((System.Func<WorkItemTypeEntry, int?>) (wite => wite.Id));
        foreach (WorkItemTypeUsageRecord itemTypeUsageRecord in workItemTypeUsageRecords)
        {
          WorkItemTypeEntry workItemTypeEntry1;
          if (dictionary.TryGetValue(new int?(itemTypeUsageRecord.WorkItemTypeId), out workItemTypeEntry1))
          {
            workItemTypeEntry1.AddField(itemTypeUsageRecord.FieldId);
          }
          else
          {
            foreach (WorkItemTypeEntry workItemTypeEntry2 in dictionary.Values)
              workItemTypeEntry2.AddField(itemTypeUsageRecord.FieldId);
          }
        }
        return dictionary.Values;
      }));
    }

    internal virtual IEnumerable<WorkItemTypeCategoryRecord> GetWorkItemTypeCategories(
      Guid projectId)
    {
      this.PrepareDynamicProcedure("dynprc_GetWorkItemTypeCategories", "\r\n            DECLARE @projectName NVARCHAR(MAX)\r\n\r\n            SELECT @projectName = T.Name\r\n            FROM TreeNodes T\r\n            WHERE   T.CSSNodeId       = cast(@cssNodeId as NVARCHAR(255))\r\n                    AND T.[TypeID]    = -42\r\n                    AND T.PartitionId = @partitionId\r\n                    AND T.[fDeleted]  = 0\r\n            OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\n            EXEC GetWorkItemTypeCategories @partitionId, @projectName, @witCategoryRefNames");
      this.BindUserSid();
      this.BindGuid("@cssNodeId", projectId);
      this.BindStringTable("@witCategoryRefNames", (IEnumerable<string>) null);
      return (IEnumerable<WorkItemTypeCategoryRecord>) this.ExecuteUnknown<List<WorkItemTypeCategoryRecord>>((System.Func<IDataReader, List<WorkItemTypeCategoryRecord>>) (reader => new WorkItemTrackingMetadataComponent.WorkItemTypeCategoryRecordBinder().BindAll(reader).ToList<WorkItemTypeCategoryRecord>()));
    }

    public virtual IEnumerable<WorkItemTypeAction> GetWorkItemTypeActions(
      string projectName,
      string workItemType)
    {
      this.PrepareStoredProcedure("GetAllActionsForWorkItemType");
      this.BindUserSid();
      this.BindString("@projectName", projectName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@workItemType", workItemType, 256, true, SqlDbType.NVarChar);
      return (IEnumerable<WorkItemTypeAction>) this.ExecuteUnknown<List<WorkItemTypeAction>>((System.Func<IDataReader, List<WorkItemTypeAction>>) (reader => new WorkItemTrackingMetadataComponent.WorkItemTypeActionBinder().BindAll(reader).ToList<WorkItemTypeAction>()));
    }

    internal void StampDb()
    {
      this.PrepareStoredProcedure(nameof (StampDb));
      this.ExecuteNonQueryEx();
    }

    public virtual WorkItemTypeEntry UpdateWorkItemTypeName(
      Guid projectId,
      Guid teamFoundationId,
      string oldWorkItemTypeName,
      string newWorkItemTypeName)
    {
      this.RequestContext.WitContext();
      TreeNode treeNode = this.RequestContext.WitContext().TreeService.GetTreeNode(projectId, projectId);
      Microsoft.VisualStudio.Services.Identity.Identity identity = this.RequestContext.WitContext().IdentityService.ReadIdentities(this.RequestContext, (IList<Guid>) new Guid[1]
      {
        teamFoundationId
      }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.PrepareDynamicProcedure("dynprc_UpdateWorkItemTypeName", "\r\nSET NOCOUNT ON\r\nSET XACT_ABORT ON\r\n\r\nDECLARE @changerId INT\r\nDECLARE @status INT\r\nDECLARE @now DATETIME = GETUTCDATE()\r\n\r\n-- Return if we cannot find the user in WIT database\r\nEXEC @status = dbo.RebuildCallersViews @partitionId, @changerId output, @userSid\r\nIF @status <> 0 RETURN\r\n\r\nBEGIN TRAN\r\n\r\nEXEC @status = dbo.RenameWorkItemType @partitionId, @workItemTypeName, @newWorkitemTypeName, @projectName, @changerId, @now\r\nIF @status <> 0\r\nBEGIN\r\n    ROLLBACK\r\n    RETURN\r\nEND\r\n\r\nCOMMIT\r\n");
      this.BindString("@workItemTypeName", oldWorkItemTypeName, 256, false, SqlDbType.NVarChar);
      this.BindString("@newWorkItemTypeName", newWorkItemTypeName, 256, false, SqlDbType.NVarChar);
      this.BindString("@projectName", treeNode.GetName(this.RequestContext), 120, false, SqlDbType.NVarChar);
      this.BindString("@userSID", identity.Descriptor.Identifier, 256, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      return this.GetWorkItemTypes(Enumerable.Repeat<int>(treeNode.Id, 1)).FirstOrDefault<WorkItemTypeEntry>((System.Func<WorkItemTypeEntry, bool>) (wit => wit.Name == newWorkItemTypeName));
    }

    public virtual IEnumerable<GroupMembershipRecord> GetRuleDependentGroups() => throw new NotImplementedException();

    public virtual IEnumerable<ForNotRuleGroupRecord> GetForNotRuleGroups() => throw new NotImplementedException();

    protected virtual WorkItemTrackingMetadataComponent.ForNotRuleGroupRecordBinder GetForNotRuleGroupRecordBinder() => new WorkItemTrackingMetadataComponent.ForNotRuleGroupRecordBinder();

    public virtual void PerformDistinctNameMigration() => throw new NotImplementedException();

    public virtual void SyncIdentities(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities) => throw new NotImplementedException();

    public virtual void SaveConstantSets(IEnumerable<RuleSetRecord> setReferences) => throw new NotImplementedException();

    internal virtual void CreateDefaultWorkItemType(Guid projectId)
    {
      this.PrepareDynamicProcedure("dynprc_CreateDefaultWorkItemType", "\r\nSET IDENTITY_INSERT dbo.WorkItemTypes ON\r\n\r\nINSERT INTO dbo.WorkItemTypes\r\n(\r\n        PartitionId,\r\n        WorkItemTypeID,\r\n        NameConstantID,\r\n        ProjectID,\r\n        DescriptionID,\r\n        fDeleted\r\n)\r\nSELECT  @partitionId,\r\n        -p.Id,\r\n        0,\r\n        p.Id,\r\n        0,\r\n        0\r\nFROM    dbo.tbl_ClassificationNode p\r\nWHERE   p.PartitionId = @partitionId\r\n        AND p.Identifier = @projectId\r\n        AND p.ParentId IS NULL\r\n        AND NOT EXISTS (\r\n            SELECT  *\r\n            FROM    dbo.WorkItemTypes w\r\n            WHERE   w.PartitionId = @partitionId\r\n                    AND w.WorkItemTypeID = -p.Id\r\n        )\r\nOPTION (OPTIMIZE FOR (@partitionId UNKNOWN))\r\n\r\nSET IDENTITY_INSERT dbo.WorkItemTypes OFF");
      this.BindGuid("@projectId", projectId);
      this.ExecuteNonQuery();
    }

    public virtual void SetIdentityFieldBit()
    {
    }

    internal virtual void CreateFields(IReadOnlyCollection<CustomFieldEntry> fields, Guid changedBy) => throw new NotSupportedException();

    internal virtual void UpdateField(
      string referenceName,
      string description,
      Guid changedBy,
      Guid? convertToPicklistId,
      bool? isIdentityFromProcess = null)
    {
      throw new NotSupportedException();
    }

    internal virtual IEnumerable<ConstantRecord> GetNonIdentityConstants(
      IEnumerable<string> displayTextValues,
      bool includeInactiveConstants = true)
    {
      throw new NotSupportedException();
    }

    public virtual IEnumerable<Guid> GetForceProcessADObjects() => Enumerable.Empty<Guid>();

    public virtual void ForceSyncADObjects(IEnumerable<ImsSyncIdentity> identities)
    {
    }

    public virtual IEnumerable<ConstantAuditEntry> GetDuplicateIdentityConstants() => Enumerable.Empty<ConstantAuditEntry>();

    internal virtual void DeleteProcess(Guid processId, Guid changedBy, bool deleteFields = true) => throw new NotSupportedException();

    internal virtual void CleanupConstants(
      IReadOnlyCollection<int> usedConstants,
      long constantsMetadataStamp)
    {
    }

    public virtual void DeleteFields(IReadOnlyCollection<int> fieldIds, Guid teamFoundationId)
    {
    }

    public virtual void SetCollectionWebLayoutVersion(int version)
    {
    }

    public virtual void SetCollectionWebLayoutVersion2(int version)
    {
    }

    public virtual List<FieldRecord> GetFieldRecordsIncremental(
      long sinceFieldCacheStamp,
      out long maxCacheStamp,
      bool disableDataspaceRls = false,
      bool includeDeleted = false)
    {
      throw new NotImplementedException();
    }

    public virtual void RestoreField(int fieldId, Guid teamFoundationId) => throw new NotImplementedException();

    public virtual void SetFieldLocked(int fieldId, Guid teamFoundationId, bool isLocked) => throw new NotImplementedException();

    public virtual void CleanupProvisionedRecordsForInheritedProject(int projectId)
    {
    }

    public virtual IReadOnlyCollection<ProcessChangedRecord> GetProcessesForChangedWorkItemTypes(
      DateTime watermark)
    {
      throw new NotImplementedException();
    }

    public virtual IReadOnlyCollection<ProjectIdChangedRecord> GetProjectsForChangedWorkItemTypes(
      long sinceWatermark)
    {
      throw new NotImplementedException();
    }

    public virtual IReadOnlyCollection<ProjectIdChangedRecord> GetProjectsForChangedWorkItemTypeCategories(
      long sinceWatermark)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<GroupMemberEntry> FindGroupMembersOfType(
      IEnumerable<string> groupNames,
      GroupType groupType,
      int maxMemberForEachGroup = 100,
      int maxIteration = 50)
    {
      return Enumerable.Empty<GroupMemberEntry>();
    }

    public virtual void DestroyMetadata()
    {
    }

    internal virtual void ProvisionAllOOBFields(IDictionary<string, string> refNameToNameMappings)
    {
    }

    internal virtual void ResetSequenceId(int identitySequenceId, int groupSequenceId)
    {
    }

    internal virtual IDictionary<int, IEnumerable<string>> GetAllowedValues(
      IEnumerable<int> fieldIds,
      int? projectId = null,
      string projectName = null,
      IEnumerable<string> workItemTypeNames = null,
      bool sortById = false,
      bool excludeIdentities = false)
    {
      throw new NotImplementedException();
    }

    public virtual IReadOnlyCollection<ProcessChangedRecord> GetProcessesForChangedWorkItemTypeBehaviors(
      DateTime sinceWatermark)
    {
      return (IReadOnlyCollection<ProcessChangedRecord>) new List<ProcessChangedRecord>().AsReadOnly();
    }

    internal virtual int GetMaxProvisionedWorkItemTypeId() => 0;

    public virtual ProvisionAllOobLinkTypesRequestResult ProvisionAllOOBLinkTypes(
      IDictionary<string, string> refNameToForwardNameMappings,
      IDictionary<string, string> refNameToReverseNameMappings)
    {
      return ProvisionAllOobLinkTypesRequestResult.SprocNotYetAvailable;
    }

    private class ConstantSetReferenceTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<ConstantSetReference>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[4]
      {
        new SqlMetaData("SetId", SqlDbType.Int),
        new SqlMetaData("fDirect", SqlDbType.Bit),
        new SqlMetaData("fIncludeGroups", SqlDbType.Bit),
        new SqlMetaData("fIncludeTop", SqlDbType.Bit)
      };

      public override string TypeName => "typ_WitConstantSetReferenceTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent.ConstantSetReferenceTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        ConstantSetReference entry)
      {
        record.SetInt32(0, entry.Id);
        record.SetBoolean(1, entry.Direct);
        record.SetBoolean(2, !entry.ExcludeGroups);
        record.SetBoolean(3, entry.IncludeTop);
      }
    }

    private class WitSyncIdentityTable2RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<IVssIdentity>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[8]
      {
        new SqlMetaData("DomainPart", SqlDbType.NVarChar, 256L),
        new SqlMetaData("NamePart", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
        new SqlMetaData("UserSid", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Vsid", SqlDbType.UniqueIdentifier),
        new SqlMetaData("IsGroup", SqlDbType.Bit),
        new SqlMetaData("ObjectCategory", SqlDbType.Int),
        new SqlMetaData("ObjectSpecialType", SqlDbType.Int)
      };

      public override string TypeName => "typ_WitSyncIdentityTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent.WitSyncIdentityTable2RecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        IVssIdentity identity)
      {
        record.SetString(0, identity.GetProperty<string>("Domain", string.Empty));
        record.SetString(1, identity.GetProperty<string>("Account", string.Empty));
        record.SetString(2, identity.DisplayName);
        record.SetString(3, identity.Descriptor.Identifier);
        record.SetGuid(4, identity.Id);
        record.SetBoolean(5, identity.IsContainer);
        record.SetInt32(6, (int) IdentityConstantsNormalizer.GetBisIdentityType(identity));
        record.SetInt32(7, (int) identity.GetProperty<GroupSpecialType>("SpecialType", GroupSpecialType.Generic));
      }
    }

    private class WitSyncMembershipTable2RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<(Guid UserVsid, string GroupSid)>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
      {
        new SqlMetaData("UserVsid", SqlDbType.UniqueIdentifier),
        new SqlMetaData("GroupSid", SqlDbType.NVarChar, 256L)
      };

      public override string TypeName => "typ_WitSyncMembershipTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent.WitSyncMembershipTable2RecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        (Guid UserVsid, string GroupSid) membership)
      {
        record.SetGuid(0, membership.UserVsid);
        record.SetString(1, membership.GroupSid);
      }
    }

    private class RuleSetRecordTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<RuleSetRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[5]
      {
        new SqlMetaData("SetId", SqlDbType.Int),
        new SqlMetaData("ParentID", SqlDbType.Int),
        new SqlMetaData("ConstID", SqlDbType.Int),
        new SqlMetaData("Cachestamp", SqlDbType.Binary, 8L),
        new SqlMetaData("fDeleted", SqlDbType.Bit)
      };

      public override string TypeName => "typ_WitConstantSetTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent.RuleSetRecordTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        RuleSetRecord entry)
      {
        record.SetInt32(0, entry.RuleSetId);
        record.SetInt32(1, entry.ParentId);
        record.SetInt32(2, entry.ConstId);
        record.SetBoolean(4, entry.Deleted);
      }
    }

    private class TempIdMapTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<TempIdRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
      {
        new SqlMetaData("TempId", SqlDbType.Int),
        new SqlMetaData("Id", SqlDbType.Int)
      };

      public override string TypeName => "typ_WitTempIdMapTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent.TempIdMapTableRecordBinder.s_metadata;

      public override void SetRecordValues(WorkItemTrackingSqlDataRecord record, TempIdRecord entry)
      {
        record.SetInt32(0, entry.TempId);
        record.SetInt32(1, entry.Id);
      }
    }

    private class OrderedStringTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<OrderedString>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
      {
        new SqlMetaData("Value", SqlDbType.NVarChar, 4000L),
        new SqlMetaData("Order", SqlDbType.Int)
      };

      public override string TypeName => "typ_WitOrderedStringTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingMetadataComponent.OrderedStringTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        OrderedString entry)
      {
        record.SetString(0, entry.Value);
        record.SetInt32(1, entry.Order);
      }
    }

    protected class FieldRecordBinder : WorkItemTrackingObjectBinder<FieldRecord>
    {
      protected SqlColumnBinder m_fieldReferenceName = new SqlColumnBinder("ReferenceName");
      protected SqlColumnBinder m_fieldType = new SqlColumnBinder("Type");
      protected SqlColumnBinder m_fieldId;
      protected SqlColumnBinder m_fieldName = new SqlColumnBinder("Name");
      protected SqlColumnBinder m_fieldReportable;
      protected SqlColumnBinder m_fieldEditable = new SqlColumnBinder("fEditable");
      protected SqlColumnBinder m_fieldOftenQueriedAsText;
      protected SqlColumnBinder m_fieldSupportsTextQuery;
      protected SqlColumnBinder m_fieldReportingType = new SqlColumnBinder("ReportingType");
      protected SqlColumnBinder m_fieldReportingFormula = new SqlColumnBinder("ReportingFormula");
      protected SqlColumnBinder m_fieldReportingReferenceName = new SqlColumnBinder("ReportingReferenceName");
      protected SqlColumnBinder m_fieldReportingName = new SqlColumnBinder("ReportingName");
      protected SqlColumnBinder m_fieldCore;
      protected SqlColumnBinder m_fieldObjectId;
      protected SqlColumnBinder m_isIdentityField;
      protected SqlColumnBinder m_isLocked = new SqlColumnBinder("IsLocked");

      public FieldRecordBinder()
      {
        this.m_fieldId = new SqlColumnBinder("FldId");
        this.m_fieldReportable = new SqlColumnBinder("fReportingEnabled");
        this.m_fieldOftenQueriedAsText = new SqlColumnBinder("fOftenQueriedAsText");
        this.m_fieldSupportsTextQuery = new SqlColumnBinder("fSupportsTextQuery");
        this.m_fieldCore = new SqlColumnBinder("fCore");
        this.m_fieldObjectId = new SqlColumnBinder("ObjectID");
        this.m_isIdentityField = new SqlColumnBinder("IsIdentityField");
      }

      public override FieldRecord Bind(IDataReader reader)
      {
        int int32 = this.m_fieldId.GetInt32(reader);
        bool flag1 = !this.m_fieldCore.ColumnExists(reader) ? !this.m_fieldEditable.GetBoolean(reader) : this.m_fieldCore.GetBoolean(reader);
        bool? nullable1 = !this.m_fieldOftenQueriedAsText.ColumnExists(reader) ? new bool?() : new bool?(this.m_fieldOftenQueriedAsText.GetBoolean(reader));
        bool? nullable2 = !this.m_fieldSupportsTextQuery.ColumnExists(reader) ? new bool?() : new bool?(this.m_fieldSupportsTextQuery.GetBoolean(reader));
        InternalFieldUsages internalFieldUsages = !this.m_fieldObjectId.ColumnExists(reader) ? (int32 == 100 ? InternalFieldUsages.WorkItemLink : InternalFieldUsages.WorkItem) : FieldHelpers.GetFieldUsage(this.m_fieldObjectId.GetInt32(reader));
        bool flag2 = false;
        if (this.m_isIdentityField.ColumnExists(reader))
          flag2 = this.m_isIdentityField.GetBoolean(reader, false);
        return new FieldRecord()
        {
          Id = int32,
          ReferenceName = this.m_fieldReferenceName.GetString(reader, false),
          Name = this.m_fieldName.GetString(reader, false),
          FieldDataType = this.m_fieldType.GetInt32(reader),
          IsCore = flag1,
          Usages = internalFieldUsages,
          OftenQueriedAsText = nullable1,
          SupportsTextQuery = nullable2,
          IsReportable = this.m_fieldReportable.GetBoolean(reader),
          ReportingType = this.m_fieldReportingType.GetInt32(reader),
          ReportingFormula = this.m_fieldReportingFormula.GetInt32(reader),
          ReportingReferenceName = this.m_fieldReportingReferenceName.GetString(reader, false),
          ReportingName = this.m_fieldReportingName.GetString(reader, false),
          IsIdentity = flag2,
          IsLocked = this.m_isLocked.GetBoolean(reader, false, false)
        };
      }
    }

    protected class LinkTypeBinder : WorkItemTrackingObjectBinder<WorkItemLinkTypeRecord>
    {
      private SqlColumnBinder linkTypeId = new SqlColumnBinder("ID");
      private SqlColumnBinder linkTypeReverseId = new SqlColumnBinder("ReverseID");
      private SqlColumnBinder linkTypeName = new SqlColumnBinder("Name");
      private SqlColumnBinder linkTypeRules = new SqlColumnBinder("Rules");
      private SqlColumnBinder linkTypeReverse = new SqlColumnBinder("fReverse");
      private SqlColumnBinder linkTypeReferenceName = new SqlColumnBinder("ReferenceName");

      public override WorkItemLinkTypeRecord Bind(IDataReader reader) => new WorkItemLinkTypeRecord()
      {
        Id = (int) this.linkTypeId.GetInt16(reader),
        ReverseId = (int) this.linkTypeReverseId.GetInt16(reader),
        Name = this.linkTypeName.GetString(reader, false),
        Rules = this.linkTypeRules.GetInt32(reader),
        ReversedDirection = this.linkTypeReverse.GetBoolean(reader),
        ReferenceName = this.linkTypeReferenceName.GetString(reader, false)
      };
    }

    protected class RuleRecordBinder : WorkItemTrackingObjectBinder<RuleRecord>
    {
      private SqlColumnBinder RuleIDColumn = new SqlColumnBinder("RuleID");
      private SqlColumnBinder AreaIDColumn = new SqlColumnBinder("AreaID");
      private SqlColumnBinder Fld1IDColumn = new SqlColumnBinder("Fld1ID");
      private SqlColumnBinder Fld1IsConstIDColumn = new SqlColumnBinder("Fld1IsConstID");
      private SqlColumnBinder Fld1WasConstIDColumn = new SqlColumnBinder("Fld1WasConstID");
      private SqlColumnBinder Fld2IDColumn = new SqlColumnBinder("Fld2ID");
      private SqlColumnBinder Fld2IsConstIDColumn = new SqlColumnBinder("Fld2IsConstID");
      private SqlColumnBinder Fld2WasConstIDColumn = new SqlColumnBinder("Fld2WasConstID");
      private SqlColumnBinder Fld2IsColumn = new SqlColumnBinder("Fld2Is");
      private SqlColumnBinder Fld2WasColumn = new SqlColumnBinder("Fld2Was");
      private SqlColumnBinder Fld3IDColumn = new SqlColumnBinder("Fld3ID");
      private SqlColumnBinder Fld3IsColumn = new SqlColumnBinder("Fld3Is");
      private SqlColumnBinder Fld3IsConstIDColumn = new SqlColumnBinder("Fld3IsConstID");
      private SqlColumnBinder Fld3WasConstIDColumn = new SqlColumnBinder("Fld3WasConstID");
      private SqlColumnBinder Fld4IDColumn = new SqlColumnBinder("Fld4ID");
      private SqlColumnBinder Fld4IsConstIDColumn = new SqlColumnBinder("Fld4IsConstID");
      private SqlColumnBinder Fld4WasConstIDColumn = new SqlColumnBinder("Fld4WasConstID");
      private SqlColumnBinder IfColumn = new SqlColumnBinder("If");
      private SqlColumnBinder IfTeamFoundationIdColumn = new SqlColumnBinder("IfTeamFoundationId");
      private SqlColumnBinder IfConstIDColumn = new SqlColumnBinder("IfConstID");
      private SqlColumnBinder IfFldIDColumn = new SqlColumnBinder("IfFldID");
      private SqlColumnBinder If2ConstIDColumn = new SqlColumnBinder("If2ConstID");
      private SqlColumnBinder If2FldIDColumn = new SqlColumnBinder("If2FldID");
      private SqlColumnBinder ObjectTypeScopeIDColumn = new SqlColumnBinder("ObjectTypeScopeID");
      private SqlColumnBinder PersonColumn = new SqlColumnBinder("Person");
      private SqlColumnBinder PersonIDColumn = new SqlColumnBinder("PersonID");
      private SqlColumnBinder RootTreeIDColumn = new SqlColumnBinder("RootTreeID");
      private SqlColumnBinder ThenConstIDColumn = new SqlColumnBinder("ThenConstID");
      private SqlColumnBinder ThenColumn = new SqlColumnBinder("Then");
      private SqlColumnBinder ThenTeamFoundationIdColumn = new SqlColumnBinder("ThenTeamFoundationId");
      private SqlColumnBinder ThenFldIDColumn = new SqlColumnBinder("ThenFldID");
      private SqlColumnBinder RuleFlags1Column = new SqlColumnBinder("RuleFlags1");
      private SqlColumnBinder RuleFlags2Column = new SqlColumnBinder("RuleFlags2");
      private SqlColumnBinder FormColumn = new SqlColumnBinder("Form");

      public override RuleRecord Bind(IDataReader reader) => new RuleRecord()
      {
        RuleID = this.RuleIDColumn.GetInt32(reader),
        AreaID = this.AreaIDColumn.GetInt32(reader),
        Fld1ID = this.Fld1IDColumn.GetInt32(reader),
        Fld1IsConstID = this.Fld1IsConstIDColumn.GetInt32(reader),
        Fld1WasConstID = this.Fld1WasConstIDColumn.GetInt32(reader),
        Fld2ID = this.Fld2IDColumn.GetInt32(reader),
        Fld2Is = this.Fld2IsColumn.GetString(reader, true),
        Fld2IsConstID = this.Fld2IsConstIDColumn.GetInt32(reader),
        Fld2Was = this.Fld2WasColumn.GetString(reader, true),
        Fld2WasConstID = this.Fld2WasConstIDColumn.GetInt32(reader),
        Fld3ID = this.Fld3IDColumn.GetInt32(reader),
        Fld3Is = this.Fld3IsColumn.GetString(reader, true),
        Fld3IsConstID = this.Fld3IsConstIDColumn.GetInt32(reader),
        Fld3WasConstID = this.Fld3WasConstIDColumn.GetInt32(reader),
        Fld4ID = this.Fld4IDColumn.GetInt32(reader),
        Fld4IsConstID = this.Fld4IsConstIDColumn.GetInt32(reader),
        Fld4WasConstID = this.Fld4WasConstIDColumn.GetInt32(reader),
        IfConstID = this.IfConstIDColumn.GetInt32(reader),
        If = this.IfColumn.GetString(reader, true),
        IfFldID = this.IfFldIDColumn.GetInt32(reader),
        If2ConstID = this.If2ConstIDColumn.GetInt32(reader),
        If2FldID = this.If2FldIDColumn.GetInt32(reader),
        ObjectTypeScopeID = this.ObjectTypeScopeIDColumn.GetInt32(reader),
        Person = this.PersonColumn.GetString(reader, true),
        PersonID = this.PersonIDColumn.GetInt32(reader),
        RootTreeID = this.RootTreeIDColumn.GetInt32(reader),
        Then = this.ThenColumn.GetString(reader, true),
        ThenConstID = this.ThenConstIDColumn.GetInt32(reader),
        ThenFldID = this.ThenFldIDColumn.GetInt32(reader),
        RuleFlags = (RuleFlags) this.RuleFlags1Column.GetInt32(reader),
        RuleFlags2 = (RuleFlags2) this.RuleFlags2Column.GetInt32(reader),
        Form = this.FormColumn.GetString(reader, true)
      };
    }

    protected class SetRecordBinder : WorkItemTrackingObjectBinder<SetRecord>
    {
      private SqlColumnBinder ParentIdColumn = new SqlColumnBinder("ParentID");
      private SqlColumnBinder ItemColumn = new SqlColumnBinder("Item");
      private SqlColumnBinder ItemIdColumn = new SqlColumnBinder("ItemID");
      private SqlColumnBinder IsListColumn = new SqlColumnBinder("IsList");
      private SqlColumnBinder Direct = new SqlColumnBinder("fDirect");
      private SqlColumnBinder IncludeTop = new SqlColumnBinder("fIncludeTop");
      private SqlColumnBinder IncludeGroups = new SqlColumnBinder("fIncludeGroups");
      private SqlColumnBinder TeamFoundationId = new SqlColumnBinder(nameof (TeamFoundationId));

      public override SetRecord Bind(IDataReader reader) => new SetRecord()
      {
        ParentId = this.ParentIdColumn.GetInt32(reader),
        Item = this.ItemColumn.GetString(reader, false),
        ItemId = this.ItemIdColumn.GetInt32(reader),
        IsList = this.IsListColumn.GetBoolean(reader),
        Direct = this.Direct.GetBoolean(reader),
        IncludeTop = this.IncludeTop.GetBoolean(reader),
        IncludeGroups = this.IncludeGroups.GetBoolean(reader),
        TeamFoundationId = this.TeamFoundationId.GetGuid(reader, true, Guid.Empty)
      };
    }

    protected class ConstantRecordBinder : WorkItemTrackingObjectBinder<ConstantRecord>
    {
      private SqlColumnBinder OrderColumn = new SqlColumnBinder("Order");
      private SqlColumnBinder StringColumn = new SqlColumnBinder("String");
      private SqlColumnBinder SidColumn = new SqlColumnBinder("SID");
      private SqlColumnBinder IdColumn = new SqlColumnBinder("ConstID");
      private SqlColumnBinder DomainColumn = new SqlColumnBinder("DomainPart");
      private SqlColumnBinder NameColumn = new SqlColumnBinder("NamePart");
      private SqlColumnBinder DisplayTextColumn = new SqlColumnBinder("DisplayPart");
      private SqlColumnBinder TeamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");
      private bool m_backCompat;

      public ConstantRecordBinder(bool backCompat = false) => this.m_backCompat = backCompat;

      public override ConstantRecord Bind(IDataReader reader)
      {
        if (this.m_backCompat)
        {
          ConstantRecord constantRecord = new ConstantRecord()
          {
            Id = this.IdColumn.GetInt32(reader),
            DisplayText = this.DisplayTextColumn.GetString(reader, true),
            TeamFoundationId = this.TeamFoundationIdColumn.GetGuid(reader, true, Guid.Empty)
          };
          if (!string.IsNullOrEmpty(this.SidColumn.GetString(reader, true)))
          {
            string str = this.StringColumn.GetString(reader, false);
            string[] strArray = str.Split('\\');
            if (strArray.Length > 1)
            {
              constantRecord.Domain = strArray[0];
              constantRecord.Name = strArray[1];
            }
            else
              constantRecord.Name = str;
          }
          return constantRecord;
        }
        return new ConstantRecord()
        {
          Id = this.IdColumn.GetInt32(reader),
          Domain = this.DomainColumn.GetString(reader, true),
          Name = this.NameColumn.GetString(reader, true),
          DisplayText = this.DisplayTextColumn.GetString(reader, true),
          TeamFoundationId = this.TeamFoundationIdColumn.GetGuid(reader, true, Guid.Empty),
          StringValue = this.StringColumn.GetString(reader, (string) null)
        };
      }
    }

    protected class PersonNameConstantRecordBinder : 
      WorkItemTrackingObjectBinder<PersonNameConstantRecord>
    {
      private SqlColumnBinder FieldDisplayName = new SqlColumnBinder(nameof (FieldDisplayName));
      private SqlColumnBinder IdColumn = new SqlColumnBinder("ConstID");
      private SqlColumnBinder DomainColumn = new SqlColumnBinder("DomainPart");
      private SqlColumnBinder NameColumn = new SqlColumnBinder("NamePart");
      private SqlColumnBinder DisplayTextColumn = new SqlColumnBinder("DisplayPart");
      private SqlColumnBinder TeamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");

      public override PersonNameConstantRecord Bind(IDataReader reader)
      {
        PersonNameConstantRecord nameConstantRecord = new PersonNameConstantRecord();
        nameConstantRecord.FieldDisplayName = this.FieldDisplayName.GetString(reader, true);
        nameConstantRecord.Id = this.IdColumn.GetInt32(reader);
        nameConstantRecord.Domain = this.DomainColumn.GetString(reader, true);
        nameConstantRecord.Name = this.NameColumn.GetString(reader, true);
        nameConstantRecord.DisplayText = this.DisplayTextColumn.GetString(reader, true);
        nameConstantRecord.TeamFoundationId = this.TeamFoundationIdColumn.GetGuid(reader, true, Guid.Empty);
        return nameConstantRecord;
      }
    }

    protected class SearchConstantRecordBinder : WorkItemTrackingObjectBinder<ConstantsSearchRecord>
    {
      private SqlColumnBinder IdColumn = new SqlColumnBinder("ConstID");
      private SqlColumnBinder DomainColumn = new SqlColumnBinder("DomainPart");
      private SqlColumnBinder StringColumn = new SqlColumnBinder("String");
      private SqlColumnBinder NameColumn = new SqlColumnBinder("NamePart");
      private SqlColumnBinder DisplayTextColumn = new SqlColumnBinder("DisplayPart");
      private SqlColumnBinder TeamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");
      private SqlColumnBinder SearchValueColumn = new SqlColumnBinder("OriginalSearchValue");

      public override ConstantsSearchRecord Bind(IDataReader reader)
      {
        ConstantsSearchRecord constantsSearchRecord = new ConstantsSearchRecord();
        constantsSearchRecord.Id = this.IdColumn.GetInt32(reader);
        constantsSearchRecord.Domain = this.DomainColumn.GetString(reader, true);
        constantsSearchRecord.Account = this.NameColumn.GetString(reader, true);
        constantsSearchRecord.DisplayPart = this.DisplayTextColumn.GetString(reader, true);
        constantsSearchRecord.SearchValue = this.SearchValueColumn.GetString(reader, true);
        constantsSearchRecord.TeamFoundationId = this.TeamFoundationIdColumn.GetGuid(reader, true, Guid.Empty);
        return constantsSearchRecord;
      }
    }

    protected class TeamFoundationIdBinder : WorkItemTrackingObjectBinder<Guid>
    {
      private SqlColumnBinder teamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");

      public override Guid Bind(IDataReader reader) => this.teamFoundationIdColumn.GetGuid(reader);
    }

    protected class IdentityConstantRecordBinder : 
      WorkItemTrackingObjectBinder<IdentityConstantRecord>
    {
      private SqlColumnBinder IdColumn = new SqlColumnBinder("ConstID");
      private SqlColumnBinder DomainColumn = new SqlColumnBinder("DomainPart");
      private SqlColumnBinder StringColumn = new SqlColumnBinder("String");
      private SqlColumnBinder NameColumn = new SqlColumnBinder("NamePart");
      private SqlColumnBinder DisplayTextColumn = new SqlColumnBinder("DisplayPart");
      private SqlColumnBinder TeamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");
      private SqlColumnBinder IdentityCategoryColumn = new SqlColumnBinder("IdentityCategory");

      public override IdentityConstantRecord Bind(IDataReader reader) => new IdentityConstantRecord()
      {
        Id = this.IdColumn.GetInt32(reader),
        Domain = this.DomainColumn.GetString(reader, true),
        Account = this.NameColumn.GetString(reader, true),
        DisplayPart = this.DisplayTextColumn.GetString(reader, true),
        TeamFoundationId = this.TeamFoundationIdColumn.GetGuid(reader, true, Guid.Empty),
        IdentityCategory = (IdentityType) this.IdentityCategoryColumn.GetByte(reader)
      };
    }

    protected class WorkItemTypeRecordBinder : WorkItemTrackingObjectBinder<WorkItemTypeRecord>
    {
      private SqlColumnBinder IdColumn = new SqlColumnBinder("ID");
      private SqlColumnBinder ProjectIdColumn = new SqlColumnBinder("ProjectID");
      private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
      private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");

      public override WorkItemTypeRecord Bind(IDataReader reader) => new WorkItemTypeRecord()
      {
        Id = this.IdColumn.GetInt32(reader),
        Name = this.NameColumn.GetString(reader, false),
        ReferenceName = this.NameColumn.GetString(reader, false),
        Description = this.DescriptionColumn.GetString(reader, false),
        ProjectId = this.ProjectIdColumn.GetInt32(reader)
      };
    }

    protected class WorkItemTypeUsageRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemTypeUsageRecord>
    {
      private SqlColumnBinder FieldIDColumn = new SqlColumnBinder("FieldID");
      private SqlColumnBinder WorkItemTypeIDColumn = new SqlColumnBinder("WorkItemTypeID");
      private SqlColumnBinder ProjectIdColumn = new SqlColumnBinder("ProjectId");

      public override WorkItemTypeUsageRecord Bind(IDataReader reader) => new WorkItemTypeUsageRecord()
      {
        FieldId = this.FieldIDColumn.GetInt32(reader),
        WorkItemTypeId = this.WorkItemTypeIDColumn.GetInt32(reader),
        ProjectId = this.ProjectIdColumn.GetInt32(reader)
      };
    }

    internal class WorkItemTypeCategoryRecordBinder : 
      WorkItemTrackingObjectBinder<WorkItemTypeCategoryRecord>
    {
      private SqlColumnBinder WorkItemTypeNameListColumn = new SqlColumnBinder("ListWorkItemType");
      protected SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
      protected SqlColumnBinder DefaultWorkItemTypeNameColumn = new SqlColumnBinder("DefaultWorkItemTypeName");
      protected SqlColumnBinder ReferenceNameColumn = new SqlColumnBinder("ReferenceName");

      public override WorkItemTypeCategoryRecord Bind(IDataReader reader) => new WorkItemTypeCategoryRecord()
      {
        WorkItemTypeNames = (IEnumerable<string>) this.WorkItemTypeNameListColumn.FromXml(reader),
        Name = this.NameColumn.GetString(reader, false),
        DefaultWorkItemTypeName = this.DefaultWorkItemTypeNameColumn.GetString(reader, false),
        ReferenceName = this.ReferenceNameColumn.GetString(reader, false)
      };
    }

    internal class WorkItemTypeActionBinder : WorkItemTrackingObjectBinder<WorkItemTypeAction>
    {
      private SqlColumnBinder FromState = new SqlColumnBinder(nameof (FromState));
      private SqlColumnBinder ToState = new SqlColumnBinder(nameof (ToState));
      private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
      private SqlColumnBinder WorkItemType = new SqlColumnBinder(nameof (WorkItemType));

      public override WorkItemTypeAction Bind(IDataReader reader) => new WorkItemTypeAction()
      {
        FromState = this.FromState.GetString(reader, true),
        ToState = this.ToState.GetString(reader, true),
        Name = this.Name.GetString(reader, true),
        WorkItemType = this.WorkItemType.GetString(reader, true)
      };
    }

    protected class ForNotRuleGroupRecordBinder : WorkItemTrackingObjectBinder<ForNotRuleGroupRecord>
    {
      private SqlColumnBinder StringColumn = new SqlColumnBinder("String");

      public override ForNotRuleGroupRecord Bind(IDataReader reader) => new ForNotRuleGroupRecord()
      {
        StringValue = this.StringColumn.GetString(reader, true)
      };
    }
  }
}
