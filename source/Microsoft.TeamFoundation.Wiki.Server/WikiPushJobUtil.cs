// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPushJobUtil
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  internal static class WikiPushJobUtil
  {
    public static void QueueWikiPushOneTimeJobs(
      IVssRequestContext requestContext,
      WikiPushJobData wikiPushJobdata,
      TimedCiEvent ciData)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WikiPushJobData>(wikiPushJobdata, nameof (wikiPushJobdata));
      try
      {
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IList<(IWikiPushOneTimeJob Job, IList<string> FeatureFlags)> pushSubscriberJobs = vssRequestContext.GetService<IWikiPushJobDetailsService>().GetWikiPushSubscriberJobs(vssRequestContext);
        if (pushSubscriberJobs == null)
        {
          requestContext.TraceErrorAlways("jobDetails should not be null", "Error_WikiPushJobUtil", ciData, 15250705, nameof (WikiPushJobUtil));
        }
        else
        {
          ciData.Properties.AddOrIncrement("WikiPushJobExtensionsCount", (long) pushSubscriberJobs.Count);
          foreach ((IWikiPushOneTimeJob Job, IList<string> FeatureFlags) tuple in (IEnumerable<(IWikiPushOneTimeJob Job, IList<string> FeatureFlags)>) pushSubscriberJobs)
          {
            if (WikiPushJobUtil.IsAllFeatureEnabled(requestContext, tuple.FeatureFlags))
            {
              Type type = tuple.Job.GetType();
              service.QueueOneTimeJob(requestContext, type.Name, type.FullName, TeamFoundationSerializationUtility.SerializeToXml((object) wikiPushJobdata), tuple.Job.JobPriorityLevel, tuple.Job.JobPriorityClass, new TimeSpan(tuple.Job.QueueWithDelayInHours(), 0, 0));
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(ex, 15250705, nameof (WikiPushJobUtil));
      }
    }

    internal static void ConstructJobDataAndQueueWikiPushOneTimeJobs(
      IVssRequestContext requestContext,
      int pushId,
      WikiV2 wiki,
      GitVersionDescriptor version,
      List<WikiPageWithId> addedPages,
      List<WikiPageWithId> editedPages,
      List<WikiPageWithId> renamedPages,
      List<WikiPageWithId> deletedPages,
      List<(string OldFilePath, string NewFilePath, bool IsEdited)> renamedDiffs,
      TimedCiEvent ciEvent)
    {
      WikiPushJobData wikiPushJobdata = WikiPushJobUtil.ConstructWikiPushJobData(pushId, wiki, version, addedPages, editedPages, renamedPages, deletedPages, renamedDiffs, ciEvent);
      WikiPushJobUtil.QueueWikiPushOneTimeJobs(requestContext, wikiPushJobdata, ciEvent);
    }

    internal static WikiPushJobData ConstructWikiPushJobData(
      int pushId,
      WikiV2 wiki,
      GitVersionDescriptor version,
      List<WikiPageWithId> addedPages,
      List<WikiPageWithId> editedPages,
      List<WikiPageWithId> renamedPages,
      List<WikiPageWithId> deletedPages,
      List<(string OldFilePath, string NewFilePath, bool IsEdited)> renamedDiffs,
      TimedCiEvent ciEvent)
    {
      using (new StopWatchHelper(ciEvent, nameof (ConstructWikiPushJobData)))
      {
        List<WikiPageChangeInfo> wikiPageChangeInfoList = new List<WikiPageChangeInfo>();
        wikiPageChangeInfoList.AddRange(addedPages.Select<WikiPageWithId, WikiPageChangeInfo>((Func<WikiPageWithId, WikiPageChangeInfo>) (page => new WikiPageChangeInfo(page.PageId, page.GitFriendlyPagePath.PrependPath(wiki.MappedPath), TfsGitChangeType.Add, (string) null))));
        wikiPageChangeInfoList.AddRange(editedPages.Select<WikiPageWithId, WikiPageChangeInfo>((Func<WikiPageWithId, WikiPageChangeInfo>) (page => new WikiPageChangeInfo(page.PageId, page.GitFriendlyPagePath.PrependPath(wiki.MappedPath), TfsGitChangeType.Edit, (string) null))));
        wikiPageChangeInfoList.AddRange(deletedPages.Select<WikiPageWithId, WikiPageChangeInfo>((Func<WikiPageWithId, WikiPageChangeInfo>) (page => new WikiPageChangeInfo(page.PageId, page.GitFriendlyPagePath.PrependPath(wiki.MappedPath), TfsGitChangeType.Delete, (string) null))));
        Dictionary<string, (string, string, bool)> renamedDiffMap = renamedDiffs.ToDictionary<(string, string, bool), string, (string, string, bool)>((Func<(string, string, bool), string>) (p => p.NewFilePath), (Func<(string, string, bool), (string, string, bool)>) (p => p));
        wikiPageChangeInfoList.AddRange(renamedPages.Select<WikiPageWithId, WikiPageChangeInfo>((Func<WikiPageWithId, WikiPageChangeInfo>) (page =>
        {
          string gitFriendlyPagePath = page.GitFriendlyPagePath.PrependPath(wiki.MappedPath);
          (string, string, bool) tuple = renamedDiffMap[page.GitFriendlyPagePath];
          string oldGitFriendlyPagePath = tuple.Item1.PrependPath(wiki.MappedPath);
          return new WikiPageChangeInfo(page.PageId, gitFriendlyPagePath, tuple.Item3 ? TfsGitChangeType.Edit | TfsGitChangeType.Rename : TfsGitChangeType.Rename, oldGitFriendlyPagePath);
        })));
        return new WikiPushJobData()
        {
          PushId = pushId,
          ProjectId = wiki.ProjectId,
          WikiId = wiki.Id,
          WikiVersion = version,
          ChangedPages = wikiPageChangeInfoList
        };
      }
    }

    internal static bool IsAllFeatureEnabled(IVssRequestContext requestContext, IList<string> flags) => flags.Count == 0 || flags.All<string>(new Func<string, bool>(((VssRequestContextExtensions) requestContext).IsFeatureEnabled));
  }
}
