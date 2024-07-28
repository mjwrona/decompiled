// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyNotificationResult
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Policy.WebApi;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public class PolicyNotificationResult : PolicyActionResult
  {
    internal PolicyNotificationResult(
      PolicyEvaluationStatus newStatus,
      TeamFoundationPolicyEvaluationRecordContext context)
      : base(newStatus, context)
    {
    }

    public static PolicyNotificationResult<TContext> Approved<TContext>(TContext context) where TContext : TeamFoundationPolicyEvaluationRecordContext => PolicyNotificationResult.Construct<TContext>(PolicyEvaluationStatus.Approved, context);

    public static PolicyNotificationResult<TContext> NotApplicable<TContext>(TContext context) where TContext : TeamFoundationPolicyEvaluationRecordContext => PolicyNotificationResult.Construct<TContext>(PolicyEvaluationStatus.NotApplicable, context);

    public static PolicyNotificationResult<TContext> Rejected<TContext>(TContext context) where TContext : TeamFoundationPolicyEvaluationRecordContext => PolicyNotificationResult.Construct<TContext>(PolicyEvaluationStatus.Rejected, context);

    public static PolicyNotificationResult<TContext> Queued<TContext>(TContext context) where TContext : TeamFoundationPolicyEvaluationRecordContext => PolicyNotificationResult.Construct<TContext>(PolicyEvaluationStatus.Queued, context);

    public static PolicyNotificationResult<TContext> Running<TContext>(TContext context) where TContext : TeamFoundationPolicyEvaluationRecordContext => PolicyNotificationResult.Construct<TContext>(PolicyEvaluationStatus.Running, context);

    public static PolicyNotificationResult<TContext> Construct<TContext>(
      PolicyEvaluationStatus status,
      TContext context)
      where TContext : TeamFoundationPolicyEvaluationRecordContext
    {
      return new PolicyNotificationResult<TContext>(status, context);
    }

    public static PolicyNotificationResult<TContext> FromCheckResult<TContext>(
      PolicyCheckResult<TContext> checkResult)
      where TContext : TeamFoundationPolicyEvaluationRecordContext
    {
      return checkResult == null ? (PolicyNotificationResult<TContext>) null : new PolicyNotificationResult<TContext>(checkResult.Status, checkResult.Context);
    }
  }
}
