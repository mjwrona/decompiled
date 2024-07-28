// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildSourcesController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "sources", ResourceVersion = 2)]
  public class BuildSourcesController : BuildApiController
  {
    [ClientIgnore]
    [HttpGet]
    public virtual HttpResponseMessage GetSources(int buildId, string sourceVersion = null) => this.GetSources(this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, includeDeleted: true) ?? throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId)).Expected("Build2"), sourceVersion);

    protected HttpResponseMessage GetSources(BuildData build, string sourceVersion)
    {
      if (string.IsNullOrEmpty(sourceVersion))
        sourceVersion = build.SourceVersion;
      if (build.Repository == null)
        throw new MissingRepositoryException(Resources.BuildHasNoRepository((object) build.Id));
      string displayUrl;
      if (!this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, build.Repository.Type).TryGetSourceVersionDisplayUrl(this.TfsRequestContext.Elevate(), build, sourceVersion, out displayUrl))
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.CouldNotRetrieveSourceVersionDisplayUrl((object) build.Id, (object) sourceVersion));
      return new HttpResponseMessage(HttpStatusCode.Found)
      {
        Headers = {
          Location = new Uri(displayUrl, UriKind.Absolute)
        }
      };
    }
  }
}
