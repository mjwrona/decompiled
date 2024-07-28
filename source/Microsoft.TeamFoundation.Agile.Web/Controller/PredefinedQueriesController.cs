// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.PredefinedQueriesController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.RecentActivity;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Mention.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "predefinedQueries", ResourceVersion = 1)]
  [ClientInternalUseOnly(true)]
  [ClientInclude(RestClientLanguages.CSharp)]
  public class PredefinedQueriesController : TfsTeamApiController
  {
    private static readonly IDictionary<string, string> IdToNameMap = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      ["AssignedToMe"] = ResourceStrings.AssignedToMe(),
      ["Following"] = ResourceStrings.Following(),
      ["Mentioned"] = ResourceStrings.Mentioned(),
      ["MyActivity"] = ResourceStrings.MyActivity()
    };

    [HttpGet]
    public IEnumerable<PredefinedQuery> GetPredefinedQueries() => (IEnumerable<PredefinedQuery>) new PredefinedQuery[4]
    {
      this.GetPredefinedQuery("AssignedToMe"),
      this.GetPredefinedQuery("Following"),
      this.GetPredefinedQuery("Mentioned"),
      this.GetPredefinedQuery("MyActivity")
    };

    [HttpGet]
    public PredefinedQuery GetPredefinedQueryResults(string id, [FromUri(Name = "$top")] int top = 2147483647, [FromUri(Name = "includeCompleted")] bool includeCompleted = false)
    {
      if (string.IsNullOrEmpty(id))
        throw new VssPropertyValidationException(nameof (id), ResourceStrings.NullOrEmptyParameter((object) nameof (id)));
      if (top <= 0)
        throw new VssPropertyValidationException("$top", ResourceStrings.QueryParameterOutOfRange((object) "$top"));
      id = id.ToLower();
      int max = top;
      if (max < int.MaxValue)
        ++max;
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> source;
      if (string.Equals(id, "AssignedToMe", StringComparison.OrdinalIgnoreCase))
        source = this.GetAssignedToMeData(includeCompleted, max);
      else if (string.Equals(id, "Following", StringComparison.OrdinalIgnoreCase))
        source = this.GetFollowingData(includeCompleted, max);
      else if (string.Equals(id, "Mentioned", StringComparison.OrdinalIgnoreCase))
      {
        source = this.GetMentionedData(includeCompleted, max);
      }
      else
      {
        if (!string.Equals(id, "MyActivity", StringComparison.OrdinalIgnoreCase))
          throw new VssPropertyValidationException(nameof (id), ResourceStrings.QueryParameterOutOfRange((object) nameof (id)));
        source = this.GetMyActivityData(includeCompleted, max);
      }
      bool flag = source.Count<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>() > top;
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = source.Take<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>(top);
      PredefinedQuery predefinedQuery = this.GetPredefinedQuery(id);
      predefinedQuery.HasMore = new bool?(flag);
      predefinedQuery.Results = workItems;
      return predefinedQuery;
    }

    private PredefinedQuery GetPredefinedQuery(string id) => new PredefinedQuery()
    {
      Id = id,
      WebUrl = WitUrlHelper.GetWorkItemsHubUrl(this.TfsRequestContext, this.ProjectInfo.Name, this.Team?.Name, id),
      Url = PredefinedQueriesController.GetPredefinedQueryUrl(this.TfsRequestContext, this.ProjectInfo.Name, this.Team?.Name, id),
      Name = PredefinedQueriesController.IdToNameMap[id]
    };

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetAssignedToMeData(
      bool includeCompleted,
      int max)
    {
      return this.ExecuteWiqlQuery(!includeCompleted ? string.Format(CommonQueries.AssignedToMeAdditionalQuery(this.TfsRequestContext, this.ProjectId), (object) this.GetCompletedClause()) : CommonQueries.AssignedToMeQuery(this.TfsRequestContext, this.ProjectId), max);
    }

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetFollowingData(
      bool includeCompleted,
      int max)
    {
      return this.ExecuteWiqlQuery(!includeCompleted ? string.Format(CommonQueries.FollowsAdditionalQuery(this.TfsRequestContext, this.ProjectId), (object) this.GetCompletedClause()) : CommonQueries.FollowsQuery(this.TfsRequestContext, this.ProjectId), max);
    }

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetMentionedData(
      bool includeCompleted,
      int max)
    {
      return this.FilterWorkItems(this.TfsRequestContext.GetService<ITeamFoundationMentionService>().GetRecentMentionsForCurrentUser(this.TfsRequestContext, new Guid?(this.ProjectId), "WorkItem", int.MaxValue).Select<LastMentionedInfo, string>((Func<LastMentionedInfo, string>) (m => m.NormalizedSourceId)).SafeToWorkItemIds(), includeCompleted, max);
    }

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetMyActivityData(
      bool includeCompleted,
      int max)
    {
      return this.FilterWorkItems(this.TfsRequestContext.GetService<ITeamFoundationRecentActivityService>().GetUserActivities(this.TfsRequestContext, this.TfsRequestContext.GetUserIdentity().Id, WorkItemArtifactKinds.WorkItem).OrderByDescending<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>, DateTime>((Func<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>, DateTime>) (kv => kv.Value.ActivityDate)).Select<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>, string>((Func<KeyValuePair<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity>, string>) (kv => kv.Key)).SafeToWorkItemIds(), includeCompleted, max);
    }

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> FilterWorkItems(
      IEnumerable<int> unfilteredWorkItemIds,
      bool includeCompleted,
      int max)
    {
      if (unfilteredWorkItemIds == null || !unfilteredWorkItemIds.Any<int>())
        return Enumerable.Empty<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>();
      Dictionary<int?, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> filteredWorkItems = this.ExecuteWiqlQuery(!includeCompleted ? string.Format(CommonQueries.FilterAdditionalQuery(this.TfsRequestContext, this.ProjectId), (object) string.Join<int>(",", unfilteredWorkItemIds), (object) this.GetCompletedClause()) : string.Format(CommonQueries.FilterQuery(this.TfsRequestContext, this.ProjectId), (object) string.Join<int>(",", unfilteredWorkItemIds)), max).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem, int?>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem, int?>) (wiRef => wiRef.Id));
      return unfilteredWorkItemIds.Where<int>((Func<int, bool>) (wid => filteredWorkItems.ContainsKey(new int?(wid)))).Select<int, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>((Func<int, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) (wid => filteredWorkItems[new int?(wid)]));
    }

    private string GetCompletedClause()
    {
      IEnumerable<string> states = (IEnumerable<string>) CommonQueries.GetStates(this.TfsRequestContext, this.ProjectId, StateGroup.Done);
      if (!states.Any<string>())
        return "[" + CoreFieldReferenceNames.Id + "] > 0";
      IEnumerable<string> values = states.Select<string, string>((Func<string, string>) (s => "'" + s + "'"));
      return "[" + CoreFieldReferenceNames.State + "] not in (" + string.Join(",", values) + ")";
    }

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> ExecuteWiqlQuery(
      string wiql,
      int max)
    {
      Dictionary<string, string> context = new Dictionary<string, string>()
      {
        ["project"] = this.ProjectInfo.Name
      };
      return this.ConvertToWorkItem(this.TfsRequestContext.GetService<IWorkItemQueryService>().ExecuteQuery(this.TfsRequestContext, wiql, (IDictionary) context, new Guid?(this.ProjectId), max, skipWiqlTextLimitValidation: true).WorkItemIds);
    }

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> ConvertToWorkItem(
      IEnumerable<int> workItemIds)
    {
      return workItemIds.Select<int, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>((Func<int, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) (wiId =>
      {
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem = WorkItemFactory.Create(this.WitRequestContext, new WorkItemFieldData((IDictionary<int, object>) new Dictionary<int, object>()
        {
          [-3] = (object) wiId
        }));
        workItem.Rev = new int?();
        workItem.Fields = (IDictionary<string, object>) null;
        return workItem;
      }));
    }

    private static string GetPredefinedQueryUrl(
      IVssRequestContext requestContext,
      string project,
      string team,
      string id)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, "work", PredefinedQueriesContants.PredefinedQueriesGuid, (object) new
        {
          team = team,
          project = project,
          id = id
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.PredefinedQueriesUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    internal virtual WorkItemTrackingRequestContext WitRequestContext => this.TfsRequestContext.WitContext();
  }
}
