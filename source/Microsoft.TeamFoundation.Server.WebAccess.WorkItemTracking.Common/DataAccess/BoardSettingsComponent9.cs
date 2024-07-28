// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.BoardSettingsComponent9
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
  internal class BoardSettingsComponent9 : BoardSettingsComponent8
  {
    public override void UpdateCardRules(
      Guid projectId,
      string scope,
      Guid boardId,
      IEnumerable<BoardCardRuleRow> rules,
      IEnumerable<RuleAttributeRow> attributes,
      List<string> typesToBeDeleted)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(boardId, nameof (boardId));
      ArgumentUtility.CheckForNull<IEnumerable<BoardCardRuleRow>>(rules, nameof (rules));
      ArgumentUtility.CheckForNull<IEnumerable<RuleAttributeRow>>(attributes, nameof (attributes));
      ArgumentUtility.CheckStringForNullOrEmpty(scope, nameof (scope));
      ArgumentUtility.CheckForNull<List<string>>(typesToBeDeleted, nameof (typesToBeDeleted));
      this.PrepareStoredProcedure("prc_UpdateBoardCardRules");
      this.BindDataspace(projectId);
      this.BindString("@boardScope", scope, (int) byte.MaxValue, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindGuid("@boardScopeId", boardId);
      this.BindTable("@rules", "typ_CardRuleTable", TVPBinders.BindCardRuleRows(rules));
      this.BindTable("@attributes", "typ_CardRuleAttributeTable2", TVPBinders.BindCardStyleRows2(attributes));
      this.BindTable("@typesToBeDeleted", "typ_CardRuleTypeTable", TVPBinders.BindCardRuleType(typesToBeDeleted));
      this.ExecuteNonQuery();
    }
  }
}
