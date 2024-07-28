// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ContentValidation.ContentValidator
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Ops.Cvs.Client;
using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.ContentValidation
{
  internal class ContentValidator : IDisposable
  {
    private bool m_disposed;
    private CvsClient m_cvsClient;
    private WebRequestHandler m_requestHandler;
    private readonly ITeamFoundationStrongBoxService m_strongBox;
    private readonly IVssRegistryService m_registry;
    private readonly IInstanceManagementService m_instMgmt;
    private readonly Func<WebRequestHandler> m_webRequestHandlerFactory;
    private readonly int m_failedRetryWaitMs;
    private readonly AsyncReaderWriterLock m_clientLock;
    private readonly Uri m_deploymentCallbackEndpoint;
    private readonly string m_contactEmail;
    private static readonly CommandSetter s_breakerCommandSetter = new CommandSetter((CommandGroupKey) "Framework.").AndCommandKey(new CommandKey(typeof (CvsClient))).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(TimeSpan.FromSeconds(30.0)).WithExecutionMaxConcurrentRequests(50));
    private static readonly VssPerformanceCounter s_uriSubmissionsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.ContentValidation.PerfCounters.TotalUriSubmissions");
    private static readonly VssPerformanceCounter s_inlineSubmissionsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.ContentValidation.PerfCounters.TotalInlineSubmissions");
    private static readonly VssPerformanceCounter s_totalRequestsCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Cloud.ContentValidation.PerfCounters.TotalRequestsSent");
    private const int c_maxBytesPerCvsRequest = 4000000;
    private const string c_area = "HostedContentValidationService";

    public ContentValidator(
      IVssRequestContext deploymentRequestContext,
      ILocationService locationSvc,
      IVssRegistryService registry,
      ITeamFoundationStrongBoxService strongBox,
      IInstanceManagementService instMgmt,
      Func<WebRequestHandler> webRequestHandlerFactory,
      int failedRetryWaitMs)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      this.m_strongBox = strongBox;
      this.m_registry = registry;
      this.m_instMgmt = instMgmt;
      this.m_webRequestHandlerFactory = webRequestHandlerFactory;
      this.m_failedRetryWaitMs = failedRetryWaitMs;
      this.m_clientLock = new AsyncReaderWriterLock();
      this.m_contactEmail = this.m_registry.GetValue<string>(deploymentRequestContext, in HostedContentValidationConstants.CvsContactNotificationEmailQuery, (string) null);
      this.m_deploymentCallbackEndpoint = this.m_registry.GetValue<Uri>(deploymentRequestContext, in HostedContentValidationConstants.CvsCallbackEndpointQuery, (Uri) null);
      if (this.m_deploymentCallbackEndpoint == (Uri) null || !this.m_deploymentCallbackEndpoint.IsAbsoluteUri)
      {
        try
        {
          this.m_deploymentCallbackEndpoint = locationSvc.GetResourceUri(deploymentRequestContext, "contentValidation", ContentValidationResourceIds.CvsCallbackLocationId, (object) null, false, false);
        }
        catch (Exception ex) when (!deploymentRequestContext.ServiceHost.IsProduction)
        {
          deploymentRequestContext.TraceAlways(15289016, TraceLevel.Error, "HostedContentValidationService", "ctor", "Couldn't find CVS API endpoint, using default access mapping: {0}", (object) ex);
          this.m_deploymentCallbackEndpoint = new Uri(locationSvc.GetDefaultAccessMapping(deploymentRequestContext).AccessPoint);
          this.m_deploymentCallbackEndpoint = VssHttpUriUtility.ConcatUri(this.m_deploymentCallbackEndpoint, HttpRouteCollectionExtensions.DefaultRoutePrefix + "/cvsCallback");
        }
      }
      this.m_deploymentCallbackEndpoint = this.m_deploymentCallbackEndpoint.AppendQuery("api-version", "4.1-preview");
    }

    public void Dispose()
    {
      if (!this.m_disposed)
      {
        this.m_cvsClient?.Dispose();
        this.m_cvsClient = (CvsClient) null;
        this.m_requestHandler?.Dispose();
        this.m_requestHandler = (WebRequestHandler) null;
      }
      this.m_disposed = true;
    }

    public Task SubmitAsync(
      IVssRequestContext rc,
      Guid projectId,
      IEnumerable<ContentValidationKey> toScan,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress)
    {
      this.CheckNotDisposed();
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<IEnumerable<ContentValidationKey>>(toScan, nameof (toScan));
      creatorIpAddress = creatorIpAddress ?? rc.RemoteIPAddress();
      contentCreator = contentCreator ?? rc.GetUserIdentity();
      IEnumerable<ContentItem> contentItems = this.TranslateToContentItems(rc, contentCreator, creatorIpAddress, toScan);
      return this.SendContentItemsInBatches(rc, new Guid?(projectId), contentItems, ContentValidationTakedownTarget.AllProjectsInCollection);
    }

    public Task SubmitAsync(
      IVssRequestContext rc,
      Guid projectId,
      byte[] rawContent,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress,
      string fileName,
      ContentValidationScanType? scanType)
    {
      this.CheckNotDisposed();
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<byte[]>(rawContent, nameof (rawContent));
      long num = (long) ((rawContent.Length + 2) / 3 * 4);
      if (num > 4000000L)
      {
        rc.TraceAlways(15289014, TraceLevel.Error, "HostedContentValidationService", nameof (SubmitAsync), "Estimated base64 string length ({0}) exceeds CVS's limit ({1}). Skipping submission.", (object) num, (object) 4000000);
        return Task.CompletedTask;
      }
      if (rawContent.Length < MimeMapper.MinMimeDetectHeaderBytes)
        return Task.CompletedTask;
      if (!scanType.HasValue)
        scanType = !string.IsNullOrWhiteSpace(fileName) ? new ContentValidationScanType?(ContentValidationUtil.GetScanTypeFromFileName(fileName)) : new ContentValidationScanType?(ContentValidationUtil.DetectScanTypeFromFileHeader((IReadOnlyCollection<byte>) new ArraySegment<byte>(rawContent, 0, Math.Min(MimeMapper.SuggestedMimeDetectHeaderBytes, rawContent.Length))));
      return scanType.Value != ContentValidationScanType.None ? this.SubmitAsync(rc, new Guid?(projectId), Convert.ToBase64String(rawContent, Base64FormattingOptions.None), scanType.Value, fileName, contentCreator, creatorIpAddress, ContentValidationTakedownTarget.AllProjectsInCollection) : Task.CompletedTask;
    }

    public Task SubmitAsync(
      IVssRequestContext rc,
      Guid? projectId,
      string base64EncodedContent,
      ContentValidationScanType scanType,
      string fileName,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress,
      ContentValidationTakedownTarget takedownTarget)
    {
      this.CheckNotDisposed();
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      if (projectId.HasValue)
        ArgumentUtility.CheckForEmptyGuid(projectId.Value, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(base64EncodedContent, nameof (base64EncodedContent));
      int length = base64EncodedContent.Length;
      if (length > 4000000)
      {
        rc.TraceAlways(15289014, TraceLevel.Error, "HostedContentValidationService", nameof (SubmitAsync), "Base64 string length ({0}) exceeds CVS's limit ({1}). Skipping submission.", (object) length, (object) 4000000);
        return Task.CompletedTask;
      }
      if (scanType == ContentValidationScanType.None)
        return Task.CompletedTask;
      contentCreator = contentCreator ?? rc.GetUserIdentity();
      creatorIpAddress = creatorIpAddress ?? rc.RemoteIPAddress();
      ContentType contentType;
      switch (scanType)
      {
        case ContentValidationScanType.Image:
          contentType = (ContentType) 3;
          break;
        case ContentValidationScanType.Video:
          contentType = (ContentType) 4;
          break;
        default:
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Invalid scan type: {0}", (object) scanType)));
      }
      ContentValidationExternalId validationExternalId = new ContentValidationExternalId()
      {
        FileName = fileName,
        E2EID = rc.E2EId
      };
      ContentItem contentItem = new ContentItem()
      {
        ExternalId = JsonConvert.SerializeObject((object) validationExternalId),
        ContentType = contentType,
        ReporteeName = ContentValidator.GetReportableIdentityString(contentCreator),
        MsaPuidHex = ContentValidator.GetPuidIfMsa(contentCreator),
        ReporteeAddress = creatorIpAddress,
        IncidentTime = new DateTime?(DateTime.UtcNow),
        Representation = (ContentRepresentation) 1,
        Value = base64EncodedContent
      };
      ContentValidator.s_inlineSubmissionsCounter.Increment();
      return this.SendContentItemsInBatches(rc, projectId, (IEnumerable<ContentItem>) new ContentItem[1]
      {
        contentItem
      }, takedownTarget);
    }

    public async Task GetJobResultSummaryAsync(
      IVssRequestContext rc,
      string jobId,
      Action<string> logInfo,
      Action<string> logError)
    {
      if (this.m_cvsClient == null)
      {
        logInfo("Setting up CVS client...");
        await this.SetupCvsClient(rc, (CvsClient) null);
      }
      logInfo("Retrieving job " + jobId + "...");
      Job job;
      using (await this.m_clientLock.ReaderLockAsync())
        job = await this.m_cvsClient.GetJobAsync(jobId).ConfigureAwait(false);
      logInfo(string.Format("Job {0} status: {1}", (object) ((BaseResource) job).Id, (object) job.Status));
      foreach (string str in job.SerializeChunkedWithPiiSanitized(rc.RequestTracer))
        logInfo(str);
      switch (job.Status - 1)
      {
        case 0:
        case 1:
          logInfo("No action required.");
          return;
        case 2:
          ProviderResult determination1 = job.Determination;
          if ((determination1 != null ? (determination1.PolicyViolationCount == 0 ? 1 : 0) : 0) == 0)
          {
            ProviderResult determination2 = job.Determination;
            if ((determination2 != null ? (determination2.PolicyViolationCount > 0 ? 1 : 0) : 0) == 0)
              break;
            goto case 3;
          }
          else
            goto case 0;
        case 3:
        case 4:
          logError("! ACTION POSSIBLY REQUIRED !");
          IEnumerable<ProviderResult> providerResults = job.ProviderResults;
          List<ProviderResult> list = providerResults != null ? providerResults.ToList<ProviderResult>() : (List<ProviderResult>) null;
          // ISSUE: explicit non-virtual call
          if (list != null && __nonvirtual (list.Count) > 0 && list.Any<ProviderResult>((Func<ProviderResult, bool>) (pr => pr.PolicyViolationCount > 0)))
          {
            logError("!!!! Policy violations found !!!!");
          }
          else
          {
            logError("The job didn't succeed. Faults:");
            foreach (string faultDescription in job.ExtractUniqueFaultDescriptions())
              logError(faultDescription);
          }
          logError("! ACTION POSSIBLY REQUIRED !");
          return;
      }
      throw new InvalidOperationException(string.Format("Job status {0} is unknown.", (object) job.Status));
    }

    private void CheckNotDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException("m_cvsClient");
    }

    private ProcessingConfiguration GetProcessingConfiguration(
      Guid sourceHostId,
      Guid? sourceProjectId,
      ContentValidationTakedownTarget takedownTarget)
    {
      Uri uri = this.m_deploymentCallbackEndpoint.AppendQuery(nameof (sourceHostId), sourceHostId.ToString()).AppendQuery("takedownArtifact", takedownTarget.ToString());
      if (sourceProjectId.HasValue)
        uri = uri.AppendQuery(nameof (sourceProjectId), sourceProjectId.Value.ToString());
      return new ProcessingConfiguration()
      {
        JobConfiguration = new JobConfiguration()
        {
          IsSynchronous = new bool?(false),
          LegalInfo = FormattableString.Invariant(FormattableStringFactory.Create("Contact {0} | Source Host ID: {1}", (object) this.m_contactEmail, (object) sourceHostId)),
          CallbackEndpoint = uri.ToString(),
          NotificationEmail = this.m_contactEmail
        }
      };
    }

    private static string GetPuidIfMsa(Microsoft.VisualStudio.Services.Identity.Identity contentCreator) => contentCreator.IsMsa() ? IdentityHelper.GetPuid((IReadOnlyVssIdentity) contentCreator).Value : (string) null;

    private static string GetReportableIdentityString(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ContentValidationIdentity validationIdentity = new ContentValidationIdentity(identity);
      string reportableIdentityString = JsonConvert.SerializeObject((object) validationIdentity);
      if (reportableIdentityString.Length > 128)
      {
        validationIdentity.DisplayName = (string) null;
        reportableIdentityString = JsonConvert.SerializeObject((object) validationIdentity);
      }
      return reportableIdentityString;
    }

    private async Task SendContentItemsInBatches(
      IVssRequestContext rc,
      Guid? projectId,
      IEnumerable<ContentItem> toSend,
      ContentValidationTakedownTarget takedownTarget)
    {
      ContentValidator contentValidator1 = this;
      if (contentValidator1.m_cvsClient == null)
        await contentValidator1.SetupCvsClient(rc, (CvsClient) null);
      ProcessingConfiguration config = contentValidator1.GetProcessingConfiguration(rc.ServiceHost.InstanceId, projectId, takedownTarget);
      List<ContentItem> batch = new List<ContentItem>();
      foreach (ContentItem contentItem in toSend)
      {
        if (contentItem.ContentType == 4)
        {
          ContentValidator contentValidator2 = contentValidator1;
          IVssRequestContext rc1 = rc;
          Guid? sourceProjectId = projectId;
          ProcessingConfiguration config1 = config;
          List<ContentItem> batch1 = new List<ContentItem>();
          batch1.Add(contentItem);
          int takedownTarget1 = (int) takedownTarget;
          await contentValidator2.SendBatch(rc1, sourceProjectId, config1, batch1, (ContentValidationTakedownTarget) takedownTarget1);
        }
        else
        {
          batch.Add(contentItem);
          if (batch.Count == 100)
          {
            await contentValidator1.SendBatch(rc, projectId, config, batch, takedownTarget);
            batch.Clear();
          }
        }
      }
      if (batch.Count <= 0)
      {
        config = (ProcessingConfiguration) null;
        batch = (List<ContentItem>) null;
      }
      else
      {
        await contentValidator1.SendBatch(rc, projectId, config, batch, takedownTarget);
        config = (ProcessingConfiguration) null;
        batch = (List<ContentItem>) null;
      }
    }

    private async Task SendBatch(
      IVssRequestContext rc,
      Guid? sourceProjectId,
      ProcessingConfiguration config,
      List<ContentItem> batch,
      ContentValidationTakedownTarget takedownTarget)
    {
      if (rc.IsFeatureEnabled("VisualStudio.Services.ContentValidation.TokenizeSubmissions"))
        CvsTokenUtil.PopulateInContentItem(batch[0], CvsTokenUtil.Get(rc, rc.ServiceHost.InstanceId, takedownTarget, sourceProjectId));
      int curAttempt = 1;
      while (curAttempt <= 3)
      {
        int msToWait = curAttempt * this.m_failedRetryWaitMs;
        CvsClient client = (CvsClient) null;
        try
        {
          Job result = (Job) null;
          await new CommandServiceAsync(rc, ContentValidator.s_breakerCommandSetter, (Func<Task>) (async () =>
          {
            using (await this.m_clientLock.ReaderLockAsync())
            {
              client = this.m_cvsClient;
              LoggingWebRequestHandler.E2EID.Value = rc.E2EId;
              LoggingWebRequestHandler.ActivityId.Value = rc.ActivityId;
              result = await client.CreateJobAsync(config, (IEnumerable<ContentItem>) batch);
            }
          }), continueOnCapturedContext: true).Execute();
          ContentValidator.s_totalRequestsCounter.Increment();
          if (result.Status == 5 || result.Status == null)
          {
            rc.TraceAlways(15289009, TraceLevel.Error, "HostedContentValidationService", "SendContentItemsInBatches", "Attempt {0}/{1}: Job creation failed: {2}.\nWaiting {3}ms before retry...", (object) curAttempt, (object) 3, (object) result, (object) msToWait);
            await Task.Delay(msToWait);
            ++curAttempt;
          }
          else
          {
            rc.TraceAlways(15289010, TraceLevel.Info, "HostedContentValidationService", "SendContentItemsInBatches", "Sent batch of {0} items on attempt {1}. Job ID: {2}", (object) batch.Count, (object) curAttempt, (object) ((BaseResource) result).Id);
            break;
          }
        }
        catch (Exception ex) when (curAttempt < 3)
        {
          rc.TraceAlways(15289011, TraceLevel.Error, "HostedContentValidationService", "SendContentItemsInBatches", "Attempt {0}/{1}: exception during job creation: {2}\nWaiting {3}ms before retry...", (object) curAttempt, (object) 3, (object) ex, (object) msToWait);
          await Task.Delay(msToWait);
          await this.SetupCvsClient(rc, client);
          ++curAttempt;
        }
      }
    }

    private async Task SetupCvsClient(IVssRequestContext rc, CvsClient prevClient)
    {
      if (prevClient != this.m_cvsClient)
        return;
      X509Certificate2 newClientCert = (X509Certificate2) null;
      WebRequestHandler newHandler = (WebRequestHandler) null;
      bool replacedClient = false;
      try
      {
        IVssRequestContext requestContext = rc.Elevate().To(TeamFoundationHostType.Deployment);
        Guid drawerId = this.m_strongBox.UnlockDrawer(requestContext, "ConfigurationSecrets", true);
        string cvsEndpoint = this.m_registry.GetValue<string>(requestContext, in HostedContentValidationConstants.CvsApiEndpointQuery, (string) null);
        if (cvsEndpoint == null)
          throw new InvalidConfigurationException(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting for {0} missing.", (object) "cvsEndpoint")));
        newClientCert = this.m_strongBox.RetrieveFileAsCertificate(requestContext, drawerId, "CvsAuthCertificate");
        string cvsSubscriptionKey = this.m_strongBox.GetString(requestContext, drawerId, "CvsAPIMKey");
        newHandler = this.m_webRequestHandlerFactory();
        using (await this.m_clientLock.WriterLockAsync())
        {
          if (prevClient == this.m_cvsClient)
          {
            WebRequestHandler requestHandler = this.m_requestHandler;
            this.m_requestHandler = newHandler;
            this.m_cvsClient = new CvsClientBuilder(cvsEndpoint).MessageHandler((HttpMessageHandler) this.m_requestHandler).ClientCertificate(newClientCert).SubscriptionKey(cvsSubscriptionKey).Client();
            replacedClient = true;
            prevClient?.Dispose();
            requestHandler?.Dispose();
          }
        }
        cvsEndpoint = (string) null;
        cvsSubscriptionKey = (string) null;
      }
      finally
      {
        if (!replacedClient)
          newHandler?.Dispose();
      }
      newClientCert = (X509Certificate2) null;
      newHandler = (WebRequestHandler) null;
    }

    private IEnumerable<ContentItem> TranslateToContentItems(
      IVssRequestContext rc,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress,
      IEnumerable<ContentValidationKey> toScan)
    {
      int itemCount = 0;
      HashSet<string> checkedHosts = new HashSet<string>();
      foreach (ContentValidationKey contentValidationKey in toScan)
      {
        ContentType contentType;
        switch (contentValidationKey.ScanType)
        {
          case ContentValidationScanType.None:
            continue;
          case ContentValidationScanType.Image:
            contentType = (ContentType) 3;
            break;
          case ContentValidationScanType.Video:
            contentType = (ContentType) 4;
            break;
          default:
            throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported scan type: {0}", (object) contentValidationKey.ScanType)));
        }
        string str = contentValidationKey.Uri.ToString();
        IVssRequestContext requestContext = rc.To(TeamFoundationHostType.Deployment);
        string host = contentValidationKey.Uri.Host;
        if (!checkedHosts.Contains(host))
        {
          if (!this.m_instMgmt.IsRegisteredServiceDomain(requestContext, host))
            throw new ArgumentException(str + " (host: " + host + ") cannot be scanned because it's on an unsupported domain. Supported domains: " + string.Join(", ", (IEnumerable<string>) this.m_instMgmt.GetRegisteredServiceDomains(requestContext)));
          checkedHosts.Add(host);
        }
        ContentValidationExternalId validationExternalId = new ContentValidationExternalId()
        {
          FileName = contentValidationKey.FileName,
          Uri = str,
          E2EID = rc.E2EId
        };
        yield return new ContentItem()
        {
          ExternalId = JsonConvert.SerializeObject((object) validationExternalId),
          ContentType = contentType,
          ReporteeName = ContentValidator.GetReportableIdentityString(contentCreator),
          MsaPuidHex = ContentValidator.GetPuidIfMsa(contentCreator),
          ReporteeAddress = creatorIpAddress,
          IncidentTime = new DateTime?(DateTime.UtcNow),
          Representation = (ContentRepresentation) 2,
          Value = str
        };
        ++itemCount;
      }
      ContentValidator.s_uriSubmissionsCounter.IncrementBy((long) itemCount);
    }
  }
}
