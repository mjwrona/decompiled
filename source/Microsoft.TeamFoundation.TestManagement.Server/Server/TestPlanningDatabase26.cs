// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningDatabase26
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanningDatabase26 : TestPlanningDatabase25
  {
    internal TestPlanningDatabase26(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestPlanningDatabase26()
    {
    }

    internal override bool CleanDeletedTestPlans(
      int waitDaysForCleanup,
      int planArtifactsDeletionBatchSize)
    {
      this.PrepareStoredProcedure("TestManagement.prc_DeleteTestPlan");
      this.BindInt("@waitDaysForCleanup", waitDaysForCleanup);
      this.BindInt("@deletionBatchSize", planArtifactsDeletionBatchSize);
      return (int) this.ExecuteScalar() == 0;
    }

    internal override List<SuiteIdAndType> QueryTestSuites(
      Guid projectGuid,
      Dictionary<string, List<object>> parametersMap)
    {
      this.PrepareStoredProcedure("TestManagement.prc_QuerySuites");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.BindIdTypeTable("@suiteIds", parametersMap.ContainsKey("Id") ? (IEnumerable<int>) Array.ConvertAll<object, int>(parametersMap["Id"].ToArray(), TestPlanningDatabase26.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase26.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32))) : (IEnumerable<int>) Array.Empty<int>());
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.BindIdTypeTable("@planIds", parametersMap.ContainsKey("PlanId") ? (IEnumerable<int>) Array.ConvertAll<object, int>(parametersMap["PlanId"].ToArray(), TestPlanningDatabase26.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase26.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32))) : (IEnumerable<int>) Array.Empty<int>());
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.BindTestManagement_TinyIntTypeTable("@suiteTypes", parametersMap.ContainsKey("SuiteType") ? (IEnumerable<byte>) Array.ConvertAll<object, byte>(parametersMap["SuiteType"].ToArray(), TestPlanningDatabase26.\u003C\u003EO.\u003C1\u003E__ToByte ?? (TestPlanningDatabase26.\u003C\u003EO.\u003C1\u003E__ToByte = new Converter<object, byte>(Convert.ToByte))) : (IEnumerable<byte>) Array.Empty<byte>());
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.BindIdTypeTable("@requirementIds", parametersMap.ContainsKey("RequirementId") ? (IEnumerable<int>) Array.ConvertAll<object, int>(parametersMap["RequirementId"].ToArray(), TestPlanningDatabase26.\u003C\u003EO.\u003C0\u003E__ToInt32 ?? (TestPlanningDatabase26.\u003C\u003EO.\u003C0\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32))) : (IEnumerable<int>) Array.Empty<int>());
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.BindTestManagement_TinyIntTypeTable("@suiteSources", parametersMap.ContainsKey("SuiteSource") ? (IEnumerable<byte>) Array.ConvertAll<object, byte>(parametersMap["SuiteSource"].ToArray(), TestPlanningDatabase26.\u003C\u003EO.\u003C1\u003E__ToByte ?? (TestPlanningDatabase26.\u003C\u003EO.\u003C1\u003E__ToByte = new Converter<object, byte>(Convert.ToByte))) : (IEnumerable<byte>) Array.Empty<byte>());
      return this.PerformSuiteQuery();
    }

    internal override bool CleanDeletedTestSuites(int waitDaysForCleanup, int deletionBatchSize)
    {
      this.PrepareStoredProcedure("TestManagement.prc_DeleteTestSuite");
      this.BindInt("@waitDaysForCleanup", waitDaysForCleanup);
      this.BindInt("@deletionBatchSize", deletionBatchSize);
      return (int) this.ExecuteScalar() == 0;
    }
  }
}
