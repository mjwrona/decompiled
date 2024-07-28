// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CodeCoverageSummary
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CodeCoverageSummary
  {
    public IList<CodeCoverageData> CoverageData { get; set; } = (IList<CodeCoverageData>) new List<CodeCoverageData>();

    public CoverageSummaryStatus SummaryStatus { get; set; }

    public CoverageDetailedSummaryStatus CoverageDetailedSummaryStatus { get; set; }

    public string BuildUri { get; set; }

    public string DeltaBuildUri { get; set; }

    public static CodeCoverageSummary QuerySummary(
      TestManagementRequestContext context,
      string projectName,
      string buildUri,
      string deltaBuildUri)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new CodeCoverageSummary()
        {
          CoverageData = (IList<CodeCoverageData>) new List<CodeCoverageData>()
        };
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        CodeCoverageSummary codeCoverageSummary = managementDatabase.QueryCodeCoverageSummary(projectFromName.GuidId, buildUri, deltaBuildUri);
        return new CodeCoverageSummary()
        {
          CoverageData = codeCoverageSummary.CoverageData,
          BuildUri = codeCoverageSummary.BuildUri,
          DeltaBuildUri = codeCoverageSummary.DeltaBuildUri,
          SummaryStatus = codeCoverageSummary.SummaryStatus
        };
      }
    }

    public static CodeCoverageSummary QueryDetailedSummary(
      TestManagementRequestContext context,
      string projectName,
      string buildUri,
      string deltaBuildUri)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return new CodeCoverageSummary()
        {
          CoverageData = (IList<CodeCoverageData>) new List<CodeCoverageData>()
        };
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
      {
        CodeCoverageSummary codeCoverageSummary = managementDatabase.QueryCoverageDetailedSummary(projectFromName.GuidId, buildUri, deltaBuildUri);
        return new CodeCoverageSummary()
        {
          CoverageData = codeCoverageSummary.CoverageData,
          BuildUri = codeCoverageSummary.BuildUri,
          DeltaBuildUri = codeCoverageSummary.DeltaBuildUri,
          SummaryStatus = codeCoverageSummary.SummaryStatus,
          CoverageDetailedSummaryStatus = codeCoverageSummary.CoverageDetailedSummaryStatus
        };
      }
    }

    public static void AddOrUpdateSummaryWithStatus(
      TestManagementRequestContext context,
      string projectName,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus summaryStatus)
    {
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) coverageData, "CoverageData");
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) summaryStatus, "CoverageSummaryStatus");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      buildRef.BranchName = GitHelper.GetModifiedBranchName(buildRef.BranchName);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.AddOrUpdateCodeCoverageSummaryWithStatus(projectFromName.GuidId, buildRef, coverageData, summaryStatus);
    }

    public static void AddOrUpdateSummaryWithStatus(
      TestManagementRequestContext context,
      string projectName,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) coverageData, "CoverageData");
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) coverageDetailedSummaryStatus, "CoverageSummaryStatus");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      buildRef.BranchName = GitHelper.GetModifiedBranchName(buildRef.BranchName);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.AddOrUpdateCoverageDetailedSummaryWithStatus(projectFromName.GuidId, buildRef, coverageData, summaryStatus, coverageDetailedSummaryStatus);
    }

    public static void AddOrUpdateSummaryStatus(
      TestManagementRequestContext context,
      string projectName,
      BuildConfiguration buildRef,
      CoverageSummaryStatus summaryStatus)
    {
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) summaryStatus, "CoverageSummaryStatus");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      buildRef.BranchName = GitHelper.GetModifiedBranchName(buildRef.BranchName);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.AddOrUpdateCodeCoverageSummaryStatus(projectFromName.GuidId, buildRef, summaryStatus);
    }

    public static void AddOrUpdateSummaryStatus(
      TestManagementRequestContext context,
      string projectName,
      BuildConfiguration buildRef,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus)
    {
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) coverageDetailedSummaryStatus, "CoverageDetailedSummaryStatus");
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      buildRef.BranchName = GitHelper.GetModifiedBranchName(buildRef.BranchName);
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        managementDatabase.AddOrUpdateCoverageDetailedSummaryStatus(projectFromName.GuidId, buildRef, summaryStatus, coverageDetailedSummaryStatus);
    }

    public static CoverageSummaryStatusResult QuerySummaryStatus(
      TestManagementRequestContext context,
      string projectName,
      BuildConfiguration buildRef)
    {
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      return CodeCoverageSummary.QuerySummaryStatus(context, projectFromName.GuidId, buildRef);
    }

    public static CoverageSummaryStatusResult QuerySummaryStatus(
      TestManagementRequestContext context,
      Guid projectId,
      BuildConfiguration buildRef)
    {
      using (new SimpleTimer(context.RequestContext, string.Format("CoverageMonitorJob: QuerySummaryStatus {0} {1}", (object) buildRef.BuildId, (object) projectId)))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          return managementDatabase.QueryCodeCoverageSummaryStatus(projectId, buildRef);
      }
    }

    public static CoverageSummaryStatusResult QueryDetailedSummaryStatus(
      TestManagementRequestContext context,
      Guid projectId,
      BuildConfiguration buildRef)
    {
      using (new SimpleTimer(context.RequestContext, string.Format("CoverageMonitorJob: QuerySummaryStatus {0} {1}", (object) buildRef.BuildId, (object) projectId)))
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          return managementDatabase.QueryCoverageDetailedSummaryStatus(projectId, buildRef);
      }
    }
  }
}
