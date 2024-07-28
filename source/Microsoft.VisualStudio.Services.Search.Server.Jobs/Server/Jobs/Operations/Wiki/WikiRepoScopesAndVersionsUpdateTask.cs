// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.WikiRepoScopesAndVersionsUpdateTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Wiki;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class WikiRepoScopesAndVersionsUpdateTask
  {
    [StaticSafe("Grandfathered")]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080718, "Indexing Pipeline", "IndexingOperation");
    private readonly WikiRepoSyncAnalyzer m_wikiRepoSyncAnalyzer;
    private readonly IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private readonly IIndexingUnitWikisDataAccess m_indexingUnitWikisDataAccess;
    private readonly Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent m_indexingUnitChangeEvent;
    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnit m_indexingUnit;

    public WikiRepoScopesAndVersionsUpdateTask(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      IIndexingUnitWikisDataAccess indexingUnitWikisDataAccess,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      WikiRepoSyncAnalyzer wikiRepoSyncAnalyzer)
    {
      this.m_indexingUnit = indexingUnit;
      this.m_indexingUnitDataAccess = indexingUnitDataAccess;
      this.m_indexingUnitWikisDataAccess = indexingUnitWikisDataAccess;
      this.m_indexingUnitChangeEvent = indexingUnitChangeEvent;
      this.m_wikiRepoSyncAnalyzer = wikiRepoSyncAnalyzer;
    }

    public string HandleScopeAndVersionsUpdates(IndexingExecutionContext indexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(WikiRepoScopesAndVersionsUpdateTask.s_traceMetadata, nameof (HandleScopeAndVersionsUpdates));
      StringBuilder resultMessage = new StringBuilder();
      try
      {
        IndexingUnitWikisEntry indexingUnitWikisEntry = this.m_indexingUnitWikisDataAccess.GetIndexingUnitWikisEntry(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit.IndexingUnitId);
        if (indexingUnitWikisEntry != null)
        {
          IEnumerable<WikiV2> wikisInTfsRepo = this.m_wikiRepoSyncAnalyzer.GetWikisInTfsRepo(indexingExecutionContext.IndexingUnit.TFSEntityId, indexingExecutionContext.ProjectId.Value);
          int num1 = this.HandleUnpublishedWikisAndUpdateWikisEntry(indexingExecutionContext, indexingUnitWikisEntry, wikisInTfsRepo, resultMessage);
          int num2 = this.HandleWikiVersionChangesAndUpdateWikisEntry(indexingExecutionContext, indexingUnitWikisEntry, wikisInTfsRepo, resultMessage);
          int num3 = this.HandleWikiRenameChangesAndUpdateWikisEntry(indexingUnitWikisEntry, wikisInTfsRepo);
          IEnumerable<WikiV2> wikisAddedInTfs = this.m_wikiRepoSyncAnalyzer.GetWikisAddedInTfs((IEnumerable<WikiV2>) indexingUnitWikisEntry.Wikis, wikisInTfsRepo);
          if (wikisAddedInTfs.Count<WikiV2>() > 0)
            indexingUnitWikisEntry.Wikis.AddRange(wikisAddedInTfs);
          if (wikisAddedInTfs.Count<WikiV2>() > 0 || num2 > 0 || num1 > 0 || num3 > 0)
            this.m_indexingUnitWikisDataAccess.UpdateIndexingUnitWikisEntry(indexingExecutionContext.RequestContext, indexingUnitWikisEntry);
          if (wikisAddedInTfs.Count<WikiV2>() > 0 || num2 > 0 || num3 > 0)
          {
            if (this.m_indexingUnit.EraseIndexingWaterMarks())
              this.m_indexingUnit = this.m_indexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, this.m_indexingUnit);
            this.m_wikiRepoSyncAnalyzer.QueueGitRepoBIOperation((ExecutionContext) indexingExecutionContext, this.m_indexingUnit, new HashSet<string>(), this.m_indexingUnitChangeEvent.LeaseId);
            resultMessage.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Queued BI operation after identifying {0} new wikis, {1} wiki version and {2} renamed wiki updates for wiki IndexingUnit [{3}]", (object) wikisAddedInTfs.Count<WikiV2>(), (object) num2, (object) num3, (object) this.m_indexingUnit.ToString())));
          }
        }
        else
        {
          resultMessage.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Found no entry in IndexingUnitWikis for IndexingUnit [{0}]", (object) this.m_indexingUnit.ToString())));
          this.UpgradeForOldProjectWikis(indexingExecutionContext, resultMessage);
        }
        if (string.IsNullOrWhiteSpace(resultMessage.ToString()))
          resultMessage.AppendLine("Processed wiki scopes and versions update successfully and found no changes.");
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(WikiRepoScopesAndVersionsUpdateTask.s_traceMetadata, nameof (HandleScopeAndVersionsUpdates));
      }
      return resultMessage.ToString();
    }

    internal int HandleUnpublishedWikisAndUpdateWikisEntry(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnitWikisEntry wikisEntry,
      IEnumerable<WikiV2> wikisInTfs,
      StringBuilder resultMessage)
    {
      List<WikiV2> list = this.m_wikiRepoSyncAnalyzer.GetWikisRemovedInTfs((IEnumerable<WikiV2>) wikisEntry.Wikis, wikisInTfs).ToList<WikiV2>();
      int num = list.Count<WikiV2>();
      if (num > 0)
      {
        foreach (WikiV2 wikiV2 in list)
          wikisEntry.Wikis.Remove(wikiV2);
        resultMessage.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Queueing WikiDelete for {0} wikis which have been unpublished for IndexingUnit[{1}]", (object) num, (object) this.m_indexingUnit.ToString())));
        this.DeleteWikiDocuments(indexingExecutionContext, list, resultMessage);
      }
      return num;
    }

    internal virtual void DeleteWikiDocuments(
      IndexingExecutionContext indexingExecutionContext,
      List<WikiV2> wikisToBeDeleted,
      StringBuilder resultMessage)
    {
      List<WikiDeleteMetadata> wikiMetadataList = new List<WikiDeleteMetadata>();
      foreach (WikiV2 wikiV2 in wikisToBeDeleted)
      {
        List<WikiDeleteMetadata> wikiDeleteMetadataList = wikiMetadataList;
        Guid guid = indexingExecutionContext.IndexingUnit.TFSEntityId;
        string repositoryId = guid.ToString();
        guid = wikiV2.Id;
        string wikiId = guid.ToString();
        WikiDeleteMetadata wikiDeleteMetadata = new WikiDeleteMetadata(repositoryId, wikiId);
        wikiDeleteMetadataList.Add(wikiDeleteMetadata);
      }
      List<WikiDeleteMetadata> source = this.DeleteWikis(indexingExecutionContext, wikiMetadataList);
      if (source != null && source.Count > 0)
        resultMessage.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("WikiDelete failed for wikis {0}", (object) string.Join(",", source.Select<WikiDeleteMetadata, string>((Func<WikiDeleteMetadata, string>) (x => x.WikiId))))));
      else
        resultMessage.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("WikiDelete succeeded for wiki IndexingUnit [{0}]", (object) this.m_indexingUnit.ToString())));
    }

    internal int HandleWikiVersionChangesAndUpdateWikisEntry(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnitWikisEntry wikisEntry,
      IEnumerable<WikiV2> wikisInTfs,
      StringBuilder resultMessage)
    {
      int num = 0;
      foreach (WikiV2 wikisInTf in wikisInTfs)
      {
        WikiV2 wikiInTfs = wikisInTf;
        IEnumerable<WikiV2> source = wikisEntry.Wikis.Where<WikiV2>((Func<WikiV2, bool>) (w => w.Id == wikiInTfs.Id));
        WikiV2 wikiV2 = source.Any<WikiV2>() ? source.First<WikiV2>() : (WikiV2) null;
        if (wikiV2 != null)
        {
          IEnumerable<GitVersionDescriptor> versions1 = wikiV2.Versions;
          if ((versions1 != null ? (versions1.Count<GitVersionDescriptor>() > 0 ? 1 : 0) : 0) != 0)
          {
            IEnumerable<GitVersionDescriptor> versions2 = wikiInTfs.Versions;
            if ((versions2 != null ? (versions2.Count<GitVersionDescriptor>() > 0 ? 1 : 0) : 0) != 0)
            {
              string versionInSearch = wikiV2.Versions.First<GitVersionDescriptor>().Version;
              if (!wikiInTfs.Versions.Any<GitVersionDescriptor>((Func<GitVersionDescriptor, bool>) (v => v.Version == versionInSearch)))
              {
                resultMessage.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Queueing WikiDelete for IndexingUnit [{0}] as branch [{1}] has been unconfigured", (object) this.m_indexingUnit.ToString(), (object) versionInSearch)));
                IndexingExecutionContext indexingExecutionContext1 = indexingExecutionContext;
                List<WikiV2> wikisToBeDeleted = new List<WikiV2>();
                wikisToBeDeleted.Add(wikiV2);
                StringBuilder resultMessage1 = resultMessage;
                this.DeleteWikiDocuments(indexingExecutionContext1, wikisToBeDeleted, resultMessage1);
                wikiV2.Versions = wikiInTfs.Versions.Take<GitVersionDescriptor>(1);
                ++num;
              }
            }
          }
        }
      }
      return num;
    }

    internal int HandleWikiRenameChangesAndUpdateWikisEntry(
      IndexingUnitWikisEntry wikisEntry,
      IEnumerable<WikiV2> wikisInTfs)
    {
      int num = 0;
      foreach (WikiV2 wikisInTf in wikisInTfs)
      {
        WikiV2 wikiInTfs = wikisInTf;
        IEnumerable<WikiV2> source = wikisEntry.Wikis.Where<WikiV2>((Func<WikiV2, bool>) (w => w.Id == wikiInTfs.Id && !w.Name.Equals(wikiInTfs.Name)));
        WikiV2 wikiV2 = source.Any<WikiV2>() ? source.First<WikiV2>() : (WikiV2) null;
        if (wikiV2 != null)
        {
          wikiV2.Name = wikiInTfs.Name;
          ++num;
        }
      }
      return num;
    }

    internal virtual List<WikiDeleteMetadata> DeleteWikis(
      IndexingExecutionContext indexingExecutionContext,
      List<WikiDeleteMetadata> wikiMetadataList)
    {
      IndexIdentity index = indexingExecutionContext.GetIndex();
      return new WikiDeleter().DeleteWikis(indexingExecutionContext, wikiMetadataList, index);
    }

    private void UpgradeForOldProjectWikis(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      IEnumerable<WikiV2> wikisAddedInTfs = this.m_wikiRepoSyncAnalyzer.GetWikisAddedInTfs(Enumerable.Empty<WikiV2>(), this.m_wikiRepoSyncAnalyzer.GetWikisInTfsRepo(indexingExecutionContext.IndexingUnit.TFSEntityId, indexingExecutionContext.ProjectId.Value));
      if (wikisAddedInTfs.Count<WikiV2>() != 1 || wikisAddedInTfs.First<WikiV2>().Type != WikiType.ProjectWiki || string.IsNullOrWhiteSpace(indexingExecutionContext.RepositoryIndexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes ? entityAttributes.DefaultBranch : (string) null))
        return;
      IndexingUnitWikisEntry indexingUnitWikisEntry1 = new IndexingUnitWikisEntry()
      {
        IndexingUnitId = indexingExecutionContext.IndexingUnit.IndexingUnitId
      };
      IndexingUnitWikisEntry indexingUnitWikisEntry2 = indexingUnitWikisEntry1;
      List<WikiV2> wikiV2List = new List<WikiV2>();
      WikiV2 wikiV2 = new WikiV2();
      wikiV2.Id = indexingExecutionContext.IndexingUnit.TFSEntityId;
      wikiV2.Name = indexingExecutionContext.RepositoryName;
      wikiV2.MappedPath = "/";
      wikiV2.Type = WikiType.ProjectWiki;
      wikiV2.Versions = (IEnumerable<GitVersionDescriptor>) new List<GitVersionDescriptor>()
      {
        new GitVersionDescriptor()
        {
          VersionType = GitVersionType.Branch,
          Version = entityAttributes.DefaultBranch.Substring("refs/heads/".Length)
        }
      };
      wikiV2List.Add(wikiV2);
      indexingUnitWikisEntry2.Wikis = wikiV2List;
      this.m_indexingUnitWikisDataAccess.AddIndexingUnitWikisEntry(indexingExecutionContext.RequestContext, indexingUnitWikisEntry1);
      resultMessage.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Added entry in IndexingUnitWikis for IndexingUnit [{0}]", (object) this.m_indexingUnit.ToString())));
    }
  }
}
