// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.BuildProjectAuthorizedResourcesController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "authorizedresources", ResourceVersion = 1)]
  public class BuildProjectAuthorizedResourcesController : BuildApiController
  {
    [HttpGet]
    public List<DefinitionResourceReference> GetProjectResources(string type = null, string id = null)
    {
      ResourceType? resourceType = new ResourceType?();
      if (!string.IsNullOrEmpty(type))
      {
        ResourceType result;
        if (!ResourceTypeNames.TryParse(type, out result))
          throw new ArgumentOutOfRangeException(nameof (type), (object) type, Microsoft.TeamFoundation.Build2.WebServer.Resources.InvalidResourceTypeName((object) type)).Expected("Build2");
        resourceType = new ResourceType?(result);
      }
      return this.TfsRequestContext.GetService<IBuildResourceAuthorizationService>().GetAuthorizedResources(this.TfsRequestContext, this.ProjectId, resourceType, id).ToDefinitionResourcesList(this.TfsRequestContext, this.ProjectId, (ISecuredObject) this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext));
    }

    [HttpPatch]
    public List<DefinitionResourceReference> AuthorizeProjectResources(
      [FromBody] List<DefinitionResourceReference> resources)
    {
      Microsoft.TeamFoundation.Build2.Server.BuildProcessResources processResources = resources.ToBuildProcessResources();
      return this.TfsRequestContext.GetService<IBuildResourceAuthorizationService>().UpdateResources(this.TfsRequestContext, this.ProjectId, processResources).ToDefinitionResourcesList(this.TfsRequestContext, this.ProjectId, (ISecuredObject) this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext));
    }
  }
}
