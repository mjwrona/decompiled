// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent7
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent7 : WorkItemTypeExtensionComponent6
  {
    public override WorkItemTypeletRecord UpdateExtension(
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      Guid changedBy,
      string name,
      string description,
      IList<CustomFieldEntry> fields,
      WorkItemExtensionPredicate predicate,
      IList<WorkItemFieldRule> fieldRules,
      string form,
      int rank)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemTypeExtension");
      this.BindGuid("@eventAuthor", this.Author);
      this.BindExtension(extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules, form, rank);
      return this.ReadExtensions().FirstOrDefault<WorkItemTypeletRecord>();
    }

    public override WorkItemTypeletRecord CreateExtension(
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      Guid changedBy,
      string name,
      string description,
      IList<CustomFieldEntry> fields,
      WorkItemExtensionPredicate predicate,
      IList<WorkItemFieldRule> fieldRules,
      string form,
      int rank)
    {
      this.PrepareStoredProcedure("prc_CreateWorkItemTypeExtension");
      this.BindGuid("@eventAuthor", this.Author);
      this.BindExtension(extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules, form, rank);
      return this.ReadExtensions().FirstOrDefault<WorkItemTypeletRecord>();
    }

    protected void BindExtension(
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IList<CustomFieldEntry> fields,
      WorkItemExtensionPredicate predicate,
      IList<WorkItemFieldRule> fieldRules,
      string form,
      int rank)
    {
      string serializedRuleXml = fieldRules == null || !fieldRules.Any<WorkItemFieldRule>() ? (string) null : CommonWITUtils.GetSerializedRuleXML(fieldRules.ToArray<WorkItemFieldRule>());
      string parameterValue = predicate == null ? (string) null : TeamFoundationSerializationUtility.SerializeToString<WorkItemExtensionPredicate>(predicate);
      this.BindGuid("@id", extensionId);
      this.BindGuid("@projectId", projectId);
      this.BindGuid("@ownerId", ownerId);
      this.BindString("@name", name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", description, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (parameterValue != null && this.MaxPredicateLength > 0 && parameterValue.Length > this.MaxPredicateLength)
        throw new NotSupportedException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ErrorSerializedExtensionPredicateTooLong());
      this.BindString("@predicate", parameterValue, this.MaxPredicateLength, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@fieldRules", serializedRuleXml, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindCustomFieldTable("@fields", (IEnumerable<CustomFieldEntry>) fields);
      this.BindString("@form", form, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@rank", rank);
    }

    protected override WorkItemTypeExtensionBinder GetWorkItemTypeExtensionBinder() => (WorkItemTypeExtensionBinder) new WorkItemTypeExtensionBinder2();

    protected override SqlParameter BindCustomFieldTable(
      string parameterName,
      IEnumerable<CustomFieldEntry> fieldEntries)
    {
      return this.BindBasicTvp<CustomFieldEntry>((WorkItemTrackingResourceComponent.TvpRecordBinder<CustomFieldEntry>) new WorkItemTypeExtensionComponent7.CustomFieldTableRecordBinder2(), parameterName, fieldEntries);
    }

    protected class CustomFieldTableRecordBinder2 : 
      WorkItemTypeExtensionComponent.CustomFieldTableRecordBinder
    {
      public override string TypeName => "typ_WitCustomFieldTable3";

      protected override SqlMetaData[] TvpMetadata => new List<SqlMetaData>((IEnumerable<SqlMetaData>) WorkItemTypeExtensionComponent.CustomFieldTableRecordBinder.s_metadata)
      {
        new SqlMetaData("ParentFieldId", SqlDbType.Int)
      }.ToArray();

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        CustomFieldEntry fieldEntry)
      {
        base.SetRecordValues(record, fieldEntry);
        record.SetInt32(9, fieldEntry.ParentFieldId);
      }
    }
  }
}
