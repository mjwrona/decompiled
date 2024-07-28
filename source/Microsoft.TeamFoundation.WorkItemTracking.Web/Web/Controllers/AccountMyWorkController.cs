// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.AccountMyWorkController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "accountMyWork", ResourceVersion = 1)]
  [ClientInternalUseOnly(true)]
  public class AccountMyWorkController : TfsApiController
  {
    [HttpGet]
    [ClientLocationId("DEF3D688-DDF5-4096-9024-69BEEA15CDBD")]
    [ClientResponseType(typeof (AccountMyWorkResult), null, null)]
    public HttpResponseMessage GetAccountMyWorkData([FromUri(Name = "$queryOption")] QueryOption queryOption = QueryOption.Doing)
    {
      PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(this.TfsRequestContext, "Agile", "AccountMyWorkController.GetAccountMyWorkData");
      IDictionary<string, PerformanceTimingGroup> allTimings = PerformanceTimer.GetAllTimings(this.TfsRequestContext);
      performanceScenarioHelper.Add("WebServerTimings", (object) allTimings);
      MyWorkService service = this.TfsRequestContext.GetService<MyWorkService>();
      IList<AccountWorkWorkItemModel> workWorkItemModelList;
      try
      {
        switch (queryOption)
        {
          case QueryOption.Doing:
            workWorkItemModelList = service.GetDoingWorkItemsAssignedToMe(this.TfsRequestContext);
            break;
          case QueryOption.Done:
            workWorkItemModelList = service.GetDoneWorkItemsAssignedToMe(this.TfsRequestContext);
            break;
          case QueryOption.Followed:
            workWorkItemModelList = service.GetWorkItemsFollowedByMe(this.TfsRequestContext);
            break;
          default:
            return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Not implemented");
        }
      }
      finally
      {
        performanceScenarioHelper.EndScenario();
      }
      return this.Request.CreateResponse<AccountMyWorkResult>(HttpStatusCode.OK, new AccountMyWorkResult()
      {
        WorkItemDetails = workWorkItemModelList,
        QuerySizeLimitExceeded = workWorkItemModelList.Count == service.GetMaxCount(this.TfsRequestContext, queryOption == QueryOption.Followed)
      });
    }

    public override string ActivityLogArea => "WorkItem Tracking";
  }
}
