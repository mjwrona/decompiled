// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.TeamSettingsApiControllerBase
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Location;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  public abstract class TeamSettingsApiControllerBase : TfsTeamApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CapacityNotSetException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<KeyNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlTypeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.InvalidTeamSettingsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.InvalidTeamSettingsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamDaysOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.TeamDaysOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<TeamFieldArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NoTeamFieldValuesSelectedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DuplicateTeamFieldValuesException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NoDefaultTeamFieldValueException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTeamSettingsIterationException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardPageWorkItemsLimitException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidTeamSettingsForUpdateException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ChartOptionInvalidJsonException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SetBoardCardRulesInvalidArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ReorderWorkItemsInvalidOperationsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UnauthorizedAccessException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Core.WebApi.TeamSecurityException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IterationNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.IterationNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<IterationPathNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<WorkItemTypeCategoryNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.InvalidTeamSettingsException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.InvalidTeamSettingsException));
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IterationNotFoundException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.IterationNotFoundException));
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamDaysOutOfRangeException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.TeamDaysOutOfRangeException));
      exceptionMap.AddTranslation(typeof (Microsoft.Azure.Devops.Teams.Service.TeamSecurityException), typeof (Microsoft.TeamFoundation.Core.WebApi.TeamSecurityException));
    }

    protected virtual ITeamSettings GetTeamSettingsInternal(bool bypassCache = false) => this.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamSettings(this.TfsRequestContext, this.Team, false, bypassCache);

    protected virtual Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode GetTreeNode(
      Guid iterationId,
      bool throwIfNotFound = false)
    {
      return this.TfsRequestContext.GetService<WorkItemTrackingTreeService>().GetTreeNode(this.TfsRequestContext, this.ProjectId, iterationId, throwIfNotFound);
    }

    protected virtual bool TryGetTreeNode(string iterationPath, out Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode) => this.TfsRequestContext.GetService<WorkItemTrackingTreeService>().GetSnapshot(this.TfsRequestContext).TryGetNodeFromPath(this.TfsRequestContext, iterationPath, TreeStructureType.Iteration, out treeNode);

    protected virtual string GetClassificationNodeUrl(Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode) => WitUrlHelper.GetClassificationNodeUrl(this.TfsRequestContext, treeNode);

    protected virtual ProjectProcessConfiguration GetProcessSettings() => this.TfsRequestContext.GetProjectProcessConfiguration(CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo).Uri, false);

    protected virtual ReferenceLinks GetReferenceLinks(
      string selfUrl,
      TeamSettingsApiControllerBase.CommonUrlLink linksToInclude,
      Guid iterationId = default (Guid))
    {
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", selfUrl);
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      referenceLinks.AddLink("project", service.GetCoreResourceUriString(this.TfsRequestContext, CoreConstants.ProjectsLocationId, (object) new
      {
        projectId = this.ProjectId
      }));
      referenceLinks.AddLink("team", service.GetCoreResourceUriString(this.TfsRequestContext, CoreConstants.TeamsLocationId, (object) new
      {
        projectId = this.ProjectId,
        teamId = this.TeamId
      }));
      if ((linksToInclude & TeamSettingsApiControllerBase.CommonUrlLink.TeamSettings) == TeamSettingsApiControllerBase.CommonUrlLink.TeamSettings)
      {
        string resourceUriString = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, TeamSettingsApiConstants.LocationId, this.ProjectId, this.TeamId, (object) null, true);
        referenceLinks.AddLink("teamSettings", resourceUriString);
      }
      if ((linksToInclude & TeamSettingsApiControllerBase.CommonUrlLink.Iterations) == TeamSettingsApiControllerBase.CommonUrlLink.Iterations)
      {
        string resourceUriString = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, TeamSettingsApiConstants.IterationsLocationId, this.ProjectId, this.TeamId, (object) new Dictionary<string, object>()
        {
          {
            "id",
            (object) null
          }
        }, true);
        referenceLinks.AddLink("teamIterations", resourceUriString);
      }
      if (iterationId != Guid.Empty && (linksToInclude & TeamSettingsApiControllerBase.CommonUrlLink.Iteration) == TeamSettingsApiControllerBase.CommonUrlLink.Iteration)
      {
        string iterationLinkUrl = this.GetIterationLinkUrl(iterationId);
        referenceLinks.AddLink("teamIteration", iterationLinkUrl);
      }
      if ((linksToInclude & TeamSettingsApiControllerBase.CommonUrlLink.TeamFieldValues) == TeamSettingsApiControllerBase.CommonUrlLink.TeamFieldValues)
      {
        string resourceUriString = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, TeamSettingsApiConstants.TeamFieldValuesLocationId, this.ProjectId, this.TeamId, (object) string.Empty, true);
        referenceLinks.AddLink("teamFieldValues", resourceUriString);
      }
      if (iterationId != Guid.Empty && (linksToInclude & TeamSettingsApiControllerBase.CommonUrlLink.Capacities) == TeamSettingsApiControllerBase.CommonUrlLink.Capacities)
      {
        string resourceUriString = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, TeamSettingsApiConstants.CapacityLocationId, this.ProjectId, this.TeamId, (object) new Dictionary<string, object>()
        {
          {
            "teammemberid",
            (object) null
          },
          {
            nameof (iterationId),
            (object) iterationId
          }
        }, true);
        referenceLinks.AddLink("capacity", resourceUriString);
      }
      if (iterationId != Guid.Empty && (linksToInclude & TeamSettingsApiControllerBase.CommonUrlLink.IterationWorkItems) == TeamSettingsApiControllerBase.CommonUrlLink.IterationWorkItems && ResourceAreaHelper.IsResourceRegistered(this.TfsRequestContext, WorkWebConstants.RestAreaGuid, TeamSettingsApiConstants.IterationWorkItemsLocationId))
      {
        string workItemsLinkUrl = this.GetIterationWorkItemsLinkUrl(iterationId);
        referenceLinks.AddLink("workitems", workItemsLinkUrl);
      }
      if ((linksToInclude & TeamSettingsApiControllerBase.CommonUrlLink.RootClassificationNodeAreas) == TeamSettingsApiControllerBase.CommonUrlLink.RootClassificationNodeAreas)
      {
        string classificationNodeUrl = this.GetClassificationNodeUrl((object) new
        {
          structureGroup = TreeStructureGroup.Areas
        });
        referenceLinks.AddLink("areaPathClassificationNodes", classificationNodeUrl);
      }
      return referenceLinks;
    }

    protected virtual void AddClassificationNodeLink(ReferenceLinks links, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode) => links.AddLink("classificationNode", this.GetClassificationNodeUrl(treeNode));

    protected virtual void AddClassificationNodeLink(ReferenceLinks links, Guid iterationId)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = this.GetTreeNode(iterationId, true);
      links.AddLink("classificationNode", this.GetClassificationNodeUrl(treeNode));
    }

    protected virtual string GetClassificationNodeUrl(object routeValues) => WitUrlHelper.GetClassificationNodeUrl(this.TfsRequestContext, this.TfsRequestContext.GetService<WorkItemTrackingTreeService>().GetSnapshot(this.TfsRequestContext).GetRootTreeNodes(this.ProjectId).FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, bool>) (node => node.Type == TreeStructureType.Area)));

    protected virtual string GetAgileResourceUriString(Guid locationId, object routeValues = null) => AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, locationId, this.ProjectId, this.TeamId, routeValues, true);

    protected virtual string GetIterationLinkUrl(Guid iterationId) => this.GetAgileResourceUriString(TeamSettingsApiConstants.IterationsLocationId, (object) new Dictionary<string, object>()
    {
      {
        "id",
        (object) iterationId
      }
    });

    protected virtual string GetIterationWorkItemsLinkUrl(Guid iterationId) => AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, TeamSettingsApiConstants.IterationWorkItemsLocationId, this.ProjectId, this.TeamId, (object) new Dictionary<string, object>()
    {
      {
        nameof (iterationId),
        (object) iterationId
      }
    }, true);

    public enum CommonUrlLink
    {
      TeamSettings = 1,
      TeamFieldValues = 2,
      Iterations = 4,
      Iteration = 8,
      Capacities = 16, // 0x00000010
      RootClassificationNodeAreas = 32, // 0x00000020
      IterationWorkItems = 64, // 0x00000040
    }
  }
}
