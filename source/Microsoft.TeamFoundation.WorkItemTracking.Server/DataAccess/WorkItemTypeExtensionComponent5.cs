// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent5 : WorkItemTypeExtensionComponent4
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
      string form)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemTypeExtension");
      this.BindGuid("@eventAuthor", this.Author);
      this.BindExtension(extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules, form);
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
      string form)
    {
      this.PrepareStoredProcedure("prc_CreateWorkItemTypeExtension");
      this.BindGuid("@eventAuthor", this.Author);
      this.BindExtension(extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules, form);
      return this.ReadExtensions().FirstOrDefault<WorkItemTypeletRecord>();
    }

    private void BindExtension(
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IList<CustomFieldEntry> fields,
      WorkItemExtensionPredicate predicate,
      IList<WorkItemFieldRule> fieldRules,
      string form)
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
    }

    public override List<WorkItemTypeletRecord> GetExtensionsById(IList<Guid> ids)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeExtensionsByIds");
      this.BindGuidTable("@ids", (IEnumerable<Guid>) ids);
      this.BindBoolean("@includePredicate", true);
      this.BindBoolean("@includeRules", true);
      this.BindBoolean("@includeFields", true);
      this.BindBoolean("@includeForm", true);
      return this.ReadExtensions();
    }

    public override List<WorkItemTypeletRecord> GetExtensions(Guid? projectId, Guid? ownerId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeExtensions");
      if (projectId.HasValue)
        this.BindGuid("@projectId", projectId.Value);
      else
        this.BindNullValue("@projectId", SqlDbType.UniqueIdentifier);
      if (ownerId.HasValue)
        this.BindGuid("@ownerId", ownerId.Value);
      else
        this.BindNullValue("@ownerId", SqlDbType.UniqueIdentifier);
      this.BindBoolean("@includePredicate", true);
      this.BindBoolean("@includeRules", true);
      this.BindBoolean("@includeFields", true);
      this.BindBoolean("@includeForm", true);
      return this.ReadExtensions();
    }
  }
}
