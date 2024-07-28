// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.PipelineReleaseSettingsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(6.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "pipelineReleaseSettings")]
  [SuppressMessage("Microsoft.Naming", "CA1704: IdentifiersShouldBeSpelledCorrectly", Justification = "Correct Spelling")]
  public class PipelineReleaseSettingsController : ReleaseManagementProjectControllerBase
  {
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This can't be a property.")]
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public ProjectPipelineReleaseSettings GetPipelineReleaseSettings()
    {
      ProjectVisibility projectVisibility = this.TfsRequestContext.GetService<IProjectService>().GetProjectVisibility(this.TfsRequestContext, this.ProjectId);
      ProjectPipelineReleaseSettingsHelper releaseSettingsHelper = new ProjectPipelineReleaseSettingsHelper(this.TfsRequestContext, this.ProjectId);
      return new ProjectPipelineReleaseSettings()
      {
        EnforceJobAuthScope = releaseSettingsHelper.EnforceJobAuthScope,
        OrgEnforceJobAuthScope = releaseSettingsHelper.OrgEnforceJobAuthScope,
        HasManageSettingsPermission = releaseSettingsHelper.HasEditPermission,
        PublicProject = projectVisibility == ProjectVisibility.Public
      };
    }

    [HttpPatch]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public ProjectPipelineReleaseSettings UpdatePipelineReleaseSettings(
      [FromBody] ProjectPipelineReleaseSettingsUpdateParameters newSettings)
    {
      if (newSettings == null)
        throw new InvalidRequestException(Resources.PipelineReleaseSettingsCannotBeNull);
      ProjectVisibility projectVisibility = this.TfsRequestContext.GetService<IProjectService>().GetProjectVisibility(this.TfsRequestContext, this.ProjectId);
      ProjectPipelineReleaseSettingsHelper releaseSettingsHelper = new ProjectPipelineReleaseSettingsHelper(this.TfsRequestContext, this.ProjectId);
      if (newSettings.EnforceJobAuthScope.HasValue)
        releaseSettingsHelper.UpdateSettings(this.TfsRequestContext, newSettings.EnforceJobAuthScope.Value);
      return new ProjectPipelineReleaseSettings()
      {
        EnforceJobAuthScope = releaseSettingsHelper.EnforceJobAuthScope,
        OrgEnforceJobAuthScope = releaseSettingsHelper.OrgEnforceJobAuthScope,
        HasManageSettingsPermission = releaseSettingsHelper.HasEditPermission,
        PublicProject = projectVisibility == ProjectVisibility.Public
      };
    }
  }
}
