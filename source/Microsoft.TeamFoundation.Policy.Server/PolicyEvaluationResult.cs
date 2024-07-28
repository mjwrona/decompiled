// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyEvaluationResult
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

namespace Microsoft.TeamFoundation.Policy.Server
{
  public class PolicyEvaluationResult
  {
    internal PolicyEvaluationResult(bool isPassing, string rejectionReason, bool requiresBypass)
    {
      this.IsPassed = isPassing;
      this.RejectionReason = rejectionReason;
      this.RequiresBypass = requiresBypass;
    }

    public bool IsPassed { get; private set; }

    public bool RequiresBypass { get; private set; }

    public string RejectionReason { get; private set; }
  }
}
