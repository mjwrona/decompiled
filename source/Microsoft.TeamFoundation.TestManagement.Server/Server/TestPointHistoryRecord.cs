// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointHistoryRecord
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPointHistoryRecord
  {
    public TesterInfo Tester;
    private int? m_testRunId;
    private int? m_testResultId;

    public int TestPointId { get; internal set; }

    public int TestPlanId { get; internal set; }

    public int TestSuiteId { get; internal set; }

    public int TestConfigurationId { get; internal set; }

    public int Revision { get; internal set; }

    public byte State { get; internal set; }

    public bool Enabled { get; internal set; }

    public DateTime ChangedDate { get; internal set; }

    public byte TestResultOutcome { get; internal set; }

    public int TestCaseId { get; internal set; }

    public bool IsDeleted { get; internal set; }

    public int? TestRunId
    {
      get => this.m_testRunId;
      internal set => this.m_testRunId = !value.HasValue || value.Value == 0 ? new int?() : value;
    }

    public int? TestResultId
    {
      get => this.m_testResultId;
      internal set => this.m_testResultId = !value.HasValue || value.Value == 0 ? new int?() : value;
    }

    public TestArtifactSource DataSourceId { get; internal set; }
  }
}
