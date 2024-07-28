// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskHubLicenseDetailsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "hublicense")]
  public class TaskHubLicenseDetailsController : DistributedTaskApiController
  {
    [HttpGet]
    public virtual TaskHubLicenseDetails GetTaskHubLicenseDetails(
      string hubName,
      bool includeEnterpriseUsersCount = false,
      bool includeHostedAgentMinutesCount = false)
    {
      return this.TaskHubLicenseService.GetTaskHubLicenseDetails(this.TfsRequestContext, hubName, includeEnterpriseUsersCount, includeHostedAgentMinutesCount);
    }

    [HttpPut]
    public TaskHubLicenseDetails UpdateTaskHubLicenseDetails(
      string hubName,
      [FromBody] TaskHubLicenseDetails taskHubLicenseDetails)
    {
      return this.TaskHubLicenseService.UpdateTaskHubLicenseDetails(this.TfsRequestContext, hubName, taskHubLicenseDetails);
    }

    private PlatformTaskHubLicenseService TaskHubLicenseService => this.TfsRequestContext.GetService<PlatformTaskHubLicenseService>();
  }
}
