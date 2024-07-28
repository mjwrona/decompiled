// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseSettingsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.License;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "releasesettings")]
  [StakeholderLicenseHandler(true)]
  public class ReleaseSettingsController : ReleaseManagementProjectControllerBase
  {
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This can't be a property.")]
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public ReleaseSettings GetReleaseSettings() => this.GetService<ReleaseSettingsService>().GetReleaseSettings(this.TfsRequestContext, this.ProjectId).ToWebApiReleaseSettings();

    [HttpPut]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public ReleaseSettings UpdateReleaseSettings([FromBody] ReleaseSettings releaseSettings) => this.GetService<ReleaseSettingsService>().UpdateReleaseSettings(this.TfsRequestContext, this.ProjectId, releaseSettings.ToServerReleaseSettings()).ToWebApiReleaseSettings();
  }
}
