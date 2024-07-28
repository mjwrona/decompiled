// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalsQueryParameters
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [DataContract]
  public sealed class ApprovalsQueryParameters
  {
    private List<Guid> m_approvalIds;
    private List<Guid> m_userIds;
    private List<string> m_assignedTo;
    private ApprovalStatus m_approvalStatus;
    private int m_top;

    [DataMember(EmitDefaultValue = false)]
    public List<Guid> ApprovalIds
    {
      get => this.m_approvalIds ?? (this.m_approvalIds = new List<Guid>());
      set => this.m_approvalIds = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public List<Guid> UserIds
    {
      get => this.m_userIds ?? (this.m_userIds = new List<Guid>());
      set => this.m_userIds = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public List<string> AssignedTo
    {
      get => this.m_assignedTo ?? (this.m_assignedTo = new List<string>());
      set => this.m_assignedTo = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalStatus ApproverStatus
    {
      get => this.m_approvalStatus;
      set => this.m_approvalStatus = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public int Top
    {
      get => this.m_top;
      set => this.m_top = value;
    }
  }
}
