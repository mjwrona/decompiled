// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalCompletedNotificationEvent
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [DataContract]
  [ServiceEventObject]
  public class ApprovalCompletedNotificationEvent : ApprovalNotificationEventBase
  {
    public ApprovalCompletedNotificationEvent(Guid projectId, Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval)
      : base(projectId, approval)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>(approval, nameof (approval));
      ArgumentUtility.CheckForEmptyGuid(approval.Id, "approvalId");
    }

    public ApprovalCompletedNotificationEvent(
      Guid projectId,
      Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval,
      IdentityRef modifiedBy)
      : this(projectId, approval)
    {
      if (!approval.Status.Equals((object) ApprovalStatus.Skipped))
        return;
      ArgumentUtility.CheckForNull<IdentityRef>(modifiedBy, nameof (modifiedBy));
      ArgumentUtility.CheckStringForNullOrEmpty(modifiedBy.Id, "modifiedById");
      this.ModifiedBy = modifiedBy;
    }

    public IdentityRef ModifiedBy { get; set; }
  }
}
