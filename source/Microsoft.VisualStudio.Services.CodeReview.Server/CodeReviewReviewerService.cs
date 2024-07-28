// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewReviewerService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Server.Utils;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewReviewerService : 
    CodeReviewServiceBase,
    ICodeReviewReviewerService,
    IVssFrameworkService
  {
    public IEnumerable<Reviewer> GetReviewers(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      IEnumerable<Reviewer> reviewers = (IEnumerable<Reviewer>) null;
      this.ExecuteAndTrace(requestContext, nameof (GetReviewers), 1380411, 1380412, 1380413, (Action) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        this.TraceReviewersInfo(requestContext, projectId, reviewId, reviewers, 1380414, "Getting reviewers");
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, this.GetReviewRaw(requestContext, projectId, reviewId).SourceArtifactId);
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          reviewers = component.GetReviewers(projectId, reviewId);
        reviewers = IdentityHelper.FillReviewerIdentities(requestContext, reviewers);
      }));
      return reviewers;
    }

    public Reviewer SaveReviewer(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Reviewer reviewer)
    {
      Reviewer savedReviewer = (Reviewer) null;
      this.ExecuteAndTrace(requestContext, nameof (SaveReviewer), 1380441, 1380442, 1380443, (Action) (() =>
      {
        IEnumerable<Reviewer> reviewers = this.SaveReviewersInternal(requestContext, projectId, reviewId, (IEnumerable<Reviewer>) new Reviewer[1]
        {
          reviewer
        });
        if (reviewers == null)
          return;
        foreach (Reviewer reviewer1 in reviewers)
        {
          if (reviewer1.Identity.Id.Equals(reviewer.Identity.Id, StringComparison.OrdinalIgnoreCase))
          {
            savedReviewer = reviewer1;
            break;
          }
        }
        requestContext.Trace(1380444, TraceLevel.Verbose, this.Area, this.Layer, "Saving a reviewer: review id: '{0}', project id: '{1}', reviewer id: '{2}'", (object) reviewId, (object) projectId, (object) reviewer.Identity.Id);
      }));
      return savedReviewer;
    }

    public IEnumerable<Reviewer> SaveReviewers(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<Reviewer> reviewers)
    {
      this.ExecuteAndTrace(requestContext, nameof (SaveReviewers), 1380401, 1380402, 1380403, (Action) (() =>
      {
        this.TraceReviewersInfo(requestContext, projectId, reviewId, reviewers, 1380404, "Saving reviewers");
        reviewers = this.SaveReviewersInternal(requestContext, projectId, reviewId, reviewers);
      }));
      return reviewers;
    }

    protected virtual IEnumerable<Reviewer> SaveReviewersInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<Reviewer> reviewersToSave)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      IList<Reviewer> list = (IList<Reviewer>) this.GetReviewers(requestContext, projectId, reviewId).ToList<Reviewer>();
      ValidationHelper.ValidateReviewers(requestContext, userIdentity, reviewersToSave, list);
      IList<Tuple<Guid, Guid>> votedForList;
      IDictionary<string, Reviewer> reviewerVotedForList = this.GetReviewerVotedForList(requestContext, reviewersToSave, list, out votedForList);
      List<Reviewer> reviewerList1 = new List<Reviewer>();
      List<Reviewer> reviewerList2 = new List<Reviewer>();
      foreach (KeyValuePair<string, Reviewer> keyValuePair in (IEnumerable<KeyValuePair<string, Reviewer>>) reviewerVotedForList)
      {
        if (list.Select<Reviewer, string>((Func<Reviewer, string>) (r => r.Identity.Id)).Contains<string>(keyValuePair.Key))
          reviewerList1.Add(keyValuePair.Value);
        else
          reviewerList2.Add(keyValuePair.Value);
      }
      CodeReviewEventNotification crEvent1 = reviewerList2.Count <= 0 ? (CodeReviewEventNotification) new ReviewersStateUpdatedNotification(projectId, reviewRaw, (IEnumerable<Reviewer>) reviewerList1, reviewRaw.UpdatedDate) : (CodeReviewEventNotification) new ReviewersAddedNotification(projectId, reviewRaw, (IEnumerable<Reviewer>) reviewerList2, reviewRaw.UpdatedDate);
      EventServiceHelper.PublishDecisionPoint(requestContext, crEvent1);
      DateTime? priorReviewUpdatedTimestamp;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        reviewersToSave = component.SaveReviewers(projectId, reviewId, (IEnumerable<Reviewer>) reviewerVotedForList.Values, userIdentity.Id, (IEnumerable<Tuple<Guid, Guid>>) votedForList, out priorReviewUpdatedTimestamp);
      reviewersToSave = IdentityHelper.FillReviewerIdentities(requestContext, reviewersToSave);
      if (reviewersToSave.Any<Reviewer>())
      {
        Review review = reviewRaw.ShallowClone();
        review.UpdatedDate = reviewersToSave.Last<Reviewer>().ModifiedDate;
        CodeReviewEventNotification crEvent2 = !(crEvent1 is ReviewersAddedNotification) ? (CodeReviewEventNotification) new ReviewersStateUpdatedNotification(projectId, review, reviewersToSave, priorReviewUpdatedTimestamp) : (CodeReviewEventNotification) new ReviewersAddedNotification(projectId, review, reviewersToSave, priorReviewUpdatedTimestamp);
        EventServiceHelper.PublishNotification(requestContext, crEvent2, this.Area, this.Layer);
      }
      return reviewersToSave;
    }

    public void RemoveReviewer(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Guid identityId)
    {
      this.ExecuteAndTrace(requestContext, nameof (RemoveReviewer), 1380431, 1380432, 1380433, (Action) (() =>
      {
        requestContext.Trace(1380434, TraceLevel.Verbose, this.Area, this.Layer, "Removing a reviewer: review id: '{0}', project id: '{1}', reviewer id: '{2}'", (object) reviewId, (object) projectId, (object) identityId);
        this.RemoveReviewersInternal(requestContext, projectId, reviewId, (IEnumerable<Guid>) new Guid[1]
        {
          identityId
        });
      }));
    }

    public void RemoveReviewers(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<Guid> identityIds)
    {
      this.ExecuteAndTrace(requestContext, nameof (RemoveReviewers), 1380421, 1380422, 1380423, (Action) (() =>
      {
        this.TraceReviewersInfo(requestContext, projectId, reviewId, identityIds, 1380424, "Removing reviewers");
        this.RemoveReviewersInternal(requestContext, projectId, reviewId, identityIds);
      }));
    }

    protected virtual void RemoveReviewersInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<Guid> identityIds)
    {
      if (identityIds == null || identityIds.Count<Guid>() == 0)
        return;
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
      ReviewSecurityEvaluator.CheckWriteReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
      IEnumerable<Reviewer> reviewers = this.GetReviewers(requestContext, projectId, reviewId);
      List<Reviewer> reviewerList = new List<Reviewer>();
      List<Reviewer> reviewersToSave = new List<Reviewer>();
      if (identityIds != null)
      {
        HashSet<Guid> guidSet = new HashSet<Guid>(identityIds);
        foreach (Reviewer reviewer in reviewers)
        {
          if (guidSet.Contains(new Guid(reviewer.Identity.Id)))
          {
            IdentityRef identityRef = new IdentityRef()
            {
              Id = reviewer.Identity.Id
            };
            reviewersToSave.Add(new Reviewer()
            {
              Identity = identityRef,
              ReviewerStateId = new short?((short) 0)
            });
            reviewerList.Add(reviewer);
          }
        }
      }
      IDictionary<string, Reviewer> reviewerVotedForList = this.GetReviewerVotedForList(requestContext, (IEnumerable<Reviewer>) reviewersToSave, (IList<Reviewer>) reviewers.ToList<Reviewer>(), out IList<Tuple<Guid, Guid>> _);
      CodeReviewEventNotification crEvent1 = (CodeReviewEventNotification) new ReviewersRemovedNotification(projectId, reviewRaw, (IEnumerable<Reviewer>) reviewerList, reviewRaw.UpdatedDate, new DateTime?(DateTime.UtcNow));
      EventServiceHelper.PublishDecisionPoint(requestContext, crEvent1);
      UpdateTimestamps reviewUpdatedTimestamps;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        component.RemoveReviewers(projectId, reviewId, identityIds, (IEnumerable<Reviewer>) reviewerVotedForList.Values, out reviewUpdatedTimestamps);
      Review review = reviewRaw.ShallowClone();
      review.UpdatedDate = new DateTime?(reviewUpdatedTimestamps.Current);
      CodeReviewEventNotification crEvent2 = (CodeReviewEventNotification) new ReviewersRemovedNotification(projectId, review, (IEnumerable<Reviewer>) reviewerList, new DateTime?(reviewUpdatedTimestamps.Prior), new DateTime?(reviewUpdatedTimestamps.Current));
      EventServiceHelper.PublishNotification(requestContext, crEvent2, this.Area, this.Layer);
    }

    public IEnumerable<Reviewer> ResetAllReviewerStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      IEnumerable<Reviewer> reviewers = (IEnumerable<Reviewer>) null;
      this.ExecuteAndTrace(requestContext, nameof (ResetAllReviewerStatuses), 1380451, 1380452, 1380453, (Action) (() =>
      {
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
        IEnumerable<Reviewer> reviewers1;
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          reviewers1 = component.GetReviewers(projectId, reviewId);
        IEnumerable<Reviewer> reviewers2 = IdentityHelper.FillReviewerIdentities(requestContext, reviewers1);
        List<Reviewer> list = reviewers2.Where<Reviewer>((Func<Reviewer, bool>) (r =>
        {
          short? reviewerStateId3 = r.ReviewerStateId;
          int? nullable = reviewerStateId3.HasValue ? new int?((int) reviewerStateId3.GetValueOrDefault()) : new int?();
          int num3 = 0;
          if (nullable.GetValueOrDefault() == num3 & nullable.HasValue)
            return false;
          short? reviewerStateId4 = r.ReviewerStateId;
          nullable = reviewerStateId4.HasValue ? new int?((int) reviewerStateId4.GetValueOrDefault()) : new int?();
          int num4 = -15;
          return !(nullable.GetValueOrDefault() == num4 & nullable.HasValue);
        })).ToList<Reviewer>();
        foreach (Reviewer reviewer in list)
        {
          reviewer.ReviewerStateId = new short?((short) 0);
          reviewer.VotedFor = (IList<Guid>) new Guid[0];
          reviewer.VotedForGroups = (IList<Reviewer>) new Reviewer[0];
        }
        ReviewersStateUpdatedNotification crEvent = new ReviewersStateUpdatedNotification(projectId, reviewRaw, (IEnumerable<Reviewer>) list, reviewRaw.UpdatedDate);
        requestContext.TraceAlways(1380454, TraceLevel.Verbose, this.Area, this.Layer, "Resetting all reviewer votes for: review id: '{0}', project id: '{1}', num reviewers: '{2}', num votes reset: '{3}'", (object) reviewId, (object) projectId, (object) reviewers2.Count<Reviewer>(), (object) list.Count<Reviewer>());
        EventServiceHelper.PublishDecisionPoint(requestContext, (CodeReviewEventNotification) crEvent);
        DateTime? priorReviewUpdatedTimestamp;
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          reviewers = component.ResetAllReviewerStatuses(projectId, reviewId, out priorReviewUpdatedTimestamp);
        reviewers = IdentityHelper.FillReviewerIdentities(requestContext, reviewers2);
        if (!reviewers.Any<Reviewer>())
          return;
        Review review = reviewRaw.ShallowClone();
        review.UpdatedDate = reviewers.Last<Reviewer>().ModifiedDate;
        EventServiceHelper.PublishNotification(requestContext, (CodeReviewEventNotification) new ReviewersStateUpdatedNotification(projectId, review, reviewers, priorReviewUpdatedTimestamp)
        {
          HasAllReviewerStatusesReset = true
        }, this.Area, this.Layer);
      }));
      return reviewers;
    }

    public IEnumerable<Reviewer> ResetSomeReviewerStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<Guid> reviewerIdsToReset)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      IEnumerable<Reviewer> reviewers = (IEnumerable<Reviewer>) null;
      this.ExecuteAndTrace(requestContext, nameof (ResetSomeReviewerStatuses), 1380461, 1380462, 1380463, (Action) (() =>
      {
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, this.GetReviewRaw(requestContext, projectId, reviewId).SourceArtifactId);
        IEnumerable<Reviewer> reviewers1;
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          reviewers1 = component.GetReviewers(projectId, reviewId);
        List<Reviewer> list = IdentityHelper.FillReviewerIdentities(requestContext, reviewers1).Where<Reviewer>((Func<Reviewer, bool>) (r =>
        {
          short? reviewerStateId = r.ReviewerStateId;
          int? nullable = reviewerStateId.HasValue ? new int?((int) reviewerStateId.GetValueOrDefault()) : new int?();
          int num = 0;
          return !(nullable.GetValueOrDefault() == num & nullable.HasValue);
        })).Where<Reviewer>((Func<Reviewer, bool>) (r => reviewerIdsToReset.Contains<Guid>(new Guid(r.Identity.Id)))).ToList<Reviewer>();
        foreach (Reviewer reviewer in list)
        {
          reviewer.ReviewerStateId = new short?((short) 0);
          reviewer.VotedFor = (IList<Guid>) new Guid[0];
          reviewer.VotedForGroups = (IList<Reviewer>) new Reviewer[0];
        }
        reviewers = this.SaveReviewersInternal(requestContext, projectId, reviewId, (IEnumerable<Reviewer>) list);
      }));
      return reviewers;
    }

    private IDictionary<string, Reviewer> GetReviewerVotedForList(
      IVssRequestContext requestContext,
      IEnumerable<Reviewer> reviewersToSave,
      IList<Reviewer> existingReviewers,
      out IList<Tuple<Guid, Guid>> votedForList)
    {
      votedForList = (IList<Tuple<Guid, Guid>>) new List<Tuple<Guid, Guid>>();
      Dictionary<Guid, Reviewer> dictionary = existingReviewers.ToDictionary<Reviewer, Guid>((Func<Reviewer, Guid>) (r => new Guid(r.Identity.Id)));
      foreach (Reviewer reviewer in reviewersToSave)
      {
        Guid key = new Guid(reviewer.Identity.Id);
        if (dictionary.ContainsKey(key))
        {
          if (reviewer.ReviewerStateId.HasValue)
            dictionary[key].ReviewerStateId = reviewer.ReviewerStateId;
          if (reviewer.IterationId.HasValue)
            dictionary[key].IterationId = reviewer.IterationId;
        }
        else
          dictionary.Add(key, reviewer);
      }
      Dictionary<Guid, List<Reviewer>> groupsWithMembers = this.GetAllGroupsWithMembers(requestContext, dictionary);
      if (groupsWithMembers.Count == 0)
        return (IDictionary<string, Reviewer>) reviewersToSave.ToDictionary<Reviewer, string, Reviewer>((Func<Reviewer, string>) (x => x.Identity.Id), (Func<Reviewer, Reviewer>) (x => x));
      IDictionary<string, Reviewer> modifiedReviewers = (IDictionary<string, Reviewer>) new Dictionary<string, Reviewer>();
      votedForList = this.GenerateGroupReviewerVotes(groupsWithMembers, dictionary, ref modifiedReviewers);
      foreach (Reviewer reviewer in reviewersToSave.Where<Reviewer>((Func<Reviewer, bool>) (newReviewer => !modifiedReviewers.ContainsKey(newReviewer.Identity.Id))))
        modifiedReviewers.Add(reviewer.Identity.Id, reviewer);
      return modifiedReviewers;
    }

    private IList<Tuple<Guid, Guid>> GenerateGroupReviewerVotes(
      Dictionary<Guid, List<Reviewer>> allGroups,
      Dictionary<Guid, Reviewer> allReviewers,
      ref IDictionary<string, Reviewer> reviewersToSave)
    {
      IList<Tuple<Guid, Guid>> votedList = (IList<Tuple<Guid, Guid>>) new List<Tuple<Guid, Guid>>();
      foreach (KeyValuePair<Guid, List<Reviewer>> allGroup in allGroups)
      {
        int voteIteration;
        short groupVote = this.CalculateGroupVote(allGroup.Key, (IEnumerable<Reviewer>) allGroup.Value, ref votedList, out voteIteration);
        Reviewer allReviewer = allReviewers[allGroup.Key];
        int num1 = (int) groupVote;
        short? reviewerStateId = allReviewer.ReviewerStateId;
        int? nullable = reviewerStateId.HasValue ? new int?((int) reviewerStateId.GetValueOrDefault()) : new int?();
        int valueOrDefault1 = nullable.GetValueOrDefault();
        if (num1 == valueOrDefault1 & nullable.HasValue)
        {
          int num2 = voteIteration;
          nullable = allReviewer.IterationId;
          int valueOrDefault2 = nullable.GetValueOrDefault();
          if (num2 == valueOrDefault2)
            continue;
        }
        allReviewer.ReviewerStateId = new short?(groupVote);
        allReviewer.IterationId = new int?(voteIteration);
        reviewersToSave[allReviewer.Identity.Id] = allReviewer;
      }
      return votedList;
    }

    private short CalculateGroupVote(
      Guid groupId,
      IEnumerable<Reviewer> reviewers,
      ref IList<Tuple<Guid, Guid>> votedList,
      out int voteIteration)
    {
      voteIteration = 0;
      short groupVote = 0;
      foreach (Reviewer reviewer in reviewers)
      {
        short num1 = reviewer.ReviewerStateId.Value;
        if (!ReviewerExtensions.IsPendingResponseState(num1))
        {
          groupVote = ReviewerExtensions.IsInitialState(groupVote) ? num1 : Math.Min(groupVote, num1);
          int? iterationId = reviewer.IterationId;
          if (iterationId.HasValue)
          {
            ref int local = ref voteIteration;
            int val1 = voteIteration;
            iterationId = reviewer.IterationId;
            int val2 = iterationId.Value;
            int num2 = Math.Max(val1, val2);
            local = num2;
          }
          votedList.Add(new Tuple<Guid, Guid>(new Guid(reviewer.Identity.Id), groupId));
        }
      }
      return groupVote;
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetAllReviewerIdentities(
      IVssRequestContext requestContext,
      IdentityService identityService,
      IList<Guid> allReviewersIds)
    {
      return IdentityHelper.GetMappedIdentities(requestContext, allReviewersIds).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null));
    }

    private Dictionary<Guid, List<Reviewer>> GetAllGroupsWithMembers(
      IVssRequestContext requestContext,
      Dictionary<Guid, Reviewer> allReviewers)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> reviewerIdentities = this.GetAllReviewerIdentities(requestContext, service, (IList<Guid>) allReviewers.Keys.ToList<Guid>());
      Dictionary<Guid, List<Reviewer>> groupsWithMembers = new Dictionary<Guid, List<Reviewer>>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in reviewerIdentities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (reviewerIdentity => reviewerIdentity.IsContainer)))
      {
        groupsWithMembers.Add(identity1.Id, new List<Reviewer>());
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity2 in reviewerIdentities)
        {
          if (!identity2.IsContainer && identity2.Id != identity1.Id && allReviewers.ContainsKey(identity2.Id) && service.IsMember(requestContext, identity1.Descriptor, identity2.Descriptor))
            groupsWithMembers[identity1.Id].Add(allReviewers[identity2.Id]);
        }
      }
      return groupsWithMembers;
    }

    private void TraceReviewersInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<Reviewer> reviewers,
      int tracePoint,
      string description)
    {
      if (!requestContext.IsTracing(tracePoint, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (reviewers == null)
        return;
      foreach (Reviewer reviewer in reviewers)
        stringBuilder.Append(string.Format("'{0}', ", (object) reviewer.Identity.Id));
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "{0}: review id: '{1}', project id: '{2}', reviewer ids: {3}", (object) description, (object) reviewId, (object) projectId, (object) stringBuilder.ToString());
    }

    private void TraceReviewersInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<Guid> reviewers,
      int tracePoint,
      string description)
    {
      if (!requestContext.IsTracing(tracePoint, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (reviewers == null)
        return;
      foreach (Guid reviewer in reviewers)
        stringBuilder.Append(string.Format("'{0}', ", (object) reviewer));
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "{0}: review id: '{1}', project id: '{2}', reviewer ids: {3}", (object) description, (object) reviewId, (object) projectId, (object) stringBuilder.ToString());
    }
  }
}
