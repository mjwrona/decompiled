// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.TraceConstants
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

namespace Microsoft.Azure.Pipelines.Approval.Server
{
  public static class TraceConstants
  {
    public const string Area = "PipelinePolicy.Approval";
    public const string ServiceLayer = "Service";
    public const string ApprovalService = "ApprovalService";
    public const int Start = 34001700;
    public const int DefaultTracePoint = 34001700;
    public const int ApprovalCompletedEventPublishInfo = 34001701;
    public const int ApprovalCompletedEventPublishError = 34001702;
    public const int ApprovalPendingEventPublishInfo = 34001703;
    public const int ApprovalPendingEventPublishError = 34001704;
    public const int ApprovalEventListenerPublishError = 34001705;
    public const int ReassignRequestInvalid = 34001706;
    public const int ReassignRequestUnauthorized = 34001707;
    public const int ReassignParametersInvalid = 34001708;
    public const int ApprovalUpdateInfo = 34001709;
    public const int ApprovalUpdateCompleteInfo = 34001710;
    public const int ApprovalNotFound = 34001711;
    public const int ApproverReassigned = 34001712;
    public const int FailedToRaiseReassignAuditEvent = 34001713;
    public const int QueryApprovalsTooManyGroups = 34001714;
    public const int QueryApprovalsInvalidIdentity = 34001715;
    public const int ApprovalSkipped = 34001716;
    public const int SkippedRequestUnauthorized = 34001717;
    public const int FailedToRaiseSkippedAuditEvent = 34001718;
    public const int DeferredApprovalJob = 34001719;
  }
}
