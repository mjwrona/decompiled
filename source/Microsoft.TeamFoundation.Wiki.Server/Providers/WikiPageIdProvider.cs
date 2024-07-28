// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Providers.WikiPageIdProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.Server.Exceptions;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server.Providers
{
  internal class WikiPageIdProvider
  {
    public IEnumerable<WikiPageWithId> GetAllPagesForWikiVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      GitVersionDescriptor wikiVersion)
    {
      this.EnsureValidWikiVersion(wikiVersion);
      using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
        return component.GetPages(projectId, wikiId, this.GetWikiVersionString(wikiVersion));
    }

    public virtual int? GetPageIdByPath(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      GitVersionDescriptor wikiVersion,
      string pagePath,
      TimedCiEvent ciEvent)
    {
      using (new StopWatchHelper(ciEvent, nameof (GetPageIdByPath)))
      {
        this.EnsureValidWikiVersion(wikiVersion);
        using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
          return component.GetPageIdByPath(projectId, wikiId, this.GetWikiVersionString(wikiVersion), pagePath);
      }
    }

    public void UpdatePageId(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor version,
      List<string> addedDiffs,
      List<(string OldFilePath, string NewFilePath, bool IsEdited)> renamedDiffs,
      IEnumerable<string> deletedDiffs,
      int associatedPushId,
      out List<WikiPageWithId> addedPagesInDb,
      out List<WikiPageWithId> renamedPagesInDb,
      out List<WikiPageWithId> deletedPagesInDb,
      TimedCiEvent ciEvent)
    {
      using (new StopWatchHelper(ciEvent, nameof (UpdatePageId)))
      {
        this.EnsureValidWikiVersion(version);
        string wikiVersionString = this.GetWikiVersionString(version);
        List<WikiPageWithId> addedPages = new List<WikiPageWithId>();
        List<WikiPageWithId> deletedPages = new List<WikiPageWithId>();
        List<WikiRenamedPage> renamedPages = new List<WikiRenamedPage>();
        if (!addedDiffs.IsNullOrEmpty<string>())
        {
          long startId = requestContext.GetService<TeamFoundationCounterService>().ReserveCounterIds(requestContext, "Wiki_PageId", (long) addedDiffs.Count, (string) null, new Guid(), false);
          addedPages.AddRange(addedDiffs.Select<string, WikiPageWithId>((Func<string, int, WikiPageWithId>) ((diff, i) => new WikiPageWithId(Convert.ToInt32(startId + (long) i), diff))));
        }
        if (!renamedDiffs.IsNullOrEmpty<(string, string, bool)>())
          renamedPages.AddRange(renamedDiffs.Select<(string, string, bool), WikiRenamedPage>((Func<(string, string, bool), WikiRenamedPage>) (i => new WikiRenamedPage(i.OldFilePath, i.NewFilePath))));
        if (!deletedDiffs.IsNullOrEmpty<string>())
          deletedPages.AddRange(deletedDiffs.Select<string, WikiPageWithId>((Func<string, WikiPageWithId>) (diff => new WikiPageWithId(-1, diff))));
        addedPagesInDb = new List<WikiPageWithId>();
        renamedPagesInDb = new List<WikiPageWithId>();
        deletedPagesInDb = new List<WikiPageWithId>();
        using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
        {
          if (requestContext.IsFeatureEnabled("Wiki.DisableWikiPageIdSyncDb"))
          {
            component.UpdatePageId(wiki.ProjectId, wiki.Id, wikiVersionString, (IList<WikiPageWithId>) addedPages, (IList<WikiRenamedPage>) renamedPages, (IList<WikiPageWithId>) deletedPages, associatedPushId);
          }
          else
          {
            try
            {
              component.UpdatePageIdIfNeeded(wiki.ProjectId, wiki.Id, wikiVersionString, (IList<WikiPageWithId>) addedPages, (IList<WikiRenamedPage>) renamedPages, (IList<WikiPageWithId>) deletedPages, associatedPushId, out addedPagesInDb, out renamedPagesInDb, out deletedPagesInDb);
            }
            catch (PushNotLatestException ex)
            {
              if (ciEvent != null)
                ciEvent.Properties.AddOrIncrement("Warning_PushNotLatest", 1L);
              requestContext.TraceAlways(15250700, TraceLevel.Info, "Wiki", nameof (WikiPageIdProvider), string.Format("ProjectId:{0} Wikiid:{1}, version:{2} pushid:{3} is not latest", (object) wiki.ProjectId, (object) wiki.Id, (object) wikiVersionString, (object) associatedPushId));
            }
          }
        }
        deletedPagesInDb = deletedPagesInDb.Where<WikiPageWithId>((Func<WikiPageWithId, bool>) (page =>
        {
          bool deletedFileMatchWithDiff = false;
          deletedDiffs.ForEach<string>((Action<string>) (deletedDiff =>
          {
            if (!deletedDiff.Equals(page.GitFriendlyPagePath))
              return;
            deletedFileMatchWithDiff = true;
          }));
          return deletedFileMatchWithDiff;
        })).ToList<WikiPageWithId>();
      }
    }

    public void UnpublishWikiVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      GitVersionDescriptor version,
      TimedCiEvent ciEvent)
    {
      using (new StopWatchHelper(ciEvent, "UpdatePageId"))
      {
        this.EnsureValidWikiVersion(version);
        using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
        {
          string wikiVersionString = this.GetWikiVersionString(version);
          List<WikiPageWithId> deletedPagesInDb;
          component.UnpublishWikiVersion(projectId, wikiId, wikiVersionString, out deletedPagesInDb);
          if (deletedPagesInDb.IsNullOrEmpty<WikiPageWithId>())
            return;
          new WikiJobHandler().QueueWikiMetaDataDeletionJob(requestContext, projectId, wikiId, version, deletedPagesInDb);
        }
      }
    }

    public void DeleteAllWikiPageIds(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      GitVersionDescriptor version,
      int associatedPushId,
      TimedCiEvent ciEvent)
    {
      using (new StopWatchHelper(ciEvent, nameof (DeleteAllWikiPageIds)))
      {
        this.EnsureValidWikiVersion(version);
        using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
        {
          string wikiVersion = string.Format("refs/heads/" + version.Version);
          component.DeleteAllPageIds(projectId, wikiId, wikiVersion, associatedPushId);
        }
      }
    }

    private void EnsureValidWikiVersion(GitVersionDescriptor version)
    {
      ArgumentUtility.CheckForNull<GitVersionDescriptor>(version, nameof (version));
      if (version.VersionType != GitVersionType.Branch)
        throw new NotSupportedException(string.Format("Only Branches are supported, given version descriptor:{0}", (object) version.VersionType));
    }

    private string GetWikiVersionString(GitVersionDescriptor version) => string.Format("refs/heads/" + version.Version);
  }
}
