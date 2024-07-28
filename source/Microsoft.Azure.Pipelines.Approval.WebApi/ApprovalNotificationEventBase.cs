// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalNotificationEventBase
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using System;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  public abstract class ApprovalNotificationEventBase
  {
    public ApprovalNotificationEventBase(Guid projectId, Microsoft.Azure.Pipelines.Approval.WebApi.Approval approval)
    {
      this.ProjectId = projectId;
      this.Approval = approval;
    }

    public Microsoft.Azure.Pipelines.Approval.WebApi.Approval Approval { get; set; }

    public Guid ProjectId { get; set; }
  }
}
