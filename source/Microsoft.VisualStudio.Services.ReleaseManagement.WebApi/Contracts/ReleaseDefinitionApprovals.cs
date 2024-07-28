// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionApprovals
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseDefinitionApprovals : ReleaseManagementSecuredObject
  {
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember(EmitDefaultValue = false)]
    public List<ReleaseDefinitionApprovalStep> Approvals { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ApprovalOptions ApprovalOptions { get; set; }

    public ReleaseDefinitionApprovals() => this.Approvals = new List<ReleaseDefinitionApprovalStep>();

    public override bool Equals(object obj)
    {
      if (obj == null || obj.GetType() != this.GetType())
        return false;
      ReleaseDefinitionApprovals definitionApprovals = (ReleaseDefinitionApprovals) obj;
      List<ReleaseDefinitionApprovalStep> approvals1 = definitionApprovals.Approvals;
      ApprovalOptions approvalOptions = definitionApprovals.ApprovalOptions;
      bool flag1 = this.Approvals.Any<ReleaseDefinitionApprovalStep>() && this.Approvals.First<ReleaseDefinitionApprovalStep>().IsAutomated;
      bool flag2 = approvals1.Any<ReleaseDefinitionApprovalStep>() && approvals1.First<ReleaseDefinitionApprovalStep>().IsAutomated;
      if (this.Approvals.Any<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, bool>) (a1 => !approvals1.Any<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, bool>) (a2 => a2.Equals((object) a1))))))
        return false;
      if (this.ApprovalOptions == null && approvalOptions == null)
        return true;
      return this.ApprovalOptions != null && approvalOptions != null ? this.ApprovalOptions.Equals((object) approvalOptions) : flag2 & flag1;
    }

    public override int GetHashCode()
    {
      int hashCode = this.ApprovalOptions != null ? this.ApprovalOptions.GetHashCode() : 0;
      return this.Approvals.GetHashCode() ^ hashCode;
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.Approvals?.ForEach((Action<ReleaseDefinitionApprovalStep>) (i => i?.SetSecuredObject(token, requiredPermissions)));
      this.ApprovalOptions?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
