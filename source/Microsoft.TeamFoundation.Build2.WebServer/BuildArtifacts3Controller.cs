// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildArtifacts3Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
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
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "artifacts", ResourceVersion = 3)]
  public class BuildArtifacts3Controller : BuildApiController
  {
    [HttpGet]
    public virtual List<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> GetArtifacts(
      int buildId)
    {
      BuildData build = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      if (build == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      return this.BuildService.GetArtifacts(this.TfsRequestContext, build).Select<Microsoft.TeamFoundation.Build2.Server.BuildArtifact, Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>((Func<Microsoft.TeamFoundation.Build2.Server.BuildArtifact, Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) (x => x.ToWebApiBuildArtifact(this.TfsRequestContext, build.ToSecuredObject(), this.ProjectId, buildId, this.GetApiResourceVersion()))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>();
    }

    [HttpGet]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildArtifact), null, null)]
    [ClientResponseType(typeof (Stream), "GetArtifactContentZip", "application/zip")]
    public virtual HttpResponseMessage GetArtifact(int buildId, [ClientQueryParameter] string artifactName)
    {
      BuildData buildById = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      if (buildById == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      Microsoft.TeamFoundation.Build2.Server.BuildArtifact buildArtifact = this.BuildService.GetArtifacts(this.TfsRequestContext, buildById, artifactName).FirstOrDefault<Microsoft.TeamFoundation.Build2.Server.BuildArtifact>();
      if (buildArtifact == null)
        throw new ArtifactNotFoundException(Resources.ArtifactNotFoundForBuild((object) artifactName, (object) buildId));
      Guid projectId = this.ProjectId;
      if (projectId == Guid.Empty)
        projectId = buildById.ProjectId;
      if (MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Json,
        RequestMediaType.Zip
      }).FirstOrDefault<RequestMediaType>() != RequestMediaType.Zip)
        return this.Request.CreateResponse<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>(HttpStatusCode.OK, buildArtifact.ToWebApiBuildArtifact(this.TfsRequestContext, buildById.ToSecuredObject(), projectId, buildId, this.GetApiResourceVersion()));
      if (buildArtifact.Resource == null)
        throw new MissingRequiredParameterException("artifact.Resource");
      IArtifactProvider artifactProvider;
      if (!this.TfsRequestContext.GetService<IBuildArtifactProviderService>().TryGetArtifactProvider(this.TfsRequestContext, buildArtifact.Resource.Type, out artifactProvider))
        throw new ArtifactTypeNotSupportedException(Resources.ArtifactTypeNotSupported((object) buildArtifact.Resource.Type));
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ProjectId", (object) projectId);
      properties.Add("ArtifactResourceType", buildArtifact.Resource.Type);
      properties.Add("UserAgent", this.TfsRequestContext.UserAgent ?? string.Empty);
      this.TfsRequestContext.GetService<CustomerIntelligenceService>().Publish(this.TfsRequestContext, this.TfsRequestContext.ServiceHost.InstanceId, "Build", "BuildArtifactDownload", properties);
      return artifactProvider.GetDownloadResponse(this.TfsRequestContext, this.Request, projectId, buildArtifact, buildById.ToSecuredObject());
    }

    [HttpPost]
    public virtual Microsoft.TeamFoundation.Build.WebApi.BuildArtifact CreateArtifact(
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
