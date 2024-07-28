// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.DataAccess.PermissionsReportStoreItem
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using System;

namespace Microsoft.TeamFoundation.PermissionsReport.DataAccess
{
  public sealed class PermissionsReportStoreItem
  {
    public PermissionsReportStoreItem()
    {
    }

    public PermissionsReportStoreItem(
      Guid reportId,
      string reportName,
      Guid requestorId,
      string reportData)
      : this(reportId, reportName, DateTime.Now, requestorId, Guid.Empty, 0, 0, 0, DateTime.Now, (string) null, reportData)
    {
    }

    public PermissionsReportStoreItem(
      Guid reportId,
      string reportName,
      DateTime requestedTime,
      Guid requestorId,
      Guid jobId,
      int jobRetryAttempt,
      int fileReferenceId,
      int reportStatus,
      DateTime reportStatusLastUpdatedTime,
      string error,
      string reportData)
    {
      this.ReportId = reportId;
      this.ReportName = reportName;
      this.RequestedTime = requestedTime;
      this.RequestorId = requestorId;
      this.JobId = jobId;
      this.JobRetryAttempt = jobRetryAttempt;
      this.FileReferenceId = fileReferenceId;
      this.ReportStatus = reportStatus;
      this.ReportStatusLastUpdatedTime = reportStatusLastUpdatedTime;
      this.Error = error;
      this.ReportData = reportData;
    }

    public Guid ReportId { get; set; }

    public string ReportName { get; set; }

    public DateTime RequestedTime { get; set; }

    public Guid RequestorId { get; set; }

    public Guid JobId { get; set; }

    public int JobRetryAttempt { get; set; }

    public int FileReferenceId { get; set; }

    public int ReportStatus { get; set; }

    public DateTime ReportStatusLastUpdatedTime { get; set; }

    public string Error { get; set; }

    public string ReportData { get; set; }
  }
}
