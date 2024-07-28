// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestDiscussionConstants
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class PullRequestDiscussionConstants
  {
    public static readonly HashSet<string> SingleTfIdKeys = new HashSet<string>()
    {
      CodeReviewDiscussionConstants.CodeReviewRefUpdatedByTfId,
      CodeReviewDiscussionConstants.CodeReviewVotedByTfId,
      CodeReviewDiscussionConstants.CodeReviewVotedByInitiatorTfId,
      CodeReviewDiscussionConstants.CodeReviewResetAllVotesInitiatorTfId,
      CodeReviewDiscussionConstants.CodeReviewResetMultipleVotesInitiatorTfId,
      CodeReviewDiscussionConstants.CodeReviewStatusUpdatedByTfId,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedByTfId,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedAddedTfId,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedChangedTfId,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedRemovedTfId,
      CodeReviewDiscussionConstants.CodeReviewSourceBranchChangedByTfId,
      CodeReviewDiscussionConstants.CodeReviewAutoCompleteUpdatedByTfId,
      CodeReviewDiscussionConstants.CodeReviewIsDraftUpdatedByTfId,
      CodeReviewDiscussionConstants.CodeReviewAssociatedStatusUpdatedByTfId
    };
    public static readonly HashSet<string> MultiTfIdKeys = new HashSet<string>()
    {
      CodeReviewDiscussionConstants.CodeReviewRequiredReviewerExampleReviewerId,
      CodeReviewDiscussionConstants.CodeReviewResetMultipleVotesExampleVoterIds
    };
    public static readonly HashSet<string> IdentityKeys = new HashSet<string>(PullRequestDiscussionConstants.SingleTfIdKeys.Concat<string>((IEnumerable<string>) PullRequestDiscussionConstants.MultiTfIdKeys))
    {
      CodeReviewDiscussionConstants.CodeReviewRefUpdatedBy,
      CodeReviewDiscussionConstants.CodeReviewRefUpdatedByDisplayName,
      CodeReviewDiscussionConstants.CodeReviewVotedByDisplayName,
      CodeReviewDiscussionConstants.CodeReviewVotedByInitiatorDisplayName,
      CodeReviewDiscussionConstants.CodeReviewResetAllVotesInitiatorDisplayName,
      CodeReviewDiscussionConstants.CodeReviewResetMultipleVotesInitiatorDisplayName,
      CodeReviewDiscussionConstants.CodeReviewResetMultipleVotesExampleVoterDisplayNames,
      CodeReviewDiscussionConstants.CodeReviewStatusUpdatedByDisplayName,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedByDisplayname,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedAddedDisplayName,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedChangedDisplayName,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedRemovedDisplayName,
      CodeReviewDiscussionConstants.CodeReviewAutoCompleteUpdatedByDisplayName,
      CodeReviewDiscussionConstants.CodeReviewIsDraftUpdatedByDisplayName,
      CodeReviewDiscussionConstants.CodeReviewAssociatedStatusUpdatedByDisplayName,
      CodeReviewDiscussionConstants.CodeReviewRequiredReviewerExampleReviewerDisplayName
    };
    public static readonly Dictionary<string, string> IdToIdentityKeyMap = new Dictionary<string, string>()
    {
      {
        CodeReviewDiscussionConstants.CodeReviewRefUpdatedByTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewRefUpdatedByIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewVotedByTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewVotedByIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewVotedByInitiatorTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewVotedByInitiatorIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewResetAllVotesInitiatorTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewResetAllVotesInitiatorIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewResetMultipleVotesInitiatorTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewResetMultipleVotesInitiatorIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewStatusUpdatedByTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewStatusUpdatedByIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedByTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewReviewersUpdatedByIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedAddedTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewReviewersUpdatedAddedIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedChangedTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewReviewersUpdatedChangedIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedRemovedTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewReviewersUpdatedRemovedIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewSourceBranchChangedByTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewSourceBranchChangedByIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewAutoCompleteUpdatedByTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewAutoCompleteUpdatedByIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewIsDraftUpdatedByTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewIsDraftUpdatedByIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewAssociatedStatusUpdatedByTfId,
        CodeReviewDiscussionIdentityConstants.CodeReviewAssociatedStatusUpdatedByIdentity
      },
      {
        CodeReviewDiscussionConstants.CodeReviewResetMultipleVotesExampleVoterIds,
        CodeReviewDiscussionIdentityConstants.CodeReviewResetMultipleVotesExampleVoterIdentities
      },
      {
        CodeReviewDiscussionConstants.CodeReviewRequiredReviewerExampleReviewerId,
        CodeReviewDiscussionIdentityConstants.CodeReviewRequiredReviewerExampleReviewerIdentities
      }
    };
    public static readonly Dictionary<string, string> IdentityToIdKeyMap = PullRequestDiscussionConstants.IdToIdentityKeyMap.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (x => x.Value), (Func<KeyValuePair<string, string>, string>) (x => x.Key));
    public static readonly HashSet<string> PropertiesToRemoveFromPR = new HashSet<string>()
    {
      "Microsoft.TeamFoundation.Discussion.ItemPath",
      "Microsoft.TeamFoundation.Discussion.Position.PositionContext",
      "Microsoft.TeamFoundation.Discussion.Position.StartLine",
      "Microsoft.TeamFoundation.Discussion.Position.StartColumn",
      "Microsoft.TeamFoundation.Discussion.Position.EndLine",
      "Microsoft.TeamFoundation.Discussion.Position.EndColumn"
    };
  }
}
