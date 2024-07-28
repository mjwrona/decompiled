// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementTraceabilityService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementTraceabilityService))]
  public interface ITeamFoundationTestManagementTraceabilityService : IVssFrameworkService
  {
    WorkItemToTestLinks AddRequirementToTestLinks(
      TestManagementRequestContext context,
      GuidAndString projectId,
      int workItemId,
      List<TestMethod> testMethods);

    TestToWorkItemLinks QueryLinkedRequirementsForTest(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      TestMethod test);

    void DeleteRequirementToTestLink(
      TestManagementRequestContext context,
      GuidAndString projectId,
      int workItemId,
      TestMethod testMethod);

    void RestoreRequirementToTestLink(
      TestManagementRequestContext context,
      GuidAndString projectId,
      int workItemId);

    void DestroyRequirementToTestLink(
      TestManagementRequestContext context,
      IEnumerable<int> workItemIds);

    void SyncRequirementTestLinks(
      TestManagementRequestContext context,
      Guid projectId,
      int workItemId,
      IEnumerable<int> testCaseRefIds);

    List<TestSummaryForWorkItem> QueryAggregatedDataByRequirementForBuild(
      TestManagementRequestContext context,
      GuidAndString projectId,
      List<int> workItemIds,
      BuildConfiguration buildConfiguration);

    List<TestSummaryForWorkItem> QueryAggregatedDataByRequirementForRelease(
      TestManagementRequestContext context,
      GuidAndString projectId,
      List<int> workItemIds,
      ReleaseReference releaseReference);
  }
}
