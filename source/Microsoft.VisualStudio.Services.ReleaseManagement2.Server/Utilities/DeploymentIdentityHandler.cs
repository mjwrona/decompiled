// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.DeploymentIdentityHandler
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public class DeploymentIdentityHandler
  {
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Depends on number of fields returned from stored procedure")]
    public static IList<string> GetTeamFoundationIds(Deployment deployment)
    {
      if (deployment == null)
        throw new ArgumentNullException(nameof (deployment));
      List<IdentityRef> first = new List<IdentityRef>()
      {
        deployment.RequestedBy,
        deployment.RequestedFor,
        deployment.LastModifiedBy
      };
      IEnumerable<IdentityRef> second1 = deployment.PreDeployApprovals.Select<ReleaseApproval, IdentityRef>((Func<ReleaseApproval, IdentityRef>) (s => s.Approver));
      IEnumerable<IdentityRef> second2 = deployment.PreDeployApprovals.Select<ReleaseApproval, IdentityRef>((Func<ReleaseApproval, IdentityRef>) (s => s.ApprovedBy));
      IEnumerable<IdentityRef> second3 = deployment.PostDeployApprovals.Select<ReleaseApproval, IdentityRef>((Func<ReleaseApproval, IdentityRef>) (s => s.Approver));
      IEnumerable<IdentityRef> second4 = deployment.PostDeployApprovals.Select<ReleaseApproval, IdentityRef>((Func<ReleaseApproval, IdentityRef>) (s => s.ApprovedBy));
      List<IdentityRef> second5 = new List<IdentityRef>();
      List<IdentityRef> second6 = new List<IdentityRef>();
      List<IdentityRef> second7 = new List<IdentityRef>();
      List<IdentityRef> second8 = new List<IdentityRef>();
      foreach (ReleaseApproval releaseApproval in deployment.PreDeployApprovals.Where<ReleaseApproval>((Func<ReleaseApproval, bool>) (s => s.History != null)))
      {
        second5 = releaseApproval.History.Select<ReleaseApprovalHistory, IdentityRef>((Func<ReleaseApprovalHistory, IdentityRef>) (e => e.Approver)).ToList<IdentityRef>();
        second6 = releaseApproval.History.Select<ReleaseApprovalHistory, IdentityRef>((Func<ReleaseApprovalHistory, IdentityRef>) (e => e.ChangedBy)).ToList<IdentityRef>();
      }
      foreach (ReleaseApproval releaseApproval in deployment.PostDeployApprovals.Where<ReleaseApproval>((Func<ReleaseApproval, bool>) (s => s.History != null)))
      {
        second7 = releaseApproval.History.Select<ReleaseApprovalHistory, IdentityRef>((Func<ReleaseApprovalHistory, IdentityRef>) (e => e.Approver)).ToList<IdentityRef>();
        second8 = releaseApproval.History.Select<ReleaseApprovalHistory, IdentityRef>((Func<ReleaseApprovalHistory, IdentityRef>) (e => e.ChangedBy)).ToList<IdentityRef>();
      }
      return (IList<string>) first.Union<IdentityRef>(second1).Union<IdentityRef>(second2).Union<IdentityRef>(second3).Union<IdentityRef>(second4).Union<IdentityRef>((IEnumerable<IdentityRef>) second5).Union<IdentityRef>((IEnumerable<IdentityRef>) second6).Union<IdentityRef>((IEnumerable<IdentityRef>) second7).Union<IdentityRef>((IEnumerable<IdentityRef>) second8).Where<IdentityRef>((Func<IdentityRef, bool>) (id => id != null)).Select<IdentityRef, string>((Func<IdentityRef, string>) (identity => identity.Id)).Distinct<string>().ToList<string>();
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Required for functionality.")]
    public static void PopulateIdentities(
      IVssRequestContext context,
      Deployment deployment,
      IdentityHelper identityHelper)
    {
      if (deployment == null)
        throw new ArgumentNullException(nameof (deployment));
      if (identityHelper == null)
        throw new ArgumentNullException(nameof (identityHelper));
      deployment.RequestedBy = identityHelper.GetIdentity(deployment.RequestedBy);
      deployment.RequestedFor = identityHelper.GetIdentity(deployment.RequestedFor);
      deployment.LastModifiedBy = identityHelper.GetIdentity(deployment.LastModifiedBy);
      foreach (ReleaseApproval preDeployApproval in deployment.PreDeployApprovals)
      {
        preDeployApproval.Approver = identityHelper.GetIdentity(preDeployApproval.Approver);
        preDeployApproval.ApprovedBy = identityHelper.GetIdentity(preDeployApproval.ApprovedBy);
        if (preDeployApproval.History != null)
        {
          foreach (ReleaseApprovalHistory releaseApprovalHistory in preDeployApproval.History)
          {
            releaseApprovalHistory.Approver = identityHelper.GetIdentity(releaseApprovalHistory.Approver);
            releaseApprovalHistory.ChangedBy = identityHelper.GetIdentity(releaseApprovalHistory.ChangedBy);
          }
        }
      }
      foreach (ReleaseApproval postDeployApproval in deployment.PostDeployApprovals)
      {
        postDeployApproval.Approver = identityHelper.GetIdentity(postDeployApproval.Approver);
        postDeployApproval.ApprovedBy = identityHelper.GetIdentity(postDeployApproval.ApprovedBy);
        if (postDeployApproval.History != null)
        {
          foreach (ReleaseApprovalHistory releaseApprovalHistory in postDeployApproval.History)
          {
            releaseApprovalHistory.Approver = identityHelper.GetIdentity(releaseApprovalHistory.Approver);
            releaseApprovalHistory.ChangedBy = identityHelper.GetIdentity(releaseApprovalHistory.ChangedBy);
          }
        }
      }
    }
  }
}
