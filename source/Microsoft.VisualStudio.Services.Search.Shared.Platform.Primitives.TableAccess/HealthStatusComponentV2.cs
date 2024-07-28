// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.HealthStatusComponentV2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class HealthStatusComponentV2 : HealthStatusComponent
  {
    private const string ServiceName = "Search_HealthStatus";
    private static readonly SqlMetaData[] s_jobStatusTable = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.TinyInt)
    };

    public HealthStatusComponentV2()
    {
    }

    [Info("InternalForTestPurpose")]
    internal HealthStatusComponentV2(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public override List<HealthStatusRecord> GetHealthStatusJobDataRecords(
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
          resultCollection.AddBinder<HealthStatusRecord>((ObjectBinder<HealthStatusRecord>) new HealthStatusComponentV2.HealthStatusColumnsV2(this.m_knownEntityTypes));
          ObjectBinder<HealthStatusRecord> current = resultCollection.GetCurrent<HealthStatusRecord>();
          return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<HealthStatusRecord>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve entry with Collection ID {0}, JobName {1} with SQL Azure platform.", (object) collectionId, (object) jobName)));
      }
    }

    public virtual List<HealthStatusRecord> GetHealthStatusRecordsWithStatus(
      int count,
      Guid collectionId,
      string jobName,
      List<JobStatus> statusList)
    {
      this.ValidateNotNullOrEmptyString(nameof (jobName), jobName);
      this.ValidateNotNullOrEmptyList<JobStatus>(nameof (statusList), (IList<JobStatus>) statusList);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryHealthStatusTableWithStatus");
        List<byte> list = ((IEnumerable<byte>) Array.ConvertAll<JobStatus, byte>(statusList.ToArray(), (Converter<JobStatus, byte>) (value => (byte) value))).ToList<byte>();
        this.BindInt("@count", count);
        this.BindGuid("@collectionId", collectionId);
        this.BindJobStatusTable("@statusList", (IEnumerable<byte>) list);
        this.BindString("@jobName", jobName, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<HealthStatusRecord>((ObjectBinder<HealthStatusRecord>) new HealthStatusComponentV2.HealthStatusColumnsV2(this.m_knownEntityTypes));
          ObjectBinder<HealthStatusRecord> current = resultCollection.GetCurrent<HealthStatusRecord>();
          return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<HealthStatusRecord>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to retrieve entry with Collection ID {0}, JobName {1} with SQL Azure platform.", (object) collectionId, (object) jobName)));
      }
    }

    protected SqlParameter BindJobStatusTable(string parameterName, IEnumerable<byte> rows)
    {
      rows = rows ?? Enumerable.Empty<byte>();
      System.Func<byte, SqlDataRecord> selector = (System.Func<byte, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(HealthStatusComponentV2.s_jobStatusTable);
        sqlDataRecord.SetByte(0, entity);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_TinyIntTable", rows.Select<byte, SqlDataRecord>(selector));
    }

    protected class HealthStatusColumnsV2 : HealthStatusComponent.HealthStatusColumns
    {
      private SqlColumnBinder m_status = new SqlColumnBinder("Status");

      protected override HealthStatusRecord Bind()
      {
        HealthStatusRecord healthStatusRecord = base.Bind();
        healthStatusRecord.Status = (JobStatus) this.m_status.GetByte((IDataReader) this.Reader);
        return healthStatusRecord;
      }

      public HealthStatusColumnsV2(IEnumerable<Type> knownEntityTypes)
        : base(knownEntityTypes)
      {
      }
    }
  }
}
