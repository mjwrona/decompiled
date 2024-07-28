// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleaseEnvironments2Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "environments", ResourceVersion = 2)]
  public class RmReleaseEnvironments2Controller : RmReleaseEnvironmentsController
  {
    [HttpPatch]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.None)]
    [ClientExample("PATCH__UpdateReleaseEnvironment.json", "Start deployment on an environment", null, null)]
    public override ReleaseEnvironment UpdateReleaseEnvironment(
      int releaseId,
      int environmentId,
      [FromBody] ReleaseEnvironmentUpdateMetadata environmentUpdateData)
    {
      return this.UpdateReleaseEnvironmentImplementation(releaseId, environmentId, environmentUpdateData);
    }
  }
}
