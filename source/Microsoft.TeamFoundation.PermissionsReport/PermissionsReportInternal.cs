// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.PermissionsReportInternal
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.PermissionsReport.Client;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  public sealed class PermissionsReportInternal : Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport
  {
    public PermissionsReportInternal(
      Guid reportId,
      string reportName,
      DateTime requestedTime,
      SubjectDescriptor requestor,
      Guid jobId,
      int jobRetryAttempt,
      int fileReferenceId,
      PermissionsReportStatus reportStatus,
      DateTime reportStatusLastUpdatedTime,
      string error,
      string reportData)
      : base(reportId, reportName, requestedTime, requestor, reportStatus, reportStatusLastUpdatedTime, error)
    {
      this.JobId = jobId;
      this.JobRetryAttempt = jobRetryAttempt;
      this.FileReferenceId = fileReferenceId;
      this.ReportData = reportData;
    }

    public Guid JobId { get; private set; }

    public int JobRetryAttempt { get; private set; }

    public int FileReferenceId { get; private set; }

    public string ReportData { get; private set; }
  }
}
