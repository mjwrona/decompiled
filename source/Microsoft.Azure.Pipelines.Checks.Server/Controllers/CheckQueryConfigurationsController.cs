// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.Controllers.CheckQueryConfigurationsController
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
  [ControllerApiVersion(5.2)]
  [ClientGroupByResource("CheckConfigurations")]
  [VersionedApiControllerCustomName(Area = "PipelinesChecks", ResourceName = "queryconfigurations")]
  public class CheckQueryConfigurationsController : PipelineChecksProjectApiController
  {
    private ICheckConfigurationService CheckConfigurationService => this.TfsRequestContext.GetService<ICheckConfigurationService>();

    public override string ActivityLogArea => "CheckConfigurationService";

    [HttpPost]
    [ClientExample("POST__QueryCheckConfigurationsOnResources.json", null, null, null)]
    public List<CheckConfiguration> QueryCheckConfigurationsOnResources(
      [FromBody] List<Resource> resources,
      [FromUri(Name = "$expand")] CheckConfigurationExpandParameter expand = CheckConfigurationExpandParameter.None)
    {
      List<CheckConfiguration> configurationsOnResources = this.CheckConfigurationService.GetCheckConfigurationsOnResources(this.TfsRequestContext, this.ProjectId, resources, true, expand);
      configurationsOnResources.RemoveAll((Predicate<CheckConfiguration>) (checkConfiguration => checkConfiguration != null && checkConfiguration.Type != null && checkConfiguration.Type.Id == AuthorizationCheckConstants.AuthorizationCheckTypeId));
      return configurationsOnResources;
    }
  }
}
