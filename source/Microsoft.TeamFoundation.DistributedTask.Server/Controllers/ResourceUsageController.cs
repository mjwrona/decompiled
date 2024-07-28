// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.ResourceUsageController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "resourceusage")]
  public class ResourceUsageController : DistributedTaskApiController
  {
    private const int DefaultMaxRequestsCount = 50;

    [HttpGet]
    [ClientResponseType(typeof (ResourceUsage), null, null)]
    public virtual ResourceUsage GetResourceUsage(
      [ClientQueryParameter] string parallelismTag = null,
      [ClientQueryParameter] bool poolIsHosted = false,
      [ClientQueryParameter] bool includeRunningRequests = false)
    {
      return this.ResourceService.GetResourceUsage(this.TfsRequestContext, parallelismTag, poolIsHosted, true, includeRunningRequests, 50);
    }
  }
}
