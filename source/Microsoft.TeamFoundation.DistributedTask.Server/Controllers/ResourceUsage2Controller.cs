// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.ResourceUsage2Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [ClientInternalUseOnly(true, OmitFromTypeScriptDeclareFile = false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "resourceusage", ResourceVersion = 2)]
  public class ResourceUsage2Controller : ResourceUsageController
  {
    [HttpGet]
    [ClientResponseType(typeof (ResourceUsage), null, null)]
    public override ResourceUsage GetResourceUsage(
      [ClientQueryParameter] string parallelismTag = null,
      [ClientQueryParameter] bool poolIsHosted = false,
      [ClientQueryParameter] bool includeRunningRequests = false)
    {
      return base.GetResourceUsage(parallelismTag, poolIsHosted, includeRunningRequests);
    }
  }
}
