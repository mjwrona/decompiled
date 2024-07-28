// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementCodeCoverageService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementCodeCoverageService))]
  public interface ITeamFoundationTestManagementCodeCoverageService : IVssFrameworkService
  {
    List<TestRunCoverage> GetTestRunCodeCoverage(
      IVssRequestContext requestContext,
      TeamProjectReference projectRef,
      TestRun testRun,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags);

    List<BuildCoverage> GetBuildCodeCoverage(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectRef,
      string buildUri,
      Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags flags);

    string GetCodeCoverageFileUrl(
      IVssRequestContext tfsRequestContext,
      TeamProjectReference projectReference,
      BuildConfiguration buildConfiguration,
      RestApiResourceDetails restApiResource);

    void UpdateCodeCoverageSummary(
      IVssRequestContext requestContext,
      TeamProjectReference projRef,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus summaryStatus);

    void UpdateCodeCoverageSummary(
      IVssRequestContext requestContext,
      TeamProjectReference projRef,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus);

    void UpdateCodeCoverageSummaryStatus(
      IVssRequestContext requestContext,
      TeamProjectReference projRef,
      BuildConfiguration buildRef,
      CoverageSummaryStatus summaryStatus);

    void UpdateCodeCoverageSummaryStatus(
      IVssRequestContext requestContext,
      TeamProjectReference projRef,
      BuildConfiguration buildRef,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus);

    bool TryQueueMergeJobInvokerJobForRunningBuild(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Dictionary<string, object> ciData);

    CodeCoverageSummary GetCodeCoverageSummary(
      IVssRequestContext requestContext,
      TeamProjectReference projRef,
      string buildUri,
      string deltaBuildUri);

    Task GetFileLevelCoverageAsync(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      FileCoverageRequest fileCoverageRequest,
      Stream stream);
  }
}
