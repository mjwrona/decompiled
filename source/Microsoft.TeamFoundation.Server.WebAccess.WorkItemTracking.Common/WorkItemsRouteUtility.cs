// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemsRouteUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Specialized;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class WorkItemsRouteUtility
  {
    public static bool IsEditWorkItem(string action, string id) => WorkItemsRouteUtility.AreEqualActions(action, "edit") && WorkItemsRouteUtility.IsValidWorkItemIdFormat(id);

    public static bool IsNewWorkItem(string action, string witd) => WorkItemsRouteUtility.AreEqualActions(action, "new") && !string.IsNullOrWhiteSpace(witd);

    public static bool IsQueryHubRouteValue(string routeValue) => WorkItemsRouteUtility.AreEqualActions(routeValue, "resultsbyid") || WorkItemsRouteUtility.AreEqualActions(routeValue, "results") || WorkItemsRouteUtility.AreEqualActions(routeValue, "adhocquery");

    public static bool ShouldPrefetchQueryItem(string action) => WorkItemsRouteUtility.AreEqualActions(action, "folder") || WorkItemsRouteUtility.IsQueryAction(action);

    public static bool IsFavoriteQueriesAction(string action) => WorkItemsRouteUtility.AreEqualActions(action, "favorites");

    public static bool IsAllQueriesAction(string action) => WorkItemsRouteUtility.AreEqualActions(action, "all");

    public static bool IsQueryHubQueryParameters(
      string tempQueryId,
      string action,
      string id,
      string queryId)
    {
      return (WorkItemsRouteUtility.IsValidQueryIdFormat(tempQueryId) || WorkItemsRouteUtility.IsQueryAction(action) || WorkItemsRouteUtility.IsNoActionWithGuidId(action, id)) && !WorkItemsRouteUtility.IsVsOpenWithWorkItemIdAndInvalidQueryId(action, id, queryId);
    }

    public static bool IsTempQueryIdWithNoQueryAction(string action, string tempQueryId) => WorkItemsRouteUtility.IsValidQueryIdFormat(tempQueryId) && !WorkItemsRouteUtility.AreEqualActions(action, "query");

    public static bool ShouldAddTemplateOwnerId(NameValueCollection queryParameters)
    {
      string str1 = queryParameters.Get("templateId");
      string str2 = queryParameters.Get("ownerId");
      return !string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2);
    }

    public static bool IsVsOpenWithWorkItemIdAndInvalidQueryId(
      string action,
      string id,
      string queryId)
    {
      return WorkItemsRouteUtility.AreEqualActions(action, "vsopen") && !WorkItemsRouteUtility.IsValidQueryIdFormat(queryId) && !string.IsNullOrEmpty(id);
    }

    public static bool IsNoActionWithValidWorkItemId(string action, string id) => string.IsNullOrEmpty(action) && WorkItemsRouteUtility.IsValidWorkItemIdFormat(id);

    public static NameValueCollection RemoveActionRelatedParameters(
      NameValueCollection queryParameters)
    {
      queryParameters.Remove("_a");
      queryParameters.Remove("id");
      queryParameters.Remove("witd");
      return queryParameters;
    }

    public static bool IsQueryAction(string action) => WorkItemsRouteUtility.AreEqualActions(action, "query") || WorkItemsRouteUtility.AreEqualActions(action, "query-edit") || WorkItemsRouteUtility.AreEqualActions(action, "query-charts") || WorkItemsRouteUtility.AreEqualActions(action, "search") || WorkItemsRouteUtility.AreEqualActions(action, "contribution") || WorkItemsRouteUtility.AreEqualActions(action, "vsopen");

    internal static bool IsNoActionWithGuidId(string action, string id) => string.IsNullOrEmpty(action) && WorkItemsRouteUtility.IsValidQueryIdFormat(id);

    internal static bool IsValidQueryIdFormat(string value) => !string.IsNullOrEmpty(value) && Guid.TryParse(value, out Guid _);

    public static bool AreEqualActions(string action1, string action2) => string.Equals(action1, action2, StringComparison.OrdinalIgnoreCase);

    internal static bool IsValidWorkItemIdFormat(string id)
    {
      int result;
      return !string.IsNullOrEmpty(id) && int.TryParse(id, out result) && result > 0;
    }
  }
}
