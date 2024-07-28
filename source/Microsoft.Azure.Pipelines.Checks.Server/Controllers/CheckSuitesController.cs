// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.Controllers.CheckSuitesController
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Checks.Server.Controllers
{
  [ControllerApiVersion(5.1)]
  [ClientGroupByResource("CheckEvaluations")]
  [VersionedApiControllerCustomName(Area = "PipelinesChecks", ResourceName = "runs")]
  public class CheckSuitesController : PipelineChecksProjectApiController
  {
    public override string ActivityLogArea => "CheckSuiteService";

    [HttpPost]
    [ClientExample("POST__EvaluateCheckSuite.json", null, null, null)]
    public CheckSuite EvaluateCheckSuite(
      [FromBody] CheckSuiteRequest request,
      [FromUri(Name = "$expand")] CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      ArgumentUtility.CheckForNull<CheckSuiteRequest>(request, nameof (request));
      return this.CheckSuiteService.Evaluate(this.TfsRequestContext, this.ProjectId, request.Id, request.Resources, request.Context, expand);
    }

    [HttpPatch]
    [ClientExample("PATCH__UpdateCheckSuiteRun.json", null, null, null)]
    public CheckSuite UpdateCheckSuite(
      Guid checkSuiteId,
      [FromBody] CheckSuiteUpdateParameter request,
      [FromUri(Name = "$expand")] CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      ArgumentUtility.CheckForEmptyGuid(checkSuiteId, nameof (checkSuiteId));
      ArgumentUtility.CheckForNull<CheckSuiteUpdateParameter>(request, nameof (request));
      ArgumentUtility.CheckForDefinedEnum<CheckSuiteUpdateParameter.UpdateAction>(request.Action, "Action");
      ArgumentUtility.CheckForEmptyGuid(request.CheckId, "CheckId");
      if (request.Action == CheckSuiteUpdateParameter.UpdateAction.Rerun)
        return this.CheckSuiteService.RerunCheckRun(this.TfsRequestContext, this.ProjectId, checkSuiteId, request.CheckId);
      if (request.Action == CheckSuiteUpdateParameter.UpdateAction.Bypass)
        return this.CheckSuiteService.BypassCheckRun(this.TfsRequestContext, this.ProjectId, checkSuiteId, request.CheckId, expand);
      throw new InvalidCheckRunUpdateException("Action provided in the request body of check run update is not supported. This endpoint is not yet available.");
    }

    [HttpGet]
    [ClientExample("GET__CheckSuite.json", null, null, null)]
    public CheckSuite GetCheckSuite(Guid checkSuiteId, [FromUri(Name = "$expand")] CheckSuiteExpandParameter expand = CheckSuiteExpandParameter.None)
    {
      ArgumentUtility.CheckForEmptyGuid(checkSuiteId, nameof (checkSuiteId));
      return this.CheckSuiteService.GetCheckSuite(this.TfsRequestContext, this.ProjectId, checkSuiteId, expand);
    }

    private ICheckSuiteService CheckSuiteService => this.TfsRequestContext.GetService<ICheckSuiteService>();
  }
}
