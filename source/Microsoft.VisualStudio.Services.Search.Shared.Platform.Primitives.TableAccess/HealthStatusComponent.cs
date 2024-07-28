// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.HealthStatusComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class HealthStatusComponent : SQLTable<HealthStatusRecord>
  {
    private const string ServiceName = "Search_HealthStatus";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<HealthStatusComponent>(1, true),
      (IComponentCreator) new ComponentCreator<HealthStatusComponentV2>(2)
    }, "Search_HealthStatus");
    protected IEnumerable<Type> m_knownEntityTypes;

    public HealthStatusComponent()
      : base(false)
    {
    }

    [Info("InternalForTestPurpose")]
    internal HealthStatusComponent(string connectionString, IVssRequestContext requestContext)
      : base(false)
    {
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(connectionString);
      this.m_knownEntityTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntitiesKnownTypes();
      this.Initialize(connectionInfo, 3600, 20, 1, 1, this.GetDefaultLogger(), (CircuitBreakerDatabaseProperties) null);
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.m_knownEntityTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntitiesKnownTypes();
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public override HealthStatusRecord Insert(HealthStatusRecord heathStatus)
    {
      this.ValidateNotNull<HealthStatusRecord>(nameof (heathStatus), heathStatus);
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddEntryForHealthStatusTable");
        this.BindGuid("@collectionId", heathStatus.CollectionId);
        this.BindString("@jobName", heathStatus.JobName, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        if (heathStatus.Data != null)
          this.BindString("@data", SQLTable<HealthStatusRecord>.ToJsonString((object) heathStatus.Data, typeof (HealthStatusJobData), this.m_knownEntityTypes), int.MaxValue, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        else
          this.BindString("@data", string.Empty, int.MaxValue, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindByte("@mode", (byte) heathStatus.Mode);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format("Failed to Add entry with Collection ID {0}, JobName {1} with SQL Azure platform.", (object) heathStatus.CollectionId, (object) heathStatus.JobName));
      }
      return (HealthStatusRecord) null;
    }

    public virtual List<HealthStatusRecord> GetHealthStatusJobDataRecords(
      int count,
      Guid collectionId,
      string jobName,
      JobStatus status)
    {
      this.ValidateNotNullOrEmptyString(nameof (jobName), jobName);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryHealthStatusTable");
        this.BindInt("@count", count);
        this.BindGuid("@collectionId", collectionId);
        this.BindByte("@status", (byte) status);
        this.BindString("@jobName", jobName, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<HealthStatusRecord>((ObjectBinder<HealthStatusRecord>) new HealthStatusComponent.HealthStatusColumns(this.m_knownEntityTypes));
          ObjectBinder<HealthStatusRecord> current = resultCollection.GetCurrent<HealthStatusRecord>();
          return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<HealthStatusRecord>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve entry with Collection ID {0}, JobName {1} with SQL Azure platform.", (object) collectionId, (object) jobName)));
      }
    }

    public virtual void UpdateStatusInHealthStatusTable(int id, JobStatus status)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_UpdateStatus");
        this.BindInt("@id", id);
        this.BindByte("@status", (byte) status);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to Update the status of entry with id {0} as {1} with SQL Azure platform.", (object) id, (object) status)));
      }
    }

    protected class HealthStatusColumns : ObjectBinder<HealthStatusRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_collectionId = new SqlColumnBinder("CollectionId");
      private SqlColumnBinder m_jobName = new SqlColumnBinder("JobName");
      private SqlColumnBinder m_data = new SqlColumnBinder("Data");
      private SqlColumnBinder m_mode = new SqlColumnBinder("Mode");
      private SqlColumnBinder m_lastUpdatedTimeStamp = new SqlColumnBinder("LastUpdatedTimeStamp");
      private IEnumerable<Type> m_knownEntityTypes;

      protected override HealthStatusRecord Bind() => new HealthStatusRecord(this.m_collectionId.GetGuid((IDataReader) this.Reader), this.m_jobName.GetString((IDataReader) this.Reader, false))
      {
        Id = this.m_id.GetInt32((IDataReader) this.Reader),
        Data = this.m_data.GetString((IDataReader) this.Reader, true) != string.Empty ? (HealthStatusJobData) SQLTable<HealthStatusRecord>.FromJsonString(this.m_data.GetString((IDataReader) this.Reader, true), typeof (HealthStatusJobData), this.m_knownEntityTypes) : (HealthStatusJobData) null,
        Mode = (JobMode) this.m_mode.GetByte((IDataReader) this.Reader)
      };

      public HealthStatusColumns(IEnumerable<Type> knownEntityTypes) => this.m_knownEntityTypes = knownEntityTypes;
    }
  }
}
