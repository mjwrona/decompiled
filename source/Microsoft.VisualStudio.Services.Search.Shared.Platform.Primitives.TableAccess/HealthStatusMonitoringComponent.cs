// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.HealthStatusMonitoringComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class HealthStatusMonitoringComponent : SQLTable<HealthStatusMonitoringRecord>
  {
    private const string ServiceName = "Search_HealthStatusMonitoring";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<HealthStatusMonitoringComponent>(1, true)
    }, "Search_HealthStatusMonitoring");
    protected readonly HealthStatusMonitoringComponent.HealthStatusMonitoringColumns HealthStatusMonitoringColumn = new HealthStatusMonitoringComponent.HealthStatusMonitoringColumns();

    public HealthStatusMonitoringComponent()
      : base(false)
    {
    }

    [Info("InternalForTestPurpose")]
    internal HealthStatusMonitoringComponent(string connectionString)
      : base(false)
    {
      this.Initialize(SqlConnectionInfoFactory.Create(connectionString), 3600, 20, 1, 1, this.GetDefaultLogger(), (CircuitBreakerDatabaseProperties) null);
    }

    public override HealthStatusMonitoringRecord Insert(
      HealthStatusMonitoringRecord heathStatusMonitoring)
    {
      this.ValidateNotNull<HealthStatusMonitoringRecord>(nameof (heathStatusMonitoring), heathStatusMonitoring);
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddEntryForMonitoringTable");
        this.BindGuid("@hostId", heathStatusMonitoring.HostId);
        this.BindGuid("@jobId", heathStatusMonitoring.JobId);
        this.BindString("@actionRunData", heathStatusMonitoring.ActionRunData == null ? string.Empty : SQLTable<HealthStatusMonitoringRecord>.ToJsonString((object) heathStatusMonitoring.ActionRunData, typeof (ActionRunData)), int.MaxValue, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        this.BindString("@actionName", heathStatusMonitoring.ActionName, 128, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindShort("@status", (short) heathStatusMonitoring.Status);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format("Failed to Add entry with host ID {0}, JobId {1} with SQL Azure platform.", (object) heathStatusMonitoring.HostId, (object) heathStatusMonitoring.JobId));
      }
      return (HealthStatusMonitoringRecord) null;
    }

    public virtual List<HealthStatusMonitoringRecord> GetHealthStatusMonitoringDataRecords(
      int count,
      Guid hostId,
      Guid jobId,
      JobStatus status)
    {
      this.ValidateNotEmptyGuid(nameof (jobId), jobId);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryHealthStatusMonitoringTable");
        this.BindGuid("@hostId", hostId);
        this.BindGuid("@jobId", jobId);
        this.BindShort("@status", (short) status);
        this.BindInt("@count", count);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<HealthStatusMonitoringRecord>((ObjectBinder<HealthStatusMonitoringRecord>) new HealthStatusMonitoringComponent.HealthStatusMonitoringColumns());
          ObjectBinder<HealthStatusMonitoringRecord> current = resultCollection.GetCurrent<HealthStatusMonitoringRecord>();
          return current?.Items == null || current.Items.Count <= 0 ? new List<HealthStatusMonitoringRecord>() : current.Items;
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve entry with Host ID {0}, JobId {1} with SQL Azure platform.", (object) hostId, (object) jobId)));
      }
    }

    public virtual List<HealthStatusMonitoringRecord> GetHealthStatusRecordsWithStatus(
      int count,
      Guid hostId,
      Guid jobId,
      List<JobStatus> statusList)
    {
      this.ValidateNotEmptyGuid(nameof (jobId), jobId);
      this.ValidateNotNullOrEmptyList<JobStatus>(nameof (statusList), (IList<JobStatus>) statusList);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryHealthStatusMonitoringTableWithStatus");
        List<byte> list = ((IEnumerable<byte>) Array.ConvertAll<JobStatus, byte>(statusList.ToArray(), (Converter<JobStatus, byte>) (value => (byte) value))).ToList<byte>();
        this.BindGuid("@hostId", hostId);
        this.BindGuid("@jobId", jobId);
        this.BindParameterTable<byte>("@statusList", (IEnumerable<byte>) list, SqlDbType.TinyInt, "Val");
        this.BindInt("@count", count);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<HealthStatusMonitoringRecord>((ObjectBinder<HealthStatusMonitoringRecord>) new HealthStatusMonitoringComponent.HealthStatusMonitoringColumns());
          ObjectBinder<HealthStatusMonitoringRecord> current = resultCollection.GetCurrent<HealthStatusMonitoringRecord>();
          return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<HealthStatusMonitoringRecord>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve entry with Host ID {0}, JobName {1} with SQL Azure platform.", (object) hostId, (object) jobId)));
      }
    }

    public virtual void UpdateStatusInHealthStatusTable(int id, JobStatus status)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_UpdateMonitoringStatus");
        this.BindInt("@id", id);
        this.BindByte("@status", (byte) status);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to Update the status of entry with id {0} as {1} with SQL Azure platform.", (object) id, (object) status)));
      }
    }

    protected class HealthStatusMonitoringColumns : ObjectBinder<HealthStatusMonitoringRecord>
    {
      private SqlColumnBinder m_id = new SqlColumnBinder("Id");
      private SqlColumnBinder m_hostId = new SqlColumnBinder("HostId");
      private SqlColumnBinder m_jobId = new SqlColumnBinder("JobId");
      private SqlColumnBinder m_actionName = new SqlColumnBinder("ActionName");
      private SqlColumnBinder m_status = new SqlColumnBinder("Status");
      private SqlColumnBinder m_actionRunData = new SqlColumnBinder("ActionRunData");
      private SqlColumnBinder m_createdTimeStamp = new SqlColumnBinder("CreatedTimeStamp");
      private SqlColumnBinder m_lastUpdatedTimeStamp = new SqlColumnBinder("LastUpdatedTimeStamp");

      protected override HealthStatusMonitoringRecord Bind() => new HealthStatusMonitoringRecord(this.m_hostId.GetGuid((IDataReader) this.Reader), this.m_jobId.GetGuid((IDataReader) this.Reader, false))
      {
        Id = this.m_id.GetInt32((IDataReader) this.Reader),
        ActionRunData = this.m_actionRunData.GetString((IDataReader) this.Reader, true) != string.Empty ? (ActionRunData) SQLTable<HealthStatusMonitoringRecord>.FromJsonString(this.m_actionRunData.GetString((IDataReader) this.Reader, true), typeof (ActionRunData)) : (ActionRunData) null,
        ActionName = this.m_actionName.GetString((IDataReader) this.Reader, false),
        LastUpdatedTimeStamp = this.m_lastUpdatedTimeStamp.GetDateTime((IDataReader) this.Reader),
        CreatedTimeStamp = this.m_createdTimeStamp.GetDateTime((IDataReader) this.Reader),
        Status = (JobStatus) Enum.Parse(typeof (JobStatus), this.m_status.GetByte((IDataReader) this.Reader).ToString(), true)
      };
    }
  }
}
