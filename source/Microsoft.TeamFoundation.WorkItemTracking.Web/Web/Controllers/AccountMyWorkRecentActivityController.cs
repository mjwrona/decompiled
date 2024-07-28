// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.AccountMyWorkRecentActivityController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "accountMyWorkRecentActivity", ResourceVersion = 1)]
  [ClientInternalUseOnly(true)]
  public class AccountMyWorkRecentActivityController : 
    AccountMyWorkRecentActivityControllerBase<AccountRecentActivityWorkItemModel>
  {
    [HttpGet]
    [ClientLocationId("1BC988F4-C15F-4072-AD35-497C87E3A909")]
    [ClientResponseType(typeof (IList<AccountRecentActivityWorkItemModel>), null, null)]
    public HttpResponseMessage GetRecentActivityData()
    {
      IReadOnlyCollection<AccountRecentActivityWorkItemModel> activityWorkItemModels = (IReadOnlyCollection<AccountRecentActivityWorkItemModel>) null;
      PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(this.TfsRequestContext, "Agile", "AccountMyWorkRecentActivityController.GetRecentActivityData");
      IDictionary<string, PerformanceTimingGroup> allTimings = PerformanceTimer.GetAllTimings(this.TfsRequestContext);
      performanceScenarioHelper.Add("WebServerTimings", (object) allTimings);
      try
      {
        activityWorkItemModels = (IReadOnlyCollection<AccountRecentActivityWorkItemModel>) this.GetMyRecentWorkActivities();
      }
      finally
      {
        performanceScenarioHelper.EndScenario();
      }
      return this.Request.CreateResponse<IReadOnlyCollection<AccountRecentActivityWorkItemModel>>(HttpStatusCode.OK, activityWorkItemModels);
    }

    protected override IEnumerable<AccountRecentActivityWorkItemModel> CreateRecentActivityPayload(
      IEnumerable<WorkItemFieldData> fieldValues,
      IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity> recentActivities)
    {
      return (IEnumerable<AccountRecentActivityWorkItemModel>) fieldValues.Select<WorkItemFieldData, AccountRecentActivityWorkItemModel>((Func<WorkItemFieldData, AccountRecentActivityWorkItemModel>) (fieldData =>
      {
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemRecentActivityType result = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemRecentActivityType.Visited;
        string key = fieldData.Id.ToString();
        if (!System.Enum.TryParse<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemRecentActivityType>(recentActivities[key].ActivityDetails, out result))
          result = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemRecentActivityType.Visited;
        return new AccountRecentActivityWorkItemModel()
        {
          AssignedTo = fieldData.AssignedTo,
          Id = fieldData.Id,
          ChangedDate = fieldData.ModifiedDate,
          State = fieldData.State,
          Title = fieldData.Title,
          WorkItemType = fieldData.WorkItemType,
          TeamProject = fieldData.GetProjectName(this.TfsRequestContext),
          ActivityDate = recentActivities[key].ActivityDate,
          ActivityType = result
        };
      })).OrderByDescending<AccountRecentActivityWorkItemModel, DateTime>((Func<AccountRecentActivityWorkItemModel, DateTime>) (item => item.ChangedDate));
    }
  }
}
