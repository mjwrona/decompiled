// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementCodeCoverageService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementCodeCoverageService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementCodeCoverageService,
    IVssFrameworkService
  {
    private const string BuildCoverage_FileContainerNamePrefix = "BUILD_COVERAGE_";

    public TeamFoundationTestManagementCodeCoverageService()
    {
    }

    public TeamFoundationTestManagementCodeCoverageService(
      TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<TestRunCoverage> GetTestRunCodeCoverage(
      IVssRequestContext requestContext,
      TeamProjectReference projectRef,
      TestRun testRun,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, "TeamProjectReference", requestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectRef.Name, "TeamProjectName", requestContext.ServiceName);
      ArgumentUtility.CheckForNull<TestRun>(testRun, "TestRun", requestContext.ServiceName);
      TestManagementRequestContext context = new TestManagementRequestContext(requestContext);
      context.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementCodeCoverageService.GetTestRunCodeCoverage invoked with run details : {0}, flags : {1}", (object) testRun.ToString(), (object) flags);
      return TestRunCoverage.Query(context, projectRef.Name, testRun.TestRunId, flags);
    }

    public List<BuildCoverage> GetBuildCodeCoverage(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectRef,
      string buildUri,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, "TeamProjectReference", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectRef.Name, "TeamProjectName", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(buildUri, "BuildUri", tfsRequestContext.ServiceName);
      TestManagementRequestContext context = new TestManagementRequestContext(tfsRequestContext);
      context.TraceInfo("RestLayer", "TeamFoundationTestManagementCodeCoverageService.GetBuildCodeCoverage invoked with buildUri = {0}, flags = {1}", (object) buildUri, (object) flags);
      return BuildCoverage.Query(context, projectRef.Name, buildUri, flags);
    }

    public string GetCodeCoverageFileUrl(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectReference,
      BuildConfiguration buildConfiguration,
      RestApiResourceDetails restApiResource)
    {
      using (new SimpleTimer(tfsRequestContext, nameof (GetCodeCoverageFileUrl)))
      {
        TestManagementRequestContext managementRequestContext = new TestManagementRequestContext(tfsRequestContext, "TestManagement", "BusinessLayer");
        string codeCoverageFileUrl1 = (string) null;
        if (buildConfiguration == null)
        {
          managementRequestContext.Logger.Info(1015602, "GetCodeCoverageFileUrl: build configuration is null");
          return codeCoverageFileUrl1;
        }
        TestManagementServiceUtility utility = new TestManagementServiceUtility(managementRequestContext);
        Microsoft.TeamFoundation.Build.WebApi.Build build = this.BuildServiceHelper.QueryBuildByUri(tfsRequestContext, projectReference.Id, buildConfiguration.BuildUri, false);
        if (build == null)
        {
          managementRequestContext.Logger.Warning(1015603, string.Format("GetCodeCoverageFileUrl: build is null. Build Id {0}", (object) buildConfiguration.BuildId));
          return codeCoverageFileUrl1;
        }
        string coverageFilePath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/BuildCoverage/{0}", (object) CoverageFileNameUtility.GetCoverageFileName(build.BuildNumber, buildConfiguration));
        string codeCoverageFileUrl2;
        if (restApiResource.ServiceInstanceType == TestManagementServerConstants.TFSServiceInstanceType)
          codeCoverageFileUrl2 = TeamFoundationTestManagementCodeCoverageService.GetTfsCodeCoverageUrl(projectReference, utility, build, coverageFilePath1);
        else if (buildConfiguration.IsBuildMigrated)
        {
          codeCoverageFileUrl2 = this.GetTcmCodeCoverageUrl(tfsRequestContext, projectReference, buildConfiguration, managementRequestContext, build, coverageFilePath1);
          if (codeCoverageFileUrl2 == null)
          {
            string coverageFilePath2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/BuildCoverage/{0}", (object) CoverageFileNameUtility.GetCoverageFileName(build.BuildNumber, buildConfiguration, true));
            codeCoverageFileUrl2 = TeamFoundationTestManagementCodeCoverageService.GetTfsCodeCoverageUrl(projectReference, utility, build, coverageFilePath2);
          }
        }
        else
        {
          codeCoverageFileUrl2 = this.GetTcmCodeCoverageUrl(tfsRequestContext, projectReference, buildConfiguration, managementRequestContext, build, coverageFilePath1);
          if (string.IsNullOrEmpty(codeCoverageFileUrl2))
            codeCoverageFileUrl2 = TeamFoundationTestManagementCodeCoverageService.GetTfsCodeCoverageUrl(projectReference, utility, build, coverageFilePath1);
        }
        return codeCoverageFileUrl2;
      }
    }

    public void UpdateCodeCoverageSummary(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectRef,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus summaryStatus)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, "TeamProjectReference", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectRef.Name, "TeamProjectName", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(buildRef.BuildUri, "BuildUri", tfsRequestContext.ServiceName);
      TestManagementRequestContext context = new TestManagementRequestContext(tfsRequestContext);
      context.TraceInfo("RestLayer", "TeamFoundationTestManagementCodeCoverageService.UpdateCodeCoverageSummary invoked for buildUri = {0}", (object) buildRef.BuildUri);
      CodeCoverageSummary.AddOrUpdateSummaryWithStatus(context, projectRef.Name, buildRef, coverageData, summaryStatus);
    }

    public void UpdateCodeCoverageSummary(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectRef,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, "TeamProjectReference", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectRef.Name, "TeamProjectName", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(buildRef.BuildUri, "BuildUri", tfsRequestContext.ServiceName);
      TestManagementRequestContext context = new TestManagementRequestContext(tfsRequestContext);
      context.TraceInfo("RestLayer", "TeamFoundationTestManagementCodeCoverageService.UpdateCodeCoverageSummary invoked for buildUri = {0}", (object) buildRef.BuildUri);
      CodeCoverageSummary.AddOrUpdateSummaryWithStatus(context, projectRef.Name, buildRef, coverageData, summaryStatus, coverageDetailedSummaryStatus);
    }

    public void UpdateCodeCoverageSummaryStatus(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectRef,
      BuildConfiguration buildRef,
      CoverageSummaryStatus summaryStatus)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, "TeamProjectReference", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectRef.Name, "TeamProjectName", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(buildRef.BuildUri, "BuildUri", tfsRequestContext.ServiceName);
      TestManagementRequestContext context = new TestManagementRequestContext(tfsRequestContext);
      context.TraceInfo("RestLayer", "TeamFoundationTestManagementCodeCoverageService.UpdateCodeCoverageSummary invoked for buildUri = {0}", (object) buildRef.BuildUri);
      CodeCoverageSummary.AddOrUpdateSummaryStatus(context, projectRef.Name, buildRef, summaryStatus);
    }

    public void UpdateCodeCoverageSummaryStatus(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectRef,
      BuildConfiguration buildRef,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, "TeamProjectReference", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectRef.Name, "TeamProjectName", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(buildRef.BuildUri, "BuildUri", tfsRequestContext.ServiceName);
      TestManagementRequestContext context = new TestManagementRequestContext(tfsRequestContext);
      context.TraceInfo("RestLayer", "TeamFoundationTestManagementCodeCoverageService.UpdateCodeCoverageSummary invoked for buildUri = {0}", (object) buildRef.BuildUri);
      CodeCoverageSummary.AddOrUpdateSummaryStatus(context, projectRef.Name, buildRef, summaryStatus, coverageDetailedSummaryStatus);
    }

    public CodeCoverageSummary GetCodeCoverageSummary(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectRef,
      string buildUri,
      string deltaBuildUri)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, "TeamProjectReference", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectRef.Name, "TeamProjectName", tfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(buildUri, "BuildUri", tfsRequestContext.ServiceName);
      TestManagementRequestContext context = new TestManagementRequestContext(tfsRequestContext);
      context.TraceInfo("RestLayer", "TeamFoundationTestManagementCodeCoverageService.GetCodeCoverageSummary invoked for buildUri = {0}", (object) buildUri);
      return context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableCoverageDetailedStatus") ? CodeCoverageSummary.QueryDetailedSummary(context, projectRef.Name, buildUri, deltaBuildUri) : CodeCoverageSummary.QuerySummary(context, projectRef.Name, buildUri, deltaBuildUri);
    }

    public async Task GetFileLevelCoverageAsync(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      FileCoverageRequest fileCoverageRequest,
      Stream targetStream)
    {
      TestManagementRequestContext managementRequestContext = new TestManagementRequestContext(requestContext);
      int buildId = 0;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(managementRequestContext))
        buildId = managementDatabase.QueryTestPullRequestBuildId(projectInfo.Id, Guid.Parse(fileCoverageRequest.RepoId), fileCoverageRequest.PullRequestId, fileCoverageRequest.PullRequestIterationId);
      if (buildId == 0)
        managementRequestContext.TraceInfo("RestLayer", string.Format("TeamFoundationTestManagementCodeCoverageService.GetFileLevelCoverageAsync: build not found for PR: {0}, Iteration: {1}", (object) fileCoverageRequest.PullRequestId, (object) fileCoverageRequest.PullRequestIterationId));
      else
        await new DiffCoverageProvider().GetFileLevelCoverageAsync(managementRequestContext, projectInfo, buildId, fileCoverageRequest.FilePath, targetStream).ConfigureAwait(false);
    }

    public bool TryQueueMergeJobInvokerJobForRunningBuild(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Dictionary<string, object> ciData)
    {
      TestManagementRequestContext tcmRequestContext = new TestManagementRequestContext(context);
      DateTime utcNow = DateTime.UtcNow;
      try
      {
        PipelineContext pipelineContext = CommonHelper.CreatePipelineContext(tcmRequestContext, build);
        CommonHelper.PopulateCiDataWithPipelineContext(pipelineContext, ciData);
        ciData.Add("BuildResult", (object) build.Result);
        MergeInvokerJobData jobData = new MergeInvokerJobData()
        {
          QueueTime = DateTime.UtcNow,
          PipelineContext = pipelineContext,
          JobInvoker = 2
        };
        string format = string.Format("The build {0} was either canceled or is completed. Bailing out.", (object) build.Id);
        if (build.Result.HasValue)
        {
          BuildResult? result = build.Result;
          BuildResult buildResult = BuildResult.None;
          if (!(result.GetValueOrDefault() == buildResult & result.HasValue))
          {
            tcmRequestContext.Logger.Verbose(1015138, format);
            ciData.Add("Error", (object) format);
            return false;
          }
        }
        CoverageJobHelper.QueueOneTimeJob<MergeInvokerJobData>(tcmRequestContext, jobData);
        return true;
      }
      catch (Exception ex)
      {
        string str = string.Format("CodeCoverageHelper: QueueMergeJobInvokerJobForRunningBuild failed with {0}", (object) ex);
        ciData.Add("Error", (object) str);
        tcmRequestContext.RequestContext.TraceCatch(1015152, TraceLevel.Error, "TestManagement", "RestLayer", ex);
        return false;
      }
    }

    private string GetTcmCodeCoverageUrl(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectReference,
      BuildConfiguration buildConfiguration,
      TestManagementRequestContext tcmRequestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string coverageFilePath)
    {
      using (new SimpleTimer(tfsRequestContext, "TCM.GetCodeCoverageFileUrl"))
      {
        try
        {
          string locationServiceUrl = tfsRequestContext.GetService<ILocationService>().GetLocationServiceUrl(tfsRequestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker);
          ITeamFoundationFileContainerService service = tcmRequestContext.RequestContext.GetService<ITeamFoundationFileContainerService>();
          List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> fileContainerList = service.QueryContainers(tfsRequestContext, (IList<Uri>) new Uri[1]
          {
            build.Uri
          }, projectReference.Id);
          if (fileContainerList == null || fileContainerList.Count <= 0)
          {
            tcmRequestContext.Logger.Warning(1015604, string.Format("GetTcmCodeCoverageUrl: no container found for the build: {0}", (object) buildConfiguration.BuildId));
            return (string) null;
          }
          Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer = fileContainerList[0];
          if (string.IsNullOrEmpty(fileContainer.Name))
          {
            tcmRequestContext.Logger.Warning(1015605, string.Format("GetTcmCodeCoverageUrl: container found but with null or empty name, for the build: {0}", (object) buildConfiguration.BuildId));
            return (string) null;
          }
          if (!fileContainer.Name.Contains("BUILD_COVERAGE_"))
            tcmRequestContext.Logger.Warning(1015606, string.Format("GetTcmCodeCoverageUrl: container found but with old naming scheme, build: {0}, container name: {1}", (object) buildConfiguration.BuildId, (object) fileContainer.Name));
          List<FileContainerItem> fileContainerItemList = service.QueryItems(tfsRequestContext, fileContainer.Id, coverageFilePath, projectReference.Id);
          if (fileContainerItemList != null)
          {
            if (fileContainerItemList.Count == 1)
            {
              if (fileContainerItemList[0].FileType > 0 && fileContainerItemList[0].Status == ContainerItemStatus.Created)
              {
                if (TeamFoundationTestManagementCodeCoverageService.FileExists(tcmRequestContext, (long) fileContainerItemList[0].FileId))
                  return FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}/_apis/testresults/codecoverage/download/{2}?filePath={3}", (object) locationServiceUrl, (object) Uri.EscapeDataString(projectReference.Name), (object) fileContainer.Id, (object) Uri.EscapeDataString(coverageFilePath)));
                tcmRequestContext.Logger.Warning(1015612, string.Format("GetTcmCodeCoverageUrl: Coverage file container entry found, however the file is missing. ProjectId = {0}, ContainerId = {1}, FilePath = {2}, ItemFileType: {3}, Status: {4}", (object) projectReference.Id, (object) fileContainer.Id, (object) coverageFilePath, (object) fileContainerItemList[0].FileType, (object) fileContainerItemList[0].Status));
              }
              else
                tcmRequestContext.Logger.Warning(1015608, string.Format("GetTcmCodeCoverageUrl: Coverage file found, however the container item status is invalid. ProjectId = {0}, ContainerId = {1}, FilePath = {2}, ItemFileType: {3}, Status: {4}", (object) projectReference.Id, (object) fileContainer.Id, (object) coverageFilePath, (object) fileContainerItemList[0].FileType, (object) fileContainerItemList[0].Status));
            }
          }
        }
        catch (DataspaceNotFoundException ex)
        {
          tcmRequestContext.Logger.Warning(1015609, string.Format("GetCodeCoverageFileUrl: File container service not faulted in. Build Id {0}, Exception {1}.", (object) buildConfiguration.BuildId, (object) ex));
        }
        return (string) null;
      }
    }

    private static bool FileExists(TestManagementRequestContext tcmRequestContext, long fileId)
    {
      try
      {
        tcmRequestContext.RequestContext.GetService<TeamFoundationFileService>().RetrieveFile(tcmRequestContext.RequestContext, fileId, out CompressionType _);
      }
      catch (FileIdNotFoundException ex)
      {
        tcmRequestContext.Logger.Warning(1015610, string.Format("FileExists: File with Id {0} not found", (object) fileId));
        return false;
      }
      catch (Exception ex)
      {
        tcmRequestContext.Logger.Warning(1015611, string.Format("FileExists: Error checking if file with Id {0} exists or not. Ignoring the error. Exception {1}", (object) fileId, (object) ex));
      }
      return true;
    }

    private static string GetTfsCodeCoverageUrl(
      TeamProjectReference projectReference,
      TestManagementServiceUtility utility,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string coverageFilePath)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/_api/_build/ItemContent?buildUri={2}&path=/{3}", (object) utility.GetTeamProjectCollectionUrl(), (object) Uri.EscapeDataString(projectReference.Name), (object) Uri.EscapeDataString(build.Uri.ToString()), (object) Uri.EscapeDataString(coverageFilePath));
    }
  }
}
