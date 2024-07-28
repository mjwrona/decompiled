// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AutomationRules.AutomationRulesService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.AutomationRules
{
  public class AutomationRulesService : IAutomaitonRulesService, IVssFrameworkService
  {
    private const string ActivateParentWhenAnyChildActivated = "ActivateParentWhenAnyChildActivated";
    private const string CompleteParentWhenAllChildrenCompleted = "CompleteParentWhenAllChildrenCompleted";
    private const string ResolveParentWhenAllChildrenResolved = "ResolveParentWhenAllChildrenResolved";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<TeamAutomationRulesSettings> GetTeamAutomationRulesSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      using (TeamAutomationRulesComponent component = requestContext.CreateComponent<TeamAutomationRulesComponent>())
        return component.GetTeamAutomationRulesSettings(projectId, new Guid?(teamId));
    }

    public IEnumerable<TeamAutomationRulesSettings> GetTeamAutomationRulesSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      int areaId)
    {
      using (TeamAutomationRulesComponent component = requestContext.CreateComponent<TeamAutomationRulesComponent>())
      {
        TeamAutomationRulesComponent automationRulesComponent = component;
        Guid projectId1 = projectId;
        int? nullable = new int?(areaId);
        Guid? teamId = new Guid?();
        int? areaId1 = nullable;
        return automationRulesComponent.GetTeamAutomationRulesSettings(projectId1, teamId, areaId1);
      }
    }

    public void UpdateWorkItemAutomationRule(
      IVssRequestContext requestContext,
      WebApiTeam team,
      Guid projectId,
      TeamAutomationRulesSettings teamAutomationRulesSettings)
    {
      if (!requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.Backlog.TeamAutomationRulesEnabled"))
        throw new InvalidOperationException();
      if (!requestContext.GetService<ITeamService>().UserIsTeamAdmin(requestContext, team.Identity))
        throw Microsoft.Azure.Devops.Teams.Service.TeamSecurityException.CreateGenericWritePermissionException();
      (bool, bool) resolvedSettingsState = this.GetCompletedAndResolvedSettingsState(teamAutomationRulesSettings?.RulesStates);
      if (resolvedSettingsState.Item1 & resolvedSettingsState.Item2)
        throw new InvalidOperationException("Resolved and Completed categories can't be enabled at once");
      TeamAutomationRulesSettings ruleSetting = this.CreateRuleSetting(teamAutomationRulesSettings);
      using (TeamAutomationRulesComponent component = requestContext.CreateComponent<TeamAutomationRulesComponent>())
        component.UpdateWorkItemAutomationRulesSettings(projectId, ruleSetting);
    }

    private (bool, bool) GetCompletedAndResolvedSettingsState(IDictionary<string, bool> rulesStates)
    {
      if (rulesStates == null)
        return (false, false);
      bool flag1;
      rulesStates.TryGetValue("CompleteParentWhenAllChildrenCompleted", out flag1);
      bool flag2;
      rulesStates.TryGetValue("ResolveParentWhenAllChildrenResolved", out flag2);
      return (flag1, flag2);
    }

    private TeamAutomationRulesSettings CreateRuleSetting(
      TeamAutomationRulesSettings workItemAutomationRuleSettings)
    {
      return new TeamAutomationRulesSettings(workItemAutomationRuleSettings.TeamId, workItemAutomationRuleSettings.BacklogLevelId, workItemAutomationRuleSettings.RulesStates);
    }
  }
}
