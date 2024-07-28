// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.MyPlansSkinnyViewDataProvider
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
  public class MyPlansSkinnyViewDataProvider : IExtensionDataProvider
  {
    protected Func<IVssRequestContext, ContextIdentifier> GetProject;
    protected Func<TestManagementRequestContext, IWorkItemServiceHelper> GetWorkItemServiceHelper;
    protected Func<TestManagementRequestContext, ISecurityManager> GetSecurityManager;
    protected const int BatchSize = 200;
    protected const string OrOperator = "OR";
    protected const string AndOperator = "AND";
    protected const string FavoriteScopeType = "Project";
    protected IWorkItemServiceHelper WorkItemServiceHelper;

    public MyPlansSkinnyViewDataProvider()
    {
      this.GetProject = (Func<IVssRequestContext, ContextIdentifier>) (requestContext => WebPageDataProviderUtil.GetPageSource(requestContext).Project);
      this.GetWorkItemServiceHelper = (Func<TestManagementRequestContext, IWorkItemServiceHelper>) (requestContext => requestContext.WorkItemServiceHelper);
      this.GetSecurityManager = (Func<TestManagementRequestContext, ISecurityManager>) (requestContext => requestContext.SecurityManager);
    }

    public virtual string Name => "TestManagement.Provider.MineSkinnyViewDataProvider";

    public virtual object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      MyPlansSkinnyViewDataProviderContract data1 = (MyPlansSkinnyViewDataProviderContract) null;
      Stopwatch getProjectTime = (Stopwatch) null;
      Stopwatch testResultsPermissionTime = (Stopwatch) null;
      Stopwatch getTeamsTime = (Stopwatch) null;
      Stopwatch getAssociatedPlansTime = (Stopwatch) null;
      int plansCount = 0;
      int teamsCount = 0;
      int favoritesCount = 0;
      using (requestContext.TraceBlock(1015687, 1015687, "TestManagement", "WebService", this.Name))
      {
        using (PerformanceTimer.StartMeasure(requestContext, "TestManagement.MyPlansSkinnyViewDataProvider"))
        {
          getProjectTime = Stopwatch.StartNew();
          ContextIdentifier project = this.GetProject(requestContext);
          getProjectTime.Stop();
          testResultsPermissionTime = Stopwatch.StartNew();
          TestManagementRequestContext context = new TestManagementRequestContext(requestContext);
          bool flag = false;
          if (project != null)
            flag = this.GetSecurityManager(context).HasViewTestResultsPermission(context, CommonStructureUtils.GetProjectUri(project.Id));
          testResultsPermissionTime.Stop();
          if (!flag)
          {
            this.TraceInfo(requestContext, "User doesn't have view test results permission.");
            MyPlansViewDataProviderContract data2 = new MyPlansViewDataProviderContract(Enumerable.Empty<WebApiTeam>());
            data2.UserRestricted = true;
            return (object) data2;
          }
          this.WorkItemServiceHelper = this.GetWorkItemServiceHelper(context);
          getTeamsTime = Stopwatch.StartNew();
          IEnumerable<WebApiTeam> myTeams = MyPlansSkinnyViewDataProvider.GetMyTeams(requestContext, project);
          teamsCount = myTeams.Count<WebApiTeam>();
          this.TraceInfo(requestContext, "Retrieved {0} teams.", (object) teamsCount);
          getTeamsTime.Stop();
          getAssociatedPlansTime = Stopwatch.StartNew();
          Dictionary<WebApiTeam, List<int>> associatedToTeams = this.GetPlansAssociatedToTeams(requestContext, project, myTeams);
          this.TraceInfo(requestContext, "Got {0} teamToPlanIds items", (object) associatedToTeams.Count);
          getAssociatedPlansTime.Stop();
          data1 = new MyPlansSkinnyViewDataProviderContract(myTeams);
          foreach (KeyValuePair<WebApiTeam, List<int>> keyValuePair in associatedToTeams)
            data1.AddPlanIdsToTeam(keyValuePair.Key, keyValuePair.Value);
          data1.Teams.ForEach((Action<Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts.Team>) (team => plansCount += team.TestPlans.Count));
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
          ciData.Add("getAssociatedPlansTime", (double) getAssociatedPlansTime.ElapsedMilliseconds);
          ciData.Add("plansCount", (double) plansCount);
          ciData.Add("teamsCount", (double) teamsCount);
          ciData.Add("favoritesCount", (double) favoritesCount);
        }));
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015688, TraceLevel.Error, "TestManagement", "WebService", "MyPlansSkinnyViewDataProvider " + ex.Message);
      }
      return (object) data1;
    }

    private Dictionary<WebApiTeam, List<int>> GetPlansAssociatedToTeams(
      IVssRequestContext requestContext,
      ContextIdentifier project,
      IEnumerable<WebApiTeam> myTeams)
    {
      using (requestContext.TraceBlock(1015687, 1015687, "TestManagement", "WebService", nameof (GetPlansAssociatedToTeams)))
      {
        Dictionary<WebApiTeam, TeamFieldValues> dictionary = new Dictionary<WebApiTeam, TeamFieldValues>();
        foreach (WebApiTeam myTeam in myTeams)
        {
          TeamFieldValues teamFieldValues = this.WorkItemServiceHelper.GetTeamFieldValues(new Microsoft.TeamFoundation.Core.WebApi.Types.TeamContext(project.Id, new Guid?(myTeam.Id)));
          if (teamFieldValues != null)
          {
            this.TraceInfo(requestContext, "Got teamFieldValues for team {0}, referenceName: {1}", (object) myTeam.Name, (object) teamFieldValues.Field.ReferenceName);
            dictionary.Add(myTeam, teamFieldValues);
          }
        }
        IEnumerable<string> fields = dictionary.Select<KeyValuePair<WebApiTeam, TeamFieldValues>, string>((Func<KeyValuePair<WebApiTeam, TeamFieldValues>, string>) (kvp => kvp.Value.Field.ReferenceName)).Distinct<string>();
        IEnumerable<string> filters = dictionary.SelectMany<KeyValuePair<WebApiTeam, TeamFieldValues>, string>((Func<KeyValuePair<WebApiTeam, TeamFieldValues>, IEnumerable<string>>) (kvp => this.GetWiqlFilters(kvp.Value))).Distinct<string>();
        IEnumerable<string> strings = this.BuildWiqlQueries(Utils.SelectTestPlansWiqlQuery, filters);
        List<int> ids = new List<int>();
        foreach (string wiqlQuery in strings)
        {
          this.TraceInfo(requestContext, "Running WIQL query: {0}", (object) wiqlQuery);
          WorkItemQueryResult workItemQueryResult = this.WorkItemServiceHelper.QueryByWiql(project.Id, wiqlQuery);
          if (workItemQueryResult != null && workItemQueryResult.WorkItems != null && workItemQueryResult.WorkItems.Count<WorkItemReference>() > 0)
          {
            this.TraceInfo(requestContext, "Query returned {0} work items", (object) workItemQueryResult.WorkItems.Count<WorkItemReference>());
            ids.AddRange(workItemQueryResult.WorkItems.Select<WorkItemReference, int>((Func<WorkItemReference, int>) (result => result.Id)));
          }
          else
            this.TraceInfo(requestContext, "Query returned 0 work items");
        }
        IList<WorkItem> workItems = this.GetWorkItems((IEnumerable<int>) ids, fields);
        return Utils.GetTeamToPlanIds(requestContext, myTeams, dictionary, workItems);
      }
    }

    protected IList<WorkItem> GetWorkItems(IEnumerable<int> ids, IEnumerable<string> fields)
    {
      List<WorkItem> workItems1 = new List<WorkItem>();
      int count = 0;
      IList<WorkItem> workItems2;
      do
      {
        workItems2 = this.WorkItemServiceHelper.GetWorkItems((IList<int>) ids.Skip<int>(count).Take<int>(200).ToList<int>(), (IList<string>) fields.ToList<string>(), WorkItemErrorPolicy.Fail);
        workItems1.AddRange((IEnumerable<WorkItem>) workItems2);
        count += 200;
      }
      while (workItems2.Count >= 200);
      return (IList<WorkItem>) workItems1;
    }

    private IEnumerable<string> BuildWiqlQueries(string selectClause, IEnumerable<string> filters)
    {
      int num = Utils.WiqlQueryLengthLimit - (selectClause.Length + " AND (".Length + ")".Length);
      List<string> source = new List<string>();
      string str = string.Empty;
      foreach (string filter in filters)
      {
        if (str.Length + " OR ".Length + filter.Length > num)
        {
          source.Add(str);
          str = filter;
        }
        else
          str = str + (string.IsNullOrEmpty(str) ? string.Empty : " OR ") + filter;
      }
      if (!string.IsNullOrEmpty(str))
        source.Add(str);
      return source.Select<string, string>((Func<string, string>) (filter => selectClause + " AND (" + filter + ")"));
    }

    private IEnumerable<string> GetWiqlFilters(TeamFieldValues teamFieldValues)
    {
      bool referencedByAreaPath = Utils.IsAreaPath(teamFieldValues.Field.ReferenceName);
      return teamFieldValues.Values.Select<TeamFieldValue, string>((Func<TeamFieldValue, string>) (teamFieldValue => "[" + teamFieldValues.Field.ReferenceName + "] " + MyPlansSkinnyViewDataProvider.GetComparator(referencedByAreaPath, teamFieldValue) + " '" + teamFieldValue.Value + "'"));
    }

    private void TraceInfo(
      IVssRequestContext requestContext,
      string message,
      params object[] parameters)
    {
      VssRequestContextExtensions.Trace(requestContext, 1015687, TraceLevel.Info, "TestManagement", "WebService", "MyPlansSkinnyViewDataProvider " + message, parameters);
    }

    private static IEnumerable<WebApiTeam> GetMyTeams(
      IVssRequestContext requestContext,
      ContextIdentifier project)
    {
      return (IEnumerable<WebApiTeam>) requestContext.GetService<ITeamService>().QueryMyTeamsInProject(requestContext, requestContext.UserContext, project.Id);
    }

    private static string GetComparator(bool referencedByAreaPath, TeamFieldValue value) => !referencedByAreaPath || !value.IncludeChildren ? "=" : "UNDER";

    protected List<string> WorkItemFields => new List<string>()
    {
      "System.AreaPath",
      "System.AssignedTo",
      "System.Id",
      "System.IterationPath",
      "System.State",
      "System.Title"
    };
  }
}
