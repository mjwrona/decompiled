// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent9
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent9 : WorkItemTypeExtensionComponent8
  {
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
      this.BindExtension(extensionId, projectId, ownerId, changedBy, name, description, fields, predicate, fieldRules, form, rank);
      return this.ReadExtensions().FirstOrDefault<WorkItemTypeletRecord>();
    }

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
      this.BindExtension(extensionId, projectId, ownerId, changedBy, name, description, fields, predicate, fieldRules, form, rank);
      return this.ReadExtensions().FirstOrDefault<WorkItemTypeletRecord>();
    }

    internal override bool DeleteExtensions(IEnumerable<Guid> extensionIds, Guid changedBy)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkItemTypeExtensions");
      this.BindGuidTable("@ids", extensionIds);
      this.BindGuid("@eventAuthor", this.Author);
      this.BindGuid("@changedBy", changedBy);
      this.ExecuteNonQueryEx();
      return true;
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
      this.BindGuid("@changedBy", changedBy);
      this.BindFieldIdTable("@fields", (IEnumerable<int>) fields);
      WorkItemTypeletRecord workItemTypelet = this.ReadExtensions((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder()).FirstOrDefault<WorkItemTypeletRecord>();
      this.EnsureConstantsForBackcompat(fieldRules, name);
      return workItemTypelet;
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
      this.BindGuid("@changedBy", changedBy);
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
      this.EnsureConstantsForBackcompat(fieldRules);
      return itemTypeletRecord;
    }

    protected void BindExtension(
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
      this.BindExtension(extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules, form, rank);
      this.BindGuid("@changedBy", changedBy);
    }
  }
}
