// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.LegacyTCMServiceHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal class LegacyTCMServiceHelper : ILegacyTCMServiceHelper
  {
    private const string PointOutcomeSynchronizationRetryCount = "/Service/TestManagement/Settings/PointOutcomeSyncJob/RetryManagerRetryCount";
    private IVssRequestContext m_requestContext;
    private Guid m_exemptedS2SPrincipal;

    public LegacyTCMServiceHelper(IVssRequestContext context) => this.m_requestContext = context;

    public bool TryDeleteTestRunIds(IVssRequestContext context, DeleteTestRunRequest deleteRequest)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryDeleteTestRunIds), "Tcm")))
        return this.InvokeAction((Func<bool>) (() =>
        {
          if (!this.IsImpersonating())
          {
            int[] testRunIds = deleteRequest.TestRunIds;
            if ((testRunIds != null ? (((IEnumerable<int>) testRunIds).Any<int>() ? 1 : 0) : 0) != 0 && this.TcmHttpClient != null)
            {
              this.TcmHttpClient.DeleteTestRunIdsAsync(deleteRequest)?.Wait();
              return true;
            }
          }
          return false;
        }));
    }

    public bool TryCreateLogEntries(
      IVssRequestContext context,
      CreateTestMessageLogEntryRequest logEntryRequest,
      out List<int> logs)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateLogEntries), "Tcm")))
      {
        List<int> logIds = (List<int>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, logEntryRequest.TestRunId, true) || this.TcmHttpClient == null)
            return false;
          logIds = this.TcmHttpClient.CreateTestMessageLogEntriesAsync(logEntryRequest)?.Result;
          return true;
        })) ? 1 : 0;
        logs = logIds;
        return num != 0;
      }
    }

    public bool TryQueryLogEntries(
      IVssRequestContext context,
      QueryTestMessageLogEntryRequest logEntryRequest,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry> logEntries)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryLogEntries), "Tcm")))
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry> logs = (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || !this.IsTestRunInTCM(context, logEntryRequest.TestRunId, true) || this.TcmHttpClient == null)
            return false;
          logs = this.TcmHttpClient.QueryTestMessageLogEntryAsync(logEntryRequest)?.Result;
          return true;
        })) ? 1 : 0;
        logEntries = logs;
        return num != 0;
      }
    }

    public bool TryGetTestSettingsCompatById(
      IVssRequestContext context,
      Guid projectId,
      int testSettingsId,
      out LegacyTestSettings newTestSettings)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryGetTestSettingsCompatById), "Tcm")))
      {
        LegacyTestSettings responseFromRemote = (LegacyTestSettings) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.GetTestSettingsCompatByIdAsync(projectId, testSettingsId)?.Result;
          return true;
        })) ? 1 : 0;
        newTestSettings = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryCreateTestSettingsCompat(
      IVssRequestContext context,
      Guid projectId,
      LegacyTestSettings legacyTestSettings,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties newUpdateProperties)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateTestSettingsCompat), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties responseFromRemote = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.CreateTestSettingsCompatAsync(legacyTestSettings, projectId)?.Result;
          return true;
        })) ? 1 : 0;
        newUpdateProperties = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryTestSettings(
      IVssRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      bool omitSettings,
      out List<LegacyTestSettings> newTestSettings)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestSettings), "Tcm")))
      {
        List<LegacyTestSettings> responseFromRemote = (List<LegacyTestSettings>) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.QueryTestSettingsAsync(query, query.TeamProjectName, omitSettings)?.Result;
          return true;
        })) ? 1 : 0;
        newTestSettings = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryQueryTestSettingsCount(
      IVssRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out int? testSettingsCount)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryQueryTestSettingsCount), "Tcm")))
      {
        int? responseFromRemote = new int?();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.QueryTestSettingsCountAsync(query, query.TeamProjectName)?.Result;
          return true;
        })) ? 1 : 0;
        testSettingsCount = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryUpdateTestSettings(
      IVssRequestContext context,
      Guid projectId,
      LegacyTestSettings legacyTestSettings,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties newUpdateProperties)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryUpdateTestSettings), "Tcm")))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties responseFromRemote = (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          responseFromRemote = this.TcmHttpClient.UpdateTestSettingsAsync(legacyTestSettings, projectId)?.Result;
          return true;
        })) ? 1 : 0;
        newUpdateProperties = responseFromRemote;
        return num != 0;
      }
    }

    public bool TryCreateBlockedResults(
      IVssRequestContext context,
      GuidAndString projectId,
      List<LegacyTestCaseResult> testCaseResults,
      out List<LegacyTestCaseResult> newBlockedResults)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateBlockedResults), "Tcm")))
      {
        List<LegacyTestCaseResult> newTestCaseResults = new List<LegacyTestCaseResult>();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          newTestCaseResults = this.TcmHttpClient.CreateBatchedBlockedResultsAsync((IEnumerable<LegacyTestCaseResult>) testCaseResults, projectId.GuidId)?.Result;
          return true;
        })) ? 1 : 0;
        newBlockedResults = newTestCaseResults;
        return num != 0;
      }
    }

    public bool TryUploadAttachments(
      IVssRequestContext requestContext,
      int testRunId,
      int sessionId,
      Dictionary<string, string> requestParams,
      List<HttpPostedTcmAttachment> files)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryUploadAttachments), "Tcm")))
        return this.InvokeAction((Func<bool>) (() =>
        {
          if (sessionId > 0 || !this.IsTestRunInTCM(requestContext, testRunId, true) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          UploadAttachmentsRequest request = new UploadAttachmentsRequest();
          request.RequestParams = requestParams;
          request.Attachments = files;
          CancellationToken cancellationToken = new CancellationToken();
          tcmHttpClient.UploadAttachmentsLegacyAsync(request, cancellationToken: cancellationToken)?.Wait();
          return true;
        }));
    }

    public bool TryDownloadAttachments(
      TfsTestManagementRequestContext tfsRequestContext,
      List<int> attachmentIds,
      List<long> lengths,
      out TcmAttachment attachments)
    {
      using (PerfManager.Measure(tfsRequestContext.RequestContext, "CrossService", TraceUtils.GetActionName(nameof (TryDownloadAttachments), "Tcm")))
      {
        TcmAttachment downloadedAttachments = (TcmAttachment) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsAttachmentInTfs(tfsRequestContext, attachmentIds[0]) || this.TcmHttpClient == null)
            return false;
          TcmHttpClient tcmHttpClient = this.TcmHttpClient;
          DownloadAttachmentsRequest request = new DownloadAttachmentsRequest();
          request.Ids = attachmentIds;
          request.Lengths = lengths;
          CancellationToken cancellationToken = new CancellationToken();
          downloadedAttachments = tcmHttpClient.DownloadAttachmentsLegacyAsync(request, cancellationToken: cancellationToken)?.Result;
          return true;
        })) ? 1 : 0;
        attachments = downloadedAttachments;
        return num != 0;
      }
    }

    public bool TryCreateAfnStrip(
      IVssRequestContext context,
      Guid projectId,
      AfnStrip afnStrip,
      out AfnStrip createdAfnStrip)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryCreateAfnStrip), "Tcm")))
      {
        AfnStrip response = (AfnStrip) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          response = this.TcmHttpClient.CreateAfnStripAsync(afnStrip, projectId)?.Result;
          return true;
        })) ? 1 : 0;
        createdAfnStrip = response;
        return num != 0;
      }
    }

    public bool TryGetDefaultAfnStrips(
      IVssRequestContext context,
      Guid projectId,
      IList<int> testCaseIds,
      out List<AfnStrip> afnStrips)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName("TryCreateAfnStrip", "Tcm")))
      {
        List<AfnStrip> response = new List<AfnStrip>();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          response = this.TcmHttpClient.GetAfnStripsAsync(projectId, (IEnumerable<int>) testCaseIds)?.Result;
          return true;
        })) ? 1 : 0;
        afnStrips = response;
        return num != 0;
      }
    }

    public bool TryFilterPoints(
      IVssRequestContext context,
      Guid projectGuid,
      FilterPointQuery pointQuery,
      out List<PointLastResult> filteredPoints)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (TryFilterPoints), "Tcm")))
      {
        List<PointLastResult> response = new List<PointLastResult>();
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          if (this.IsImpersonating() || this.TcmHttpClient == null)
            return false;
          response = this.TcmHttpClient.FilterPointsAsync(pointQuery, projectGuid)?.Result;
          return true;
        })) ? 1 : 0;
        filteredPoints = response;
        return num != 0;
      }
    }

    public bool TryGetManualTestResultsByUpdatedDate(
      IVssRequestContext requestContext,
      Guid projectGuid,
      DateTime updateDate,
      int runId,
      int resultId,
      out TestResultsWithWatermark results)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (TryGetManualTestResultsByUpdatedDate), "Tcm")))
      {
        TestResultsWithWatermark responseFromRemote = (TestResultsWithWatermark) null;
        int num = this.InvokeAction((Func<bool>) (() =>
        {
          TcmHttpClient tcmHttpClientSqlReadOnly = this.TcmHttpClientSqlReadOnly("TestManagement.Server.GetManualTestResultsByUpdatedDate.EnableSqlReadReplica");
          if (requestContext.IsImpersonating || tcmHttpClientSqlReadOnly == null)
            return false;
          responseFromRemote = new RetryManager(requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/PointOutcomeSyncJob/RetryManagerRetryCount", 3), TimeSpan.FromSeconds(10.0), closure_0 ?? (closure_0 = (Action<Exception>) (ex => requestContext.TraceException("PointsOutcomeMigration", ex)))).InvokeFunc<TestResultsWithWatermark>((Func<TestResultsWithWatermark>) (() => tcmHttpClientSqlReadOnly.GetManualTestResultsByUpdatedDateAsync(projectGuid, updateDate, runId, resultId)?.Result));
          return true;
        })) ? 1 : 0;
        results = responseFromRemote;
        return num != 0;
      }
    }

    public bool IsTestRunInTCM(IVssRequestContext requestContext, int runId, bool queryFlow = true) => TCMServiceDataMigrationRestHelper.IsMigrationCompleted(requestContext) || !this.IsTestRunInTfs(runId, queryFlow);

    internal TcmHttpClient TcmHttpClient => this.m_requestContext.GetClient<TcmHttpClient>();

    internal TcmHttpClient TcmHttpClientSqlReadOnly(string FeatureFlag) => this.m_requestContext.GetClient<TcmHttpClient>(this.m_requestContext.GetHttpClientOptionsForEventualReadConsistencyLevel(FeatureFlag));

    internal Guid ExemptedS2SPrincipal
    {
      get => !(this.m_exemptedS2SPrincipal == Guid.Empty) ? this.m_exemptedS2SPrincipal : TestManagementServerConstants.TFSPrincipal;
      set => this.m_exemptedS2SPrincipal = value;
    }

    private bool IsTestRunInTfs(int runId, bool queryFlow = true)
    {
      if (this.m_requestContext.IsFeatureEnabled("TestManagement.Server.DisableCallToTfsForTestRun"))
        return false;
      IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
      IVssRequestContext requestContext1 = this.m_requestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceTestRunIdThreshold";
      ref RegistryQuery local1 = ref registryQuery;
      int num = service.GetValue<int>(requestContext1, in local1, int.MaxValue);
      IVssRequestContext requestContext2 = this.m_requestContext;
      registryQuery = (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceDataImportAccount";
      ref RegistryQuery local2 = ref registryQuery;
      return !(this.m_requestContext.ExecutionEnvironment.IsHostedDeployment & service.GetValue<bool>(requestContext2, in local2, false)) && (num != int.MaxValue && runId < num || runId % 2 != 0);
    }

    private bool IsAttachmentInTfs(
      TfsTestManagementRequestContext context,
      int attachmentId,
      bool queryFlow = true)
    {
      IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
      int num = service.GetValue<int>(this.m_requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceTestAttachmentIdThreshold", int.MaxValue);
      return service.GetValue<bool>(this.m_requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmServiceDataImportAccount", false) && this.m_requestContext.ExecutionEnvironment.IsHostedDeployment && attachmentId < num || num != int.MaxValue && attachmentId < num || attachmentId % 2 != 0;
    }

    private bool IsImpersonating() => this.m_requestContext.IsImpersonating && this.m_requestContext.GetAuthenticatedId() != this.ExemptedS2SPrincipal;

    private bool InvokeAction(Func<bool> func)
    {
      try
      {
        return func();
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException != null)
          throw ex.InnerException;
        throw;
      }
    }
  }
}
