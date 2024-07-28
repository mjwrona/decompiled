// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildWorkItemsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Build2.Xaml;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "workitems", ResourceVersion = 2)]
  public class BuildWorkItemsController : BuildCompatApiController
  {
    private const int DefaultMaxItems = 50;

    [HttpGet]
    [ClientLocationId("5A21F5D2-5642-47E4-A0BD-1356E6731BEE")]
    public List<ResourceRef> GetBuildWorkItemsRefs(int buildId, [FromUri(Name = "$top")] int top = 50, Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null) => this.ExecuteCompatMethod<List<ResourceRef>>(type, (Func<List<ResourceRef>>) (() => this.InternalBuildService.GetBuildWorkItemRefs(this.TfsRequestContext, buildId, (IEnumerable<string>) null, top).ToList<ResourceRef>()), (Func<List<ResourceRef>>) (() => this.TfsRequestContext.GetService<IXamlBuildProvider>().GetBuildWorkItemRefs(this.TfsRequestContext, this.ProjectInfo, buildId, top).ToList<ResourceRef>()));

    [HttpGet]
    public List<ResourceRef> GetWorkItemsBetweenBuilds(int? fromBuildId, int? toBuildId, [FromUri(Name = "$top")] int top = 50)
    {
      int? nullable1 = fromBuildId;
      int num1 = 0;
      if (nullable1.GetValueOrDefault() > num1 & nullable1.HasValue)
      {
        int? nullable2 = toBuildId;
        int num2 = 0;
        if (nullable2.GetValueOrDefault() > num2 & nullable2.HasValue)
          return this.InternalBuildService.GetWorkItemsBetweenBuilds(this.TfsRequestContext, fromBuildId.Value, toBuildId.Value, (IEnumerable<string>) null, top).ToList<ResourceRef>();
      }
      throw new InvalidBuildQueryException(Resources.InvalidInputForGetChangesAndWorkItemsBetweenBuilds());
    }

    [HttpPost]
    [ClientLocationId("5A21F5D2-5642-47E4-A0BD-1356E6731BEE")]
    public List<ResourceRef> GetBuildWorkItemsRefsFromCommits(
      int buildId,
      IEnumerable<string> commitIds,
      [FromUri(Name = "$top")] int top = 50,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null)
    {
      BuildData build = (BuildData) null;
      Microsoft.TeamFoundation.Build.WebApi.Build webApiBuild = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      if (((int) type ?? 2) == 2)
        build = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId);
      if (build != null)
        return this.InternalBuildService.GetBuildWorkItemRefs(this.TfsRequestContext, (IReadOnlyBuildData) build, commitIds, top).ToList<ResourceRef>();
      if (build == null && ((int) type ?? 1) == 1)
      {
        IXamlBuildProvider service = this.TfsRequestContext.GetService<IXamlBuildProvider>();
        if (commitIds == null || !commitIds.Any<string>())
          return service.GetBuildWorkItemRefs(this.TfsRequestContext, this.ProjectInfo, buildId, top).ToList<ResourceRef>();
        webApiBuild = service.GetBuild(this.TfsRequestContext, this.ProjectInfo, buildId);
      }
      if (build == null && webApiBuild == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      return this.InternalBuildService.GetBuildWorkItemRefs(this.TfsRequestContext, (IReadOnlyBuildData) webApiBuild.ToBuildServerBuildData(this.TfsRequestContext), commitIds, top).ToList<ResourceRef>();
    }
  }
}
