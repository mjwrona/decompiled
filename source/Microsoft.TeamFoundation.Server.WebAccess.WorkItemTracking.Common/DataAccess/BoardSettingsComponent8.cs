// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent8
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class BoardSettingsComponent8 : BoardSettingsComponent7
  {
    protected void BindParamsSetCardRules(
      Guid projectId,
      string scope,
      Guid boardId,
      IEnumerable<BoardCardRuleRow> rules,
      IEnumerable<RuleAttributeRow> attributes)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(boardId, nameof (boardId));
      ArgumentUtility.CheckForNull<IEnumerable<BoardCardRuleRow>>(rules, nameof (rules));
      ArgumentUtility.CheckForNull<IEnumerable<RuleAttributeRow>>(attributes, nameof (attributes));
      ArgumentUtility.CheckStringForNullOrEmpty(scope, nameof (scope));
      this.PrepareStoredProcedure("prc_SetBoardCardRules");
      this.BindDataspace(projectId);
      this.BindString("@boardScope", scope, (int) byte.MaxValue, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid("@boardScopeId", boardId);
      this.BindTable("@rules", "typ_CardRuleTable", TVPBinders.BindCardRuleRows(rules));
      this.BindTable("@attributes", "typ_CardRuleAttributeTable2", TVPBinders.BindCardStyleRows2(attributes));
    }

    public override void SetCardRules(
      Guid projectId,
      string scope,
      Guid boardId,
      IEnumerable<BoardCardRuleRow> rules,
      IEnumerable<RuleAttributeRow> attributes)
    {
      this.BindParamsSetCardRules(projectId, scope, boardId, rules, attributes);
      this.ExecuteNonQuery();
    }

    public override void UpdateCardRules(
      Guid projectId,
      string scope,
      Guid boardId,
      IEnumerable<BoardCardRuleRow> rules,
      IEnumerable<RuleAttributeRow> attributes,
      List<string> typesToBeDeleted)
    {
      this.SetCardRules(projectId, scope, boardId, rules, attributes);
    }
  }
}
