// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.DataAccess.PermissionsReportComponent
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.PermissionsReport.DataAccess
{
  public class PermissionsReportComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<PermissionsReportComponent>(1),
      (IComponentCreator) new ComponentCreator<PermissionsReportComponent2>(2)
    }, "PermissionsReport");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        3890008,
        new SqlExceptionFactory(typeof (PermissionsReportNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new PermissionsReportNotFoundException(Resources.PermissionsReportNotFound((object) sqEr.ExtractString("id")))))
      }
    };
    private const string c_area = "PermissionsReport";
    private const string c_layer = "PermissionsReportComponent";
    private const string c_serviceName = "PermissionsReport";
    protected const int c_maxResourceLength = 1024;
    protected const int c_maxReportDataLength = 4096;

    public PermissionsReportComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void Initialize()
    {
      this.PrepareStoredProcedure("prc_InstallPermissionsReportData");
      this.ExecuteNonQuery();
    }

    public virtual PermissionsReportStoreItem CreatePermissionsReport(
      PermissionsReportStoreItem permissionsReportStoreItem)
    {
      ArgumentUtility.CheckForEmptyGuid(permissionsReportStoreItem.ReportId, "ReportId");
      ArgumentUtility.CheckStringForNullOrEmpty(permissionsReportStoreItem.ReportName, "ReportName");
      ArgumentUtility.CheckStringForNullOrEmpty(permissionsReportStoreItem.ReportData, "ReportData");
      this.PrepareStoredProcedure("prc_CreatePermissionsReport");
      this.BindGuid("@ReportId", permissionsReportStoreItem.ReportId);
      this.BindGuid("@JobId", permissionsReportStoreItem.JobId);
      this.BindString("@ReportName", permissionsReportStoreItem.ReportName, 1024, false, SqlDbType.VarChar);
      this.BindString("@ReportData", permissionsReportStoreItem.ReportData, 4096, false, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionsReportStoreItem>(this.CreatePermissionsReportColumns());
        return resultCollection.GetCurrent<PermissionsReportStoreItem>().FirstOrDefault<PermissionsReportStoreItem>();
      }
    }

    public virtual IEnumerable<PermissionsReportStoreItem> GetPermissionsReports()
    {
      this.PrepareStoredProcedure("prc_QueryPermissionsReports");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionsReportStoreItem>(this.CreatePermissionsReportColumns());
        return (IEnumerable<PermissionsReportStoreItem>) resultCollection.GetCurrent<PermissionsReportStoreItem>().ToList<PermissionsReportStoreItem>();
      }
    }

    public virtual IEnumerable<PermissionsReportStoreItem> GetActivePermissionsReports()
    {
      this.PrepareStoredProcedure("prc_QueryActivePermissionsReports");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionsReportStoreItem>(this.CreatePermissionsReportColumns());
        return (IEnumerable<PermissionsReportStoreItem>) resultCollection.GetCurrent<PermissionsReportStoreItem>().ToList<PermissionsReportStoreItem>();
      }
    }

    public virtual PermissionsReportStoreItem GetPermissionsReportByReportId(Guid reportId)
    {
      ArgumentUtility.CheckForEmptyGuid(reportId, nameof (reportId));
      this.PrepareStoredProcedure("prc_QueryPermissionsReportByReportId");
      this.BindGuid("@ReportId", reportId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionsReportStoreItem>(this.CreatePermissionsReportColumns());
        return resultCollection.GetCurrent<PermissionsReportStoreItem>().FirstOrDefault<PermissionsReportStoreItem>();
      }
    }

    public virtual void UpdatePermissionsReport(
      Guid reportId,
      Guid jobId,
      int jobRetryAttempt,
      int fileReferenceId,
      int reportStatus,
      string error)
    {
      ArgumentUtility.CheckForEmptyGuid(reportId, nameof (reportId));
      this.PrepareStoredProcedure("prc_UpdatePermissionsReport");
      this.BindGuid("@ReportId", reportId);
      this.BindGuid("@JobId", jobId);
      this.BindInt("@JobRetryAttempt", jobRetryAttempt);
      this.BindInt("@FileReferenceId", fileReferenceId);
      this.BindInt("@ReportStatus", reportStatus);
      this.BindString("@Error", error, 1024, false, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    protected virtual ObjectBinder<PermissionsReportStoreItem> CreatePermissionsReportColumns() => (ObjectBinder<PermissionsReportStoreItem>) new PermissionsReportBinder();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PermissionsReportComponent.s_sqlExceptionFactories;
  }
}
