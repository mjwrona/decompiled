// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.RemoteLinking.RemoteLinkingS2SOnlyController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.RemoteLinking
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "remoteLinking", ResourceVersion = 1)]
  public class RemoteLinkingS2SOnlyController : WorkItemTrackingApiController
  {
    private readonly List<int> fieldsToRead = new List<int>()
    {
      -2,
      25,
      1
    };

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<RemoteLinkingUnauthorizedAccessException>(HttpStatusCode.NotFound);
    }

    public override string TraceArea => "remoteLinking";

    [HttpPatch]
    [ClientLocationId("3F0377F8-D4BF-445B-B1E7-F9E5F1BA8FDB")]
    public IEnumerable<RemoteWorkItemLinkUpdateResult> UpdateRemoteLinks(
      IEnumerable<RemoteWorkItemLinkUpdate> links)
    {
      this.ValidateIsS2SCall();
      if (!CommonWITUtils.IsRemoteLinkingEnabled(this.TfsRequestContext))
        throw new FeatureDisabledException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RemoteLinkFeatureDisabled());
      ArgumentUtility.CheckForNull<IEnumerable<RemoteWorkItemLinkUpdate>>(links, nameof (links));
      this.ValidateLinksInput(links);
      IdentityDescriptor authorizedByDescriptor = links.Select<RemoteWorkItemLinkUpdate, IdentityDescriptor>((Func<RemoteWorkItemLinkUpdate, IdentityDescriptor>) (x => x.AuthorizedByDescriptor)).FirstOrDefault<IdentityDescriptor>();
      if (authorizedByDescriptor == (IdentityDescriptor) null)
        throw new ArgumentNullException("authorizedByDescriptor");
      IDictionary<int, WorkItemFieldData> workItemsLookup = this.GetWorkItemsLookup(links.Select<RemoteWorkItemLinkUpdate, int>((Func<RemoteWorkItemLinkUpdate, int>) (l => l.RemoteWorkItemId)).Distinct<int>());
      WorkItemTrackingLinkService linkService = this.TfsRequestContext.WitContext().LinkService;
      IEnumerable<WorkItemUpdateResult> updateResults = this.UpdateWorkItems(links.GroupBy<RemoteWorkItemLinkUpdate, int>((Func<RemoteWorkItemLinkUpdate, int>) (link => link.RemoteWorkItemId)).Select<IGrouping<int, RemoteWorkItemLinkUpdate>, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>((Func<IGrouping<int, RemoteWorkItemLinkUpdate>, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>) (group => new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate()
      {
        Id = group.Key,
        LinkUpdates = group.Select<RemoteWorkItemLinkUpdate, WorkItemLinkUpdate>((Func<RemoteWorkItemLinkUpdate, WorkItemLinkUpdate>) (link => this.ConvertToWorkItemLinkUpdate(link)))
      })), authorizedByDescriptor);
      IEnumerable<RemoteWorkItemLinkUpdateResult> linkUpdateResults = updateResults.SelectMany<WorkItemUpdateResult, WorkItemLinkUpdateResult>((Func<WorkItemUpdateResult, IEnumerable<WorkItemLinkUpdateResult>>) (wur => (IEnumerable<WorkItemLinkUpdateResult>) wur.LinkUpdates)).Select<WorkItemLinkUpdateResult, RemoteWorkItemLinkUpdateResult>((Func<WorkItemLinkUpdateResult, RemoteWorkItemLinkUpdateResult>) (lur => this.ConvertToRemoteWorkItemLinkUpdateResult(lur, workItemsLookup)));
      HashSet<(int, int, string)> successSets = new HashSet<(int, int, string)>(linkUpdateResults.Select<RemoteWorkItemLinkUpdateResult, (int, int, string)>((Func<RemoteWorkItemLinkUpdateResult, (int, int, string)>) (link => (link.LocalWorkItemId, link.RemoteWorkItemId, link.LinkTypeEnd))));
      IEnumerable<RemoteWorkItemLinkUpdateResult> second = links.Where<RemoteWorkItemLinkUpdate>((Func<RemoteWorkItemLinkUpdate, bool>) (link => !successSets.Contains(this.ConvertTupleToOtherEndPerspective(link.LocalWorkItemId, link.RemoteWorkItemId, link.LinkTypeEnd)))).Select<RemoteWorkItemLinkUpdate, RemoteWorkItemLinkUpdateResult>((Func<RemoteWorkItemLinkUpdate, RemoteWorkItemLinkUpdateResult>) (link =>
      {
        return new RemoteWorkItemLinkUpdateResult()
        {
          ChangeType = link.ChangeType,
          LinkTypeEnd = this.GetOppositeLinkTypeEnd(link.LinkTypeEnd).ReferenceName,
          LocalWorkItemId = link.RemoteWorkItemId,
          RemoteWorkItemId = link.LocalWorkItemId,
          RemoteStatus = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.RemoteStatus.Failed,
          RemoteStatusMessage = updateResults.FirstOrDefault<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (result => result.Id == link.RemoteWorkItemId))?.Exception?.Message,
          IsNonRetryableExceptionType = this.CheckForNonRetryableException(updateResults.FirstOrDefault<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (result => result.Id == link.RemoteWorkItemId))?.Exception),
          LocalHostId = this.TfsRequestContext.ServiceHost.InstanceId
        };
      }));
      IEnumerable<RemoteWorkItemLinkUpdateResult> source = linkUpdateResults.Concat<RemoteWorkItemLinkUpdateResult>(second);
      return source == null ? (IEnumerable<RemoteWorkItemLinkUpdateResult>) null : (IEnumerable<RemoteWorkItemLinkUpdateResult>) source.ToList<RemoteWorkItemLinkUpdateResult>();
    }

    [HttpPost]
    [ClientLocationId("3F0377F8-D4BF-445B-B1E7-F9E5F1BA8FDB")]
    public void CleanupRemoteLinksFromProjectDelete(
      Guid remoteHostId,
      IEnumerable<Guid> remoteProjectIds)
    {
      this.ValidateIsS2SCall();
      RemoteLinkingUtils.WriteRemoteDeletedProjectsToRegistry(this.TfsRequestContext, remoteHostId, remoteProjectIds);
      this.TfsRequestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(this.TfsRequestContext, (IEnumerable<Guid>) new Guid[1]
      {
        WorkItemTrackingJobs.RemoteLinkingProjectDelete
      });
    }

    internal virtual void ValidateIsS2SCall()
    {
      if (!ServicePrincipals.IsServicePrincipal(this.TfsRequestContext, this.TfsRequestContext.GetAuthenticatedDescriptor()))
        throw new RemoteLinkingUnauthorizedAccessException();
    }

    internal virtual IEnumerable<WorkItemUpdateResult> UpdateWorkItems(
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> updates,
      IdentityDescriptor authorizedByDescriptor)
    {
      try
      {
        using (IVssRequestContext userContext = this.TfsRequestContext.CreateUserContext(authorizedByDescriptor))
          return this.WorkItemService.UpdateWorkItemsRemoteLinkOnly(userContext, updates, true);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(904935, TraceLevel.Error, "Rest", this.TraceArea, ex);
        throw;
      }
    }

    internal virtual IDictionary<int, WorkItemFieldData> GetWorkItemsLookup(
      IEnumerable<int> workitemIds)
    {
      return (IDictionary<int, WorkItemFieldData>) this.WorkItemService.GetWorkItemFieldValues(this.TfsRequestContext, workitemIds, (IEnumerable<int>) this.fieldsToRead).ToDictionary<WorkItemFieldData, int, WorkItemFieldData>((Func<WorkItemFieldData, int>) (f => f.Id), (Func<WorkItemFieldData, WorkItemFieldData>) (f => f));
    }

    private WorkItemLinkUpdate ConvertToWorkItemLinkUpdate(RemoteWorkItemLinkUpdate link)
    {
      WorkItemLinkUpdate workItemLinkUpdate = new WorkItemLinkUpdate();
      workItemLinkUpdate.LinkType = this.GetOppositeLinkTypeEnd(link.LinkTypeEnd).Id;
      workItemLinkUpdate.SourceWorkItemId = link.RemoteWorkItemId;
      workItemLinkUpdate.TargetWorkItemId = link.LocalWorkItemId;
      workItemLinkUpdate.UpdateType = link.ChangeType == RemoteLinkChangeType.CreateOrUpdate ? LinkUpdateType.Add : LinkUpdateType.Delete;
      workItemLinkUpdate.Comment = link.Comment;
      workItemLinkUpdate.RemoteHostId = new Guid?(link.LocalHostId);
      workItemLinkUpdate.RemoteProjectId = new Guid?(link.LocalProjectId);
      workItemLinkUpdate.RemoteStatus = new Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus?(Microsoft.TeamFoundation.WorkItemTracking.Common.RemoteStatus.Success);
      workItemLinkUpdate.RemoteWatermark = new long?(link.LocalWatermark);
      workItemLinkUpdate.RemoteWorkItemTitle = link.LocalWorkItemTitle;
      workItemLinkUpdate.RemoteWorkItemType = link.LocalWorkItemType;
      return workItemLinkUpdate;
    }

    private RemoteWorkItemLinkUpdateResult ConvertToRemoteWorkItemLinkUpdateResult(
      WorkItemLinkUpdateResult result,
      IDictionary<int, WorkItemFieldData> workItemsLookup)
    {
      WorkItemFieldData workItemFieldData = workItemsLookup[result.SourceWorkItemId];
      RemoteWorkItemLinkUpdateResult linkUpdateResult = new RemoteWorkItemLinkUpdateResult();
      linkUpdateResult.LinkTypeEnd = this.WitRequestContext.LinkService.GetLinkTypeEndById(this.TfsRequestContext, result.LinkType).ReferenceName;
      linkUpdateResult.LocalWorkItemId = result.SourceWorkItemId;
      linkUpdateResult.RemoteWorkItemId = result.TargetWorkItemId;
      linkUpdateResult.ChangeType = result.UpdateType == LinkUpdateType.Delete ? RemoteLinkChangeType.Remove : RemoteLinkChangeType.CreateOrUpdate;
      linkUpdateResult.LocalHostId = this.TfsRequestContext.ServiceHost.InstanceId;
      linkUpdateResult.LocalProjectId = workItemFieldData.GetProjectGuid(this.TfsRequestContext);
      linkUpdateResult.RemoteStatus = (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.RemoteStatus) result.RemoteStatus.Value;
      linkUpdateResult.LocalWatermark = result.Timestamp;
      linkUpdateResult.LocalWorkItemTitle = workItemFieldData.Title;
      linkUpdateResult.LocalWorkItemType = workItemFieldData.WorkItemType;
      return linkUpdateResult;
    }

    private (int localId, int remoteId, string linkTypeEnd) ConvertTupleToOtherEndPerspective(
      int localId,
      int remoteId,
      string linkTypeEnd)
    {
      return (remoteId, localId, this.GetOppositeLinkTypeEnd(linkTypeEnd).ReferenceName);
    }

    private WorkItemLinkTypeEnd GetOppositeLinkTypeEnd(string linkTypeEnd) => this.WitRequestContext.LinkService.GetLinkTypeEndByReferenceName(this.TfsRequestContext, linkTypeEnd).OppositeEnd;

    private void ValidateLinksInput(IEnumerable<RemoteWorkItemLinkUpdate> links)
    {
      IEnumerable<MDWorkItemLinkType> source = this.WitRequestContext.LinkService.GetLinkTypes(this.TfsRequestContext).Where<MDWorkItemLinkType>((Func<MDWorkItemLinkType, bool>) (l => l.IsRemote));
      HashSet<string> stringSet = new HashSet<string>(source.Select<MDWorkItemLinkType, string>((Func<MDWorkItemLinkType, string>) (l => l.ForwardEnd.ReferenceName)).Concat<string>(source.Select<MDWorkItemLinkType, string>((Func<MDWorkItemLinkType, string>) (l => l.ReverseEnd.ReferenceName))));
      foreach (RemoteWorkItemLinkUpdate link in links)
      {
        ArgumentUtility.CheckForEmptyGuid(link.LocalHostId, "LocalHostId");
        ArgumentUtility.CheckForEmptyGuid(link.LocalProjectId, "LocalProjectId");
        ArgumentUtility.CheckForOutOfRange(link.LocalWorkItemId, "LocalWorkItemId", 1);
        ArgumentUtility.CheckForOutOfRange(link.RemoteWorkItemId, "RemoteWorkItemId", 1);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(link.LocalWorkItemType, "LocalWorkItemType");
        if (!stringSet.Contains(link.LinkTypeEnd))
          throw new ArgumentOutOfRangeException("LinkTypeEnd");
      }
    }

    private bool CheckForNonRetryableException(TeamFoundationServiceException exception)
    {
      switch (exception)
      {
        case WorkItemUnauthorizedAccessException _:
        case WorkItemLinkCircularException _:
          return true;
        default:
          return false;
      }
    }
  }
}
