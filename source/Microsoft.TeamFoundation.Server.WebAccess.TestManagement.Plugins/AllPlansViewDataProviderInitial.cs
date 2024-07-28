// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.AllPlansViewDataProviderInitial
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class AllPlansViewDataProviderInitial : AllPlansViewDataProvider
  {
    private const string SortPlansAndSuitesAlphabeticallyFF = "WebAccess.TestManagement.SortPlansAndSuitesAlphabetically";

    public override string Name => "TestManagement.Provider.AllPlansViewDataProviderInitial";

    public AllPlansViewDataProviderInitial() => this.resultMaxLimit = 25;

    public override object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ContextIdentifier contextIdentifier = this.GetProject(requestContext);
      TestManagementRequestContext context = new TestManagementRequestContext(requestContext);
      if (!this.GetSecurityManager(context).HasViewTestResultsPermission(context, CommonStructureUtils.GetProjectUri(contextIdentifier.Id)))
      {
        AllPlansViewDataProviderContract data = new AllPlansViewDataProviderContract(Enumerable.Empty<WebApiTeam>());
        data.UserRestricted = true;
        return (object) data;
      }
      this.WorkItemServiceHelper = this.GetWorkItemServiceHelper(context);
      List<int> intList = new List<int>();
      bool flag = requestContext.IsFeatureEnabled("WebAccess.TestManagement.SortPlansAndSuitesAlphabetically");
      string wiqlQuery = flag ? "SELECT [System.Id] \r\n                                                                   FROM   WorkItems \r\n                                                                   WHERE  [System.WorkItemType] in group 'Microsoft.TestPlanCategory'\r\n                                                                   AND [System.TeamProject] = @project\r\n                                                                   ORDER BY [System.Title] asc" : Utils.SelectTestPlansWiqlQuery;
      WorkItemQueryResult workItemQueryResult = this.WorkItemServiceHelper.QueryByWiql(contextIdentifier.Id, wiqlQuery, new int?(this.resultMaxLimit));
      if (workItemQueryResult != null && workItemQueryResult.WorkItems != null && workItemQueryResult.WorkItems.Count<WorkItemReference>() > 0)
        intList.AddRange(workItemQueryResult.WorkItems.Select<WorkItemReference, int>((Func<WorkItemReference, int>) (result => result.Id)));
      AllPlansInitialViewDataProviderContract data1 = new AllPlansInitialViewDataProviderContract(Enumerable.Empty<WebApiTeam>());
      data1.PlansWithoutTeams = new List<int>((IEnumerable<int>) intList);
      data1.AddPlanIdsToTeam(new WebApiTeam(), intList.ToList<int>());
      IList<WorkItem> workItemList = this.GetWorkItems((IEnumerable<int>) intList, (IEnumerable<string>) this.WorkItemFields);
      if (flag)
        workItemList = (IList<WorkItem>) workItemList.OrderBy<WorkItem, object>((Func<WorkItem, object>) (x => x.Fields["System.Title"])).ToList<WorkItem>();
      data1.AddTestPlans(workItemList);
      return (object) data1;
    }

    private IList<WorkItem> GetWorkItems(IEnumerable<int> ids, IEnumerable<string> fields)
    {
      List<WorkItem> workItemList = new List<WorkItem>();
      int count = 0;
      int resultMaxLimit = this.resultMaxLimit;
      return (IList<WorkItem>) this.WorkItemServiceHelper.GetWorkItems((IList<int>) ids.Skip<int>(count).Take<int>(resultMaxLimit).ToList<int>(), (IList<string>) fields.ToList<string>(), WorkItemErrorPolicy.Fail).ToList<WorkItem>();
    }

    private List<string> WorkItemFields => new List<string>()
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
