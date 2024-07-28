// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestOutcomeUpdateRequest
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestOutcomeUpdateRequest : ITestOutcomeUpdateRequest
  {
    public TestOutcomeUpdateRequest(
      string projectName,
      int planId,
      int[] testPointIds,
      TestOutcome outcome,
      Guid userId)
      : this(projectName, planId, 0, testPointIds, outcome, userId)
    {
    }

    public TestOutcomeUpdateRequest(
      string projectName,
      int planId,
      int suiteId,
      int[] testPointIds,
      TestOutcome outcome,
      Guid userId)
    {
      this.ProjectName = projectName;
      this.PlanId = planId;
      this.TestPointIds = testPointIds;
      this.Outcome = outcome;
      this.UserId = userId;
      this.SuiteId = suiteId;
    }

    public TestOutcomeUpdateRequest()
    {
    }

    public string ProjectName { get; set; }

    public int PlanId { get; set; }

    public int SuiteId { get; set; }

    public int[] TestPointIds { get; set; }

    public TestOutcome Outcome { get; set; }

    public Guid UserId { get; set; }

    public override string ToString() => TeamFoundationSerializationUtility.SerializeToString<TestOutcomeUpdateRequest>(this);
  }
}
