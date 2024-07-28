// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.OAuthConfigurationController
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "oauthconfiguration")]
  public class OAuthConfigurationController : ServiceEndpointsApiController
  {
    protected OAuthConfigurationController()
    {
    }

    [HttpGet]
    public virtual OAuthConfiguration GetOAuthConfiguration(Guid configurationId) => (OAuthConfiguration) this.TfsRequestContext.GetService<PlatformOAuthConfigurationService>().GetAuthConfiguration(this.TfsRequestContext, configurationId);

    [HttpGet]
    public virtual IList<OAuthConfiguration> GetOAuthConfigurations(
      [FromUri(Name = "endpointType")] string endpointType = null,
      OAuthConfigurationActionFilter actionFilter = OAuthConfigurationActionFilter.None)
    {
      return this.TfsRequestContext.GetService<PlatformOAuthConfigurationService>().GetAuthConfigurations(this.TfsRequestContext, endpointType, actionFilter).ConvertToOAuthConfiguration();
    }

    [HttpPost]
    public virtual OAuthConfiguration CreateOAuthConfiguration(
      OAuthConfigurationParams configurationParams)
    {
      return (OAuthConfiguration) this.TfsRequestContext.GetService<PlatformOAuthConfigurationService>().CreateAuthConfiguration(this.TfsRequestContext, configurationParams);
    }

    [HttpDelete]
    public virtual OAuthConfiguration DeleteOAuthConfiguration(Guid configurationId) => (OAuthConfiguration) this.TfsRequestContext.GetService<PlatformOAuthConfigurationService>().DeleteAuthConfiguration(this.TfsRequestContext, configurationId);

    [HttpPut]
    public virtual OAuthConfiguration UpdateOAuthConfiguration(
      Guid configurationId,
      OAuthConfigurationParams configurationParams)
    {
      return (OAuthConfiguration) this.TfsRequestContext.GetService<PlatformOAuthConfigurationService>().UpdateAuthConfiguration(this.TfsRequestContext, configurationId, configurationParams);
    }
  }
}
