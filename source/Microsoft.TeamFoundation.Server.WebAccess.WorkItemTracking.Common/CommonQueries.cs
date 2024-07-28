// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CommonQueries
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class CommonQueries
  {
    private static readonly IEnumerable<string> AssignedToMeFieldReferenceNames = (IEnumerable<string>) new string[8]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.CommentCount,
      CoreFieldReferenceNames.ChangedDate
    };
    private static IEnumerable<string> FollowsFieldReferenceNames = (IEnumerable<string>) new string[9]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.CommentCount,
      CoreFieldReferenceNames.ChangedDate
    };
    private static IEnumerable<string> MentionFieldReferenceNames = (IEnumerable<string>) new string[8]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.CommentCount
    };
    private static IEnumerable<string> MyActivityFieldReferenceNames = (IEnumerable<string>) new string[8]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.CommentCount
    };
    private static IEnumerable<string> RecentlyUpdatedFieldReferenceNames = (IEnumerable<string>) new string[8]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.CommentCount
    };
    private static readonly IEnumerable<string> RecentlyCreatedFieldReferenceNames = (IEnumerable<string>) new string[9]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.CommentCount,
      CoreFieldReferenceNames.CreatedDate
    };
    private static readonly IEnumerable<string> RecentlyCompletedFieldReferenceNames = (IEnumerable<string>) new string[9]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.CommentCount,
      CoreFieldReferenceNames.ChangedDate
    };
    private static readonly IEnumerable<string> MyTeamsFieldReferenceNames = (IEnumerable<string>) new string[10]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AreaPath,
      CoreFieldReferenceNames.IterationPath,
      CoreFieldReferenceNames.Tags,
      CoreFieldReferenceNames.CommentCount,
      CoreFieldReferenceNames.ChangedDate
    };
    private static IEnumerable<string> FilterFieldReferenceNames = (IEnumerable<string>) new string[1]
    {
      CoreFieldReferenceNames.Id
    };
    private static readonly string CurrentProjectWhereClause = "[" + CoreFieldReferenceNames.TeamProject + "] = @project";
    public static readonly string AssignedToMeWhereClause = "[" + CoreFieldReferenceNames.AssignedTo + "] = @me and " + CommonQueries.CurrentProjectWhereClause;
    public static readonly string AssignedToMeAdditionalWhereClause = "[" + CoreFieldReferenceNames.AssignedTo + "] = @me and " + CommonQueries.CurrentProjectWhereClause + " and ({0})";
    public static readonly string FollowsWhereClause = "[" + CoreFieldReferenceNames.Id + "] in (@follows) and " + CommonQueries.CurrentProjectWhereClause;
    public static readonly string FollowsAdditionalWhereClause = "[" + CoreFieldReferenceNames.Id + "] in (@follows) and " + CommonQueries.CurrentProjectWhereClause + " and ({0})";
    public static readonly string MentionWhereClause = "[" + CoreFieldReferenceNames.Id + "] in (@recentMentions) and " + CommonQueries.CurrentProjectWhereClause;
    public static readonly string MyActivityWhereClause = "[" + CoreFieldReferenceNames.Id + "] in (@myRecentActivity) and " + CommonQueries.CurrentProjectWhereClause;
    public static readonly string RecentlyUpdatedWhereClause = "[" + CoreFieldReferenceNames.Id + "] in (@recentProjectActivity) and " + CommonQueries.CurrentProjectWhereClause;
    public static readonly string FilterWhereClause = "[" + CoreFieldReferenceNames.Id + "] in ({0}) and " + CommonQueries.CurrentProjectWhereClause;
    public static readonly string FilterAdditionalWhereClause = "[" + CoreFieldReferenceNames.Id + "] in ({0}) and " + CommonQueries.CurrentProjectWhereClause + " and ({1})";
    public static readonly string AssignedToMeOrderByField = CoreFieldReferenceNames.ChangedDate;
    public static readonly string FollowsOrderByField = CoreFieldReferenceNames.ChangedDate;
    public static readonly string MentionOrderByField = CoreFieldReferenceNames.ChangedDate;
    public static readonly string MyActivityOrderByField = CoreFieldReferenceNames.ChangedDate;
    public static readonly string RecentlyUpdatedOrderByField = CoreFieldReferenceNames.ChangedDate;
    public static readonly string RecentlyCreatedOrderByField = CoreFieldReferenceNames.CreatedDate;
    public static readonly string RecentlyCompletedOrderByField = CoreFieldReferenceNames.ChangedDate;
    public static readonly string MyTeamsOrderByField = CoreFieldReferenceNames.ChangedDate;
    public static readonly string FilterOrderByField = CoreFieldReferenceNames.Id;
    public static readonly string AssignedToMeOrderByClause = CommonQueries.AssignedToMeOrderByField + " desc";
    public static readonly string FollowsOrderByClause = CommonQueries.FollowsOrderByField + " desc";
    public static readonly string MentionOrderByClause = CommonQueries.MentionOrderByField + " desc";
    public static readonly string MyActivityOrderByClause = CommonQueries.MyActivityOrderByField + " desc";
    public static readonly string RecentlyUpdatedOrderByClause = CommonQueries.RecentlyUpdatedOrderByField + " desc";
    public static readonly string RecentlyCreatedOrderByClause = CommonQueries.RecentlyCreatedOrderByField + " desc";
    public static readonly string RecentlyCompletedOrderByClause = CommonQueries.RecentlyCompletedOrderByField + " desc";
    public static readonly string MyTeamsOrderByClause = CommonQueries.MyTeamsOrderByField + " desc";
    public static readonly string FilterOrderByClause = CommonQueries.FilterOrderByField + " desc";

    public static IEnumerable<string> GetAssignedToMeFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.RemoveUnsupportedFields(CommonQueries.AssignedToMeFieldReferenceNames, requestContext, projectId);
    }

    public static IEnumerable<string> GetFollowsFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.RemoveUnsupportedFields(CommonQueries.FollowsFieldReferenceNames, requestContext, projectId);
    }

    public static IEnumerable<string> GetMentionFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.RemoveUnsupportedFields(CommonQueries.MentionFieldReferenceNames, requestContext, projectId);
    }

    public static IEnumerable<string> GetMyActivityFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.RemoveUnsupportedFields(CommonQueries.MyActivityFieldReferenceNames, requestContext, projectId);
    }

    public static IEnumerable<string> GetRecentlyUpdatedFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.RemoveUnsupportedFields(CommonQueries.RecentlyUpdatedFieldReferenceNames, requestContext, projectId);
    }

    public static IEnumerable<string> GetRecentlyCreatedFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.RemoveUnsupportedFields(CommonQueries.RecentlyCreatedFieldReferenceNames, requestContext, projectId);
    }

    public static IEnumerable<string> GetRecentlyCompletedFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.RemoveUnsupportedFields(CommonQueries.RecentlyCompletedFieldReferenceNames, requestContext, projectId);
    }

    public static IEnumerable<string> GetMyTeamsFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.RemoveUnsupportedFields(CommonQueries.MyTeamsFieldReferenceNames, requestContext, projectId);
    }

    public static IEnumerable<string> GetFilterFieldReferenceNames(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.RemoveUnsupportedFields(CommonQueries.FilterFieldReferenceNames, requestContext, projectId);
    }

    public static string AssignedToMeQueryTemplate(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.GenerateQueryTemplate(CommonQueries.GetAssignedToMeFieldReferenceNames(requestContext, projectId));
    }

    public static string FollowsQueryTemplate(IVssRequestContext requestContext, Guid projectId) => CommonQueries.GenerateQueryTemplate(CommonQueries.GetFollowsFieldReferenceNames(requestContext, projectId));

    public static string MyActivityQueryTemplate(IVssRequestContext requestContext, Guid projectId) => CommonQueries.GenerateQueryTemplate(CommonQueries.GetMyActivityFieldReferenceNames(requestContext, projectId));

    public static string RecentlyUpdatedQueryTemplate(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.GenerateQueryTemplate(CommonQueries.GetRecentlyUpdatedFieldReferenceNames(requestContext, projectId));
    }

    public static string RecentMentionedQueryTemplate(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.GenerateQueryTemplate(CommonQueries.GetMentionFieldReferenceNames(requestContext, projectId));
    }

    public static string RecentlyCreatedQueryTemplate(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.GenerateQueryTemplate(CommonQueries.GetRecentlyCreatedFieldReferenceNames(requestContext, projectId));
    }

    public static string RecentlyCompletedQueryTemplate(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.GenerateQueryTemplate(CommonQueries.GetRecentlyCompletedFieldReferenceNames(requestContext, projectId));
    }

    public static string MyTeamsQueryTemplate(IVssRequestContext requestContext, Guid projectId) => CommonQueries.GenerateQueryTemplate(CommonQueries.GetMyTeamsFieldReferenceNames(requestContext, projectId));

    public static string FilterQueryTemplate(IVssRequestContext requestContext, Guid projectId) => CommonQueries.GenerateQueryTemplate(CommonQueries.GetFilterFieldReferenceNames(requestContext, projectId));

    public static string AssignedToMeQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      bool includeCompletedItems = true)
    {
      return includeCompletedItems ? CommonQueries.GenerateQuery(CommonQueries.AssignedToMeQueryTemplate(requestContext, projectId), CommonQueries.AssignedToMeWhereClause, CommonQueries.AssignedToMeOrderByClause) : string.Format(CommonQueries.AssignedToMeAdditionalQuery(requestContext, projectId), (object) CommonQueries.GetDoingClause(requestContext, projectId));
    }

    public static string AssignedToMeAdditionalQuery(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonQueries.GenerateQuery(CommonQueries.AssignedToMeQueryTemplate(requestContext, projectId), CommonQueries.AssignedToMeAdditionalWhereClause, CommonQueries.AssignedToMeOrderByClause);
    }

    public static string FollowsQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      bool includeCompletedItems = true)
    {
      return includeCompletedItems ? CommonQueries.GenerateQuery(CommonQueries.FollowsQueryTemplate(requestContext, projectId), CommonQueries.FollowsWhereClause, CommonQueries.FollowsOrderByClause) : string.Format(CommonQueries.FollowsAdditionalQuery(requestContext, projectId), (object) CommonQueries.GetDoingClause(requestContext, projectId));
    }

    public static string FollowsAdditionalQuery(IVssRequestContext requestContext, Guid projectId) => CommonQueries.GenerateQuery(CommonQueries.FollowsQueryTemplate(requestContext, projectId), CommonQueries.FollowsAdditionalWhereClause, CommonQueries.FollowsOrderByClause);

    public static string FilterQuery(IVssRequestContext requestContext, Guid projectId) => CommonQueries.GenerateQuery(CommonQueries.FilterQueryTemplate(requestContext, projectId), CommonQueries.FilterWhereClause, CommonQueries.FilterOrderByClause);

    public static string FilterAdditionalQuery(IVssRequestContext requestContext, Guid projectId) => CommonQueries.GenerateQuery(CommonQueries.FilterQueryTemplate(requestContext, projectId), CommonQueries.FilterAdditionalWhereClause, CommonQueries.FilterOrderByClause);

    public static string GetRecentlyCreatedQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      int recentlyCreatedDurationInDays,
      bool includeCompletedItems = true)
    {
      return CommonQueries.GenerateQuery(CommonQueries.RecentlyCreatedQueryTemplate(requestContext, projectId), CommonQueries.GetRecentlyCreatedWhereClause(requestContext, recentlyCreatedDurationInDays, projectId, includeCompletedItems), CommonQueries.RecentlyCreatedOrderByClause);
    }

    public static string GetRecentlyCreatedWhereClause(
      IVssRequestContext requestContext,
      int recentlyCreatedDurationInDays,
      Guid projectId,
      bool includeCompletedItems = true)
    {
      string createdWhereClause = string.Format("[{0}] >= {1} - {2} and {3}", (object) CoreFieldReferenceNames.CreatedDate, (object) "@today", (object) recentlyCreatedDurationInDays, (object) CommonQueries.CurrentProjectWhereClause);
      if (!includeCompletedItems)
        createdWhereClause = createdWhereClause + " and " + CommonQueries.GetDoingClause(requestContext, projectId);
      return createdWhereClause;
    }

    public static string GetRecentlyCompletedQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      int recentlyCompletedDurationInDays)
    {
      return CommonQueries.GenerateQuery(CommonQueries.RecentlyCompletedQueryTemplate(requestContext, projectId), CommonQueries.GetRecentlyCompletedWhereClause(requestContext, projectId, recentlyCompletedDurationInDays), CommonQueries.RecentlyCompletedOrderByClause);
    }

    public static string GetMyTeamsQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      string teamsAreasClauses,
      bool includeCompletedItems = true)
    {
      string whereClause = teamsAreasClauses;
      if (!includeCompletedItems)
        whereClause = teamsAreasClauses + " and (" + CommonQueries.GetDoingClause(requestContext, projectId) + ")";
      return CommonQueries.GenerateQuery(CommonQueries.MyTeamsQueryTemplate(requestContext, projectId), whereClause, CommonQueries.RecentlyCompletedOrderByClause);
    }

    public static string GetRecentlyCompletedWhereClause(
      IVssRequestContext requestContext,
      Guid projectId,
      int recentlyCompletedDurationInDays)
    {
      string str = string.Join(",", CommonQueries.GetStates(requestContext, projectId, StateGroup.Done).Select<string, string>((Func<string, string>) (state => "'" + state + "'")));
      return string.Format("[{0}] IN ({1}) and [{2}] >= {3} - {4} and {5}", (object) CoreFieldReferenceNames.State, (object) str, (object) CoreFieldReferenceNames.ChangedDate, (object) "@today", (object) recentlyCompletedDurationInDays, (object) CommonQueries.CurrentProjectWhereClause);
    }

    public static IReadOnlyCollection<string> GetStates(
      IVssRequestContext requestContext,
      Guid projectId,
      StateGroup stateGroup)
    {
      using (PerformanceTimer.StartMeasure(requestContext, nameof (GetStates)))
        return requestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemStates(requestContext, projectId, stateGroup, false);
    }

    private static string GetDoingClause(IVssRequestContext requestContext, Guid projectId)
    {
      IEnumerable<string> states = (IEnumerable<string>) CommonQueries.GetStates(requestContext, projectId, StateGroup.Done);
      if (!states.Any<string>())
        return "[" + CoreFieldReferenceNames.Id + "] > 0";
      IEnumerable<string> values = states.Select<string, string>((Func<string, string>) (s => "'" + s + "'"));
      return "[" + CoreFieldReferenceNames.State + "] not in (" + string.Join(",", values) + ")";
    }

    private static string GenerateQuery(
      string queryTemplate,
      string whereClause,
      string orderByClause)
    {
      return string.Format(queryTemplate, (object) whereClause, (object) orderByClause);
    }

    public static string GenerateQueryTemplate(IEnumerable<string> fields) => "select " + string.Join(", ", fields) + " from WorkItems where {0} order by {1}";

    private static IEnumerable<string> RemoveUnsupportedFields(
      IEnumerable<string> fieldNames,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      List<string> stringList = new List<string>(fieldNames);
      if (requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.WorkItemsHub.DisableCommentCount") || !requestContext.WitContext().FieldDictionary.TryGetFieldByNameOrId(CoreFieldReferenceNames.CommentCount, out FieldEntry _))
        stringList.RemoveAll((Predicate<string>) (fieldName => TFStringComparer.WorkItemFieldReferenceName.Equals(CoreFieldReferenceNames.CommentCount, fieldName)));
      return (IEnumerable<string>) stringList;
    }
  }
}
