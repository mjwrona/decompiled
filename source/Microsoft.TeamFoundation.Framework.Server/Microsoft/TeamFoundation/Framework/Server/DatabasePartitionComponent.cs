// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabasePartitionComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabasePartitionComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[14]
    {
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent>(1),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent2>(2),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent2>(3),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent2>(4),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent5>(5),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent6>(6),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent7>(7),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent8>(8),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent9>(9),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent10>(10),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent11>(11),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent12>(12),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent13>(13),
      (IComponentCreator) new ComponentCreator<DatabasePartitionComponent14>(14)
    }, "DatabasePartition");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    private const string c_area = "DatabasePartition";
    private const string c_layer = "DatabasePartitionComponent";

    static DatabasePartitionComponent()
    {
      DatabasePartitionComponent.s_sqlExceptionFactories.Add(800079, new SqlExceptionFactory(typeof (DatabasePartitionNotFoundException)));
      DatabasePartitionComponent.s_sqlExceptionFactories.Add(800112, new SqlExceptionFactory(typeof (DatabasePartitionCannotBeCreatedException)));
      DatabasePartitionComponent.s_sqlExceptionFactories.Add(800081, new SqlExceptionFactory(typeof (DatabasePartitionCannotBeDeletedException)));
      DatabasePartitionComponent.s_sqlExceptionFactories.Add(800113, new SqlExceptionFactory(typeof (DatabasePartitionIdInUseException)));
      DatabasePartitionComponent.s_sqlExceptionFactories.Add(800084, new SqlExceptionFactory(typeof (DatabasePartitionNotFoundException)));
      DatabasePartitionComponent.s_sqlExceptionFactories.Add(800111, new SqlExceptionFactory(typeof (DatabasePartitionForeignKeyException)));
      DatabasePartitionComponent.s_sqlExceptionFactories.Add(800114, new SqlExceptionFactory(typeof (HostHasUndrainedActiveRequestsException)));
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
      this.InfoMessage += new SqlInfoMessageEventHandler(this.DatabasePartitionComponent_InfoMessage);
    }

    protected override void Initialize(
      ISqlConnectionInfo connectionInfo,
      int commandTimeout,
      int deadlockPause,
      int maxDeadlockRetries,
      int serviceVersion,
      ITFLogger logger,
      CircuitBreakerDatabaseProperties circuitBreakerProperties)
    {
      base.Initialize(connectionInfo, commandTimeout, deadlockPause, maxDeadlockRetries, serviceVersion, logger, circuitBreakerProperties);
      this.InfoMessage += new SqlInfoMessageEventHandler(this.DatabasePartitionComponent_InfoMessage);
    }

    public override void Dispose()
    {
      base.Dispose();
      this.InfoMessage -= new SqlInfoMessageEventHandler(this.DatabasePartitionComponent_InfoMessage);
    }

    private void DatabasePartitionComponent_InfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
      if (e.Message == null || e.Message.IndexOf("TRUNCATE TABLE", StringComparison.OrdinalIgnoreCase) < 0)
        return;
      if (this.RequestContext != null)
        this.RequestContext.TraceAlways(51894303, TraceLevel.Info, "DatabasePartition", nameof (DatabasePartitionComponent), e.Message);
      else
        TeamFoundationTracingService.TraceRawAlwaysOn(51894303, TraceLevel.Info, "DatabasePartition", nameof (DatabasePartitionComponent), e.Message);
    }

    public bool CanQueryPartitions()
    {
      this.PrepareSqlBatch("SELECT COALESCE(OBJECT_ID('dbo.prc_QueryPartitions', 'P'), 0)".Length);
      this.AddStatement("SELECT COALESCE(OBJECT_ID('dbo.prc_QueryPartitions', 'P'), 0)");
      return (int) this.ExecuteScalar() != 0;
    }

    public virtual int CreatePartition(
      Guid hostId,
      DatabasePartitionState state,
      TeamFoundationHostType hostType,
      int? partitionId)
    {
      this.PrepareStoredProcedure("prc_CreatePartition");
      this.BindGuid("@serviceHostId", hostId);
      this.BindByte("@partitionState", (byte) state);
      if (partitionId.HasValue)
        this.BindInt("@partitionId", partitionId.Value);
      int partition = (int) this.ExecuteScalar();
      if (partitionId.HasValue && partitionId.Value != partition)
        throw new InvalidOperationException(string.Format("Partition for the host {0} already exists in the {1} database, but has id {2} instead of {3}", (object) hostId, (object) this.Database, (object) partition, (object) partitionId.Value));
      return partition;
    }

    public DatabasePartition QueryPartition(Guid hostId, bool includeDeleted = true)
    {
      this.PrepareStoredProcedure("prc_QueryPartition");
      this.BindGuid("@serviceHostId", hostId);
      if (this.Version >= 11)
        this.BindBoolean("@includeDeleted", includeDeleted);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryPartition", this.RequestContext);
      resultCollection.AddBinder<DatabasePartition>((ObjectBinder<DatabasePartition>) this.CreateDatabasePartitionBinder());
      return resultCollection.GetCurrent<DatabasePartition>().FirstOrDefault<DatabasePartition>();
    }

    public virtual List<DatabasePartition> QueryPartitions(Guid hostId, bool includeDeleted)
    {
      this.PrepareStoredProcedure("prc_QueryPartition");
      this.BindGuid("@serviceHostId", hostId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryPartition", this.RequestContext);
      resultCollection.AddBinder<DatabasePartition>((ObjectBinder<DatabasePartition>) this.CreateDatabasePartitionBinder());
      return resultCollection.GetCurrent<DatabasePartition>().Items;
    }

    public int QueryPartitionId(Guid hostId, bool includeDeleted = true)
    {
      DatabasePartition databasePartition = this.QueryPartition(hostId, includeDeleted);
      return databasePartition == null ? DatabasePartitionConstants.InvalidPartitionId : databasePartition.PartitionId;
    }

    public DatabasePartition QueryPartition(int partitionId)
    {
      this.PrepareStoredProcedure("prc_QueryPartitionById");
      this.BindInt("@partitionId", partitionId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryPartitionById", this.RequestContext);
      resultCollection.AddBinder<DatabasePartition>((ObjectBinder<DatabasePartition>) this.CreateDatabasePartitionBinder());
      return resultCollection.GetCurrent<DatabasePartition>().FirstOrDefault<DatabasePartition>();
    }

    public Guid QueryPartitionHostId(int partitionId)
    {
      DatabasePartition databasePartition = this.QueryPartition(partitionId);
      return databasePartition == null ? Guid.Empty : databasePartition.ServiceHostId;
    }

    public ResultCollection QueryPartitionUsage(int? partitionId)
    {
      this.PrepareStoredProcedure("prc_QueryPartitionUsage");
      if (partitionId.HasValue)
        this.BindInt("@partitionId", partitionId.Value);
      else
        this.BindNullValue("@partitionId", SqlDbType.Int);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationDatabaseTenantUsage>((ObjectBinder<TeamFoundationDatabaseTenantUsage>) new DatabaseTenantUsageBinder());
      return resultCollection;
    }

    public virtual ResultCollection QueryPartitionUsageDetailed(int? partitionId) => throw new NotSupportedException();

    public virtual ResultCollection QueryPartitionUsageEstimated(int partitionId) => throw new NotSupportedException();

    public List<DatabasePartition> QueryPartitions()
    {
      this.PrepareStoredProcedure("prc_QueryPartitions");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryPartitions", this.RequestContext);
      resultCollection.AddBinder<DatabasePartition>((ObjectBinder<DatabasePartition>) this.CreateDatabasePartitionBinder());
      return resultCollection.GetCurrent<DatabasePartition>().Items;
    }

    public virtual DatabasePartition QueryOnlyPartition(bool throwIfNoPartitionFound = true)
    {
      List<DatabasePartition> databasePartitionList = this.QueryPartitions();
      DatabasePartition databasePartition;
      if (databasePartitionList == null || databasePartitionList.Count == 0)
      {
        if (throwIfNoPartitionFound)
          throw new DatabasePartitionNotFoundException(this.ConnectionInfo.InitialCatalog);
        databasePartition = (DatabasePartition) null;
      }
      else
        databasePartition = databasePartitionList.Count <= 1 ? databasePartitionList[0] : throw new MultiplePartitionsNotSupportedException(this.ConnectionInfo.InitialCatalog);
      return databasePartition;
    }

    public void SetPartitionState(Guid serviceHostId, DatabasePartitionState partitionState)
    {
      int num = 0;
      while (true)
      {
        try
        {
          this.PrepareStoredProcedure("prc_SetPartitionState");
          this.BindGuid("@serviceHostId", serviceHostId);
          this.BindByte("@partitionState", (byte) partitionState);
          this.ExecuteNonQuery();
          break;
        }
        catch (DatabasePartitionNotFoundException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          if (++num > 5)
            throw;
          else
            Thread.Sleep(TimeSpan.FromSeconds((double) (num * num + 2)));
        }
      }
    }

    public void UpdatePartitionHostId(Guid originalHostId, Guid newHostId)
    {
      this.PrepareStoredProcedure("prc_UpdatePartitionHostId");
      this.BindGuid("@originalHostId", originalHostId);
      this.BindGuid("@hostId", newHostId);
      if ((int) this.ExecuteScalar() == 0)
        throw new DatabasePartitionNotFoundException(originalHostId);
    }

    public virtual void DeletePartition(int partitionId) => throw new NotSupportedException();

    public virtual void DeleteTable(int partitionId, string schema, string table) => throw new NotSupportedException();

    public virtual List<DatabasePartition> QueryPartitionsByState(
      DatabasePartitionState partitionState)
    {
      throw new NotSupportedException();
    }

    public virtual int QueryPartitionCount(DatabasePartitionState? state) => throw new NotSupportedException();

    public virtual void Restore(int partitionId, DatabasePartitionState state) => throw new NotSupportedException();

    public virtual void SetReadOnlyPartitionState(
      Guid serviceHostId,
      bool readOnly,
      bool waitForActiveRequestsToDrain)
    {
      throw new NotSupportedException();
    }

    public virtual bool IsReadOnlyPartition(int partitionId) => throw new NotSupportedException();

    public static DatabasePartitionComponent CreateComponent(ISqlConnectionInfo connectionInfo)
    {
      DatabasePartitionComponent component;
      if (!DatabasePartitionComponent.TryCreateComponent(connectionInfo, out component))
        component = (DatabasePartitionComponent) DatabasePartitionComponent.ComponentFactory.GetLastComponentCreator().Create(connectionInfo, 3600, 0, 10, (ITFLogger) null, (CircuitBreakerDatabaseProperties) null);
      return component;
    }

    public static bool TryCreateComponent(
      ISqlConnectionInfo connectionInfo,
      out DatabasePartitionComponent component)
    {
      return TeamFoundationResourceManagementService.TryCreateComponentRaw<DatabasePartitionComponent>(connectionInfo, 3600, 0, 10, out component, true);
    }

    public static int GetPartitionId(ISqlConnectionInfo connectionInfo, Guid hostId)
    {
      int partitionId = DatabasePartitionConstants.InvalidPartitionId;
      DatabasePartitionComponent component;
      if (DatabasePartitionComponent.TryCreateComponent(connectionInfo, out component))
      {
        using (component)
        {
          DatabasePartition databasePartition = component.QueryPartition(hostId);
          if (databasePartition != null)
            partitionId = databasePartition.PartitionId;
        }
      }
      return partitionId;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) DatabasePartitionComponent.s_sqlExceptionFactories;

    protected virtual DatabasePartitionBinder CreateDatabasePartitionBinder() => new DatabasePartitionBinder();
  }
}
