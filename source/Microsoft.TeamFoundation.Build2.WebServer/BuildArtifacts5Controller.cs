// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildArtifacts5Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "artifacts", ResourceVersion = 3)]
  public class BuildArtifacts5Controller : BuildArtifacts4Controller
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DedupExceptionMapping.ServerErrorMap;

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetFile", MediaType = "application/octet-stream")]
    [PublicProjectRequestRestrictions]
    public virtual HttpResponseMessage GetFile(
      int buildId,
      [ClientQueryParameter] string artifactName,
      [ClientQueryParameter] string fileId,
      [ClientQueryParameter] string fileName)
    {
      if (this.Request.Headers?.Range != null)
        throw new ArgumentException(Resources.RangeHeaderNotAllowed());
      UrlValidationResult urlValidationResult = this.TfsRequestContext.GetService<IUrlSigningService>().Validate(this.TfsRequestContext, this.TfsRequestContext.RequestUri());
      BuildData buildById = this.BuildService.GetBuildById(this.TfsRequestContext, this.ProjectId, buildId);
      if (buildById == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      Microsoft.TeamFoundation.Build2.Server.BuildArtifact artifact = this.BuildService.GetArtifacts(this.TfsRequestContext, buildById, artifactName).FirstOrDefault<Microsoft.TeamFoundation.Build2.Server.BuildArtifact>();
      if (artifact == null)
        throw new ArtifactNotFoundException(Resources.ArtifactNotFoundForBuild((object) artifactName, (object) buildId));
      IArtifactProvider artifactProvider;
      if (!this.TfsRequestContext.GetService<IBuildArtifactProviderService>().TryGetArtifactProvider(this.TfsRequestContext, artifact.Resource.Type, out artifactProvider))
        throw new ArtifactTypeNotSupportedException(Resources.ArtifactTypeNotSupported((object) "Container"));
      if (!artifactProvider.ValidateFileIdentifier(this.TfsRequestContext, fileId, artifact))
        throw new ArgumentException(Resources.ArtifactInvalidFileId((object) fileId, (object) artifactName));
      return artifactProvider.GetFileDownloadResponse(this.TfsRequestContext, fileId, fileName, artifact, urlValidationResult, buildById.ToSecuredObject());
    }
  }
}
