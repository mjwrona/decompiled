// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewVisitService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewVisitService : 
    CodeReviewServiceBase,
    ICodeReviewVisitService,
    IVssFrameworkService
  {
    private const int c_MaximumViewedStateLength = 500000;
    private const int c_maxArtifactsToQueryFor = 1000;
    private static readonly HashSet<string> c_supportedCodeReviewThreadTypes = new HashSet<string>((IEnumerable<string>) new string[11]
    {
      CodeReviewDiscussionConstants.MergeAttempt,
      CodeReviewDiscussionConstants.RefUpdate,
      CodeReviewDiscussionConstants.IterationPublished,
      CodeReviewDiscussionConstants.VoteUpdate,
      CodeReviewDiscussionConstants.ResetAllVotes,
      CodeReviewDiscussionConstants.StatusUpdate,
      CodeReviewDiscussionConstants.PolicyStatusUpdate,
      CodeReviewDiscussionConstants.ReviewersUpdate,
      CodeReviewDiscussionConstants.AutoCompleteUpdate,
      CodeReviewDiscussionConstants.AttachmentUpdate,
      CodeReviewDiscussionConstants.TargetChanged
    });
    private static readonly List<string> c_identityConstantsList = new List<string>()
    {
      CodeReviewDiscussionConstants.CodeReviewAutoCompleteUpdatedByTfId,
      CodeReviewDiscussionConstants.CodeReviewRefUpdatedByTfId,
      CodeReviewDiscussionConstants.CodeReviewIterationPublishedByTfId,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedByTfId,
      CodeReviewDiscussionConstants.CodeReviewReviewersUpdatedRemovedTfId,
      CodeReviewDiscussionConstants.CodeReviewStatusUpdatedByTfId,
      CodeReviewDiscussionConstants.CodeReviewVotedByTfId,
      CodeReviewDiscussionConstants.CodeReviewTargetChangedByTfId
    };

    public IEnumerable<ArtifactVisit> QueryVisits(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactVisit> visitsIn)
    {
      IEnumerable<ArtifactVisit> visitsOut = (IEnumerable<ArtifactVisit>) null;
      this.ExecuteAndTrace(requestContext, nameof (QueryVisits), 1383901, 1383902, 1383903, (Action) (() =>
      {
        this.TraceVisitInfo(requestContext, visitsIn, 1383904, "Querying last visited date");
        visitsOut = this.QueryVisitsInternal(requestContext, visitsIn, false);
      }));
      return visitsOut;
    }

    public ArtifactVisit QueryVisit(IVssRequestContext requestContext, ArtifactVisit visitIn)
    {
      ArtifactVisit visitOut = (ArtifactVisit) null;
      this.ExecuteAndTrace(requestContext, nameof (QueryVisit), 1383911, 1383912, 1383913, (Action) (() =>
      {
        IEnumerable<ArtifactVisit> artifactVisits = (IEnumerable<ArtifactVisit>) new List<ArtifactVisit>()
        {
          visitIn
        };
        this.TraceVisitInfo(requestContext, artifactVisits, 1383914, "Querying last visited dates");
        IEnumerable<ArtifactVisit> source = this.QueryVisitsInternal(requestContext, artifactVisits, true);
        if (source.Count<ArtifactVisit>() <= 0)
          return;
        visitOut = source.First<ArtifactVisit>();
      }));
      return visitOut;
    }

    private IEnumerable<ArtifactVisit> QueryVisitsInternal(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactVisit> visitsIn,
      bool includeViewedState)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) visitsIn, nameof (visitsIn));
      IdentityRef requesterIdentityRef = IdentityHelper.GetRequesterIdentityRef(requestContext);
      ArgumentUtility.CheckForNull<IdentityRef>(requesterIdentityRef, "identity");
      ArgumentUtility.CheckStringForNullOrEmpty(requesterIdentityRef.Id, "identity.Id");
      if (visitsIn.Count<ArtifactVisit>() > 1000)
        throw new ArgumentException(CodeReviewResources.TooManyVisitsToQueryFor((object) visitsIn.Count<ArtifactVisit>(), (object) 1000), nameof (visitsIn));
      IEnumerable<string> strings = visitsIn.Select<ArtifactVisit, string>((Func<ArtifactVisit, string>) (visit => visit.ArtifactId));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) strings, "artifactIds");
      if (strings.Any<string>((Func<string, bool>) (id => string.IsNullOrEmpty(id))))
        throw new ArgumentException("artifactIds");
      if (strings.Any<string>((Func<string, bool>) (id => ArgumentUtility.HasMismatchedSurrogates(id) || ArgumentUtility.HasSurrogates(id))))
        throw new ArgumentException(CodeReviewResources.InvalidArtifactIdString(), "artifactId");
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        return component.QueryVisits(requesterIdentityRef, strings, includeViewedState);
    }

    public ArtifactVisit UpdateLastVisit(IVssRequestContext requestContext, ArtifactVisit visitIn)
    {
      ArtifactVisit previousLastVisit = (ArtifactVisit) null;
      this.ExecuteAndTrace(requestContext, nameof (UpdateLastVisit), 1383921, 1383922, 1383923, (Action) (() =>
      {
        this.TraceVisitInfo(requestContext, (IEnumerable<ArtifactVisit>) new List<ArtifactVisit>()
        {
          visitIn
        }, 1383924, "Updating last visited date");
        previousLastVisit = this.UpdateLastVisitInternal(requestContext, visitIn);
      }));
      return previousLastVisit;
    }

    private ArtifactVisit UpdateLastVisitInternal(
      IVssRequestContext requestContext,
      ArtifactVisit visitIn)
    {
      this.ValidateVisitArgument(visitIn);
      IdentityRef currentIdentity = this.GetCurrentIdentity(requestContext);
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        return component.UpdateLastVisit(currentIdentity, visitIn.ArtifactId);
    }

    public ArtifactVisit UpdateViewedState(
      IVssRequestContext requestContext,
      ArtifactVisit visitIn,
      string[] hashes,
      ViewedStatus newViewedStatus)
    {
      this.ValidateVisitArgument(visitIn);
      IdentityRef identity = this.GetCurrentIdentity(requestContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) hashes, nameof (hashes));
      ArgumentUtility.CheckForOutOfRange((int) newViewedStatus, nameof (newViewedStatus), 1, 2);
      ArtifactVisit visitOut = (ArtifactVisit) null;
      string newViewedState = this.UpdateViewedStateJson(this.QueryVisit(requestContext, visitIn)?.ViewedState, hashes, newViewedStatus);
      if (newViewedState.Length > 500000)
        throw new InvalidOperationException(CodeReviewResources.ViewedStatusCountLimitReached());
      this.ExecuteAndTrace(requestContext, nameof (UpdateViewedState), 1383941, 1383942, 1383943, (Action) (() =>
      {
        this.TraceVisitInfo(requestContext, (IEnumerable<ArtifactVisit>) new List<ArtifactVisit>()
        {
          visitIn
        }, 1383944, "Updating viewed state");
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          visitOut = component.UpdateViewedState(identity, visitIn.ArtifactId, newViewedState);
      }));
      return visitOut;
    }

    private string UpdateViewedStateJson(
      string viewedStatusJson,
      string[] hashes,
      ViewedStatus newViewedStatus)
    {
      JObject dictionary = string.IsNullOrEmpty(viewedStatusJson) ? new JObject() : JObject.Parse(viewedStatusJson);
      if (!(dictionary.GetValueOrDefault<string, JToken>(nameof (hashes)) is JObject jobject))
      {
        jobject = new JObject();
        dictionary[nameof (hashes)] = (JToken) jobject;
      }
      foreach (string hash in hashes)
      {
        if (newViewedStatus == ViewedStatus.Viewed)
          jobject[hash] = (JToken) (int) newViewedStatus;
        else
          jobject.Remove(hash);
      }
      return dictionary.Serialize<JObject>();
    }

    public IEnumerable<ArtifactStats> QueryStats(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactStats> statsIn,
      bool includeUpdatesSinceLastVisit)
    {
      IEnumerable<ArtifactStats> statsOut = (IEnumerable<ArtifactStats>) null;
      this.ExecuteAndTrace(requestContext, nameof (QueryStats), 1383931, 1383932, 1383933, (Action) (() =>
      {
        this.TraceStatsInfo(requestContext, statsIn, 1383934, "Querying artifact stats");
        statsOut = this.QueryStatsInternal(requestContext, statsIn, includeUpdatesSinceLastVisit);
      }));
      return statsOut;
    }

    public ArtifactStats QueryStats(
      IVssRequestContext requestContext,
      ArtifactStats statsIn,
      bool includeUpdatesSinceLastVisit)
    {
      ArtifactStats statsOut = (ArtifactStats) null;
      this.ExecuteAndTrace(requestContext, nameof (QueryStats), 1383931, 1383932, 1383933, (Action) (() =>
      {
        IEnumerable<ArtifactStats> artifactStatses = (IEnumerable<ArtifactStats>) new List<ArtifactStats>()
        {
          statsIn
        };
        this.TraceStatsInfo(requestContext, artifactStatses, 1383934, "Querying artifact stats");
        IEnumerable<ArtifactStats> source = this.QueryStatsInternal(requestContext, artifactStatses, includeUpdatesSinceLastVisit);
        if (source.Count<ArtifactStats>() <= 0)
          return;
        statsOut = source.First<ArtifactStats>();
      }));
      return statsOut;
    }

    private IEnumerable<ArtifactStats> QueryStatsInternal(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactStats> statsIn,
      bool includeUpdatesSinceLastVisit)
    {
      IdentityRef requesterIdentityRef = IdentityHelper.GetRequesterIdentityRef(requestContext);
      ArgumentUtility.CheckForNull<IdentityRef>(requesterIdentityRef, "identity");
      ArgumentUtility.CheckStringForNullOrEmpty(requesterIdentityRef.Id, "identity.Id");
      ArgumentUtility.CheckForNull<IEnumerable<ArtifactStats>>(statsIn, nameof (statsIn));
      if (statsIn.Count<ArtifactStats>() == 0)
        throw new ArgumentException(nameof (statsIn));
      if (statsIn.Count<ArtifactStats>() > 1000)
        throw new ArgumentException(CodeReviewResources.TooManyVisitsToQueryFor((object) statsIn.Count<ArtifactStats>(), (object) 1000), nameof (statsIn));
      IEnumerable<string> source1 = statsIn.Select<ArtifactStats, string>((Func<ArtifactStats, string>) (s => s.ArtifactId));
      IEnumerable<string> source2 = statsIn.Select<ArtifactStats, string>((Func<ArtifactStats, string>) (s => string.IsNullOrEmpty(s.DiscussionArtifactId) ? s.ArtifactId : s.DiscussionArtifactId));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) source1, "artifactIds");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) source2, "artifactIdsForDiscussions");
      if (source1.Any<string>((Func<string, bool>) (id => string.IsNullOrEmpty(id))))
        throw new ArgumentException("artifactIds");
      if (source2.Any<string>((Func<string, bool>) (id => string.IsNullOrEmpty(id))))
        throw new ArgumentException("artifactIdsForDiscussions");
      IDictionary<string, ArtifactVisit> dictionary1 = (IDictionary<string, ArtifactVisit>) new Dictionary<string, ArtifactVisit>();
      IList<ArtifactStats> list = (IList<ArtifactStats>) statsIn.ToList<ArtifactStats>();
      Dictionary<string, IEnumerable<DiscussionThread>> dictionary2 = requestContext.GetService<ITeamFoundationDiscussionService>().QueryDiscussionsByArtifactUris(requestContext, source2.ToArray<string>());
      if (includeUpdatesSinceLastVisit)
      {
        ICodeReviewVisitService service = requestContext.GetService<ICodeReviewVisitService>();
        IEnumerable<ArtifactVisit> artifactVisits = source1.Distinct<string>().Select<string, ArtifactVisit>((Func<string, ArtifactVisit>) (id => new ArtifactVisit()
        {
          ArtifactId = id
        }));
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<ArtifactVisit> visits = artifactVisits;
        IEnumerable<ArtifactVisit> source3 = service.QueryVisits(requestContext1, visits);
        if (source3 != null)
          dictionary1 = (IDictionary<string, ArtifactVisit>) source3.ToDictionary<ArtifactVisit, string, ArtifactVisit>((Func<ArtifactVisit, string>) (v => v.ArtifactId), (Func<ArtifactVisit, ArtifactVisit>) (v => v));
      }
      foreach (ArtifactStats stats in (IEnumerable<ArtifactStats>) list)
      {
        string artifactId = stats.ArtifactId;
        string key = !string.IsNullOrEmpty(stats.DiscussionArtifactId) ? stats.DiscussionArtifactId : stats.ArtifactId;
        DateTime? nullable1;
        int num;
        if (includeUpdatesSinceLastVisit && dictionary1.ContainsKey(artifactId))
        {
          nullable1 = dictionary1[artifactId].LastVisitedDate;
          num = nullable1.HasValue ? 1 : 0;
        }
        else
          num = 0;
        bool shouldComputeNewCounts = num != 0;
        stats.User = requesterIdentityRef;
        ArtifactStats artifactStats = stats;
        nullable1 = new DateTime?();
        DateTime? nullable2 = nullable1;
        artifactStats.LastUpdatedDate = nullable2;
        stats.CommentsCount = (Dictionary<CommentThreadType, int?>) null;
        stats.NewCommentsCount = (Dictionary<CommentThreadType, int?>) null;
        IEnumerable<DiscussionThread> discussionThreads = dictionary2.ContainsKey(key) ? dictionary2[key] : Enumerable.Empty<DiscussionThread>();
        this.InitializeStats(stats, shouldComputeNewCounts);
        foreach (DiscussionThread thread in discussionThreads)
        {
          if (this.IsThreadSupported(thread))
          {
            CommentThreadType threadType = this.DetermineThreadType(thread);
            this.IncrementCounts(stats.CommentsCount, threadType);
            nullable1 = stats.LastUpdatedDate;
            if (nullable1.HasValue)
            {
              DateTime lastUpdatedDate = thread.LastUpdatedDate;
              nullable1 = stats.LastUpdatedDate;
              if ((nullable1.HasValue ? (lastUpdatedDate > nullable1.GetValueOrDefault() ? 1 : 0) : 0) == 0)
                goto label_22;
            }
            stats.LastUpdatedDate = new DateTime?(thread.LastUpdatedDate);
label_22:
            if (shouldComputeNewCounts && this.DetermineIsThreadNew(thread, threadType, requesterIdentityRef.Id, dictionary1[artifactId].LastVisitedDate))
              this.IncrementCounts(stats.NewCommentsCount, threadType);
          }
        }
      }
      return (IEnumerable<ArtifactStats>) list;
    }

    private IdentityRef GetCurrentIdentity(IVssRequestContext requestContext)
    {
      IdentityRef requesterIdentityRef = IdentityHelper.GetRequesterIdentityRef(requestContext);
      ArgumentUtility.CheckForNull<IdentityRef>(requesterIdentityRef, "identity");
      ArgumentUtility.CheckStringForNullOrEmpty(requesterIdentityRef.Id, "identity.Id");
      return requesterIdentityRef;
    }

    private void ValidateVisitArgument(ArtifactVisit visit)
    {
      ArgumentUtility.CheckForNull<ArtifactVisit>(visit, "visitIn");
      ArgumentUtility.CheckStringForNullOrEmpty(visit.ArtifactId, "visitIn.ArtifactId");
      if (!LinkingUtilities.IsUriWellFormed(visit.ArtifactId))
        throw new ArgumentException("visitIn.ArtifactId");
    }

    public bool VerifyArtifactId(IVssRequestContext requestContext, string artifactId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(artifactId, nameof (artifactId));
      foreach (IArtifactProvider providerExtension in (IEnumerable<IArtifactProvider>) this.ArtifactProviderExtensions)
      {
        if (providerExtension.CanResolve(requestContext, artifactId))
          return true;
      }
      return false;
    }

    private bool IsThreadSupported(DiscussionThread thread)
    {
      if (thread == null)
        return false;
      if (!string.IsNullOrEmpty(thread.ItemPath) || thread.Properties == null || !thread.Properties.ContainsKey(CodeReviewDiscussionConstants.CodeReviewThreadType))
        return true;
      if (thread.Properties == null || !thread.Properties.ContainsKey(CodeReviewDiscussionConstants.CodeReviewThreadType))
        return false;
      string str = thread.Properties[CodeReviewDiscussionConstants.CodeReviewThreadType]?.ToString();
      return CodeReviewVisitService.c_supportedCodeReviewThreadTypes.Contains(str);
    }

    private CommentThreadType DetermineThreadType(DiscussionThread thread)
    {
      CommentThreadType threadType = CommentThreadType.All;
      if (thread == null || thread.IsDeleted || thread.CommentsCount == 0)
        return threadType;
      if (!string.IsNullOrEmpty(thread.ItemPath))
        threadType = CommentThreadType.Code;
      else if (thread.Properties == null || !thread.Properties.ContainsKey(CodeReviewDiscussionConstants.CodeReviewThreadType))
        threadType = CommentThreadType.ReviewLevel;
      if (thread.Properties != null)
      {
        if (thread.Properties.ContainsKey(CodeReviewDiscussionConstants.CodeReviewThreadType) && (thread.Properties[CodeReviewDiscussionConstants.CodeReviewThreadType].ToString() == CodeReviewDiscussionConstants.RefUpdate || thread.Properties[CodeReviewDiscussionConstants.CodeReviewThreadType].ToString() == CodeReviewDiscussionConstants.IterationPublished || thread.Properties[CodeReviewDiscussionConstants.CodeReviewThreadType].ToString() == CodeReviewDiscussionConstants.TargetChanged))
          threadType = CommentThreadType.Iteration;
        if (thread.Properties.ContainsKey(CodeReviewDiscussionConstants.CodeReviewThreadType) && thread.Properties[CodeReviewDiscussionConstants.CodeReviewThreadType].ToString() == CodeReviewDiscussionConstants.VoteUpdate)
          threadType = CommentThreadType.Vote;
      }
      return threadType;
    }

    private bool DetermineIsThreadNew(
      DiscussionThread thread,
      CommentThreadType type,
      string currentUserId,
      DateTime? lastVisit)
    {
      if (!lastVisit.HasValue)
        return true;
      DateTime lastUpdatedDate1 = thread.LastUpdatedDate;
      DateTime? nullable1 = lastVisit;
      DateTime lastUpdatedDate2 = thread.LastUpdatedDate;
      if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() > lastUpdatedDate2 ? 1 : 0) : 0) != 0)
        return false;
      if (type != CommentThreadType.Code && type != CommentThreadType.ReviewLevel)
      {
        foreach (string identityConstants in CodeReviewVisitService.c_identityConstantsList)
        {
          if (thread.Properties != null && thread.Properties.ContainsKey(identityConstants))
            return thread.Properties[identityConstants].ToString() != currentUserId;
        }
      }
      else
      {
        if (thread.CommentsCount != 0 && (thread.Comments == null || ((IEnumerable<DiscussionComment>) thread.Comments).Count<DiscussionComment>() == 0))
          return true;
        foreach (DiscussionComment comment in thread.Comments)
        {
          DateTime? nullable2 = lastVisit;
          DateTime contentUpdatedDate = comment.LastContentUpdatedDate;
          if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() < contentUpdatedDate ? 1 : 0) : 0) != 0 && currentUserId != comment.Author.Id)
            return true;
        }
      }
      return false;
    }

    private void IncrementCounts(Dictionary<CommentThreadType, int?> counts, CommentThreadType type)
    {
      Dictionary<CommentThreadType, int?> dictionary1 = counts;
      int? nullable1 = dictionary1[CommentThreadType.All];
      dictionary1[CommentThreadType.All] = nullable1.HasValue ? new int?(nullable1.GetValueOrDefault() + 1) : new int?();
      if (type == CommentThreadType.Code || type == CommentThreadType.ReviewLevel)
      {
        Dictionary<CommentThreadType, int?> dictionary2 = counts;
        nullable1 = dictionary2[CommentThreadType.Text];
        dictionary2[CommentThreadType.Text] = nullable1.HasValue ? new int?(nullable1.GetValueOrDefault() + 1) : new int?();
      }
      else
      {
        Dictionary<CommentThreadType, int?> dictionary3 = counts;
        nullable1 = dictionary3[CommentThreadType.System];
        dictionary3[CommentThreadType.System] = nullable1.HasValue ? new int?(nullable1.GetValueOrDefault() + 1) : new int?();
      }
      if (type == CommentThreadType.All)
        return;
      Dictionary<CommentThreadType, int?> dictionary4 = counts;
      CommentThreadType key1 = type;
      int? nullable2 = dictionary4[key1];
      int key2 = (int) key1;
      nullable1 = nullable2;
      int? nullable3 = nullable1.HasValue ? new int?(nullable1.GetValueOrDefault() + 1) : new int?();
      dictionary4[(CommentThreadType) key2] = nullable3;
    }

    private void InitializeStats(ArtifactStats stats, bool shouldComputeNewCounts = false)
    {
      stats.CommentsCount = new Dictionary<CommentThreadType, int?>();
      stats.CommentsCount[CommentThreadType.All] = new int?(0);
      stats.CommentsCount[CommentThreadType.System] = new int?(0);
      stats.CommentsCount[CommentThreadType.Text] = new int?(0);
      stats.CommentsCount[CommentThreadType.ReviewLevel] = new int?(0);
      stats.CommentsCount[CommentThreadType.Code] = new int?(0);
      stats.CommentsCount[CommentThreadType.Iteration] = new int?(0);
      stats.CommentsCount[CommentThreadType.Vote] = new int?(0);
      if (!shouldComputeNewCounts)
        return;
      stats.NewCommentsCount = new Dictionary<CommentThreadType, int?>();
      stats.NewCommentsCount[CommentThreadType.All] = new int?(0);
      stats.NewCommentsCount[CommentThreadType.System] = new int?(0);
      stats.NewCommentsCount[CommentThreadType.Text] = new int?(0);
      stats.NewCommentsCount[CommentThreadType.ReviewLevel] = new int?(0);
      stats.NewCommentsCount[CommentThreadType.Code] = new int?(0);
      stats.NewCommentsCount[CommentThreadType.Iteration] = new int?(0);
      stats.NewCommentsCount[CommentThreadType.Vote] = new int?(0);
    }

    private void TraceVisitInfo(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactVisit> visits,
      int tracePoint,
      string description)
    {
      this.TraceArtifactsInfo(requestContext, visits != null ? visits.Select<ArtifactVisit, string>((Func<ArtifactVisit, string>) (v => v.ArtifactId)) : (IEnumerable<string>) null, tracePoint, description);
    }

    private void TraceStatsInfo(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactStats> stats,
      int tracePoint,
      string description)
    {
      this.TraceArtifactsInfo(requestContext, stats != null ? stats.Select<ArtifactStats, string>((Func<ArtifactStats, string>) (s => s.ArtifactId)) : (IEnumerable<string>) null, tracePoint, description);
    }

    private void TraceArtifactsInfo(
      IVssRequestContext requestContext,
      IEnumerable<string> artifactIds,
      int tracePoint,
      string description)
    {
      if (!requestContext.IsTracing(tracePoint, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      string id = IdentityHelper.GetRequesterIdentityRef(requestContext)?.Id;
      if (artifactIds == null)
        return;
      foreach (string artifactId in artifactIds)
        stringBuilder.Append(artifactId ?? "");
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "{0}: identity id: '{1}', artifact ids: {2}", (object) description, (object) id, (object) stringBuilder.ToString());
    }
  }
}
