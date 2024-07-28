// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanRecord
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestPlanRecord
  {
    public Guid ProjectGuid { get; internal set; }

    public int TestPlanId { get; internal set; }

    public int RootSuiteId { get; internal set; }

    public byte State { get; internal set; }

    public int BuildDefinitionId { get; internal set; }

    public string BuildUri { get; internal set; }

    public int ReleaseDefinitionId { get; internal set; }

    public int ReleaseEnvDefinitionId { get; internal set; }

    public TestArtifactSource DataSourceId { get; internal set; }
  }
}
