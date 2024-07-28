// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Controllers.ImageDetails2ApiController
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Services;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Deployment.Controllers
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "Deployment", ResourceName = "imagedetails", ResourceVersion = 2)]
  public class ImageDetails2ApiController : ImageDetailsApiController
  {
    [HttpGet]
    public IEnumerable<string> GetImageResourceIds(int runId) => this.TfsRequestContext.GetService<ImageDetailsService>().GetImageResourceIds(this.TfsRequestContext, this.ProjectId, runId);
  }
}
