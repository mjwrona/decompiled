// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.AllPlansViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class AllPlansViewDataProvider : IExtensionDataProvider
  {
    protected Func<IVssRequestContext, ContextIdentifier> GetProject;
    protected internal Func<TestManagementRequestContext, IWorkItemServiceHelper> GetWorkItemServiceHelper;
    protected Func<TestManagementRequestContext, ISecurityManager> GetSecurityManager;
    private const int WiqlQueryResultMaxLimit = 19999;
    protected int resultMaxLimit = 19999;
    private const int BatchSize = 200;
    protected IWorkItemServiceHelper WorkItemServiceHelper;
    private const string AllTestPlansInProjectFF = "TestManagement.Server.EnableGetAllTestPlansInProject";
    private const string SortPlansAndSuitesAlphabeticallyFF = "WebAccess.TestManagement.SortPlansAndSuitesAlphabetically";

    public AllPlansViewDataProvider()
    {
      this.GetProject = (Func<IVssRequestContext, ContextIdentifier>) (requestContext => WebPageDataProviderUtil.GetPageSource(requestContext).Project);
      this.GetWorkItemServiceHelper = (Func<TestManagementRequestContext, IWorkItemServiceHelper>) (requestContext => requestContext.WorkItemServiceHelper);
      this.GetSecurityManager = (Func<TestManagementRequestContext, ISecurityManager>) (requestContext => requestContext.SecurityManager);
    }

    public virtual string Name => "TestManagement.Provider.AllPlansViewDataProvider";

    public virtual object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      AllPlansViewDataProviderContract data1 = (AllPlansViewDataProviderContract) null;
      Stopwatch getProjectTime = new Stopwatch();
      Stopwatch testResultsPermissionTime = new Stopwatch();
      Stopwatch getTeamsTime = new Stopwatch();
      Stopwatch getTeamFieldsTime = new Stopwatch();
      Stopwatch getAllPlanIdsTime = new Stopwatch();
      Stopwatch getAllPlansTime = new Stopwatch();
      Stopwatch getTeamToPlanIdsTime = new Stopwatch();
      int plansCount = 0;
      int teamsCount = 0;
      List<int> intList = new List<int>();
      bool flag1 = requestContext.IsFeatureEnabled("TestManagement.Server.EnableGetAllTestPlansInProject");
      bool flag2 = requestContext.IsFeatureEnabled("WebAccess.TestManagement.SortPlansAndSuitesAlphabetically");
      using (requestContext.TraceBlock(1015687, 1015687, "TestManagement", "WebService", this.Name))
      {
        using (PerformanceTimer.StartMeasure(requestContext, "TestManagement.AllPlansViewDataProvider"))
        {
          getProjectTime = Stopwatch.StartNew();
          ContextIdentifier project = this.GetProject(requestContext);
          getProjectTime.Stop();
          testResultsPermissionTime = Stopwatch.StartNew();
          TestManagementRequestContext context = new TestManagementRequestContext(requestContext);
          bool flag3 = false;
          if (project != null)
            flag3 = this.GetSecurityManager(context).HasViewTestResultsPermission(context, CommonStructureUtils.GetProjectUri(project.Id));
          testResultsPermissionTime.Stop();
          if (!flag3)
          {
            AllPlansViewDataProviderContract data2 = new AllPlansViewDataProviderContract(Enumerable.Empty<WebApiTeam>());
            data2.UserRestricted = true;
            return (object) data2;
          }
          this.WorkItemServiceHelper = this.GetWorkItemServiceHelper(context);
          getAllPlanIdsTime = Stopwatch.StartNew();
          string wiqlQuery = flag2 ? "SELECT [System.Id] \r\n                                                                   FROM   WorkItems \r\n                                                                   WHERE  [System.WorkItemType] in group 'Microsoft.TestPlanCategory'\r\n                                                                   AND [System.TeamProject] = @project\r\n                                                                   ORDER BY [System.Title] asc" : Utils.SelectTestPlansWiqlQuery;
          WorkItemQueryResult workItemQueryResult = this.WorkItemServiceHelper.QueryByWiql(project.Id, wiqlQuery, new int?(this.resultMaxLimit));
          getAllPlanIdsTime.Stop();
          if (workItemQueryResult != null && workItemQueryResult.WorkItems != null && workItemQueryResult.WorkItems.Count<WorkItemReference>() > 0)
            intList.AddRange(workItemQueryResult.WorkItems.Select<WorkItemReference, int>((Func<WorkItemReference, int>) (result => result.Id)));
          if (flag1)
          {
            data1 = new AllPlansViewDataProviderContract(Enumerable.Empty<WebApiTeam>());
            data1.PlansWithoutTeams = new List<int>();
            data1.AddPlanIdsToTeam(new WebApiTeam(), intList.ToList<int>());
            plansCount = intList.Count<int>();
          }
          else
          {
            getTeamsTime = Stopwatch.StartNew();
            IEnumerable<WebApiTeam> teams = AllPlansViewDataProvider.GetTeams(requestContext, project);
            teamsCount = teams.Count<WebApiTeam>();
            getTeamsTime.Stop();
            getTeamFieldsTime = Stopwatch.StartNew();
            Dictionary<WebApiTeam, TeamFieldValues> dictionary = new Dictionary<WebApiTeam, TeamFieldValues>();
            if (teams != null)
            {
              foreach (WebApiTeam key in teams)
              {
                if (key == null)
                {
                  this.TraceInfo(requestContext, "Null team instance was returned");
                }
                else
                {
                  TeamFieldValues teamFieldValues = this.WorkItemServiceHelper.GetTeamFieldValues(new Microsoft.TeamFoundation.Core.WebApi.Types.TeamContext(project.Id, new Guid?(key.Id)));
                  if (teamFieldValues != null && teamFieldValues.Field != null)
                  {
                    this.TraceInfo(requestContext, "{0} teamFieldValues returned for team {1}", (object) teamFieldValues.Field.ReferenceName, (object) key.Name);
                    dictionary.Add(key, teamFieldValues);
                  }
                }
              }
            }
            IEnumerable<string> fields = dictionary.Select<KeyValuePair<WebApiTeam, TeamFieldValues>, string>((Func<KeyValuePair<WebApiTeam, TeamFieldValues>, string>) (kvp => kvp.Value.Field.ReferenceName)).Distinct<string>();
            getTeamFieldsTime.Stop();
            getAllPlansTime = Stopwatch.StartNew();
            IList<WorkItem> workItems = this.GetWorkItems(requestContext, (IEnumerable<int>) intList, fields);
            plansCount = workItems.Count;
            getAllPlansTime.Stop();
            getTeamToPlanIdsTime = Stopwatch.StartNew();
            Dictionary<WebApiTeam, List<int>> teamToPlanIds = Utils.GetTeamToPlanIds(requestContext, teams, dictionary, workItems);
            getTeamToPlanIdsTime.Stop();
            data1 = new AllPlansViewDataProviderContract(teams);
            List<int> first = new List<int>((IEnumerable<int>) intList);
            data1.PlansWithoutTeams = first.Except<int>(teamToPlanIds.Values.SelectMany<List<int>, int>((Func<List<int>, IEnumerable<int>>) (ids => (IEnumerable<int>) ids))).ToList<int>();
            foreach (KeyValuePair<WebApiTeam, List<int>> keyValuePair in teamToPlanIds)
              data1.AddPlanIdsToTeam(keyValuePair.Key, keyValuePair.Value);
          }
        }
      }
      try
      {
        PerformanceTimer.SendCustomerIntelligenceData(requestContext, (Action<CustomerIntelligenceData>) (ciData =>
        {
          ciData.Add("Timings", requestContext.GetTraceTimingAsString());
          ciData.Add("ElapsedMillis", requestContext.LastTracedBlockElapsedMilliseconds());
          ciData.Add("getProjectTime", (double) getProjectTime.ElapsedMilliseconds);
          ciData.Add("testResultsPermissionTime", (double) testResultsPermissionTime.ElapsedMilliseconds);
          ciData.Add("getTeamsTime", (double) getTeamsTime.ElapsedMilliseconds);
          ciData.Add("getTeamFieldsTime", (double) getTeamFieldsTime.ElapsedMilliseconds);
          ciData.Add("getAllPlanIdsTime", (double) getAllPlanIdsTime.ElapsedMilliseconds);
          ciData.Add("getAllPlansTime", (double) getAllPlansTime.ElapsedMilliseconds);
          ciData.Add("getTeamToPlanIdsTime", (double) getTeamToPlanIdsTime.ElapsedMilliseconds);
          ciData.Add("plansCount", (double) plansCount);
          ciData.Add("teamsCount", (double) teamsCount);
        }));
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015687, TraceLevel.Error, "TestManagement", "WebService", "AllPlansViewDataProvider " + ex.Message);
      }
      return (object) data1;
    }

    private IList<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> ids,
      IEnumerable<string> fields)
    {
      using (requestContext.TraceBlock(1015687, 1015687, "TestManagement", "WebService", "AllPlansViewDataProvider.GetWorkItems"))
      {
        List<WorkItem> workItems1 = new List<WorkItem>();
        int count = 0;
        while (true)
        {
          IList<WorkItem> workItems2;
          do
          {
            workItems2 = this.WorkItemServiceHelper.GetWorkItems((IList<int>) ids.Skip<int>(count).Take<int>(200).ToList<int>(), (IList<string>) fields.ToList<string>(), WorkItemErrorPolicy.Fail);
            if (workItems2 != null)
            {
              this.TraceInfo(requestContext, "Received {0} items from WIT", (object) workItems2.Count);
              workItems1.AddRange((IEnumerable<WorkItem>) workItems2);
              count += 200;
            }
            else
              goto label_4;
          }
          while (workItems2.Count >= 200);
          break;
label_4:
          this.TraceInfo(requestContext, "GetWorkItems call returend null");
        }
        return (IList<WorkItem>) workItems1;
      }
    }

    private void TraceInfo(
      IVssRequestContext requestContext,
      string message,
      params object[] parameters)
    {
      VssRequestContextExtensions.Trace(requestContext, 1015687, TraceLevel.Info, "TestManagement", "WebService", "AllPlansViewDataProvider " + message, parameters);
    }

    private static IEnumerable<WebApiTeam> GetTeams(
      IVssRequestContext requestContext,
      ContextIdentifier project)
    {
      using (requestContext.TraceBlock(1015687, 1015687, "TestManagement", "WebService", "AllPlansViewDataProvider.GetTeams"))
      {
        IReadOnlyCollection<WebApiTeam> source = requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext, project.Id);
        requestContext.Trace(1015687, TraceLevel.Verbose, "TestManagement", "WebService", "{0} teams returned from TeamService", (object) (source != null ? new int?(source.Count<WebApiTeam>()) : new int?()));
        return (IEnumerable<WebApiTeam>) source;
      }
    }
  }
}
