// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementTestPlanService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementTestPlanService))]
  public interface ITeamFoundationTestManagementTestPlanService : IVssFrameworkService
  {
    List<TestPlan> GetPlanFromIds(
      IVssRequestContext requestContext,
      List<int> testPlanIds,
      string projectName);

    List<TestPlanRecord> QueryTestPlansByChangedDate(
      IVssRequestContext requestContext,
      int projectId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource);

    bool RestoreSoftDeletedTestPlan(
      IVssRequestContext requestContext,
      Guid projectId,
      int testPlanId,
      bool restoreWorkItem,
      bool restoreForTestPlan);

    bool RestoreSoftDeletedTestSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      int testPlanId,
      int testSuiteIdToRecover,
      bool restoreWorkItem,
      bool restoreForTestPlan);
  }
}
