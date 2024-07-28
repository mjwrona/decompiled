// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CodeCoverageHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.Settings.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class CodeCoverageHelper : RestApiHelper
  {
    internal CodeCoverageHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> GetTestRunCodeCoverage(
      string projectId,
      int runId,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
    {
      ArgumentUtility.CheckForNull<string>(projectId, nameof (projectId), this.RequestContext.ServiceName);
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), this.RequestContext.ServiceName);
      ArgumentUtility.CheckGreaterThanZero((float) flags, nameof (flags), this.RequestContext.ServiceName);
      TeamProjectReference projRef = this.GetProjectReference(projectId);
      if (projRef == null || string.IsNullOrEmpty(projRef.Name))
        throw new TeamProjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectNotFound, (object) projectId));
      this.RequestContext.TraceInfo("RestLayer", "CodeCoverageHelper.GetTestRunCodeCoverage projectId = {0}, runId = {1}, flags = {2}", (object) projectId, (object) runId, (object) flags);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>>("CodeCoverageHelper.GetTestRunCodeCoverage", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>>) (() =>
      {
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> runCoverage;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestRunCodeCoverage(this.RequestContext, projRef.Id, runId, (int) flags, out runCoverage))
            return runCoverage;
        }
        else if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        return this.GetTestRunCodeCoverageLocal(projRef, runId, flags);
      }), 1015061, "TestManagement");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> GetBuildCodeCoverage(
      string projectId,
      int buildId,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
    {
      ArgumentUtility.CheckForNull<string>(projectId, nameof (projectId), this.RequestContext.ServiceName);
      ArgumentUtility.CheckGreaterThanZero((float) buildId, nameof (buildId), this.RequestContext.ServiceName);
      ArgumentUtility.CheckGreaterThanZero((float) flags, nameof (flags), this.RequestContext.ServiceName);
      TeamProjectReference projRef = this.GetProjectReference(projectId);
      if (projRef == null || string.IsNullOrEmpty(projRef.Name))
        throw new TeamProjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectNotFound, (object) projectId));
      string buildUri = this.GetBuildUriFromId(buildId.ToString());
      this.RequestContext.TraceInfo("RestLayer", "CodeCoverageHelper.GetBuildCodeCoverage projectId = {0}, buildUri = {1}, flags = {2}", (object) projectId, (object) buildId, (object) flags);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>>("CodeCoverageHelper.GetBuildCodeCoverage", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>>) (() =>
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverageDataContract = new List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>();
        bool flag1 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS");
        bool flag2 = this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.RemoveMergeLogicFromTCM");
        if (!flag1 && !flag2)
        {
          TCMServiceDataMigrationRestHelper.InvokeAction((Action) (() =>
          {
            foreach (BuildCoverage buildCoverage in this.TestManagementCodeCoverageService.GetBuildCodeCoverage(this.RequestContext, projRef, buildUri, flags))
              buildCoverageDataContract.Add(this.ConvertBuildCoverageToDataContract(buildCoverage, projRef));
          }), this.RequestContext);
          List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverage1;
          if (!this.TestManagementRequestContext.TcmServiceHelper.TryGetBuildCodeCoverage(this.RequestContext, projRef.Id, buildId, (int) flags, out buildCoverage1))
            return buildCoverageDataContract;
          List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCodeCoverage = this.TestManagementRequestContext.MergeDataHelper.MergeBuildCoverages(buildCoverageDataContract, buildCoverage1);
          foreach (TestManagementBaseSecuredObject baseSecuredObject in buildCodeCoverage)
            baseSecuredObject.InitializeSecureObject((ISecuredObject) new CodeCoverageSecuredObject(projRef.Id));
          return buildCodeCoverage;
        }
        if (flag1 && !this.TestManagementRequestContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new TestManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TFSUsingHelperlayer));
        foreach (BuildCoverage buildCoverage in this.TestManagementCodeCoverageService.GetBuildCodeCoverage(this.RequestContext, projRef, buildUri, flags))
          buildCoverageDataContract.Add(this.ConvertBuildCoverageToDataContract(buildCoverage, projRef));
        return buildCoverageDataContract;
      }), 1015061, "TestManagement");
    }

    internal virtual Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary GetCodeCoverageSummary(
      string projectId,
      int buildId,
      int deltaBuildId)
    {
      try
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string buildUriFromId = this.GetBuildUriFromId(buildId.ToString());
        return this.GetCodeCoverageSummary(projectId, buildId, deltaBuildId, buildUriFromId, projectReference);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceCatch(1015151, TraceLevel.Error, "TestManagement", "RestLayer", ex);
        return (Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary) null;
      }
    }

    internal Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary GetCodeCoverageSummary(
      string projectId,
      int buildId,
      int deltaBuildId,
      string buildUri,
      TeamProjectReference projRef)
    {
      try
      {
        this.RequestContext.TraceEnter("RestLayer", "CodeCoverageHelper.GetCodeCoverageSummary");
        Microsoft.TeamFoundation.Build.WebApi.Build build1 = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
        string deltaBuildUri = (string) null;
        if (deltaBuildId >= 1)
        {
          Microsoft.TeamFoundation.Build.WebApi.Build build2 = this.BuildServiceHelper.QueryBuildByUri(this.RequestContext, projRef.Id, this.GetBuildUriFromId(deltaBuildId.ToString()), true);
          if (build2 != null)
          {
            build1 = this.GetBuildDetail(buildUri, projRef.Id);
            if (build1 == null)
            {
              Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary codeCoverageSummary = new Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary();
              codeCoverageSummary.InitializeSecureObject((ISecuredObject) new CodeCoverageSecuredObject(projRef.Id));
              return codeCoverageSummary;
            }
            bool flag = string.Equals(build2.Definition?.Uri?.ToString(), build1.Definition?.Uri?.ToString());
            DateTime? finishTime1 = (DateTime?) build2?.FinishTime;
            DateTime? finishTime2 = (DateTime?) build1?.FinishTime;
            deltaBuildUri = ((finishTime1.HasValue & finishTime2.HasValue ? (finishTime1.GetValueOrDefault() < finishTime2.GetValueOrDefault() ? 1 : 0) : 0) & (flag ? 1 : 0)) != 0 ? build2.Uri?.ToString() : (string) null;
          }
        }
        if (deltaBuildId != -10 && string.IsNullOrEmpty(deltaBuildUri))
        {
          if (build1 == null)
            build1 = this.GetBuildDetail(buildUri, projRef.Id);
          if (build1 != null)
          {
            BuildConfiguration currentBuild = this.BuildServiceHelper.QueryBuildConfigurationByBuildUri(this.RequestContext, projRef.Id, buildUri);
            if (currentBuild != null && build1.StartTime.HasValue && build1.StartTime.HasValue)
            {
              BuildConfiguration buildConfiguration = this.BuildServiceHelper.QueryLastCompleteSuccessfulBuild(this.RequestContext, projRef.Id, currentBuild, build1.StartTime.Value);
              if (buildConfiguration != null)
                deltaBuildUri = buildConfiguration.BuildUri;
            }
          }
          else
          {
            Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary codeCoverageSummary = new Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary();
            codeCoverageSummary.InitializeSecureObject((ISecuredObject) new CodeCoverageSecuredObject(projRef.Id));
            return codeCoverageSummary;
          }
        }
        this.RequestContext.TraceInfo("RestLayer", "CodeCoverageHelper.GetCodeCoverageSummary projectId = {0}, buildUri = {1}", (object) projectId, (object) buildUri);
        Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary codeCoverageSummary1 = (Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary) null;
        if (!this.TestManagementRequestContext.IsTcmService)
          this.TestManagementRequestContext.TcmServiceHelper.TryGetCodeCoverageSummary(this.RequestContext, projRef.Id, buildId, deltaBuildId, out codeCoverageSummary1);
        if (codeCoverageSummary1 == null || codeCoverageSummary1.CoverageData.Count == 0 || !TCMServiceDataMigrationRestHelper.IsMigrationCompleted(this.RequestContext))
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary codeCoverageSummary2 = this.TestManagementRequestContext.MergeDataHelper.MergeCodeCoverageSummary(this.ConvertCodeCoverageToDataContract(this.TestManagementCodeCoverageService.GetCodeCoverageSummary(this.RequestContext, projRef, buildUri, deltaBuildUri), projRef, buildId), codeCoverageSummary1);
          codeCoverageSummary2.InitializeSecureObject((ISecuredObject) new CodeCoverageSecuredObject(projRef.Id));
          return codeCoverageSummary2;
        }
        codeCoverageSummary1?.InitializeSecureObject((ISecuredObject) new CodeCoverageSecuredObject(projRef.Id));
        return codeCoverageSummary1;
      }
      finally
      {
        this.RequestContext.TraceLeave("RestLayer", "CodeCoverageHelper.GetCodeCoverageSummary");
      }
    }

    internal void UpdateCodeCoverageSummary(
      string projectId,
      int buildId,
      CodeCoverageData coverageData,
      CoverageSummaryStatus summaryStatus)
    {
      try
      {
        this.RequestContext.TraceEnter("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary");
        ArgumentUtility.CheckForNull<CodeCoverageData>(coverageData, nameof (coverageData), this.RequestContext.ServiceName);
        ArgumentUtility.CheckGreaterThanZero((float) buildId, nameof (buildId), this.RequestContext.ServiceName);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string buildUriFromId = this.GetBuildUriFromId(buildId.ToString());
        BuildConfiguration buildRef = this.BuildServiceHelper.QueryBuildConfigurationByBuildUri(this.RequestContext, projectReference.Id, buildUriFromId);
        if (buildRef == null)
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BuildNotFound, (object) buildUriFromId), ObjectTypes.Other);
        buildRef.BuildPlatform = coverageData.BuildPlatform ?? string.Empty;
        buildRef.BuildFlavor = coverageData.BuildFlavor ?? string.Empty;
        this.RequestContext.TraceInfo("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary projectId = {0}, buildUri = {1}", (object) projectId, (object) buildUriFromId);
        if (this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableCoverageDetailedStatus"))
          this.TestManagementCodeCoverageService.UpdateCodeCoverageSummary(this.RequestContext, projectReference, buildRef, coverageData, summaryStatus, this.ValueOfCoverageDetailedSummaryStatus(summaryStatus));
        else
          this.TestManagementCodeCoverageService.UpdateCodeCoverageSummary(this.RequestContext, projectReference, buildRef, coverageData, summaryStatus);
      }
      finally
      {
        this.RequestContext.TraceLeave("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary");
      }
    }

    internal void UpdateCodeCoverageSummary(
      string projectId,
      int buildId,
      CodeCoverageData coverageData,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      try
      {
        this.RequestContext.TraceEnter("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary");
        ArgumentUtility.CheckForNull<CodeCoverageData>(coverageData, nameof (coverageData), this.RequestContext.ServiceName);
        ArgumentUtility.CheckGreaterThanZero((float) buildId, nameof (buildId), this.RequestContext.ServiceName);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string buildUriFromId = this.GetBuildUriFromId(buildId.ToString());
        BuildConfiguration buildRef = this.BuildServiceHelper.QueryBuildConfigurationByBuildUri(this.RequestContext, projectReference.Id, buildUriFromId);
        if (buildRef == null)
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BuildNotFound, (object) buildUriFromId), ObjectTypes.Other);
        buildRef.BuildPlatform = coverageData.BuildPlatform ?? string.Empty;
        buildRef.BuildFlavor = coverageData.BuildFlavor ?? string.Empty;
        this.RequestContext.TraceInfo("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary projectId = {0}, buildUri = {1}", (object) projectId, (object) buildUriFromId);
        this.TestManagementCodeCoverageService.UpdateCodeCoverageSummary(this.RequestContext, projectReference, buildRef, coverageData, summaryStatus, coverageDetailedSummaryStatus);
      }
      finally
      {
        this.RequestContext.TraceLeave("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary");
      }
    }

    internal void UpdateCodeCoverageSummaryStatus(
      string projectId,
      int buildId,
      CoverageSummaryStatus summaryStatus)
    {
      try
      {
        this.RequestContext.TraceEnter("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummaryStatus");
        ArgumentUtility.CheckGreaterThanZero((float) buildId, nameof (buildId), this.RequestContext.ServiceName);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string buildUriFromId = this.GetBuildUriFromId(buildId.ToString());
        BuildConfiguration buildRef = this.BuildServiceHelper.QueryBuildConfigurationByBuildUri(this.RequestContext, projectReference.Id, buildUriFromId);
        if (buildRef == null)
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BuildNotFound, (object) buildUriFromId), ObjectTypes.Other);
        this.RequestContext.TraceInfo("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary projectId = {0}, buildUri = {1}", (object) projectId, (object) buildUriFromId);
        this.TestManagementCodeCoverageService.UpdateCodeCoverageSummaryStatus(this.RequestContext, projectReference, buildRef, summaryStatus);
      }
      finally
      {
        this.RequestContext.TraceLeave("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary");
      }
    }

    internal void UpdateCodeCoverageSummaryStatus(
      string projectId,
      int buildId,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      try
      {
        this.RequestContext.TraceEnter("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummaryStatus");
        ArgumentUtility.CheckGreaterThanZero((float) buildId, nameof (buildId), this.RequestContext.ServiceName);
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        string buildUriFromId = this.GetBuildUriFromId(buildId.ToString());
        BuildConfiguration buildRef = this.BuildServiceHelper.QueryBuildConfigurationByBuildUri(this.RequestContext, projectReference.Id, buildUriFromId);
        if (buildRef == null)
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.BuildNotFound, (object) buildUriFromId), ObjectTypes.Other);
        this.RequestContext.TraceInfo("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary projectId = {0}, buildUri = {1}", (object) projectId, (object) buildUriFromId);
        this.TestManagementCodeCoverageService.UpdateCodeCoverageSummaryStatus(this.RequestContext, projectReference, buildRef, summaryStatus, coverageDetailedSummaryStatus);
      }
      finally
      {
        this.RequestContext.TraceLeave("RestLayer", "CodeCoverageHelper.UpdateCodeCoverageSummary");
      }
    }

    internal Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary QueueInvokerJobForPatchSummaryRequest(
      Guid projectId,
      int buildId)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      try
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId.ToString());
        string buildUriFromId = this.GetBuildUriFromId(buildId.ToString());
        Microsoft.TeamFoundation.Build.WebApi.Build buildDetail = this.GetBuildDetail(buildUriFromId, projectReference.Id);
        Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary codeCoverageSummary = this.GetCodeCoverageSummary(projectId.ToString(), buildId, -1, buildUriFromId, projectReference);
        if (this.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return codeCoverageSummary;
        dictionary["CurrentCoverageSummaryStatus"] = (object) codeCoverageSummary.Status;
        dictionary["CurrentCoverageDetailedSummaryStatus"] = (object) codeCoverageSummary.CoverageDetailedSummaryStatus;
        switch (codeCoverageSummary.Status)
        {
          case CoverageSummaryStatus.InProgress:
          case CoverageSummaryStatus.Pending:
            if (this.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableCoverageDetailedStatus"))
              this.UpdateCodeCoverageSummaryStatus(projectId.ToString(), buildId, CoverageSummaryStatus.UpdateRequestQueued, CoverageDetailedSummaryStatus.UpdateRequestQueued);
            else
              this.UpdateCodeCoverageSummaryStatus(projectId.ToString(), buildId, CoverageSummaryStatus.UpdateRequestQueued);
            codeCoverageSummary.Status = CoverageSummaryStatus.UpdateRequestQueued;
            codeCoverageSummary.CoverageDetailedSummaryStatus = CoverageDetailedSummaryStatus.UpdateRequestQueued;
            return codeCoverageSummary;
          case CoverageSummaryStatus.Finalized:
            dictionary["QueueInvokerJobForPatchSummaryResponse"] = (object) "CoverageSummaryStatusConflictException";
            throw new CoverageSummaryStatusConflictException(string.Format(CoverageResources.CoverageSummaryStatusConflictMessage, (object) Enum.GetName(typeof (CoverageSummaryStatus), (object) codeCoverageSummary.Status)));
          case CoverageSummaryStatus.UpdateRequestQueued:
            return codeCoverageSummary;
          default:
            if (this.TestManagementCodeCoverageService.TryQueueMergeJobInvokerJobForRunningBuild(this.RequestContext, projectId, buildDetail, dictionary))
            {
              if (this.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableCoverageDetailedStatus"))
                this.UpdateCodeCoverageSummaryStatus(projectId.ToString(), buildId, CoverageSummaryStatus.Pending, CoverageDetailedSummaryStatus.Pending);
              else
                this.UpdateCodeCoverageSummaryStatus(projectId.ToString(), buildId, CoverageSummaryStatus.Pending);
              codeCoverageSummary.Status = CoverageSummaryStatus.Pending;
              codeCoverageSummary.CoverageDetailedSummaryStatus = CoverageDetailedSummaryStatus.Pending;
              return codeCoverageSummary;
            }
            codeCoverageSummary.CoverageDetailedSummaryStatus = CoverageDetailedSummaryStatus.BuildBailedOut;
            dictionary["QueueInvokerJobForPatchSummaryResponse"] = (object) "Failed";
            throw new UnsuccessfulQueueInvokerJobException();
        }
      }
      finally
      {
        this.TelemetryLogger.PublishData(this.RequestContext, nameof (QueueInvokerJobForPatchSummaryRequest), new CustomerIntelligenceData((IDictionary<string, object>) dictionary));
      }
    }

    internal async Task GetFileLevelCoverageAsync(
      FileCoverageRequest fileCoverageRequest,
      ProjectInfo projectInfo,
      Stream targetStream)
    {
      CodeCoverageHelper codeCoverageHelper = this;
      await codeCoverageHelper.TestManagementCodeCoverageService.GetFileLevelCoverageAsync(codeCoverageHelper.RequestContext, projectInfo, fileCoverageRequest, targetStream).ConfigureAwait(false);
    }

    internal Stream DownloadCoverageReport(Guid projectId, long containerId, string filePath)
    {
      if (!this.TestManagementRequestContext.SecurityManager.HasViewTestResultsPermission(this.TestManagementRequestContext, this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(projectId).Uri))
        throw new AccessDeniedException(ServerResources.TestPlanViewTestResultPermission);
      List<FileContainerItem> fileContainerItemList = this.RequestContext.GetService<ITeamFoundationFileContainerService>().QueryItems(this.RequestContext, containerId, filePath, projectId);
      if (fileContainerItemList != null && fileContainerItemList.Count == 1)
      {
        if (fileContainerItemList[0].FileType > 0 && fileContainerItemList[0].Status == ContainerItemStatus.Created)
        {
          TeamFoundationFileService service = this.RequestContext.GetService<TeamFoundationFileService>();
          long num = 0;
          IVssRequestContext requestContext = this.RequestContext;
          long fileId = (long) fileContainerItemList[0].FileId;
          byte[] numArray;
          ref byte[] local1 = ref numArray;
          ref long local2 = ref num;
          CompressionType compressionType;
          ref CompressionType local3 = ref compressionType;
          return service.RetrieveFile(requestContext, fileId, false, out local1, out local2, out local3);
        }
        this.RequestContext.TraceInfo("RestLayer", "CodeCoverageHelper.DownloadCoverageReport. Invalid container status: projectId = {0}, containerId = {1}, filePath = {2}", (object) projectId, (object) containerId, (object) filePath);
      }
      return (Stream) null;
    }

    internal List<SourceViewBuildCoverage> FetchFolderViewCodeCoverageReport(
      Guid projectId,
      int buildId)
    {
      ITestLogClientService service = this.TestManagementRequestContext.RequestContext.GetService<ITestLogClientService>();
      string str = "SourceReportFile.json";
      TestLogQueryParameters logQueryParameters = new TestLogQueryParameters();
      TestLogReference logReference = new TestLogReference()
      {
        BuildId = buildId,
        Scope = TestLogScope.Build,
        Type = TestLogType.GeneralAttachment,
        FilePath = str
      };
      PagedList<TestLog> result1 = service.QueryTestLogAsync(this.TestManagementRequestContext, projectId, logQueryParameters, logReference).Result;
      if (result1 == null || result1.Count == 0)
      {
        this.RequestContext.TraceError("RestLayer", "SourceViewReport is not found for the build = {0}", (object) buildId);
        return new List<SourceViewBuildCoverage>();
      }
      List<SourceViewBuildCoverage> viewBuildCoverageList = new List<SourceViewBuildCoverage>();
      TestLog testLog = result1[0];
      using (MemoryStream memoryStream = new MemoryStream((int) testLog.Size))
      {
        TestLogStatus result2 = service.DownloadTestLogAsync(this.TestManagementRequestContext, projectId, testLog.LogReference, (Stream) memoryStream).Result;
        if (result2.Status != TestLogStatusCode.Success)
          this.RequestContext.TraceError("RestLayer", "Failed to download file: " + testLog.LogReference.FilePath, (object) string.Format("Error Code:{0} Exception:{1}", (object) result2.Status, (object) result2.Exception));
        memoryStream.Position = 0L;
        using (StreamReader streamReader = new StreamReader((Stream) memoryStream))
        {
          try
          {
            viewBuildCoverageList = JsonUtilities.Deserialize<List<SourceViewBuildCoverage>>(streamReader.ReadToEnd());
          }
          catch (Exception ex)
          {
            this.RequestContext.TraceError("RestLayer", "Failed to deserialize sourceViewJson file: " + ex.Message);
          }
        }
      }
      foreach (TestManagementBaseSecuredObject baseSecuredObject in viewBuildCoverageList)
        baseSecuredObject.InitializeSecureObject((ISecuredObject) new CodeCoverageSecuredObject(projectId));
      return viewBuildCoverageList;
    }

    internal virtual bool IsAnonymousAccessAllowed(IVssRequestContext rc, Guid projectId) => JObject.Parse(rc.GetClient<SettingsHttpClient>(ServiceInstanceTypes.TFS).GetEntriesForScopeAsync(SettingsUserScope.AllUsers.ToString(), "Project", projectId.ToString(), "Pipelines/General").SyncResult<Dictionary<string, object>>()["Settings"].ToString()).GetValue("statusBadgesArePublic", StringComparison.OrdinalIgnoreCase).ToString().Equals("true", StringComparison.OrdinalIgnoreCase);

    public CoverageDetailedSummaryStatus ValueOfCoverageDetailedSummaryStatus(
      CoverageSummaryStatus status)
    {
      switch (status)
      {
        case CoverageSummaryStatus.None:
          return CoverageDetailedSummaryStatus.None;
        case CoverageSummaryStatus.InProgress:
          return CoverageDetailedSummaryStatus.InProgress;
        case CoverageSummaryStatus.Completed:
          return CoverageDetailedSummaryStatus.CodeCoverageSuccess;
        case CoverageSummaryStatus.Finalized:
          return CoverageDetailedSummaryStatus.Finalized;
        case CoverageSummaryStatus.Pending:
          return CoverageDetailedSummaryStatus.Pending;
        case CoverageSummaryStatus.UpdateRequestQueued:
          return CoverageDetailedSummaryStatus.UpdateRequestQueued;
        default:
          return CoverageDetailedSummaryStatus.None;
      }
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> GetTestRunCodeCoverageLocal(
      TeamProjectReference projRef,
      int runId,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
    {
      TestRun testRun = (TestRun) null;
      if (!this.TryGetTestRunFromRunId(projRef.Name, runId, out testRun))
        throw new TestObjectNotFoundException(this.RequestContext, runId, ObjectTypes.TestRun);
      List<TestRunCoverage> testRunCodeCoverage = this.TestManagementCodeCoverageService.GetTestRunCodeCoverage(this.RequestContext, projRef, testRun, flags);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> codeCoverageLocal = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>();
      foreach (TestRunCoverage testRunCoverage in testRunCodeCoverage)
        codeCoverageLocal.Add(this.ConvertTestRunCoverageToDataContract(projRef, testRun, testRunCoverage));
      return codeCoverageLocal;
    }

    private TeamProjectReference GetProjectRefFromProjectId(string projectId)
    {
      ArgumentUtility.CheckForNull<string>(projectId, nameof (projectId), this.RequestContext.ServiceName);
      this.RequestContext.TraceVerbose("RestLayer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CodeCoverageHelper.GetProjectRefFromProjectId Getting project reference for project Id: {0}", (object) projectId));
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      if (projectReference == null || string.IsNullOrEmpty(projectReference.Name))
      {
        TeamProjectNotFoundException notFoundException = new TeamProjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TeamProjectNotFound, (object) projectId));
        this.RequestContext.TraceVerbose("RestLayer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CodeCoverageHelper.GetProjectRefFromProjectId Received exception: {0}", (object) notFoundException.Message));
        throw notFoundException;
      }
      return projectReference;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary ConvertCodeCoverageToDataContract(
      CodeCoverageSummary codeCoverage,
      TeamProjectReference projectReference,
      int buildId = 0)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary();
      dataContract.Build = this.BuildServiceHelper.GetBuildRepresentation(this.RequestContext, new BuildConfiguration()
      {
        BuildUri = codeCoverage.BuildUri,
        BuildId = buildId
      });
      dataContract.DeltaBuild = this.BuildServiceHelper.GetBuildRepresentation(this.RequestContext, new BuildConfiguration()
      {
        BuildUri = codeCoverage.DeltaBuildUri
      });
      dataContract.CoverageData = codeCoverage.CoverageData;
      dataContract.Status = codeCoverage.SummaryStatus;
      dataContract.CoverageDetailedSummaryStatus = codeCoverage.CoverageDetailedSummaryStatus;
      dataContract.InitializeSecureObject((ISecuredObject) new CodeCoverageSecuredObject(projectReference.Id));
      return dataContract;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage ConvertBuildCoverageToDataContract(
      BuildCoverage buildCoverage,
      TeamProjectReference projectReference)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage dataContract1 = new Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage();
      dataContract1.LastError = buildCoverage.LastError;
      dataContract1.State = buildCoverage.State.ToString();
      dataContract1.Configuration = this.ConvertBuildConfigurationToDataContract(buildCoverage.Configuration);
      dataContract1.Modules = new List<Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage>();
      bool flag1 = true;
      bool flag2 = false;
      bool flag3 = false;
      foreach (ModuleCoverage module in buildCoverage.Modules)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage dataContract2 = this.ConvertModuleToDataContract(module);
        dataContract1.Modules.Add(dataContract2);
        if (!string.IsNullOrEmpty(dataContract2.FileUrl))
          flag3 = true;
        if (dataContract2.Statistics.BlocksCovered > 0 || dataContract2.Statistics.BlocksNotCovered > 0)
          flag2 = true;
      }
      if (flag3)
        flag1 = false;
      if (!flag2 && !flag3)
        flag1 = false;
      if (flag1)
        dataContract1.CodeCoverageFileUrl = this.TestManagementCodeCoverageService.GetCodeCoverageFileUrl(this.RequestContext, projectReference, buildCoverage.Configuration, this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.CodeCoverage]);
      dataContract1.InitializeSecureObject((ISecuredObject) new CodeCoverageSecuredObject(projectReference.Id));
      return dataContract1;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage ConvertTestRunCoverageToDataContract(
      TeamProjectReference projectRef,
      TestRun testRun,
      TestRunCoverage testRunCoverage)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage();
      dataContract.LastError = testRunCoverage.LastError;
      dataContract.State = testRunCoverage.State.ToString();
      dataContract.TestRun = this.GetRunRepresentation(projectRef.Name, testRun);
      dataContract.Modules = new List<Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage>();
      foreach (ModuleCoverage module in testRunCoverage.Modules)
        dataContract.Modules.Add(this.ConvertModuleToDataContract(module));
      return dataContract;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration ConvertBuildConfigurationToDataContract(
      BuildConfiguration buildConfiguration)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration()
      {
        Id = buildConfiguration.BuildConfigurationId,
        Uri = buildConfiguration.BuildUri,
        Flavor = buildConfiguration.BuildFlavor,
        Platform = buildConfiguration.BuildPlatform,
        Project = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
        {
          Name = buildConfiguration.TeamProjectName
        }
      };
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage ConvertModuleToDataContract(
      ModuleCoverage module)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage();
      dataContract.BlockCount = module.BlockCount;
      dataContract.BlockData = module.BlockData;
      dataContract.Name = module.Name;
      dataContract.Signature = module.Signature;
      dataContract.SignatureAge = module.SignatureAge;
      dataContract.FileUrl = module.CoverageFileUrl;
      dataContract.Statistics = this.ConvertStatisticsToDataContract(module.Statistics);
      dataContract.Functions = new List<Microsoft.TeamFoundation.TestManagement.WebApi.FunctionCoverage>();
      foreach (FunctionCoverage function in module.Functions)
        dataContract.Functions.Add(this.ConvertFunctionToDataContract(function));
      return dataContract;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.CoverageStatistics ConvertStatisticsToDataContract(
      CoverageStatistics statistics)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.CoverageStatistics()
      {
        BlocksCovered = statistics.BlocksCovered,
        BlocksNotCovered = statistics.BlocksNotCovered,
        LinesCovered = statistics.LinesCovered,
        LinesNotCovered = statistics.LinesNotCovered,
        LinesPartiallyCovered = statistics.LinesPartiallyCovered
      };
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.FunctionCoverage ConvertFunctionToDataContract(
      FunctionCoverage function)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.FunctionCoverage()
      {
        Class = function.Class,
        Name = function.Name,
        Namespace = function.Namespace,
        SourceFile = function.SourceFile,
        Statistics = this.ConvertStatisticsToDataContract(function.Statistics)
      };
    }

    public string ExtractFilename(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      string str = path.TrimEnd('/');
      int num = str.Length != 0 ? str.LastIndexOf('/') : throw new ArgumentException(FrameworkResources.CannotExtractFilenameFromFileContainerPath((object) path));
      return num >= 0 ? str.Substring(num + 1) : str;
    }

    public string GetMimeTypeFromFileExtension(HttpResponseMessage response)
    {
      string extension = Path.GetExtension(response.Content.Headers.ContentDisposition.FileName);
      if (extension == null)
        return "text/plain";
      string lower = extension.ToLower();
      if (lower != null)
      {
        switch (lower.Length)
        {
          case 3:
            if (lower == ".js")
              return "text/javascript";
            goto label_21;
          case 4:
            switch (lower[1])
            {
              case 'b':
                if (lower == ".bmp")
                  return "image/bmp";
                goto label_21;
              case 'c':
                if (lower == ".css")
                  return "text/css";
                goto label_21;
              case 'g':
                if (lower == ".gif")
                  return "image/gif";
                goto label_21;
              case 'h':
                if (lower == ".htm")
                  break;
                goto label_21;
              case 'j':
                if (lower == ".jpg")
                  goto label_19;
                else
                  goto label_21;
              case 'p':
                if (lower == ".png")
                  return "image/png";
                goto label_21;
              default:
                goto label_21;
            }
            break;
          case 5:
            switch (lower[1])
            {
              case 'h':
                if (lower == ".html")
                  break;
                goto label_21;
              case 'j':
                if (lower == ".jpeg")
                  goto label_19;
                else
                  goto label_21;
              default:
                goto label_21;
            }
            break;
          default:
            goto label_21;
        }
        return "text/html";
label_19:
        return "image/jpeg";
      }
label_21:
      return "text/plain";
    }

    public string GetDownloadFileName(
      long containerId,
      string path,
      string downloadFileName,
      bool isZip)
    {
      string downloadFileName1;
      if (!string.IsNullOrEmpty(downloadFileName))
        downloadFileName1 = CodeCoverageHelper.IsValidFileName(downloadFileName) ? downloadFileName : throw new ArgumentException(FrameworkResources.InvalidQueryParamFileName((object) downloadFileName, (object) nameof (downloadFileName)));
      else
        downloadFileName1 = !string.IsNullOrEmpty(path) ? this.ExtractFilename(path) : containerId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (isZip && !downloadFileName1.EndsWith(".zip", true, CultureInfo.InvariantCulture))
        downloadFileName1 += ".zip";
      return downloadFileName1;
    }

    private static bool IsValidFileName(string fileName) => !string.IsNullOrEmpty(fileName) && fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0;
  }
}
