// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.DataAccess.PermissionsReportBinder
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.PermissionsReport.DataAccess
{
  public class PermissionsReportBinder : ObjectBinder<PermissionsReportStoreItem>
  {
    protected SqlColumnBinder m_ReportId = new SqlColumnBinder("ReportId");
    protected SqlColumnBinder m_ReportName = new SqlColumnBinder("ReportName");
    protected SqlColumnBinder m_RequestedTime = new SqlColumnBinder("RequestedTime");
    protected SqlColumnBinder m_JobId = new SqlColumnBinder("JobId");
    protected SqlColumnBinder m_JobRetryAttempt = new SqlColumnBinder("JobRetryAttempt");
    protected SqlColumnBinder m_FileReferenceId = new SqlColumnBinder("FileReferenceId");
    protected SqlColumnBinder m_ReportStatus = new SqlColumnBinder("ReportStatus");
    protected SqlColumnBinder m_ReportStatusLastUpdatedTime = new SqlColumnBinder("ReportStatusLastUpdatedTime");
    protected SqlColumnBinder m_Error = new SqlColumnBinder("Error");
    protected SqlColumnBinder m_ReportData = new SqlColumnBinder("ReportData");

    protected override PermissionsReportStoreItem Bind() => new PermissionsReportStoreItem(this.m_ReportId.GetGuid((IDataReader) this.Reader, false), this.m_ReportName.GetString((IDataReader) this.Reader, false), this.m_RequestedTime.GetDateTime((IDataReader) this.Reader), Guid.Empty, this.m_JobId.GetGuid((IDataReader) this.Reader, true), this.m_JobRetryAttempt.GetInt32((IDataReader) this.Reader, -1), this.m_FileReferenceId.GetInt32((IDataReader) this.Reader, -1), (int) this.m_ReportStatus.GetInt16((IDataReader) this.Reader, (short) -1), this.m_ReportStatusLastUpdatedTime.GetDateTime((IDataReader) this.Reader), this.m_Error.GetString((IDataReader) this.Reader, true), this.m_ReportData.GetString((IDataReader) this.Reader, true));
  }
}
