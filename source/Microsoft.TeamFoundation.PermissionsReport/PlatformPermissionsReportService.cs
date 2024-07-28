// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.PlatformPermissionsReportService
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.PermissionsReport.Client;
using Microsoft.TeamFoundation.PermissionsReport.DataAccess;
using Microsoft.TeamFoundation.PermissionsReport.Store;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  public class PlatformPermissionsReportService : IPermissionsReportService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private IPermissionsReportStore m_permissionsReportStore;
    private const string CreatePermissionsReportJobExtension = "Microsoft.TeamFoundation.PermissionsReport.Extensions.CreatePermissionsReportJob";
    private const string DeletePermissionsReportJobExtension = "Microsoft.TeamFoundation.PermissionsReport.Extensions.DeletePermissionsReportJob";
    private const int MaxActiveReportsInProgress = 5;
    private const string MaxActiveReportsInProgressRegistryKey = "/Service/PermissionsReport/Settings/MaxActiveReportsInProgress";
    private const string EnableForAllUsersAndGroups = "Microsoft.TeamFoundation.PermissionsReport.EnableForAllUsersAndGroups";
    private const string EnableForAllResourceTypes = "Microsoft.TeamFoundation.PermissionsReport.EnableForAllResourceTypes";
    private static readonly string s_area = "PermissionsReport";
    private static readonly string s_layer = nameof (PlatformPermissionsReportService);

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
      PermissionsReportStore permissionsReportStore = new PermissionsReportStore();
      permissionsReportStore.Initialize(context);
      this.m_permissionsReportStore = (IPermissionsReportStore) permissionsReportStore;
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport CreatePermissionsReport(
      IVssRequestContext context,
      PermissionsReportRequest permissionsReportRequest)
    {
      this.ValidateRequestContext(context);
      this.CheckPermissions(context);
      ArgumentUtility.CheckForNull<PermissionsReportRequest>(permissionsReportRequest, nameof (permissionsReportRequest));
      ArgumentUtility.CheckForNull<PermissionsReportResource[]>(permissionsReportRequest.Resources, "Resources");
      ArgumentUtility.CheckForNull<string[]>(permissionsReportRequest.Descriptors, "Descriptors");
      if (string.IsNullOrWhiteSpace(permissionsReportRequest.ReportName))
        permissionsReportRequest.ReportName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Permissions Report requested at {0}", (object) DateTime.UtcNow);
      if (!context.IsFeatureEnabled("Microsoft.TeamFoundation.PermissionsReport.EnableForAllUsersAndGroups") && permissionsReportRequest.Descriptors.Length != 1 && !context.IsSystemContext)
        throw new PermissionsReportRequestInvalidException("Permissions report request should contain one Descriptor.");
      if (!context.IsFeatureEnabled("Microsoft.TeamFoundation.PermissionsReport.EnableForAllResourceTypes") && (permissionsReportRequest.Resources.Length != 1 || permissionsReportRequest.Resources[0].ResourceType != ResourceType.Repo && permissionsReportRequest.Resources[0].ResourceType != ResourceType.Ref && permissionsReportRequest.Resources[0].ResourceType != ResourceType.ProjectGit && permissionsReportRequest.Resources[0].ResourceType != ResourceType.Release && permissionsReportRequest.Resources[0].ResourceType != ResourceType.Tfvc))
        throw new PermissionsReportRequestInvalidException("Permissions report request should contain one Resource of type 'Repo', 'Ref', 'Release', 'Tfvc' or 'ProjectGit'.");
      PermissionsReportRequestHelper.ValidateDescriptors(context, permissionsReportRequest.Descriptors);
      PermissionsReportRequestHelper.ValidateResources(context, permissionsReportRequest.Resources);
      context.TraceEnter(34003600, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, nameof (CreatePermissionsReport));
      try
      {
        IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
        int num = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/PermissionsReport/Settings/MaxActiveReportsInProgress", 5);
        if (this.m_permissionsReportStore.GetActivePermissionsReports(context).Count<PermissionsReportStoreItem>() >= num)
          throw new PermissionsReportException(string.Format("You have more than {0} active permissions report job in active state. Please try after some time.", (object) num));
        string reportData = JsonConvert.SerializeObject((object) permissionsReportRequest);
        Guid reportId = Guid.NewGuid();
        Guid id = context.GetUserIdentity().Id;
        PermissionsReportStoreItem permissionsReportStoreItem = new PermissionsReportStoreItem(reportId, permissionsReportRequest.ReportName, id, reportData);
        Guid permissionsReportJob = PlatformPermissionsReportService.QueueCreatePermissionsReportJob(context, reportId);
        permissionsReportStoreItem.JobId = permissionsReportJob;
        context.Trace(34003611, TraceLevel.Info, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, string.Format("Create Permissions Report called with id:{0}, jobId: {1}, name:{2}, data:{3}", (object) reportId, (object) permissionsReportJob, (object) permissionsReportRequest.ReportName, (object) reportData));
        return this.m_permissionsReportStore.AddPermissionsReport(context, permissionsReportStoreItem).ToPermissionsReport(context);
      }
      finally
      {
        context.TraceLeave(34003601, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, nameof (CreatePermissionsReport));
      }
    }

    public void QueuePermissionsReportDeletion(IVssRequestContext context, Guid reportId)
    {
      Guid guid = PlatformPermissionsReportService.QueueDeletePermissionsReportJob(context, reportId);
      context.Trace(34003611, TraceLevel.Info, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, string.Format("Delete Permissions Report called with id:{0}, jobId: {1}", (object) reportId, (object) guid));
    }

    public IEnumerable<Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport> GetPermissionsReports(
      IVssRequestContext context)
    {
      this.ValidateRequestContext(context);
      this.CheckPermissions(context);
      context.TraceEnter(34003602, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, nameof (GetPermissionsReports));
      try
      {
        return this.m_permissionsReportStore.GetPermissionsReports(context).Select<PermissionsReportStoreItem, Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport>((Func<PermissionsReportStoreItem, Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport>) (item => item.ToPermissionsReport(context)));
      }
      finally
      {
        context.TraceLeave(34003603, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, nameof (GetPermissionsReports));
      }
    }

    public Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport GetPermissionsReportByReportId(
      IVssRequestContext context,
      Guid reportId,
      bool includeDetails = false)
    {
      this.ValidateRequestContext(context);
      this.CheckPermissions(context);
      ArgumentUtility.CheckForEmptyGuid(reportId, nameof (reportId));
      context.TraceEnter(34003604, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, nameof (GetPermissionsReportByReportId));
      try
      {
        PermissionsReportStoreItem reportByReportId = this.m_permissionsReportStore.GetPermissionsReportByReportId(context, reportId);
        return includeDetails ? reportByReportId.ToPermissionsReportInternal(context) : reportByReportId.ToPermissionsReport(context);
      }
      finally
      {
        context.TraceLeave(34003605, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, nameof (GetPermissionsReportByReportId));
      }
    }

    public Stream GetPermissionsReportForDownload(
      IVssRequestContext context,
      Guid reportId,
      out CompressionType commpressionType)
    {
      this.ValidateRequestContext(context);
      this.CheckPermissions(context);
      ArgumentUtility.CheckForEmptyGuid(reportId, nameof (reportId));
      context.TraceEnter(34003608, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, nameof (GetPermissionsReportForDownload));
      try
      {
        commpressionType = CompressionType.None;
        Stream reportForDownload = (Stream) null;
        PermissionsReportInternal permissionsReportInternal = (PermissionsReportInternal) this.m_permissionsReportStore.GetPermissionsReportByReportId(context, reportId).ToPermissionsReportInternal(context);
        if ((permissionsReportInternal.ReportStatus == PermissionsReportStatus.CompletedSuccessfully || permissionsReportInternal.ReportStatus == PermissionsReportStatus.CompletedWithErrors) && permissionsReportInternal.FileReferenceId > 0)
          reportForDownload = context.GetService<ITeamFoundationFileService>().RetrieveFile(context, (long) permissionsReportInternal.FileReferenceId, out commpressionType);
        if (reportForDownload == null)
        {
          context.Trace(34003611, TraceLevel.Info, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, "File service returned null.");
          throw new PermissionsReportDownloadNotAvailableException("Permissions report is not available for download.");
        }
        return reportForDownload;
      }
      catch (FileIdNotFoundException ex)
      {
        context.TraceException(34003610, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, (Exception) ex);
        throw new PermissionsReportDownloadNotAvailableException("Unable to retrieve file for download.");
      }
      finally
      {
        context.TraceLeave(34003609, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, nameof (GetPermissionsReportForDownload));
      }
    }

    public void UpdatePermissionsReport(
      IVssRequestContext context,
      Guid reportId,
      Guid? jobId,
      int? jobRetryAttempt,
      int? fileReferenceId,
      PermissionsReportStatus? reportStatus,
      string error)
    {
      this.ValidateRequestContext(context);
      this.CheckPermissions(context);
      ArgumentUtility.CheckForEmptyGuid(reportId, nameof (reportId));
      context.TraceEnter(34003606, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, "GetPermissionsReportByReportId");
      try
      {
        PermissionsReportStoreItem reportByReportId = this.m_permissionsReportStore.GetPermissionsReportByReportId(context, reportId);
        context.Trace(34003611, TraceLevel.Info, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, string.Format("Update permissions report resource called with id:'{0}', jobId:'{1}', jobRetryAttempt:'{2}', fileReferenceId:'{3}', reportStatus:'{4}', error:'{5}'.", (object) reportId, (object) jobId, (object) jobRetryAttempt, (object) fileReferenceId, (object) reportStatus, (object) error));
        IPermissionsReportStore permissionsReportStore = this.m_permissionsReportStore;
        IVssRequestContext context1 = context;
        Guid reportId1 = reportId;
        Guid jobId1 = jobId ?? reportByReportId.JobId;
        int? nullable = jobRetryAttempt;
        int jobRetryAttempt1 = nullable ?? reportByReportId.JobRetryAttempt;
        nullable = fileReferenceId;
        int fileReferenceId1 = nullable ?? reportByReportId.FileReferenceId;
        int reportStatus1 = reportStatus.HasValue ? (int) reportStatus.Value : reportByReportId.ReportStatus;
        string error1 = (reportByReportId.Error + Environment.NewLine + error).Trim();
        permissionsReportStore.UpdatePermissionsReport(context1, reportId1, jobId1, jobRetryAttempt1, fileReferenceId1, reportStatus1, error1);
      }
      finally
      {
        context.TraceLeave(34003607, PlatformPermissionsReportService.s_area, PlatformPermissionsReportService.s_layer, "GetPermissionsReportByReportId");
      }
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private void CheckPermissions(IVssRequestContext context)
    {
      if (!context.IsSystemContext && !context.GetService<TeamFoundationIdentityService>().IsMember(context, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, context.UserContext))
        throw new AccessCheckException(string.Format("The requester '{0}' does not have permissions to perform this operation.", (object) context.UserContext));
    }

    private static Guid QueueCreatePermissionsReportJob(IVssRequestContext context, Guid reportId)
    {
      ITeamFoundationJobService service = context.GetService<ITeamFoundationJobService>();
      GeneratePermissionsReportJobData objectToSerialize = new GeneratePermissionsReportJobData(reportId);
      IVssRequestContext requestContext = context;
      string jobName = string.Format("Create PermissionsReport {0}", (object) reportId);
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize);
      return service.QueueOneTimeJob(requestContext, jobName, "Microsoft.TeamFoundation.PermissionsReport.Extensions.CreatePermissionsReportJob", xml, false);
    }

    private static Guid QueueDeletePermissionsReportJob(IVssRequestContext context, Guid reportId)
    {
      ITeamFoundationJobService service = context.GetService<ITeamFoundationJobService>();
      GeneratePermissionsReportJobData objectToSerialize = new GeneratePermissionsReportJobData(reportId);
      IVssRequestContext requestContext = context;
      string jobName = string.Format("Delete PermissionsReport {0}", (object) reportId);
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize);
      TimeSpan startOffset = TimeSpan.FromDays(28.0);
      return service.QueueOneTimeJob(requestContext, jobName, "Microsoft.TeamFoundation.PermissionsReport.Extensions.DeletePermissionsReportJob", xml, startOffset);
    }
  }
}
