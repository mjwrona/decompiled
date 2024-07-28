// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ReleaseEnvironmentDefinition
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class ReleaseEnvironmentDefinition : IEquatable<ReleaseEnvironmentDefinition>
  {
    public int ReleaseDefinitionId { get; set; }

    public int ReleaseEnvDefinitionId { get; set; }

    public bool Equals(ReleaseEnvironmentDefinition other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.ReleaseDefinitionId == other.ReleaseDefinitionId && this.ReleaseEnvDefinitionId == other.ReleaseEnvDefinitionId;
    }

    public override int GetHashCode() => (391 + this.ReleaseDefinitionId) * 23 + this.ReleaseEnvDefinitionId;
  }
}
