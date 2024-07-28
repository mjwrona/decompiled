// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITraceabilityDatabase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ITraceabilityDatabase
  {
    void AddRequirementToTestLinks(
      GuidAndString projectId,
      int workItemId,
      List<TestMethod> testMethods,
      Guid createdBy);

    void DeleteRequirementToTestLink(
      GuidAndString projectId,
      int workItemId,
      TestMethod testMethod,
      Guid deletedBy);

    List<int> QueryLinkedRequirementsForTest(Guid projectId, TestMethod testMethod);

    Dictionary<int, TestSummaryForWorkItem> QueryAggregatedDataByRequirementForBuild(
      GuidAndString projectId,
      List<int> workItemIds,
      BuildConfiguration buildConfigurationInfo,
      string sourceWorkflow,
      int runIdThreshold = 0);

    Dictionary<int, AggregatedDataForResultTrend> QueryAggregatedDataByRequirementForBuild2(
      GuidAndString projectId,
      List<int> workItemIds,
      BuildConfiguration buildConfigurationInfo,
      string sourceWorkflow,
      int numberOfDaysAgo,
      int runIdThreshold = 0);

    Dictionary<int, TestSummaryForWorkItem> QueryAggregatedDataByRequirementForRelease(
      GuidAndString projectId,
      List<int> workItemIds,
      int releaseDefinitionId,
      int releaseEnvironmentDefId,
      string sourceWorkflow,
      int runIdThreshold = 0);

    Dictionary<int, AggregatedDataForResultTrend> QueryAggregatedDataByRequirementForRelease2(
      GuidAndString projectId,
      List<int> testCaseRefIds,
      int releaseDefinitionId,
      int releaseEnvironmentDefId,
      string sourceWorkflow,
      int numberOfDaysAgo,
      int runIdThreshold = 0);

    void DestroyRequirementToTestLink(IEnumerable<int> workItemIds, int batchSize);
  }
}
