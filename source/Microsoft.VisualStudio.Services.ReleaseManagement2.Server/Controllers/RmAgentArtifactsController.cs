// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmAgentArtifactsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "ReleaseManagement", ResourceName = "agentartifacts")]
  public class RmAgentArtifactsController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [ClientLocationId("D843590D-370D-47EF-97F5-BEA3CEFF021F")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    public virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentArtifactDefinition> GetAgentArtifactDefinitions(
      int releaseId)
    {
      return this.GetArtifactDefinitionsForRelease(releaseId);
    }

    [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Boolean.TryParse(System.String,System.Boolean@)", Justification = "By design")]
    protected IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentArtifactDefinition> GetArtifactDefinitionsForRelease(
      int releaseId)
    {
      return this.ToAgentArtifactsController(this.TfsRequestContext.GetService<AgentArtifactsService>().GetAgentArtifacts(this.TfsRequestContext, this.ProjectId, releaseId, true));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By design.")]
    private IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentArtifactDefinition> ToAgentArtifactsController(
      IEnumerable<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition> agentArtifacts)
    {
      return agentArtifacts == null ? (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentArtifactDefinition>) null : agentArtifacts.Select<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentArtifactDefinition>((Func<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentArtifactDefinition>) (x => new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentArtifactDefinition()
      {
        Alias = x.Alias,
        ArtifactType = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.AgentArtifactType) Enum.ToObject(typeof (Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactType), (int) x.ArtifactType),
        Details = x.Details,
        Name = x.Name,
        Version = x.Version
      }));
    }
  }
}
