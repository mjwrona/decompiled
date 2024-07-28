// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Builds6Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "builds", ResourceVersion = 5)]
  public class Builds6Controller : Builds5r5Controller
  {
    [HttpPost]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.Build), null, null)]
    public override HttpResponseMessage QueueBuild(
      [FromBody] Microsoft.TeamFoundation.Build.WebApi.Build build,
      [FromUri] bool ignoreWarnings = false,
      [FromUri] string checkInTicket = null,
      [FromUri] int? sourceBuildId = null,
      [FromUri] int? definitionId = null)
    {
      if (sourceBuildId.HasValue)
      {
        if (build?.Definition != null)
          throw new InvalidBuildException(Resources.BodyMustBeEmptyForRebuild());
        Microsoft.TeamFoundation.Build.WebApi.Build build1 = this.GetBuild(sourceBuildId.Value, (string) null);
        if (build1.Reason != BuildReason.PullRequest)
          throw new BuildOptionNotSupportedException(Resources.RebuildMustBePRBuild());
        build1.BuildNumber = (string) null;
        build1.BuildNumberRevision = new int?();
        build1.RequestedBy = (IdentityRef) null;
        build1.TriggeredByBuild = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
        build1.QueueTime = new DateTime?();
        build1.StartTime = new DateTime?();
        build1.FinishTime = new DateTime?();
        return this.QueueBuildInternal(build1, ignoreWarnings, checkInTicket, sourceBuildId);
      }
      if (build?.Definition != null || !definitionId.HasValue)
        return this.QueueBuildInternal(build, ignoreWarnings, checkInTicket);
      return this.QueueBuildInternal(new Microsoft.TeamFoundation.Build.WebApi.Build()
      {
        Definition = new DefinitionReference()
        {
          Id = definitionId.Value
        }
      }, ignoreWarnings, checkInTicket);
    }
  }
}
