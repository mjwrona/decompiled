// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RMTagsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "tags", ResourceVersion = 1)]
  public class RMTagsController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<string>), null, null)]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("86CEE25A-68BA-4BA3-9171-8AD6FFC6DF93")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.ViewReleases)]
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It has to be a method only")]
    public HttpResponseMessage GetTags()
    {
      IEnumerable<string> tags = this.GetService<TagsService>().GetTags(this.TfsRequestContext, this.ProjectId);
      List<ReleaseManagementSecuredString> results = new List<ReleaseManagementSecuredString>();
      foreach (string val in tags)
        results.Add(new ReleaseManagementSecuredString(val));
      this.TfsRequestContext.SetSecuredObjects<ReleaseManagementSecuredString>((IEnumerable<ReleaseManagementSecuredString>) results);
      return this.Request.CreateResponse<List<ReleaseManagementSecuredString>>(HttpStatusCode.OK, results);
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<string>), null, null)]
    [ClientLocationId("C5B602B6-D1B3-4363-8A51-94384F78068F")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases, true)]
    public HttpResponseMessage GetReleaseTags(int releaseId) => this.Request.CreateResponse<IList<string>>(HttpStatusCode.OK, this.GetService<ReleasesService>().GetRelease(this.TfsRequestContext, this.ProjectId, releaseId).Tags);

    [HttpPost]
    [ClientLocationId("C5B602B6-D1B3-4363-8A51-94384F78068F")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ManageReleases)]
    public IEnumerable<string> AddReleaseTags(int releaseId, IEnumerable<string> tags) => this.GetService<TagsService>().AddTags(this.TfsRequestContext, this.ProjectId, releaseId, tags);

    [HttpPatch]
    [ClientLocationId("C5B602B6-D1B3-4363-8A51-94384F78068F")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ManageReleases)]
    public IEnumerable<string> AddReleaseTag(int releaseId, string tag) => this.GetService<TagsService>().AddTags(this.TfsRequestContext, this.ProjectId, releaseId, (IEnumerable<string>) new string[1]
    {
      tag
    });

    [HttpDelete]
    [ClientLocationId("C5B602B6-D1B3-4363-8A51-94384F78068F")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ManageReleases)]
    public IEnumerable<string> DeleteReleaseTag(int releaseId, string tag) => this.GetService<TagsService>().DeleteTags(this.TfsRequestContext, this.ProjectId, releaseId, (IEnumerable<string>) new string[1]
    {
      tag
    });

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "revision")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Optional params")]
    [HttpGet]
    [ClientLocationId("3D21B4C8-C32E-45B2-A7CB-770A369012F4")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public IEnumerable<string> GetDefinitionTags(int releaseDefinitionId) => (IEnumerable<string>) this.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(this.TfsRequestContext, this.ProjectId, releaseDefinitionId).Tags;

    [HttpPost]
    [ClientLocationId("3D21B4C8-C32E-45B2-A7CB-770A369012F4")]
    [ReleaseManagementSecurityPermission("releaseDefinitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    public IEnumerable<string> AddDefinitionTags(int releaseDefinitionId, IEnumerable<string> tags) => this.GetService<TagsService>().AddDefinitionTags(this.TfsRequestContext, this.ProjectId, releaseDefinitionId, tags);

    [HttpPatch]
    [ClientLocationId("3D21B4C8-C32E-45B2-A7CB-770A369012F4")]
    [ReleaseManagementSecurityPermission("releaseDefinitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    public IEnumerable<string> AddDefinitionTag(int releaseDefinitionId, string tag) => this.GetService<TagsService>().AddDefinitionTags(this.TfsRequestContext, this.ProjectId, releaseDefinitionId, (IEnumerable<string>) new List<string>()
    {
      tag
    });

    [HttpDelete]
    [ClientLocationId("3D21B4C8-C32E-45B2-A7CB-770A369012F4")]
    [ReleaseManagementSecurityPermission("releaseDefinitionId", ReleaseManagementSecurityArgumentType.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.EditReleaseDefinition)]
    public IEnumerable<string> DeleteDefinitionTag(int releaseDefinitionId, string tag) => this.GetService<TagsService>().DeleteDefinitionTags(this.TfsRequestContext, this.ProjectId, releaseDefinitionId, (IEnumerable<string>) new List<string>()
    {
      tag
    });
  }
}
