// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardsRestApiHelper
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  internal class BoardsRestApiHelper : IBoardsRestApiHelper
  {
    private IAgileSettings m_AgileSettings;
    private Dictionary<string, string> m_LowerBacklogLevelName2BacklogLevelId;
    private Dictionary<string, string> m_BacklogLevelId2BacklogLevelName;
    private List<string> m_backlogLevelIds;

    internal BoardsRestApiHelper()
    {
    }

    public BoardsRestApiHelper(
      IVssRequestContext tfsRequestContext,
      ProjectInfo project,
      WebApiTeam team)
    {
      this.TfsRequestContext = tfsRequestContext;
      this.Team = team;
      this.ProjectInfo = project;
    }

    public string GetBoardBacklogLevelIdByNameOrId(BoardService boardService, string board)
    {
      Guid result;
      if (Guid.TryParse(board, out result))
      {
        ArgumentUtility.CheckForEmptyGuid(result, "boardId");
        BoardRecord boardMatchId = KanbanUtils.Instance.GetBoardRecordById(boardService, this.TfsRequestContext, this.Team.Id, result);
        if (boardMatchId == null || !this.AgileSettings.BacklogConfiguration.GetAllBacklogLevels().Any<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, bool>) (b => TFStringComparer.BacklogLevelId.Equals(b.Id, boardMatchId.BacklogLevelId))))
          throw new BoardDoesNotExistException();
        return boardMatchId.BacklogLevelId;
      }
      if (this.LowerBacklogLevelName2BacklogLevelId.ContainsKey(board))
        return this.LowerBacklogLevelName2BacklogLevelId[board];
      if (this.LowerBacklogLevelName2BacklogLevelId.Values.Any<string>((Func<string, bool>) (value => TFStringComparer.BacklogLevelId.Equals(value, board))))
        return board;
      throw new BoardDoesNotExistException();
    }

    public Guid GetBoardIdFromNameOrId(BoardService boardService, string board) => this.GetOrCreateBoardSettings(this.GetBoardBacklogLevelIdByNameOrId(boardService, board)).Id.Value;

    public Dictionary<string, BoardCardRules> ConvertCardRulesToRowView(
      BoardCardRuleSettings boardCardRuleSettings)
    {
      ArgumentUtility.CheckForNull<BoardCardRuleSettings>(boardCardRuleSettings, nameof (boardCardRuleSettings));
      ArgumentUtility.CheckForNull<Dictionary<string, List<Microsoft.TeamFoundation.Work.WebApi.Rule>>>(boardCardRuleSettings.rules, "rules");
      Dictionary<string, BoardCardRules> rowView = new Dictionary<string, BoardCardRules>();
      foreach (string key in boardCardRuleSettings.rules.Keys)
      {
        BoardCardRules boardCardRules = new BoardCardRules();
        int order = 1;
        List<Microsoft.TeamFoundation.Work.WebApi.Rule> rule1 = boardCardRuleSettings.rules[key];
        ArgumentUtility.CheckForNull<List<Microsoft.TeamFoundation.Work.WebApi.Rule>>(rule1, "boardCardRuleSettings.rules." + key);
        foreach (Microsoft.TeamFoundation.Work.WebApi.Rule rule2 in rule1)
        {
          ArgumentUtility.CheckForNull<attribute>(rule2.settings, "rule");
          ArgumentUtility.CheckForNull<attribute>(rule2.settings, "settings");
          bool result;
          if (!bool.TryParse(rule2.isEnabled, out result))
            throw new CardRulesValidationFailureException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.TeamFoundation.Agile.Server.AgileResources.UpdateBoardCardRules_isEnabledBoolean));
          BoardCardRuleRow boardCardRuleRow;
          try
          {
            if (!string.IsNullOrEmpty(rule2.filter))
            {
              rule2.filter = CardSettingsUtils.Instance.TransformWiqlNamesToIds(this.TfsRequestContext, rule2.filter);
              boardCardRuleRow = new BoardCardRuleRow(rule2.name, key, result, order, DateTime.UtcNow, rule2.filter, CardSettingsUtils.Instance.GetFilterModel(rule2.filter, this.TfsRequestContext));
            }
            else
            {
              rule2.filter = string.Empty;
              boardCardRuleRow = new BoardCardRuleRow(rule2.name, key, result, order, DateTime.UtcNow, rule2.filter, (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel) null);
            }
          }
          catch (Exception ex)
          {
            throw new CardRulesValidationFailureException(ex.Message);
          }
          foreach (KeyValuePair<string, string> setting in (Dictionary<string, string>) rule2.settings)
          {
            RuleAttributeRow ruleAttributeRow = new RuleAttributeRow(rule2.name, setting.Key, setting.Value, key);
            boardCardRules.Attributes.Add(ruleAttributeRow);
          }
          boardCardRules.Rules.Add(boardCardRuleRow);
          ++order;
        }
        rowView[key] = boardCardRules;
      }
      return rowView;
    }

    public virtual BoardSettings GetOrCreateBoardSettings(string backlogLevelId)
    {
      BoardSettings boardSettings = KanbanUtils.Instance.GetOrCreateBoardSettings(this.TfsRequestContext, this.CssProjectInfo, this.Team, this.AgileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(backlogLevelId), 0);
      if (boardSettings != null)
        return boardSettings;
      this.TfsRequestContext.Trace(290099, TraceLevel.Verbose, "AgileService", "AgileService", "Failed to get board with category reference name {0}, team Id: {1}.", (object) backlogLevelId, (object) this.Team.Id);
      throw new BoardDoesNotExistException();
    }

    public virtual BoardSettings GetBoardSettings(string backlogLevelId)
    {
      BoardSettings boardSettings = KanbanUtils.Instance.GetBoardSettings(this.TfsRequestContext, this.Team, this.AgileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(backlogLevelId), this.CssProjectInfo);
      if (boardSettings != null)
        return boardSettings;
      this.TfsRequestContext.Trace(290099, TraceLevel.Verbose, "AgileService", "AgileService", "Failed to get board with category reference name {0}, team Id: {1}.", (object) backlogLevelId, (object) this.Team.Id);
      throw new BoardDoesNotExistException();
    }

    public Dictionary<string, string> LowerBacklogLevelName2BacklogLevelId
    {
      get
      {
        if (this.m_LowerBacklogLevelName2BacklogLevelId == null)
          this.m_LowerBacklogLevelName2BacklogLevelId = this.AgileSettings.BacklogConfiguration.GetAllBacklogLevels().Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, bool>) (backlogLevel => !string.IsNullOrEmpty(backlogLevel?.Name) && this.ApplicableBacklogLevelIds.Contains(backlogLevel.Id))).ToDictionary<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, string, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, string>) (backlogLevel => backlogLevel.Name), (Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, string>) (backlogLevel => backlogLevel.Id), (IEqualityComparer<string>) TFStringComparer.BacklogPluralName);
        return this.m_LowerBacklogLevelName2BacklogLevelId;
      }
    }

    public Dictionary<string, string> BacklogLevelId2BacklogLevelName
    {
      get
      {
        if (this.m_BacklogLevelId2BacklogLevelName == null)
          this.m_BacklogLevelId2BacklogLevelName = this.AgileSettings.BacklogConfiguration.GetAllBacklogLevels().ToDictionary<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, string, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, string>) (backlogLevel => backlogLevel.Id), (Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, string>) (backlogLevel => backlogLevel.Name));
        return this.m_BacklogLevelId2BacklogLevelName;
      }
    }

    public List<string> ApplicableBacklogLevelIds
    {
      get
      {
        if (this.m_backlogLevelIds == null)
        {
          this.m_backlogLevelIds = new List<string>()
          {
            this.AgileSettings.BacklogConfiguration.RequirementBacklog.Id
          };
          this.AgileSettings.BacklogConfiguration.PortfolioBacklogs.ForEach<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration>((Action<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration>) (backlog => this.m_backlogLevelIds.Add(backlog.Id)));
        }
        return this.m_backlogLevelIds;
      }
    }

    public virtual IAgileSettings AgileSettings
    {
      get
      {
        if (this.m_AgileSettings == null)
        {
          ArgumentUtility.CheckForNull<WebApiTeam>(this.Team, "TfsTeamApiController.Team");
          this.m_AgileSettings = (IAgileSettings) new Microsoft.TeamFoundation.Agile.Server.AgileSettings(this.TfsRequestContext, this.CssProjectInfo, this.Team);
        }
        return this.m_AgileSettings;
      }
    }

    public CommonStructureProjectInfo CssProjectInfo => CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo);

    internal virtual IVssRequestContext TfsRequestContext { get; }

    internal virtual WebApiTeam Team { get; }

    internal virtual ProjectInfo ProjectInfo { get; }
  }
}
