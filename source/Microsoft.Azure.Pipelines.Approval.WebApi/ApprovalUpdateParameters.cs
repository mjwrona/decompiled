// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalUpdateParameters
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [DataContract]
  public sealed class ApprovalUpdateParameters
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid ApprovalId { get; set; }

    [IgnoreDataMember]
    public int? StepId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalStatus? Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef AssignedApprover { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ReassignTo { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? DeferredTo { get; set; }
  }
}
