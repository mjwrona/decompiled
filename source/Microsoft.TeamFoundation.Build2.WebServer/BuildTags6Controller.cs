// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildTags6Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "tags", ResourceVersion = 3)]
  public class BuildTags6Controller : BuildTags5Controller
  {
    [HttpPatch]
    [ClientLocationId("6E6114B2-8161-44C8-8F6C-C5505782427F")]
    [ClientResponseType(typeof (List<string>), null, null)]
    public SecuredTagList UpdateBuildTags(int buildId, UpdateTagParameters updateParameters)
    {
      ArgumentUtility.CheckForNonPositiveInt(buildId, nameof (buildId));
      ArgumentUtility.CheckForNull<UpdateTagParameters>(updateParameters, nameof (updateParameters));
      BuildData buildById = this.BuildService.GetBuildById(this.TfsRequestContext, this.ProjectId, buildId);
      IEnumerable<string> tags = buildById != null ? (IEnumerable<string>) buildById.Tags : throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      if (updateParameters.tagsToRemove != null && updateParameters.tagsToRemove.Count > 0)
        tags = this.BuildService.DeleteTags(this.TfsRequestContext, buildById, (IEnumerable<string>) updateParameters.tagsToRemove);
      if (updateParameters.tagsToAdd != null && updateParameters.tagsToAdd.Count > 0)
        tags = this.BuildService.AddTags(this.TfsRequestContext, buildById, (IEnumerable<string>) updateParameters.tagsToAdd);
      return new SecuredTagList(tags, buildById.ToSecuredObject());
    }

    [HttpPatch]
    [ClientLocationId("CB894432-134A-4D31-A839-83BECEAACE4B")]
    [ClientResponseType(typeof (List<string>), null, null)]
    public SecuredTagList UpdateDefinitionTags(
      int definitionId,
      UpdateTagParameters updateParameters)
    {
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId);
      if (definition == null)
        throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) definitionId));
      ArgumentUtility.CheckForNull<UpdateTagParameters>(updateParameters, nameof (updateParameters));
      IEnumerable<string> tags = (IEnumerable<string>) definition.Tags;
      if (updateParameters.tagsToRemove != null && updateParameters.tagsToRemove.Count > 0)
        tags = this.DefinitionService.DeleteTags(this.TfsRequestContext, definition, (IEnumerable<string>) updateParameters.tagsToRemove);
      if (updateParameters.tagsToAdd != null && updateParameters.tagsToAdd.Count > 0)
        tags = this.DefinitionService.AddTags(this.TfsRequestContext, definition, (IEnumerable<string>) updateParameters.tagsToAdd);
      return new SecuredTagList(tags, definition.ToSecuredObject());
    }
  }
}
