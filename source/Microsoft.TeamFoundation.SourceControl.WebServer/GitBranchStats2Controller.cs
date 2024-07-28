// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitBranchStats2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("Stats")]
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "BranchStats", ResourceVersion = 2)]
  public class GitBranchStats2Controller : GitBranchStatsController
  {
    [HttpGet]
    [ClientLocationId("D5B216DE-D8D5-4D32-AE76-51DF755B16D3")]
    [ClientExample("GET__git_repositories__repositoryId__stats_branches.json", "For all branches", null, null)]
    [PublicProjectRequestRestrictions]
    public override IList<GitBranchStats> GetBranches(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor baseVersionDescriptor = null)
    {
      return this.GetBranchStatistics(repositoryId, projectId, versionDescriptor: baseVersionDescriptor);
    }

    [HttpGet]
    [ClientLocationId("D5B216DE-D8D5-4D32-AE76-51DF755B16D3")]
    [ClientExample("GET__git_repositories__repositoryId__stats_branches__name_.json", "For a single branch", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__stats_branches__name__baseVersionType-_baseVersionType__baseVersion-_baseVersion_.json", "For a tag or commit", null, null)]
    [PublicProjectRequestRestrictions]
    public override GitBranchStats GetBranch(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      string name,
      [ClientIgnore] string projectId = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor baseVersionDescriptor = null)
    {
      return name != null ? this.GetBranchStatistics(repositoryId, projectId, name, baseVersionDescriptor)[0] : this.GetBranches(repositoryId, projectId, (GitVersionDescriptor) null)[0];
    }

    [HttpPost]
    [ClientLocationId("D5B216DE-D8D5-4D32-AE76-51DF755B16D3")]
    [ClientResponseType(typeof (IList<GitBranchStats>), null, null)]
    [ClientInternalUseOnly(false)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetBranchStatsBatch(
      GitQueryBranchStatsCriteria searchCriteria,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      return this.Request.CreateResponse<IList<GitBranchStats>>(HttpStatusCode.OK, this.GetBranchStatsBatchInternal(searchCriteria, repositoryId, projectId));
    }
  }
}
