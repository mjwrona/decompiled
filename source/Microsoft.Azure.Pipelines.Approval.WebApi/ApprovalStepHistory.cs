// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalStepHistory
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [DataContract]
  public sealed class ApprovalStepHistory
  {
    [IgnoreDataMember]
    public Guid ApprovalId { get; set; }

    [IgnoreDataMember]
    public int StepId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef AssignedTo { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }
  }
}
