// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent15
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent15 : WorkItemTypeExtensionComponent14
  {
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
      fieldRules.Select<WorkItemFieldRule, string>((System.Func<WorkItemFieldRule, string>) (r => r.Field)).ToList<string>().ForEach((Action<string>) (r => ArgumentUtility.CheckStringForNullOrEmpty(r, "fieldRules.Select(r => r.Field)")));
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
      this.BindString("@color", color, 10, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (states != null)
        this.BindStates("@states", (IEnumerable<WorkItemStateDeclaration>) states);
      if (isDisabled.HasValue)
        this.BindBoolean("@disabled", isDisabled.Value);
      WorkItemTypeletRecord workItemTypelet = this.ReadExtensions((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder3()).FirstOrDefault<WorkItemTypeletRecord>();
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
      this.BindString("@color", color, 10, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (isDisabled.HasValue)
        this.BindBoolean("@disabled", isDisabled.Value);
      this.BindDateTime("@readVersion", readVersion);
      WorkItemTypeletRecord itemTypeletRecord = this.ReadExtensions((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder3()).FirstOrDefault<WorkItemTypeletRecord>();
      this.EnsureConstantsForBackcompat(fieldRules);
      return itemTypeletRecord;
    }

    public override List<WorkItemTypeletRecord> GetWorkItemTypelets(Guid processId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypelets");
      this.BindGuid("@processId", processId);
      return this.ReadTypelets((ObjectBinder<WorkItemTypeletRecord>) new WorkItemTypeletRecordBinder3());
    }
  }
}
