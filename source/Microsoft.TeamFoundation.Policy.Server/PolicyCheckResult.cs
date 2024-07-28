// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyCheckResult
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Policy.WebApi;
using System;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public class PolicyCheckResult : PolicyActionResult
  {
    internal PolicyCheckResult(
      PolicyEvaluationStatus newStatus,
      string rejectionReason,
      TeamFoundationPolicyEvaluationRecordContext context)
      : base(newStatus, context)
    {
      this.RejectionReason = rejectionReason;
    }

    public string RejectionReason { get; private set; }

    public static PolicyCheckResult<TContext> Approved<TContext>(TContext context) where TContext : TeamFoundationPolicyEvaluationRecordContext => new PolicyCheckResult<TContext>(PolicyEvaluationStatus.Approved, (string) null, context);

    public static PolicyCheckResult<TContext> NotApplicable<TContext>(TContext context) where TContext : TeamFoundationPolicyEvaluationRecordContext => new PolicyCheckResult<TContext>(PolicyEvaluationStatus.NotApplicable, (string) null, context);

    public static PolicyCheckResult<TContext> Rejected<TContext>(
      string rejectionReason,
      TContext context)
      where TContext : TeamFoundationPolicyEvaluationRecordContext
    {
      return PolicyCheckResult.RejectedCustom<TContext>(PolicyEvaluationStatus.Rejected, rejectionReason, context);
    }

    public static PolicyCheckResult<TContext> Queued<TContext>(
      string rejectionReason,
      TContext context)
      where TContext : TeamFoundationPolicyEvaluationRecordContext
    {
      return PolicyCheckResult.RejectedCustom<TContext>(PolicyEvaluationStatus.Queued, rejectionReason, context);
    }

    public static PolicyCheckResult<TContext> Running<TContext>(
      string rejectionReason,
      TContext context)
      where TContext : TeamFoundationPolicyEvaluationRecordContext
    {
      return PolicyCheckResult.RejectedCustom<TContext>(PolicyEvaluationStatus.Running, rejectionReason, context);
    }

    internal static PolicyCheckResult Broken(string rejectionReason, Guid errorCode)
    {
      BrokenPolicyEvaluationRecordContext context = new BrokenPolicyEvaluationRecordContext(errorCode);
      return new PolicyCheckResult(PolicyEvaluationStatus.Broken, rejectionReason, (TeamFoundationPolicyEvaluationRecordContext) context);
    }

    public static PolicyCheckResult<TContext> RejectedCustom<TContext>(
      PolicyEvaluationStatus newStatus,
      string rejectionReason,
      TContext context)
      where TContext : TeamFoundationPolicyEvaluationRecordContext
    {
      return new PolicyCheckResult<TContext>(newStatus, rejectionReason, context);
    }
  }
}
