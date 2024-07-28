// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ApprovalOptionsConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ApprovalOptionsConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions FromWebApiApprovalOptions(
      this Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions approvalOptions)
    {
      if (approvalOptions == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions) null;
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions()
      {
        ReqApproverCount = approvalOptions.RequiredApproverCount,
        ReleaseCreatorCanBeApprover = approvalOptions.ReleaseCreatorCanBeApprover,
        AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped = approvalOptions.AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped,
        EnforceIdentityRevalidation = approvalOptions.EnforceIdentityRevalidation,
        TimeoutInMinutes = approvalOptions.TimeoutInMinutes,
        ExecutionOrder = approvalOptions.ExecutionOrder
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions ToWebApiApprovalOptions(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ApprovalOptions approvalOptions)
    {
      if (approvalOptions == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions) null;
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ApprovalOptions()
      {
        RequiredApproverCount = approvalOptions.ReqApproverCount,
        ReleaseCreatorCanBeApprover = approvalOptions.ReleaseCreatorCanBeApprover,
        AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped = approvalOptions.AutoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped,
        EnforceIdentityRevalidation = approvalOptions.EnforceIdentityRevalidation,
        TimeoutInMinutes = approvalOptions.TimeoutInMinutes,
        ExecutionOrder = approvalOptions.ExecutionOrder
      };
    }
  }
}
