// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionApprovalStep
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "We don't need additional operator overload")]
  [DataContract]
  public class ReleaseDefinitionApprovalStep : ReleaseDefinitionEnvironmentStep, IComparable
  {
    [DataMember]
    public int Rank { get; set; }

    [DataMember]
    public bool IsAutomated { get; set; }

    [DataMember]
    public bool IsNotificationOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef Approver { get; set; }

    public override int GetHashCode() => ((this.Rank * 397 ^ this.IsAutomated.GetHashCode()) * 397 ^ this.IsNotificationOn.GetHashCode()) * 397 ^ (this.Approver != null ? this.Approver.GetHashCode() : 0);

    public override bool Equals(object obj) => obj != null && !(obj.GetType() != this.GetType()) && this.CompareTo(obj) == 0;

    public int CompareTo(object obj)
    {
      if (obj == null)
        return -1;
      ReleaseDefinitionApprovalStep definitionApprovalStep = (ReleaseDefinitionApprovalStep) obj;
      return this.Id != definitionApprovalStep.Id || this.IsNotificationOn != definitionApprovalStep.IsNotificationOn || this.IsAutomated != definitionApprovalStep.IsAutomated || !this.IsAutomated && (this.Approver == null && definitionApprovalStep.Approver != null || this.Approver != null && definitionApprovalStep.Approver == null || this.Approver != null && definitionApprovalStep.Approver != null && this.Approver.Id != definitionApprovalStep.Approver.Id) ? -1 : 0;
    }
  }
}
