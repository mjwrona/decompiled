// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Utils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.SignalR;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public static class Utils
  {
    public static readonly string SelectTestPlansWiqlQuery = "SELECT [System.Id] \r\n                                                                   FROM   WorkItems \r\n                                                                   WHERE  [System.TeamProject] = @project\r\n                                                                   AND [System.WorkItemType] in group 'Microsoft.TestPlanCategory'";
    public static readonly string SelectTestPlansWiqlQueryInitial = "SELECT [System.Id] \r\n                                                                   FROM   WorkItems \r\n                                                                   WHERE  [System.WorkItemType] in group 'Microsoft.TestPlanCategory'\r\n                                                                   AND [System.TeamProject] = @project\r\n                                                                   ORDER BY [System.Id] asc";
    public static readonly int WiqlQueryLengthLimit = 32000;
    private const char AreaPathSeparator = '\\';
    private static readonly string ReleaseIdQueryParamater = "releaseId";
    private static readonly string ReleaseEnvIdQueryParamater = "environmentId";
    private static readonly string BuildIdQueryParamater = "buildId";

    public static Dictionary<WebApiTeam, List<int>> GetTeamToPlanIds(
      IVssRequestContext requestContext,
      IEnumerable<WebApiTeam> teams,
      Dictionary<WebApiTeam, TeamFieldValues> teamToTeamFieldValues,
      IList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems)
    {
      using (requestContext.TraceBlock(1015686, 1015686, "TestManagement", "WebService", "Utils.GetTeamToPlanIds"))
      {
        IEnumerable<string> allTeamFields = teamToTeamFieldValues.Select<KeyValuePair<WebApiTeam, TeamFieldValues>, string>((Func<KeyValuePair<WebApiTeam, TeamFieldValues>, string>) (kvp => kvp.Value.Field.ReferenceName)).Distinct<string>();
        Dictionary<string, Dictionary<string, Dictionary<bool, List<int>>>> partitionedWorkItems = Utils.GetPartitionedWorkItems(requestContext, allTeamFields, workItems);
        Dictionary<WebApiTeam, List<int>> dictionary = teams.ToDictionary<WebApiTeam, WebApiTeam, List<int>>((Func<WebApiTeam, WebApiTeam>) (team => team), (Func<WebApiTeam, List<int>>) (team => new List<int>()));
        foreach (KeyValuePair<WebApiTeam, TeamFieldValues> toTeamFieldValue in teamToTeamFieldValues)
        {
          foreach (TeamFieldValue teamFieldValue in toTeamFieldValue.Value.Values)
          {
            string referenceName = toTeamFieldValue.Value.Field.ReferenceName;
            requestContext.Trace(1015686, TraceLevel.Info, "TestManagement", "WebService", "referencedByField = {0}", (object) referenceName);
            if (!partitionedWorkItems[referenceName].ContainsKey(teamFieldValue.Value))
            {
              requestContext.Trace(1015686, TraceLevel.Info, "TestManagement", "WebService", "Skipping teamFieldValue {0} because it doesn't exist in partition[{1}]", (object) teamFieldValue.Value, (object) referenceName);
            }
            else
            {
              dictionary[toTeamFieldValue.Key].AddRange((IEnumerable<int>) partitionedWorkItems[referenceName][teamFieldValue.Value][true]);
              if (Utils.IsAreaPath(toTeamFieldValue.Value.Field.ReferenceName) && teamFieldValue.IncludeChildren)
                dictionary[toTeamFieldValue.Key].AddRange((IEnumerable<int>) partitionedWorkItems[toTeamFieldValue.Value.Field.ReferenceName][teamFieldValue.Value][false]);
            }
          }
        }
        return dictionary;
      }
    }

    public static (Guid projectId, string projectName, Guid teamId, string pageUrl) GetProjectAndTeamData(
      IVssRequestContext requestContext)
    {
      WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(requestContext);
      Guid? id = pageSource?.Project?.Id;
      Guid guid1 = id ?? Guid.Empty;
      string str = pageSource?.Project?.Name ?? string.Empty;
      id = pageSource?.Team?.Id;
      Guid guid2 = id ?? Guid.Empty;
      string url = pageSource?.Url;
      return (guid1, str, guid2, url);
    }

    public static UserFriendlyTestOutcome GetUserFriendlyTestOutcome(string outcome, string state)
    {
      if (string.IsNullOrEmpty(outcome) || string.IsNullOrEmpty(state))
        return UserFriendlyTestOutcome.Ready;
      if (outcome.Equals("Timeout", StringComparison.OrdinalIgnoreCase))
        return UserFriendlyTestOutcome.Timeout;
      if (outcome.Equals("Warning", StringComparison.OrdinalIgnoreCase))
        return UserFriendlyTestOutcome.Warning;
      if (outcome.Equals("Error", StringComparison.OrdinalIgnoreCase))
        return UserFriendlyTestOutcome.Error;
      if (outcome.Equals("Aborted", StringComparison.OrdinalIgnoreCase))
        return UserFriendlyTestOutcome.Aborted;
      if (outcome.Equals("NotExecuted", StringComparison.OrdinalIgnoreCase))
        return UserFriendlyTestOutcome.NotExecuted;
      if (outcome.Equals("Inconclusive", StringComparison.OrdinalIgnoreCase))
        return UserFriendlyTestOutcome.Inconclusive;
      if (outcome.Equals("None", StringComparison.OrdinalIgnoreCase))
        return UserFriendlyTestOutcome.None;
      if (outcome.Equals("Unspecified", StringComparison.OrdinalIgnoreCase))
        return UserFriendlyTestOutcome.Unspecified;
      if (outcome.Equals("NotImpacted", StringComparison.OrdinalIgnoreCase))
        return UserFriendlyTestOutcome.NotImpacted;
      if (state.Equals("Completed", StringComparison.OrdinalIgnoreCase))
      {
        if (outcome.Equals("NotApplicable", StringComparison.OrdinalIgnoreCase))
          return UserFriendlyTestOutcome.NotApplicable;
        if (outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase))
          return UserFriendlyTestOutcome.Passed;
        if (outcome.Equals("Blocked", StringComparison.OrdinalIgnoreCase))
          return UserFriendlyTestOutcome.Blocked;
        return outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase) ? UserFriendlyTestOutcome.Failed : UserFriendlyTestOutcome.Ready;
      }
      return outcome.Equals("Paused", StringComparison.OrdinalIgnoreCase) ? UserFriendlyTestOutcome.Paused : UserFriendlyTestOutcome.InProgress;
    }

    public static bool IsAreaPath(string referenceName) => referenceName == "System.AreaPath";

    public static IEnumerable<WebApiTeam> GetTeams(
      IVssRequestContext requestContext,
      ContextIdentifier project)
    {
      return (IEnumerable<WebApiTeam>) requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext, project.Id);
    }

    public static WebApiTeam GetDefaultTeam(
      IVssRequestContext requestContext,
      ContextIdentifier project)
    {
      return requestContext.GetService<ITeamService>().GetDefaultTeam(requestContext, project.Id);
    }

    public static bool TryGetReleaseEnvIdFromQueryParameter(
      IVssRequestContext requestContext,
      int tracePoint,
      out int releaseEnvId)
    {
      if (int.TryParse(requestContext.GetService<IContributionRoutingService>().GetQueryParameter(requestContext, Utils.ReleaseEnvIdQueryParamater), out releaseEnvId) || releaseEnvId != 0)
        return true;
      requestContext.Trace(tracePoint, TraceLevel.Error, "TestManagement", "WebService", "Provider could not get releaseEnvId");
      return false;
    }

    public static bool TryGetReleaseIdFromQueryParameter(
      IVssRequestContext requestContext,
      int tracePoint,
      out int releaseId)
    {
      if (int.TryParse(requestContext.GetService<IContributionRoutingService>().GetQueryParameter(requestContext, Utils.ReleaseIdQueryParamater), out releaseId) || releaseId != 0)
        return true;
      requestContext.Trace(tracePoint, TraceLevel.Error, "TestManagement", "WebService", "Provider could not get releaseId");
      return false;
    }

    public static bool TryGetProjectInfo(
      IVssRequestContext requestContext,
      int tracePoint,
      out ProjectInfo projectInfo)
    {
      IRequestProjectService service = requestContext.GetService<IRequestProjectService>();
      projectInfo = service.GetProject(requestContext);
      if (projectInfo != null)
        return true;
      requestContext.Trace(tracePoint, TraceLevel.Error, "TestManagement", "WebService", "Provider could not get project");
      return false;
    }

    public static bool TryGetBuildIdFromQueryParameter(
      IVssRequestContext requestContext,
      int tracePoint,
      out int buildId)
    {
      if (int.TryParse(requestContext.GetService<IContributionRoutingService>().GetQueryParameter(requestContext, Utils.BuildIdQueryParamater), out buildId) || buildId != 0)
        return true;
      requestContext.Trace(tracePoint, TraceLevel.Error, "TestManagement", "WebService", "Provider could not get buildId");
      return false;
    }

    public static string GetTcmServiceUrl(IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsHostedDeployment ? requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, TestManagementServerConstants.TCMServiceInstanceType, AccessMappingConstants.ClientAccessMappingMoniker) : (string) null;

    public static string GetConnectionUrl(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        return (string) null;
      ProjectInfo projectInfo;
      string url;
      return requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.RootContext.Items.TryGetValue<string, ProjectInfo>("RequestProject", out projectInfo) && Utils.TryGetUrl(requestContext, "tfs.signalr.project", (object) new
      {
        project = projectInfo.Id.ToString("D")
      }, out url) ? url : VssSignalRUtility.GetHubsUrl(requestContext);
    }

    private static bool TryGetUrl(
      IVssRequestContext requestContext,
      string routeName,
      object routeValues,
      out string url)
    {
      url = (string) null;
      if (!(RouteTable.Routes[routeName] is Route route))
        return false;
      string str1 = !requestContext.ExecutionEnvironment.IsHostedDeployment ? VirtualPathUtility.ToAbsolute(requestContext.VirtualPath()) : requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, TestManagementServerConstants.TCMServiceInstanceType, AccessMappingConstants.ClientAccessMappingMoniker);
      if (str1 == null)
        return false;
      string uriString = str1.TrimEnd('/');
      string str2 = VssHttpUriUtility.ReplaceRouteValues(route.Url, VssHttpUriUtility.ToRouteDictionary(routeValues)).TrimStart('/');
      if (Utils.UseSignalRAppPool(requestContext))
      {
        if (requestContext.IsVstsDomainRequest())
        {
          url = uriString + "/_signalr/" + str2;
        }
        else
        {
          Uri uri = new Uri(uriString);
          url = uri.Scheme + "://" + uri.Authority + "/_signalr" + uri.AbsolutePath + "/" + str2;
        }
      }
      else
        url = uriString + "/" + str2;
      return true;
    }

    private static bool UseSignalRAppPool(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("TestManagement.SignalR.SeparateAppPool") && requestContext.ExecutionEnvironment.IsHostedDeployment;

    private static Dictionary<string, Dictionary<string, Dictionary<bool, List<int>>>> GetPartitionedWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<string> allTeamFields,
      IList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems)
    {
      using (requestContext.TraceBlock(1015686, 1015686, "TestManagement", "WebService", "Utils.GetPartitionedWorkItems"))
      {
        Dictionary<string, Dictionary<string, Dictionary<bool, List<int>>>> partitionedWorkItems = new Dictionary<string, Dictionary<string, Dictionary<bool, List<int>>>>();
        requestContext.Trace(1015686, TraceLevel.Info, "TestManagement", "WebService", "allTeamFields contains {0} elements", (object) allTeamFields.Count<string>());
        HashSet<string> stringSet = new HashSet<string>(allTeamFields);
        foreach (string allTeamField in allTeamFields)
        {
          requestContext.Trace(1015686, TraceLevel.Info, "TestManagement", "WebService", "Adding teamField {0} to map", (object) allTeamField);
          partitionedWorkItems.Add(allTeamField, new Dictionary<string, Dictionary<bool, List<int>>>());
        }
        foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) workItems)
        {
          foreach (KeyValuePair<string, object> field in (IEnumerable<KeyValuePair<string, object>>) workItem.Fields)
          {
            if (!stringSet.Contains(field.Key))
            {
              requestContext.Trace(1015686, TraceLevel.Info, "TestManagement", "WebService", "Skipping field {0} of work item {1} since it isn't present in hashset", (object) field.Key, (object) workItem.Id.GetValueOrDefault());
            }
            else
            {
              string key1 = Convert.ToString(field.Value);
              if (!partitionedWorkItems[field.Key].ContainsKey(key1))
                partitionedWorkItems[field.Key].Add(key1, new Dictionary<bool, List<int>>()
                {
                  {
                    true,
                    new List<int>()
                  },
                  {
                    false,
                    new List<int>()
                  }
                });
              List<int> intList1 = partitionedWorkItems[field.Key][key1][true];
              int? id = workItem.Id;
              int valueOrDefault1 = id.GetValueOrDefault();
              intList1.Add(valueOrDefault1);
              if (Utils.IsAreaPath(field.Key))
              {
                string[] array = ((IEnumerable<string>) key1.Split('\\')).Reverse<string>().Skip<string>(1).Reverse<string>().ToArray<string>();
                string key2 = string.Empty;
                foreach (string str in array)
                {
                  key2 = key2 + (string.IsNullOrEmpty(key2) ? string.Empty : '\\'.ToString()) + str;
                  if (!partitionedWorkItems[field.Key].ContainsKey(key2))
                    partitionedWorkItems[field.Key].Add(key2, new Dictionary<bool, List<int>>()
                    {
                      {
                        true,
                        new List<int>()
                      },
                      {
                        false,
                        new List<int>()
                      }
                    });
                  List<int> intList2 = partitionedWorkItems[field.Key][key2][false];
                  id = workItem.Id;
                  int valueOrDefault2 = id.GetValueOrDefault();
                  intList2.Add(valueOrDefault2);
                }
              }
            }
          }
        }
        return partitionedWorkItems;
      }
    }

    public static T InvokeAction<T>(Func<T> func)
    {
      try
      {
        return func();
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        throw;
      }
    }
  }
}
