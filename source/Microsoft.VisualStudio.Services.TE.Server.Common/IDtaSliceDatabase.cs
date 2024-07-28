// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.IDtaSliceDatabase
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Test.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  public interface IDtaSliceDatabase
  {
    TestAutomationRunSlice GetSlice(int testAgentId, string testAgentCapabilities, int testRunId);

    void QueueSlices(List<TestAutomationRunSlice> sliceDetailsList);

    void QueueSliceForAgents(TestAutomationRunSlice slice, List<int> autAgents);

    void UpdateSlice(TestAutomationRunSlice sliceUpdatePackage);

    void CancelSlices(int testRunId);

    TestAutomationRunSlice QuerySliceById(int sliceId);

    IEnumerable<TestAutomationRunSlice> QuerySlicesByTestRunId(int testRunId);

    void AbortSlicesByRunId(int testRunId);

    IEnumerable<int> RetrySlicesOfUnReachableAgentsAndGetTestRunIds(
      int allowedDownTimeInSecs,
      int maxRetryCount,
      int testRunId);

    IEnumerable<int> AbortSlicesIfAllAgentsAreDownAndGetTestRunIds(
      int allowedDownTimeInSecs,
      int testRunId);
  }
}
