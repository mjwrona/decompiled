// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.Controllers.ElasticPoolLogsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "elasticpoollogs")]
  public sealed class ElasticPoolLogsController : ElasticApiController
  {
    [HttpGet]
    public async Task<IEnumerable<ElasticPoolLog>> GetElasticPoolLogs(int poolId, [FromUri(Name = "$top")] int? top = null)
    {
      ElasticPoolLogsController poolLogsController = this;
      return (IEnumerable<ElasticPoolLog>) await poolLogsController.ElasticPoolLogService.GetElasticPoolLogsAsync(poolLogsController.TfsRequestContext, poolId, top.HasValue ? top.Value : -1);
    }
  }
}
