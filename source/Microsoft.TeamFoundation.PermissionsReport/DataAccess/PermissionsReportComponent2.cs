// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.DataAccess.PermissionsReportComponent2
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.PermissionsReport.DataAccess
{
  public class PermissionsReportComponent2 : PermissionsReportComponent
  {
    public override PermissionsReportStoreItem CreatePermissionsReport(
      PermissionsReportStoreItem permissionsReportStoreItem)
    {
      ArgumentUtility.CheckForEmptyGuid(permissionsReportStoreItem.ReportId, "ReportId");
      ArgumentUtility.CheckStringForNullOrEmpty(permissionsReportStoreItem.ReportName, "ReportName");
      ArgumentUtility.CheckStringForNullOrEmpty(permissionsReportStoreItem.ReportData, "ReportData");
      ArgumentUtility.CheckForEmptyGuid(permissionsReportStoreItem.RequestorId, "RequestorId");
      this.PrepareStoredProcedure("prc_CreatePermissionsReport");
      this.BindGuid("@ReportId", permissionsReportStoreItem.ReportId);
      this.BindGuid("@RequestorId", permissionsReportStoreItem.RequestorId);
      this.BindGuid("@JobId", permissionsReportStoreItem.JobId);
      this.BindString("@ReportName", permissionsReportStoreItem.ReportName, 1024, false, SqlDbType.VarChar);
      this.BindString("@ReportData", permissionsReportStoreItem.ReportData, 4096, false, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionsReportStoreItem>(this.CreatePermissionsReportColumns());
        return resultCollection.GetCurrent<PermissionsReportStoreItem>().FirstOrDefault<PermissionsReportStoreItem>();
      }
    }

    protected override ObjectBinder<PermissionsReportStoreItem> CreatePermissionsReportColumns() => (ObjectBinder<PermissionsReportStoreItem>) new PermissionsReportBinder2();
  }
}
