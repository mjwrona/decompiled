// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Controllers.ImageDetailsApiController
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Deployment.Controllers
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "Deployment", ResourceName = "imagedetails")]
  public class ImageDetailsApiController : DeploymentProjectApiController
  {
    [HttpPost]
    public void AddImageDetails(ImageDetails imageDetails) => this.TfsRequestContext.GetService<ImageDetailsService>().AddImageDetails(this.TfsRequestContext, this.ProjectId, imageDetails);
  }
}
