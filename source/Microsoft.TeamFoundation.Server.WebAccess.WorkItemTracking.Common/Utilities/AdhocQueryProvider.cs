// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities.AdhocQueryProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities
{
  public static class AdhocQueryProvider
  {
    public static readonly Guid SearchResults = new Guid("2CBF5136-1AE5-4948-B59A-36F526D9AC73");
    public static readonly Guid CustomWiqlQuery = new Guid("08E20883-D56C-4461-88EB-CE77C0C7936D");
    public static readonly Guid AssignedToMe = new Guid("A2108D31-086C-4FB0-AFDA-097E4CC46DF4");
    public static readonly Guid FollowedWorkItems = new Guid("202230E0-821E-401D-96D1-24A7202330D0");
    public static readonly Guid UnsavedWorkItems = new Guid("B7A26A56-EA87-4C97-A504-3F028808BB9F");
    public static readonly Guid WorkItemsCreatedByMe = new Guid("53FB153F-C52C-42F1-90B6-CA17FC3561A8");
    public static readonly Guid RecycleBin = new Guid("2650C586-0DE4-4156-BA0E-14BCFB664CCA");
    public static readonly Guid[] QueryIds = new Guid[6]
    {
      AdhocQueryProvider.AssignedToMe,
      AdhocQueryProvider.UnsavedWorkItems,
      AdhocQueryProvider.FollowedWorkItems,
      AdhocQueryProvider.WorkItemsCreatedByMe,
      AdhocQueryProvider.SearchResults,
      AdhocQueryProvider.RecycleBin
    };

    public static void SaveAdhocQuery(
      IVssRequestContext requestContext,
      Guid projectGuid,
      Guid queryId,
      string wiql)
    {
      WiqlTextHelper.ValidateWiqlTextRequirements(requestContext, wiql);
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(requestContext))
      {
        userSettingsHive.WriteValue(AdhocQueryProvider.Etags.GetQueryRegistryPath(projectGuid, queryId), wiql);
        userSettingsHive.WriteValue(AdhocQueryProvider.Etags.GetAdhocQueriesTimestampRegistryPath(projectGuid), Convert.ToString(DateTime.Now.Ticks, 16));
      }
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetAdhocQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      WebUserSettingsHive hive,
      Guid queryId,
      string name,
      string defaultWiql)
    {
      string str = hive.ReadSetting<string>(AdhocQueryProvider.Etags.GetQueryRegistryPath(projectId, queryId), (string) null);
      if (string.IsNullOrEmpty(str))
        str = defaultWiql;
      Query query = new Query();
      query.Wiql = str;
      WiqlIdToNameTransformer.Transform(requestContext, (Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem) query);
      return new Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem()
      {
        Id = queryId,
        ParentId = Guid.Empty,
        Name = name,
        QueryText = query.Wiql
      };
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetAssignedToMeQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      WebUserSettingsHive hive)
    {
      string str = ", [System.Tags]";
      return AdhocQueryProvider.GetAdhocQuery(requestContext, projectId, hive, AdhocQueryProvider.AssignedToMe, Resources.AssignedToMeQuery, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.State], [System.AreaPath], [System.IterationPath]{0} FROM WorkItems WHERE [System.TeamProject] = @project AND [System.AssignedTo] = @me ORDER BY [System.ChangedDate] DESC", (object) str));
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetFollowedWorkItemsQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      WebUserSettingsHive hive)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem adhocQuery = AdhocQueryProvider.GetAdhocQuery(requestContext, projectId, hive, AdhocQueryProvider.FollowedWorkItems, Resources.FollowedWorkItemsQuery, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.State], [System.AreaPath], [System.IterationPath] FROM WorkItems WHERE [System.Id] in (@{0})", (object) "Follows"));
      if (adhocQuery.QueryText.EndsWith("= 0", StringComparison.OrdinalIgnoreCase))
        adhocQuery.QueryText = adhocQuery.QueryText.Replace("= 0", "in (@Follows)");
      return adhocQuery;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetUnsavedWorkItemsQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      WebUserSettingsHive hive)
    {
      string str = ", [System.Tags]";
      return AdhocQueryProvider.GetAdhocQuery(requestContext, projectId, hive, AdhocQueryProvider.UnsavedWorkItems, Resources.UnsavedWorkItemsQuery, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.State], [System.AreaPath], [System.IterationPath]{0} FROM WorkItems WHERE [System.Id] = 0", (object) str));
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetRecycleBinQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      WebUserSettingsHive hive)
    {
      return AdhocQueryProvider.GetAdhocQuery(requestContext, projectId, hive, AdhocQueryProvider.RecycleBin, Resources.RecycleBin, "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.AreaPath], [System.ChangedDate], [System.ChangedBy], [System.Tags] FROM WorkItems\r\n                                            WHERE [System.TeamProject] = @project AND [System.IsDeleted] = true\r\n                                            AND ([System.WorkItemType] NOT IN GROUP 'Microsoft.TestPlanCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestSuiteCategory'\r\n                                                 AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestCaseCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedParameterCategory'\r\n                                                 AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedStepCategory')\r\n                                                 ORDER BY [System.ChangedDate] DESC");
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetWorkItemsCreatedByMeQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      WebUserSettingsHive hive)
    {
      string str = ", [System.Tags]";
      return AdhocQueryProvider.GetAdhocQuery(requestContext, projectId, hive, AdhocQueryProvider.WorkItemsCreatedByMe, Resources.WorkItemsCreatedByMeQuery, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.State], [System.ChangedDate]{0} FROM WorkItems WHERE [System.TeamProject] = @project AND [System.CreatedBy] = @me ORDER BY [System.ChangedDate] DESC", (object) str));
    }

    public static class Etags
    {
      internal static string GetQueryRegistryPath(Guid projectId, Guid queryId) => "/Projects/" + projectId.ToString() + "/Queries/" + queryId.ToString() + "/Query";

      public static string CalculateAdhocQueryEtag(
        IVssRequestContext requestContext,
        Guid currentProjectGuid)
      {
        using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(requestContext))
          return userSettingsHive.ReadValue(AdhocQueryProvider.Etags.GetAdhocQueriesTimestampRegistryPath(currentProjectGuid));
      }

      public static string GetAdhocQueriesTimestampRegistryPath(Guid projectId) => "/Projects/" + projectId.ToString() + "/AdhocQueriesTimestamp";

      public static string GetAdHocQueriesETag(IVssRequestContext requestContext, Guid projectGuid)
      {
        string adhocQueryEtag = AdhocQueryProvider.Etags.CalculateAdhocQueryEtag(requestContext, projectGuid);
        if (requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.VisualizeFollows"))
          adhocQueryEtag += "-follows4";
        return adhocQueryEtag;
      }
    }
  }
}
