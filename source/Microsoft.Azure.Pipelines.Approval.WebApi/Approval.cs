// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.Approval
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [DataContract]
  public sealed class Approval
  {
    private List<ApprovalStep> m_approvalSteps;
    private List<IdentityRef> m_blockedApprovers;

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<ApprovalStep> Steps
    {
      get => this.m_approvalSteps ?? (this.m_approvalSteps = new List<ApprovalStep>());
      set => this.m_approvalSteps = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastModifiedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Instructions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalExecutionOrder ExecutionOrder { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? MinRequiredApprovers { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<IdentityRef> BlockedApprovers
    {
      get => this.m_blockedApprovers ?? (this.m_blockedApprovers = new List<IdentityRef>());
      set => this.m_blockedApprovers = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalPermissions? Permissions { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [IgnoreDataMember]
    public ApprovalOwner Owner { get; set; }

    [IgnoreDataMember]
    public JObject Context { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public JObject Pipeline { get; set; }

    public bool IsCompleted => (this.Status & ApprovalStatus.Completed) != 0;
  }
}
