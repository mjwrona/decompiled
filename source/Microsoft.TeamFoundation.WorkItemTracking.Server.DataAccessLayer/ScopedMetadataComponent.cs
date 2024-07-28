// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ScopedMetadataComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ScopedMetadataComponent : SqlAccess
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[56]
    {
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent2>(2),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent3>(3),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent4>(4),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent5>(5),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent6>(6),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent7>(7),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent7>(8),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent9>(9),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent9>(10),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent11>(11),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent11>(12),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent12>(13),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent12>(14),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent12>(15),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent12>(16),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(17),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(18),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(19),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(20),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(21),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(22),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(23),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(24),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(25),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(26),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(27),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(28),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(29),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(30),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(31),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(32),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(33),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(34),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(35),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(36),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(37),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(38),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent17>(39),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(40),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(41),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(42),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(43),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(44),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(45),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(46),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(47),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(48),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent40>(49),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent50>(50),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent50>(51),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent50>(52),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent50>(53),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent50>(54),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent50>(55),
      (IComponentCreator) new ComponentCreator<ScopedMetadataComponent50>(56)
    }, "WorkItemMetadata");
    private static readonly SqlMetaData[] typ_WitOrderedStringTable = new SqlMetaData[2]
    {
      new SqlMetaData("Value", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("Order", SqlDbType.Int)
    };

    protected SqlParameter BindActionsTable(
      string parameterName,
      IEnumerable<WorkItemTypeAction> actions)
    {
      return this.BindBasicTvp<WorkItemTypeAction>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTypeAction>) new ScopedMetadataComponent.WitWorkItemTypeActionTableRecordBinder(), parameterName, actions);
    }

    public static ScopedMetadataComponent CreateComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<ScopedMetadataComponent>("WorkItem");

    public ScopedMetadataComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.None;

    protected virtual void BindUserSid() => this.BindString("@userSID", this.RequestContext.UserContext.Identifier, 256, false, SqlDbType.NVarChar);

    protected virtual FieldDefinitionRecordBinder GetFieldDefinitionRecordBinder() => new FieldDefinitionRecordBinder();

    protected virtual FieldUsageRecordBinder GetFieldUsageRecordBinder() => new FieldUsageRecordBinder();

    public ResultCollection GetFields()
    {
      this.PrepareStoredProcedure(nameof (GetFields));
      this.BindUserSid();
      ResultCollection fields = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      fields.AddBinder<FieldDefinitionRecord>((ObjectBinder<FieldDefinitionRecord>) this.GetFieldDefinitionRecordBinder());
      fields.AddBinder<FieldUsageRecord>((ObjectBinder<FieldUsageRecord>) this.GetFieldUsageRecordBinder());
      return fields;
    }

    internal virtual IList<DalWorkItemTypeField> GetWorkItemTypeFields(bool disableDataspaceRls = false)
    {
      this.DataspaceRlsEnabled = !disableDataspaceRls;
      string sqlStatement = "\r\nSELECT W.ProjectID AS ProjectId,\r\n       C.DisplayPart AS WorkItemTypeName,\r\n       W.WorkItemTypeID AS WorkItemTypeId,\r\n       U.FieldID AS FieldId\r\nFROM WorkItemTypes W\r\nJOIN Constants C ON W.NameConstantID = C.ConstID\r\nJOIN WorkItemTypeUsages U ON U.WorkItemTypeID = W.WorkItemTypeID\r\nWHERE W.fDeleted = 0\r\n      AND W.WorkItemTypeID > 0\r\n      AND U.fDeleted = 0";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DalWorkItemTypeField>((ObjectBinder<DalWorkItemTypeField>) new WorkItemTypeFieldBinder());
        return (IList<DalWorkItemTypeField>) resultCollection.GetCurrent<DalWorkItemTypeField>().Items;
      }
    }

    public Dictionary<int, string> GetConstants(IEnumerable<int> constantIds)
    {
      this.PrepareStoredProcedure(nameof (GetConstants));
      this.BindUserSid();
      this.BindInt32Table("@constIds", constantIds);
      return WorkItemTrackingResourceComponent.Bind<KeyValuePair<int, string>>(this.ExecuteReader(), (System.Func<IDataReader, KeyValuePair<int, string>>) (reader1 => new KeyValuePair<int, string>(reader1.GetInt32(0), reader1.GetString(1)))).ToDictionary<KeyValuePair<int, string>, int, string>((System.Func<KeyValuePair<int, string>, int>) (pair => pair.Key), (System.Func<KeyValuePair<int, string>, string>) (pair => pair.Value));
    }

    public ResultCollection GetWorkItemLinkTypes()
    {
      this.PrepareStoredProcedure(nameof (GetWorkItemLinkTypes));
      this.BindUserSid();
      ResultCollection workItemLinkTypes = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      workItemLinkTypes.AddBinder<WorkItemLinkTypeRecord>((ObjectBinder<WorkItemLinkTypeRecord>) new WorkItemLinkTypeRecordBinder());
      return workItemLinkTypes;
    }

    internal virtual IDictionary<int, IEnumerable<string>> GetAllowedValues(
      IEnumerable<int> fieldIds,
      string projectName,
      IEnumerable<string> workItemTypeNames,
      bool sortById = false,
      bool excludeIdentities = false)
    {
      if (!fieldIds.Any<int>())
        return (IDictionary<int, IEnumerable<string>>) null;
      int fieldId = fieldIds.ElementAt<int>(0);
      IEnumerable<string> allowedValues = this.GetAllowedValues(fieldId, projectName, workItemTypeNames, sortById, excludeIdentities);
      return (IDictionary<int, IEnumerable<string>>) new Dictionary<int, IEnumerable<string>>()
      {
        {
          fieldId,
          allowedValues
        }
      };
    }

    internal virtual IEnumerable<string> GetAllowedValues(
      int fieldId,
      string projectName,
      IEnumerable<string> workItemTypeNames,
      bool sortById = false,
      bool excludeIdentities = false)
    {
      this.PrepareStoredProcedure(nameof (GetAllowedValues));
      this.BindUserSid();
      this.BindInt("@fieldId", fieldId);
      this.BindString("@projectName", projectName, (int) byte.MaxValue, true, SqlDbType.NVarChar);
      this.BindBoolean("@sortById", sortById);
      if (!string.IsNullOrEmpty(projectName))
        this.BindStringTable("@workItemTypeNames", workItemTypeNames);
      return (IEnumerable<string>) WorkItemTrackingResourceComponent.Bind<string>(this.ExecuteReader(), (System.Func<IDataReader, string>) (reader1 => reader1.GetString(0))).ToList<string>();
    }

    internal IEnumerable<string> GetGlobalAndProjectGroups(int projectId, bool includeGlobal)
    {
      this.PrepareStoredProcedure(nameof (GetGlobalAndProjectGroups));
      this.BindUserSid();
      this.BindInt("@projectId", projectId);
      this.BindBoolean("@fIncludeGlobal", includeGlobal);
      return (IEnumerable<string>) WorkItemTrackingResourceComponent.Bind<string>(this.ExecuteReader(), (System.Func<IDataReader, string>) (reader1 => reader1.GetString(0))).ToList<string>();
    }

    internal ResultCollection GetWorkItemTypeActions(string workItemType, string projectName)
    {
      ResultCollection workItemTypeActions;
      try
      {
        this.PrepareStoredProcedure("GetAllActionsForWorkItemType");
        this.BindUserSid();
        this.BindString("@workItemType", workItemType, 256, true, SqlDbType.NVarChar);
        this.BindString("@projectName", projectName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
        workItemTypeActions = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        workItemTypeActions.AddBinder<WorkItemTypeAction>((ObjectBinder<WorkItemTypeAction>) new WorkItemTypeActionBinder());
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
      return workItemTypeActions;
    }

    internal void AddWorkItemTypeActions(
      IEnumerable<WorkItemTypeAction> actions,
      string projectName)
    {
      this.PrepareStoredProcedure(nameof (AddWorkItemTypeActions));
      this.BindActionsTable("@actions", actions);
      this.BindUserSid();
      this.BindString("@projectName", projectName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal void SetDisplayForm(string projectName, string workItemType, string displayForm)
    {
      this.PrepareStoredProcedure("SetDisplayFormForWorkItemType");
      this.BindUserSid();
      this.BindString("@projectName", projectName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@workItemType", workItemType, 256, false, SqlDbType.NVarChar);
      this.BindString("@displayForm", displayForm, 0, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal List<ConstantRecord> GetConstantRecords(
      string[] searchValues,
      ConstantRecordSearchFactor searchFactor,
      bool includeInactiveIdentities = false)
    {
      this.PrepareStoredProcedure(nameof (GetConstantRecords));
      List<OrderedString> rows = new List<OrderedString>(searchValues.Length);
      for (int index = 0; index < searchValues.Length; ++index)
        rows.Add(new OrderedString()
        {
          Value = searchValues[index],
          Order = index
        });
      this.BindUserSid();
      this.BindWitOrderedStringTable("@values", (IEnumerable<OrderedString>) rows);
      this.BindInt("@searchFactor", (int) searchFactor);
      this.BindGetInactiveIdentities(includeInactiveIdentities);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ConstantRecord>((ObjectBinder<ConstantRecord>) new ConstantRecordBinder());
        return resultCollection.GetCurrent<ConstantRecord>().Items;
      }
    }

    internal virtual void BindGetInactiveIdentities(bool includeInactiveIdentities)
    {
    }

    internal IEnumerable<long> GetQueryItemsTimestamps(int projectId)
    {
      this.PrepareStoredProcedure(nameof (GetQueryItemsTimestamps));
      this.BindIdentityColumn((IVssIdentity) this.RequestContext.GetUserIdentity());
      this.BindInt("@projectID", projectId);
      List<long> queryItemsTimestamps = new List<long>(2);
      IDataReader reader = this.ExecuteReader();
      do
      {
        queryItemsTimestamps.AddRange(WorkItemTrackingResourceComponent.Bind<long>(reader, (System.Func<IDataReader, long>) (rdr => rdr.IsDBNull(0) ? 0L : rdr.GetInt64(0))));
      }
      while (reader.NextResult());
      return (IEnumerable<long>) queryItemsTimestamps;
    }

    internal IEnumerable<int> GetProjectsForProvisioning(
      IEnumerable<int> fields,
      IEnumerable<int> fieldUsages,
      IEnumerable<int> treeProperties,
      IEnumerable<int> rules,
      IEnumerable<int> constants,
      IEnumerable<int> sets,
      IEnumerable<int> workItemtypes,
      IEnumerable<int> workItemTypeUsages,
      IEnumerable<int> actions,
      IEnumerable<int> workItemTypeCategories,
      IEnumerable<int> workItemTypeCategoryMembers)
    {
      this.PrepareStoredProcedure(nameof (GetProjectsForProvisioning));
      this.BindInt32Table("@fields", fields);
      this.BindInt32Table("@fieldUsages", fieldUsages);
      this.BindInt32Table("@treeProperties", treeProperties);
      this.BindInt32Table("@rules", rules);
      this.BindInt32Table("@constants", constants);
      this.BindInt32Table("@sets", sets);
      this.BindInt32Table("@workItemtypes", workItemtypes);
      this.BindInt32Table("@workItemTypeUsages", workItemTypeUsages);
      this.BindInt32Table("@actions", actions);
      this.BindInt32Table("@workItemTypeCategories", workItemTypeCategories);
      this.BindInt32Table("@workItemTypeCategoryMembers", workItemTypeCategoryMembers);
      return WorkItemTrackingResourceComponent.Bind<int>(this.ExecuteReader(), (System.Func<IDataReader, int>) (reader => reader.GetInt32(0)));
    }

    protected virtual void BindIdentityColumn(IVssIdentity caller) => this.BindString("@userSID", caller.Descriptor.Identifier, 256, false, SqlDbType.NVarChar);

    internal SqlParameter BindWitOrderedStringTable(
      string parameterName,
      IEnumerable<OrderedString> rows)
    {
      rows = rows ?? Enumerable.Empty<OrderedString>();
      System.Func<OrderedString, SqlDataRecord> selector = (System.Func<OrderedString, SqlDataRecord>) (orderedString =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ScopedMetadataComponent.typ_WitOrderedStringTable);
        sqlDataRecord.SetString(0, orderedString.Value);
        sqlDataRecord.SetSqlInt32(1, (SqlInt32) orderedString.Order);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_WitOrderedStringTable", rows.Select<OrderedString, SqlDataRecord>(selector));
    }

    private class WitWorkItemTypeActionTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTypeAction>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[4]
      {
        new SqlMetaData("WorkItemType", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
        new SqlMetaData("FromState", SqlDbType.NVarChar, 256L),
        new SqlMetaData("ToState", SqlDbType.NVarChar, 256L)
      };

      public override string TypeName => "typ_WitWorkItemTypeActionTable";

      protected override SqlMetaData[] TvpMetadata => ScopedMetadataComponent.WitWorkItemTypeActionTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemTypeAction entry)
      {
        record.SetString(0, entry.WorkItemType);
        record.SetString(1, entry.Name);
        record.SetString(2, entry.FromState);
        record.SetString(3, entry.ToState);
      }
    }
  }
}
