// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildTagsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "tags", ResourceVersion = 2)]
  public class BuildTagsController : BuildApiController
  {
    [HttpGet]
    [ClientLocationId("D84AC5C6-EDC7-43D5-ADC9-1B34BE5DEA09")]
    public List<string> GetTags() => this.TfsRequestContext.GetService<ITeamFoundationBuildService2>().GetTags(this.TfsRequestContext, this.ProjectId).ToList<string>();

    [HttpGet]
    [ClientLocationId("6E6114B2-8161-44C8-8F6C-C5505782427F")]
    public List<string> GetBuildTags(int buildId) => (this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId) ?? throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId))).Tags;

    [HttpPost]
    [ClientLocationId("6E6114B2-8161-44C8-8F6C-C5505782427F")]
    public List<string> AddBuildTags(int buildId, IEnumerable<string> tags) => this.BuildService.AddTags(this.TfsRequestContext, this.ProjectId, buildId, tags).ToList<string>();

    [HttpPut]
    [ClientLocationId("6E6114B2-8161-44C8-8F6C-C5505782427F")]
    public List<string> AddBuildTag(int buildId, string tag) => this.BuildService.AddTags(this.TfsRequestContext, this.ProjectId, buildId, (IEnumerable<string>) new string[1]
    {
      tag
    }).ToList<string>();

    [HttpDelete]
    [ClientLocationId("6E6114B2-8161-44C8-8F6C-C5505782427F")]
    public IEnumerable<string> DeleteBuildTag(int buildId, string tag) => this.BuildService.DeleteTags(this.TfsRequestContext, this.ProjectId, buildId, (IEnumerable<string>) new string[1]
    {
      tag
    });
  }
}
