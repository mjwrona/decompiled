// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ReleaseEnvironmentStepIdentityHandler
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public class ReleaseEnvironmentStepIdentityHandler
  {
    private readonly Func<IVssRequestContext, IList<string>, IdentityHelper> getIdentityHelper;

    public ReleaseEnvironmentStepIdentityHandler()
      : this(ReleaseEnvironmentStepIdentityHandler.\u003C\u003EO.\u003C0\u003E__GetIdentityHelper ?? (ReleaseEnvironmentStepIdentityHandler.\u003C\u003EO.\u003C0\u003E__GetIdentityHelper = new Func<IVssRequestContext, IList<string>, IdentityHelper>(IdentityHelper.GetIdentityHelper)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is the intended design")]
    protected ReleaseEnvironmentStepIdentityHandler(
      Func<IVssRequestContext, IList<string>, IdentityHelper> getIdentityHelper)
    {
      this.getIdentityHelper = getIdentityHelper;
    }

    public void PopulateReleaseApprovalsContract(
      IVssRequestContext context,
      IEnumerable<ReleaseEnvironmentStep> serverApprovals,
      IEnumerable<ReleaseApproval> releaseApprovals,
      Guid projectId)
    {
      IList<string> stringList = releaseApprovals != null ? ReleaseEnvironmentStepIdentityHandler.GetUniqueTeamFoundationIds(releaseApprovals) : throw new ArgumentNullException(nameof (serverApprovals));
      IdentityHelper identityHelper = this.getIdentityHelper(context, stringList);
      foreach (ReleaseApproval releaseApproval1 in releaseApprovals)
      {
        ReleaseApproval releaseApproval = releaseApproval1;
        ReleaseEnvironmentStepIdentityHandler.PopulateApprovalIdentities(releaseApproval, identityHelper);
        ReleaseEnvironmentStepIdentityHandler.PopulateShallowReferences(context, releaseApproval, serverApprovals.First<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (i => i.Id == releaseApproval.Id)), projectId);
      }
    }

    public void PopulateReleaseApprovalContract(
      IVssRequestContext context,
      ReleaseEnvironmentStep serverApproval,
      ReleaseApproval releaseApproval,
      Guid projectId)
    {
      if (releaseApproval == null)
        throw new ArgumentNullException(nameof (releaseApproval));
      if (serverApproval == null)
        throw new ArgumentNullException(nameof (serverApproval));
      IList<string> teamFoundationIds = ReleaseEnvironmentStepIdentityHandler.GetUniqueTeamFoundationIds(releaseApproval);
      IdentityHelper identityHelper = this.getIdentityHelper(context, teamFoundationIds);
      ReleaseEnvironmentStepIdentityHandler.PopulateApprovalIdentities(releaseApproval, identityHelper);
      ReleaseEnvironmentStepIdentityHandler.PopulateShallowReferences(context, releaseApproval, serverApproval, projectId);
    }

    private static void PopulateApprovalIdentities(
      ReleaseApproval releaseApproval,
      IdentityHelper identityHelper)
    {
      releaseApproval.Approver = identityHelper.GetIdentity(releaseApproval.Approver);
      releaseApproval.ApprovedBy = identityHelper.GetIdentity(releaseApproval.ApprovedBy);
      if (releaseApproval.History == null)
        return;
      foreach (ReleaseApprovalHistory releaseApprovalHistory in releaseApproval.History)
      {
        releaseApprovalHistory.Approver = identityHelper.GetIdentity(releaseApprovalHistory.Approver);
        if (releaseApprovalHistory.ChangedBy != null)
          releaseApprovalHistory.ChangedBy = identityHelper.GetIdentity(releaseApprovalHistory.ChangedBy);
      }
    }

    private static IList<string> GetUniqueTeamFoundationIds(
      IEnumerable<ReleaseApproval> releaseApprovals)
    {
      HashSet<string> source = new HashSet<string>();
      foreach (ReleaseApproval releaseApproval in releaseApprovals)
      {
        IList<string> teamFoundationIds = ReleaseEnvironmentStepIdentityHandler.GetUniqueTeamFoundationIds(releaseApproval);
        source.UnionWith((IEnumerable<string>) teamFoundationIds);
      }
      return (IList<string>) source.ToList<string>();
    }

    private static IList<string> GetUniqueTeamFoundationIds(ReleaseApproval releaseApproval)
    {
      List<string> source = new List<string>()
      {
        releaseApproval.Approver.Id
      };
      if (releaseApproval.ApprovedBy != null)
        source.Add(releaseApproval.ApprovedBy.Id);
      if (releaseApproval.History != null)
      {
        source.AddRange(releaseApproval.History.Select<ReleaseApprovalHistory, string>((Func<ReleaseApprovalHistory, string>) (e => e.Approver.Id)));
        source.AddRange(releaseApproval.History.Where<ReleaseApprovalHistory>((Func<ReleaseApprovalHistory, bool>) (e => e.ChangedBy != null)).Select<ReleaseApprovalHistory, string>((Func<ReleaseApprovalHistory, string>) (e => e.ChangedBy.Id)));
      }
      return (IList<string>) source.Distinct<string>().ToList<string>();
    }

    private static void PopulateShallowReferences(
      IVssRequestContext context,
      ReleaseApproval releaseApproval,
      ReleaseEnvironmentStep serverApproval,
      Guid projectId)
    {
      releaseApproval.ReleaseReference = ShallowReferencesHelper.CreateReleaseShallowReference(context, projectId, serverApproval.ReleaseId, serverApproval.ReleaseName);
      releaseApproval.ReleaseDefinitionReference = ShallowReferencesHelper.CreateDefinitionShallowReference(context, projectId, serverApproval.ReleaseDefinitionId, serverApproval.ReleaseDefinitionName, serverApproval.ReleaseDefinitionPath);
      releaseApproval.ReleaseEnvironmentReference = ShallowReferencesHelper.CreateReleaseEnvironmentShallowReference(context, projectId, serverApproval.ReleaseId, serverApproval.ReleaseEnvironmentId, serverApproval.ReleaseEnvironmentName);
      releaseApproval.Url = WebAccessUrlBuilder.GetReleaseApprovalRestUri(context, projectId, releaseApproval.Id);
    }
  }
}
