// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.KubernetesResourceController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "kubernetes")]
  public sealed class KubernetesResourceController : DistributedTaskProjectApiController
  {
    [HttpPost]
    public async Task<KubernetesResource> AddKubernetesResource(
      int environmentId,
      [FromBody] KubernetesResourceCreateParameters createParameters)
    {
      KubernetesResourceController resourceController = this;
      return await resourceController.TfsRequestContext.GetService<IKubernetesResourceService>().AddKubernetesResourceAsync(resourceController.TfsRequestContext, resourceController.ProjectId, environmentId, createParameters);
    }

    [HttpGet]
    public KubernetesResource GetKubernetesResource(int environmentId, int resourceId) => this.TfsRequestContext.GetService<IKubernetesResourceService>().GetEnvironmentResource(this.TfsRequestContext, this.ProjectId, environmentId, resourceId);

    [HttpPatch]
    [ClientIgnore]
    public KubernetesResource UpdateKubernetesResource(
      int environmentId,
      [FromBody] KubernetesResource resource)
    {
      resource.EnvironmentReference = new EnvironmentReference()
      {
        Id = environmentId
      };
      return this.TfsRequestContext.GetService<IKubernetesResourceService>().UpdateEnvironmentResource(this.TfsRequestContext, this.ProjectId, resource);
    }

    [HttpDelete]
    public void DeleteKubernetesResource(int environmentId, int resourceId) => this.TfsRequestContext.GetService<IKubernetesResourceService>().DeleteKubernetesResource(this.TfsRequestContext, this.ProjectId, environmentId, resourceId);
  }
}
