// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.PlansApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "plans")]
  public class PlansApiController : ScaledAgileApiControllerBase
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<NoPermissionReadTeamException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<PlanLimitExceededException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<ScaledAgileInvalidPermissionException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<ViewNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<TeamDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ViewPropertiesFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ViewPropertiesMissingException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CardSettingsInvalidFormatException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CardSettingsDuplicateAdditionalFieldsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CardSettingsInvalidFieldIdentifierException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CardSettingsMaxAdditionalFieldsExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ScaledAgileViewDefinitionInvalidException>(HttpStatusCode.BadRequest);
    }

    [HttpGet]
    public Plan GetPlan(string id)
    {
      ArgumentUtility.CheckForNull<string>(id, nameof (id), this.TfsRequestContext.ServiceName);
      this.CheckPlansLicense();
      Guid planId = ParseHelper.GetPlanId(id);
      Plan planWithRestUrl = this.GetPlanWithRestUrl(this.TfsRequestContext.GetService<IScaledAgileViewService>().GetView(this.TfsRequestContext, this.ProjectInfo.Id, planId) ?? throw new ViewNotFoundException());
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
      return planWithRestUrl;
    }

    [HttpGet]
    public IEnumerable<Plan> GetPlans()
    {
      this.CheckPlansLicense();
      IEnumerable<Plan> plans = this.TfsRequestContext.GetService<IScaledAgileViewService>().GetViewDefinitions(this.TfsRequestContext, this.ProjectId).AsEmptyIfNull<Plan>().Select<Plan, Plan>((Func<Plan, Plan>) (x => this.GetPlanWithRestUrl(x)));
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
      return plans;
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeletePlan(string id)
    {
      ArgumentUtility.CheckForNull<string>(id, nameof (id), this.TfsRequestContext.ServiceName);
      this.CheckPlansLicense();
      Guid planId = ParseHelper.GetPlanId(id);
      this.TfsRequestContext.GetService<IScaledAgileViewService>().SoftDeleteView(this.TfsRequestContext, this.ProjectId, planId);
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    [HttpPost]
    [ValidateModel]
    [ClientResponseType(typeof (Plan), null, null)]
    public HttpResponseMessage CreatePlan([FromBody] Microsoft.TeamFoundation.Work.WebApi.CreatePlan postedPlan)
    {
      this.CheckPlansLicense();
      Plan planWithRestUrl = this.GetPlanWithRestUrl(this.TfsRequestContext.GetService<IScaledAgileViewService>().CreateView(this.TfsRequestContext, this.ProjectId, postedPlan));
      HttpResponseMessage response = this.Request.CreateResponse<Plan>(HttpStatusCode.Created, planWithRestUrl);
      response.Headers.Location = new Uri(planWithRestUrl.Url);
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
      return response;
    }

    [HttpPut]
    [ValidateModel]
    [ClientResponseType(typeof (Plan), null, null)]
    public Plan UpdatePlan([FromBody] Microsoft.TeamFoundation.Work.WebApi.UpdatePlan updatedPlan, string id)
    {
      ArgumentUtility.CheckForNull<string>(id, nameof (id), this.TfsRequestContext.ServiceName);
      this.CheckPlansLicense();
      Guid planId = ParseHelper.GetPlanId(id);
      IScaledAgileViewService service = this.TfsRequestContext.GetService<IScaledAgileViewService>();
      service.UpdateView(this.TfsRequestContext, this.ProjectInfo.Id, planId, updatedPlan);
      Plan planWithRestUrl = this.GetPlanWithRestUrl(service.GetView(this.TfsRequestContext, this.ProjectInfo.Id, planId) ?? throw new ViewNotFoundException());
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
      return planWithRestUrl;
    }

    private Plan GetPlanWithRestUrl(Plan plan)
    {
      plan.Url = this.GetPlanRestLink(plan.Id);
      return plan;
    }
  }
}
