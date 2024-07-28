// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentStepExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseEnvironmentStepExtensions
  {
    private static readonly ReleaseEnvironmentStepIdentityHandler IdentityHandler = new ReleaseEnvironmentStepIdentityHandler();

    public static IEnumerable<ReleaseApproval> ToReleaseApprovalContract(
      this IEnumerable<ReleaseEnvironmentStep> serverApprovals,
      IVssRequestContext context,
      IEnumerable<ReleaseEnvironmentStep> approvalHistorySteps,
      Guid projectId)
    {
      List<ReleaseApproval> list = serverApprovals.Select<ReleaseEnvironmentStep, ReleaseApproval>((Func<ReleaseEnvironmentStep, ReleaseApproval>) (i => i.ToWebApiApprovalWithHistory(approvalHistorySteps))).ToList<ReleaseApproval>();
      ReleaseEnvironmentStepExtensions.IdentityHandler.PopulateReleaseApprovalsContract(context, serverApprovals, (IEnumerable<ReleaseApproval>) list, projectId);
      return (IEnumerable<ReleaseApproval>) list;
    }

    public static ReleaseApproval ToReleaseApprovalContract(
      this ReleaseEnvironmentStep serverApproval,
      IVssRequestContext context,
      IEnumerable<ReleaseEnvironmentStep> approvalHistorySteps,
      Guid projectId)
    {
      ReleaseApproval approvalWithHistory = serverApproval.ToWebApiApprovalWithHistory(approvalHistorySteps);
      ReleaseEnvironmentStepExtensions.IdentityHandler.PopulateReleaseApprovalContract(context, serverApproval, approvalWithHistory, projectId);
      return approvalWithHistory;
    }

    public static bool HasHistory(this ReleaseEnvironmentStep step) => step != null && step.GroupStepId != 0 && step.Status != ReleaseEnvironmentStepStatus.Reassigned;
  }
}
