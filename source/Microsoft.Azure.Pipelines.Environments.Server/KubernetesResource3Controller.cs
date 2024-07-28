// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.KubernetesResource3Controller
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "environments", ResourceName = "kubernetes", ResourceVersion = 2)]
  public sealed class KubernetesResource3Controller : KubernetesResource2Controller
  {
    [HttpPatch]
    [FeatureEnabled("DistributedTask.Environments")]
    [FeatureEnabled("Environments.EnableEnvironmentOwnedServiceConnections")]
    public KubernetesResource UpdateKubernetesResource(
      int environmentId,
      [FromBody] KubernetesResourcePatchParameters patchRequest)
    {
      return this.TfsRequestContext.GetService<IKubernetesResourceService>().UpdateKubernetesResource(this.TfsRequestContext, this.ProjectId, environmentId, patchRequest);
    }
  }
}
