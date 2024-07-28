// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.RuleEngineConfiguration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class RuleEngineConfiguration : IEquatable<RuleEngineConfiguration>
  {
    public static readonly RuleEngineConfiguration ServerFull = new RuleEngineConfiguration()
    {
      ApplyMakeTrueLogic = true,
      ApplyInverseMapRules = true,
      ExpandServerDefaultValue = true,
      ExecutablePhases = RuleEnginePhase.CopyRules | RuleEnginePhase.DefaultRules | RuleEnginePhase.OtherRules
    };
    public static readonly RuleEngineConfiguration ServerFullNoInverse = new RuleEngineConfiguration()
    {
      ApplyMakeTrueLogic = true,
      ApplyInverseMapRules = false,
      ExpandServerDefaultValue = true,
      ExecutablePhases = RuleEnginePhase.CopyRules | RuleEnginePhase.DefaultRules | RuleEnginePhase.OtherRules
    };
    public static readonly RuleEngineConfiguration ServerValidationOnly = new RuleEngineConfiguration()
    {
      ApplyMakeTrueLogic = false,
      ApplyInverseMapRules = true,
      ExpandServerDefaultValue = true,
      ExecutablePhases = RuleEnginePhase.OtherRules
    };

    public bool ApplyMakeTrueLogic { get; private set; }

    public bool ApplyInverseMapRules { get; private set; }

    public bool ExpandServerDefaultValue { get; private set; }

    public RuleEnginePhase ExecutablePhases { get; private set; }

    bool IEquatable<RuleEngineConfiguration>.Equals(RuleEngineConfiguration other) => this.ApplyMakeTrueLogic == other.ApplyMakeTrueLogic && this.ApplyInverseMapRules == other.ApplyInverseMapRules && this.ExpandServerDefaultValue == other.ExpandServerDefaultValue && this.ExecutablePhases == other.ExecutablePhases;

    public override int GetHashCode()
    {
      int executablePhases = (int) this.ExecutablePhases;
      if (this.ApplyMakeTrueLogic)
        executablePhases ^= int.MinValue;
      if (this.ExpandServerDefaultValue)
        executablePhases ^= 1073741824;
      if (this.ApplyInverseMapRules)
        executablePhases ^= 536870912;
      return executablePhases;
    }

    public override bool Equals(object obj)
    {
      if (obj is RuleEngineConfiguration)
        this.Equals((object) (RuleEngineConfiguration) obj);
      return false;
    }
  }
}
