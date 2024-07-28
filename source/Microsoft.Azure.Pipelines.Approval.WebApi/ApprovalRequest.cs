// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalRequest
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [DataContract]
  public sealed class ApprovalRequest
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid ApprovalId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalConfig Config { get; set; }

    [IgnoreDataMember]
    public JObject Context { get; set; }

    [IgnoreDataMember]
    public ApprovalOwner Owner { get; set; }
  }
}
