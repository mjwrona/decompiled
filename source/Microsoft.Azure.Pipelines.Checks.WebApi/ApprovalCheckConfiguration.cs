// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.ApprovalCheckConfiguration
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [DataContract]
  public sealed class ApprovalCheckConfiguration : CheckConfiguration
  {
    public ApprovalCheckConfiguration() => this.Type = new CheckType()
    {
      Id = ApprovalCheckConstants.ApprovalCheckTypeId,
      Name = "Approval"
    };

    [DataMember(EmitDefaultValue = false)]
    public ApprovalConfigSettings Settings { get; set; }
  }
}
