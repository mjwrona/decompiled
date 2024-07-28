// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildSettingsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "settings", ResourceVersion = 1)]
  public class BuildSettingsController : BuildApiController
  {
    [HttpGet]
    public virtual Microsoft.TeamFoundation.Build.WebApi.BuildSettings GetBuildSettings()
    {
      ProjectInfo projectInfo = this.ProjectInfo;
      TeamProjectReference projectReference = projectInfo != null ? projectInfo.ToTeamProjectReference(this.TfsRequestContext) : (TeamProjectReference) null;
      ITeamFoundationBuildService2 service = this.TfsRequestContext.GetService<ITeamFoundationBuildService2>();
      return new Microsoft.TeamFoundation.Build.WebApi.BuildSettings((ISecuredObject) projectReference)
      {
        DefaultRetentionPolicy = service.GetDefaultRetentionPolicy(this.TfsRequestContext).ToWebApiRetentionPolicy((ISecuredObject) projectReference),
        MaximumRetentionPolicy = service.GetMaximumRetentionPolicy(this.TfsRequestContext).ToWebApiRetentionPolicy((ISecuredObject) projectReference),
        DaysToKeepDeletedBuildsBeforeDestroy = service.GetDaysToKeepDeletedBuildsBeforeDestroy(this.TfsRequestContext)
      };
    }

    [HttpPatch]
    public Microsoft.TeamFoundation.Build.WebApi.BuildSettings UpdateBuildSettings(
      Microsoft.TeamFoundation.Build.WebApi.BuildSettings settings)
    {
      this.CheckRequestContent((object) settings);
      ProjectInfo projectInfo = this.ProjectInfo;
      TeamProjectReference projectReference = projectInfo != null ? projectInfo.ToTeamProjectReference(this.TfsRequestContext) : (TeamProjectReference) null;
      ITeamFoundationBuildService2 service = this.TfsRequestContext.GetService<ITeamFoundationBuildService2>();
      return new Microsoft.TeamFoundation.Build.WebApi.BuildSettings((ISecuredObject) projectReference)
      {
        MaximumRetentionPolicy = service.SetMaximumRetentionPolicy(this.TfsRequestContext, settings.MaximumRetentionPolicy.ToBuildServerRetentionPolicy()).ToWebApiRetentionPolicy((ISecuredObject) projectReference),
        DefaultRetentionPolicy = service.SetDefaultRetentionPolicy(this.TfsRequestContext, settings.DefaultRetentionPolicy.ToBuildServerRetentionPolicy()).ToWebApiRetentionPolicy((ISecuredObject) projectReference),
        DaysToKeepDeletedBuildsBeforeDestroy = service.SetDaysToKeepDeletedBuildsBeforeDestroy(this.TfsRequestContext, settings.DaysToKeepDeletedBuildsBeforeDestroy)
      };
    }
  }
}
