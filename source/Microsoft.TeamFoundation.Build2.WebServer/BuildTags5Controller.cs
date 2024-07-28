// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildTags5Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "tags", ResourceVersion = 2)]
  public class BuildTags5Controller : BuildApiController
  {
    [HttpGet]
    [ClientLocationId("D84AC5C6-EDC7-43D5-ADC9-1B34BE5DEA09")]
    [ClientResponseType(typeof (List<string>), null, null)]
    [PublicProjectRequestRestrictions]
    public SecuredTagList GetTags() => new SecuredTagList(this.TfsRequestContext.GetService<ITeamFoundationBuildService2>().GetTags(this.TfsRequestContext, this.ProjectId), (ISecuredObject) this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext));

    [HttpGet]
    [ClientLocationId("6E6114B2-8161-44C8-8F6C-C5505782427F")]
    [ClientResponseType(typeof (List<string>), null, null)]
    [PublicProjectRequestRestrictions]
    public SecuredTagList GetBuildTags(int buildId)
    {
      BuildData buildById = this.BuildService.GetBuildById(this.TfsRequestContext, this.ProjectId, buildId);
      return buildById != null ? new SecuredTagList((IEnumerable<string>) buildById.Tags, buildById.ToSecuredObject()) : throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
    }

    [HttpPost]
    [ClientLocationId("6E6114B2-8161-44C8-8F6C-C5505782427F")]
    [ClientResponseType(typeof (List<string>), null, null)]
    [ClientExample("POST_build_addBuildTag.json", null, null, null)]
    public SecuredTagList AddBuildTags(int buildId, IEnumerable<string> tags) => this.AddBuildTagsInternal(buildId, tags);

    [HttpPut]
    [ClientLocationId("6E6114B2-8161-44C8-8F6C-C5505782427F")]
    [ClientResponseType(typeof (List<string>), null, null)]
    public SecuredTagList AddBuildTag(int buildId, string tag) => this.AddBuildTagsInternal(buildId, (IEnumerable<string>) new string[1]
    {
      tag
    });

    [HttpDelete]
    [ClientLocationId("6E6114B2-8161-44C8-8F6C-C5505782427F")]
    [ClientResponseType(typeof (List<string>), null, null)]
    public SecuredTagList DeleteBuildTag(int buildId, string tag)
    {
      ArgumentUtility.CheckForNonPositiveInt(buildId, nameof (buildId));
      BuildData buildById = this.BuildService.GetBuildById(this.TfsRequestContext, this.ProjectId, buildId);
      if (buildById == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      return new SecuredTagList(this.BuildService.DeleteTags(this.TfsRequestContext, buildById, (IEnumerable<string>) new string[1]
      {
        tag
      }), buildById.ToSecuredObject());
    }

    [HttpGet]
    [ClientLocationId("CB894432-134A-4D31-A839-83BECEAACE4B")]
    [ClientResponseType(typeof (List<string>), null, null)]
    [PublicProjectRequestRestrictions]
    public SecuredTagList GetDefinitionTags(int definitionId, int? revision = null)
    {
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId, revision);
      return definition != null ? new SecuredTagList((IEnumerable<string>) definition.Tags, definition.ToSecuredObject()) : throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) definitionId));
    }

    [HttpPost]
    [ClientLocationId("CB894432-134A-4D31-A839-83BECEAACE4B")]
    [ClientResponseType(typeof (List<string>), null, null)]
    public SecuredTagList AddDefinitionTags(int definitionId, IEnumerable<string> tags) => this.AddDefinitionTagsInternal(definitionId, tags);

    [HttpPut]
    [ClientLocationId("CB894432-134A-4D31-A839-83BECEAACE4B")]
    [ClientResponseType(typeof (List<string>), null, null)]
    public SecuredTagList AddDefinitionTag(int definitionId, string tag) => this.AddDefinitionTagsInternal(definitionId, (IEnumerable<string>) new string[1]
    {
      tag
    });

    [HttpDelete]
    [ClientLocationId("CB894432-134A-4D31-A839-83BECEAACE4B")]
    [ClientResponseType(typeof (List<string>), null, null)]
    public SecuredTagList DeleteDefinitionTag(int definitionId, string tag)
    {
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId);
      if (definition == null)
        throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) definitionId));
      return new SecuredTagList(this.DefinitionService.DeleteTags(this.TfsRequestContext, definition, (IEnumerable<string>) new string[1]
      {
        tag
      }), definition.ToSecuredObject());
    }

    [HttpDelete]
    [ClientLocationId("D84AC5C6-EDC7-43D5-ADC9-1B34BE5DEA09")]
    [ClientResponseType(typeof (List<string>), null, null)]
    public SecuredTagList DeleteTag(string tag) => new SecuredTagList(this.TagService.DeleteTags(this.TfsRequestContext, this.ProjectId, (IEnumerable<string>) new string[1]
    {
      tag
    }), (ISecuredObject) this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext));

    private SecuredTagList AddBuildTagsInternal(int buildId, IEnumerable<string> tags)
    {
      BuildData buildById = this.BuildService.GetBuildById(this.TfsRequestContext, this.ProjectId, buildId);
      if (buildById == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      return new SecuredTagList(this.BuildService.AddTags(this.TfsRequestContext, buildById, tags), buildById.ToSecuredObject());
    }

    private SecuredTagList AddDefinitionTagsInternal(int definitionId, IEnumerable<string> tags)
    {
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId);
      if (definition == null)
        throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) definitionId));
      return new SecuredTagList(this.DefinitionService.AddTags(this.TfsRequestContext, definition, tags), definition.ToSecuredObject());
    }
  }
}
