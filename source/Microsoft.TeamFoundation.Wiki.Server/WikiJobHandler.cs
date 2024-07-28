// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiJobHandler
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiJobHandler
  {
    internal void QueueWikiCreatedJob(IVssRequestContext requestContext, WikiV2 wiki)
    {
      if (!this.IsWikiFeatureFlagEnabled(requestContext, "Wiki.WikiOneTimeJobs", "WikiCreated"))
        return;
      this.QueueWikiVersionPublishedJob(requestContext, wiki, wiki.Versions.First<GitVersionDescriptor>());
    }

    internal void QueueWikiDeletedJob(IVssRequestContext requestContext, WikiV2 wiki)
    {
      if (!this.IsWikiFeatureFlagEnabled(requestContext, "Wiki.WikiOneTimeJobs", "WikiDeleted"))
        return;
      this.QueueWikiVersionsUnpublishedJob(requestContext, wiki, wiki.Versions);
    }

    internal void QueueWikiMetaDataDeletionJob(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      GitVersionDescriptor version,
      List<WikiPageWithId> wikiPages)
    {
      List<WikiPageChangeInfo> wikiPageChangeInfoList = new List<WikiPageChangeInfo>();
      foreach (WikiPageWithId wikiPage in wikiPages)
        wikiPageChangeInfoList.Add(new WikiPageChangeInfo(wikiPage.PageId, (string) null, TfsGitChangeType.Delete, (string) null));
      WikiPushJobData jobData = new WikiPushJobData()
      {
        PushId = -1,
        ProjectId = projectId,
        WikiId = wikiId,
        WikiVersion = version,
        ChangedPages = wikiPageChangeInfoList
      };
      this.QueueOneTimeJob(requestContext, "WikiMetaDataDeletionJob", "Microsoft.TeamFoundation.Wiki.Server.PlugIns.Jobs.WikiMetaDataDeletionJob", (object) jobData, JobPriorityLevel.Normal, JobPriorityClass.Normal, new TimeSpan(requestContext.GetWikiPageMetaDeletionDelayInHours(), 0, 0));
    }

    internal void QueueWikiVersionChangeJob(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IEnumerable<GitVersionDescriptor> addedVersions,
      IEnumerable<GitVersionDescriptor> deletedVersions)
    {
      if (!this.IsWikiFeatureFlagEnabled(requestContext, "Wiki.WikiOneTimeJobs", "WikiVersionChanged"))
        return;
      if (addedVersions == null)
        addedVersions = Enumerable.Empty<GitVersionDescriptor>();
      if (deletedVersions == null)
        deletedVersions = Enumerable.Empty<GitVersionDescriptor>();
      foreach (GitVersionDescriptor addedVersion in addedVersions)
        this.QueueWikiVersionPublishedJob(requestContext, wiki, addedVersion);
      if (!deletedVersions.Any<GitVersionDescriptor>())
        return;
      this.QueueWikiVersionsUnpublishedJob(requestContext, wiki, deletedVersions);
    }

    internal void QueueWikiVersionWaterMarkCatchUpJob(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor version)
    {
      WikiVersionPublishedJobData jobData = new WikiVersionPublishedJobData()
      {
        ProjectId = wiki.ProjectId,
        WikiId = wiki.Id,
        Version = version
      };
      this.QueueOneTimeJob(requestContext, "WikiVersionWaterMarkCatchupJob", "Microsoft.TeamFoundation.Wiki.Server.PlugIns.WikiVersionWaterMarkCatchupJob", (object) jobData, JobPriorityLevel.Normal, JobPriorityClass.Normal, TimeSpan.FromHours((double) requestContext.GetWikiPageMetaDeletionDelayInHours()));
    }

    private void QueueWikiVersionPublishedJob(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor version)
    {
      WikiVersionPublishedJobData jobData = new WikiVersionPublishedJobData()
      {
        ProjectId = wiki.ProjectId,
        WikiId = wiki.Id,
        Version = version
      };
      this.QueueOneTimeJob(requestContext, "WikiPostPublishJob", "Microsoft.TeamFoundation.Wiki.Server.PlugIns.WikiPostPublishJob", (object) jobData, JobPriorityLevel.Normal, JobPriorityClass.Normal, TimeSpan.Zero);
    }

    private void QueueWikiVersionsUnpublishedJob(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IEnumerable<GitVersionDescriptor> versions)
    {
      WikiVersionsUnpublishedJobData jobData = new WikiVersionsUnpublishedJobData()
      {
        ProjectId = wiki.ProjectId,
        WikiId = wiki.Id,
        Versions = versions.ToList<GitVersionDescriptor>()
      };
      this.QueueOneTimeJob(requestContext, "WikiPostUnpublishJob", "Microsoft.TeamFoundation.Wiki.Server.PlugIns.WikiPostUnpublishJob", (object) jobData, JobPriorityLevel.Normal, JobPriorityClass.Normal, TimeSpan.Zero);
    }

    private Guid QueueOneTimeJob(
      IVssRequestContext requestContext,
      string jobName,
      string extensionName,
      object jobData,
      JobPriorityLevel priorityLevel,
      JobPriorityClass priorityClass,
      TimeSpan startOffset)
    {
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml(jobData);
      Guid guid = requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, jobName, extensionName, xml, priorityLevel, priorityClass, startOffset);
      requestContext.TraceAlways(15250701, TraceLevel.Info, "Wiki", "Service", string.Format("Queued a job - jobName: {0}", (object) guid));
      return guid;
    }

    public bool IsWikiFeatureFlagEnabled(
      IVssRequestContext requestContext,
      string featureName,
      string actionStr)
    {
      if (requestContext.IsFeatureEnabled(featureName))
        return true;
      requestContext.TraceAlways(15250702, TraceLevel.Info, "Wiki", "Service", "Wiki Feature Disabled: " + featureName + " For action: " + actionStr);
      return false;
    }
  }
}
