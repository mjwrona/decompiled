// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildRetentionSettings1Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "retention", ResourceVersion = 1)]
  [CheckWellFormedProject(Required = true)]
  public class BuildRetentionSettings1Controller : BuildApiController
  {
    [HttpGet]
    public virtual ProjectRetentionSetting GetRetentionSettings() => new ProjectRetentionSettingsHelper(this.TfsRequestContext, this.ProjectInfo.Id, false).ProjectRetentionSettings.ToWebApiProjectRetentionSetting((ISecuredObject) this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext));

    [HttpPatch]
    public virtual ProjectRetentionSetting UpdateRetentionSettings(
      [FromBody] UpdateProjectRetentionSettingModel updateModel)
    {
      TeamProjectReference projectReference = this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext);
      ProjectRetentionSettingsHelper retentionSettingsHelper = new ProjectRetentionSettingsHelper(this.TfsRequestContext, this.ProjectInfo.Id, false);
      if (updateModel == null)
        throw new ArgumentException(BuildServerResources.MustSpecifyAtLeastOneRetentionProperty());
      retentionSettingsHelper.UpdateSettings(this.TfsRequestContext, this.ProjectInfo, updateModel.ArtifactsRetention?.Value, updateModel.RunRetention?.Value, updateModel.PullRequestRunRetention?.Value, updateModel.RetainRunsPerProtectedBranch?.Value);
      return retentionSettingsHelper.ProjectRetentionSettings.ToWebApiProjectRetentionSetting((ISecuredObject) projectReference);
    }
  }
}
