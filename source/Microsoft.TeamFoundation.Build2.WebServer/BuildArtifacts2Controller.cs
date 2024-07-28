// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildArtifacts2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "artifacts", ResourceVersion = 3)]
  public sealed class BuildArtifacts2Controller : BuildArtifactsController
  {
    [HttpPost]
    public override Microsoft.TeamFoundation.Build.WebApi.BuildArtifact CreateArtifact(
      int buildId,
      [FromBody] Microsoft.TeamFoundation.Build.WebApi.BuildArtifact artifact)
    {
      this.CheckRequestContent((object) artifact);
      ArgumentUtility.CheckForNull<ArtifactResource>(artifact.Resource, "artifact.Resource");
      ArgumentUtility.CheckStringForNullOrEmpty(artifact.Resource.Type, "artifact.Resource.Type");
      BuildData buildById = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      if (buildById == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      return this.BuildService.AddArtifact(this.TfsRequestContext, buildById, artifact.ToBuildServerBuildArtifact()).ToWebApiBuildArtifact(this.TfsRequestContext, buildById.ToSecuredObject(), this.ProjectId, buildId, this.GetApiResourceVersion());
    }
  }
}
