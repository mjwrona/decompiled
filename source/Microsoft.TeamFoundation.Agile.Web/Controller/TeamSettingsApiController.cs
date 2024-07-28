// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.TeamSettingsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "teamsettings")]
  public class TeamSettingsApiController : TeamSettingsApiControllerBase
  {
    [HttpGet]
    [ClientExample("GET__work_teamsettings.json", "Get team settings for a team", null, null)]
    public TeamSetting GetTeamSettings()
    {
      this.TfsRequestContext.TraceEnter(290110, "AgileService", "AgileService", nameof (GetTeamSettings));
      try
      {
        ITeamSettings settingsInternal = this.GetTeamSettingsInternal();
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode1 = this.GetTreeNode(settingsInternal.BacklogIterationId);
        this.GetIterationLinkUrl(settingsInternal.BacklogIterationId);
        TeamSetting teamSettings = new TeamSetting();
        teamSettings.BacklogIteration = new TeamSettingsIteration()
        {
          Id = settingsInternal.BacklogIterationId
        };
        teamSettings.BugsBehavior = settingsInternal.BugsBehavior.ToWebApiBugsBehavior();
        teamSettings.WorkingDays = settingsInternal.Weekends.Days.GetRemainingDays();
        teamSettings.BacklogVisibilities = settingsInternal.BacklogVisibilities;
        string resourceUriString = this.GetAgileResourceUriString(TeamSettingsApiConstants.LocationId);
        ReferenceLinks referenceLinks = this.GetReferenceLinks(resourceUriString, TeamSettingsApiControllerBase.CommonUrlLink.TeamFieldValues | TeamSettingsApiControllerBase.CommonUrlLink.Iterations);
        if (treeNode1 != null)
        {
          teamSettings.BacklogIteration.Name = treeNode1.GetSanitizedName(this.TfsRequestContext);
          teamSettings.BacklogIteration.Path = treeNode1.RelativePath;
          teamSettings.BacklogIteration.Url = this.GetClassificationNodeUrl(treeNode1);
          this.AddClassificationNodeLink(referenceLinks, treeNode1);
        }
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode2 = (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null;
        if (settingsInternal.DefaultIterationId != Guid.Empty)
          treeNode2 = this.GetTreeNode(settingsInternal.DefaultIterationId);
        if (treeNode2 == null && (string.IsNullOrEmpty(settingsInternal.DefaultIterationMacro) || WiqlOperators.IsCurrentIterationMacro(settingsInternal.DefaultIterationMacro, false)))
        {
          teamSettings.DefaultIterationMacro = "@currentIteration";
          if (settingsInternal.Iterations != null && settingsInternal.Iterations.Count<ITeamIteration>() > 0)
          {
            Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode3 = (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null;
            try
            {
              treeNode3 = settingsInternal.GetCurrentIterationNode(this.TfsRequestContext, this.ProjectId);
            }
            catch (InvalidTeamSettingsException ex)
            {
              this.TfsRequestContext.TraceCatch(290122, TraceLevel.Info, "AgileService", "AgileService", (Exception) ex);
            }
            catch (Exception ex)
            {
              this.TfsRequestContext.TraceCatch(290123, "AgileService", "AgileService", ex);
            }
            if (treeNode3 != null)
              treeNode2 = treeNode3;
          }
        }
        if (treeNode2 != null)
        {
          TeamSetting teamSetting = teamSettings;
          TeamSettingsIteration settingsIteration = new TeamSettingsIteration();
          settingsIteration.Id = treeNode2.CssNodeId;
          settingsIteration.Name = treeNode2.GetSanitizedName(this.TfsRequestContext);
          settingsIteration.Path = treeNode2.RelativePath;
          settingsIteration.Url = this.GetClassificationNodeUrl(treeNode2);
          teamSetting.DefaultIteration = settingsIteration;
          this.AddClassificationNodeLink(referenceLinks, treeNode2);
        }
        teamSettings.Url = resourceUriString;
        teamSettings.Links = referenceLinks;
        return teamSettings;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290120, "AgileService", "AgileService", nameof (GetTeamSettings));
      }
    }

    [HttpPatch]
    [ClientExample("PATCH__work_teamsettings.json", "Update team settings. Example 1", null, null)]
    [ClientExample("PATCH__work_teamsettings2.json", "Update team settings. Example 2", null, null)]
    public TeamSetting UpdateTeamSettings([FromBody] TeamSettingsPatch teamSettingsPatch)
    {
      this.TfsRequestContext.TraceEnter(290121, "AgileService", "AgileService", "PatchTeamSettings");
      try
      {
        ArgumentUtility.CheckForNull<TeamSettingsPatch>(teamSettingsPatch, nameof (teamSettingsPatch));
        Guid? backlogIteration = teamSettingsPatch.BacklogIteration;
        Guid? defaultIteration = teamSettingsPatch.DefaultIteration;
        DayOfWeek[] workingDays = teamSettingsPatch.WorkingDays;
        ITeamConfigurationService service = this.TfsRequestContext.GetService<ITeamConfigurationService>();
        ITeamSettings settingsInternal = this.GetTeamSettingsInternal(true);
        if (defaultIteration.HasValue && defaultIteration.HasValue && !string.IsNullOrEmpty(teamSettingsPatch.DefaultIterationMacro))
        {
          this.TfsRequestContext.Trace(290116, TraceLevel.Error, "AgileService", "AgileService", "Both defaultIterationId and defaultIterationMacro are set: defaultIterationId {0}, defaultIterationMacro {1}, project {2} and Team {3}", (object) defaultIteration.ToString(), (object) teamSettingsPatch.DefaultIterationMacro, (object) this.ProjectId, (object) this.TeamId);
          throw new InvalidTeamSettingsForUpdateException(Microsoft.TeamFoundation.Agile.Server.AgileResources.InvalidDefaultIterationParameters);
        }
        if (defaultIteration.HasValue && defaultIteration.HasValue && defaultIteration.Value != settingsInternal.DefaultIterationId || !string.IsNullOrEmpty(teamSettingsPatch.DefaultIterationMacro))
        {
          string defaultIterationMacro;
          if (defaultIteration.HasValue && defaultIteration.HasValue)
          {
            if (this.GetTreeNode(defaultIteration.Value) == null)
            {
              this.TfsRequestContext.Trace(290115, TraceLevel.Error, "AgileService", "AgileService", "Could not find TreeNode for DefaultIterationId {0}, for the Project {1} and Team {2}", (object) defaultIteration.ToString(), (object) this.ProjectId, (object) this.TeamId);
              throw new InvalidTeamSettingsForUpdateException(string.Format(Microsoft.TeamFoundation.Agile.Server.AgileResources.InvalidIterationPathMessage, (object) defaultIteration.Value.ToString()));
            }
            defaultIterationMacro = defaultIteration.Value.ToString();
          }
          else if (WiqlOperators.IsCurrentIterationMacro(teamSettingsPatch.DefaultIterationMacro, false))
          {
            defaultIterationMacro = teamSettingsPatch.DefaultIterationMacro;
          }
          else
          {
            this.TfsRequestContext.Trace(290117, TraceLevel.Error, "AgileService", "AgileService", "Invalid iteration macro", (object) this.ProjectId, (object) this.TeamId);
            throw new InvalidTeamSettingsForUpdateException(string.Format(Microsoft.TeamFoundation.Agile.Server.AgileResources.InvalidIterationMacroMessage, (object) teamSettingsPatch.DefaultIterationMacro));
          }
          if (!string.IsNullOrEmpty(defaultIterationMacro))
            service.SaveDefaultIteration(this.TfsRequestContext, this.Team, defaultIterationMacro);
        }
        if (backlogIteration.HasValue)
        {
          Guid? nullable = backlogIteration;
          Guid backlogIterationId = settingsInternal.BacklogIterationId;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != backlogIterationId ? 1 : 0) : 0) : 1) != 0)
          {
            if (this.GetTreeNode(backlogIteration.Value) == null)
            {
              this.TfsRequestContext.Trace(290111, TraceLevel.Error, "AgileService", "AgileService", "Could not find TreeNode for BacklogIterationId {0}, for the Project {1} and Team {2}", (object) settingsInternal.BacklogIterationId, (object) this.ProjectId, (object) this.TeamId);
              throw new InvalidTeamSettingsForUpdateException(string.Format(Microsoft.TeamFoundation.Agile.Server.AgileResources.InvalidIterationPathMessage, (object) backlogIteration.Value.ToString()));
            }
            service.SaveBacklogIterations(this.TfsRequestContext, this.Team, settingsInternal.Iterations.Select<ITeamIteration, Guid>((Func<ITeamIteration, Guid>) (i => i.IterationId)), backlogIteration.Value);
          }
        }
        Microsoft.TeamFoundation.Work.WebApi.BugsBehavior? bugsBehavior = teamSettingsPatch.BugsBehavior;
        if (bugsBehavior.HasValue)
        {
          bugsBehavior = teamSettingsPatch.BugsBehavior;
          Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior serverBugsBehavior = bugsBehavior.Value.ToServerBugsBehavior();
          if (serverBugsBehavior != settingsInternal.BugsBehavior)
            service.SetBugsBehavior(this.TfsRequestContext, this.Team, serverBugsBehavior);
        }
        if (workingDays != null && ((IEnumerable<DayOfWeek>) workingDays).Any<DayOfWeek>())
          service.SaveTeamWeekends(this.TfsRequestContext, this.Team, (ITeamWeekends) new TeamWeekends()
          {
            Days = workingDays.GetRemainingDays()
          });
        if (teamSettingsPatch.BacklogVisibilities != null && teamSettingsPatch.BacklogVisibilities.Any<KeyValuePair<string, bool>>())
          service.SetBacklogVisibilities(this.TfsRequestContext, this.Team, teamSettingsPatch.BacklogVisibilities);
        return this.GetTeamSettings();
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290130, "AgileService", "AgileService", "PatchTeamSettings");
      }
    }
  }
}
