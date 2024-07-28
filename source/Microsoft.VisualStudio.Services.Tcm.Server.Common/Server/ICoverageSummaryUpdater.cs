// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ICoverageSummaryUpdater
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ICoverageSummaryUpdater
  {
    void UpdateCodeCoverageSummary(
      TestManagementRequestContext context,
      PipelineContext pipelineContext,
      string buildPlatform,
      string buildFlavor,
      CoverageSummaryStatus summaryStatus);

    void UpdateCodeCoverageSummary(
      TestManagementRequestContext context,
      PipelineContext pipelineContext,
      string buildPlatform,
      string buildFlavor,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus);

    bool UpdateCodeCoverageSummary(
      TestManagementRequestContext context,
      BuildConfiguration buildConfiguration,
      Guid projectId,
      CoverageSummaryStatus summaryStatus);

    bool UpdateCodeCoverageSummary(
      TestManagementRequestContext context,
      BuildConfiguration buildConfiguration,
      Guid projectId,
      CoverageSummaryStatus summaryStatus,
      CoverageDetailedSummaryStatus coverageDetailedSummaryStatus);
  }
}
