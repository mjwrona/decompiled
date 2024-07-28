// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.DefinitionResourcesController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "resources", ResourceVersion = 1)]
  public class DefinitionResourcesController : BuildApiController
  {
    [HttpGet]
    public List<DefinitionResourceReference> GetDefinitionResources(int definitionId)
    {
      ArgumentUtility.CheckForNonPositiveInt(definitionId, nameof (definitionId));
      return this.TfsRequestContext.GetService<IBuildResourceAuthorizationService>().GetResources(this.TfsRequestContext, this.ProjectId, definitionId).ToDefinitionResourcesList(this.TfsRequestContext, this.ProjectId, (this.TfsRequestContext.GetService<IBuildDefinitionService>().GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId) ?? throw new DefinitionNotFoundException(Microsoft.TeamFoundation.Build2.WebServer.Resources.DefinitionNotFound((object) definitionId))).ToSecuredObject());
    }

    [HttpPatch]
    public List<DefinitionResourceReference> AuthorizeDefinitionResources(
      int definitionId,
      [FromBody] List<DefinitionResourceReference> resources)
    {
      ArgumentUtility.CheckForNonPositiveInt(definitionId, nameof (definitionId));
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = this.TfsRequestContext.GetService<IBuildDefinitionService>().GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId);
      if (definition == null)
        throw new DefinitionNotFoundException(Microsoft.TeamFoundation.Build2.WebServer.Resources.DefinitionNotFound((object) definitionId));
      Microsoft.TeamFoundation.Build2.Server.BuildProcessResources processResources = resources.ToBuildProcessResources();
      return this.TfsRequestContext.GetService<IBuildResourceAuthorizationService>().UpdateResources(this.TfsRequestContext, this.ProjectId, definitionId, processResources).ToDefinitionResourcesList(this.TfsRequestContext, this.ProjectId, definition.ToSecuredObject());
    }
  }
}
