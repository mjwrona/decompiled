// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ReviewerExtensions
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  public static class ReviewerExtensions
  {
    private static readonly Dictionary<short, string> s_defaultReviewerStates = new Dictionary<short, string>()
    {
      {
        (short) 0,
        CodeReviewWebAPIResources.NoResponseReviewerState()
      },
      {
        (short) 10,
        CodeReviewWebAPIResources.ApprovedReviewerState()
      },
      {
        (short) 5,
        CodeReviewWebAPIResources.ApprovedWithCommentsReviewerState()
      },
      {
        (short) -5,
        CodeReviewWebAPIResources.CodeNotReadyYetReviewerState()
      },
      {
        (short) -10,
        CodeReviewWebAPIResources.RejectedReviewerState()
      },
      {
        (short) -15,
        CodeReviewWebAPIResources.DeclinedReviewerState()
      }
    };

    public static List<Reviewer> GetSortedReviewers(IList<Reviewer> allReviewers)
    {
      List<Reviewer> list = allReviewers.ToList<Reviewer>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      list.Sort(ReviewerExtensions.\u003C\u003EO.\u003C0\u003E__ReviewerVoteComparer ?? (ReviewerExtensions.\u003C\u003EO.\u003C0\u003E__ReviewerVoteComparer = new Comparison<Reviewer>(ReviewerExtensions.ReviewerVoteComparer)));
      return list;
    }

    public static bool IsValid(this ReviewerState reviewerState) => reviewerState != null && ReviewerExtensions.IsValid(reviewerState.Value);

    public static bool IsValid(short stateId) => Enum.IsDefined(typeof (DefaultReviewerStates), (object) stateId);

    public static bool HasInitialState(this Reviewer reviewer) => reviewer.ReviewerStateId.HasValue && ReviewerExtensions.IsInitialState(ReviewerExtensions.GetValidStateId(reviewer.ReviewerStateId.Value));

    public static bool HasDeclined(this Reviewer reviewer) => reviewer.ReviewerStateId.HasValue && ReviewerExtensions.GetValidStateId(reviewer.ReviewerStateId.Value) == (short) -15;

    public static bool HasApproved(this Reviewer reviewer)
    {
      if (!reviewer.ReviewerStateId.HasValue)
        return false;
      short validStateId = ReviewerExtensions.GetValidStateId(reviewer.ReviewerStateId.Value);
      return validStateId == (short) 10 || validStateId == (short) 5;
    }

    public static bool HasDisapproved(this Reviewer reviewer)
    {
      if (!reviewer.ReviewerStateId.HasValue)
        return false;
      short validStateId = ReviewerExtensions.GetValidStateId(reviewer.ReviewerStateId.Value);
      return validStateId == (short) -10 || validStateId == (short) -5;
    }

    [Obsolete]
    public static bool HasRejected(this Reviewer reviewer) => reviewer.HasDisapproved();

    public static bool HasExplicitlyRejected(this Reviewer reviewer) => reviewer.ReviewerStateId.HasValue && ReviewerExtensions.GetValidStateId(reviewer.ReviewerStateId.Value) == (short) -10;

    public static bool HasWaiting(this Reviewer reviewer) => reviewer.ReviewerStateId.HasValue && ReviewerExtensions.GetValidStateId(reviewer.ReviewerStateId.Value) == (short) -5;

    public static bool HasPendingResponse(this Reviewer reviewer) => reviewer.ReviewerStateId.HasValue && ReviewerExtensions.IsPendingResponseState(ReviewerExtensions.GetValidStateId(reviewer.ReviewerStateId.Value));

    public static ReviewerState GetDefaultState(short stateId) => ReviewerExtensions.IsValid(stateId) ? new ReviewerState()
    {
      Value = stateId,
      Description = ReviewerExtensions.s_defaultReviewerStates[stateId]
    } : throw new ArgumentOutOfRangeException(CodeReviewWebAPIResources.InvalidReviewerStateId((object) stateId));

    internal static bool IsPendingResponseState(short stateId) => stateId == (short) 0 || stateId == (short) -15;

    internal static bool IsInitialState(short stateId) => stateId == (short) 0;

    internal static bool IsDeclinedState(short stateId) => stateId == (short) -15;

    private static short GetValidStateId(short reviewerStateId) => ReviewerExtensions.IsValid(reviewerStateId) ? reviewerStateId : throw new ArgumentOutOfRangeException(CodeReviewWebAPIResources.InvalidReviewerStateId((object) reviewerStateId));

    private static int ReviewerVoteComparer(Reviewer a, Reviewer b)
    {
      ReviewerKind? kind1 = a.Kind;
      ReviewerKind reviewerKind1 = ReviewerKind.Required;
      if (kind1.GetValueOrDefault() == reviewerKind1 & kind1.HasValue)
      {
        ReviewerKind? kind2 = b.Kind;
        ReviewerKind reviewerKind2 = ReviewerKind.Required;
        if (!(kind2.GetValueOrDefault() == reviewerKind2 & kind2.HasValue))
          return -1;
      }
      ReviewerKind? kind3 = a.Kind;
      ReviewerKind reviewerKind3 = ReviewerKind.Required;
      if (!(kind3.GetValueOrDefault() == reviewerKind3 & kind3.HasValue))
      {
        ReviewerKind? kind4 = b.Kind;
        ReviewerKind reviewerKind4 = ReviewerKind.Required;
        if (kind4.GetValueOrDefault() == reviewerKind4 & kind4.HasValue)
          return 1;
      }
      if (a.ReviewerStateId.Value != (short) 0 && b.ReviewerStateId.Value == (short) 0)
        return -1;
      if (a.ReviewerStateId.Value == (short) 0 && b.ReviewerStateId.Value != (short) 0)
        return 1;
      if (a.Identity.IsContainer && !b.Identity.IsContainer)
        return -1;
      if (!a.Identity.IsContainer && b.Identity.IsContainer)
        return 1;
      int num1 = (int) a.ReviewerStateId.Value;
      short? reviewerStateId = b.ReviewerStateId;
      int num2 = (int) reviewerStateId.Value;
      if (num1 == num2)
        return ReviewerExtensions.FirstAlphabeticalCompare(a, b);
      reviewerStateId = a.ReviewerStateId;
      int num3 = (int) reviewerStateId.Value;
      reviewerStateId = b.ReviewerStateId;
      int num4 = (int) reviewerStateId.Value;
      return num3 - num4;
    }

    private static int FirstAlphabeticalCompare(Reviewer x, Reviewer y)
    {
      if (x == null)
        return y == null ? 0 : -1;
      if (y == null)
        return 1;
      if (x.Identity.IsContainer && !y.Identity.IsContainer)
        return -1;
      return !x.Identity.IsContainer && y.Identity.IsContainer ? 1 : string.Compare(x.Identity.DisplayName, y.Identity.DisplayName, StringComparison.OrdinalIgnoreCase);
    }
  }
}
