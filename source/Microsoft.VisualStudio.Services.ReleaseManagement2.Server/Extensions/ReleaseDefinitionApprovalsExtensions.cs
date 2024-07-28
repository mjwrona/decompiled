// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionApprovalsExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseDefinitionApprovalsExtensions
  {
    public static ReleaseDefinitionApprovals PopulateAutomatedApprovalIfApprovalsNotExist(
      this ReleaseDefinitionApprovals releaseDefinitionApproval)
    {
      if (releaseDefinitionApproval == null)
      {
        List<ReleaseDefinitionApprovalStep> definitionApprovalStepList = new List<ReleaseDefinitionApprovalStep>()
        {
          ReleaseDefinitionApprovalStepExtensions.GetAutomatedApprovalStep()
        };
        releaseDefinitionApproval = new ReleaseDefinitionApprovals()
        {
          Approvals = definitionApprovalStepList,
          ApprovalOptions = (ApprovalOptions) null
        };
      }
      return releaseDefinitionApproval;
    }
  }
}
