// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.AccountMyWorkRecentActivity2Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
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
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "accountMyWorkRecentActivity", ResourceVersion = 2)]
  [ControllerApiVersion(5.0)]
  public class AccountMyWorkRecentActivity2Controller : 
    AccountMyWorkRecentActivityControllerBase<AccountRecentActivityWorkItemModel2>
  {
    [HttpGet]
    [ClientLocationId("1BC988F4-C15F-4072-AD35-497C87E3A909")]
    [ClientResponseType(typeof (IList<AccountRecentActivityWorkItemModel2>), null, null)]
    public HttpResponseMessage GetRecentActivityData()
    {
      IReadOnlyCollection<AccountRecentActivityWorkItemModel2> activityWorkItemModel2s = (IReadOnlyCollection<AccountRecentActivityWorkItemModel2>) null;
      PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(this.TfsRequestContext, "Agile", "AccountMyWorkController.GetRecentActivityData");
      IDictionary<string, PerformanceTimingGroup> allTimings = PerformanceTimer.GetAllTimings(this.TfsRequestContext);
      performanceScenarioHelper.Add("WebServerTimings", (object) allTimings);
      try
      {
        activityWorkItemModel2s = (IReadOnlyCollection<AccountRecentActivityWorkItemModel2>) this.GetMyRecentWorkActivities();
      }
      finally
      {
        performanceScenarioHelper.EndScenario();
      }
      return this.Request.CreateResponse<IReadOnlyCollection<AccountRecentActivityWorkItemModel2>>(HttpStatusCode.OK, activityWorkItemModel2s);
    }

    protected override IEnumerable<AccountRecentActivityWorkItemModel2> CreateRecentActivityPayload(
      IEnumerable<WorkItemFieldData> fieldValues,
      IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity> recentActivities)
    {
      return (IEnumerable<AccountRecentActivityWorkItemModel2>) fieldValues.Select<WorkItemFieldData, AccountRecentActivityWorkItemModel2>((Func<WorkItemFieldData, AccountRecentActivityWorkItemModel2>) (fieldData =>
      {
        Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemRecentActivityType result = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemRecentActivityType.Visited;
        string key = fieldData.Id.ToString();
        if (!System.Enum.TryParse<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemRecentActivityType>(recentActivities[key].ActivityDetails, out result))
          result = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemRecentActivityType.Visited;
        return new AccountRecentActivityWorkItemModel2()
        {
          AssignedTo = string.IsNullOrEmpty(fieldData.AssignedTo) ? (IdentityRef) null : (IdentityRef) IdentityReferenceBuilder.CreateFromWitIdentityName(this.TfsRequestContext.WitContext(), fieldData.AssignedTo),
          Id = fieldData.Id,
          ChangedDate = fieldData.ModifiedDate,
          State = fieldData.State,
          Title = fieldData.Title,
          WorkItemType = fieldData.WorkItemType,
          TeamProject = fieldData.GetProjectName(this.TfsRequestContext),
          ActivityDate = recentActivities[key].ActivityDate,
          ActivityType = result
        };
      })).OrderByDescending<AccountRecentActivityWorkItemModel2, DateTime>((Func<AccountRecentActivityWorkItemModel2, DateTime>) (item => item.ChangedDate));
    }
  }
}
