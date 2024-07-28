// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.Controllers.CheckConfigurationsController
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Checks.Server.Controllers
{
  [ControllerApiVersion(5.1)]
  [ClientGroupByResource("CheckConfigurations")]
  [VersionedApiControllerCustomName(Area = "PipelinesChecks", ResourceName = "configurations")]
  public class CheckConfigurationsController : PipelineChecksProjectApiController
  {
    private ICheckConfigurationService CheckConfigurationService => this.TfsRequestContext.GetService<ICheckConfigurationService>();

    public override string ActivityLogArea => "CheckConfigurationService";

    [HttpPost]
    [ClientExample("POST__AddApprovalCheckConfiguration.json", "Approval", null, null)]
    [ClientExample("POST__AddTaskCheckConfiguration.json", "Task Check", null, null)]
    public CheckConfiguration AddCheckConfiguration(CheckConfiguration configuration)
    {
      if (configuration == null)
        throw new ArgumentNullException(nameof (configuration));
      return this.CheckConfigurationService.AddCheckConfiguration(this.TfsRequestContext, this.ProjectId, configuration);
    }

    [HttpGet]
    [ClientExample("GET__CheckConfiguration.json", null, null, null)]
    public CheckConfiguration GetCheckConfiguration(
      int id,
      [FromUri(Name = "$expand")] CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None)
    {
      return this.CheckConfigurationService.GetCheckConfiguration(this.TfsRequestContext, this.ProjectId, id, expand);
    }

    [HttpGet]
    [ClientExample("GET__CheckConfigurationsOnResource.json", null, null, null)]
    public List<CheckConfiguration> GetCheckConfigurationsOnResource(
      [ClientQueryParameter] string resourceType = null,
      [ClientQueryParameter] string resourceId = null,
      [FromUri(Name = "$expand")] CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None)
    {
      if (resourceType == null)
        throw new ArgumentNullException(nameof (resourceType));
      if (resourceId == null)
        throw new ArgumentNullException(nameof (resourceId));
      List<CheckConfiguration> configurationsOnResource = this.CheckConfigurationService.GetCheckConfigurationsOnResource(this.TfsRequestContext, this.ProjectId, new Resource(resourceType, resourceId), true, expand);
      configurationsOnResource.RemoveAll((Predicate<CheckConfiguration>) (checkConfiguration => checkConfiguration != null && checkConfiguration.Type != null && checkConfiguration.Type.Id == AuthorizationCheckConstants.AuthorizationCheckTypeId));
      return configurationsOnResource;
    }

    [HttpPatch]
    [ClientExample("PATCH__TaskCheckConfiguration.json", null, null, null)]
    public CheckConfiguration UpdateCheckConfiguration(int id, [FromBody] CheckConfiguration configuration) => this.CheckConfigurationService.UpdateCheckConfiguration(this.TfsRequestContext, this.ProjectId, id, configuration);

    [HttpDelete]
    [ClientExample("DELETE__CheckConfiguration.json", null, null, null)]
    public void DeleteCheckConfiguration(int id) => this.CheckConfigurationService.DeleteCheckConfiguration(this.TfsRequestContext, this.ProjectId, id);
  }
}
