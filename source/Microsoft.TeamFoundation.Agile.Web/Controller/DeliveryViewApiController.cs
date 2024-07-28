// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.DeliveryViewApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "deliverytimeline")]
  public class DeliveryViewApiController : ScaledAgileApiControllerBase
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<NoPermissionReadTeamException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<ViewNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TeamDoesNotExistException>(HttpStatusCode.NotFound);
    }

    [HttpGet]
    [TraceFilter(103201, 103210)]
    public DeliveryViewData GetDeliveryTimelineData(
      string id,
      int? revision = null,
      DateTime? startDate = null,
      DateTime? endDate = null)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(this.ProjectInfo, "ProjectInfo", this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<WebApiTeam>(this.Team, "Team", this.TfsRequestContext.ServiceName);
      this.CheckPlansLicense();
      Guid planId = ParseHelper.GetPlanId(id);
      IScaledAgileViewService service = this.TfsRequestContext.GetService<IScaledAgileViewService>();
      Plan view = service.GetView(this.TfsRequestContext, this.ProjectId, planId);
      if (view == null)
        throw new ViewNotFoundException();
      if (revision.HasValue && revision.Value != view.Revision)
        throw new ViewRevisionMismatchException();
      IDictionary<string, object> filterProperties = this.CreateFilterProperties(startDate, endDate);
      PlanViewFilter planFilter = PlanFilterHelper.CreatePlanFilter(view, filterProperties);
      DeliveryViewData viewData = service.GetViewData(this.TfsRequestContext, planId, PlanType.DeliveryTimelineView, planFilter) as DeliveryViewData;
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
      return viewData;
    }

    internal IDictionary<string, object> CreateFilterProperties(
      DateTime? startDate,
      DateTime? endDate)
    {
      Dictionary<string, object> filterProperties = new Dictionary<string, object>();
      DateTime dateTime;
      if (startDate.HasValue)
      {
        Dictionary<string, object> dictionary = filterProperties;
        dateTime = startDate.Value;
        // ISSUE: variable of a boxed type
        __Boxed<DateTime> universalTime = (ValueType) dateTime.ToUniversalTime();
        dictionary.Add("StartDate", (object) universalTime);
      }
      if (endDate.HasValue)
      {
        Dictionary<string, object> dictionary = filterProperties;
        dateTime = endDate.Value;
        // ISSUE: variable of a boxed type
        __Boxed<DateTime> universalTime = (ValueType) dateTime.ToUniversalTime();
        dictionary.Add("EndDate", (object) universalTime);
      }
      return (IDictionary<string, object>) filterProperties;
    }
  }
}
