// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitPushJobService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class TeamFoundationGitPushJobService : 
    ITeamFoundationGitPushJobService,
    IVssFrameworkService
  {
    private const string c_layer = "PushJobService";
    private static ConcurrentDictionary<Type, string[]> s_featureFlagAttributeCache = new ConcurrentDictionary<Type, string[]>();
    private IDisposableReadOnlyList<IGitRefUpdateSubscriberOneTimeJob> m_onRefUpdateOneTimeJobs;
    private IDisposableReadOnlyList<GitKeyedJob<RepoJobKey>> m_onRefUpdateRepoScopedJobs;
    private IDisposableReadOnlyList<GitKeyedJob<OdbJobKey>> m_onRefUpdateOdbScopedJobs;
    private IDisposableReadOnlyList<IGitIndexUpdateSubscriberOneTimeJob> m_onIndexUpdateOneTimeJobs;
    private IDisposableReadOnlyList<GitKeyedJob<RepoJobKey>> m_onIndexUpdateRepoScopedJobs;
    private IDisposableReadOnlyList<GitKeyedJob<OdbJobKey>> m_onIndexUpdateOdbScopedJobs;

    public QueuedGitPushJobsContext QueueOnRefUpdateJobs(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      PushNotification pushNotification,
      QueuedGitPushJobsContext queuedGitPushJobsContext = null)
    {
      return this.QueueJobs(requestContext, repository, pushNotification, queuedGitPushJobsContext, (IDisposableReadOnlyList<IGitPushSubscriberOneTimeJob>) this.m_onRefUpdateOneTimeJobs, this.m_onRefUpdateRepoScopedJobs, this.m_onRefUpdateOdbScopedJobs);
    }

    public QueuedGitPushJobsContext QueueOnIndexUpdateJobs(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      PushNotification pushNotification,
      QueuedGitPushJobsContext queuedGitPushJobsContext = null)
    {
      return this.QueueJobs(requestContext, repository, pushNotification, queuedGitPushJobsContext, (IDisposableReadOnlyList<IGitPushSubscriberOneTimeJob>) this.m_onIndexUpdateOneTimeJobs, this.m_onIndexUpdateRepoScopedJobs, this.m_onIndexUpdateOdbScopedJobs);
    }

    private QueuedGitPushJobsContext QueueJobs(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      PushNotification pushNotification,
      QueuedGitPushJobsContext queuedGitPushJobsContext,
      IDisposableReadOnlyList<IGitPushSubscriberOneTimeJob> oneTimeJobs,
      IDisposableReadOnlyList<GitKeyedJob<RepoJobKey>> repoScopedJobs,
      IDisposableReadOnlyList<GitKeyedJob<OdbJobKey>> odbScopedJobs)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repo, nameof (repo));
      ArgumentUtility.CheckForNull<PushNotification>(pushNotification, nameof (pushNotification));
      ArgumentUtility.CheckForNull<IDisposableReadOnlyList<IGitPushSubscriberOneTimeJob>>(oneTimeJobs, nameof (oneTimeJobs));
      ArgumentUtility.CheckForNull<IDisposableReadOnlyList<GitKeyedJob<RepoJobKey>>>(repoScopedJobs, nameof (repoScopedJobs));
      ArgumentUtility.CheckForNull<IDisposableReadOnlyList<GitKeyedJob<OdbJobKey>>>(odbScopedJobs, nameof (odbScopedJobs));
      if (queuedGitPushJobsContext == null)
        queuedGitPushJobsContext = new QueuedGitPushJobsContext();
      if (pushNotification != null)
      {
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        XmlNode jsonXmlNode = pushNotification.SerializeToJsonXmlNode();
        foreach (IGitPushSubscriberOneTimeJob oneTimeJob in (IEnumerable<IGitPushSubscriberOneTimeJob>) oneTimeJobs)
        {
          Type type = oneTimeJob.GetType();
          if (TeamFoundationGitPushJobService.IsFeatureFlagEnabled(requestContext, (ITeamFoundationJobExtension) oneTimeJob) && oneTimeJob.ShouldQueueJob(requestContext, repo, pushNotification) && !queuedGitPushJobsContext.QueuedJobs.Contains(type.FullName))
          {
            service.QueueOneTimeJob(requestContext, type.Name, type.FullName, jsonXmlNode, oneTimeJob.JobPriorityLevel, oneTimeJob.JobPriorityClass, TimeSpan.Zero);
            queuedGitPushJobsContext.QueuedJobs.Add(type.FullName);
          }
        }
        this.DoQueueKeyScopedJobs<RepoJobKey>(requestContext, (IEnumerable<GitKeyedJob<RepoJobKey>>) repoScopedJobs, repo, queuedGitPushJobsContext, (Func<RepoKey, RepoJobKey>) (rk => new RepoJobKey()
        {
          RepositoryId = repo.Key.RepoId
        }));
        this.DoQueueKeyScopedJobs<OdbJobKey>(requestContext, (IEnumerable<GitKeyedJob<OdbJobKey>>) odbScopedJobs, repo, queuedGitPushJobsContext, (Func<RepoKey, OdbJobKey>) (rk => new OdbJobKey(repo.Key.OdbId)));
      }
      return queuedGitPushJobsContext;
    }

    private void DoQueueKeyScopedJobs<T>(
      IVssRequestContext rc,
      IEnumerable<GitKeyedJob<T>> keyScopedJobs,
      ITfsGitRepository repo,
      QueuedGitPushJobsContext queuedGitPushJobsContext,
      Func<RepoKey, T> jobKeyBuilder)
      where T : IGitJobKey
    {
      foreach (GitKeyedJob<T> keyScopedJob in keyScopedJobs)
      {
        Type type = keyScopedJob.GetType();
        if (TeamFoundationGitPushJobService.IsFeatureFlagEnabled(rc, (ITeamFoundationJobExtension) keyScopedJob) && !queuedGitPushJobsContext.QueuedJobs.Contains(type.FullName))
        {
          KeyScopedJobUtil.QueueFor<T>(rc, jobKeyBuilder(repo.Key), type.Name, type.FullName, keyScopedJob.JobPriorityLevel, keyScopedJob.JobPriorityClass, keyScopedJob.GetQueueDelaySeconds(rc, repo));
          queuedGitPushJobsContext.QueuedJobs.Add(type.FullName);
        }
      }
    }

    private static bool IsFeatureFlagEnabled(
      IVssRequestContext requestContext,
      ITeamFoundationJobExtension oneTimeJob)
    {
      Type type = oneTimeJob.GetType();
      string[] source;
      if (!TeamFoundationGitPushJobService.s_featureFlagAttributeCache.TryGetValue(type, out source))
      {
        TeamFoundationGitPushJobService.s_featureFlagAttributeCache[type] = type.GetCustomAttributes(typeof (FeatureEnabledAttribute), true).Cast<FeatureEnabledAttribute>().Select<FeatureEnabledAttribute, string>((Func<FeatureEnabledAttribute, string>) (attribute => attribute.FeatureFlag)).ToArray<string>();
        source = TeamFoundationGitPushJobService.s_featureFlagAttributeCache[type];
      }
      return source.Length == 0 || ((IEnumerable<string>) source).All<string>((Func<string, bool>) (featureFlag => requestContext.IsFeatureEnabled(featureFlag)));
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1013701, GitServerUtils.TraceArea, "PushJobService", "ServiceStart");
      try
      {
        this.m_onRefUpdateOneTimeJobs = systemRequestContext.GetExtensions<IGitRefUpdateSubscriberOneTimeJob>();
        this.m_onRefUpdateRepoScopedJobs = KeyScopedJobUtil.GetGitKeyedJobs<RepoJobKey>(systemRequestContext, (Func<GitKeyedJob<RepoJobKey>, bool>) (dog => dog.QueueOnRefUpdate));
        this.m_onRefUpdateOdbScopedJobs = KeyScopedJobUtil.GetGitKeyedJobs<OdbJobKey>(systemRequestContext, (Func<GitKeyedJob<OdbJobKey>, bool>) (oj => oj.QueueOnRefUpdate));
        this.m_onIndexUpdateOneTimeJobs = systemRequestContext.GetExtensions<IGitIndexUpdateSubscriberOneTimeJob>();
        this.m_onIndexUpdateRepoScopedJobs = KeyScopedJobUtil.GetGitKeyedJobs<RepoJobKey>(systemRequestContext, (Func<GitKeyedJob<RepoJobKey>, bool>) (dog => dog.QueueOnIndexUpdate));
        this.m_onIndexUpdateOdbScopedJobs = KeyScopedJobUtil.GetGitKeyedJobs<OdbJobKey>(systemRequestContext, (Func<GitKeyedJob<OdbJobKey>, bool>) (oj => oj.QueueOnIndexUpdate));
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1013702, GitServerUtils.TraceArea, "PushJobService", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1013703, GitServerUtils.TraceArea, "PushJobService", "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1013705, GitServerUtils.TraceArea, "PushJobService", "ServiceEnd");
      DisposeList<IGitRefUpdateSubscriberOneTimeJob>(ref this.m_onRefUpdateOneTimeJobs);
      DisposeList<GitKeyedJob<RepoJobKey>>(ref this.m_onRefUpdateRepoScopedJobs);
      DisposeList<GitKeyedJob<OdbJobKey>>(ref this.m_onRefUpdateOdbScopedJobs);
      DisposeList<IGitIndexUpdateSubscriberOneTimeJob>(ref this.m_onIndexUpdateOneTimeJobs);
      DisposeList<GitKeyedJob<RepoJobKey>>(ref this.m_onIndexUpdateRepoScopedJobs);
      DisposeList<GitKeyedJob<OdbJobKey>>(ref this.m_onIndexUpdateOdbScopedJobs);

      static void DisposeList<T>(ref IDisposableReadOnlyList<T> list)
      {
        if (list == null)
          return;
        list.Dispose();
        list = (IDisposableReadOnlyList<T>) null;
      }
    }
  }
}
