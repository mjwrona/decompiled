// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.MyFavoritePlansViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TestPlanning;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Favorites.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  internal class MyFavoritePlansViewDataProvider : IExtensionDataProvider
  {
    protected Func<IVssRequestContext, ContextIdentifier> GetProject;
    protected Func<TestManagementRequestContext, IWorkItemServiceHelper> GetWorkItemServiceHelper;
    private const string FavoriteScopeType = "Project";
    private const int BatchSize = 200;
    private const string OrOperator = "OR";
    private const string AndOperator = "AND";
    private IWorkItemServiceHelper WorkItemServiceHelper;

    public MyFavoritePlansViewDataProvider()
    {
      this.GetProject = (Func<IVssRequestContext, ContextIdentifier>) (requestContext => WebPageDataProviderUtil.GetPageSource(requestContext).Project);
      this.GetWorkItemServiceHelper = (Func<TestManagementRequestContext, IWorkItemServiceHelper>) (requestContext => requestContext.WorkItemServiceHelper);
    }

    public virtual bool ProjectHasTestPlans(
      IVssRequestContext requestContext,
      TfsTestManagementRequestContext tfsTestManagementRequestContext,
      string projectId)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "MyFavoritePlansViewDataProvider.CheckProjectHasPlans"))
        return new RevisedTestPlansHelper((TestManagementRequestContext) tfsTestManagementRequestContext).UserHasTestPlans(projectId);
    }

    public string Name => "TestManagement.Provider.MyFavoritePlansViewDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      using (requestContext.TraceBlock(1015687, 1015687, "TestManagement", "WebService", this.Name))
      {
        TfsTestManagementRequestContext tfsTestManagementRequestContext = new TfsTestManagementRequestContext(requestContext);
        bool? nullable1 = providerContext?.Properties?.ContainsKey("WorkItemServiceHelper");
        this.WorkItemServiceHelper = (!nullable1.HasValue || !nullable1.Value ? (object) this.GetWorkItemServiceHelper((TestManagementRequestContext) tfsTestManagementRequestContext) : providerContext?.Properties?["WorkItemServiceHelper"]) as IWorkItemServiceHelper;
        bool? nullable2 = providerContext?.Properties?.ContainsKey("Project");
        ContextIdentifier project = (!nullable2.HasValue || !nullable2.Value ? (object) this.GetProject(requestContext) : providerContext?.Properties["Project"]) as ContextIdentifier;
        MyFavoritePlansDataProviderContract data = new MyFavoritePlansDataProviderContract();
        data.HasTestPlans = this.ProjectHasTestPlans(requestContext, tfsTestManagementRequestContext, project.Id.ToString());
        this.TraceInfo(requestContext, "HasTestPlans received as: {0}", (object) data.HasTestPlans.ToString());
        IEnumerable<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite> favoriteTestPlans = MyFavoritePlansViewDataProvider.GetFavoriteTestPlans(requestContext, project);
        IVssRequestContext requestContext1 = requestContext;
        object[] objArray1 = new object[1];
        int num;
        string str1;
        if (favoriteTestPlans == null)
        {
          str1 = "null";
        }
        else
        {
          num = favoriteTestPlans.Count<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite>();
          str1 = num.ToString();
        }
        objArray1[0] = (object) str1;
        this.TraceInfo(requestContext1, "Returned {0} favorite plans", objArray1);
        IList<WorkItem> workItems = this.GetWorkItems(favoriteTestPlans);
        IVssRequestContext requestContext2 = requestContext;
        object[] objArray2 = new object[1];
        string str2;
        if (workItems == null)
        {
          str2 = "null";
        }
        else
        {
          num = workItems.Count<WorkItem>();
          str2 = num.ToString();
        }
        objArray2[0] = (object) str2;
        this.TraceInfo(requestContext2, "Returned {0} work items", objArray2);
        data.AddFavorites(favoriteTestPlans, workItems);
        return (object) data;
      }
    }

    private IList<WorkItem> GetWorkItems(IEnumerable<int> ids, IEnumerable<string> fields)
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

    private static IEnumerable<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite> GetFavoriteTestPlans(
      IVssRequestContext requestContext,
      ContextIdentifier project)
    {
      using (requestContext.TraceBlock(1015687, 1015687, "TestManagement", "WebService", nameof (GetFavoriteTestPlans)))
        return requestContext.GetService<IFavoriteService>().GetFavorites(requestContext, new FavoriteFilter()
        {
          Type = "Microsoft.TeamFoundation.TestManagement.Plan",
          ArtifactScope = new ArtifactScope("Project", project.Id.ToString())
        }, false, (OwnerScope) null).Where<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite>((Func<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite, bool>) (fav => !fav.ArtifactIsDeleted));
    }

    private IList<WorkItem> GetWorkItems(IEnumerable<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite> myFavorites) => this.GetWorkItems(myFavorites.Select<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite, int>((Func<Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite, int>) (fav => int.Parse(fav.ArtifactId))), (IEnumerable<string>) this.WorkItemFields);

    private void TraceInfo(
      IVssRequestContext requestContext,
      string message,
      params object[] parameters)
    {
      VssRequestContextExtensions.Trace(requestContext, 1015687, TraceLevel.Info, "TestManagement", "WebService", "MyFavoritePlansViewDataProvider " + message, parameters);
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
