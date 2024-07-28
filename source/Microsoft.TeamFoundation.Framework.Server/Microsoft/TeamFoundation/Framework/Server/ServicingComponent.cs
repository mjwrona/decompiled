// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[13]
    {
      (IComponentCreator) new ComponentCreator<ServicingComponent>(1),
      (IComponentCreator) new ComponentCreator<ServicingComponent2>(2),
      (IComponentCreator) new ComponentCreator<ServicingComponent3>(3),
      (IComponentCreator) new ComponentCreator<ServicingComponent4>(4),
      (IComponentCreator) new ComponentCreator<ServicingComponent5>(5),
      (IComponentCreator) new ComponentCreator<ServicingComponent6>(6),
      (IComponentCreator) new ComponentCreator<ServicingComponent7>(7),
      (IComponentCreator) new ComponentCreator<ServicingComponent8>(8),
      (IComponentCreator) new ComponentCreator<ServicingComponent9>(9),
      (IComponentCreator) new ComponentCreator<ServicingComponent10>(10),
      (IComponentCreator) new ComponentCreator<ServicingComponent11>(11),
      (IComponentCreator) new ComponentCreator<ServicingComponent12>(12),
      (IComponentCreator) new ComponentCreator<ServicingComponent13>(13)
    }, "Servicing");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800034,
        new SqlExceptionFactory(typeof (ServicingStepGroupNotFoundException))
      },
      {
        800035,
        new SqlExceptionFactory(typeof (ServicingStepGroupAlreadyExistsException))
      },
      {
        800037,
        new SqlExceptionFactory(typeof (ServicingOperationAlreadyExistsException))
      },
      {
        800036,
        new SqlExceptionFactory(typeof (ServicingStepGroupInUseException))
      }
    };
    internal const string c_ExecutionHandlerSeparator = ";";

    public static ServicingComponent Create(IVssRequestContext requestContext)
    {
      try
      {
        return requestContext.CreateComponent<ServicingComponent>();
      }
      catch (RequestCanceledException ex)
      {
        return requestContext.FrameworkConnectionInfo.CreateComponentRaw<ServicingComponent>();
      }
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
      this.HostId = requestContext.ServiceHost.InstanceId;
    }

    public Guid HostId { get; set; }

    public ResultCollection GetServicingOperation(string servicingOperation)
    {
      this.PrepareStoredProcedure("prc_GetServicingOperation");
      this.BindString("@servicingOperation", servicingOperation, 128, false, SqlDbType.NVarChar);
      ResultCollection servicingOperation1 = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetServicingOperation", this.RequestContext);
      servicingOperation1.AddBinder<ServicingOperation>((ObjectBinder<ServicingOperation>) this.GetServicingOperationBinder());
      servicingOperation1.AddBinder<ServicingStepGroup>((ObjectBinder<ServicingStepGroup>) new ServicingStepGroupColumns());
      servicingOperation1.AddBinder<ServicingStep>((ObjectBinder<ServicingStep>) new ServicingStepColumns());
      return servicingOperation1;
    }

    public virtual List<string> QueryServicingOperationNames()
    {
      string str = "SELECT ServicingOperation FROM tbl_ServicingOperation";
      this.PrepareSqlBatch(str.Length);
      this.AddStatement(str);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext);
      resultCollection.AddBinder<string>((ObjectBinder<string>) new ServicingOperationNameColumns());
      return resultCollection.GetCurrent<string>().Items;
    }

    public bool DeleteServicingOperation(string servicingOperation)
    {
      this.PrepareStoredProcedure("prc_DeleteServicingOperation");
      this.BindString("@servicingOperation", servicingOperation, 128, false, SqlDbType.NVarChar);
      return (bool) this.ExecuteScalar();
    }

    public ResultCollection QueryServicingStepGroupOperations(string stepGroup)
    {
      this.PrepareStoredProcedure("prc_QueryServicingStepGroupOperations");
      this.BindString("@group", stepGroup, 128, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServicingStepGroupOperations", this.RequestContext);
      resultCollection.AddBinder<ServicingOperation>((ObjectBinder<ServicingOperation>) new ServicingOperationColumns());
      return resultCollection;
    }

    public virtual ServicingOperationColumns GetServicingOperationBinder() => new ServicingOperationColumns();

    public virtual bool BindOperationTarget() => false;

    public virtual ResultCollection QueryServicingStepDetails(
      Guid hostId,
      Guid jobId,
      DateTime queueTime,
      ServicingStepDetailFilterOptions filterOptions,
      long minDetailId)
    {
      this.PrepareStoredProcedure("prc_QueryServicingStepDetails");
      this.BindGuid("@jobSource", this.HostId);
      this.BindGuid("@jobId", jobId);
      this.BindInt("@filterOptions", (int) filterOptions);
      if (minDetailId > 0L)
        this.BindLong("@minDetailId", minDetailId);
      if (filterOptions == ServicingStepDetailFilterOptions.SpecificQueueTime)
        this.BindDateTime("@queueTime", queueTime.ToUniversalTime());
      else
        this.BindNullValue("@queueTime", SqlDbType.DateTime);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServicingStepDetails", this.RequestContext);
      resultCollection.AddBinder<ServicingJobDetail>((ObjectBinder<ServicingJobDetail>) new ServicingJobDetailColumns());
      resultCollection.AddBinder<ServicingStepDetail>((ObjectBinder<ServicingStepDetail>) new ServicingStepDetailColumns());
      return resultCollection;
    }

    public virtual List<ServicingJobDetail> QueryServicingJobDetails(
      Guid hostId,
      string operationClass)
    {
      string sqlStatement = "\r\n                SELECT  HostId,\r\n                        JobId,\r\n                        OperationClass,\r\n                        Result,\r\n                        JobStatus,\r\n                        Operations,\r\n                        QueueTime,\r\n                        StartTime,\r\n                        EndTime,\r\n                        -1 QueuePosition,\r\n                        CompletedStepCount,\r\n                        TotalStepCount\r\n                FROM    tbl_ServicingJobDetail\r\n                WHERE   HostId = @hostId";
      if (operationClass != null)
        sqlStatement += " AND OperationClass = @operationClass";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindGuid("@hostId", hostId);
      if (operationClass != null)
        this.BindString("@operationClass", operationClass, 64, true, SqlDbType.VarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "", this.RequestContext);
      resultCollection.AddBinder<ServicingJobDetail>((ObjectBinder<ServicingJobDetail>) new ServicingJobDetailColumns());
      return resultCollection.GetCurrent<ServicingJobDetail>().Items;
    }

    public virtual ServicingJobDetail UpdateServicingJobDetail(
      Guid hostId,
      Guid jobId,
      string operationClass,
      string operations,
      DateTime queueTime,
      ServicingJobStatus jobStatus,
      ServicingJobResult result,
      short totalStepCount)
    {
      this.PrepareStoredProcedure("prc_UpdateServicingJobDetail");
      this.BindGuid("@hostId", hostId);
      this.BindGuid("@jobId", jobId);
      this.BindString("@operationClass", operationClass, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindDateTime("@queueTime", queueTime);
      this.BindString("@operations", operations, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindInt("@jobStatus", (int) jobStatus);
      if (jobStatus == ServicingJobStatus.Complete)
        this.BindInt("@result", (int) result);
      else
        this.BindNullValue("@result", SqlDbType.Int);
      this.ExecuteNonQuery();
      return (ServicingJobDetail) null;
    }

    public virtual void StartServicingDeployment(string serviceLevels) => throw new NotSupportedException();

    public virtual void FinishServicingDeployment(string serviceLevels) => throw new NotSupportedException();

    public virtual ServicingDeploymentInfo GetServicingDeploymentInfo(string serviceLevels) => throw new NotSupportedException();

    public virtual void CleanupServicingTablesBatch(IEnumerable<Tuple<Guid, Guid>> jobHostMapping) => throw new NotSupportedException();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ServicingComponent.s_sqlExceptionFactories;
  }
}
