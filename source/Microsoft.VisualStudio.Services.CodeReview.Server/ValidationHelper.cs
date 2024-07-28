// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ValidationHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public static class ValidationHelper
  {
    private const int c_minTop = 1;
    private const int c_minSkip = 0;
    private const int c_defaultTop = 100;
    private const int c_defaultSkip = 0;
    private const int c_maxTop = 1000;

    internal static void ValidateCommonIterationArguments(
      Iteration iteration,
      bool isIterationUpdate = false)
    {
      DateTime? nullable = iteration.Author == null ? iteration.CreatedDate : throw new ArgumentException(CodeReviewResources.CannotSpecifyAuthor(), "author");
      nullable = !nullable.HasValue ? iteration.UpdatedDate : throw new ArgumentException(CodeReviewResources.CannotSpecifyCreatedDate(), "createdDate");
      if (nullable.HasValue)
        throw new ArgumentException(CodeReviewResources.CannotSpecifyUpdatedDate(), "updatedDate");
      int? id = iteration.Id;
      if (!id.HasValue)
        throw new CodeReviewIterationArgumentNullException("id");
      if (isIterationUpdate && iteration.ChangeList != null)
      {
        id = iteration.Id;
        throw new CodeReviewChangesAlreadyExistException(id.Value);
      }
      if (iteration.Statuses == null || !iteration.Statuses.Any<Status>())
        return;
      if (isIterationUpdate)
        throw new ArgumentException(CodeReviewResources.StatusesCannotBeUpdated(), "statusList");
      throw new ArgumentException(CodeReviewResources.StatusesCannotBeCreated(), "statusList");
    }

    internal static void ValidateReviewers(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requesterIdentity,
      IEnumerable<Reviewer> reviewers,
      IList<Reviewer> existingReviewers)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Reviewer>>(reviewers, nameof (reviewers));
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (Reviewer reviewer in reviewers)
      {
        ArgumentUtility.CheckForNull<Reviewer>(reviewer, "reviewer");
        if (reviewer.Identity == null)
          throw new ArgumentException(CodeReviewResources.ReviewerIdArgumentNullException());
        Guid result;
        if (string.IsNullOrEmpty(reviewer.Identity.Id) || !Guid.TryParse(reviewer.Identity.Id, out result))
          throw new ArgumentException(CodeReviewResources.ReviewerIdArgumentValidGuid());
        if (ValidationHelper.IsNewReviewer(existingReviewers, reviewer.Identity.Id))
        {
          if (!reviewer.Kind.HasValue)
            reviewer.Kind = new ReviewerKind?(ReviewerKind.Optional);
          if (!reviewer.ReviewerStateId.HasValue)
            reviewer.ReviewerStateId = new short?((short) 0);
        }
        else if (!reviewer.Kind.HasValue && !reviewer.ReviewerStateId.HasValue)
          throw new ArgumentException(CodeReviewResources.ReviewerStateOrKindMustBeSetForUpdate((object) reviewer.Identity.Id));
        if (reviewer.ReviewerStateId.HasValue && !ReviewerExtensions.IsValid(reviewer.ReviewerStateId.Value))
          throw new ArgumentException(CodeReviewResources.InvalidReviewerStateId((object) reviewer.ReviewerStateId, (object) reviewer.Identity.Id));
        if (reviewer.ReviewerStateId.HasValue && !ReviewerExtensions.IsInitialState(reviewer.ReviewerStateId.Value) && requesterIdentity.Id != result)
          throw new ArgumentException(CodeReviewResources.CannotVoteForAnotherReviewer((object) requesterIdentity.Id, (object) reviewer.ReviewerStateId, (object) reviewer.Identity.Id));
        if (reviewer.IterationId.HasValue)
          ArgumentUtility.CheckForOutOfRange(reviewer.IterationId.Value, "iterationId", 1);
        if (!source.Add(result))
          throw new ArgumentException(CodeReviewResources.DuplicateReviewersInReviewerList((object) result));
      }
      IList<Guid> invalidIdentities = Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.IdentityHelper.GetInvalidIdentities(requestContext, (IList<Guid>) source.ToList<Guid>());
      if (invalidIdentities.Count > 0)
        throw new ArgumentException(CodeReviewResources.InvalidReviewers((object) string.Join<Guid>(",", (IEnumerable<Guid>) invalidIdentities)), nameof (reviewers));
    }

    public static void ProcessIterationAndChanges(
      IVssRequestContext requestContext,
      Iteration iteration,
      int maxChangeEntries)
    {
      int? id = iteration.Id;
      int num = 1;
      if (id.GetValueOrDefault() > num & id.HasValue)
        throw new ArgumentException(CodeReviewResources.IterationCannotBeCreated(), "Iterations");
      ValidationHelper.ValidateCommonIterationArguments(iteration);
      bool changeTrackingId = FileChangeUtilities.ShouldComputeChangeTrackingId(iteration.Id.Value, (IEnumerable<ChangeEntry>) iteration.ChangeList);
      IList<ChangeEntry> changeEntriesToCompute = ValidationHelper.SanitizeChangeEntries(iteration.Id, iteration.ChangeList, changeTrackingId, maxChangeEntries);
      if (!changeTrackingId)
        return;
      Stopwatch stopwatch = Stopwatch.StartNew();
      new CodeReviewChangeTracker().ComputeChangeTrackingIds(changeEntriesToCompute);
      TelemetryHelper.Publish(requestContext, "Review", new List<Tuple<string, string>>()
      {
        new Tuple<string, string>("KeyCodeReviewChangeTrackingIdComputationElapsedMs", stopwatch.ElapsedMilliseconds.ToString())
      });
    }

    internal static void ValidateDownloadContentsCriteria(
      DownloadContentsCriteria downloadContentsCriteria)
    {
      ArgumentUtility.CheckForNull<DownloadContentsCriteria>(downloadContentsCriteria, "downloadContentCriteria");
      if (downloadContentsCriteria.ContentHashes != null && downloadContentsCriteria.ContentHashes.Count > 0)
      {
        ArgumentUtility.EnsureIsNull((object) downloadContentsCriteria.IterationIds, "iterationIds");
        ArgumentUtility.EnsureIsNull((object) downloadContentsCriteria.FilterBy, "filterBy");
      }
      else
      {
        if (downloadContentsCriteria.IterationIds == null || downloadContentsCriteria.IterationIds.Count <= 0)
          throw new ArgumentException(CodeReviewResources.EmptyDownloadContentCriteriaNotAllowed());
        ArgumentUtility.EnsureIsNull((object) downloadContentsCriteria.ContentHashes, "contentHashes");
        ArgumentUtility.EnsureIsNull((object) downloadContentsCriteria.FileType, "reviewFileType");
      }
    }

    internal static Iteration SanitizeIterationInput(
      IVssRequestContext requestContext,
      Iteration iteration,
      bool updateIteration,
      bool reviewExists = true)
    {
      if (!updateIteration && iteration.Description == null)
        iteration.Description = string.Empty;
      if (!updateIteration & reviewExists)
        iteration.Author = IdentityHelper.GetRequesterIdentityRef(requestContext);
      return iteration;
    }

    internal static IList<ChangeEntry> SanitizeChangeEntries(
      int? iterationId,
      IList<ChangeEntry> changeList,
      bool shouldComputeChangeTrackingId,
      int maxChangeEntriesInUnsavedIteration)
    {
      if (changeList == null)
        return (IList<ChangeEntry>) null;
      bool flag = shouldComputeChangeTrackingId && changeList.Count > maxChangeEntriesInUnsavedIteration;
      IList<ChangeEntry> changeEntryList = flag ? (IList<ChangeEntry>) new List<ChangeEntry>(maxChangeEntriesInUnsavedIteration) : changeList;
      HashSet<int> uniqueChangeTrackingIds = new HashSet<int>();
      int num = 1;
      foreach (ChangeEntry change in (IEnumerable<ChangeEntry>) changeList)
      {
        ArgumentUtility.CheckForNull<ChangeEntry>(change, "ChangeEntry");
        change.ValidateAndNormalizeInputs(iterationId, uniqueChangeTrackingIds, !shouldComputeChangeTrackingId);
        if (flag && num <= maxChangeEntriesInUnsavedIteration)
          changeEntryList.Add(change);
        ++num;
      }
      return changeEntryList;
    }

    internal static void ValidateIterationPayloadReviewId(int reviewId, Iteration iteration)
    {
      if (iteration == null)
        throw new ArgumentNullException(nameof (iteration), CodeReviewResources.IterationMalformed());
      if (iteration.ReviewId != 0 && iteration.ReviewId != reviewId)
        throw new ArgumentException(CodeReviewResources.MismatchedReviewIds((object) reviewId, (object) iteration.ReviewId), nameof (reviewId));
    }

    internal static void EvaluateTop(int? top, out int topValue, int maxTop = 1000)
    {
      ref int local = ref topValue;
      int num1;
      if (top.HasValue)
      {
        int? nullable = top;
        int num2 = 1;
        if (!(nullable.GetValueOrDefault() < num2 & nullable.HasValue))
        {
          num1 = top.Value;
          goto label_4;
        }
      }
      num1 = 100;
label_4:
      local = num1;
      topValue = Math.Min(maxTop, topValue);
    }

    internal static void EvaluateTopSkip(
      int? top,
      int? skip,
      out int topValue,
      out int skipValue,
      int maxTop = 1000)
    {
      ValidationHelper.EvaluateTop(top, out topValue, maxTop);
      ref int local = ref skipValue;
      int num1;
      if (skip.HasValue)
      {
        int? nullable = skip;
        int num2 = 0;
        if (!(nullable.GetValueOrDefault() < num2 & nullable.HasValue))
        {
          num1 = skip.Value;
          goto label_4;
        }
      }
      num1 = 0;
label_4:
      local = num1;
    }

    internal static void EvaluateNextTopSkip(
      int? top,
      int? skip,
      int? totalAvailableRows,
      out int? nextTop,
      out int? nextSkip,
      int maxTop = 1000)
    {
      ValidationHelper.EvaluateNextTopSkip(top, skip, out nextTop, out nextSkip, maxTop);
      if (!totalAvailableRows.HasValue)
        return;
      int? nullable1 = nextSkip;
      int? nullable2 = totalAvailableRows;
      if (!(nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() & nullable1.HasValue & nullable2.HasValue))
        return;
      nextTop = new int?();
      nextSkip = new int?();
    }

    internal static void EvaluateNextTopSkip(
      int? top,
      int? skip,
      out int? nextTop,
      out int? nextSkip,
      int maxTop = 1000)
    {
      int topValue;
      int skipValue;
      ValidationHelper.EvaluateTopSkip(top, skip, out topValue, out skipValue, maxTop);
      int num = skipValue + topValue;
      nextTop = new int?(topValue);
      nextSkip = new int?(num);
    }

    private static bool IsNewReviewer(IList<Reviewer> existingReviewers, string reviewerIdentity)
    {
      bool flag = false;
      if (existingReviewers != null)
      {
        foreach (Reviewer existingReviewer in (IEnumerable<Reviewer>) existingReviewers)
        {
          if (existingReviewer.Identity.Id.Equals(reviewerIdentity, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            break;
          }
        }
      }
      return !flag;
    }
  }
}
