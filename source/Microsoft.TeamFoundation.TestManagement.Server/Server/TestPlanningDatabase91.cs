// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase91
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase91 : TestPlanningDatabase90
  {
    internal TestPlanningDatabase91(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase91()
    {
    }

    internal override bool RestoreSoftDeletedTestPlan(
      Guid projectId,
      int testPlanId,
      bool restoreWorkItem,
      bool restoreForTestPlan)
    {
      this.RequestContext.TraceEnter("Database", "TestPlanDatabase.RestoreSoftDeletedTestPlan");
      try
      {
        Guid userId = this.RequestContext.GetUserId();
        this.PrepareStoredProcedure("prc_RestoreSoftDeletedTestPlan");
        this.BindGuid("@projectId", projectId);
        this.BindInt("@testPlanId", testPlanId);
        this.BindBoolean("@restoreWorkItem", restoreWorkItem);
        this.BindBoolean("@restoreForTestPlan", restoreForTestPlan);
        this.BindGuid("@lastUpdatedBy", userId);
        return (int) this.ExecuteScalar() != 0;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException("Database", ex);
        return false;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanDatabase.RestoreSoftDeletedTestPlan");
      }
    }

    internal override bool RestoreSoftDeletedTestSuite(
      Guid projectId,
      int testPlanId,
      int testSuiteIdToRecover,
      bool restoreWorkItem,
      bool restoreForTestPlan)
    {
      this.RequestContext.TraceEnter("Database", "TestPlanDatabase.RestoreSoftDeletedTestSuite");
      try
      {
        Guid userId = this.RequestContext.GetUserId();
        this.PrepareStoredProcedure("prc_RestoreSoftDeletedTestSuite");
        this.BindGuid("@projectId", projectId);
        this.BindInt("@testPlanId", testPlanId);
        this.BindInt("@testSuiteIdToRecover", testSuiteIdToRecover);
        this.BindBoolean("@restoreWorkItem", restoreWorkItem);
        this.BindBoolean("@restoreForTestPlan", restoreForTestPlan);
        this.BindGuid("@lastUpdatedBy", userId);
        return (int) this.ExecuteScalar() != 0;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException("Database", ex);
        return false;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestPlanDatabase.RestoreSoftDeletedTestSuite");
      }
    }
  }
}
