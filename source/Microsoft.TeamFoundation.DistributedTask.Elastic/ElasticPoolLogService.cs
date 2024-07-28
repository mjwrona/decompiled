// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticPoolLogService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal class ElasticPoolLogService : IElasticPoolLogService, IVssFrameworkService
  {
    private const int c_defaultTop = 200;

    public async Task AddElasticPoolLogAsync(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.LogLevel level,
      OperationType operation,
      string message)
    {
      using (requestContext.TraceScope(ElasticPoolLogService.TraceLayer, nameof (AddElasticPoolLogAsync)))
      {
        ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        try
        {
          requestContext.TraceAlways(10015201, TraceLevel.Info, "DistributedTask", "ElasticPools", string.Format("PoolId:{0}, Level:{1}, Operation:{2}, {3}", (object) poolId, (object) level, (object) operation, (object) message));
          using (ElasticPoolComponent component = requestContext.CreateComponent<ElasticPoolComponent>())
          {
            if (await component.AddElasticPoolLogAsync(new ElasticPoolLog(poolId, level, operation, message)) == null)
              requestContext.TraceError(10015200, "ElasticPoolService", "Add ElasticPoolLog failed for pool id {0}", (object) poolId);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ElasticPoolLogService.TraceLayer, ex);
        }
      }
    }

    public async Task<IReadOnlyList<ElasticPoolLog>> GetElasticPoolLogsAsync(
      IVssRequestContext requestContext,
      int poolId,
      int maxLogCount = -1)
    {
      IReadOnlyList<ElasticPoolLog> elasticPoolLogsAsync;
      using (requestContext.TraceScope(ElasticPoolLogService.TraceLayer, nameof (GetElasticPoolLogsAsync)))
      {
        ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, poolId);
        if (maxLogCount < 0 || maxLogCount > 200)
          maxLogCount = 200;
        using (ElasticPoolComponent component = requestContext.CreateComponent<ElasticPoolComponent>())
          elasticPoolLogsAsync = await component.GetElasticPoolLogsAsync(poolId, maxLogCount);
      }
      return elasticPoolLogsAsync;
    }

    public async Task DeleteElasticPoolLogsByTimeAsync(
      IVssRequestContext requestContext,
      int poolId,
      DateTime lastLogTimestampToKeep)
    {
      using (requestContext.TraceScope(ElasticPoolLogService.TraceLayer, nameof (DeleteElasticPoolLogsByTimeAsync)))
      {
        ElasticHelpers.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        using (ElasticPoolComponent component = requestContext.CreateComponent<ElasticPoolComponent>())
          await component.DeleteElasticPoolLogsByTimeAsync(poolId, lastLogTimestampToKeep);
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private static string TraceLayer => nameof (ElasticPoolLogService);
  }
}
