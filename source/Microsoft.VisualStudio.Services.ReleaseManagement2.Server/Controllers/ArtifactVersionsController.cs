// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ArtifactVersionsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "versions")]
  public class ArtifactVersionsController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [ReleaseManagementSecurityPermission("releaseDefinitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ViewReleaseDefinition)]
    public ArtifactVersionQueryResult GetArtifactVersions(int releaseDefinitionId) => this.TfsRequestContext.GetService<ArtifactVersionsService>().GetArtifactVersions(this.TfsRequestContext, this.ProjectInfo, releaseDefinitionId);

    [HttpPost]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public ArtifactVersionQueryResult GetArtifactVersionsForSources([FromBody] IList<Artifact> artifacts)
    {
      if (artifacts == null)
        return new ArtifactVersionQueryResult()
        {
          ArtifactVersions = (IList<ArtifactVersion>) new List<ArtifactVersion>()
        };
      List<ArtifactSource> list = artifacts.Select<Artifact, ArtifactSource>((Func<Artifact, ArtifactSource>) (artifact => artifact.ToServerSource(this.TfsRequestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion")))).ToList<ArtifactSource>();
      return this.TfsRequestContext.GetService<ArtifactVersionsService>().GetArtifactVersions(this.TfsRequestContext, this.ProjectInfo, (IList<ArtifactSource>) list);
    }
  }
}
