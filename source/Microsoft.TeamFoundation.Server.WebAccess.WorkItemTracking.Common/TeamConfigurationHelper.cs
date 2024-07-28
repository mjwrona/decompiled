// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamConfigurationHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class TeamConfigurationHelper
  {
    public static void SetTeamConfiguration(
      IVssRequestContext requestContext,
      Guid teamId,
      string backlogIterationPath,
      TeamFieldValue[] teamFieldValues,
      string[] iterations,
      bool skipKanbanBoardProvisioning = false)
    {
      TeamConfigurationHelper.SetTeamConfigurationInternal(requestContext, requestContext.GetService<ITeamService>().GetTeamByGuid(requestContext, teamId) ?? throw new IdentityNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.TeamConfigurationHelper_MissingTeam, (object) teamId)), backlogIterationPath, teamFieldValues, iterations, skipKanbanBoardProvisioning);
    }

    public static void SetTeamConfiguration(
      IVssRequestContext requestContext,
      Guid projectGuid,
      Guid teamId,
      string backlogIterationPath,
      TeamFieldValue[] teamFieldValues,
      string[] iterations,
      bool skipKanbanBoardProvisioning = false)
    {
      TeamConfigurationHelper.SetTeamConfigurationInternal(requestContext, requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, projectGuid, teamId.ToString()) ?? throw new IdentityNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, Resources.TeamConfigurationHelper_MissingTeam, (object) teamId)), backlogIterationPath, teamFieldValues, iterations, skipKanbanBoardProvisioning);
    }

    private static void SetTeamConfigurationInternal(
      IVssRequestContext requestContext,
      WebApiTeam team,
      string backlogIterationPath,
      TeamFieldValue[] teamFieldValues,
      string[] iterations,
      bool skipKanbanBoardProvisioning = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<string>(backlogIterationPath, nameof (backlogIterationPath));
      ArgumentUtility.CheckForNull<TeamFieldValue[]>(teamFieldValues, nameof (teamFieldValues));
      ArgumentUtility.CheckForNull<string[]>(iterations, nameof (iterations));
      using (requestContext.TraceBlock(290060, 290061, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (SetTeamConfigurationInternal)))
      {
        ITeamConfigurationService service = requestContext.GetService<ITeamConfigurationService>();
        bool skipKanbanBoardProvisioning1 = true;
        service.SaveTeamFields(requestContext, team, (ITeamFieldValue[]) teamFieldValues, 0, false, skipKanbanBoardProvisioning1);
        ITreeDictionary snapshot = requestContext.GetService<WorkItemTrackingTreeService>().GetSnapshot(requestContext);
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode1;
        if (!snapshot.TryGetNodeFromPath(requestContext, backlogIterationPath, TreeStructureType.Iteration, out treeNode1))
          throw new WorkItemTrackingTreeNodeNotFoundException(backlogIterationPath);
        if (treeNode1.IsProject)
          treeNode1 = treeNode1.Children.Values.First<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, bool>) (n => n.Type == TreeStructureType.Iteration));
        List<Guid> iterationIds = new List<Guid>();
        foreach (string iteration in iterations)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode2;
          if (!snapshot.TryGetNodeFromPath(requestContext, iteration, TreeStructureType.Iteration, out treeNode2))
            throw new WorkItemTrackingTreeNodeNotFoundException(iteration);
          iterationIds.Add(treeNode2.CssNodeId);
        }
        service.SaveBacklogIterations(requestContext, team, (IEnumerable<Guid>) iterationIds, treeNode1.CssNodeId, skipKanbanBoardProvisioning);
      }
    }

    public static void SetDefaultSettings(
      IVssRequestContext requestContext,
      ProjectInfo project,
      WebApiTeam team,
      TeamAreaAction teamAreaAction)
    {
      TeamConfigurationService service1 = requestContext.GetService<TeamConfigurationService>();
      Guid id = project.Id;
      WorkItemTrackingTreeService service2 = requestContext.GetService<WorkItemTrackingTreeService>();
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> rootTreeNodes = service2.GetSnapshot(requestContext).GetRootTreeNodes(id);
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode1 = rootTreeNodes.First<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, bool>) (n => n.Type == TreeStructureType.Iteration));
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode2 = rootTreeNodes.First<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, bool>) (n => n.Type == TreeStructureType.Area));
      bool skipKanbanBoardProvisioning = teamAreaAction != TeamAreaAction.DoNothing && requestContext.IsTeamFieldAreaPath(project);
      service1.SaveBacklogIterations(requestContext, team, (IEnumerable<Guid>) new List<Guid>(), treeNode1.CssNodeId, skipKanbanBoardProvisioning);
      if (teamAreaAction == TeamAreaAction.DoNothing)
        return;
      Guid guid = Guid.NewGuid();
      if (teamAreaAction == TeamAreaAction.CreateNew)
      {
        ClassificationNodeUpdate classificationNodeUpdate = new ClassificationNodeUpdate()
        {
          Identifier = guid,
          Name = team.Name,
          ParentIdentifier = treeNode2.CssNodeId,
          StructureType = TreeStructureType.Area
        };
        service2.CreateNodes(requestContext, id, (ICollection<ClassificationNodeUpdate>) new ClassificationNodeUpdate[1]
        {
          classificationNodeUpdate
        });
      }
      if (!requestContext.IsTeamFieldAreaPath(project))
        return;
      TeamFieldValue teamFieldValue = new TeamFieldValue()
      {
        Value = guid.ToString(),
        IncludeChildren = TeamConstants.DefaultIncludeChildrenValue
      };
      service1.SaveTeamFields(requestContext, team, (ITeamFieldValue[]) new TeamFieldValue[1]
      {
        teamFieldValue
      }, 0, false, true);
    }

    internal static string GetDefaultValue(this ITeamFieldSettings teamFieldSettings)
    {
      ArgumentUtility.CheckForNull<ITeamFieldSettings>(teamFieldSettings, nameof (teamFieldSettings));
      if (teamFieldSettings.TeamFieldValues != null)
      {
        if (teamFieldSettings.DefaultValueIndex >= 0 && teamFieldSettings.DefaultValueIndex < teamFieldSettings.TeamFieldValues.Length)
          return teamFieldSettings.TeamFieldValues[teamFieldSettings.DefaultValueIndex].Value;
        if (((IEnumerable<ITeamFieldValue>) teamFieldSettings.TeamFieldValues).Any<ITeamFieldValue>())
          return ((IEnumerable<ITeamFieldValue>) teamFieldSettings.TeamFieldValues).First<ITeamFieldValue>().Value;
      }
      return string.Empty;
    }

    internal static void ValidateBacklogIteration(
      Guid iterationId,
      IVssRequestContext requestContext)
    {
      try
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNode(requestContext, iterationId);
        ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(treeNode, "node");
        if (treeNode.Type != TreeStructureType.Iteration)
          throw new InvalidTeamSettingsException(Resources.Settings_InvalidBacklogIteration, TeamSettingsFields.BacklogIteration);
      }
      catch (ArgumentException ex)
      {
        throw new InvalidTeamSettingsException(Resources.Settings_MissingBacklogIteration, TeamSettingsFields.BacklogIteration);
      }
      catch (WorkItemTrackingTreeNodeNotFoundException ex)
      {
        throw new InvalidTeamSettingsException(Resources.Settings_InvalidBacklogIteration, TeamSettingsFields.BacklogIteration);
      }
    }

    internal static void ValidateDefaultIteration(
      IVssRequestContext requestContext,
      string defaultIteration)
    {
      Guid result = Guid.Empty;
      if (Guid.TryParse(defaultIteration, out result))
      {
        try
        {
          if (requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNode(requestContext, result).Type != TreeStructureType.Iteration)
            throw new InvalidTeamSettingsException(Resources.Settings_InvalidDefaultIteration, TeamSettingsFields.DefaultIteration);
        }
        catch (WorkItemTrackingTreeNodeNotFoundException ex)
        {
          throw new InvalidTeamSettingsException(Resources.Settings_InvalidDefaultIteration, TeamSettingsFields.DefaultIteration);
        }
      }
      else if (!WiqlOperators.IsCurrentIterationMacro(defaultIteration, false))
        throw new InvalidTeamSettingsException(Resources.Settings_InvalidDefaultIteration, TeamSettingsFields.DefaultIteration);
    }

    internal static void EnsureValidBacklogVisibilities(
      IVssRequestContext requestContext,
      IDictionary<string, bool> visibilities,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      bool fixIssues,
      bool addIfMissing,
      out IList<string> defaultedVisibilities)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<string, bool>>(visibilities, nameof (visibilities));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration>(backlogConfiguration, nameof (backlogConfiguration));
      defaultedVisibilities = (IList<string>) new List<string>();
      if (visibilities.Count == 0 && !fixIssues)
        return;
      IEnumerable<string> strings = (backlogConfiguration.PortfolioBacklogs != null ? backlogConfiguration.PortfolioBacklogs : (IReadOnlyCollection<BacklogLevelConfiguration>) new BacklogLevelConfiguration[0]).Concat<BacklogLevelConfiguration>((IEnumerable<BacklogLevelConfiguration>) new BacklogLevelConfiguration[1]
      {
        backlogConfiguration.RequirementBacklog
      }).Where<BacklogLevelConfiguration>((Func<BacklogLevelConfiguration, bool>) (x => x != null)).Select<BacklogLevelConfiguration, string>((Func<BacklogLevelConfiguration, string>) (x => x.Id)).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x)));
      IEnumerable<string> list1 = (IEnumerable<string>) visibilities.Keys.Except<string>(strings, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName).ToList<string>();
      if (list1.Any<string>())
      {
        if (!fixIssues)
          throw new InvalidTeamSettingsException(Resources.Settings_UnrecognizedBacklogVisibilities, TeamSettingsFields.BacklogVisibilities);
        foreach (string key in list1)
          visibilities.Remove(key);
      }
      IEnumerable<string> list2 = (IEnumerable<string>) strings.Except<string>((IEnumerable<string>) visibilities.Keys, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName).ToList<string>();
      if (list2.Any<string>())
      {
        if (!(fixIssues | addIfMissing))
          throw new InvalidTeamSettingsException(Resources.Settings_IncompleteBacklogVisibilities, TeamSettingsFields.BacklogVisibilities);
        IEnumerable<string> source = (IEnumerable<string>) ((object) backlogConfiguration.HiddenBacklogs ?? (object) new string[0]);
        foreach (string str in list2)
        {
          int num = source.Contains<string>(str, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName) ? 1 : 0;
          bool flag = num == 0;
          if (num == 0 && backlogConfiguration.GetBacklogLevelConfiguration(str).Custom)
            flag = false;
          visibilities.Add(str, flag);
          defaultedVisibilities.Add(str);
        }
      }
      if (visibilities.Any<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (x => x.Value)))
        return;
      if (!fixIssues)
        throw new InvalidTeamSettingsException(Resources.Settings_AllBacklogsHidden, TeamSettingsFields.BacklogVisibilities);
      if (backlogConfiguration.RequirementBacklog == null || string.IsNullOrWhiteSpace(backlogConfiguration.RequirementBacklog.Id))
        return;
      visibilities[backlogConfiguration.RequirementBacklog.Id] = true;
    }
  }
}
