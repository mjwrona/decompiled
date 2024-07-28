// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.AreaIdToTeamMappingsHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal static class AreaIdToTeamMappingsHelper
  {
    public static void UpdateAreaIdToTeamMappingsResult(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode node,
      IDictionary<string, IDictionary<Guid, bool>> selectedTeamFieldsForProject,
      IDictionary<Guid, string> validTeams,
      IDictionary<Guid, IDictionary<Guid, TeamFieldProperty>> result)
    {
      Guid cssNodeId = node.CssNodeId;
      IDictionary<Guid, bool> teamIdToPropertyMap;
      selectedTeamFieldsForProject.TryGetValue(cssNodeId.ToString(), out teamIdToPropertyMap);
      if (teamIdToPropertyMap != null)
        AreaIdToTeamMappingsHelper.UpdateAreaIdToTeamMappingsResultInternal(requestContext, cssNodeId, teamIdToPropertyMap, validTeams, result);
      AreaIdToTeamMappingsHelper.UpdateAreaIdToTeamMappingsResultInternalWithParent(requestContext, projectId, node, result);
      IDictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> children = node.Children;
      if (children.Count <= 0)
        return;
      foreach (KeyValuePair<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> keyValuePair in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>>) children)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode node1 = keyValuePair.Value;
        AreaIdToTeamMappingsHelper.UpdateAreaIdToTeamMappingsResult(requestContext, projectId, node1, selectedTeamFieldsForProject, validTeams, result);
      }
    }

    public static void UpdateAreaIdToTeamMappingsResultInternal(
      IVssRequestContext requestContext,
      Guid nodeId,
      IDictionary<Guid, bool> teamIdToPropertyMap,
      IDictionary<Guid, string> validTeams,
      IDictionary<Guid, IDictionary<Guid, TeamFieldProperty>> result)
    {
      Func<IDictionary<Guid, TeamFieldProperty>> createValueToAdd = (Func<IDictionary<Guid, TeamFieldProperty>>) (() => (IDictionary<Guid, TeamFieldProperty>) new Dictionary<Guid, TeamFieldProperty>());
      IDictionary<Guid, TeamFieldProperty> orAddValue = result.GetOrAddValue<Guid, IDictionary<Guid, TeamFieldProperty>>(nodeId, createValueToAdd);
      foreach (KeyValuePair<Guid, bool> teamIdToProperty in (IEnumerable<KeyValuePair<Guid, bool>>) teamIdToPropertyMap)
      {
        Guid key = teamIdToProperty.Key;
        bool flag = teamIdToProperty.Value;
        string teamName;
        if (validTeams.TryGetValue(key, out teamName))
          orAddValue.TryAdd<Guid, TeamFieldProperty>(key, new TeamFieldProperty(teamName, flag, flag));
      }
    }

    public static void UpdateAreaIdToTeamMappingsResultInternalWithParent(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode node,
      IDictionary<Guid, IDictionary<Guid, TeamFieldProperty>> result)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = requestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(requestContext, projectId, node.ParentId, false);
      IDictionary<Guid, TeamFieldProperty> dictionary;
      result.TryGetValue(treeNode.CssNodeId, out dictionary);
      if (dictionary == null)
        return;
      Func<IDictionary<Guid, TeamFieldProperty>> createValueToAdd = (Func<IDictionary<Guid, TeamFieldProperty>>) (() => (IDictionary<Guid, TeamFieldProperty>) new Dictionary<Guid, TeamFieldProperty>());
      IDictionary<Guid, TeamFieldProperty> orAddValue = result.GetOrAddValue<Guid, IDictionary<Guid, TeamFieldProperty>>(node.CssNodeId, createValueToAdd);
      foreach (KeyValuePair<Guid, TeamFieldProperty> keyValuePair in (IEnumerable<KeyValuePair<Guid, TeamFieldProperty>>) dictionary)
      {
        Guid key = keyValuePair.Key;
        string teamName = keyValuePair.Value.TeamName;
        bool includeChildren = false;
        bool parentIncludeChildren = keyValuePair.Value.ParentIncludeChildren;
        if (parentIncludeChildren)
          orAddValue.TryAdd<Guid, TeamFieldProperty>(key, new TeamFieldProperty(teamName, includeChildren, parentIncludeChildren));
      }
    }
  }
}
