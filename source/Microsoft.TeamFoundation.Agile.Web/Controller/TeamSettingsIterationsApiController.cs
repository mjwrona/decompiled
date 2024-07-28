// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.TeamSettingsIterationsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Web;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "iterations")]
  public class TeamSettingsIterationsApiController : TeamSettingsApiControllerBase
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.CurrentIterationDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.CurrentIterationDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddTranslation<Microsoft.TeamFoundation.Agile.Server.Exceptions.CurrentIterationDoesNotExistException, Microsoft.Azure.Boards.Agile.Common.Exceptions.CurrentIterationDoesNotExistException>();
    }

    [HttpGet]
    [ClientExample("GET__work_teamsettings_iterations__timeframe-_timeframe_.json", "Get a team's iterations using timeframe filter", null, null)]
    public IList<TeamSettingsIteration> GetTeamIterations([FromUri(Name = "$timeframe")] string timeframe = null)
    {
      this.TfsRequestContext.TraceEnter(290141, "AgileService", "AgileService", nameof (GetTeamIterations));
      try
      {
        SortedIterationSubscriptions sortedTeamIteration = this.TfsRequestContext.GetService<ITeamIterationsService>().GetSortedTeamIterations(this.TfsRequestContext, this.ProjectId, (IEnumerable<WebApiTeam>) new WebApiTeam[1]
        {
          this.Team
        })[this.TeamId];
        List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> treeNodeList = sortedTeamIteration.Iterations.ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
        int currentIterationIndex = sortedTeamIteration.CurrentIterationIndex;
        if (!string.IsNullOrEmpty(timeframe))
        {
          if (!string.Equals(timeframe, "current", StringComparison.OrdinalIgnoreCase))
            throw new InvalidTeamSettingsIterationException(nameof (timeframe));
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = currentIterationIndex >= 0 ? treeNodeList[currentIterationIndex] : (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode) null;
          if (treeNode == null)
            throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.CurrentIterationDoesNotExistException();
          treeNodeList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
          treeNodeList.Add(treeNode);
        }
        List<TeamSettingsIteration> teamIterations = new List<TeamSettingsIteration>();
        for (int index = 0; index < treeNodeList.Count; ++index)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = treeNodeList[index];
          List<TeamSettingsIteration> settingsIterationList = teamIterations;
          TeamSettingsIteration settingsIteration = new TeamSettingsIteration();
          settingsIteration.Url = this.GetIterationLinkUrl(treeNode.CssNodeId);
          settingsIteration.Id = treeNode.CssNodeId;
          settingsIteration.Name = treeNode.GetSanitizedName(this.TfsRequestContext);
          settingsIteration.Path = treeNode.GetPath(this.TfsRequestContext);
          settingsIteration.Attributes = new TeamIterationAttributes()
          {
            StartDate = treeNode.StartDate,
            FinishDate = treeNode.FinishDate,
            TimeFrame = new TimeFrame?(string.IsNullOrEmpty(timeframe) ? this.GetTimeFrame(index, currentIterationIndex) : TimeFrame.Current)
          };
          settingsIterationList.Add(settingsIteration);
        }
        return (IList<TeamSettingsIteration>) teamIterations;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290142, "AgileService", "AgileService", nameof (GetTeamIterations));
      }
    }

    [HttpGet]
    [ClientExample("GET__work_teamsettings_iterations__iterationId_.json", "Get a team's iteration by iterationId", null, null)]
    public TeamSettingsIteration GetTeamIteration(Guid id)
    {
      this.TfsRequestContext.TraceEnter(290143, "AgileService", "AgileService", nameof (GetTeamIteration));
      try
      {
        this.TfsRequestContext.GetService<ITeamConfigurationService>();
        ITeamSettings settingsInternal = this.GetTeamSettingsInternal();
        if (settingsInternal.Iterations.FirstOrDefault<ITeamIteration>((Func<ITeamIteration, bool>) (iteration => iteration.IterationId == id)) == null && id != settingsInternal.BacklogIterationId)
          throw new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IterationNotFoundException(id.ToString());
        return this.GetTeamIterationInternal(id);
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290144, "AgileService", "AgileService", nameof (GetTeamIteration));
      }
    }

    private TeamSettingsIteration GetTeamIterationInternal(Guid Id)
    {
      SortedIterationSubscriptions sortedTeamIteration = this.TfsRequestContext.GetService<ITeamIterationsService>().GetSortedTeamIterations(this.TfsRequestContext, this.ProjectId, (IEnumerable<WebApiTeam>) new WebApiTeam[1]
      {
        this.Team
      })[this.TeamId];
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode> list = sortedTeamIteration.Iterations.ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>();
      int currentIterationIndex = sortedTeamIteration.CurrentIterationIndex;
      TeamSettingsIteration iterationInternal = new TeamSettingsIteration();
      for (int index = 0; index < list.Count; ++index)
      {
        if (list[index].CssNodeId == Id)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = list[index];
          iterationInternal.Id = treeNode.CssNodeId;
          iterationInternal.Name = treeNode.GetSanitizedName(this.TfsRequestContext);
          iterationInternal.Path = treeNode.GetPath(this.TfsRequestContext);
          iterationInternal.Attributes = new TeamIterationAttributes()
          {
            StartDate = treeNode.StartDate,
            FinishDate = treeNode.FinishDate,
            TimeFrame = new TimeFrame?(this.GetTimeFrame(index, currentIterationIndex))
          };
          string iterationLinkUrl = this.GetIterationLinkUrl(Id);
          iterationInternal.Url = iterationLinkUrl;
          iterationInternal.Links = this.GetReferenceLinks(iterationLinkUrl, TeamSettingsApiControllerBase.CommonUrlLink.TeamSettings | TeamSettingsApiControllerBase.CommonUrlLink.Iterations | TeamSettingsApiControllerBase.CommonUrlLink.Capacities | TeamSettingsApiControllerBase.CommonUrlLink.IterationWorkItems, Id);
          this.AddClassificationNodeLink(iterationInternal.Links, treeNode);
          string resourceUriString = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, TeamSettingsApiConstants.TeamDaysOffLocationId, this.ProjectId, this.TeamId, (object) new
          {
            iterationId = Id
          });
          iterationInternal.Links.AddLink("teamDaysOff", resourceUriString);
          return iterationInternal;
        }
      }
      this.TfsRequestContext.TraceAlways(290161, TraceLevel.Error, "AgileService", "AgileService", string.Format("ProjectId {0}, TeamId {1}, Sorted iteration ids {2}, current iteration index {3}", (object) this.ProjectId, (object) this.TeamId, (object) string.Join(",", list.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, string>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode, string>) (x => x.CssNodeId.ToString()))), (object) currentIterationIndex));
      throw new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.IterationNotFoundException(Id.ToString());
    }

    private TimeFrame GetTimeFrame(int index, int currentIterationIndex)
    {
      if (index < currentIterationIndex)
        return TimeFrame.Past;
      return index == currentIterationIndex ? TimeFrame.Current : TimeFrame.Future;
    }

    [HttpPost]
    [ClientSuppressWarning(ClientWarnings.NamingGuidelines)]
    [ClientResponseType(typeof (TeamSettingsIteration), null, null)]
    [ClientExample("POST__work_teamsettings_iterations.json", "Add an iteration to the team", null, null)]
    public HttpResponseMessage PostTeamIteration([FromBody] TeamSettingsIteration iteration)
    {
      this.TfsRequestContext.TraceEnter(290145, "AgileService", "AgileService", nameof (PostTeamIteration));
      try
      {
        if (iteration == null)
          throw new VssPropertyValidationException(nameof (iteration), ResourceStrings.NullOrEmptyParameter((object) nameof (iteration)));
        if (iteration.Id == Guid.Empty && string.IsNullOrEmpty(iteration.Path))
          throw new InvalidTeamSettingsIterationException("Id");
        ITeamConfigurationService service = this.TfsRequestContext.GetService<ITeamConfigurationService>();
        ITeamSettings settingsInternal = this.GetTeamSettingsInternal(true);
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode;
        if (iteration.Id != Guid.Empty)
        {
          treeNode = this.GetTreeNode(iteration.Id);
          if (treeNode == null || treeNode.Type != TreeStructureType.Iteration)
            throw new InvalidTeamSettingsIterationException("Id");
        }
        else if (!this.TryGetTreeNode(iteration.Path, out treeNode))
          throw new InvalidTeamSettingsIterationException("Path");
        iteration.Id = treeNode.CssNodeId;
        if (settingsInternal.Iterations.FirstOrDefault<ITeamIteration>((Func<ITeamIteration, bool>) (i => i.IterationId == iteration.Id)) == null && iteration.Id != settingsInternal.BacklogIterationId)
        {
          List<Guid> iterationIds = new List<Guid>();
          iterationIds.AddRange(settingsInternal.Iterations.Select<ITeamIteration, Guid>((Func<ITeamIteration, Guid>) (i => i.IterationId)));
          iterationIds.Add(iteration.Id);
          service.SaveBacklogIterations(this.TfsRequestContext, this.Team, (IEnumerable<Guid>) iterationIds, settingsInternal.BacklogIterationId);
        }
        return this.Request.CreateResponse<TeamSettingsIteration>(HttpStatusCode.Created, this.GetTeamIterationInternal(iteration.Id));
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290146, "AgileService", "AgileService", nameof (PostTeamIteration));
      }
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE__work_teamsettings_iterations__iterationId_.json", "Delete a team's iteration by iterationId", null, null)]
    public HttpResponseMessage DeleteTeamIteration(Guid id)
    {
      this.TfsRequestContext.TraceEnter(290147, "AgileService", "AgileService", nameof (DeleteTeamIteration));
      try
      {
        this.TfsRequestContext.GetService<ITeamConfigurationService>().DeleteBacklogIterations(this.TfsRequestContext, this.Team, (IEnumerable<Guid>) new Guid[1]
        {
          id
        });
        return this.Request.CreateResponse(HttpStatusCode.NoContent);
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290148, "AgileService", "AgileService", nameof (DeleteTeamIteration));
      }
    }
  }
}
