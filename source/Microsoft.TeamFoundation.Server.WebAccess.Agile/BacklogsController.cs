// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BacklogsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Extensions;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [SupportedRouteArea(NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("EC7545A3-E5DB-40E8-B0D0-F64DF7619BBA", false)]
  [RequireTeam]
  [RegisterHubMruPage(true)]
  [OutputCache(CacheProfile = "NoCache")]
  public class BacklogsController : AgileAreaController
  {
    private BacklogContext m_backlogContext;
    private BacklogsBacklogActionHelper m_backlogActionHelper;
    private BacklogsIterationActionHelper m_iterationActionHelper;
    private BacklogsBoardActionHelper m_boardActionHelper;

    public BacklogsBacklogActionHelper BacklogActionHelper => this.m_backlogActionHelper ?? (this.m_backlogActionHelper = new BacklogsBacklogActionHelper(this));

    public BacklogsIterationActionHelper IterationActionHelper => this.m_iterationActionHelper ?? (this.m_iterationActionHelper = new BacklogsIterationActionHelper(this));

    public BacklogsBoardActionHelper BoardActionHelper => this.m_boardActionHelper ?? (this.m_boardActionHelper = new BacklogsBoardActionHelper(this));

    public virtual BacklogContext RequestBacklogContext
    {
      get
      {
        if (this.m_backlogContext == null)
        {
          using (WebPerformanceTimer.StartMeasure(this.TfsWebContext.RequestContext, "BacklogsController.RequestBacklogContext"))
          {
            string levelFromPath = this.GetLevelFromPath();
            bool progressFilterState = this.BacklogActionHelper.GetInProgressFilterState();
            if (levelFromPath != null)
              this.m_backlogContext = BacklogContextHelpers.GetBacklogContext(this.TfsWebContext.TfsRequestContext, this.Settings.BacklogConfiguration, this.Settings.Team, levelFromPath, progressFilterState);
            else
              this.m_backlogContext = BacklogContextHelpers.CreateBacklogContext(this.TfsWebContext.TfsRequestContext, this.Settings.Team, (this.Settings.BacklogConfiguration.GetLowestVisibleBacklogLevel() ?? throw new BacklogInvalidContextException(AgileServerResources.ProductBacklog_RequestContext_LowestVisibleLevelNull)).Name, this.Settings.BacklogConfiguration, progressFilterState);
          }
        }
        return this.m_backlogContext;
      }
    }

    public BacklogContext GetRequirementsLevelBacklogContext()
    {
      string name = this.Settings.BacklogConfiguration.RequirementBacklog.Name;
      return this.m_backlogContext != null && TFStringComparer.BacklogPluralName.Equals(this.m_backlogContext.LevelName, name) ? this.m_backlogContext : new BacklogContext(this.Settings.Team, this.Settings.BacklogConfiguration.RequirementBacklog, BacklogContextHelpers.GetBacklogPortfolioNames(this.TfsWebContext.TfsRequestContext, this.Settings.BacklogConfiguration));
    }

    public void SetBacklogContext(BacklogContext backlogContext)
    {
      ArgumentUtility.CheckForNull<BacklogContext>(backlogContext, nameof (backlogContext));
      this.m_backlogContext = backlogContext;
    }

    protected void SetBacklogContext(string levelPluralName, bool showParents)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(levelPluralName, "level");
      BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
      if (!this.Settings.BacklogConfiguration.TryGetBacklogByName(levelPluralName, out backlogLevel) || backlogLevel == this.Settings.BacklogConfiguration.TaskBacklog || !this.Settings.BacklogConfiguration.IsBacklogVisible(backlogLevel.Id))
        backlogLevel = this.Settings.BacklogConfiguration.PortfolioBacklogs.Concat<BacklogLevelConfiguration>((IEnumerable<BacklogLevelConfiguration>) new BacklogLevelConfiguration[1]
        {
          this.Settings.BacklogConfiguration.RequirementBacklog
        }).LastOrDefault<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (x => this.Settings.BacklogConfiguration.IsBacklogVisible(x.Id))) ?? this.Settings.BacklogConfiguration.RequirementBacklog;
      if (backlogLevel == this.Settings.BacklogConfiguration.GetAllBacklogLevels().OrderByDescending<BacklogLevelConfiguration, int>((Func<BacklogLevelConfiguration, int>) (bd => bd.Rank)).First<BacklogLevelConfiguration>())
        showParents = false;
      BacklogContext backlogContext = new BacklogContext(this.Settings.Team, backlogLevel, BacklogContextHelpers.GetBacklogPortfolioNames(this.TfsWebContext.TfsRequestContext, this.Settings.BacklogConfiguration));
      backlogContext.IncludeParents = showParents;
      backlogContext.ShowInProgress = this.BacklogActionHelper.GetInProgressFilterState();
      backlogContext.ActionNameFromMru = this.RequestBacklogContext.ActionNameFromMru;
      this.ViewData["ProductBacklogAnchoredLevel"] = (object) levelPluralName;
      this.ViewData["ProductBacklogShowParents"] = (object) showParents;
      this.SetBacklogContext(backlogContext);
    }

    private string GetLevelFromPath()
    {
      object obj = this.RouteData.Values["parameters"];
      if (obj != null)
      {
        string[] strArray = obj.ToString().Split('/');
        if (strArray.Length != 0)
          return this.GetLevelFromIdOrName(strArray[0]);
      }
      return (string) null;
    }

    private string GetLevelFromIdOrName(string pathLevelParameter)
    {
      Guid result;
      if (Guid.TryParse(pathLevelParameter, out result) && !result.Equals(Guid.Empty) && this.Team != null)
      {
        BoardRecord boardRecordById = KanbanUtils.Instance.GetBoardRecordById(this.TfsRequestContext.GetService<BoardService>(), this.TfsRequestContext, this.Team.Id, result);
        if (boardRecordById != null)
        {
          BacklogLevelConfiguration backlogLevel = (BacklogLevelConfiguration) null;
          if (this.BoardActionHelper.Settings.BacklogConfiguration.TryGetBacklogLevelConfiguration(boardRecordById.BacklogLevelId, out backlogLevel))
            return backlogLevel.Name;
        }
      }
      return pathLevelParameter;
    }
  }
}
