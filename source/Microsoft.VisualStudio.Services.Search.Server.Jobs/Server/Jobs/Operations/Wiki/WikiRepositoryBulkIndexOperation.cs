// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.WikiRepositoryBulkIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class WikiRepositoryBulkIndexOperation : GitRepositoryCodeIndexOperation
  {
    private IIndexingUnitWikisDataAccess m_indexingUnitWikisDataAccess;

    public WikiRepositoryBulkIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080719, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      try
      {
        if (!coreIndexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.Wiki.ProductDocumentationSearch"))
        {
          IndexingUnitWikisEntry indexingUnitWikisEntry = this.IndexingUnitWikisDataAccess.GetIndexingUnitWikisEntry(coreIndexingExecutionContext.RequestContext, this.IndexingUnit.IndexingUnitId);
          if (indexingUnitWikisEntry != null && indexingUnitWikisEntry.Wikis.Exists((Predicate<WikiV2>) (wiki => wiki.Type == WikiType.CodeWiki)))
          {
            operationResult.Status = OperationStatus.Succeeded;
            operationResult.Message = FormattableString.Invariant(FormattableStringFactory.Create("Product documentation search FF is off, so not processing the product doc wiki for Indexing Unit {0}", (object) this.IndexingUnit.TFSEntityId));
            return operationResult;
          }
        }
        return base.RunOperation(coreIndexingExecutionContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080719, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      }
    }

    protected internal virtual IIndexingUnitWikisDataAccess IndexingUnitWikisDataAccess
    {
      get
      {
        if (this.m_indexingUnitWikisDataAccess == null)
          this.m_indexingUnitWikisDataAccess = this.DataAccessFactory.GetIndexingUnitWikisDataAccess();
        return this.m_indexingUnitWikisDataAccess;
      }
      set => this.m_indexingUnitWikisDataAccess = value;
    }

    internal override CodeCrawlSpec CreateCrawlSpec(
      IndexingExecutionContext iexContext,
      ref string branchName,
      in List<string> branches)
    {
      IVssRequestContext requestContext = iexContext.RequestContext;
      requestContext.IsFeatureEnabled("Search.Server.Wiki.ProductDocumentationSearch");
      GitCodeRepoTFSAttributes entityAttributes = iexContext.RepositoryIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
      if (iexContext.IndexingUnit.IndexingUnitType == "Git_Repository")
      {
        if (!string.IsNullOrWhiteSpace(branchName) && branches != null && branches.Count > 0)
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Both BranchName and Branches(list) fields are populated in crawlspec")));
        if (!string.IsNullOrWhiteSpace(branchName))
        {
          branches.Clear();
          branches.Add(branchName);
          branchName = (string) null;
        }
        IndexingUnitWikisEntry indexingUnitWikisEntry = this.IndexingUnitWikisDataAccess.GetIndexingUnitWikisEntry(requestContext, iexContext.IndexingUnit.IndexingUnitId);
        List<WikiV2> wikisInRepo = new List<WikiV2>();
        if (indexingUnitWikisEntry == null)
        {
          if (branches.Count > 0)
          {
            List<WikiV2> wikiV2List = new List<WikiV2>();
            WikiV2 wikiV2 = new WikiV2();
            wikiV2.Id = iexContext.RepositoryIndexingUnit.TFSEntityId;
            wikiV2.Name = iexContext.RepositoryName;
            wikiV2.MappedPath = "/";
            wikiV2.Type = WikiType.ProjectWiki;
            wikiV2.Versions = (IEnumerable<GitVersionDescriptor>) new List<GitVersionDescriptor>()
            {
              new GitVersionDescriptor()
              {
                VersionType = GitVersionType.Branch,
                Version = branches[0].Substring("refs/heads/".Length)
              }
            };
            wikiV2List.Add(wikiV2);
            wikisInRepo = wikiV2List;
          }
        }
        else
          wikisInRepo = indexingUnitWikisEntry.Wikis;
        Dictionary<string, List<string>> pipelineBranches = this.GetBranchesToScopePathsAndUpdatePipelineBranches(iexContext, in branches, (IEnumerable<WikiV2>) wikisInRepo);
        return (CodeCrawlSpec) WikiCrawlSpec.Create(iexContext, entityAttributes, iexContext.RepositoryIndexingUnit.TFSEntityId, string.Empty, RepositoryConstants.BranchCreationOrDeletionCommitId, branches, pipelineBranches, wikisInRepo);
      }
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported IndexingUnitType {0} for {1}.", (object) iexContext.IndexingUnit.IndexingUnitType, (object) nameof (WikiRepositoryBulkIndexOperation))));
    }

    internal Dictionary<string, List<string>> GetBranchesToScopePathsAndUpdatePipelineBranches(
      IndexingExecutionContext iexContext,
      in List<string> branches,
      IEnumerable<WikiV2> wikisInRepo)
    {
      branches.Clear();
      Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
      if (wikisInRepo != null)
      {
        foreach (WikiV2 wikiV2 in wikisInRepo)
        {
          string mappedPath = wikiV2.MappedPath;
          foreach (GitVersionDescriptor version in wikiV2.Versions)
          {
            if (version.VersionType == GitVersionType.Branch)
            {
              string key = "refs/heads/" + version.Version;
              List<string> stringList;
              if (!dictionary.TryGetValue(key, out stringList))
              {
                stringList = new List<string>();
                dictionary[key] = stringList;
                branches.Add(key);
              }
              stringList.Add(mappedPath);
            }
          }
        }
      }
      GitHttpClientWrapper httpClientWrapper = this.GetGitHttpClientWrapper(iexContext);
      Dictionary<string, List<string>> pipelineBranches = new Dictionary<string, List<string>>();
      int currentHostConfigValue = iexContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/TFSGetMetaDataMaxThresholdForDocs", true, 10000);
      foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary)
      {
        string key = keyValuePair.Key;
        List<string> filePaths = keyValuePair.Value;
        try
        {
          GitCommit latestCommit = httpClientWrapper.GetLatestCommit(key);
          GitVersionDescriptor gitVersionDescriptor = new GitVersionDescriptor()
          {
            Version = latestCommit.CommitId,
            VersionType = GitVersionType.Commit
          };
          if (filePaths.Count > 0)
          {
            List<string> list = this.GetFilePathsToGitItems(httpClientWrapper, latestCommit, gitVersionDescriptor, filePaths, currentHostConfigValue).Where<KeyValuePair<string, GitItem>>((Func<KeyValuePair<string, GitItem>, bool>) (x => x.Value != null)).Select<KeyValuePair<string, GitItem>, string>((Func<KeyValuePair<string, GitItem>, string>) (x => x.Key)).ToList<string>();
            if (list.Count > 0)
              pipelineBranches[key] = list;
            else
              pipelineBranches[key] = new List<string>()
              {
                "/"
              };
          }
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Exception caught for the branch {0}, Exception : {1}", (object) key, (object) ex.ToString())));
          pipelineBranches[key] = filePaths;
        }
      }
      return pipelineBranches;
    }

    internal virtual IDictionary<string, GitItem> GetFilePathsToGitItems(
      GitHttpClientWrapper clientWrapper,
      GitCommit gitCommit,
      GitVersionDescriptor gitVersionDescriptor,
      List<string> filePaths,
      int maxBatchSize)
    {
      IDictionary<string, GitItem> filePathsToGitItems = (IDictionary<string, GitItem>) new Dictionary<string, GitItem>();
      int count = filePaths.Count;
      foreach (IEnumerable<string> source in filePaths.Batch<string>(maxBatchSize))
      {
        List<GitItemDescriptor> gitItemDescriptorList = new List<GitItemDescriptor>(source.Count<string>());
        foreach (string str in source)
        {
          GitItemDescriptor gitItemDescriptor = new GitItemDescriptor()
          {
            Path = str,
            RecursionLevel = VersionControlRecursionType.None,
            VersionType = GitVersionType.Commit,
            Version = gitCommit.CommitId
          };
          gitItemDescriptorList.Add(gitItemDescriptor);
        }
        GitItemRequestData gitItemRequestData = new GitItemRequestData()
        {
          ItemDescriptors = gitItemDescriptorList.ToArray(),
          IncludeContentMetadata = false,
          IncludeLinks = false,
          LatestProcessedChange = true
        };
        try
        {
          List<List<GitItem>> itemsBatch = clientWrapper.GetItemsBatch(gitItemRequestData);
          if (itemsBatch.Count != source.Count<string>())
            throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Expected number of records from GetItemsBatch : {0}, Found {1} records.", (object) source.Count<string>(), (object) itemsBatch.Count)));
          for (int index = 0; index < source.Count<string>(); ++index)
          {
            string path = gitItemDescriptorList[index].Path;
            if (itemsBatch[index] != null && itemsBatch[index].Count > 0)
            {
              foreach (GitItem gitItem in itemsBatch[index])
                filePathsToGitItems[path] = gitItem;
            }
            else
              filePathsToGitItems[path] = (GitItem) null;
          }
        }
        catch (Exception ex1)
        {
          this.ThrowIfShouldNotContinue(ex1);
          foreach (string str in source)
          {
            if (!filePathsToGitItems.ContainsKey(str))
            {
              try
              {
                filePathsToGitItems[str] = clientWrapper.GetGitItem(gitCommit, gitVersionDescriptor, str);
              }
              catch (Exception ex2)
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Exception caught in GetItem invocation for file {0}, Exception : {1}", (object) str, (object) ex2.ToString())));
                filePathsToGitItems[str] = (GitItem) null;
              }
            }
          }
        }
      }
      return filePathsToGitItems;
    }

    internal virtual void ThrowIfShouldNotContinue(Exception ex)
    {
      if (!IndexFaultMapManager.GetFaultMapper(typeof (VssThrottlingFaultMapper)).IsMatch(ex) && !IndexFaultMapManager.GetFaultMapper(typeof (ProjectNotFoundFaultMapper)).IsMatch(ex) && !IndexFaultMapManager.GetFaultMapper(typeof (RepoDoesNotExistFaultMapper)).IsMatch(ex) && !IndexFaultMapManager.GetFaultMapper(typeof (TfsBranchNotFoundFaultMapper)).IsMatch(ex))
        return;
      ExceptionDispatchInfo.Capture(ex).Throw();
    }

    internal virtual GitHttpClientWrapper GetGitHttpClientWrapper(
      IndexingExecutionContext indexingExecutionContext)
    {
      IndexingExecutionContext executionContext = indexingExecutionContext;
      Guid? nullable = indexingExecutionContext.ProjectId;
      Guid projectId = nullable.Value;
      nullable = indexingExecutionContext.RepositoryId;
      Guid repositoryId = nullable.Value;
      TraceMetaData traceMetaData = this.TraceMetaData;
      return new GitHttpClientWrapper((ExecutionContext) executionContext, projectId, repositoryId, traceMetaData);
    }
  }
}
