// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.CommitSuppressionEvaluator
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public class CommitSuppressionEvaluator
  {
    private const string c_Layer = "CommitSuppressionEvaluator";

    public BypassType ContainsCommitSuppression(
      IVssRequestContext requestContext,
      string commitMessage,
      string commitHash,
      ClientTraceData ctData)
    {
      requestContext.TraceInfo(27009017, nameof (CommitSuppressionEvaluator), "Checking for commit suppressions in commit {0} with message {1}.", (object) commitHash, (object) commitMessage);
      if (commitMessage == null)
        return BypassType.None;
      BypassType bypassType = BypassType.None;
      if (commitMessage.IndexOf("**BYPASS_SECRET_SCANNING**", StringComparison.OrdinalIgnoreCase) >= 0 || commitMessage.IndexOf("skip-secret-scanning:true", StringComparison.OrdinalIgnoreCase) >= 0 || commitMessage.IndexOf("**DISABLE_SECRET_SCANNING**", StringComparison.OrdinalIgnoreCase) >= 0)
        bypassType = BypassType.Normal;
      else if (commitMessage.IndexOf("4CE71094-6DCC-41B0-A1FA-CC3EF3228F4E", StringComparison.OrdinalIgnoreCase) >= 0)
        bypassType = BypassType.BreakGlass;
      if (bypassType != BypassType.None)
        ctData.Add("AppliedCommitSuppression", (object) commitHash);
      return bypassType;
    }
  }
}
