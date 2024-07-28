// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Controllers.MavenUpstreamingController
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Maven.Server.Controllers
{
  [ControllerApiVersion(6.1)]
  [ClientGroupByResource("Maven")]
  [VersionedApiControllerCustomName(Area = "maven", ResourceName = "upstreaming")]
  [ClientEnforceBodyParameterToComeBeforeQueryParameters(false)]
  public class MavenUpstreamingController : MavenBaseController
  {
    [HttpGet]
    public Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior GetUpstreamingBehavior(
      string feed,
      string groupId,
      string artifactId)
    {
      IFeedRequest feedRequest = this.GetFeedRequest(feed, FeedValidator.GetFeedIsNotReadOnlyValidator());
      IPackageUpstreamBehaviorService service = this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>();
      MavenPackageName mavenPackageName1 = new MavenPackageName(groupId, artifactId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      FeedCore feed1 = feedRequest.Feed;
      MavenPackageName mavenPackageName2 = mavenPackageName1;
      return service.GetBehavior(tfsRequestContext, feed1, (IPackageName) mavenPackageName2).ToWebApi();
    }

    [HttpPatch]
    public void SetUpstreamingBehavior(
      string feed,
      string groupId,
      string artifactId,
      [FromBody] Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.UpstreamingBehavior behavior)
    {
      IFeedRequest feedRequest = this.GetFeedRequest(feed, FeedValidator.GetFeedIsNotReadOnlyValidator());
      IPackageUpstreamBehaviorService service = this.TfsRequestContext.GetService<IPackageUpstreamBehaviorService>();
      MavenPackageName mavenPackageName1 = new MavenPackageName(groupId, artifactId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      FeedCore feed1 = feedRequest.Feed;
      MavenPackageName mavenPackageName2 = mavenPackageName1;
      Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior behavior1 = new Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamingBehavior(behavior.VersionsFromExternalUpstreams);
      service.SetBehavior(tfsRequestContext, feed1, (IPackageName) mavenPackageName2, behavior1);
    }
  }
}
