// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalStep
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [DataContract]
  public sealed class ApprovalStep
  {
    private IList<ApprovalStepHistory> m_stepHistory;

    [IgnoreDataMember]
    public int StepId { get; set; }

    [IgnoreDataMember]
    public Guid ApprovalId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef AssignedApprover { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ActualApprover { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? DeferredTo { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime LastModifiedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Order { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef LastModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? InitiatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalPermissions? Permissions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<ApprovalStepHistory> History
    {
      get => this.m_stepHistory ?? (this.m_stepHistory = (IList<ApprovalStepHistory>) new List<ApprovalStepHistory>());
      set => this.m_stepHistory = value;
    }
  }
}
