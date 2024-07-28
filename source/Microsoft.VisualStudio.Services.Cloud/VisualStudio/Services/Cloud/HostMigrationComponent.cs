// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationComponent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[11]
    {
      (IComponentCreator) new ComponentCreator<HostMigrationComponent>(1),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent2>(2),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent3>(3),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent4>(4),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent5>(5),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent6>(6),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent7>(7),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent8>(8),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent9>(9),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent10>(10),
      (IComponentCreator) new ComponentCreator<HostMigrationComponent11>(11)
    }, "HostMigration");
    private static readonly SqlMetaData[] typ_ContainerMigration = new SqlMetaData[6]
    {
      new SqlMetaData("MigrationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ContainerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("BlobContainerUri", SqlDbType.VarChar, 4096L),
      new SqlMetaData("SasToken", SqlDbType.VarBinary, 4096L),
      new SqlMetaData("SigningKeyId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ContainerType", SqlDbType.VarChar, 256L)
    };
    protected string m_storageOnlyName = "@blobOnly";
    protected string m_storageMigrationName = "@containers";
    private static readonly SqlMetaData[] typ_GuidGuidTable = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_SasUpdateTable = new SqlMetaData[3]
    {
      new SqlMetaData("MigrationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ContainerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("SasToken", SqlDbType.VarBinary, 4096L)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800101,
        new SqlExceptionFactory(typeof (DataMigrationEntryAlreadyExistsException))
      }
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) HostMigrationComponent.s_sqlExceptionFactories;

    protected override string TraceArea => "HostMigration";

    public virtual SourceHostMigration CreateSourceMigration(SourceHostMigration migrationRequest)
    {
      this.PrepareStoredProcedure("Migration.prc_CreateSourceMigration");
      this.BindGuid("@migrationId", migrationRequest.MigrationId);
      this.BindGuid("@hostId", migrationRequest.HostId);
      this.BindBoolean(this.m_storageOnlyName, migrationRequest.StorageOnly);
      this.BindGuid("@targetServiceInstanceId", migrationRequest.TargetServiceInstanceId);
      if (!object.Equals((object) migrationRequest.ParentMigrationId, (object) Guid.Empty))
        this.BindGuid("@parentMigrationId", migrationRequest.ParentMigrationId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SourceHostMigration>((ObjectBinder<SourceHostMigration>) this.CreateSourceMigrationColumns());
        return resultCollection.GetCurrent<SourceHostMigration>().Items.FirstOrDefault<SourceHostMigration>();
      }
    }

    public virtual SourceHostMigration GetSourceMigration(Guid migrationId)
    {
      this.PrepareStoredProcedure("Migration.prc_GetSourceMigration");
      this.BindGuid("@migrationId", migrationId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SourceHostMigration>((ObjectBinder<SourceHostMigration>) this.CreateSourceMigrationColumns());
        return resultCollection.GetCurrent<SourceHostMigration>().Items.FirstOrDefault<SourceHostMigration>();
      }
    }

    public virtual List<SourceHostMigration> GetSourceMigrationByHostId(Guid hostId)
    {
      this.PrepareStoredProcedure("Migration.prc_GetSourceMigration");
      this.BindGuid("@hostId", hostId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SourceHostMigration>((ObjectBinder<SourceHostMigration>) this.CreateSourceMigrationColumns());
        return resultCollection.GetCurrent<SourceHostMigration>().Items;
      }
    }

    public virtual ICollection<SourceHostMigration> GetSourceMigrationsByTargetId(
      Guid targetServiceInstanceId)
    {
      return (ICollection<SourceHostMigration>) Array.Empty<SourceHostMigration>();
    }

    public virtual Guid QueryLatestTargetMigrationByHostId(Guid hostId) => Guid.Empty;

    public virtual void UpdateSourceMigration(SourceHostMigration migrationEntry)
    {
      this.PrepareStoredProcedure("Migration.prc_UpdateSourceMigration");
      this.BindGuid("@migrationId", migrationEntry.MigrationId);
      this.BindInt("@status", (int) migrationEntry.State);
      this.BindString("@statusMessage", migrationEntry.StatusMessage, -1, true, SqlDbType.NVarChar);
      if (migrationEntry.CredentialId > 0)
        this.BindInt("@credentialId", migrationEntry.CredentialId);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateSourceMigration(
      Guid migrationId,
      SourceMigrationState status,
      string statusMessage)
    {
      this.PrepareStoredProcedure("Migration.prc_UpdateSourceMigration");
      this.BindGuid("@migrationId", migrationId);
      this.BindInt("@status", (int) status);
      this.BindString("@statusMessage", statusMessage, -1, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual TargetHostMigration CreateTargetMigration(TargetHostMigration entry)
    {
      this.PrepareStoredProcedure("Migration.prc_CreateTargetMigration");
      this.BindGuid("@migrationId", entry.MigrationId);
      this.BindGuid("@hostId", entry.HostProperties.Id);
      this.BindBoolean(this.m_storageOnlyName, entry.StorageOnly);
      this.BindStorageMigrationTable(this.m_storageMigrationName, (IEnumerable<StorageMigration>) entry.StorageResources);
      this.BindInt("@storageAccountId", entry.StorageAccountId);
      this.BindGuid("@sourceInstanceId", entry.SourceServiceInstanceId);
      if (!object.Equals((object) entry.ParentMigrationId, (object) Guid.Empty))
        this.BindGuid("@parentMigrationId", entry.ParentMigrationId);
      this.BindShardingInfoTable("@shardingInfo", (IEnumerable<ShardingInfo>) entry.ShardingInfo);
      return this.ReadTargetMigration();
    }

    protected virtual SqlParameter BindShardingInfoTable(
      string parameterName,
      IEnumerable<ShardingInfo> rows)
    {
      return (SqlParameter) null;
    }

    public TargetHostMigration GetTargetMigration(Guid migrationId)
    {
      this.PrepareStoredProcedure("Migration.prc_GetTargetMigration");
      this.BindGuid("@migrationId", migrationId);
      return this.ReadTargetMigration();
    }

    public List<Guid> QueryTargetMigrations(TargetMigrationState state)
    {
      this.PrepareStoredProcedure("Migration.prc_QueryTargetMigrations");
      this.BindByte("@status", (byte) state);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder migrationId = new SqlColumnBinder("MigrationId");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => migrationId.GetGuid(reader))));
        return resultCollection.GetCurrent<Guid>().Items;
      }
    }

    public List<Guid> QueryTargetMigrations()
    {
      this.PrepareStoredProcedure("Migration.prc_QueryTargetMigrations");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder migrationId = new SqlColumnBinder("MigrationId");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => migrationId.GetGuid(reader))));
        return resultCollection.GetCurrent<Guid>().Items;
      }
    }

    public virtual void UpdateTargetMigration(TargetHostMigration migrationEntry)
    {
      this.PrepareStoredProcedure("Migration.prc_UpdateTargetMigration");
      this.BindGuid("@migrationId", migrationEntry.MigrationId);
      this.BindNullableInt("@sourceDatabaseId", migrationEntry.SourceDatabaseId, 0);
      this.BindNullableInt("@targetDatabaseId", migrationEntry.TargetDatabaseId, 0);
      this.BindInt("@status", (int) migrationEntry.State);
      this.BindString("@statusMessage", migrationEntry.StatusMessage, -1, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateTargetMigration(
      Guid migrationId,
      TargetMigrationState state,
      string statusMessage)
    {
      this.PrepareStoredProcedure("Migration.prc_UpdateTargetMigration");
      this.BindGuid("@migrationId", migrationId);
      this.BindInt("@status", (int) state);
      this.BindString("@statusMessage", statusMessage, -1, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public List<StorageMigration> QueryContainerMigrations()
    {
      this.PrepareStoredProcedure("Migration.prc_QueryContainerMigrations");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StorageMigration>(this.CreatStorageMigrationColumns());
        return resultCollection.GetCurrent<StorageMigration>().Items;
      }
    }

    public List<Guid> QueryContainerMigrationsSigningKeys()
    {
      this.PrepareStoredProcedure("Migration.prc_QueryContainerMigrationsSigningKeys");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder keyIdColumn = new SqlColumnBinder("SigningKeyId");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => keyIdColumn.GetGuid(reader))));
        return resultCollection.GetCurrent<Guid>().Items;
      }
    }

    public List<StorageMigration> QueryContainerMigrationsById(Guid migrationId)
    {
      this.PrepareStoredProcedure("Migration.prc_QueryContainerMigrationsById");
      this.BindGuid("@migrationId", migrationId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StorageMigration>(this.CreatStorageMigrationColumns());
        return resultCollection.GetCurrent<StorageMigration>().Items;
      }
    }

    public void UpdateContainerMigrationStatus(
      StorageMigration container,
      StorageMigrationStatus status,
      string statusReason)
    {
      this.UpdateContainerMigrationStatus(new List<StorageMigration>()
      {
        container
      }, status, statusReason);
    }

    public void UpdateContainerMigrationStatus(
      List<StorageMigration> containers,
      StorageMigrationStatus status,
      string statusReason)
    {
      this.PrepareStoredProcedure("Migration.prc_UpdateContainerMigrationStatus");
      string type;
      System.Func<StorageMigration, SqlDataRecord> containerMigrationBinder = this.GetContainerMigrationBinder(out type);
      this.BindTable("@inputBatch", type, containers.Select<StorageMigration, SqlDataRecord>(containerMigrationBinder));
      this.BindByte("@status", (byte) status);
      this.BindString("@statusReason", statusReason, -1, true, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public List<Guid> GetTargetPreMigrations(TargetMigrationState status)
    {
      this.PrepareStoredProcedure("Migration.prc_GetTargetPreMigrations");
      this.BindByte("@status", (byte) status);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder keyIdColumn = new SqlColumnBinder("MigrationId");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => keyIdColumn.GetGuid(reader))));
        return resultCollection.GetCurrent<Guid>().Items;
      }
    }

    public virtual TargetHostMigration GetLastPreMigration(Guid hostId)
    {
      this.PrepareStoredProcedure("Migration.prc_GetLastPreMigration");
      this.BindGuid("@hostId", hostId);
      TargetHostMigration lastPreMigration = (TargetHostMigration) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TargetHostMigration>((ObjectBinder<TargetHostMigration>) this.CreateTargetMigrationColumns());
        lastPreMigration = resultCollection.GetCurrent<TargetHostMigration>().Items.FirstOrDefault<TargetHostMigration>();
        if (lastPreMigration != null)
        {
          resultCollection.NextResult();
          resultCollection.AddBinder<StorageMigration>(this.CreatStorageMigrationColumns());
          lastPreMigration.StorageResources = resultCollection.GetCurrent<StorageMigration>().Items.ToArray();
        }
      }
      return lastPreMigration;
    }

    public virtual void CleanupMigrations(int ageInDays) => throw new NotSupportedException("Implemented in v4 of HostMigrationComponent");

    public virtual void CreateResourceMigrationJob(ResourceMigrationJob resourceMigrationJob)
    {
      this.PrepareStoredProcedure("Migration.prc_CreateResourceMigrationJob");
      this.BindGuid("@migrationId", resourceMigrationJob.MigrationId);
      this.BindGuid("@jobId", resourceMigrationJob.JobId);
      this.BindString("@name", resourceMigrationJob.Name, 100, true, SqlDbType.VarChar);
      this.BindByte("@status", (byte) resourceMigrationJob.Status);
      this.BindInt("@RetriesRemaining", resourceMigrationJob.RetriesRemaining);
      this.ExecuteNonQuery();
    }

    public virtual List<ResourceMigrationJob> QueryResourceMigrationJobs(Guid migrationId)
    {
      this.PrepareStoredProcedure("Migration.prc_QueryResourceMigrationJobs");
      this.BindGuid("@migrationId", migrationId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ResourceMigrationJob>((ObjectBinder<ResourceMigrationJob>) this.CreateResourceMigrationJobColumns());
        return resultCollection.GetCurrent<ResourceMigrationJob>().Items;
      }
    }

    public List<ResourceMigrationJob> QueryResourceMigrationJobs(
      Guid jobId,
      ResourceMigrationState status)
    {
      this.PrepareStoredProcedure("Migration.prc_QueryResourceMigrationJobs");
      this.BindGuid("@jobId", jobId);
      this.BindByte("@status", (byte) status);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ResourceMigrationJob>((ObjectBinder<ResourceMigrationJob>) this.CreateResourceMigrationJobColumns());
        return resultCollection.GetCurrent<ResourceMigrationJob>().Items;
      }
    }

    public virtual List<ResourceMigrationJob> QueryMigrationJobs(Guid migrationId)
    {
      this.PrepareStoredProcedure("Migration.prc_QueryMigrationJobs");
      this.BindGuid("@migrationId", migrationId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ResourceMigrationJob>((ObjectBinder<ResourceMigrationJob>) this.CreateResourceMigrationJobColumns());
        return resultCollection.GetCurrent<ResourceMigrationJob>().Items;
      }
    }

    public void UpdateResourceMigrationJob(ResourceMigrationJob resourceMigrationJob)
    {
      this.PrepareStoredProcedure("Migration.prc_UpdateResourceMigrationJob");
      this.BindGuid("@migrationId", resourceMigrationJob.MigrationId);
      this.BindGuid("@jobId", resourceMigrationJob.JobId);
      this.BindInt("@status", (int) (byte) resourceMigrationJob.Status);
      this.BindInt("@retriesRemaining", resourceMigrationJob.RetriesRemaining);
      this.ExecuteNonQuery();
    }

    public void UpdateResourceMigrationJobStatus(
      Guid migrationId,
      Guid jobId,
      ResourceMigrationState status)
    {
      this.PrepareStoredProcedure("Migration.prc_UpdateResourceMigrationJob");
      this.BindGuid("@migrationId", migrationId);
      this.BindGuid("@jobId", jobId);
      this.BindInt("@status", (int) (byte) status);
      this.ExecuteNonQuery();
    }

    public virtual ContainerMigrationProgress CreateContainerMigrationProgress(
      Guid migrationId,
      Guid parallelBlobMigrationCoordinatorJobId,
      Guid tfsBlobMigrationJobId,
      int jobNo,
      int totalJobs,
      string uri,
      string prefix)
    {
      return (ContainerMigrationProgress) null;
    }

    public virtual List<ContainerMigrationProgress> QueryContainerMigrationProgressEntries(
      Guid migrationId,
      string uri)
    {
      return (List<ContainerMigrationProgress>) null;
    }

    public virtual ContainerMigrationProgress UpdateContainerMigrationProgress(
      ContainerMigrationProgress cmp)
    {
      return (ContainerMigrationProgress) null;
    }

    public virtual void DeleteContainerMigrationProgress(Guid migrationId)
    {
    }

    protected virtual System.Func<StorageMigration, SqlDataRecord> GetContainerMigrationBinder(
      out string type)
    {
      type = "typ_GuidGuidTable";
      return (System.Func<StorageMigration, SqlDataRecord>) (row =>
      {
        SqlDataRecord containerMigrationBinder = new SqlDataRecord(HostMigrationComponent.typ_GuidGuidTable);
        containerMigrationBinder.SetGuid(0, row.MigrationId);
        containerMigrationBinder.SetGuid(1, Guid.Parse(row.Id));
        return containerMigrationBinder;
      });
    }

    protected virtual SourceMigrationColumns CreateSourceMigrationColumns() => new SourceMigrationColumns();

    protected virtual TargetMigrationColumns CreateTargetMigrationColumns() => new TargetMigrationColumns();

    protected virtual ObjectBinder<StorageMigration> CreatStorageMigrationColumns() => (ObjectBinder<StorageMigration>) new StorageMigrationColumns();

    protected virtual ResourceMigrationJobColumns CreateResourceMigrationJobColumns() => new ResourceMigrationJobColumns();

    protected virtual ShardingInfoColumns CreateShardingInfoColumns() => (ShardingInfoColumns) null;

    protected virtual TargetHostMigration ReadTargetMigration()
    {
      TargetHostMigration targetHostMigration;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TargetHostMigration>((ObjectBinder<TargetHostMigration>) this.CreateTargetMigrationColumns());
        targetHostMigration = resultCollection.GetCurrent<TargetHostMigration>().Items.FirstOrDefault<TargetHostMigration>();
        if (targetHostMigration != null)
        {
          resultCollection.NextResult();
          resultCollection.AddBinder<StorageMigration>(this.CreatStorageMigrationColumns());
          targetHostMigration.StorageResources = resultCollection.GetCurrent<StorageMigration>().Items.ToArray();
          ObjectBinder<ShardingInfo> shardingInfoColumns = (ObjectBinder<ShardingInfo>) this.CreateShardingInfoColumns();
          if (shardingInfoColumns != null)
          {
            resultCollection.NextResult();
            resultCollection.AddBinder<ShardingInfo>(shardingInfoColumns);
            targetHostMigration.ShardingInfo = resultCollection.GetCurrent<ShardingInfo>().Items.ToArray();
          }
        }
      }
      return targetHostMigration;
    }

    protected virtual SqlParameter BindStorageMigrationTable(
      string parameterName,
      IEnumerable<StorageMigration> rows)
    {
      throw new NotSupportedException();
    }
  }
}
