// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardCardStyleSettingsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "cardrulesettings")]
  public class BoardCardStyleSettingsApiController : BoardsApiControllerBase
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.NoPermissionUpdateCardRulesException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.CardRulesUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.NoPermissionUpdateCardRulesException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.CardRulesUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.NoPermissionUpdateCardRulesException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.NoPermissionUpdateCardRulesException));
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.CardRulesUpdateFailureException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.CardRulesUpdateFailureException));
    }

    private BoardCardRuleSettings CreateCardRuleRowsModel(List<CardRule> rules, string board)
    {
      BoardCardRuleSettings boardCardRuleSettings = new BoardCardRuleSettings();
      boardCardRuleSettings.rules = new Dictionary<string, List<Microsoft.TeamFoundation.Work.WebApi.Rule>>();
      IEnumerable<IGrouping<string, CardRule>> groupings = (IEnumerable<IGrouping<string, CardRule>>) null;
      if (rules != null)
        groupings = rules.GroupBy<CardRule, string>((Func<CardRule, string>) (u => u.Type), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (groupings != null)
      {
        foreach (IGrouping<string, CardRule> grouping in groupings)
        {
          boardCardRuleSettings.rules[grouping.Key] = new List<Microsoft.TeamFoundation.Work.WebApi.Rule>();
          foreach (CardRule cardRule in (IEnumerable<CardRule>) grouping)
          {
            Microsoft.TeamFoundation.Work.WebApi.Rule rule = new Microsoft.TeamFoundation.Work.WebApi.Rule();
            rule.filter = !cardRule.QueryText.IsNullOrEmpty<char>() ? cardRule.QueryText : (string) null;
            rule.isEnabled = cardRule.IsEnabled.ToString();
            rule.name = cardRule.Name;
            rule.settings = new attribute();
            if (cardRule.QueryExpression != null)
            {
              rule.Clauses = (ICollection<Microsoft.TeamFoundation.Work.WebApi.FilterClause>) new List<Microsoft.TeamFoundation.Work.WebApi.FilterClause>();
              foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause clause in (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>) cardRule.QueryExpression.Clauses)
              {
                Microsoft.TeamFoundation.Work.WebApi.FilterClause filterClause = new Microsoft.TeamFoundation.Work.WebApi.FilterClause(clause.FieldName, clause.Index, clause.LogicalOperator, clause.Operator, clause.Value);
                rule.Clauses.Add(filterClause);
              }
            }
            else
              rule.Clauses = (ICollection<Microsoft.TeamFoundation.Work.WebApi.FilterClause>) null;
            if (cardRule.StyleAttributes != null)
            {
              foreach (KeyValuePair<string, string> styleAttribute in cardRule.StyleAttributes)
                rule.settings[styleAttribute.Key] = styleAttribute.Value;
            }
            else
              rule.settings = (attribute) null;
            boardCardRuleSettings.rules[grouping.Key].Add(rule);
          }
        }
      }
      return this.addLinksandUrl(boardCardRuleSettings, board);
    }

    private BoardCardRuleSettings addLinksandUrl(
      BoardCardRuleSettings boardCardRuleSettings,
      string board)
    {
      boardCardRuleSettings.url = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, BoardApiConstants.BoardCardStyleSettingsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        board = board,
        name = "cardrulesettings"
      });
      BoardCardRuleSettings cardRuleSettings = boardCardRuleSettings;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid guid = this.ProjectId;
      string projectId = guid.ToString();
      guid = this.TeamId;
      string teamId = guid.ToString();
      string board1 = board;
      string url = boardCardRuleSettings.url;
      ReferenceLinks referenceLinks = this.GetReferenceLinks(tfsRequestContext, projectId, teamId, board1, url);
      cardRuleSettings.Links = referenceLinks;
      return boardCardRuleSettings;
    }

    private ReferenceLinks GetReferenceLinks(
      IVssRequestContext tfsRequestContext,
      string projectId,
      string teamId,
      string board,
      string selfUrl)
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", selfUrl);
      referenceLinks.AddLink("cardsettings", AgileResourceUtils.GetAgileResourceUriString(tfsRequestContext, BoardApiConstants.BoardCardSettingsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        board = board,
        name = "cardsettings"
      }));
      referenceLinks.AddLink(nameof (board), AgileResourceUtils.GetAgileResourceUriString(tfsRequestContext, BoardApiConstants.BoardsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        id = board
      }));
      return referenceLinks;
    }

    [HttpGet]
    [ClientLocationId("B044A3D9-02EA-49C7-91A1-B730949CC896")]
    public virtual BoardCardRuleSettings GetBoardCardRuleSettings(string board)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(board, nameof (board));
      BoardSettings boardSettings = this.GetOrCreateBoardSettings(this.GetBoardBacklogLevelIdByNameOrId(this.TfsRequestContext.GetService<BoardService>(), board));
      CardSettingsUtils.Instance.ConvertTagIdToTagName(this.TfsRequestContext, boardSettings.BoardCardSettings.Rules);
      CardSettingsUtils.Instance.TransformWiqls(this.TfsRequestContext, boardSettings.BoardCardSettings.Rules);
      return this.CreateCardRuleRowsModel(boardSettings.BoardCardSettings.Rules, board);
    }

    [NonAction]
    public virtual IDictionary<Guid, string> ConvertTagNametoTagId(List<Microsoft.TeamFoundation.Work.WebApi.Rule> rules)
    {
      Dictionary<Guid, string> dictionary1 = new Dictionary<Guid, string>();
      if (rules != null && rules.Count > 0)
      {
        IEnumerable<string> tagNames = rules.Select<Microsoft.TeamFoundation.Work.WebApi.Rule, string>((Func<Microsoft.TeamFoundation.Work.WebApi.Rule, string>) (rule => rule.name));
        Dictionary<string, TagDefinition> dictionary2 = TaggingServiceUtils.EnsureTagDefinitions_NoActivations(this.TfsRequestContext.Elevate(), tagNames, new Guid[1]
        {
          WorkItemArtifactKinds.WorkItem
        }, this.ProjectId).ToDictionary<TagDefinition, string>((Func<TagDefinition, string>) (t => t.Name), (IEqualityComparer<string>) VssStringComparer.TagName);
        foreach (Microsoft.TeamFoundation.Work.WebApi.Rule rule in rules)
        {
          TagDefinition tagDefinition;
          if (dictionary2.TryGetValue(rule.name, out tagDefinition))
          {
            dictionary1.TryAdd<Guid, string>(tagDefinition.TagId, rule.name);
            rule.name = tagDefinition.TagId.ToString();
          }
        }
      }
      return (IDictionary<Guid, string>) dictionary1;
    }

    [HttpPatch]
    [ClientLocationId("B044A3D9-02EA-49C7-91A1-B730949CC896")]
    public virtual BoardCardRuleSettings UpdateBoardCardRuleSettings(
      BoardCardRuleSettings boardCardRuleSettings,
      string board)
    {
      ArgumentUtility.CheckForNull<BoardCardRuleSettings>(boardCardRuleSettings, nameof (boardCardRuleSettings));
      ArgumentUtility.CheckForNull<Dictionary<string, List<Microsoft.TeamFoundation.Work.WebApi.Rule>>>(boardCardRuleSettings.rules, "boardCardRuleSettings.rules");
      ArgumentUtility.CheckStringForNullOrEmpty(board, nameof (board));
      this.CheckBacklogManagementLicense();
      this.CheckAdminPermission();
      BoardService service = this.TfsRequestContext.GetService<BoardService>();
      Guid boardIdFromNameOrId = this.GetBoardIdFromNameOrId(service, board);
      IDictionary<Guid, string> tagNamesReplacedbyId = (IDictionary<Guid, string>) null;
      foreach (string key in boardCardRuleSettings.rules.Keys)
      {
        if (string.Equals(key, "tagStyle", StringComparison.OrdinalIgnoreCase))
        {
          tagNamesReplacedbyId = this.ConvertTagNametoTagId(boardCardRuleSettings.rules[key]);
          break;
        }
      }
      Dictionary<string, BoardCardRules> rowView = this.ConvertCardRulesToRowView(boardCardRuleSettings);
      this.ValidateBoardCardRules(rowView, boardIdFromNameOrId, tagNamesReplacedbyId);
      BoardCardRules boardCardRules = new BoardCardRules();
      List<string> typesToBeDeleted = new List<string>();
      foreach (string key in rowView.Keys)
      {
        boardCardRules.Attributes = boardCardRules.Attributes.Concat<RuleAttributeRow>((IEnumerable<RuleAttributeRow>) rowView[key].Attributes).ToList<RuleAttributeRow>();
        boardCardRules.Rules = boardCardRules.Rules.Concat<BoardCardRuleRow>((IEnumerable<BoardCardRuleRow>) rowView[key].Rules).ToList<BoardCardRuleRow>();
        typesToBeDeleted.Add(key);
      }
      boardCardRules.ScopeId = boardIdFromNameOrId;
      boardCardRules.Scope = "KANBAN";
      this.UpdateBoardCardRules(service, boardCardRules, typesToBeDeleted);
      return this.GetBoardCardRuleSettings(board);
    }
  }
}
