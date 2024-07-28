// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.RequestedTeamIterationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Service;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  public class RequestedTeamIterationService : IRequestedTeamIterationService
  {
    private const string c_AgileRequestedIterationNode = "AgileRequestedTeamIterationNode";

    public Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetRequestedIterationNode(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      bool throwIfNotFound = true,
      bool throwIfTeamNotSubscribed = false)
    {
      return this.GetRequestedIterationNode(requestContext, settings, this.GetRequestedIterationRouteValue(requestContext), throwIfNotFound, throwIfTeamNotSubscribed);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetRequestedIterationNode(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      string pathOrId,
      bool throwIfNotFound = true,
      bool throwIfTeamNotSubscribed = false)
    {
      object obj;
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode;
      if (requestContext.Items.TryGetValue(this.GetCacheKey(pathOrId), out obj))
      {
        iterationNode = obj == null ? (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null : (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) obj;
      }
      else
      {
        Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
        iterationNode = this.GetRequestedIterationNodeInternal(requestContext, project, settings, pathOrId, throwIfTeamNotSubscribed);
        requestContext.Items[this.GetCacheKey(pathOrId)] = (object) iterationNode;
      }
      if (iterationNode != null)
      {
        if (this.IsTeamSubscribedToIteration(settings, iterationNode))
          return iterationNode;
        if (throwIfTeamNotSubscribed)
          throw new AgileInvalidIterationPathException(pathOrId);
        iterationNode = (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null;
      }
      return !(iterationNode == null & throwIfNotFound) ? iterationNode : throw new AgileInvalidIterationPathException(pathOrId);
    }

    public string GetRequestedIterationRouteValue(IVssRequestContext requestContext) => requestContext.GetService<IContributionRoutingService>().GetRouteValue<string>(requestContext, "iteration");

    private Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetRequestedIterationNodeInternal(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      IAgileSettings settings,
      string pathOrId,
      bool throwIfTeamNotSubscribed = false)
    {
      if (string.IsNullOrWhiteSpace(pathOrId))
        return (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null;
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null;
      WorkItemTrackingTreeService service = requestContext.GetService<WorkItemTrackingTreeService>();
      Guid result;
      if (Guid.TryParse(pathOrId, out result))
      {
        treeNode = service.GetTreeNode(requestContext, project.Id, result, false);
      }
      else
      {
        string internalNodePath = this.ConvertUrlNodePathToInternalNodePath(pathOrId);
        if (!service.GetSnapshot(requestContext).TryGetNodeFromPath(requestContext, internalNodePath, TreeStructureType.Iteration, out treeNode))
        {
          string absolutePath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) settings.TeamSettings.GetBacklogIterationNode(requestContext).GetPath(requestContext), (object) "\\", (object) internalNodePath);
          service.GetSnapshot(requestContext).TryGetNodeFromPath(requestContext, absolutePath, TreeStructureType.Iteration, out treeNode);
        }
      }
      return treeNode;
    }

    private string ConvertUrlNodePathToInternalNodePath(string urlNodePath) => urlNodePath.Replace("/", "\\");

    private bool IsTeamSubscribedToIteration(IAgileSettings settings, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode)
    {
      if (iterationNode == null)
        return false;
      Guid iterationId = iterationNode.CssNodeId;
      return settings.TeamSettings.Iterations.Any<ITeamIteration>((Func<ITeamIteration, bool>) (i => i.IterationId == iterationId));
    }

    private string GetCacheKey(string pathOrId) => "AgileRequestedTeamIterationNode_" + pathOrId;
  }
}
