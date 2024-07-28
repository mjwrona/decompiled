// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildArtifactsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.Xaml;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "artifacts", ResourceVersion = 2)]
  public class BuildArtifactsController : BuildCompatApiController
  {
    [HttpGet]
    public virtual List<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> GetArtifacts(
      int buildId,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? definitionType = null)
    {
      return this.GetArtifactsInternal(buildId, definitionType, (string) null).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>();
    }

    [HttpGet]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildArtifact), null, null)]
    [ClientResponseType(typeof (Stream), "GetArtifactContentZip", "application/zip")]
    public virtual HttpResponseMessage GetArtifact(
      int buildId,
      [ClientQueryParameter] string artifactName,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? definitionType = null)
    {
      Microsoft.TeamFoundation.Build.WebApi.BuildArtifact webApiBuildArtifact = this.GetArtifactsInternal(buildId, definitionType, artifactName).FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>();
      if (webApiBuildArtifact == null)
        throw new ArtifactNotFoundException(Resources.ArtifactNotFoundForBuild((object) artifactName, (object) buildId));
      if (MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Json,
        RequestMediaType.Zip
      }).FirstOrDefault<RequestMediaType>() != RequestMediaType.Zip)
        return this.Request.CreateResponse<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>(HttpStatusCode.OK, webApiBuildArtifact);
      if (webApiBuildArtifact.Resource == null)
        throw new MissingRequiredParameterException("artifact.Resource");
      IArtifactProvider artifactProvider;
      if (!this.TfsRequestContext.GetService<IBuildArtifactProviderService>().TryGetArtifactProvider(this.TfsRequestContext, webApiBuildArtifact.Resource.Type, out artifactProvider))
        throw new ArtifactTypeNotSupportedException(Resources.ArtifactTypeNotSupported((object) webApiBuildArtifact.Resource.Type));
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ProjectId", (object) this.ProjectId);
      properties.Add("ArtifactResourceType", webApiBuildArtifact.Resource.Type);
      properties.Add("UserAgent", this.TfsRequestContext.UserAgent ?? string.Empty);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, this.TfsRequestContext.ServiceHost.InstanceId, "Build", "BuildArtifactDownload", properties);
      return artifactProvider.GetDownloadResponse(this.TfsRequestContext, this.Request, this.ProjectId, webApiBuildArtifact.ToBuildServerBuildArtifact(), (ISecuredObject) null);
    }

    [HttpPost]
    public virtual Microsoft.TeamFoundation.Build.WebApi.BuildArtifact CreateArtifact(
      int buildId,
      [FromBody] Microsoft.TeamFoundation.Build.WebApi.BuildArtifact artifact)
    {
      BuildData buildById = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      if (buildById == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      if (artifact != null && artifact.Resource != null && string.IsNullOrEmpty(artifact.Resource.Type))
        artifact.Resource.TryInferType();
      return this.BuildService.AddArtifact(this.TfsRequestContext, this.ProjectId, buildId, artifact.ToBuildServerBuildArtifact()).ToWebApiBuildArtifact(this.TfsRequestContext, buildById.ToSecuredObject(), this.ProjectId, buildId, this.GetApiResourceVersion());
    }

    private IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> GetArtifactsInternal(
      int buildId,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? definitionType,
      string artifactName)
    {
      return this.ExecuteCompatMethod<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>(definitionType, (Func<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>) (() =>
      {
        BuildData build = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
        if (build == null)
          throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
        Guid projectId = this.ProjectId;
        if (projectId == Guid.Empty)
          projectId = build.ProjectId;
        return this.BuildService.GetArtifacts(this.TfsRequestContext, build, artifactName).Select<Microsoft.TeamFoundation.Build2.Server.BuildArtifact, Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>((Func<Microsoft.TeamFoundation.Build2.Server.BuildArtifact, Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) (x => x.ToWebApiBuildArtifact(this.TfsRequestContext, build.ToSecuredObject(), projectId, buildId, this.GetApiResourceVersion())));
      }), (Func<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>>) (() => this.TfsRequestContext.GetService<IXamlBuildProvider>().GetArtifacts(this.TfsRequestContext, this.ProjectInfo, buildId, this.GetApiResourceVersion(), artifactName)));
    }
  }
}
