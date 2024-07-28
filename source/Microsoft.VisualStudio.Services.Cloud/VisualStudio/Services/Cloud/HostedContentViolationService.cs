// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostedContentViolationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Ops.Avert.Client;
using Microsoft.Ops.Avert.Client.DataContracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ContentValidation.Client;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal sealed class HostedContentViolationService : 
    IContentViolationService,
    IVssFrameworkService
  {
    private HostedContentViolationService.ContentViolationReporter m_reporter;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_reporter?.Dispose();
      this.m_reporter = (HostedContentViolationService.ContentViolationReporter) null;
    }

    public bool IsEnabled(IVssRequestContext requestContext) => true;

    public Task<ContentViolationReportResult> ReportAsync(
      IVssRequestContext requestContext,
      ContentViolationReport violationReport)
    {
      if (this.IsEnabled(requestContext))
      {
        this.CheckPermission(requestContext);
        if (this.m_reporter == null)
          this.SetupReporter(requestContext);
        return this.m_reporter.ReportViolationAsync(requestContext, violationReport);
      }
      return Task.FromResult<ContentViolationReportResult>(new ContentViolationReportResult()
      {
        Id = "00000000-0000-0000-0000-000000000000",
        Status = ContentViolationReportResultStatus.Unset
      });
    }

    private void CheckPermission(IVssRequestContext requestContext)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ContentValidationSecurityConstants.NamespaceId);
      ArgumentUtility.CheckForNull<IVssSecurityNamespace>(securityNamespace, "contentValidationSecurityNamespace");
      securityNamespace.CheckPermission(requestContext, ContentValidationSecurityConstants.ViolationsToken, 2, false);
    }

    private void SetupReporter(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.m_reporter = new HostedContentViolationService.ContentViolationReporter(vssRequestContext, vssRequestContext.GetService<ILocationService>(), vssRequestContext.GetService<IVssRegistryService>(), vssRequestContext.GetService<ITeamFoundationStrongBoxService>(), (HttpMessageHandler) new HttpClientHandler(), 1000);
    }

    internal class ContentViolationReporter : IDisposable
    {
      private AvertClient m_avertClient;
      private Uri m_callbackEndpoint;
      private string m_callbackEmail;
      private bool m_disposed;
      private readonly ITeamFoundationStrongBoxService m_strongBoxService;
      private readonly IVssRegistryService m_registryService;
      private readonly HttpMessageHandler m_httpMessageHandler;
      private readonly int m_failedRetryWaitMs;
      private const string c_area = "HostedContentViolationService";

      public ContentViolationReporter(
        IVssRequestContext deploymentRequestContext,
        ILocationService locationService,
        IVssRegistryService registryService,
        ITeamFoundationStrongBoxService strongBoxService,
        HttpMessageHandler httpMessageHandler,
        int failedRetryWaitMs)
      {
        this.m_registryService = registryService;
        this.m_strongBoxService = strongBoxService;
        this.m_httpMessageHandler = httpMessageHandler;
        this.m_failedRetryWaitMs = failedRetryWaitMs;
        this.m_callbackEndpoint = registryService.GetValue<Uri>(deploymentRequestContext, in HostedContentValidationConstants.AvertCallbackEndpointQuery, (Uri) null);
        if (this.m_callbackEndpoint == (Uri) null || !this.m_callbackEndpoint.IsAbsoluteUri)
        {
          this.m_callbackEndpoint = locationService.GetResourceUri(deploymentRequestContext, "contentValidation", ContentValidationResourceIds.AvertCallbackLocationId, (object) null, false, false);
          this.m_callbackEndpoint = this.m_callbackEndpoint.AppendQuery((IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
          {
            ["api-version"] = "4.1-preview"
          });
        }
        this.m_callbackEmail = registryService.GetValue<string>(deploymentRequestContext, in HostedContentValidationConstants.CvsContactNotificationEmailQuery, (string) null);
      }

      public async Task<ContentViolationReportResult> ReportViolationAsync(
        IVssRequestContext requestContext,
        ContentViolationReport violationReport)
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        this.CheckNotDisposed();
        ArgumentUtility.CheckForNull<ContentViolationReport>(violationReport, nameof (violationReport));
        ArgumentUtility.CheckForEmptyGuid(violationReport.HostId, "violationReport.HostId");
        ArgumentUtility.CheckForEmptyGuid(violationReport.ContainerId, "violationReport.ContainerId");
        ArgumentUtility.CheckStringForNullOrEmpty(violationReport.ContentUrl, "violationReport.ContentUrl");
        if (!Uri.TryCreate(violationReport.ContentUrl, UriKind.Absolute, out Uri _))
          throw new ArgumentException("A valid absolute URL needs to be specified for ContentUrl");
        if (this.m_avertClient == null)
          this.SetupAvertClient(requestContext);
        AvertInboundModel request = this.TranslateReport(requestContext, violationReport);
        ContentViolationReportResult reportResult = (ContentViolationReportResult) null;
        int currentAttempt = 1;
        while (currentAttempt <= 2)
        {
          object obj;
          int num;
          try
          {
            AvertOutboundModel avertOutboundModel = await this.m_avertClient.SubmitAsync(request);
            if (avertOutboundModel == null)
            {
              requestContext.TraceAlways(15292001, TraceLevel.Error, nameof (HostedContentViolationService), nameof (ReportViolationAsync), "Attempt {0}/{1}: Violation report submission failed since the return result is null.", (object) currentAttempt, (object) 2);
              throw new ArgumentNullException("result");
            }
            requestContext.Trace(15292002, TraceLevel.Info, nameof (HostedContentViolationService), nameof (ReportViolationAsync), "Violation report submitted on attempt {0}. Avert ID: {1}", (object) currentAttempt, (object) avertOutboundModel.AvertId);
            CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
            CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
            intelligenceData.Add("ContentUrl", violationReport.ContentUrl);
            intelligenceData.Add("Notes", violationReport.AdditionalDetails);
            intelligenceData.Add("CallbackUrl", (object) request.CallbackUrl);
            intelligenceData.Add("AvertId", avertOutboundModel.AvertId);
            intelligenceData.Add("AvertStatus", (object) avertOutboundModel.Status);
            IVssRequestContext requestContext1 = requestContext;
            CustomerIntelligenceData properties = intelligenceData;
            service.Publish(requestContext1, "ContentValidation", "SubmitAbuseReport", properties);
            reportResult = new ContentViolationReportResult()
            {
              Id = avertOutboundModel.AvertId,
              Status = this.TranslateResultStatus(avertOutboundModel.Status)
            };
            break;
          }
          catch (AvertClientException ex)
          {
            obj = (object) ex;
            num = 1;
          }
          catch (Exception ex)
          {
            this.TraceException(requestContext, ex);
            throw;
          }
          if (num == 1)
          {
            AvertClientException ex = (AvertClientException) obj;
            bool flag = false;
            if (ex.StatusCode == HttpStatusCode.Unauthorized && currentAttempt < 2)
            {
              requestContext.TraceAlways(15292003, TraceLevel.Error, nameof (HostedContentViolationService), nameof (ReportViolationAsync), "First attempt failed during violation report submission: {0}\nWaiting {1}ms before retry...", (object) ex, (object) this.m_failedRetryWaitMs);
              await Task.Delay(this.m_failedRetryWaitMs);
              this.SetupAvertClient(requestContext);
              ++currentAttempt;
              flag = true;
            }
            if (!flag)
            {
              this.TraceException(requestContext, (Exception) ex);
              if (!(obj is Exception source))
                throw obj;
              ExceptionDispatchInfo.Capture(source).Throw();
            }
            ex = (AvertClientException) null;
          }
          obj = (object) null;
        }
        ContentViolationReportResult violationReportResult = reportResult;
        request = (AvertInboundModel) null;
        reportResult = (ContentViolationReportResult) null;
        return violationReportResult;
      }

      private void TraceException(IVssRequestContext requestContext, Exception ex) => requestContext.TraceAlways(15292004, TraceLevel.Error, nameof (HostedContentViolationService), "ReportViolationAsync", "Failure during violation report submission: {0}", (object) ex);

      private void SetupAvertClient(IVssRequestContext requestContext)
      {
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        string uriString = this.m_registryService.GetValue<string>(requestContext1, in HostedContentValidationConstants.AvertApiEndpointQuery, (string) null);
        if (uriString == null)
          throw new InvalidConfigurationException("Registry setting for avertEndpoint missing.");
        IVssRequestContext requestContext2 = requestContext1.Elevate();
        StrongBoxItemInfo itemInfo = this.m_strongBoxService.GetItemInfo(requestContext2, "ConfigurationSecrets", "AvertAPIMKey", true);
        string str = this.m_strongBoxService.GetString(requestContext2, itemInfo);
        this.m_avertClient = new AvertClient(new Uri(uriString), str, 3, this.m_httpMessageHandler);
      }

      private AvertInboundModel TranslateReport(
        IVssRequestContext requestContext,
        ContentViolationReport report)
      {
        ArgumentUtility.CheckForNull<ContentViolationReport>(report, nameof (report));
        ArgumentUtility.CheckForNull<string>(report.ContentUrl, "report." + report.ContentUrl);
        ReportInfo reportInfo = new ReportInfo()
        {
          ReporterIPAddress = requestContext.RemoteIPAddress(),
          ReporterSubmissionEventTime = new DateTimeOffset?(DateTimeOffset.UtcNow)
        };
        ContentItemInbound contentItemInbound = new ContentItemInbound()
        {
          ExternalId = "Content Uri: " + report.ContentUrl + " | E2EID: " + requestContext.E2EId.ToString(),
          ContentType = (ContentType) 2,
          ContentRepresentation = (ContentRepresentation) 1,
          Value = report.ContentUrl,
          ReportInfo = reportInfo
        };
        return new AvertInboundModel()
        {
          AbuseCategory = this.TranslateCategory(report.ViolationCategory),
          CallbackUrl = this.m_callbackEndpoint.AppendQuery("hostId", report.HostId.ToString()).AppendQuery("containerId", report.ContainerId.ToString()),
          CallbackEmail = this.m_callbackEmail,
          Notes = report.AdditionalDetails,
          ContentItems = (IEnumerable<ContentItemInbound>) new List<ContentItemInbound>()
          {
            contentItemInbound
          }
        };
      }

      private AbuseCategory TranslateCategory(ContentViolationCategory violationCategory)
      {
        switch (violationCategory)
        {
          case ContentViolationCategory.Child:
            return (AbuseCategory) 1;
          case ContentViolationCategory.Nudity:
            return (AbuseCategory) 3;
          case ContentViolationCategory.Harassment:
            return (AbuseCategory) 5;
          case ContentViolationCategory.Spam:
            return (AbuseCategory) 6;
          case ContentViolationCategory.Other:
            return (AbuseCategory) 7;
          default:
            throw new ArgumentOutOfRangeException(string.Format("Violation category '{0}' is unsupported.", (object) violationCategory));
        }
      }

      private ContentViolationReportResultStatus TranslateResultStatus(ProcessingStatus status)
      {
        if (status == 1)
          return ContentViolationReportResultStatus.InProgress;
        return status == 2 ? ContentViolationReportResultStatus.Succeeded : ContentViolationReportResultStatus.Unset;
      }

      public void Dispose()
      {
        if (!this.m_disposed)
        {
          this.m_avertClient.Dispose();
          this.m_avertClient = (AvertClient) null;
        }
        this.m_disposed = true;
      }

      private void CheckNotDisposed()
      {
        if (this.m_disposed)
          throw new ObjectDisposedException("m_avertClient");
      }
    }
  }
}
