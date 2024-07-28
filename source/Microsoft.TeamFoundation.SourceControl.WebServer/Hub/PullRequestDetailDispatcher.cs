// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Hub.PullRequestDetailDispatcher
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.AspNet.SignalR;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Hub
{
  internal sealed class PullRequestDetailDispatcher : 
    IPullRequestDetailDispatcher,
    IVssFrameworkService
  {
    private IHubContext m_hubContext;
    private const string c_area = "SignalR";
    private const string c_layer = "PullRequestDispatcher";
    private const string c_hubName = "PullRequestDetailHub";
    private const int c_maxReviewers = 50;

    public GitPullRequest Subscribe(
      IVssRequestContext requestContext,
      int pullRequestId,
      string repositoryId,
      string clientId)
    {
      this.m_hubContext.Groups.AddTrackedConnection(requestContext, "PullRequestDetailHub", this.GetGroupName(requestContext, pullRequestId), clientId);
      try
      {
        using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, new Guid(repositoryId)))
        {
          if (repositoryById == null)
            return (GitPullRequest) null;
          TfsGitPullRequest pullRequest;
          requestContext.GetService<ITeamFoundationGitPullRequestService>().TryGetPullRequestDetails(requestContext, repositoryById, pullRequestId, out pullRequest);
          if (pullRequest == null)
            return (GitPullRequest) null;
          GitPullRequest webApiItem = pullRequest.ToWebApiItem(requestContext, repositoryById, pullRequest.GetForkRepositoryRef(requestContext, repositoryById));
          IdentityRefWithVote[] reviewers = webApiItem.Reviewers;
          if ((reviewers != null ? (reviewers.Length > 50 ? 1 : 0) : 0) != 0)
            webApiItem.Reviewers = (IdentityRefWithVote[]) null;
          // ISSUE: reference to a compiler-generated field
          if (PullRequestDetailDispatcher.\u003C\u003Eo__0.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PullRequestDetailDispatcher.\u003C\u003Eo__0.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target = PullRequestDetailDispatcher.\u003C\u003Eo__0.\u003C\u003Ep__1.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p1 = PullRequestDetailDispatcher.\u003C\u003Eo__0.\u003C\u003Ep__1;
          // ISSUE: reference to a compiler-generated field
          if (PullRequestDetailDispatcher.\u003C\u003Eo__0.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PullRequestDetailDispatcher.\u003C\u003Eo__0.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj = PullRequestDetailDispatcher.\u003C\u003Eo__0.\u003C\u003Ep__0.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__0.\u003C\u003Ep__0, this.m_hubContext.Clients.Client(clientId), (object) null);
          if (target((CallSite) p1, obj))
            return webApiItem;
        }
      }
      catch (Exception ex)
      {
        TracepointUtils.TraceException(requestContext, 1013679, "SignalR", "PullRequestDispatcher", ex, caller: nameof (Subscribe));
      }
      return (GitPullRequest) null;
    }

    public Task Unsubscribe(IVssRequestContext requestContext, int pullRequestId, string clientId) => this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "PullRequestDetailHub", this.GetGroupName(requestContext, pullRequestId), clientId);

    public Task Disconnect(IVssRequestContext requestContext, string clientId, bool stopCalled) => this.m_hubContext.RemoveTrackedConnection(requestContext, "PullRequestDetailHub", clientId, !stopCalled);

    public void SendRealtimeEvent(
      IVssRequestContext requestContext,
      RealTimePullRequestEvent realTimeEvent)
    {
      TracepointUtils.Tracepoint(requestContext, 1013675, "SignalR", "PullRequestDispatcher", (Func<object>) (() => (object) new
      {
        realTimeEvent = realTimeEvent
      }), caller: nameof (SendRealtimeEvent));
      try
      {
        object obj = this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PullRequestDetailHub", this.GetGroupName(requestContext, realTimeEvent.PullRequestId));
        if (!(realTimeEvent is AutoCompleteUpdatedEvent completeUpdatedEvent))
        {
          if (!(realTimeEvent is BranchUpdatedEvent branchUpdatedEvent))
          {
            if (!(realTimeEvent is CompletionErrorsEvent completionErrorsEvent))
            {
              if (!(realTimeEvent is DiscussionsUpdatedEvent discussionsUpdatedEvent))
              {
                if (!(realTimeEvent is IsDraftUpdatedEvent draftUpdatedEvent))
                {
                  if (!(realTimeEvent is LabelsUpdatedEvent labelsUpdatedEvent))
                  {
                    if (!(realTimeEvent is MergeCompletedEvent mergeCompletedEvent))
                    {
                      if (!(realTimeEvent is PolicyEvaluationUpdatedEvent evaluationUpdatedEvent))
                      {
                        if (!(realTimeEvent is ReviewersUpdatedEvent reviewersUpdatedEvent))
                        {
                          if (!(realTimeEvent is ReviewersVotesResetEvent reviewersVotesResetEvent))
                          {
                            if (!(realTimeEvent is ReviewerVoteUpdatedEvent voteUpdatedEvent))
                            {
                              if (!(realTimeEvent is StatusAddedEvent statusAddedEvent))
                              {
                                if (!(realTimeEvent is StatusesDeletedEvent statusesDeletedEvent))
                                {
                                  if (!(realTimeEvent is StatusUpdatedEvent statusUpdatedEvent))
                                  {
                                    if (!(realTimeEvent is TitleDescriptionUpdatedEvent descriptionUpdatedEvent))
                                    {
                                      if (!(realTimeEvent is RetargetEvent retargetEvent))
                                      {
                                        if (realTimeEvent == null)
                                          return;
                                        RealTimePullRequestEvent pullRequestEvent = realTimeEvent;
                                        // ISSUE: reference to a compiler-generated field
                                        if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__17 == null)
                                        {
                                          // ISSUE: reference to a compiler-generated field
                                          PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__17 = CallSite<Action<CallSite, object, IVssRequestContext, RealTimePullRequestEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnRealTimePullRequestUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                                          {
                                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                                          }));
                                        }
                                        // ISSUE: reference to a compiler-generated field
                                        // ISSUE: reference to a compiler-generated field
                                        PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__17.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__17, obj, requestContext, pullRequestEvent);
                                      }
                                      else
                                      {
                                        // ISSUE: reference to a compiler-generated field
                                        if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__16 == null)
                                        {
                                          // ISSUE: reference to a compiler-generated field
                                          PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__16 = CallSite<Action<CallSite, object, IVssRequestContext, RetargetEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnTargetChanged", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                                          {
                                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                                          }));
                                        }
                                        // ISSUE: reference to a compiler-generated field
                                        // ISSUE: reference to a compiler-generated field
                                        PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__16.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__16, obj, requestContext, retargetEvent);
                                      }
                                    }
                                    else
                                    {
                                      // ISSUE: reference to a compiler-generated field
                                      if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__15 == null)
                                      {
                                        // ISSUE: reference to a compiler-generated field
                                        PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__15 = CallSite<Action<CallSite, object, IVssRequestContext, TitleDescriptionUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnTitleDescriptionUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                                        {
                                          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                                          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                                          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                                        }));
                                      }
                                      // ISSUE: reference to a compiler-generated field
                                      // ISSUE: reference to a compiler-generated field
                                      PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__15.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__15, obj, requestContext, descriptionUpdatedEvent);
                                    }
                                  }
                                  else
                                  {
                                    // ISSUE: reference to a compiler-generated field
                                    if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__14 == null)
                                    {
                                      // ISSUE: reference to a compiler-generated field
                                      PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__14 = CallSite<Action<CallSite, object, IVssRequestContext, StatusUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnStatusUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                                      {
                                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                                      }));
                                    }
                                    // ISSUE: reference to a compiler-generated field
                                    // ISSUE: reference to a compiler-generated field
                                    PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__14.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__14, obj, requestContext, statusUpdatedEvent);
                                  }
                                }
                                else
                                {
                                  // ISSUE: reference to a compiler-generated field
                                  if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__13 == null)
                                  {
                                    // ISSUE: reference to a compiler-generated field
                                    PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__13 = CallSite<Action<CallSite, object, IVssRequestContext, StatusesDeletedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnStatusesDeleted", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                                    {
                                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                                    }));
                                  }
                                  // ISSUE: reference to a compiler-generated field
                                  // ISSUE: reference to a compiler-generated field
                                  PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__13.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__13, obj, requestContext, statusesDeletedEvent);
                                }
                              }
                              else
                              {
                                // ISSUE: reference to a compiler-generated field
                                if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__12 == null)
                                {
                                  // ISSUE: reference to a compiler-generated field
                                  PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__12 = CallSite<Action<CallSite, object, IVssRequestContext, StatusAddedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnStatusAdded", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                                  {
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                                  }));
                                }
                                // ISSUE: reference to a compiler-generated field
                                // ISSUE: reference to a compiler-generated field
                                PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__12.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__12, obj, requestContext, statusAddedEvent);
                              }
                            }
                            else
                            {
                              // ISSUE: reference to a compiler-generated field
                              if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__11 == null)
                              {
                                // ISSUE: reference to a compiler-generated field
                                PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__11 = CallSite<Action<CallSite, object, IVssRequestContext, ReviewerVoteUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnReviewerVoteUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                                {
                                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                                }));
                              }
                              // ISSUE: reference to a compiler-generated field
                              // ISSUE: reference to a compiler-generated field
                              PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__11.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__11, obj, requestContext, voteUpdatedEvent);
                            }
                          }
                          else
                          {
                            // ISSUE: reference to a compiler-generated field
                            if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__10 == null)
                            {
                              // ISSUE: reference to a compiler-generated field
                              PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__10 = CallSite<Action<CallSite, object, IVssRequestContext, ReviewersVotesResetEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnReviewersVotesReset", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                              {
                                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                              }));
                            }
                            // ISSUE: reference to a compiler-generated field
                            // ISSUE: reference to a compiler-generated field
                            PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__10.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__10, obj, requestContext, reviewersVotesResetEvent);
                          }
                        }
                        else
                        {
                          // ISSUE: reference to a compiler-generated field
                          if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__8 == null)
                          {
                            // ISSUE: reference to a compiler-generated field
                            PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__8 = CallSite<Action<CallSite, object, IVssRequestContext, ReviewersUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnReviewersUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                            {
                              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                            }));
                          }
                          // ISSUE: reference to a compiler-generated field
                          // ISSUE: reference to a compiler-generated field
                          PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__8.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__8, obj, requestContext, reviewersUpdatedEvent);
                          // ISSUE: reference to a compiler-generated field
                          if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__9 == null)
                          {
                            // ISSUE: reference to a compiler-generated field
                            PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__9 = CallSite<Action<CallSite, object, IVssRequestContext, int, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnPullRequestUpdate", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[5]
                            {
                              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                            }));
                          }
                          // ISSUE: reference to a compiler-generated field
                          // ISSUE: reference to a compiler-generated field
                          PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__9.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__9, obj, requestContext, reviewersUpdatedEvent.PullRequestId, "reviewers", (object) null);
                        }
                      }
                      else
                      {
                        // ISSUE: reference to a compiler-generated field
                        if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__7 == null)
                        {
                          // ISSUE: reference to a compiler-generated field
                          PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__7 = CallSite<Action<CallSite, object, IVssRequestContext, PolicyEvaluationUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnPolicyEvaluationUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                          {
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                          }));
                        }
                        // ISSUE: reference to a compiler-generated field
                        // ISSUE: reference to a compiler-generated field
                        PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__7.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__7, obj, requestContext, evaluationUpdatedEvent);
                      }
                    }
                    else
                    {
                      // ISSUE: reference to a compiler-generated field
                      if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__6 == null)
                      {
                        // ISSUE: reference to a compiler-generated field
                        PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__6 = CallSite<Action<CallSite, object, IVssRequestContext, MergeCompletedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnMergeCompleted", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                        {
                          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                        }));
                      }
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__6.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__6, obj, requestContext, mergeCompletedEvent);
                    }
                  }
                  else
                  {
                    // ISSUE: reference to a compiler-generated field
                    if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__5 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__5 = CallSite<Action<CallSite, object, IVssRequestContext, LabelsUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnLabelsUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__5.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__5, obj, requestContext, labelsUpdatedEvent);
                  }
                }
                else
                {
                  // ISSUE: reference to a compiler-generated field
                  if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__4 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__4 = CallSite<Action<CallSite, object, IVssRequestContext, IsDraftUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnIsDraftUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__4.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__4, obj, requestContext, draftUpdatedEvent);
                }
              }
              else
              {
                // ISSUE: reference to a compiler-generated field
                if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__3 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__3 = CallSite<Action<CallSite, object, IVssRequestContext, DiscussionsUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnDiscussionsUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__3.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__3, obj, requestContext, discussionsUpdatedEvent);
              }
            }
            else
            {
              // ISSUE: reference to a compiler-generated field
              if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__2 == null)
              {
                // ISSUE: reference to a compiler-generated field
                PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__2 = CallSite<Action<CallSite, object, IVssRequestContext, CompletionErrorsEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnCompletionErrors", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__2.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__2, obj, requestContext, completionErrorsEvent);
            }
          }
          else
          {
            // ISSUE: reference to a compiler-generated field
            if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__1 == null)
            {
              // ISSUE: reference to a compiler-generated field
              PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__1 = CallSite<Action<CallSite, object, IVssRequestContext, BranchUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnBranchUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__1.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__1, obj, requestContext, branchUpdatedEvent);
          }
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, AutoCompleteUpdatedEvent>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "OnAutoCompleteUpdated", (IEnumerable<Type>) null, typeof (PullRequestDetailDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) PullRequestDetailDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0, obj, requestContext, completeUpdatedEvent);
        }
      }
      catch (Exception ex)
      {
        TracepointUtils.TraceException(requestContext, 1013673, "SignalR", "PullRequestDispatcher", ex, caller: nameof (SendRealtimeEvent));
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<PullRequestDetailHub>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.m_hubContext = (IHubContext) null;

    private string GetGroupName(IVssRequestContext requestContext, int pullRequestId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_PullRequest_{1}", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) pullRequestId);
  }
}
