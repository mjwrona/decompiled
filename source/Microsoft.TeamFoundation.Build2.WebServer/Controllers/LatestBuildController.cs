// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.LatestBuildController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "latest", ResourceVersion = 1)]
  public class LatestBuildController : BuildApiController
  {
    internal static readonly Dictionary<Type, HttpStatusCode> s_httpExceptionMapping = new Dictionary<Type, HttpStatusCode>()
    {
      [typeof (AmbiguousDefinitionNameException)] = HttpStatusCode.BadRequest
    };

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public virtual Microsoft.TeamFoundation.Build.WebApi.Build GetLatestBuild(
      string definition,
      string branchName = null)
    {
      ArgumentUtility.CheckForNull<string>(definition, nameof (definition));
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition1 = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definition);
      if (definition1 == null)
        throw new DefinitionNotFoundException(Microsoft.TeamFoundation.Build2.WebServer.Resources.DefinitionNotFound((object) definition));
      if (string.IsNullOrWhiteSpace(branchName))
        branchName = definition1.Repository.DefaultBranch;
      return (this.BuildService.GetLatestBuildForBranch(this.TfsRequestContext, this.ProjectId, definition1.Id, definition1.Repository.Id, definition1.Repository.Type, branchName) ?? throw new BuildNotFoundException(Microsoft.TeamFoundation.Build2.WebServer.Resources.CompletedBuildNotFound((object) definition))).ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) LatestBuildController.s_httpExceptionMapping;
  }
}
