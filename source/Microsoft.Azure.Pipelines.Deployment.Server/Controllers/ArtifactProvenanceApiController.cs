// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Controllers.ArtifactProvenanceApiController
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Deployment.Controllers
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "Deployment", ResourceName = "artifactprovenances")]
  public class ArtifactProvenanceApiController : DeploymentProjectApiController
  {
    [HttpGet]
    [ClientLocationId("D943A6F4-A813-4498-823A-4DA53BF9D0CD")]
    public IEnumerable<ArtifactProvenance> GetArtifactProvenances(
      [ClientParameterAsIEnumerable(typeof (string), ',')] string resourceUri,
      string typeFilters = null)
    {
      this.CheckPermission(this.TfsRequestContext);
      return this.TfsRequestContext.GetService<ArtifactProvenanceService>().GetArtifactProvenances(this.TfsRequestContext, resourceUri, typeFilters);
    }

    private void CheckPermission(IVssRequestContext requestContext)
    {
      IdentityDescriptor userContext = requestContext.UserContext;
      IdentityService service = requestContext.GetService<IdentityService>();
      if ((ServicePrincipals.IsServicePrincipal(requestContext, userContext) ? 1 : (service.IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, userContext) ? 1 : 0)) == 0)
        throw new UnauthorizedAccessException();
    }
  }
}
