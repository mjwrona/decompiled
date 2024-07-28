// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.IPermissionsReportService
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.PermissionsReport.Client;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  [DefaultServiceImplementation(typeof (PlatformPermissionsReportService))]
  public interface IPermissionsReportService : IVssFrameworkService
  {
    Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport CreatePermissionsReport(
      IVssRequestContext context,
      PermissionsReportRequest permissionsReportRequest);

    void QueuePermissionsReportDeletion(IVssRequestContext context, Guid reportId);

    IEnumerable<Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport> GetPermissionsReports(
      IVssRequestContext context);

    Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport GetPermissionsReportByReportId(
      IVssRequestContext context,
      Guid reportId,
      bool includeDetails = false);

    void UpdatePermissionsReport(
      IVssRequestContext context,
      Guid reportId,
      Guid? jobId,
      int? jobRetryAttempt,
      int? fileReferenceId,
      PermissionsReportStatus? reportStatus,
      string error);

    Stream GetPermissionsReportForDownload(
      IVssRequestContext context,
      Guid reportId,
      out CompressionType commpressionType);
  }
}
