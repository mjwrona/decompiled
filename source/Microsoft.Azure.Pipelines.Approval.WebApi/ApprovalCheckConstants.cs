// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalCheckConstants
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using System;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  public class ApprovalCheckConstants
  {
    public const string ApprovalCheckTypeName = "Approval";
    public const string ApprovalCheckTimelineRecordType = "Checkpoint.Approval";
    public const string ApprovalCheckTypeIdString = "8C6F20A7-A545-4486-9777-F762FAFE0D4D";
    public static readonly Guid ApprovalCheckTypeId = new Guid("8C6F20A7-A545-4486-9777-F762FAFE0D4D");
    public static readonly Guid PreApprovalDefTypeId = new Guid("0F52A19B-C67E-468F-B8EB-0AE83B532C99");
    public static readonly Guid MainApprovalDefTypeId = new Guid("26014962-64A0-49F4-885B-4B874119A5CC");
    public static readonly Guid PostApprovalDefTypeId = new Guid("06441319-13FB-4756-B198-C2DA116894A4");
    public const string ApproverNameAuditFeatureFlag = "Pipelines.Checks.ApproverNameAudit";
    public const string ApproverReassignmentAuditFeatureFlag = "Pipelines.Checks.ApproverReassignmentAudit";
    public const string UseEnhancedQueryApprovalsApi = "Pipelines.Policy.UseEnhancedQueryApprovalsApi";
    public const string EnableSearchInGroupsForQueryApprovalsApi = "Pipelines.Policy.EnableSearchInGroupsForQueryApprovalsApi";
    public const string EnableSearchByEmailForQueryApprovalsApi = "Pipelines.Policy.EnableSearchByEmailForQueryApprovalsApi";
    public const string EnableDeferredApprovals = "Pipelines.Approvals.EnableDeferredApprovals";
    public const int MaxDeferralDurationInDays = 30;
  }
}
