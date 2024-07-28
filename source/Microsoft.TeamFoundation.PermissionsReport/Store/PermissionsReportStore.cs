// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Store.PermissionsReportStore
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.PermissionsReport.DataAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.PermissionsReport.Store
{
  public class PermissionsReportStore : IPermissionsReportStore
  {
    private const string c_area = "PermissionsReport";
    private const string c_layer = "PermissionsReportStore";

    public void Initialize(IVssRequestContext context)
    {
      using (PermissionsReportComponent component = PermissionsReportStore.CreateComponent(context))
        component.Initialize();
    }

    public PermissionsReportStoreItem AddPermissionsReport(
      IVssRequestContext context,
      PermissionsReportStoreItem permissionsReportStoreItem)
    {
      using (PermissionsReportComponent component = PermissionsReportStore.CreateComponent(context))
        return component.CreatePermissionsReport(permissionsReportStoreItem);
    }

    public IEnumerable<PermissionsReportStoreItem> GetPermissionsReports(IVssRequestContext context)
    {
      using (PermissionsReportComponent component = PermissionsReportStore.CreateComponent(context))
        return component.GetPermissionsReports();
    }

    public IEnumerable<PermissionsReportStoreItem> GetActivePermissionsReports(
      IVssRequestContext context)
    {
      using (PermissionsReportComponent component = PermissionsReportStore.CreateComponent(context))
        return component.GetActivePermissionsReports();
    }

    public PermissionsReportStoreItem GetPermissionsReportByReportId(
      IVssRequestContext context,
      Guid reportId)
    {
      using (PermissionsReportComponent component = PermissionsReportStore.CreateComponent(context))
        return component.GetPermissionsReportByReportId(reportId);
    }

    public void UpdatePermissionsReport(
      IVssRequestContext context,
      Guid reportId,
      Guid jobId,
      int jobRetryAttempt,
      int fileReferenceId,
      int reportStatus,
      string error)
    {
      using (PermissionsReportComponent component = PermissionsReportStore.CreateComponent(context))
        component.UpdatePermissionsReport(reportId, jobId, jobRetryAttempt, fileReferenceId, reportStatus, error);
    }

    private static PermissionsReportComponent CreateComponent(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      PermissionsReportComponent component = context.CreateComponent<PermissionsReportComponent>();
      component.Initialize();
      return component;
    }
  }
}
