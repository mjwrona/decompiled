// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.OrgPipelineReleaseSettingsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
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
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "orgPipelineReleaseSettings")]
  [SuppressMessage("Microsoft.Naming", "CA1704: IdentifiersShouldBeSpelledCorrectly", Justification = "Correct Spelling")]
  public class OrgPipelineReleaseSettingsController : ReleaseManagementProjectControllerBase
  {
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This can't be a property.")]
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public OrgPipelineReleaseSettings GetOrgPipelineReleaseSettings()
    {
      OrgPipelineReleaseSettingsHelper releaseSettingsHelper = new OrgPipelineReleaseSettingsHelper(this.TfsRequestContext);
      return new OrgPipelineReleaseSettings()
      {
        OrgEnforceJobAuthScope = releaseSettingsHelper.OrgEnforceJobAuthScope,
        HasManagePipelinePoliciesPermission = releaseSettingsHelper.HasEditPermission
      };
    }

    [HttpPatch]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public OrgPipelineReleaseSettings UpdateOrgPipelineReleaseSettings(
      [FromBody] OrgPipelineReleaseSettingsUpdateParameters newSettings)
    {
      if (newSettings == null)
        throw new InvalidRequestException(Resources.PipelineReleaseSettingsCannotBeNull);
      OrgPipelineReleaseSettingsHelper releaseSettingsHelper = new OrgPipelineReleaseSettingsHelper(this.TfsRequestContext);
      if (newSettings.OrgEnforceJobAuthScope.HasValue)
        releaseSettingsHelper.UpdateSettings(this.TfsRequestContext, newSettings.OrgEnforceJobAuthScope.Value);
      return new OrgPipelineReleaseSettings()
      {
        OrgEnforceJobAuthScope = releaseSettingsHelper.OrgEnforceJobAuthScope,
        HasManagePipelinePoliciesPermission = releaseSettingsHelper.HasEditPermission
      };
    }
  }
}
