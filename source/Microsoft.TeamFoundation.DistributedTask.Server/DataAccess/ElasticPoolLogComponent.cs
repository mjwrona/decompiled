// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.ElasticPoolLogComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class ElasticPoolLogComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ElasticPoolLogComponent>(1)
    }, "DistributedTaskElasticPoolLog", "DistributedTask");

    public ElasticPoolLogComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public virtual async Task<ElasticPoolLog> AddElasticPoolLogAsync(ElasticPoolLog epl)
    {
      ElasticPoolLogComponent poolLogComponent = this;
      ElasticPoolLog elasticPoolLog;
      using (poolLogComponent.TraceScope(nameof (AddElasticPoolLogAsync)))
      {
        poolLogComponent.PrepareStoredProcedure("Task.prc_AddElasticPoolLog");
        poolLogComponent.BindInt("@poolId", epl.PoolId);
        poolLogComponent.BindByte("@level", (byte) epl.Level);
        poolLogComponent.BindByte("@operation", (byte) epl.Operation);
        poolLogComponent.BindString("@message", epl.Message, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        poolLogComponent.BindDateTime2("@timestamp", epl.Timestamp);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await poolLogComponent.ExecuteReaderAsync(), poolLogComponent.ProcedureName, poolLogComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticPoolLog>((ObjectBinder<ElasticPoolLog>) poolLogComponent.GetElasticPoolLogBinder());
          elasticPoolLog = resultCollection.GetCurrent<ElasticPoolLog>().Items.SingleOrDefault<ElasticPoolLog>();
        }
      }
      return elasticPoolLog;
    }

    public virtual async Task<IReadOnlyList<ElasticPoolLog>> GetElasticPoolLogsAsync(
      int poolId,
      int maxLogCount)
    {
      ElasticPoolLogComponent poolLogComponent = this;
      IReadOnlyList<ElasticPoolLog> items;
      using (poolLogComponent.TraceScope(nameof (GetElasticPoolLogsAsync)))
      {
        poolLogComponent.PrepareStoredProcedure("Task.prc_GetElasticPoolLogs");
        poolLogComponent.BindInt("@poolId", poolId);
        poolLogComponent.BindInt("@maxLogCount", maxLogCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await poolLogComponent.ExecuteReaderAsync(), poolLogComponent.ProcedureName, poolLogComponent.RequestContext))
        {
          resultCollection.AddBinder<ElasticPoolLog>((ObjectBinder<ElasticPoolLog>) poolLogComponent.GetElasticPoolLogBinder());
          items = (IReadOnlyList<ElasticPoolLog>) resultCollection.GetCurrent<ElasticPoolLog>().Items;
        }
      }
      return items;
    }

    public virtual async Task DeleteElasticPoolLogsByTimeAsync(
      int poolId,
      DateTime lastLogTimestampToKeep)
    {
      ElasticPoolLogComponent poolLogComponent = this;
      using (poolLogComponent.TraceScope(nameof (DeleteElasticPoolLogsByTimeAsync)))
      {
        poolLogComponent.PrepareStoredProcedure("Task.prc_DeleteElasticPoolLogsByTime");
        poolLogComponent.BindInt("@poolId", poolId);
        poolLogComponent.BindDateTime2("@lastLogTimestampToKeep", lastLogTimestampToKeep);
        int num = await poolLogComponent.ExecuteNonQueryAsync();
      }
    }

    protected virtual ElasticPoolLogBinder GetElasticPoolLogBinder() => new ElasticPoolLogBinder();

    private IDisposable TraceScope([CallerMemberName] string method = null) => this.RequestContext.TraceScope(this.TraceLayer, method);

    private string TraceLayer => nameof (ElasticPoolLogComponent);
  }
}
