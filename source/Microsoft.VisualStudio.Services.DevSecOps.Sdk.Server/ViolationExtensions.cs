// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.ViolationExtensions
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.CodeAnalysis.Sarif;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public static class ViolationExtensions
  {
    internal const string SecretHashSha256Current = "secretHashSha256/v0";
    internal const string ValidationFingerprintHashSha256Current = "validationFingerprintHashSha256/v0";

    public static bool TryCreateViolation(
      this Result result,
      ReportingDescriptor rule,
      out Violation violation)
    {
      try
      {
        LogicalLocation logicalLocation = (LogicalLocation) null;
        PhysicalLocation physicalLocation = (PhysicalLocation) null;
        if (result.Locations != null && result.Locations.Count > 0)
        {
          logicalLocation = result.Locations[0].LogicalLocation;
          physicalLocation = result.Locations[0].PhysicalLocation;
        }
        ref Violation local = ref violation;
        Violation violation1 = new Violation();
        violation1.Id = Guid.NewGuid();
        violation1.FileUri = physicalLocation?.ArtifactLocation?.Uri?.OriginalString;
        violation1.ErrorCode = result.RuleId;
        violation1.RuleName = rule.Name;
        violation1.MatchContent = physicalLocation?.Region?.Snippet?.Text;
        violation1.MatchSecretHash = result?.Fingerprints?["secretHashSha256/v0"];
        violation1.MatchContentHash = result?.Fingerprints?["validationFingerprintHashSha256/v0"];
        int? nullable = physicalLocation?.Region?.StartLine;
        violation1.LineNumber = nullable ?? -1;
        nullable = physicalLocation?.Region?.StartColumn;
        violation1.StartColumn = nullable ?? -1;
        nullable = physicalLocation?.Region?.EndColumn;
        violation1.EndColumn = nullable ?? -1;
        violation1.MatchDetails = physicalLocation?.Region?.Snippet?.Text;
        violation1.SubRuleId = "1";
        violation1.MatchState = "NotSuppressed";
        violation1.LogicalLocation = logicalLocation?.FullyQualifiedName ?? string.Empty;
        local = violation1;
      }
      catch (Exception ex)
      {
        violation = (Violation) null;
        return false;
      }
      return true;
    }
  }
}
