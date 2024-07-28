// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.TeamFoundationMentionService
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Mention.Server.Data;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Mention.Server
{
  public class TeamFoundationMentionService : ITeamFoundationMentionService, IVssFrameworkService
  {
    private static readonly RegistryQuery s_SettingsQuery = (RegistryQuery) "/Service/Mentions/Settings/*";
    private readonly ITimeProvider timeProvider;
    protected TeamFoundationMentionService.ServiceSettings m_settings;
    private IDisposableReadOnlyList<IMentionParser> m_parsers;
    private IDisposableReadOnlyList<IMentionAction> m_actions;

    public TeamFoundationMentionService()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public TeamFoundationMentionService(ITimeProvider timeProvider) => this.timeProvider = timeProvider;

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_parsers = systemRequestContext.GetExtensions<IMentionParser>();
      this.m_actions = systemRequestContext.GetExtensions<IMentionAction>();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), true, in TeamFoundationMentionService.s_SettingsQuery);
      Interlocked.CompareExchange<TeamFoundationMentionService.ServiceSettings>(ref this.m_settings, new TeamFoundationMentionService.ServiceSettings(systemRequestContext), (TeamFoundationMentionService.ServiceSettings) null);
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_parsers != null)
      {
        this.m_parsers.Dispose();
        this.m_parsers = (IDisposableReadOnlyList<IMentionParser>) null;
      }
      if (this.m_actions != null)
      {
        this.m_actions.Dispose();
        this.m_actions = (IDisposableReadOnlyList<IMentionAction>) null;
      }
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
    }

    public IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> ProcessMentionCandidates(
      IVssRequestContext requestContext,
      IEnumerable<MentionCandidate> candidates)
    {
      return this.ProcessMentionCandidates(requestContext, candidates, out int _, out int _, out string _);
    }

    public IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> ProcessMentionCandidates(
      IVssRequestContext requestContext,
      IEnumerable<MentionCandidate> candidates,
      out int totalNumberOfMentionsFound,
      out int mentionProcessingErrors,
      out string firstErrorMessage)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<MentionCandidate>>(candidates, nameof (candidates));
      totalNumberOfMentionsFound = 0;
      mentionProcessingErrors = 0;
      firstErrorMessage = string.Empty;
      try
      {
        requestContext.TraceEnter(101001, "MentionService", "Service", nameof (ProcessMentionCandidates));
        if (candidates.Any<MentionCandidate>((Func<MentionCandidate, bool>) (c => c.CommentState == CommentState.Deleted)))
        {
          HashSet<int> commentIds = new HashSet<int>(candidates.Where<MentionCandidate>((Func<MentionCandidate, bool>) (c => c.CommentState == CommentState.Deleted)).Select<MentionCandidate, int>((Func<MentionCandidate, int>) (c => c.CommentId.Value)));
          requestContext.TraceSerializedConditionally(101003, TraceLevel.Verbose, "MentionService", "Service", "Deleting all existing mentions from comments with ids: {0} as the comments were deleted", (object) commentIds);
          this.DeleteMentionsByCommentIds(requestContext, (ISet<int>) commentIds);
        }
        if (!candidates.Any<MentionCandidate>((Func<MentionCandidate, bool>) (c => c.CommentState != CommentState.Deleted)))
          return Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>();
        List<Microsoft.TeamFoundation.Mention.Server.Mention> mentionList = new List<Microsoft.TeamFoundation.Mention.Server.Mention>();
        IEnumerable<MentionCandidate> unprocessedCandidates = candidates.Where<MentionCandidate>((Func<MentionCandidate, bool>) (c => c.CommentState != CommentState.Deleted));
        mentionList.AddRangeIfRangeNotNull<Microsoft.TeamFoundation.Mention.Server.Mention, List<Microsoft.TeamFoundation.Mention.Server.Mention>>(this.m_parsers.SelectMany<IMentionParser, Microsoft.TeamFoundation.Mention.Server.Mention, Microsoft.TeamFoundation.Mention.Server.Mention>((Func<IMentionParser, IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>>) (parser => this.FindMentionsOrReturnEmptyListOnError(requestContext, parser, unprocessedCandidates.Where<MentionCandidate>((Func<MentionCandidate, bool>) (candidate => parser.SupportedContentTypes.HasFlag((Enum) candidate.ContentType) && !string.IsNullOrEmpty(candidate.Text))).ToList<MentionCandidate>())), (Func<IMentionParser, Microsoft.TeamFoundation.Mention.Server.Mention, Microsoft.TeamFoundation.Mention.Server.Mention>) ((parser, mention) => mention))).ToList<Microsoft.TeamFoundation.Mention.Server.Mention>();
        totalNumberOfMentionsFound += mentionList.Count;
        this.PublishMentionsForTelemetry(requestContext, (IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>) mentionList);
        requestContext.Trace(101003, TraceLevel.Verbose, "MentionService", "Service", "Found {0} mentions from {1} candidates.", (object) totalNumberOfMentionsFound, (object) unprocessedCandidates.Count<MentionCandidate>());
        List<Microsoft.TeamFoundation.Mention.Server.Mention> resolvedMentions = mentionList.Where<Microsoft.TeamFoundation.Mention.Server.Mention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, bool>) (mention => mention.IsResolved)).ToList<Microsoft.TeamFoundation.Mention.Server.Mention>();
        resolvedMentions = resolvedMentions.Distinct<Microsoft.TeamFoundation.Mention.Server.Mention>((IEqualityComparer<Microsoft.TeamFoundation.Mention.Server.Mention>) new TeamFoundationMentionService.DistinctMentionComparer()).ToList<Microsoft.TeamFoundation.Mention.Server.Mention>();
        List<Microsoft.TeamFoundation.Mention.Server.Mention> list1 = Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>().ToList<Microsoft.TeamFoundation.Mention.Server.Mention>();
        bool flag1 = unprocessedCandidates.Any<MentionCandidate>((Func<MentionCandidate, bool>) (cand => cand.CommentId.HasValue && cand.CommentState == CommentState.Updated));
        if (flag1)
        {
          list1 = this.GetMentionsByCommentIds(requestContext, (ISet<int>) new HashSet<int>(unprocessedCandidates.Select<MentionCandidate, int>((Func<MentionCandidate, int>) (r => r.CommentId.Value)))).ToList<Microsoft.TeamFoundation.Mention.Server.Mention>();
          flag1 = flag1 && list1.Any<Microsoft.TeamFoundation.Mention.Server.Mention>();
        }
        ISet<string> sourceIds = (ISet<string>) new HashSet<string>();
        foreach (MentionCandidate mentionCandidate in unprocessedCandidates)
        {
          int? commentId = mentionCandidate.CommentId;
          if (commentId.HasValue)
          {
            commentId = mentionCandidate.CommentId;
            int num = 0;
            if (!(commentId.GetValueOrDefault() == num & commentId.HasValue))
              continue;
          }
          sourceIds.Add(mentionCandidate.SourceContext.Identifier);
        }
        List<Microsoft.TeamFoundation.Mention.Server.Mention> list2 = this.GetMentionsBySourceIds(requestContext, sourceIds).ToList<Microsoft.TeamFoundation.Mention.Server.Mention>();
        bool flag2 = list2.Any<Microsoft.TeamFoundation.Mention.Server.Mention>();
        if (resolvedMentions.Any<Microsoft.TeamFoundation.Mention.Server.Mention>())
        {
          if (!list1.Any<Microsoft.TeamFoundation.Mention.Server.Mention>())
            list1 = this.GetMentions(requestContext, resolvedMentions.Select<Microsoft.TeamFoundation.Mention.Server.Mention, MentionSource>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, MentionSource>) (x => new MentionSource()
            {
              ProjectGuid = x.Source.ProjectGuid,
              SourceId = x.Source.Identifier,
              SourceType = x.Source.Type
            }))).ToList<Microsoft.TeamFoundation.Mention.Server.Mention>();
          foreach (Microsoft.TeamFoundation.Mention.Server.Mention mention1 in resolvedMentions)
          {
            Microsoft.TeamFoundation.Mention.Server.Mention mention = mention1;
            mention.IsNew = !list1.Any<Microsoft.TeamFoundation.Mention.Server.Mention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, bool>) (m =>
            {
              if (!m.IsResolved || !StringComparer.OrdinalIgnoreCase.Equals(m.TargetId, mention.TargetId))
                return false;
              if (StringComparer.OrdinalIgnoreCase.Equals(m.Source.Identifier, mention.Source.Identifier))
                return true;
              int? commentId = m.CommentId;
              if (!commentId.HasValue)
                return false;
              commentId = m.CommentId;
              int num1 = commentId.Value;
              commentId = mention.CommentId;
              int num2 = commentId.Value;
              return num1 == num2;
            }));
            if (!mention.IsNew && mention.Source.SupportedActionTypes.HasFlag((Enum) MentionActionType.ResolveWorkItem))
              mention.IsNew = true;
          }
          int num = resolvedMentions.Count<Microsoft.TeamFoundation.Mention.Server.Mention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, bool>) (m => m.IsNew));
          requestContext.Trace(101003, TraceLevel.Verbose, "MentionService", "Service", "{0} new mentions detected from total {1} found mentions.", (object) num, (object) mentionList.Count);
          if (num > 0)
          {
            TeamFoundationMentionService.ServiceSettings settings = this.m_settings;
            if (num > settings.MaximumNumberOfMentions)
            {
              requestContext.Trace(101003, TraceLevel.Verbose, "MentionService", "Service", "{0} new mentions detected, greater than configured limit of {1}. Only processing {2} mentions", (object) num, (object) settings.MaximumNumberOfMentions, (object) (num - settings.MaximumNumberOfMentions));
              IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> second = resolvedMentions.Where<Microsoft.TeamFoundation.Mention.Server.Mention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, bool>) (m => !m.IsNew)).Take<Microsoft.TeamFoundation.Mention.Server.Mention>(settings.MaximumNumberOfMentions);
              resolvedMentions = resolvedMentions.Where<Microsoft.TeamFoundation.Mention.Server.Mention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, bool>) (m => m.IsNew)).Take<Microsoft.TeamFoundation.Mention.Server.Mention>(settings.MaximumNumberOfMentions).Union<Microsoft.TeamFoundation.Mention.Server.Mention>(second).ToList<Microsoft.TeamFoundation.Mention.Server.Mention>();
            }
            mentionProcessingErrors += TeamFoundationMentionService.PerformActions(requestContext, (IReadOnlyList<IMentionAction>) this.m_actions, resolvedMentions, out firstErrorMessage);
          }
          List<MentionSourceExtended> mentionSourceExtendedList = new List<MentionSourceExtended>();
          if (flag1)
            mentionSourceExtendedList.AddRangeIfRangeNotNull<MentionSourceExtended, List<MentionSourceExtended>>(list1.Where<Microsoft.TeamFoundation.Mention.Server.Mention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, bool>) (em => !resolvedMentions.Any<Microsoft.TeamFoundation.Mention.Server.Mention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, bool>) (rm => rm.TargetId.Equals(em.TargetId, StringComparison.OrdinalIgnoreCase))))).Select<Microsoft.TeamFoundation.Mention.Server.Mention, MentionSourceExtended>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, MentionSourceExtended>) (x => new MentionSourceExtended()
            {
              ProjectGuid = x.Source.ProjectGuid,
              SourceId = x.Source.Identifier,
              SourceType = x.Source.Type,
              TargetId = x.TargetId
            })));
          if (flag2)
          {
            foreach (Microsoft.TeamFoundation.Mention.Server.Mention mention2 in list2)
            {
              Microsoft.TeamFoundation.Mention.Server.Mention mention = mention2;
              if (!resolvedMentions.Where<Microsoft.TeamFoundation.Mention.Server.Mention>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, bool>) (rm => rm.Source.Identifier == mention.Source.Identifier && rm.ArtifactType == mention.ArtifactType && rm.TargetId == mention.TargetId)).Any<Microsoft.TeamFoundation.Mention.Server.Mention>())
                mentionSourceExtendedList.Add(new MentionSourceExtended()
                {
                  ProjectGuid = mention.Source.ProjectGuid,
                  SourceId = mention.Source.Identifier,
                  SourceType = mention.Source.Type,
                  TargetId = mention.TargetId
                });
            }
          }
          if (mentionSourceExtendedList.Any<MentionSourceExtended>())
          {
            requestContext.Trace(101003, TraceLevel.Verbose, "MentionService", "Service", "Found {0} mentions to delete in existing field with {1} new mentions and {2} existing mentions", (object) mentionSourceExtendedList.Count, (object) num, (object) list1.Count);
            this.DeleteMentions(requestContext, (IEnumerable<MentionSourceExtended>) mentionSourceExtendedList);
          }
        }
        else if (flag1)
        {
          HashSet<int> commentIds = new HashSet<int>(unprocessedCandidates.Select<MentionCandidate, int>((Func<MentionCandidate, int>) (c => c.CommentId.Value)));
          requestContext.TraceSerializedConditionally(101003, TraceLevel.Verbose, "MentionService", "Service", "Deleting all existing mentions from comments with ids: {0} as there are no mentions in the updated comments", (object) commentIds);
          this.DeleteMentionsByCommentIds(requestContext, (ISet<int>) commentIds);
        }
        ISet<string> stringSet = (ISet<string>) new HashSet<string>();
        foreach (MentionCandidate mentionCandidate in unprocessedCandidates)
        {
          int? commentId = mentionCandidate.CommentId;
          if (commentId.HasValue)
          {
            commentId = mentionCandidate.CommentId;
            int num = 0;
            if (!(commentId.GetValueOrDefault() == num & commentId.HasValue))
              continue;
          }
          string identifier = mentionCandidate.SourceContext.Identifier;
          stringSet.Add(identifier);
        }
        foreach (Microsoft.TeamFoundation.Mention.Server.Mention mention in mentionList)
          stringSet.Remove(mention.Source.Identifier);
        if (stringSet.Any<string>())
          this.DeleteMentionsBySourceIds(requestContext, stringSet);
        return (IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>) resolvedMentions;
      }
      finally
      {
        requestContext.TraceLeave(101009, "MentionService", "Service", nameof (ProcessMentionCandidates));
        VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionProcessingFailuresPerSecond");
        performanceCounter.IncrementBy((long) mentionProcessingErrors);
        stopwatch.Stop();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionAverageMentionCandidatesProcessingTimeBase");
        performanceCounter.IncrementBy((long) totalNumberOfMentionsFound);
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Mention.Server.Perf_Service_MentionAverageMentionCandidatesProcessingTime");
        performanceCounter.IncrementTicks(stopwatch);
      }
    }

    public void DeleteMentions(
      IVssRequestContext requestContext,
      IEnumerable<MentionSourceExtended> sources)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<MentionSourceExtended>>(sources, nameof (sources));
      requestContext.TraceEnter(101223, "MentionService", "Service", nameof (DeleteMentions));
      try
      {
        using (TeamFoundationMentionComponent component = requestContext.CreateComponent<TeamFoundationMentionComponent>())
          component.DeleteMentions(sources);
      }
      finally
      {
        requestContext.TraceLeave(101224, "MentionService", "Service", nameof (DeleteMentions));
      }
    }

    public void DeleteMentionsByCommentIds(IVssRequestContext requestContext, ISet<int> commentIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ISet<int>>(commentIds, nameof (commentIds));
      requestContext.TraceEnter(101223, "MentionService", "Service", nameof (DeleteMentionsByCommentIds));
      try
      {
        using (TeamFoundationMentionComponent component = requestContext.CreateComponent<TeamFoundationMentionComponent>())
          component.DeleteMentionsByCommentId(commentIds);
      }
      finally
      {
        requestContext.TraceLeave(101224, "MentionService", "Service", nameof (DeleteMentionsByCommentIds));
      }
    }

    public void DeleteMentionsBySourceIds(IVssRequestContext requestContext, ISet<string> sourceIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) sourceIds, "soruceIds");
      using (requestContext.TraceBlock(101223, 101221, "MentionService", "Service", nameof (DeleteMentionsBySourceIds)))
      {
        using (TeamFoundationMentionComponent component = requestContext.CreateComponent<TeamFoundationMentionComponent>())
          component.DeleteMentionsBySourceIds(sourceIds);
      }
    }

    public IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentions(
      IVssRequestContext requestContext,
      IEnumerable<MentionSource> sources)
    {
      using (TeamFoundationMentionComponent component = requestContext.CreateComponent<TeamFoundationMentionComponent>())
        return component.GetMentions(sources);
    }

    public void SaveMentions(IVssRequestContext requestContext, IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentions)
    {
      requestContext.TraceEnter(101201, "MentionService", "Service", nameof (SaveMentions));
      try
      {
        (this.m_actions.FirstOrDefault<IMentionAction>((Func<IMentionAction, bool>) (a => a.Name == "SaveMentionAction")) ?? throw new InvalidOperationException("SaveMentionAction is not registered as an IMentionAction plugin.")).PerformAction(requestContext, mentions);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(101210, "MentionService", "Service", ex);
      }
      finally
      {
        requestContext.TraceLeave(101209, "MentionService", "Service", nameof (SaveMentions));
      }
    }

    public IEnumerable<MentionRecord> UpdateMentions(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentions)
    {
      using (TeamFoundationMentionComponent component = requestContext.CreateComponent<TeamFoundationMentionComponent>())
        return component.UpdateMentions(mentions);
    }

    public virtual IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentionsByCommentIds(
      IVssRequestContext requestContext,
      ISet<int> commentIds)
    {
      using (TeamFoundationMentionComponent component = requestContext.CreateComponent<TeamFoundationMentionComponent>())
        return component.GetMentionsByCommentId(commentIds);
    }

    public virtual IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> GetMentionsBySourceIds(
      IVssRequestContext requestContext,
      ISet<string> sourceIds)
    {
      using (TeamFoundationMentionComponent component = requestContext.CreateComponent<TeamFoundationMentionComponent>())
        return component.GetMentionsBySourceIds(sourceIds);
    }

    public IEnumerable<LastMentionedInfo> GetRecentMentionsForTargets(
      IVssRequestContext requestContext,
      Guid? sourceProjectId,
      string sourceType,
      int limit,
      IEnumerable<MentionTarget> targets)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(sourceType, nameof (sourceType));
      ArgumentUtility.CheckForNull<IEnumerable<MentionTarget>>(targets, nameof (targets));
      ArgumentUtility.CheckForNonnegativeInt(limit, nameof (limit));
      using (TeamFoundationMentionComponent component = requestContext.CreateComponent<TeamFoundationMentionComponent>())
      {
        DateTime dateLimit = this.timeProvider.Now.Subtract(TimeSpan.FromSeconds((double) this.m_settings.MentionDateTimeLimitInSeconds));
        return component.GetRecentMentionsForTargets(sourceProjectId, sourceType, limit, dateLimit, targets).GroupBy<LastMentionedInfo, string>((Func<LastMentionedInfo, string>) (m => m.NormalizedSourceId)).Select<IGrouping<string, LastMentionedInfo>, LastMentionedInfo>((Func<IGrouping<string, LastMentionedInfo>, LastMentionedInfo>) (g => g.First<LastMentionedInfo>()));
      }
    }

    public IEnumerable<LastMentionedInfo> GetRecentMentionsForCurrentUser(
      IVssRequestContext requestContext,
      Guid? sourceProjectId,
      string sourceType,
      int limit)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(sourceType, nameof (sourceType));
      ArgumentUtility.CheckForNonnegativeInt(limit, nameof (limit));
      MentionTarget[] targetForCurrentUser = TeamFoundationMentionService.GetMentionTargetForCurrentUser(requestContext);
      return this.GetRecentMentionsForTargets(requestContext, sourceProjectId, sourceType, limit, (IEnumerable<MentionTarget>) targetForCurrentUser);
    }

    private static MentionTarget[] GetMentionTargetForCurrentUser(IVssRequestContext requestContext)
    {
      List<MentionTarget> mentionTargetList1 = new List<MentionTarget>();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      List<MentionTarget> mentionTargetList2 = mentionTargetList1;
      MentionTarget mentionTarget1 = new MentionTarget();
      mentionTarget1.ArtifactType = "Person";
      mentionTarget1.TargetId = requestContext.ExecutionEnvironment.IsHostedDeployment ? userIdentity.GetProperty<string>("Account", (string) null) : requestContext.AuthenticatedUserName;
      MentionTarget mentionTarget2 = mentionTarget1;
      mentionTargetList2.Add(mentionTarget2);
      List<MentionTarget> mentionTargetList3 = mentionTargetList1;
      mentionTarget1 = new MentionTarget();
      mentionTarget1.ArtifactType = "Person";
      mentionTarget1.TargetId = userIdentity.Id.ToString("D");
      MentionTarget mentionTarget3 = mentionTarget1;
      mentionTargetList3.Add(mentionTarget3);
      if (requestContext.IsFeatureEnabled(MentionFeatureFlags.ShowGroupMentions) && requestContext.IsFeatureEnabled(MentionFeatureFlags.AllowGroupMentions) && requestContext.IsFeatureEnabled(MentionFeatureFlags.UseNotificationPipeline))
      {
        using (requestContext.TraceBlock(101013, 101014, "MentionService", "Service", "TeamFoundationMentionService.GetMentionTargetForCurrentUser.ExpandGroups"))
        {
          try
          {
            IdentityService service = requestContext.GetService<IdentityService>();
            IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
            {
              userIdentity.Id
            }, QueryMembership.ExpandedUp, (IEnumerable<string>) null);
            if (identityList != null)
            {
              if (identityList.Count == 1)
              {
                if (identityList[0] != null)
                {
                  if (identityList[0].MemberOf != null)
                  {
                    if (identityList[0].MemberOf.Count > 0)
                    {
                      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new List<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) identityList[0].MemberOf), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null));
                      mentionTargetList1.AddRange(source.Select<Microsoft.VisualStudio.Services.Identity.Identity, MentionTarget>((Func<Microsoft.VisualStudio.Services.Identity.Identity, MentionTarget>) (identity => new MentionTarget()
                      {
                        ArtifactType = "Person",
                        TargetId = identity.Id.ToString("D")
                      })));
                    }
                  }
                }
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(101015, "MentionService", "Service", ex);
          }
        }
      }
      return mentionTargetList1.ToArray();
    }

    private static int PerformActions(
      IVssRequestContext requestContext,
      IReadOnlyList<IMentionAction> actions,
      List<Microsoft.TeamFoundation.Mention.Server.Mention> mentions,
      out string firstErrorMessage)
    {
      string errorMessage = string.Empty;
      int mentionProcessingErrors = 0;
      requestContext.TraceBlock<int>(101011, 101012, "MentionService", "Service", "TeamFoundationMentionService.PerformActions", (Func<int>) (() =>
      {
        foreach (IMentionAction action in (IEnumerable<IMentionAction>) actions)
        {
          try
          {
            requestContext.Trace(101002, TraceLevel.Verbose, "MentionService", "Service", "Performing action {0} on mentions", (object) action.Name);
            action.PerformAction(requestContext, action.FilterMentions(requestContext, (IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>) mentions));
          }
          catch (Exception ex)
          {
            ++mentionProcessingErrors;
            if (string.IsNullOrEmpty(errorMessage))
              errorMessage = ex.Message;
            requestContext.Trace(101804, TraceLevel.Warning, "MentionService", "Service", "Exception when processing mentions: {0}, Stack Trace: {1}.", (object) ex.Message, (object) ex.StackTrace);
          }
        }
        return mentionProcessingErrors;
      }));
      firstErrorMessage = errorMessage;
      return mentionProcessingErrors;
    }

    private IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> FindMentionsOrReturnEmptyListOnError(
      IVssRequestContext requestContext,
      IMentionParser parser,
      List<MentionCandidate> candidates)
    {
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> emptyListOnError = Enumerable.Empty<Microsoft.TeamFoundation.Mention.Server.Mention>();
      if (candidates.Count == 0)
        return emptyListOnError;
      try
      {
        emptyListOnError = parser.FindMentions(requestContext, (IEnumerable<MentionCandidate>) candidates);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(101902, "MentionService", "Service", ex);
      }
      return emptyListOnError;
    }

    protected virtual bool PublishMentionsForTelemetry(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> mentions)
    {
      if (mentions == null)
        return false;
      List<CustomerIntelligenceData> list = mentions.Select<Microsoft.TeamFoundation.Mention.Server.Mention, CustomerIntelligenceData>((Func<Microsoft.TeamFoundation.Mention.Server.Mention, CustomerIntelligenceData>) (mention =>
      {
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("ArtifactType", mention.ArtifactType);
        intelligenceData.Add("IsResolved", mention.IsResolved);
        intelligenceData.Add("Mentioner", (object) mention.Source.Mentioner);
        intelligenceData.Add("SourceType", mention.Source.Type);
        intelligenceData.Add("SourceId", mention.Source.Identifier);
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          intelligenceData.Add("CollectionName", requestContext.ServiceHost.Name);
        return intelligenceData;
      })).ToList<CustomerIntelligenceData>();
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.PublishMentionsTask), (object) list, 0));
      return true;
    }

    private void PublishMentionsTask(IVssRequestContext requestContext, object taskArgs)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      if (!(taskArgs is IEnumerable<CustomerIntelligenceData> intelligenceDatas))
        return;
      foreach (CustomerIntelligenceData properties in intelligenceDatas)
        service.Publish(requestContext, "MentionService", "Service", properties);
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<TeamFoundationMentionService.ServiceSettings>(ref this.m_settings, new TeamFoundationMentionService.ServiceSettings(requestContext));
    }

    public int CleanupMentions(IVssRequestContext requestContext, DateTime currentTime)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (TeamFoundationMentionComponent component = requestContext.CreateComponent<TeamFoundationMentionComponent>())
      {
        DateTime cleanupDate = currentTime.Subtract(TimeSpan.FromSeconds((double) this.m_settings.MentionExpirationTimeInSeconds));
        return component.CleanupMentions(cleanupDate);
      }
    }

    private class DistinctMentionComparer : IEqualityComparer<Microsoft.TeamFoundation.Mention.Server.Mention>
    {
      public bool Equals(Microsoft.TeamFoundation.Mention.Server.Mention x, Microsoft.TeamFoundation.Mention.Server.Mention y) => x.Source.Identifier == y.Source.Identifier && x.Source.Type == y.Source.Type && x.ArtifactId == y.ArtifactId && x.MentionAction == y.MentionAction;

      public int GetHashCode(Microsoft.TeamFoundation.Mention.Server.Mention obj)
      {
        int hashCode = obj.Source.Identifier.GetHashCode() ^ obj.Source.Type.GetHashCode() ^ obj.ArtifactId.GetHashCode();
        if (obj.MentionAction != null)
          obj.MentionAction.GetHashCode();
        return hashCode;
      }
    }

    protected class ServiceSettings
    {
      private const int DefaultMentionLimit = 50;
      private const int DefaultMentionExpirationTimeInSeconds = 2592000;
      private const int DefaultMentionDateTimeLimitInSeconds = 2592000;

      public int MaximumNumberOfMentions { get; }

      public int MentionExpirationTimeInSeconds { get; }

      public int MentionDateTimeLimitInSeconds { get; }

      public ServiceSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, in TeamFoundationMentionService.s_SettingsQuery);
        this.MaximumNumberOfMentions = registryEntryCollection.GetValueFromPath<int>(nameof (MaximumNumberOfMentions), 50);
        this.MentionExpirationTimeInSeconds = registryEntryCollection.GetValueFromPath<int>(nameof (MentionExpirationTimeInSeconds), 2592000);
        this.MentionDateTimeLimitInSeconds = registryEntryCollection.GetValueFromPath<int>(nameof (MentionDateTimeLimitInSeconds), 2592000);
      }
    }
  }
}
