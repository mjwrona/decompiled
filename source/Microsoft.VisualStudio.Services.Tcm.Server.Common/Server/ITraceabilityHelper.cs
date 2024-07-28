// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITraceabilityHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public interface ITraceabilityHelper
  {
    TestToWorkItemLinks QueryLinkedWorkItems(GuidAndString projectId, string testName);

    WorkItemToTestLinks QueryLinkedTests(
      GuidAndString projectId,
      WorkItemToTestLinks workItemToTestLinks);

    WorkItemToTestLinks AddWorkItemLink(
      GuidAndString projectId,
      WorkItemToTestLinks workItemToTestLinks);

    bool DeleteWorkItemLink(GuidAndString projectId, string testName, int workItemId);

    List<TestSummaryForWorkItem> QueryAggregatedDataByRequirement(
      GuidAndString projectId,
      List<int> workItemIds,
      TestResultsContext resultsContext);
  }
}
