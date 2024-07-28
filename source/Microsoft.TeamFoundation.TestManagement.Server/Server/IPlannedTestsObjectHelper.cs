// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IPlannedTestsObjectHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal interface IPlannedTestsObjectHelper : ITestManagementObjectHelper
  {
    List<TestPoint> FetchTestPointsFromIds(
      TestManagementRequestContext requestContext,
      string projectName,
      int planId,
      IdAndRev[] idsToFetch,
      string[] testCaseProperties,
      List<int> deletedIds);

    List<TestPlan> FetchTestPlans(
      TfsTestManagementRequestContext requestContext,
      IEnumerable<int> testPlanIds,
      string projectName,
      bool includeDetails = true);

    void CheckWorkItemDeletePermission(string projectName, TestManagementRequestContext context);
  }
}
