// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyActionResult
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Policy.WebApi;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public class PolicyActionResult
  {
    public PolicyEvaluationStatus Status { get; private set; }

    public TeamFoundationPolicyEvaluationRecordContext Context { get; private set; }

    internal PolicyActionResult(
      PolicyEvaluationStatus newStatus,
      TeamFoundationPolicyEvaluationRecordContext context)
    {
      this.Status = newStatus;
      this.Context = context;
    }
  }
}
