// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalConfig
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [DataContract]
  public class ApprovalConfig
  {
    private List<IdentityRef> m_approvers;
    private List<IdentityRef> m_blockedApprovers;

    [DataMember(EmitDefaultValue = false)]
    public List<IdentityRef> Approvers
    {
      get => this.m_approvers ?? (this.m_approvers = new List<IdentityRef>());
      set => this.m_approvers = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalExecutionOrder ExecutionOrder { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalCheckDefinitionReference DefinitionRef { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? MinRequiredApprovers { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Instructions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<IdentityRef> BlockedApprovers
    {
      get => this.m_blockedApprovers ?? (this.m_blockedApprovers = new List<IdentityRef>());
      set => this.m_blockedApprovers = value;
    }
  }
}
