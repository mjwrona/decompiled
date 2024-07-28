// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.ViolationsProcessor
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public class ViolationsProcessor
  {
    public string CreateErrorMessage(
      ScanResult scanResult,
      ClientTraceData ctData,
      bool prescriptiveBlockingEnabled = false,
      bool isInternalTelemetry = false,
      int maxErrorsToInclude = 5,
      string scanTargetFilePath = null)
    {
      if (scanResult == null || scanResult.Violations == null || scanResult.Violations.Count == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      int num = 1;
      foreach (Violation violation in (IEnumerable<Violation>) scanResult.Violations)
      {
        if (this.ShouldIncludeResult(violation, prescriptiveBlockingEnabled) && num++ <= maxErrorsToInclude)
          stringBuilder.Append(DevSecOpsWebApiResources.AdvancedSecurityViolationText((object) (scanTargetFilePath ?? violation.FileUri), (object) violation.LineNumber, (object) violation.StartColumn, (object) violation.EndColumn, (object) violation.ErrorCode, (object) violation.RuleName) + "\n");
        TelemetryHelper.AddToResults(ctData, "Issues", violation.ToJson());
      }
      return stringBuilder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
    }

    public bool ShouldIncludeResult(Violation violation, bool prescriptiveBlockingEnabled)
    {
      bool flag = violation.MatchState.Contains("NotSuppressed");
      if (prescriptiveBlockingEnabled && HighConfidenceSecretKinds.IsHighConfidenceSecretKind(violation))
        flag = true;
      return flag;
    }
  }
}
