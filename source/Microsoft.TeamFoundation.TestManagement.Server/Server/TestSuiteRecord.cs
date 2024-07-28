// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSuiteRecord
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestSuiteRecord
  {
    public Guid ProjectGuid { get; internal set; }

    public int TestSuiteId { get; internal set; }

    public int ParentSuiteId { get; internal set; }

    public byte SuiteType { get; internal set; }

    public int RequirementId { get; internal set; }

    public int TestPlanId { get; internal set; }

    public TestArtifactSource DataSourceId { get; internal set; }

    public string SuitePath { get; internal set; }

    public bool IsDeleted { get; internal set; }

    public int SequenceNumber { get; internal set; }
  }
}
