// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPageIdHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server.Exceptions;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  internal static class WikiPageIdHelper
  {
    public static void PublishWikiPages(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      ITfsGitRepository repo,
      GitVersionDescriptor versionDescriptor,
      WikiPageIdProvider pageIdProvider,
      out List<WikiPageWithId> wikiPages,
      TimedCiEvent ciData = null,
      StringBuilder messageBuilder = null)
    {
      using (new StopWatchHelper(ciData, nameof (PublishWikiPages)))
      {
        string refNameStr = "refs/heads/" + versionDescriptor.Version;
        ITfsGitRepository repo1 = repo;
        Guid? nullable1 = new Guid?();
        int? nullable2 = new int?(0);
        int? nullable3 = new int?(1);
        string str = refNameStr;
        DateTime? fromDate = new DateTime?();
        DateTime? toDate = new DateTime?();
        Guid? pusherId = nullable1;
        int? skip = nullable2;
        int? take = nullable3;
        string refName = str;
        List<TfsGitPushMetadata> source = repo1.QueryPushHistory(true, fromDate, toDate, pusherId, skip, take, refName);
        TfsGitPushMetadata tfsGitPushMetadata = source != null && source.Count != 0 ? source.First<TfsGitPushMetadata>() : throw new PushNotFoundException("Unable to publish on ref with no pushes");
        List<TfsGitRefLogEntry> refLog = tfsGitPushMetadata.RefLog;
        GitVersionDescriptor versionDescriptor1 = new GitVersionDescriptor()
        {
          Version = ((refLog != null ? refLog.Where<TfsGitRefLogEntry>((Func<TfsGitRefLogEntry, bool>) (r => r.Name == refNameStr)).Single<TfsGitRefLogEntry>() : (TfsGitRefLogEntry) null) ?? throw new TeamFoundationServiceException("Unable to publish on push with no reflog")).NewObjectId.ToString(),
          VersionType = GitVersionType.Commit
        };
        List<string> addedDiffs;
        try
        {
          addedDiffs = new WikiPagesProvider().GetFlattenedPagePathList(requestContext, repo, wiki.MappedPath, versionDescriptor1, VersionControlRecursionType.Full, ciData);
        }
        catch (GitCommitDoesNotExistException ex)
        {
          addedDiffs = new List<string>();
          requestContext.TraceAlways(15250700, TraceLevel.Info, "Wiki", nameof (WikiPageIdHelper), string.Format("ProjectId:{0} Wikiid:{1}, version:{2} Commit: {3} Does not exist", (object) wiki.ProjectId, (object) wiki.Id, (object) versionDescriptor.Version, (object) versionDescriptor1.Version));
        }
        pageIdProvider.UpdatePageId(requestContext, wiki, versionDescriptor, addedDiffs, (List<(string, string, bool)>) null, (IEnumerable<string>) null, tfsGitPushMetadata.PushId, out wikiPages, out List<WikiPageWithId> _, out List<WikiPageWithId> _, ciData);
        ciData?.Properties.Add("WikiPublishedPagesCount", (object) addedDiffs.Count);
        messageBuilder?.Append(string.Format("Successfully processed push {0}, added {1} pages to wiki", (object) tfsGitPushMetadata.PushId, (object) addedDiffs.Count));
        messageBuilder?.Append(string.Format("Version publish for wiki: {0}", (object) wiki.Id));
      }
    }

    public static void UnpublishWikiPages(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      GitVersionDescriptor version,
      TimedCiEvent ciData,
      WikiPageIdProvider pageIdProvider,
      StringBuilder messageBuilder)
    {
      WikiPageIdHelper.UnpublishWikiPages(requestContext, projectId, wikiId, (IEnumerable<GitVersionDescriptor>) new GitVersionDescriptor[1]
      {
        version
      }, ciData, pageIdProvider, messageBuilder);
    }

    public static void UnpublishWikiPages(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      IEnumerable<GitVersionDescriptor> versions,
      TimedCiEvent ciData,
      WikiPageIdProvider pageIdProvider,
      StringBuilder messageBuilder)
    {
      versions.ToList<GitVersionDescriptor>().ForEach((Action<GitVersionDescriptor>) (version => pageIdProvider.UnpublishWikiVersion(requestContext, projectId, wikiId, version, ciData)));
      messageBuilder.Append(string.Format("Successfully deleted {0} versions from wiki {1}", (object) versions.Count<GitVersionDescriptor>(), (object) wikiId));
      ciData.Properties.Add("WikiUnpublishedVersionsCount", (object) versions.Count<GitVersionDescriptor>());
    }

    public static string GetArtifactId(Guid projectId, Guid wikiId, int pageId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) projectId, (object) wikiId, (object) pageId);

    public static void GetIDsFromArtifactiId(
      string artifactId,
      out Guid projectId,
      out Guid wikiId,
      out int pageId)
    {
      string[] strArray = artifactId.Split('/');
      projectId = strArray.Length == 3 ? new Guid(strArray[0]) : throw new ArgumentException(Resources.WikiPageCommentInvalidArtifactId);
      wikiId = new Guid(strArray[1]);
      pageId = int.Parse(strArray[2]);
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(wikiId, nameof (wikiId));
    }

    public static void GetIDsfromViewsArtifactId(
      string artifactId,
      out Guid wikiId,
      out int pageId)
    {
      string[] strArray = artifactId.Split('/');
      wikiId = strArray.Length == 2 ? new Guid(strArray[0]) : throw new ArgumentException("Invalid ArtifactId: " + artifactId);
      pageId = int.Parse(strArray[1]);
      ArgumentUtility.CheckForEmptyGuid(wikiId, nameof (wikiId));
    }

    internal static bool ProcessWikiVersion(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersion,
      TimedCiEvent ciData,
      WikiPushWaterMarkProvider waterMarkProvider,
      WikiPageIdProvider pageIdProvider,
      out List<WikiPageWithId> addedPages,
      out List<WikiPageWithId> renamedPages,
      out List<WikiPageWithId> deletedPages)
    {
      int? wikiPushWaterMark = waterMarkProvider.GetWikiPushWaterMark(requestContext, wiki, wikiVersion, ciData);
      addedPages = new List<WikiPageWithId>();
      renamedPages = new List<WikiPageWithId>();
      deletedPages = new List<WikiPageWithId>();
      if (!wikiPushWaterMark.HasValue)
      {
        requestContext.TraceErrorAlways(string.Format("No watermark present for wiki:{0}, version:{1}, this can happen when the wiki version publish has not processed page Id yet", (object) wiki.Id, (object) wikiVersion), "Error_WaterMarkAbsent", ciData);
        WikiPageIdHelper.QueueCatchUpJobIfRequired(requestContext, wiki, wikiVersion);
        return false;
      }
      List<TfsGitRefLogEntry> gitRefLogEntries = WikiPageIdHelper.GetNextGitRefLogEntries(requestContext, wiki, repo, wikiVersion, ciData, wikiPushWaterMark);
      string mappedPath = wiki.MappedPath;
      foreach (TfsGitRefLogEntry push in gitRefLogEntries)
      {
        if (!WikiPageIdHelper.ProcessPushForWikiVersion(requestContext, repo, wiki, wikiVersion, ciData, pageIdProvider, push, mappedPath, out addedPages, out renamedPages, out deletedPages))
          return false;
      }
      return true;
    }

    internal static string GetPushWaterMarkRegistryKey(Guid projectId, Guid wikiId, string version) => string.Format("{0}/{1}/{2}/{3}", (object) "/WikiVersion/PushWaterMark", (object) projectId, (object) wikiId, (object) version);

    internal static DateTime GetPushWaterMarkRegistryValue(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersion)
    {
      string waterMarkRegistryKey = WikiPageIdHelper.GetPushWaterMarkRegistryKey(wiki.ProjectId, wiki.Id, wikiVersion.Version);
      return requestContext.GetService<IVssRegistryService>().GetValue<DateTime>(requestContext, (RegistryQuery) waterMarkRegistryKey, true, DateTime.MinValue);
    }

    private static void QueueCatchUpJobIfRequired(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersion)
    {
      DateTime markRegistryValue = WikiPageIdHelper.GetPushWaterMarkRegistryValue(requestContext, wiki, wikiVersion);
      if (!markRegistryValue.Equals(DateTime.MinValue) && markRegistryValue.Subtract(DateTime.Now).TotalHours <= (double) requestContext.GetWikiWikiPushWaterMarkCatchUpDelayInHours())
        return;
      requestContext.GetService<IVssRegistryService>().SetValue<DateTime>(requestContext, WikiPageIdHelper.GetPushWaterMarkRegistryKey(wiki.ProjectId, wiki.Id, wikiVersion.Version), DateTime.Now);
      new WikiJobHandler().QueueWikiVersionWaterMarkCatchUpJob(requestContext, wiki, wikiVersion);
    }

    private static bool ProcessPushForWikiVersion(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersion,
      TimedCiEvent ciData,
      WikiPageIdProvider pageIdProvider,
      TfsGitRefLogEntry push,
      string mappedPath,
      out List<WikiPageWithId> addedPages,
      out List<WikiPageWithId> renamedPages,
      out List<WikiPageWithId> deletedPages)
    {
      using (new StopWatchHelper(ciData, "PushPerWikiVersion"))
      {
        Sha1Id oldObjectId = push.OldObjectId;
        Sha1Id newObjectId = push.NewObjectId;
        if (oldObjectId == Sha1Id.Empty && newObjectId == Sha1Id.Empty)
        {
          requestContext.TraceErrorAlways(string.Format("Object Id cannot be empty OldObjectId:{0}, NewObjectId:{1}, wikiVersion:{2}, pushid:{3}", (object) oldObjectId, (object) newObjectId, (object) wikiVersion, (object) push.PushId), "Error_InvalidRefLog", ciData);
          addedPages = new List<WikiPageWithId>();
          renamedPages = new List<WikiPageWithId>();
          deletedPages = new List<WikiPageWithId>();
          return false;
        }
        if (newObjectId == Sha1Id.Empty)
        {
          ciData.Properties.AddOrIncrement("WikiBranchDeletedCount", 1L);
          WikiPageIdHelper.UnPublishWikiVersion(requestContext, wiki, wikiVersion, ciData);
          addedPages = new List<WikiPageWithId>();
          renamedPages = new List<WikiPageWithId>();
          deletedPages = new List<WikiPageWithId>();
        }
        else if (oldObjectId == Sha1Id.Empty)
        {
          ciData.Properties.AddOrIncrement("WikiBranchRecreatedCount", 1L);
          WikiPageIdHelper.PublishWikiPages(requestContext, wiki, repo, wikiVersion, pageIdProvider, out addedPages, ciData);
          renamedPages = new List<WikiPageWithId>();
          deletedPages = new List<WikiPageWithId>();
        }
        else
        {
          TfsGitCommit commitFromVersion1 = GitVersionParser.GetCommitFromVersion(requestContext, repo, new GitVersionDescriptor()
          {
            Version = oldObjectId.ToString(),
            VersionType = GitVersionType.Commit
          });
          TfsGitCommit commitFromVersion2 = GitVersionParser.GetCommitFromVersion(requestContext, repo, new GitVersionDescriptor()
          {
            Version = newObjectId.ToString(),
            VersionType = GitVersionType.Commit
          });
          IList<TfsGitDiffEntry> allFileDiff = WikiPageIdHelper.GetAllFileDiff(repo, commitFromVersion1, commitFromVersion2, ciData);
          List<string> addedDiffs;
          List<(string, string, bool)> renamedDiffs;
          List<string> deletedDiffs;
          List<string> editOnlyDiffs;
          bool classifiedDiffs = WikiPageIdHelper.GetClassifiedDiffs(requestContext, allFileDiff, mappedPath, out addedDiffs, out renamedDiffs, out deletedDiffs, out editOnlyDiffs, ciData);
          pageIdProvider.UpdatePageId(requestContext, wiki, wikiVersion, addedDiffs, renamedDiffs, (IEnumerable<string>) deletedDiffs, push.PushId, out addedPages, out renamedPages, out deletedPages, ciData);
          ciData.Properties.AddOrIncrement("InterestedPushPerWikiVersionCount", (long) classifiedDiffs);
          ciData.Properties.AddOrIncrement("RenamedPageCount", (long) renamedDiffs.Count);
          ciData.Properties.AddOrIncrement("DeletedPageCount", (long) deletedDiffs.Count);
          ciData.Properties.AddOrIncrement("AddedPageCount", (long) addedDiffs.Count);
          ciData.Properties.AddOrIncrement("EditedPageCount", (long) editOnlyDiffs.Count);
          ciData.Properties.AddOrIncrement("EditRenamedPageCount", (long) renamedDiffs.Where<(string, string, bool)>((Func<(string, string, bool), bool>) (diff => diff.IsEdited)).Count<(string, string, bool)>());
          ciData.Properties["WikiId"] = (object) wiki.Id;
          ciData.Properties["RepoId"] = (object) repo.Key.RepoId;
          ciData.Properties["Version"] = (object) wikiVersion.Version;
          ciData.Properties["WikiType"] = (object) wiki.Type;
          ciData.Properties["PushId"] = (object) push.PushId;
          List<WikiPageWithId> editedPages = WikiPageIdHelper.GetEditedPages(requestContext, wiki, wikiVersion, editOnlyDiffs, pageIdProvider, ciData);
          if (classifiedDiffs)
          {
            try
            {
              WikiPushJobUtil.ConstructJobDataAndQueueWikiPushOneTimeJobs(requestContext, push.PushId, wiki, wikiVersion, addedPages, editedPages, renamedPages, deletedPages, renamedDiffs, ciData);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(ex, 15250709);
            }
          }
          WikiPageIdHelper.EmitAzureDevopsWikiCi(requestContext, repo, wiki, push, ciData);
          WikiPageIdHelper.EmitPageIdProcessingDelayCi(requestContext, repo, push.PushId, ciData);
        }
        return true;
      }
    }

    public static void EmitPageIdProcessingDelayCi(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      int pushId,
      TimedCiEvent ciData)
    {
      using (new StopWatchHelper(ciData, nameof (EmitPageIdProcessingDelayCi)))
      {
        try
        {
          TfsGitPushMetadata tfsGitPushMetadata = requestContext.GetService<ITeamFoundationGitCommitService>().GetPushDataForPushIds(requestContext, repo.Key, new int[1]
          {
            pushId
          }, true).First<TfsGitPushMetadata>();
          DateTime utcNow = DateTime.UtcNow;
          ciData.Properties["PageIdJobTimeUtc"] = (object) utcNow;
          ciData.Properties["PushTimeUtc"] = (object) tfsGitPushMetadata.PushTime.ToUniversalTime();
          ciData.Properties["PageIdDelayInms"] = (object) (utcNow - tfsGitPushMetadata.PushTime).TotalMilliseconds;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ex, 15250709);
        }
      }
    }

    private static void EmitAzureDevopsWikiCi(
      IVssRequestContext requestContext,
      ITfsGitRepository repo,
      WikiV2 wiki,
      TfsGitRefLogEntry push,
      TimedCiEvent ciData)
    {
      Guid guid = new Guid("0d32bfdc-56d7-4d3a-afe2-a5fc4011a0a7");
      if (wiki.Id != guid)
        return;
      try
      {
        using (new StopWatchHelper(ciData, "AzureDevopsWikiCi"))
        {
          ITeamFoundationGitCommitService service = requestContext.GetService<ITeamFoundationGitCommitService>();
          TfsGitPushMetadata tfsGitPushMetadata = service.GetPushDataForPushIds(requestContext, repo.Key, new int[1]
          {
            push.PushId
          }, true).First<TfsGitPushMetadata>();
          TeamFoundationIdentity foundationIdentity = ((IEnumerable<TeamFoundationIdentity>) requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, new Guid[1]
          {
            tfsGitPushMetadata.PusherId
          })).First<TeamFoundationIdentity>();
          int num = 0;
label_3:
          ITeamFoundationGitCommitService gitCommitService = service;
          IVssRequestContext requestContext1 = requestContext;
          ITfsGitRepository repository = repo;
          Sha1Id? commitId = new Sha1Id?(push.OldObjectId);
          Sha1Id? nullable1 = new Sha1Id?(push.NewObjectId);
          int? nullable2 = new int?(num);
          int? nullable3 = new int?(100);
          DateTime? fromDate = new DateTime?();
          DateTime? toDate = new DateTime?();
          Sha1Id? fromCommitId = new Sha1Id?();
          Sha1Id? toCommitId = new Sha1Id?();
          Sha1Id? compareCommitId = nullable1;
          int? skip = nullable2;
          int? maxItemCount = nullable3;
          IEnumerable<TfsGitCommitHistoryEntry> source = gitCommitService.QueryCommitHistory(requestContext1, repository, commitId, (string) null, false, fromDate: fromDate, toDate: toDate, fromCommitId: fromCommitId, toCommitId: toCommitId, compareCommitId: compareCommitId, skip: skip, maxItemCount: maxItemCount);
          num += 100;
          if (source == null || !source.Any<TfsGitCommitHistoryEntry>())
            return;
          using (IEnumerator<TfsGitCommitHistoryEntry> enumerator = source.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              TfsGitCommitHistoryEntry current = enumerator.Current;
              string name;
              string email1;
              GitModelExtensions.ParseNameEmail(current.Commit.Author, out name, out email1);
              string email2;
              GitModelExtensions.ParseNameEmail(current.Commit.Committer, out name, out email2);
              if (email1 != null && foundationIdentity != null && !email1.Equals(foundationIdentity.UniqueName, StringComparison.InvariantCultureIgnoreCase))
              {
                ciData.Properties.AddOrIncrement("AuthorPusherMisMatch", 1L);
                return;
              }
              if (email2 != null && foundationIdentity != null && !email2.Equals(foundationIdentity.UniqueName, StringComparison.InvariantCultureIgnoreCase))
              {
                ciData.Properties.AddOrIncrement("CommitterPusherMisMatch", 1L);
                return;
              }
            }
            goto label_3;
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(ex);
      }
    }

    private static void UnPublishWikiVersion(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersionToUnpublish,
      TimedCiEvent ciData)
    {
      using (new StopWatchHelper(ciData, nameof (UnPublishWikiVersion)))
      {
        WikiV2 wikiById = WikiV2Helper.GetWikiById(requestContext, wiki.ProjectId, wiki.Id, ciData);
        if (wikiById?.Versions == null)
        {
          requestContext.TraceErrorAlways("Unable to unpublish wiki version:" + wikiVersionToUnpublish.Version, "Error_UnableToUnpublishVersion", ciData);
        }
        else
        {
          GitVersionDescriptorComparer comparer = new GitVersionDescriptorComparer();
          IEnumerable<GitVersionDescriptor> versionDescriptors = wikiById.Versions.Where<GitVersionDescriptor>((Func<GitVersionDescriptor, bool>) (version => !comparer.Equals(version, wikiVersionToUnpublish)));
          WikiJobHandler wikiJobHandler = new WikiJobHandler();
          if (versionDescriptors.Any<GitVersionDescriptor>())
            WikiV2Helper.UpdateWiki(requestContext, wikiById.ProjectId, wikiById.Id.ToString(), versionDescriptors, wikiJobHandler);
          else
            WikiV2Helper.DeleteWiki(requestContext, wikiById.ProjectId, wikiById.Id.ToString(), wikiJobHandler);
        }
      }
    }

    internal static List<WikiPageWithId> GetEditedPages(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor version,
      List<string> editedDiffs,
      WikiPageIdProvider pageIdProvider,
      TimedCiEvent ciEvent)
    {
      using (new StopWatchHelper(ciEvent, nameof (GetEditedPages)))
      {
        List<WikiPageWithId> editedPages = new List<WikiPageWithId>();
        foreach (string editedDiff in editedDiffs)
        {
          int? pageIdByPath = pageIdProvider.GetPageIdByPath(requestContext, wiki.ProjectId, wiki.Id, version, editedDiff, ciEvent);
          if (pageIdByPath.HasValue)
            editedPages.Add(new WikiPageWithId(pageIdByPath.Value, editedDiff));
        }
        return editedPages;
      }
    }

    internal static bool GetClassifiedDiffs(
      IVssRequestContext requestContext,
      IList<TfsGitDiffEntry> allDiffs,
      string mappedPath,
      out List<string> addedDiffs,
      out List<(string OldFilePath, string NewFilePath, bool IsEdited)> renamedDiffs,
      out List<string> deletedDiffs,
      out List<string> editOnlyDiffs,
      TimedCiEvent ciData)
    {
      IEnumerable<TfsGitDiffEntry> diffsInMappedPath = WikiPageIdHelper.GetDiffsInMappedPath(WikiPageIdHelper.GetMdFileDiff((IEnumerable<TfsGitDiffEntry>) allDiffs), mappedPath);
      return WikiPageIdHelper.ClassifyDiffs(requestContext, diffsInMappedPath, mappedPath, out addedDiffs, out renamedDiffs, out deletedDiffs, out editOnlyDiffs, ciData);
    }

    internal static bool ClassifyDiffs(
      IVssRequestContext requestContext,
      IEnumerable<TfsGitDiffEntry> diffs,
      string mappedPath,
      out List<string> addedDiffs,
      out List<(string OldFilePath, string NewFilePath, bool IsEdited)> renamedDiffs,
      out List<string> deletedDiffs,
      out List<string> editOnlyDiffs,
      TimedCiEvent ciData)
    {
      addedDiffs = new List<string>();
      renamedDiffs = new List<(string, string, bool)>();
      deletedDiffs = new List<string>();
      editOnlyDiffs = new List<string>();
      bool flag = false;
      foreach (TfsGitDiffEntry diff in diffs)
      {
        switch (diff.ChangeType)
        {
          case TfsGitChangeType.Add:
            flag = true;
            addedDiffs.Add(diff.RelativePath.GetWikiPathFromGitFilePath(mappedPath));
            continue;
          case TfsGitChangeType.Edit:
            flag = true;
            editOnlyDiffs.Add(diff.RelativePath.GetWikiPathFromGitFilePath(mappedPath));
            continue;
          case TfsGitChangeType.Delete:
            flag = true;
            deletedDiffs.Add(diff.RelativePath.GetWikiPathFromGitFilePath(mappedPath));
            continue;
          case TfsGitChangeType.Rename:
          case TfsGitChangeType.Edit | TfsGitChangeType.Rename:
            string relativePath = diff.RelativePath;
            string renameSourceItemPath = diff.RenameSourceItemPath;
            if (relativePath.IsNullOrEmpty<char>() || renameSourceItemPath.IsNullOrEmpty<char>())
            {
              requestContext.TraceErrorAlways("RelativePath:" + diff.RelativePath + " and RenameSourcePath:" + diff.RenameSourceItemPath + " cannot be null or empty", "Error_InvalidTfsGitDiff", ciData);
              throw new ArgumentNullException("TfsGitDiff's RelativePath and RenameSourceItemPath cannot be null or empty");
            }
            if (PathHelper.IsPathUnder(renameSourceItemPath, mappedPath) && renameSourceItemPath.IsMdFile() && (!PathHelper.IsPathUnder(relativePath, mappedPath) || !relativePath.IsMdFile()))
            {
              flag = true;
              deletedDiffs.Add(diff.RenameSourceItemPath.GetWikiPathFromGitFilePath(mappedPath));
              continue;
            }
            if (PathHelper.IsPathUnder(relativePath, mappedPath) && relativePath.IsMdFile() && (!PathHelper.IsPathUnder(renameSourceItemPath, mappedPath) || !renameSourceItemPath.IsMdFile()))
            {
              flag = true;
              addedDiffs.Add(diff.RelativePath.GetWikiPathFromGitFilePath(mappedPath));
              continue;
            }
            if (PathHelper.IsPathUnder(relativePath, mappedPath) && relativePath.IsMdFile())
            {
              flag = true;
              renamedDiffs.Add((diff.RenameSourceItemPath.GetWikiPathFromGitFilePath(mappedPath), diff.RelativePath.GetWikiPathFromGitFilePath(mappedPath), diff.ChangeType == (TfsGitChangeType.Edit | TfsGitChangeType.Rename)));
              continue;
            }
            continue;
          default:
            continue;
        }
      }
      return flag;
    }

    internal static IEnumerable<TfsGitDiffEntry> GetMdFileDiff(IEnumerable<TfsGitDiffEntry> allDiffs)
    {
      allDiffs = allDiffs ?? (IEnumerable<TfsGitDiffEntry>) new List<TfsGitDiffEntry>(0);
      return allDiffs.Where<TfsGitDiffEntry>((Func<TfsGitDiffEntry, bool>) (d =>
      {
        if (d.OldObjectType != GitObjectType.Blob && d.NewObjectType != GitObjectType.Blob)
          return false;
        return d.RelativePath.IsMdFile() || d.RenameSourceItemPath.IsMdFile();
      }));
    }

    private static IList<TfsGitDiffEntry> GetAllFileDiff(
      ITfsGitRepository repo,
      TfsGitCommit baseCommit,
      TfsGitCommit targetCommit,
      TimedCiEvent ciData)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      IList<TfsGitDiffEntry> allFileDiff = TfsGitDiffHelper.DiffTrees(repo, baseCommit.GetTree(), targetCommit.GetTree(), true);
      stopwatch.Stop();
      ciData.Properties.AddOrIncrement("PushDiffTimeTicks", stopwatch.ElapsedTicks);
      ciData.Properties.AddOrIncrement("PushDiffTimeCount", 1L);
      return allFileDiff;
    }

    internal static IEnumerable<TfsGitDiffEntry> GetDiffsInMappedPath(
      IEnumerable<TfsGitDiffEntry> diffs,
      string mappedPath)
    {
      return diffs.Where<TfsGitDiffEntry>((Func<TfsGitDiffEntry, bool>) (d => PathHelper.IsPathUnder(d.RelativePath, mappedPath) || PathHelper.IsPathUnder(d.RenameSourceItemPath, mappedPath)));
    }

    internal static bool ProcessPushForWikiVersionIfLatest(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      ITfsGitRepository repo,
      GitVersionDescriptor wikiVersion,
      TimedCiEvent ciData,
      WikiPushWaterMarkProvider waterMarkProvider,
      WikiPageIdProvider pageIdProvider,
      int interestedPushId,
      int numPushesToConsider,
      out List<WikiPageWithId> addedPagesInInterestPush,
      out List<WikiPageWithId> renamedPagesInInterestPush,
      int tracePoint,
      string traceLayer)
    {
      int? wikiPushWaterMark = waterMarkProvider.GetWikiPushWaterMark(requestContext, wiki, wikiVersion, ciData);
      addedPagesInInterestPush = new List<WikiPageWithId>();
      renamedPagesInInterestPush = new List<WikiPageWithId>();
      if (!wikiPushWaterMark.HasValue)
      {
        requestContext.TraceErrorAlways(string.Format("No watermark present for wiki:{0}, version:{1}, this can happen when the wiki version publish has not processed page Id yet", (object) wiki.Id, (object) wikiVersion), "Error_WaterMarkAbsent", ciData);
        return false;
      }
      if (wikiPushWaterMark.Value >= interestedPushId)
      {
        ciData.Properties.AddOrIncrement("PushAlreadyProcessed", 1L);
        return true;
      }
      List<TfsGitRefLogEntry> gitRefLogEntries = WikiPageIdHelper.GetNextGitRefLogEntries(requestContext, wiki, repo, wikiVersion, ciData, wikiPushWaterMark);
      if (gitRefLogEntries.Last<TfsGitRefLogEntry>().PushId < interestedPushId)
      {
        ciData.Properties.AddOrIncrement("TooManyPushesPending", 1L);
        return false;
      }
      string mappedPath = wiki.MappedPath;
      try
      {
        foreach (TfsGitRefLogEntry push in gitRefLogEntries)
        {
          List<WikiPageWithId> addedPages;
          List<WikiPageWithId> renamedPages;
          if (!WikiPageIdHelper.ProcessPushForWikiVersion(requestContext, repo, wiki, wikiVersion, ciData, pageIdProvider, push, mappedPath, out addedPages, out renamedPages, out List<WikiPageWithId> _))
            return false;
          if (push.PushId == interestedPushId)
          {
            addedPagesInInterestPush = addedPages;
            renamedPagesInInterestPush = renamedPages;
            break;
          }
        }
      }
      catch (PushNotLatestException ex)
      {
        ciData.Properties.AddOrIncrement("PushAlreadyProcessed", 1L);
        requestContext.TraceException((Exception) ex, tracePoint, traceLayer);
      }
      return true;
    }

    private static List<TfsGitRefLogEntry> GetNextGitRefLogEntries(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      ITfsGitRepository repo,
      GitVersionDescriptor wikiVersion,
      TimedCiEvent ciData,
      int? lastProcessedPushId)
    {
      List<TfsGitRefLogEntry> nextRefLogEntries;
      using (new StopWatchHelper(ciData, "GetNextRefLogEntries"))
      {
        nextRefLogEntries = repo.GetNextRefLogEntries(requestContext, string.Format("refs/heads/" + wikiVersion.Version), lastProcessedPushId, new int?(requestContext.GetMaxPushToBeProcessedPerRun()));
        requestContext.TraceAlways(15250700, TraceLevel.Info, "Wiki", "Jobs", string.Format("Num pushes to be processed in this run for wiki:{0}, version:{1} is #pushes:{2}", (object) wiki.Id, (object) wikiVersion, (object) nextRefLogEntries.Count));
      }
      return nextRefLogEntries;
    }
  }
}
