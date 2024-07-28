// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent8
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent8 : WorkItemTypeExtensionComponent7
  {
    public override List<WorkItemTypeletRecord> GetWorkItemTypelets(Guid processId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypelets");
      this.BindGuid("@processId", processId);
      return this.ReadExtensions((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder());
    }

    public override WorkItemTypeletRecord CreateWorkItemTypelet(
      Guid extensionId,
      Guid processId,
      string name,
      string refName,
      string description,
      string parentTypeRefName,
      IList<int> fields,
      IList<WorkItemFieldRule> fieldRules,
      IList<Guid> disabledRules,
      string form,
      Guid changedBy,
      string color = null,
      string icon = null,
      IReadOnlyCollection<WorkItemStateDeclaration> states = null,
      bool? isDisabled = null)
    {
      string serializedRuleXml = fieldRules == null || !fieldRules.Any<WorkItemFieldRule>() ? (string) null : CommonWITUtils.GetSerializedRuleXML(fieldRules.ToArray<WorkItemFieldRule>());
      this.PrepareStoredProcedure("prc_CreateWorkItemTypelet");
      this.BindGuid("@eventAuthor", this.Author);
      this.BindGuid("@id", extensionId);
      this.BindGuid("@processId", processId);
      this.BindInt("@typeletType", 1);
      this.BindString("@name", name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@refName", refName, 386, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", description, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@form", form, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@parentTypeRefName", parentTypeRefName, 386, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@fieldRules", serializedRuleXml, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@changedBy", -1);
      this.BindFieldIdTable("@fields", (IEnumerable<int>) fields);
      WorkItemTypeletRecord workItemTypelet = this.ReadExtensions((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder()).FirstOrDefault<WorkItemTypeletRecord>();
      if (fieldRules == null)
        return workItemTypelet;
      this.EnsureConstantsForBackcompat(fieldRules);
      return workItemTypelet;
    }

    protected void EnsureConstantsForBackcompat(IList<WorkItemFieldRule> fieldRules, string witName = null)
    {
      List<string> constants = new List<string>();
      if (!string.IsNullOrEmpty(witName))
        constants.Add(witName);
      if (fieldRules != null)
        constants.AddRange((IEnumerable<string>) CommonWITUtils.ExtractConstantsFromRules(fieldRules));
      this.EnsureConstantsForBackcompat((IList<string>) constants);
    }

    internal override void EnsureConstantsForBackcompat(IList<string> constants)
    {
      if (!constants.Any<string>())
        return;
      this.PrepareStoredProcedure("AddConstants");
      this.AddIgnoreCaseBindingForConstants(true);
      this.BindWitConstantTable("@constants", constants.Distinct<string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase));
      this.ExecuteNonQueryEx();
    }

    public override WorkItemTypeletRecord UpdateWorkItemTypelet(
      Guid extensionId,
      Guid processId,
      string description,
      IList<int> fields,
      IList<WorkItemFieldRule> fieldRules,
      IList<Guid> disabledRules,
      string form,
      Guid changedBy,
      DateTime readVersion,
      string color = null,
      string icon = null,
      bool? isDisabled = null)
    {
      string parameterValue = (string) null;
      if (fieldRules != null)
        parameterValue = CommonWITUtils.GetSerializedRuleXML(fieldRules.ToArray<WorkItemFieldRule>());
      this.PrepareStoredProcedure("prc_UpdateWorkItemTypelet");
      this.BindGuid("@eventAuthor", this.Author);
      this.BindInt("@changedBy", -1);
      this.BindGuid("@id", extensionId);
      this.BindGuid("@processId", processId);
      this.BindInt("@typeletType", 1);
      this.BindString("@fieldRules", parameterValue, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", description, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@form", form, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (fields != null)
      {
        this.BindFieldIdTable("@fields", (IEnumerable<int>) fields);
        this.BindBoolean("@updateFields", true);
      }
      else
        this.BindBoolean("@updateFields", false);
      WorkItemTypeletRecord itemTypeletRecord = this.ReadExtensions((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder()).FirstOrDefault<WorkItemTypeletRecord>();
      if (fieldRules == null)
        return itemTypeletRecord;
      this.EnsureConstantsForBackcompat(fieldRules);
      return itemTypeletRecord;
    }

    protected SqlParameter BindFieldIdTable(string parameterName, IEnumerable<int> fieldIds) => this.BindBasicTvp<int>((WorkItemTrackingResourceComponent.TvpRecordBinder<int>) new WorkItemTypeExtensionComponent8.FieldIdTableRecordBinder(), parameterName, fieldIds);

    protected override SqlParameter BindCustomFieldTable(
      string parameterName,
      IEnumerable<CustomFieldEntry> fieldEntries)
    {
      return this.BindBasicTvp<CustomFieldEntry>((WorkItemTrackingResourceComponent.TvpRecordBinder<CustomFieldEntry>) new WorkItemTypeExtensionComponent8.CustomFieldTableRecordBinder3(), parameterName, fieldEntries);
    }

    protected override SqlParameter BindWitConstantTable(
      string parameterName,
      IEnumerable<string> constants)
    {
      return this.BindBasicTvp<string>((WorkItemTrackingResourceComponent.TvpRecordBinder<string>) new WorkItemTypeExtensionComponent8.CreateSimpleWitConstantTableBinder(), parameterName, constants);
    }

    protected class CustomFieldTableRecordBinder3 : 
      WorkItemTypeExtensionComponent7.CustomFieldTableRecordBinder2
    {
      public override string TypeName => "typ_WitCustomFieldTable4";

      protected override SqlMetaData[] TvpMetadata => new List<SqlMetaData>((IEnumerable<SqlMetaData>) base.TvpMetadata)
      {
        new SqlMetaData("ProcessId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("Description", SqlDbType.VarChar, 256L)
      }.ToArray();

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        CustomFieldEntry fieldEntry)
      {
        base.SetRecordValues(record, fieldEntry);
      }
    }

    protected class FieldIdTableRecordBinder : WorkItemTrackingResourceComponent.TvpRecordBinder<int>
    {
      protected static readonly SqlMetaData[] s_metadata = new SqlMetaData[1]
      {
        new SqlMetaData("Val", SqlDbType.Int)
      };

      public override string TypeName => "typ_Int32Table";

      protected override SqlMetaData[] TvpMetadata => WorkItemTypeExtensionComponent8.FieldIdTableRecordBinder.s_metadata;

      public override void SetRecordValues(WorkItemTrackingSqlDataRecord record, int entry) => record.SetInt32(0, entry);
    }

    protected class CreateSimpleWitConstantTableBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<string>
    {
      protected static readonly SqlMetaData[] s_metadata = new SqlMetaData[5]
      {
        new SqlMetaData("ConstID", SqlDbType.Int),
        new SqlMetaData("DisplayPart", SqlDbType.NVarChar, 256L),
        new SqlMetaData("DomainPart", SqlDbType.NVarChar, 256L),
        new SqlMetaData("NamePart", SqlDbType.NVarChar, 256L),
        new SqlMetaData("LookupAccount", SqlDbType.Bit)
      };

      public override string TypeName => "typ_WitConstantTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemTypeExtensionComponent8.CreateSimpleWitConstantTableBinder.s_metadata;

      public override void SetRecordValues(WorkItemTrackingSqlDataRecord record, string fieldEntry)
      {
        record.SetInt32(0, 0);
        record.SetString(1, fieldEntry);
        record.SetDBNull(2);
        record.SetDBNull(3);
        record.SetBoolean(4, false);
      }
    }
  }
}
