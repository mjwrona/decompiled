// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointOutcomeUpdateFromTestResultRequest
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPointOutcomeUpdateFromTestResultRequest
  {
    public int TestPlanId { get; set; }

    public int TestPointId { get; set; }

    public byte? FailureType { get; set; }

    public int TestRunId { get; set; }

    public int TestResultId { get; set; }

    public DateTime LastUpdated { get; set; }

    public Guid LastUpdatedBy { get; set; }

    public byte? State { get; set; }

    public byte? Outcome { get; set; }

    public int ResolutionStateId { get; set; }
  }
}
