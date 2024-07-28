// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LockingComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LockingComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<LockingComponent>(1, true),
      (IComponentCreator) new ComponentCreator<LockingComponent2>(2),
      (IComponentCreator) new ComponentCreator<LockingComponent3>(3)
    }, "Locking");
    private static readonly string s_area = "Locking";
    private static readonly string s_layer = nameof (LockingComponent);
    protected int m_locksAcquiredCount;

    protected override void Initialize(
      IVssRequestContext requestContext,
      string databaseCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.Initialize(requestContext, databaseCategory, dataspaceIdentifier, serviceVersion, connectionType, logger, false);
      if (serviceVersion >= 3)
        return;
      this.PartitionId = 1;
    }

    public virtual bool AcquireLock(
      string resource,
      TeamFoundationLockMode lockMode,
      int lockTimeout)
    {
      resource = this.AppendPartitionId(resource);
      this.PrepareStoredProcedure("prc_AcquireLock", lockTimeout != -1 ? Math.Max(lockTimeout / 1000 + 1, this.CommandTimeout) : 0);
      this.BindString("@lockMode", lockMode.ToString(), 32, false, SqlDbType.NVarChar);
      this.BindString("@resource", resource, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindInt("@lockTimeout", lockTimeout);
      int num = (int) this.ExecuteScalar() >= 0 ? 1 : 0;
      if (num != 0)
        this.IncrementLocksAcquiredCount(1);
      this.ReleaseVerificationLock();
      return num != 0;
    }

    public virtual void ReleaseLock(string resource)
    {
      try
      {
        resource = this.AppendPartitionId(resource);
        this.PrepareStoredProcedure("prc_ReleaseLock");
        this.BindString("@resource", resource, (int) byte.MaxValue, false, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(98000, LockingComponent.s_area, LockingComponent.s_layer, ex);
        throw;
      }
      finally
      {
        this.DecrementLocksAcquiredCount(1);
      }
    }

    public virtual TeamFoundationLockMode QueryLockMode(string resource)
    {
      resource = this.AppendPartitionId(resource);
      this.PrepareStoredProcedure("prc_QueryLockMode");
      this.BindString("@resource", resource, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryLockMode", this.RequestContext);
      resultCollection.AddBinder<TeamFoundationLockMode>((ObjectBinder<TeamFoundationLockMode>) new LockModeColumns());
      return resultCollection.GetCurrent<TeamFoundationLockMode>().Items[0];
    }

    protected override ISqlConnectionInfo PrepareConnectionString(ISqlConnectionInfo connectionInfo)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionInfo.ConnectionString);
      connectionStringBuilder.ApplicationName = "TFS Locking service";
      if (connectionStringBuilder.MaxPoolSize < 400)
        connectionStringBuilder.MaxPoolSize = 400;
      return connectionInfo.Create(connectionStringBuilder.ConnectionString);
    }

    public virtual bool AcquireLocks(
      TeamFoundationLockMode lockMode,
      int lockTimeout,
      string[] resources,
      out string timedoutLockName)
    {
      throw new InvalidServiceVersionException("Locking", 1, 2);
    }

    public virtual void ReleaseLocks(string[] resources)
    {
      try
      {
        throw new InvalidServiceVersionException("Locking", 1, 2);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(98001, LockingComponent.s_area, LockingComponent.s_layer, ex);
        throw;
      }
    }

    protected string AppendPartitionId(string resource) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) this.PartitionId, (object) resource);

    protected void IncrementLocksAcquiredCount(int count) => this.m_locksAcquiredCount += count;

    protected void DecrementLocksAcquiredCount(int count) => this.m_locksAcquiredCount = Math.Max(0, this.m_locksAcquiredCount - count);

    public int LocksAcquiredCount => this.m_locksAcquiredCount;
  }
}
