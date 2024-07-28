// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.DataAccess.PipelinesComponent
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Server.DataAccess
{
  internal class PipelinesComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PipelinesComponent>(1, true)
    }, "PipelinesService", "Pipelines");
    private static readonly SqlMetaData[] typ_UniqueGuidTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
    };
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    private const string TraceLayer = "PipelinesComponent";

    public PipelinesComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public async Task<IList<Pipeline>> GetPipelines(Guid projectId, ICollection<Guid> pipelineIds)
    {
      PipelinesComponent pipelinesComponent = this;
      IList<Pipeline> items;
      using (pipelinesComponent.RequestContext.TraceScope(nameof (PipelinesComponent), nameof (GetPipelines)))
      {
        pipelinesComponent.PrepareStoredProcedure("Pipelines.prc_GetPipelines");
        HashSet<Guid> rows = new HashSet<Guid>((IEnumerable<Guid>) pipelineIds);
        pipelinesComponent.BindInt("@dataspaceId", pipelinesComponent.GetDataspaceId(projectId));
        pipelinesComponent.BindUniqueGuidTable("@pipelineIds", (ISet<Guid>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await pipelinesComponent.ExecuteReaderAsync(), pipelinesComponent.ProcedureName, pipelinesComponent.RequestContext))
        {
          resultCollection.AddBinder<Pipeline>(pipelinesComponent.GetPipelineBinder());
          items = (IList<Pipeline>) resultCollection.GetCurrent<Pipeline>().Items;
        }
      }
      return items;
    }

    public async Task<Pipeline> AddPipeline(
      Guid projectId,
      Guid pipelineId,
      PipelineConfiguration configuration)
    {
      PipelinesComponent pipelinesComponent = this;
      Pipeline pipeline;
      using (pipelinesComponent.RequestContext.TraceScope(nameof (PipelinesComponent), nameof (AddPipeline)))
      {
        pipelinesComponent.PrepareStoredProcedure("Pipelines.prc_AddPipeline");
        pipelinesComponent.BindInt("@dataspaceId", pipelinesComponent.GetDataspaceId(projectId));
        pipelinesComponent.BindGuid("@pipelineId", pipelineId);
        pipelinesComponent.BindInt("@configurationType", (int) configuration.Type);
        pipelinesComponent.BindInt("@configurationVersion", configuration.Version);
        pipelinesComponent.BindBinary("@configurationData", JsonUtility.Serialize((object) configuration), SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await pipelinesComponent.ExecuteReaderAsync(), pipelinesComponent.ProcedureName, pipelinesComponent.RequestContext))
        {
          resultCollection.AddBinder<Pipeline>(pipelinesComponent.GetPipelineBinder());
          pipeline = resultCollection.GetCurrent<Pipeline>().Items.Single<Pipeline>();
        }
      }
      return pipeline;
    }

    protected virtual ObjectBinder<Pipeline> GetPipelineBinder() => (ObjectBinder<Pipeline>) new PipelineBinder(this);

    protected SqlParameter BindUniqueGuidTable(string parameterName, ISet<Guid> rows)
    {
      rows = rows ?? (ISet<Guid>) new HashSet<Guid>();
      return this.BindTable(parameterName, "Pipelines.typ_UniqueGuidTable", rows.Select<Guid, SqlDataRecord>(new System.Func<Guid, SqlDataRecord>(rowBinder)));

      static SqlDataRecord rowBinder(Guid row)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PipelinesComponent.typ_UniqueGuidTable);
        sqlDataRecord.SetGuid(0, row);
        return sqlDataRecord;
      }
    }
  }
}
