// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.KubernetesResource2Controller
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "environments", ResourceName = "kubernetes")]
  public class KubernetesResource2Controller : EnvironmentsProjectApiController
  {
    [HttpPost]
    [FeatureEnabled("DistributedTask.Environments")]
    public async Task<KubernetesResource> AddKubernetesResource(
      int environmentId,
      [FromBody] KubernetesResourceCreateParametersExistingEndpoint createParameters)
    {
      KubernetesResource2Controller resource2Controller = this;
      return await resource2Controller.TfsRequestContext.GetService<IKubernetesResourceService>().AddKubernetesResourceAsync(resource2Controller.TfsRequestContext, resource2Controller.ProjectId, environmentId, (KubernetesResourceCreateParameters) createParameters);
    }

    [HttpGet]
    [FeatureEnabled("DistributedTask.Environments")]
    public KubernetesResource GetKubernetesResource(int environmentId, int resourceId) => this.TfsRequestContext.GetService<IKubernetesResourceService>().GetEnvironmentResource(this.TfsRequestContext, this.ProjectId, environmentId, resourceId);

    [HttpDelete]
    [FeatureEnabled("DistributedTask.Environments")]
    public void DeleteKubernetesResource(int environmentId, int resourceId) => this.TfsRequestContext.GetService<IKubernetesResourceService>().DeleteKubernetesResource(this.TfsRequestContext, this.ProjectId, environmentId, resourceId);
  }
}
