// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Store.IPermissionsReportStore
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.PermissionsReport.DataAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.PermissionsReport.Store
{
  internal interface IPermissionsReportStore
  {
    PermissionsReportStoreItem AddPermissionsReport(
      IVssRequestContext context,
      PermissionsReportStoreItem permissionsReportStoreItem);

    IEnumerable<PermissionsReportStoreItem> GetPermissionsReports(IVssRequestContext context);

    IEnumerable<PermissionsReportStoreItem> GetActivePermissionsReports(IVssRequestContext context);

    PermissionsReportStoreItem GetPermissionsReportByReportId(
      IVssRequestContext context,
      Guid reportId);

    void UpdatePermissionsReport(
      IVssRequestContext context,
      Guid reportId,
      Guid jobId,
      int jobRetryAttempt,
      int fileReferenceId,
      int reportStatus,
      string error);
  }
}
