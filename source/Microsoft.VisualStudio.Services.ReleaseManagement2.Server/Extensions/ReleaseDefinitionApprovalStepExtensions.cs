// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionApprovalStepExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseDefinitionApprovalStepExtensions
  {
    public static Guid GetApproverId(this ReleaseDefinitionApprovalStep approval)
    {
      Guid result;
      return approval == null || approval.Approver == null || !Guid.TryParse(approval.Approver.Id, out result) ? Guid.Empty : result;
    }

    public static bool IsAssignmentValid(this ReleaseDefinitionApprovalStep approval) => approval != null && (approval.IsAutomated || approval.GetApproverId() != Guid.Empty);

    public static bool AreApproversAssignmentValid(
      this IEnumerable<ReleaseDefinitionApprovalStep> approvals)
    {
      return approvals.All<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, bool>) (approval => approval.IsAssignmentValid()));
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "this is function name")]
    public static ReleaseDefinitionApprovalStep GetAutomatedApprovalStep()
    {
      ReleaseDefinitionApprovalStep automatedApprovalStep = new ReleaseDefinitionApprovalStep();
      automatedApprovalStep.Id = 0;
      automatedApprovalStep.IsAutomated = true;
      automatedApprovalStep.IsNotificationOn = false;
      automatedApprovalStep.Rank = 1;
      return automatedApprovalStep;
    }

    public static void ValidateApprovalSteps(
      ICollection<ReleaseDefinitionApprovalStep> approvalSteps,
      string environmentName)
    {
      string empty = string.Empty;
      if (approvalSteps == null || approvalSteps.Count == 0)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseApprovalsCannotBeEmpty, (object) environmentName));
      if (!ReleaseDefinitionApprovalStepExtensions.AreStepRanksValid((IList<int>) approvalSteps.Select<ReleaseDefinitionApprovalStep, int>((Func<ReleaseDefinitionApprovalStep, int>) (s => s.Rank)).ToList<int>()))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseApprovalRanksNeedToBeCorrect, (object) environmentName));
      int num = approvalSteps.Any<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, bool>) (s => s.IsAutomated)) ? 1 : 0;
      if ((num & (approvalSteps.Any<ReleaseDefinitionApprovalStep>((Func<ReleaseDefinitionApprovalStep, bool>) (s => !s.IsAutomated)) ? 1 : 0)) != 0)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.AllReleaseApprovalsEitherAutomatedOrNonAutomated, (object) environmentName));
      if (num != 0)
      {
        if (approvalSteps.Count != 1)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MoreThanOneAutomatedReleaseApproval, (object) environmentName));
      }
      else if (!approvalSteps.AreApproversAssignmentValid())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseApproverCannotBeEmpty, (object) environmentName));
    }

    private static bool AreStepRanksValid(IList<int> ranks)
    {
      List<int> list = ranks.Distinct<int>().ToList<int>();
      return list.OrderBy<int, int>((Func<int, int>) (r => r)).SequenceEqual<int>(Enumerable.Range(1, list.Count<int>()));
    }
  }
}
