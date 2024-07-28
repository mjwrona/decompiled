// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.PermissionsReportExtensions
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.PermissionsReport.Client;
using Microsoft.TeamFoundation.PermissionsReport.DataAccess;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using System;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  public static class PermissionsReportExtensions
  {
    public static Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport ToPermissionsReport(
      this PermissionsReportStoreItem permissionsReportStoreItem,
      IVssRequestContext context)
    {
      return permissionsReportStoreItem == null ? (Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport) null : new Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport(permissionsReportStoreItem.ReportId, permissionsReportStoreItem.ReportName, permissionsReportStoreItem.RequestedTime, PermissionsReportExtensions.GetSubjectDescriptor(context, permissionsReportStoreItem.RequestorId), (PermissionsReportStatus) permissionsReportStoreItem.ReportStatus, permissionsReportStoreItem.ReportStatusLastUpdatedTime, permissionsReportStoreItem.Error);
    }

    public static Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport ToPermissionsReportInternal(
      this PermissionsReportStoreItem permissionsReportStoreItem,
      IVssRequestContext context)
    {
      return permissionsReportStoreItem == null ? (Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport) null : (Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport) new PermissionsReportInternal(permissionsReportStoreItem.ReportId, permissionsReportStoreItem.ReportName, permissionsReportStoreItem.RequestedTime, PermissionsReportExtensions.GetSubjectDescriptor(context, permissionsReportStoreItem.RequestorId), permissionsReportStoreItem.JobId, permissionsReportStoreItem.JobRetryAttempt, permissionsReportStoreItem.FileReferenceId, (PermissionsReportStatus) permissionsReportStoreItem.ReportStatus, permissionsReportStoreItem.ReportStatusLastUpdatedTime, permissionsReportStoreItem.Error, permissionsReportStoreItem.ReportData);
    }

    public static SubjectDescriptor GetSubjectDescriptor(IVssRequestContext context, Guid subjectId)
    {
      context.CheckProjectCollectionRequestContext();
      return context.GetService<IGraphIdentifierConversionService>().GetDescriptorByStorageKey(context, subjectId);
    }
  }
}
