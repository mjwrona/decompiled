// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models.TeamWITSettingsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Models
{
  [DataContract]
  [KnownType(typeof (TeamWeekends))]
  public class TeamWITSettingsModel
  {
    public TeamWITSettingsModel()
    {
    }

    internal TeamWITSettingsModel(
      IVssRequestContext requestContext,
      WebApiTeam team,
      ProjectProcessConfiguration projectConfig,
      ITeamSettings teamSettings,
      TimeZoneInfo timeZone,
      SortedIterationSubscriptions sortedIterationSubscriptions = null)
    {
      this.TeamId = team.Id;
      this.TeamName = team.Name;
      this.TeamFieldName = projectConfig.TeamField.Name;
      int defaultValueIndex = teamSettings.TeamFieldConfig.DefaultValueIndex;
      ITeamFieldValue teamFieldValue = ((IEnumerable<ITeamFieldValue>) teamSettings.TeamFieldConfig.TeamFieldValues).ElementAtOrDefault<ITeamFieldValue>(defaultValueIndex);
      this.TeamFieldValues = ((IEnumerable<ITeamFieldValue>) teamSettings.TeamFieldConfig.TeamFieldValues).Select<ITeamFieldValue, TeamFieldValue>((Func<ITeamFieldValue, TeamFieldValue>) (v =>
      {
        if (v == null)
          return (TeamFieldValue) null;
        return new TeamFieldValue()
        {
          IncludeChildren = v.IncludeChildren,
          Value = v.Value
        };
      })).ToArray<TeamFieldValue>();
      if (teamFieldValue != null)
        this.TeamFieldDefaultValue = teamFieldValue.Value;
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode node1 = (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null;
      try
      {
        node1 = teamSettings.GetBacklogIterationNode(requestContext);
      }
      catch (NodeDoesNotExistException ex)
      {
        requestContext.TraceException(290269, TraceLevel.Info, "WebAccess", TfsTraceLayers.Content, (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, "WebAccess", TfsTraceLayers.Content, ex);
      }
      if (node1 != null)
      {
        this.ProjectId = node1.ProjectId;
        this.BacklogIteration = new AgileTreeNodeModel(requestContext, node1);
      }
      if (teamSettings.DefaultIterationId != Guid.Empty)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode node2 = (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null;
        try
        {
          node2 = teamSettings.GetDefaultIterationNode(requestContext);
        }
        catch (NodeDoesNotExistException ex)
        {
          requestContext.TraceCatch(290270, TraceLevel.Info, "WebAccess", TfsTraceLayers.Content, (Exception) ex);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(599999, "WebAccess", TfsTraceLayers.Content, ex);
        }
        if (node2 != null)
          this.DefaultIteration = new AgileTreeNodeModel(requestContext, node2);
      }
      SortedIterationSubscriptions iterationSubscriptions = sortedIterationSubscriptions ?? teamSettings.GetIterationTimeline(requestContext, team.ProjectId);
      this.DefaultIterationMacro = teamSettings.DefaultIterationMacro;
      this.CurrentIteration = iterationSubscriptions.CurrentIteration != null ? new AgileTreeNodeModel(requestContext, iterationSubscriptions.CurrentIteration) : (AgileTreeNodeModel) null;
      this.PreviousIterations = iterationSubscriptions.PreviousIterations.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, AgileTreeNodeModel>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, AgileTreeNodeModel>) (node => new AgileTreeNodeModel(requestContext, node))).ToArray<AgileTreeNodeModel>();
      this.FutureIterations = iterationSubscriptions.FutureIterations.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, AgileTreeNodeModel>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, AgileTreeNodeModel>) (node => new AgileTreeNodeModel(requestContext, node))).ToArray<AgileTreeNodeModel>();
      this.ShowBugCategoryWorkItem = new bool?(teamSettings.BugsBehavior == BugsBehavior.AsRequirements);
      this.BugsBehavior = teamSettings.BugsBehavior;
      this.BacklogVisibilities = teamSettings.BacklogVisibilities;
      this.Weekends = teamSettings.Weekends;
      this.ComputedReferencedNodes(requestContext);
    }

    internal void ComputedReferencedNodes(IVssRequestContext requestContext)
    {
      if (!(this.ProjectId != Guid.Empty))
        return;
      WorkItemTrackingTreeService treeService = requestContext.GetService<WorkItemTrackingTreeService>();
      List<ExtendedTreeNode> areaNodes = new List<ExtendedTreeNode>();
      if (this.TeamFieldDefaultValue != null)
      {
        int startIndex = this.TeamFieldDefaultValue.IndexOf('\\');
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode;
        if (startIndex > -1)
        {
          string relativePath = this.TeamFieldDefaultValue.Substring(startIndex);
          treeNode = treeService.GetTreeNode(requestContext, this.ProjectId, TreeStructureType.Area, relativePath, false);
        }
        else
          treeNode = treeService.GetTreeNode(requestContext, this.ProjectId, TreeStructureType.None, "");
        if (treeNode != null)
          areaNodes.Add(ExtendedTreeNode.Create(requestContext, treeNode.GetPath(requestContext), treeNode, (ISecuredObject) null));
      }
      Dictionary<Guid, ExtendedTreeNode> iterationNodes = new Dictionary<Guid, ExtendedTreeNode>();
      Action<AgileTreeNodeModel> action = (Action<AgileTreeNodeModel>) (cssNode =>
      {
        if (cssNode == null)
          return;
        if (cssNode.FriendlyPath.IndexOf('\\') < 0)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = treeService.GetTreeNode(requestContext, this.ProjectId, TreeStructureType.None, "");
          if (iterationNodes.ContainsKey(treeNode.CssNodeId))
            return;
          iterationNodes.Add(treeNode.CssNodeId, ExtendedTreeNode.Create(requestContext, treeNode.GetPath(requestContext), treeNode, (ISecuredObject) null));
        }
        else
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = treeService.GetTreeNode(requestContext, this.ProjectId, cssNode.Id);
          if (treeNode == null || iterationNodes.ContainsKey(treeNode.CssNodeId))
            return;
          iterationNodes.Add(treeNode.CssNodeId, ExtendedTreeNode.Create(requestContext, treeNode.GetPath(requestContext), treeNode, (ISecuredObject) null));
        }
      });
      action(this.DefaultIteration);
      action(this.CurrentIteration);
      foreach (AgileTreeNodeModel previousIteration in this.PreviousIterations)
        action(previousIteration);
      foreach (AgileTreeNodeModel futureIteration in this.FutureIterations)
        action(futureIteration);
      action(this.BacklogIteration);
      this.ReferencedNodes = new ReferencedNodes((IEnumerable<ExtendedTreeNode>) areaNodes, (IEnumerable<ExtendedTreeNode>) iterationNodes.Select<KeyValuePair<Guid, ExtendedTreeNode>, ExtendedTreeNode>((Func<KeyValuePair<Guid, ExtendedTreeNode>, ExtendedTreeNode>) (i => i.Value)).ToList<ExtendedTreeNode>(), (ISecuredObject) null);
    }

    [DataMember(Name = "teamId")]
    public Guid TeamId { get; set; }

    [DataMember(Name = "teamName")]
    public string TeamName { get; set; }

    [DataMember(Name = "backlogIteration", EmitDefaultValue = false)]
    public AgileTreeNodeModel BacklogIteration { get; set; }

    [DataMember(Name = "defaultIteration", EmitDefaultValue = false)]
    public AgileTreeNodeModel DefaultIteration { get; set; }

    [DataMember(Name = "defaultIterationMacro", EmitDefaultValue = false)]
    public string DefaultIterationMacro { get; set; }

    [DataMember(Name = "previousIterations", EmitDefaultValue = false)]
    public AgileTreeNodeModel[] PreviousIterations { get; set; }

    [DataMember(Name = "currentIteration", EmitDefaultValue = false)]
    public AgileTreeNodeModel CurrentIteration { get; set; }

    [DataMember(Name = "futureIterations", EmitDefaultValue = false)]
    public AgileTreeNodeModel[] FutureIterations { get; set; }

    [DataMember(Name = "teamFieldName", EmitDefaultValue = false)]
    public string TeamFieldName { get; set; }

    [DataMember(Name = "teamFieldValues", EmitDefaultValue = false)]
    public TeamFieldValue[] TeamFieldValues { get; set; }

    [DataMember(Name = "teamFieldDefaultValue", EmitDefaultValue = false)]
    public string TeamFieldDefaultValue { get; set; }

    [DataMember(Name = "showBugCategoryWorkItem", EmitDefaultValue = false)]
    public bool? ShowBugCategoryWorkItem { get; set; }

    [DataMember(Name = "bugsBehavior", EmitDefaultValue = false)]
    public BugsBehavior BugsBehavior { get; set; }

    [DataMember(Name = "backlogVisibilities", EmitDefaultValue = false)]
    public IDictionary<string, bool> BacklogVisibilities { get; set; }

    [DataMember(Name = "projectId", EmitDefaultValue = false)]
    public Guid ProjectId { get; set; }

    [DataMember(Name = "weekends", EmitDefaultValue = false)]
    public ITeamWeekends Weekends { get; set; }

    [DataMember(Name = "referencedNodes", EmitDefaultValue = false)]
    public ReferencedNodes ReferencedNodes { get; set; }

    internal static TeamWITSettingsModel CreateTeamWITSettingsModel(
      IVssRequestContext requestContext,
      ProjectInfo project,
      WebApiTeam team,
      TimeZoneInfo timeZone)
    {
      ProjectProcessConfiguration projectProcessSettings = requestContext.GetProjectProcessSettings(project);
      ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, true, false);
      return new TeamWITSettingsModel(requestContext, team, projectProcessSettings, teamSettings, timeZone);
    }
  }
}
