// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.HighConfidenceSecretKinds
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.VisualStudio.Services.DevSecOps.WebApi;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public static class HighConfidenceSecretKinds
  {
    internal const string HighConfidenceRuleId = "SEC101";

    public static bool IsHighConfidenceSecretKind(Violation violation) => violation.ConfidenceLevel == RuleConfidenceLevel.High;

    public static BlockType GetBlockType(
      bool isPrescriptiveBlockingEnabled,
      bool containsHighConfidenceSecretKinds,
      BypassType bypassType)
    {
      return isPrescriptiveBlockingEnabled && containsHighConfidenceSecretKinds ? (bypassType != BypassType.BreakGlass ? BlockType.Prescriptive : BlockType.None) : (bypassType == BypassType.None ? BlockType.Normal : BlockType.None);
    }
  }
}
