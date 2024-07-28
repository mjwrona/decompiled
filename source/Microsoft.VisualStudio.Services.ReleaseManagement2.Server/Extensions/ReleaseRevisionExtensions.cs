// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseRevisionExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseRevisionExtensions
  {
    public static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision> ResolveIdentityRefs(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision> revisions,
      IVssRequestContext requestContext)
    {
      if (revisions == null)
        throw new ArgumentNullException(nameof (revisions));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (!(revisions is IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision> releaseRevisionList))
        releaseRevisionList = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision>) revisions.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision>();
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision> source = releaseRevisionList;
      List<string> list = source.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision, string>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision, string>) (revision => revision.ChangedBy.Id)).ToList<string>();
      IEnumerable<ReassignedApprovalChangeDetails> reassignHistoryChangeDetails;
      IEnumerable<string> reassignedToApproverIds = ReleaseRevisionExtensions.GetReassignedToApproverIds((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision>) source, out reassignHistoryChangeDetails);
      list.AddRange(reassignedToApproverIds);
      IDictionary<string, IdentityRef> identities = list.QueryIdentities(requestContext, false);
      if (identities != null && identities.Any<KeyValuePair<string, IdentityRef>>())
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision releaseRevision in source.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision, bool>) (releaseRevision => identities.ContainsKey(releaseRevision.ChangedBy.Id))))
          releaseRevision.ChangedBy = identities[releaseRevision.ChangedBy.Id];
        if (reassignHistoryChangeDetails != null && reassignHistoryChangeDetails.Any<ReassignedApprovalChangeDetails>())
        {
          foreach (ReassignedApprovalChangeDetails approvalChangeDetails in reassignHistoryChangeDetails.Where<ReassignedApprovalChangeDetails>((Func<ReassignedApprovalChangeDetails, bool>) (change => identities.ContainsKey(change.AssignedTo.Id))))
            approvalChangeDetails.AssignedTo = identities[approvalChangeDetails.AssignedTo.Id];
        }
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision>) source;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseRevision> ToWebApiContract(
      this IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision> revisions)
    {
      return revisions == null ? (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseRevision>) null : revisions.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseRevision>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseRevision>) (releaseRevision =>
      {
        string str = releaseRevision.ChangeDetails != null ? releaseRevision.ChangeDetails.ToString() : (string) null;
        return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseRevision()
        {
          ChangedBy = releaseRevision.ChangedBy,
          ChangedDate = releaseRevision.ChangedDate,
          ChangeDetails = str,
          ChangeType = ReleaseRevisionExtensions.GetChangeType(releaseRevision.ChangeType),
          Comment = releaseRevision.Comment,
          DefinitionSnapshotRevision = releaseRevision.DefinitionSnapshotRevision,
          ReleaseId = releaseRevision.ReleaseId
        };
      }));
    }

    private static IEnumerable<string> GetReassignedToApproverIds(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision> releaseRevisions,
      out IEnumerable<ReassignedApprovalChangeDetails> reassignHistoryChangeDetails)
    {
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision> source = releaseRevisions.Where<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision, bool>) (revision => revision.ChangeDetails.Id == ReleaseHistoryMessageId.ReassignedApprovalChange));
      reassignHistoryChangeDetails = source.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision, ReassignedApprovalChangeDetails>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseRevision, ReassignedApprovalChangeDetails>) (revision => revision.ChangeDetails as ReassignedApprovalChangeDetails));
      return reassignHistoryChangeDetails.Select<ReassignedApprovalChangeDetails, string>((Func<ReassignedApprovalChangeDetails, string>) (change => change.AssignedTo.Id));
    }

    private static string GetChangeType(ReleaseHistoryChangeTypes changeType)
    {
      switch (changeType)
      {
        case ReleaseHistoryChangeTypes.Create:
          return Resources.ReleaseHistoryChangeTypesCreate;
        case ReleaseHistoryChangeTypes.Start:
          return Resources.ReleaseHistoryChangeTypesStart;
        case ReleaseHistoryChangeTypes.Update:
          return Resources.ReleaseHistoryChangeTypesUpdate;
        case ReleaseHistoryChangeTypes.Deploy:
          return Resources.ReleaseHistoryChangeTypesDeploy;
        case ReleaseHistoryChangeTypes.Approve:
          return Resources.ReleaseHistoryChangeTypesApprove;
        case ReleaseHistoryChangeTypes.Abandon:
          return Resources.ReleaseHistoryChangeTypesAbandon;
        case ReleaseHistoryChangeTypes.Delete:
          return Resources.ReleaseHistoryChangeTypesDelete;
        case ReleaseHistoryChangeTypes.Undelete:
          return Resources.ReleaseHistoryChangeTypesUndelete;
        case ReleaseHistoryChangeTypes.GateUpdate:
          return Resources.ReleaseHistoryChangeTypesGateUpdate;
        default:
          return Resources.ReleaseHistoryChangeTypesUndefined;
      }
    }
  }
}
