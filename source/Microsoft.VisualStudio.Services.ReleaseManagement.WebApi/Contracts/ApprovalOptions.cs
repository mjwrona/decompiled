// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [GenerateAllConstants(null)]
  [DataContract]
  public class ApprovalOptions : ReleaseManagementSecuredObject
  {
    public const int ApprovalMinimumTimeoutInMinutes = 0;
    public const int ApprovalMaximumTimeoutInMinutes = 525600;

    [DataMember]
    public int? RequiredApproverCount { get; set; }

    [DataMember]
    public bool ReleaseCreatorCanBeApprover { get; set; }

    [DataMember]
    public bool AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped { get; set; }

    [DataMember]
    public bool EnforceIdentityRevalidation { get; set; }

    [DataMember]
    public int TimeoutInMinutes { get; set; }

    [DataMember]
    public ApprovalExecutionOrder ExecutionOrder { get; set; }

    public ApprovalOptions()
    {
      this.TimeoutInMinutes = 0;
      this.ExecutionOrder = ApprovalExecutionOrder.BeforeGates;
    }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj)
    {
      if (!(obj is ApprovalOptions approvalOptions))
        return false;
      int valueOrDefault1 = this.RequiredApproverCount.GetValueOrDefault();
      int valueOrDefault2 = approvalOptions.RequiredApproverCount.GetValueOrDefault();
      return this.ReleaseCreatorCanBeApprover == approvalOptions.ReleaseCreatorCanBeApprover && valueOrDefault1 == valueOrDefault2 && this.AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped == approvalOptions.AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped && this.EnforceIdentityRevalidation == approvalOptions.EnforceIdentityRevalidation && this.TimeoutInMinutes == approvalOptions.TimeoutInMinutes && this.ExecutionOrder == approvalOptions.ExecutionOrder;
    }
  }
}
