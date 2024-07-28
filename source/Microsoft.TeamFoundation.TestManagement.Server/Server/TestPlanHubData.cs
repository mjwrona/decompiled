// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanHubData
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanHubData
  {
    public TestPlan TestPlan { get; set; }

    public List<ServerTestSuite> TestSuites { get; set; }

    public List<TestPoint> TestPoints { get; set; }

    public int SelectedSuiteId { get; set; }

    internal static TestPlanHubData Fetch(
      TestManagementRequestContext context,
      string projectId,
      int planId,
      int suiteId,
      int configurationId,
      Guid tester,
      bool includeTestPoints = true)
    {
      using (TestPlanningDatabase planningDatabase = TestPlanningDatabase.Create(context))
        return planningDatabase.FetchTestPlanHubData(new Guid(projectId), planId, suiteId, includeTestPoints, configurationId, tester);
    }
  }
}
