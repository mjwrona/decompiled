// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.GitRepoSyncAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.Exceptions;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.RepoSync;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class GitRepoSyncAnalyzer
  {
    internal IDataAccessFactory m_dataAccessFactory;
    private IIndexingUnitDataAccess m_indexingUnitDataAccess;
    private IIndexingUnitChangeEventDataAccess m_indexingUnitChangeEventDataAccess;
    private IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;
    private IndexMetadataStateAnalyser m_indexMetadataStateAnalyser;
    private RepositoryPushNotificationValidator m_repoPushNotificationValidator;

    protected GitRepoSyncAnalyzer(
      ExecutionContext executionContext,
      TraceMetaData traceMetaData,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : this(executionContext, traceMetaData, (IDataAccessFactory) null, indexingUnitChangeEventHandler, (IndexMetadataStateAnalyser) null)
    {
    }

    internal GitRepoSyncAnalyzer(
      ExecutionContext executionContext,
      TraceMetaData traceMetaData,
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      IndexMetadataStateAnalyser indexMetadataStateAnalyser)
    {
      this.ExecutionContext = executionContext;
      this.RequestContext = executionContext.RequestContext;
      this.TraceMetadata = traceMetaData;
      this.m_dataAccessFactory = dataAccessFactory ?? DataAccessFactory.GetInstance();
      this.m_indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      this.m_indexingUnitChangeEventDataAccess = this.m_dataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      this.m_indexingUnitChangeEventHandler = indexingUnitChangeEventHandler ?? (IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler();
      this.m_indexMetadataStateAnalyser = indexMetadataStateAnalyser;
    }

    internal abstract EntityFinalizerBase FinalizeHelper { get; }

    protected abstract IEntityType EntityType { get; }

    internal virtual IndexMetadataStateAnalyser IndexMetadataStateAnalyser
    {
      get
      {
        if (this.m_indexMetadataStateAnalyser == null)
          this.m_indexMetadataStateAnalyser = new IndexMetadataStateAnalyserFactory().GetIndexMetadataStateAnalyser(this.m_dataAccessFactory, this.m_indexingUnitChangeEventHandler, this.EntityType);
        return this.m_indexMetadataStateAnalyser;
      }
    }

    public virtual int SyncReposSecHashInCollection()
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetadata, nameof (SyncReposSecHashInCollection));
      try
      {
        int num = 0;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> filteredIndexingUnits = this.GetFilteredIndexingUnits(new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Failed
        });
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.m_indexingUnitDataAccess.GetIndexingUnits(this.RequestContext, "Project", this.EntityType, -1);
        FriendlyDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> friendlyDictionary = new FriendlyDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits)
          friendlyDictionary.Add(indexingUnit.IndexingUnitId, indexingUnit);
        IEnumerable<GitRepository> tfsRepos = (IEnumerable<GitRepository>) this.GetTfsRepos();
        if (tfsRepos != null)
        {
          IEnumerable<Guid> tfsRepoIdList = tfsRepos.Select<GitRepository, Guid>((Func<GitRepository, Guid>) (repo => repo.Id));
          filteredIndexingUnits.RemoveAll((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo => !tfsRepoIdList.Contains<Guid>(repo.TFSEntityId)));
        }
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnit1 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        RepositorySecurityAcesUpdater securityAcesUpdater = this.RepoSecAcesUpdater ?? new RepositorySecurityAcesUpdater(this.ExecutionContext);
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 in filteredIndexingUnits)
        {
          TFSEntityAttributes entityAttributes = indexingUnit2.TFSEntityAttributes;
          GitCodeRepoIndexingProperties properties = indexingUnit2.Properties as GitCodeRepoIndexingProperties;
          byte[] currentTfsSecHash = (byte[]) null;
          try
          {
            currentTfsSecHash = securityAcesUpdater.GetSecurityHashCodeForRepository(indexingUnit2.TFSEntityId, friendlyDictionary[indexingUnit2.ParentUnitId].TFSEntityId);
          }
          catch (Exception ex)
          {
            if (!this.ExecutionContext.FaultService.ShouldRetryOnError(ex))
            {
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Non retriable exception in security Hash computation of repo Id {0}\r\n                                             aborting security hash computation of remaining repos. Below are the Exception details: \\n {1}", (object) indexingUnit2.TFSEntityId, (object) ex)));
              break;
            }
          }
          if (SecurityChecksUtils.ShouldUpdateHash(properties.SecurityHashcode, currentTfsSecHash))
          {
            properties.SecurityHashcode = currentTfsSecHash;
            indexingUnit1.Add(indexingUnit2);
            ++num;
          }
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ProcessReposSecHashInCollection - Security Hash update for IndexingUnitChangeEvent Id = {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RepositoryId = {0}", (object) indexingUnit2.TFSEntityId.ToString())));
        }
        if (indexingUnit1.Count > 0)
          this.m_indexingUnitDataAccess.UpdateIndexingUnits(this.RequestContext, indexingUnit1);
        return num;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetadata, nameof (SyncReposSecHashInCollection));
      }
    }

    public virtual int SyncDefaultBranchChangeAndDeletedReposInCollection(
      StringBuilder resultMessageBuilder)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetadata, nameof (SyncDefaultBranchChangeAndDeletedReposInCollection));
      try
      {
        int num = 0;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> allIndexedGitRepos = this.GetAllIndexedGitRepos();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Found {0} indexed git Repo indexing units.", (object) allIndexedGitRepos.Count)));
        List<GitRepository> tfsRepos = this.GetTfsRepos();
        if (tfsRepos == null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, "No git repositories found while syncing DefaultBranch and Deleted and Renamed repos in collection.");
          return 0;
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Found {0} Repos in TFS.", (object) tfsRepos.Count)));
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in allIndexedGitRepos)
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexedGitRepo = indexingUnit;
          GitRepository gitRepoInTFS = tfsRepos.Find((Predicate<GitRepository>) (repo => repo.Id == indexedGitRepo.TFSEntityId));
          if (gitRepoInTFS != null)
          {
            if (!indexedGitRepo.Properties.IsDisabled)
            {
              GitCodeRepoTFSAttributes entityAttributes = indexedGitRepo.TFSEntityAttributes as GitCodeRepoTFSAttributes;
              bool flag = false;
              string str = string.IsNullOrWhiteSpace(gitRepoInTFS.DefaultBranch) ? string.Empty : gitRepoInTFS.DefaultBranch;
              List<string> source = new List<string>();
              string defaultBranch1 = entityAttributes.DefaultBranch;
              if (str != defaultBranch1)
              {
                string defaultBranch2 = entityAttributes.DefaultBranch;
                resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Identified default branch changed in RepositoryId = {0} from Branch = '{1}' to Branch = '{2}' for IndexingUnitId = {3} and isShadow = {4}", (object) indexedGitRepo.TFSEntityId, (object) defaultBranch2, (object) gitRepoInTFS.DefaultBranch, (object) indexedGitRepo.IndexingUnitId, (object) indexedGitRepo.IsShadow);
                flag = true;
                List<string> defaultBranchChange = this.GetDefaultBranchChange(indexedGitRepo, gitRepoInTFS, entityAttributes);
                if (defaultBranchChange != null && defaultBranchChange.Count > 0)
                  source.AddRange((IEnumerable<string>) defaultBranchChange);
              }
              if (this.EntityType.Name == "Code" && this.IsMultiBranchConfigurationChanged(indexedGitRepo, gitRepoInTFS))
              {
                resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Identified multi branch updates in  RepositoryId = {0} for IndexingUnitId = {1} and isShadow = {2}. ", (object) indexedGitRepo.TFSEntityId, (object) indexedGitRepo.IndexingUnitId, (object) indexedGitRepo.IsShadow);
                flag = true;
                List<string> configuredBranches = this.GetNewlyConfiguredBranches(indexedGitRepo, gitRepoInTFS);
                if (configuredBranches != null && configuredBranches.Count > 0)
                  source.AddRange((IEnumerable<string>) configuredBranches);
              }
              if (flag)
              {
                List<string> list = source.Distinct<string>().ToList<string>();
                ++num;
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created Repo Patch event [{0}] for IndexingUnit [{1}]", (object) this.HandleBranchConfigurationChanges(this.ExecutionContext, indexedGitRepo, list).ToString(), (object) indexedGitRepo.ToString()))));
              }
              if (gitRepoInTFS.Name != entityAttributes.RepositoryName || entityAttributes.RepositoryName != indexedGitRepo.Properties?.Name)
              {
                ++num;
                resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Identified Name of the Repository with RepositoryId = {0} changed from {1} to {2} for IndexingUnitId = {3} and isShadow = {4}.", (object) indexedGitRepo.TFSEntityId, (object) entityAttributes.RepositoryName, (object) gitRepoInTFS.Name, (object) indexedGitRepo.IndexingUnitId, (object) indexedGitRepo.IsShadow);
                if (this.ExecutionContext.RequestContext.IsContinuousIndexingEnabled(indexedGitRepo.EntityType))
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created Repo Rename Operation {0} for IndexingUnit {1}", (object) this.CreateRepoRenameEventForGitRepo(indexedGitRepo, gitRepoInTFS.Name).ToString(), (object) indexedGitRepo.ToString()))));
              }
            }
          }
          else
          {
            ++num;
            resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Identified deletion of RepositoryId = {0} for IndexingUnitId = {1} and isShadow = {2}.", (object) indexedGitRepo.TFSEntityId, (object) indexedGitRepo.IndexingUnitId, (object) indexedGitRepo.IsShadow);
            if (this.ExecutionContext.RequestContext.IsContinuousIndexingEnabled(indexedGitRepo.EntityType))
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Created {0} IndexingUnitChangeEvents to delete IndexingUnit {1}", (object) this.IndexMetadataStateAnalyser.CreateEntityDeleteOperationIfRequired(this.ExecutionContext, indexedGitRepo), (object) indexedGitRepo.ToString()))));
          }
        }
        return num;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetadata, nameof (SyncDefaultBranchChangeAndDeletedReposInCollection));
      }
    }

    public virtual int SyncMissedCommitsInCollection(
      IndexingExecutionContext indexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetadata, nameof (SyncMissedCommitsInCollection));
      try
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> allIndexedGitRepos = this.GetAllIndexedGitRepos();
        int num = 0;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.m_indexingUnitDataAccess.GetIndexingUnits(this.RequestContext, "Project", this.EntityType, -1);
        Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary = new Dictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits)
          dictionary.Add(indexingUnit.IndexingUnitId, indexingUnit);
        RepositoryPushNotificationValidator pushNotificationValidator = this.m_repoPushNotificationValidator ?? new RepositoryPushNotificationValidator(this.ExecutionContext);
        List<GitRepository> tfsRepos = this.GetTfsRepos();
        if (tfsRepos == null)
          throw new GitRepoSyncAnalyzerException("Get repositories from TFS failed while syncing missed commits in the collection.");
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in allIndexedGitRepos)
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexedGitRepo = indexingUnit1;
          if (!indexedGitRepo.Properties.IsDisabled)
          {
            GitRepository gitRepository = tfsRepos.Find((Predicate<GitRepository>) (repo => repo.Id == indexedGitRepo.TFSEntityId));
            if (gitRepository != null)
            {
              bool? isDisabled = gitRepository.IsDisabled;
              if (isDisabled.HasValue)
              {
                isDisabled = gitRepository.IsDisabled;
                if (isDisabled.Value)
                {
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Skipping commit sync for the disabled repo id: {0}", (object) gitRepository.Id))));
                  continue;
                }
              }
            }
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
            dictionary.TryGetValue(indexedGitRepo.ParentUnitId, out indexingUnit2);
            if (indexingUnit2 != null)
            {
              if (gitRepository != null && this.ProcessNonIndexedCommitsInGitRepo(indexingExecutionContext, indexedGitRepo, pushNotificationValidator, indexingUnit2.TFSEntityId))
                ++num;
            }
            else
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Project indexing unit for git repo with indexing unit id: {0} is not found.", (object) indexedGitRepo.IndexingUnitId));
          }
        }
        return num;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetadata, nameof (SyncMissedCommitsInCollection));
      }
    }

    public virtual int SyncMissingGitReposInCollection()
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetadata, nameof (SyncMissingGitReposInCollection));
      try
      {
        int numberOfReposProcessed = 0;
        List<GitRepository> tfsRepos = this.GetTfsRepos();
        if (tfsRepos == null)
          throw new GitRepoSyncAnalyzerException("Get repositories from TFS failed while syncing new repositories in the collection.");
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.m_indexingUnitDataAccess.GetIndexingUnits(this.RequestContext, "Git_Repository", this.EntityType, -1);
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> shadowIndexedRepos = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        if (this.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() && this.ExecutionContext.IsReindexingFailedOrInProgress(this.m_dataAccessFactory, this.EntityType))
          shadowIndexedRepos = this.m_indexingUnitDataAccess.GetIndexingUnits(this.RequestContext, "Git_Repository", true, this.EntityType, -1);
        Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> guidToIndexingUnit = new Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> guidToShadowIndexingUnit = new Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> action = (Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (indexedRepo => guidToIndexingUnit.Add(indexedRepo.TFSEntityId, indexedRepo));
        indexingUnits.ForEach(action);
        shadowIndexedRepos.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (shadowIndexedRepo => guidToShadowIndexingUnit.Add(shadowIndexedRepo.TFSEntityId, shadowIndexedRepo)));
        tfsRepos.ForEach((Action<GitRepository>) (tfsRepo =>
        {
          numberOfReposProcessed += this.SyncMissingGitRepo(tfsRepo, guidToIndexingUnit);
          if (shadowIndexedRepos.Count <= 0)
            return;
          numberOfReposProcessed += this.SyncMissingGitRepo(tfsRepo, guidToShadowIndexingUnit, true);
        }));
        return numberOfReposProcessed;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetadata, nameof (SyncMissingGitReposInCollection));
      }
    }

    private int SyncMissingGitRepo(
      GitRepository tfsRepo,
      Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> guidToIndexingUnit,
      bool isShadow = false)
    {
      int num1 = 0;
      if (tfsRepo.IsDisabled.HasValue && tfsRepo.IsDisabled.Value)
        return num1;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit) null;
      if (guidToIndexingUnit.ContainsKey(tfsRepo.Id))
        indexingUnit = guidToIndexingUnit[tfsRepo.Id];
      int documentCount;
      if (indexingUnit == null)
      {
        string sourceForTraceInfo = "SyncMissingGitReposInCollection";
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit;
        bool repoIndexingUnitCreated;
        if (this.CreateRepositoryIndexingUnitIfNeeded(this.ExecutionContext, tfsRepo, new Func<ExecutionContext, GitRepository, ProjectHttpClientWrapper, bool>(this.IsDisabled), sourceForTraceInfo, out projectIndexingUnit, out repoIndexingUnit, out repoIndexingUnitCreated, isShadow) && repoIndexingUnitCreated && !repoIndexingUnit.Properties.IsDisabled && !this.GitHttpClient.IsEmpty(this.ExecutionContext.RequestContext, projectIndexingUnit, repoIndexingUnit, out documentCount))
        {
          int num2 = num1 + 1;
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent assignmentOperation = this.CreateRoutingAssignmentOperation(this.ExecutionContext, repoIndexingUnit);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GitRepoSyncAnalyzer created RoutingAssignment event [Id = {0} IndexingUnitId = {1} isShadow = {2}]", (object) assignmentOperation.Id, (object) assignmentOperation.IndexingUnitId, (object) repoIndexingUnit.IsShadow));
          return num2;
        }
      }
      else if ((indexingUnit.Properties.IndexIndices == null || !indexingUnit.Properties.IndexIndices.Any<IndexInfo>()) && !indexingUnit.Properties.IsDisabled && !this.GitHttpClient.IsEmpty(this.ExecutionContext.RequestContext, this.GetProjectIndexingUnit(this.ExecutionContext, this.m_dataAccessFactory.GetIndexingUnitDataAccess(), tfsRepo.ProjectReference.Id), indexingUnit, out documentCount))
      {
        ++num1;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent assignmentOperation = this.CreateRoutingAssignmentOperation(this.ExecutionContext, indexingUnit);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("GitRepoSyncAnalyzer created RoutingAssignment event [Id = {0} IndexingUnitId = {1} isShadow = {2}]", (object) assignmentOperation.Id, (object) assignmentOperation.IndexingUnitId, (object) indexingUnit.IsShadow)));
      }
      return num1;
    }

    private bool IsDisabled(
      ExecutionContext executionContext,
      GitRepository gitRepository,
      ProjectHttpClientWrapper projectClientWrapper)
    {
      return gitRepository.IsFork;
    }

    public virtual void SyncGitRepositories(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      try
      {
        int num = this.SyncDefaultBranchChangeAndDeletedReposInCollection(resultMessageBuilder);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0} repositories got default branch changed or deleted. ", (object) num);
      }
      catch (Exception ex)
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories default branch change or deletion sync failed with exception. ");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories default branch change or deletion sync failed with exception: {0} ", (object) ex));
      }
      try
      {
        int num = this.SyncMissedCommitsInCollection(indexingExecutionContext);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Processed {0} GIT repositories for missed notification sync. ", (object) num);
      }
      catch (Exception ex)
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories missed notification sync failed with exception. ");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories missed notification sync failed with exception: {0} ", (object) ex));
      }
      try
      {
        int num = this.SyncMissingGitReposInCollection();
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Processed {0} new GIT repositories", (object) num);
      }
      catch (Exception ex)
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Sync for missing git repositories failed with exception. ");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Sync for missing git repositories failed with exception: {0} ", (object) ex));
      }
    }

    internal RepositorySecurityAcesUpdater RepoSecAcesUpdater { private get; set; }

    internal RepositoryPushNotificationValidator RepoPushNotificationValidator
    {
      set => this.m_repoPushNotificationValidator = value;
    }

    public GitHttpClientWrapper GitHttpClient
    {
      get
      {
        if (this.m_gitHttpClientWrapper == null)
          this.m_gitHttpClientWrapper = new GitHttpClientWrapper(this.ExecutionContext, this.TraceMetadata);
        return this.m_gitHttpClientWrapper;
      }
      set => this.m_gitHttpClientWrapper = value;
    }

    internal virtual List<string> GetBranchesToIndex(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit) => (gitRepoIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes).BranchesToIndex;

    internal bool ProcessNonIndexedCommitsInGitRepo(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepo,
      RepositoryPushNotificationValidator pushNotificationValidator,
      Guid projectId)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetadata, nameof (ProcessNonIndexedCommitsInGitRepo));
      try
      {
        GitCodeRepoIndexingProperties properties = gitRepo.Properties as GitCodeRepoIndexingProperties;
        List<string> stringList = new List<string>();
        DateTime utcNow = DateTime.UtcNow;
        foreach (string str in this.GetBranchesToIndex(gitRepo))
        {
          if (str != null)
          {
            CustomerIntelligenceData processingDelayData = this.GetCIProcessingDelayData(gitRepo.TFSEntityId, pushNotificationValidator, projectId, properties.BranchIndexInfo[str].LastIndexedCommitId, str);
            if (processingDelayData != null)
              indexingExecutionContext.ExecutionTracerContext.PublishCi(this.TraceMetadata.TraceArea, this.TraceMetadata.TraceLayer, processingDelayData);
            GitCommit gitCommit = pushNotificationValidator.FetchBranchCommitInfo(gitRepo.TFSEntityId.ToString(), projectId.ToString(), str);
            if (gitCommit == null && properties.BranchIndexInfo.ContainsKey(str) && !properties.BranchIndexInfo[str].IsDefaultLastIndexedCommitId())
              this.QueueGitBranchDeleteOperation((ExecutionContext) indexingExecutionContext, gitRepo, new HashSet<string>()
              {
                str
              }, false);
            else if (gitCommit != null && !gitRepo.IsLargeRepository(indexingExecutionContext.RequestContext) && !gitCommit.CommitId.Equals(RepositoryConstants.BranchCreationOrDeletionCommitId) && (!properties.BranchIndexInfo.ContainsKey(str) || properties.BranchIndexInfo[str].IsDefaultLastIndexedCommitId() || !string.Equals(properties.BranchIndexInfo[str].LastIndexedCommitId, gitCommit.CommitId, StringComparison.OrdinalIgnoreCase)))
            {
              stringList.Add(str);
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Out of Sync commits found for RepositoryId = {0} IndexingUnitId = {1} isShadow = {2} Branch = '{3}' PushId= {4} at {5}", (object) gitRepo.TFSEntityId, (object) gitRepo.IndexingUnitId, (object) gitRepo.IsShadow, (object) str, (object) (properties.LastProcessedPushId + 1), (object) utcNow.ToString("dd/MM/yyyy HH:mm:ss.fff", (IFormatProvider) CultureInfo.InvariantCulture)));
            }
          }
        }
        if (stringList.Any<string>())
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Queued {0} to process missed check-ins for branches ({1}) in repository {2} with IndexingUnitId = {3} and isShadow = {4}.", (object) this.CreateContinuousIndexEventForGitRepo(this.ExecutionContext, gitRepo), (object) string.Join(",", (IEnumerable<string>) stringList), (object) gitRepo.TFSEntityId, (object) gitRepo.IndexingUnitId, (object) gitRepo.IsShadow)));
          return true;
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("No missing commits found for GIT Repo {0}", (object) gitRepo)));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata, ex.ToString());
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetadata, nameof (ProcessNonIndexedCommitsInGitRepo));
      }
      return false;
    }

    private CustomerIntelligenceData GetCIProcessingDelayData(
      Guid repoId,
      RepositoryPushNotificationValidator pushNotificationValidator,
      Guid projectId,
      string lastIndexedCommitId,
      string branch)
    {
      try
      {
        CustomerIntelligenceData processingDelayData = new CustomerIntelligenceData();
        GitCommit gitCommit1 = pushNotificationValidator.FetchBranchCommitInfo(repoId.ToString(), projectId.ToString(), branch);
        if (gitCommit1 == null)
          return (CustomerIntelligenceData) null;
        GitCommit gitCommit2 = pushNotificationValidator.FetchBranchCommitInfo(repoId.ToString(), projectId.ToString(), branch, lastIndexedCommitId);
        if (gitCommit2 == null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to fetch the info of the commit {0} for the branch {1}.", (object) lastIndexedCommitId, (object) branch));
          return (CustomerIntelligenceData) null;
        }
        double num = 0.0;
        if (gitCommit1.CommitId != gitCommit2.CommitId)
        {
          GitPush gitPush = pushNotificationValidator.FetchBranchNextGitPushInfo(repoId.ToString(), projectId.ToString(), branch, gitCommit2.Push.PushId);
          num = DateTime.UtcNow.Subtract(gitPush.Date).TotalMilliseconds;
          processingDelayData.Add("FirstUnprocessedPushId", (double) gitPush.PushId);
          processingDelayData.Add("FirstUnprocessedPushTime", (object) gitPush.Date);
        }
        processingDelayData.Add("CIProcessingDelaySource", "GitRepoSyncAnalyzer_Git");
        processingDelayData.Add("RepositoryId", repoId.ToString());
        processingDelayData.Add("BranchName", branch);
        processingDelayData.Add("LatestPushId", (double) gitCommit1.Push.PushId);
        processingDelayData.Add("LatestPushTime", (object) gitCommit1.Push.Date);
        processingDelayData.Add("LastProcessedPushId", (double) gitCommit2.Push.PushId);
        processingDelayData.Add("LastProcessedPushTime", (object) gitCommit2.Push.Date);
        processingDelayData.Add("CIProcessingDelayInMiliseconds", num);
        return processingDelayData;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetCIDelayData failed for repoId = '{0}', branch = '{1}' with exception : {2}", (object) repoId, (object) branch, (object) ex.ToString()));
      }
      return (CustomerIntelligenceData) null;
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueGitRepoBIOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      HashSet<string> branchesToBeBulkIndexed,
      string leaseId = null)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(leaseId)
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new GitRepositoryBIEventData(executionContext)
        {
          BranchesToBeBulkIndexed = branchesToBeBulkIndexed.ToList<string>()
        },
        ChangeType = indexingUnit.IsLargeRepository(executionContext.RequestContext) ? "UpdateIndex" : "BeginBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.m_indexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueGitBranchDeleteOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      HashSet<string> branches,
      bool cleanupIndexingUnitData,
      string leaseId = null)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(leaseId)
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new GitBranchDeleteEventData(executionContext)
        {
          CleanUpIndexingUnitData = cleanupIndexingUnitData,
          Branches = branches
        },
        ChangeType = "BranchDelete",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.m_indexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent);
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent QueueGitRepoPatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      List<string> BranchesToIndex)
    {
      if (BranchesToIndex == null)
        BranchesToIndex = new List<string>();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = indexingUnit.IndexingUnitId;
      GitBranchAddedEventData branchAddedEventData = new GitBranchAddedEventData(executionContext);
      branchAddedEventData.Trigger = 20;
      branchAddedEventData.BranchesToBeBulkIndexed = BranchesToIndex;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) branchAddedEventData;
      indexingUnitChangeEvent1.ChangeType = "Patch";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      return this.m_indexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent2);
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateRoutingAssignmentOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = gitRepoIndexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData(executionContext),
        ChangeType = "AssignRouting",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent assignmentOperation = this.m_indexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Added {0} event to assign routing to {1}.", (object) assignmentOperation.Id, (object) gitRepoIndexingUnit)));
      return assignmentOperation;
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateContinuousIndexEventForGitRepo(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = gitRepoIndexingUnit.IndexingUnitId,
        ChangeType = "UpdateIndex",
        ChangeData = (ChangeEventData) new GitRepositoryBIEventData(executionContext)
        {
          BranchesToBeBulkIndexed = new List<string>()
        },
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexEventForGitRepo = this.m_indexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent);
      executionContext.ExecutionTracerContext.PublishCi(this.TraceMetadata.TraceArea, this.TraceMetadata.TraceLayer, "OperationCorrelationId", indexEventForGitRepo.ChangeData.CorrelationId);
      return indexEventForGitRepo;
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent HandleBranchConfigurationChanges(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit,
      List<string> branchesAdded)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = gitRepoIndexingUnit.IndexingUnitId;
      indexingUnitChangeEvent1.ChangeType = "Patch";
      GitBranchAddedEventData branchAddedEventData = new GitBranchAddedEventData(executionContext);
      branchAddedEventData.Trigger = 20;
      branchAddedEventData.BranchesToBeBulkIndexed = branchesAdded;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) branchAddedEventData;
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      return this.m_indexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent2);
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateRepoRenameEventForGitRepo(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit,
      string repositoryName)
    {
      return this.m_indexingUnitChangeEventHandler.HandleEvent(this.ExecutionContext, new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = repoIndexingUnit.IndexingUnitId,
        ChangeType = "BeginEntityRename",
        ChangeData = (ChangeEventData) new EntityRenameEventData(this.ExecutionContext)
        {
          NewEntityName = repositoryName
        },
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = (byte) 0
      });
    }

    private List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetFilteredIndexingUnits(
      List<IndexingUnitChangeEventState> ignoredStates)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.m_indexingUnitDataAccess.GetIndexingUnits(this.RequestContext, "Git_Repository", this.EntityType, -1);
      IEnumerable<IndexingUnitChangeEventDetails> pendingIndexingOps = this.m_indexingUnitChangeEventDataAccess.GetIndexingUnitChangeEvents(this.RequestContext, new List<string>()
      {
        "BeginBulkIndex",
        "UpdateIndex"
      }, ignoredStates, -1);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Filtered {0} GIT repos as those have pending indexing operations", (object) indexingUnits.RemoveAll((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo => pendingIndexingOps.Any<IndexingUnitChangeEventDetails>((Func<IndexingUnitChangeEventDetails, bool>) (op => op.IndexingUnitChangeEvent.IndexingUnitId == repo.IndexingUnitId))))));
      return indexingUnits;
    }

    protected Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetProjectIndexingUnit(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      Guid projectId)
    {
      return indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, projectId, "Project", this.EntityType);
    }

    protected Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetCollectionIndexingUnit(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      return indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), "Collection", this.EntityType);
    }

    internal virtual bool CreateRepositoryIndexingUnitIfNeeded(
      ExecutionContext executionContext,
      GitRepository gitRepository,
      Func<ExecutionContext, GitRepository, ProjectHttpClientWrapper, bool> isDisabled,
      string sourceForTraceInfo,
      out Microsoft.VisualStudio.Services.Search.Common.IndexingUnit projectIndexingUnit,
      out Microsoft.VisualStudio.Services.Search.Common.IndexingUnit repoIndexingUnit,
      out bool repoIndexingUnitCreated,
      bool isShadow = false)
    {
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      projectIndexingUnit = (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit) null;
      repoIndexingUnitCreated = false;
      repoIndexingUnit = (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit) null;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = this.GetCollectionIndexingUnit(executionContext, indexingUnitDataAccess);
      if (collectionIndexingUnit == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata.TracePoint, this.TraceMetadata.TraceArea, this.TraceMetadata.TraceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Collection Id {0} is not bulk indexed yet for {1}", (object) executionContext.RequestContext.GetCollectionID(), (object) sourceForTraceInfo));
        return false;
      }
      projectIndexingUnit = this.GetProjectIndexingUnit(executionContext, indexingUnitDataAccess, gitRepository.ProjectReference.Id);
      if (projectIndexingUnit == null)
      {
        if (gitRepository.ProjectReference.State != ProjectState.WellFormed || !this.CheckProjectExistenceInTfs(executionContext, gitRepository.ProjectReference.Id.ToString()))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata.TracePoint, this.TraceMetadata.TraceArea, this.TraceMetadata.TraceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Project Id {0} is not WellFormed in TFS for {1}", (object) gitRepository.ProjectReference.Id, (object) sourceForTraceInfo));
          return false;
        }
        projectIndexingUnit = this.CreateProjectIndexingUnit(executionContext, indexingUnitDataAccess, gitRepository.ProjectReference, collectionIndexingUnit.IndexingUnitId);
        if (projectIndexingUnit == null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata.TracePoint, this.TraceMetadata.TraceArea, this.TraceMetadata.TraceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not create Indexing Unit for git Project Id: {0} for {1}", (object) gitRepository.ProjectReference.Id, (object) sourceForTraceInfo));
          return false;
        }
      }
      ref Microsoft.VisualStudio.Services.Search.Common.IndexingUnit local = ref repoIndexingUnit;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(gitRepository.Id, "Git_Repository", this.EntityType, projectIndexingUnit.IndexingUnitId, isShadow);
      GitCodeRepoTFSAttributes repoTfsAttributes = new GitCodeRepoTFSAttributes();
      repoTfsAttributes.RepositoryName = gitRepository.Name;
      repoTfsAttributes.DefaultBranch = gitRepository.DefaultBranch;
      repoTfsAttributes.RemoteUrl = gitRepository.RemoteUrl;
      indexingUnit.TFSEntityAttributes = (TFSEntityAttributes) repoTfsAttributes;
      GitCodeRepoIndexingProperties indexingProperties = new GitCodeRepoIndexingProperties();
      indexingProperties.Name = gitRepository.Name;
      indexingProperties.IsDisabled = isDisabled(executionContext, gitRepository, (ProjectHttpClientWrapper) null);
      indexingUnit.Properties = (IndexingProperties) indexingProperties;
      local = indexingUnit;
      repoIndexingUnit = indexingUnitDataAccess.AddOrUpdateIndexingUnits(executionContext.RequestContext, new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()
      {
        repoIndexingUnit
      }, true).FirstOrDefault<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (repoIndexingUnit == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not create Indexing Unit for Git Repository Id: {0}. Project Id: {1}", (object) gitRepository.Id, (object) gitRepository.ProjectReference.Id));
        return false;
      }
      repoIndexingUnitCreated = true;
      if (!isShadow)
        CodeQueryScopingCacheUtil.SqlNotifyForRepoAddition(this.m_dataAccessFactory, executionContext.RequestContext, repoIndexingUnit);
      return true;
    }

    internal virtual bool IsMultiBranchConfigurationChanged(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit,
      GitRepository gitRepoInTFS)
    {
      try
      {
        List<string> enabledBranchesFromTfs = this.GetSearchEnabledBranchesFromTFS(gitRepoIndexingUnit, gitRepoInTFS.ProjectReference.Id);
        return (gitRepoIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes).IdentifyBranchChanges(enabledBranchesFromTfs);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, ex.ToString());
      }
      return false;
    }

    internal virtual List<string> GetNewlyConfiguredBranches(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit,
      GitRepository gitRepoInTFS)
    {
      try
      {
        List<string> enabledBranchesFromTfs = this.GetSearchEnabledBranchesFromTFS(gitRepoIndexingUnit, gitRepoInTFS.ProjectReference.Id);
        return (gitRepoIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes).GetBranchChanges(enabledBranchesFromTfs);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, ex.ToString());
      }
      return (List<string>) null;
    }

    internal virtual List<string> GetDefaultBranchChange(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit,
      GitRepository gitRepoInTFS,
      GitCodeRepoTFSAttributes gitCodeRepoTfsAttributes)
    {
      try
      {
        string str1 = string.IsNullOrWhiteSpace(gitRepoInTFS.DefaultBranch) ? string.Empty : gitRepoInTFS.DefaultBranch;
        string defaultBranch = gitCodeRepoTfsAttributes.DefaultBranch;
        List<string> enabledBranchesFromTfs = this.GetSearchEnabledBranchesFromTFS(gitRepoIndexingUnit, gitRepoInTFS.ProjectReference.Id);
        List<string> defaultBranchChange = new List<string>()
        {
          str1
        };
        string str2 = defaultBranch;
        if (enabledBranchesFromTfs.Contains(str2))
          defaultBranchChange.Add(defaultBranch);
        return defaultBranchChange;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, ex.ToString());
      }
      return (List<string>) null;
    }

    internal virtual List<string> GetSearchEnabledBranchesFromTFS(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit,
      Guid projectId)
    {
      SearchGitRepoSettingsForPolicy settingsForPolicy = this.GetSearchGitRepoSettingsForPolicy(gitRepoIndexingUnit, projectId);
      return settingsForPolicy == null || settingsForPolicy.SearchBranches == null ? new List<string>() : settingsForPolicy.SearchBranches.ToList<string>();
    }

    internal virtual SearchGitRepoSettingsForPolicy GetSearchGitRepoSettingsForPolicy(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit gitRepoIndexingUnit,
      Guid projectId)
    {
      return new SearchGitRepoSettingsProvider(this.RequestContext, projectId, gitRepoIndexingUnit.TFSEntityId).GetRepoSetting(gitRepoIndexingUnit.TFSEntityId);
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnit CreateProjectIndexingUnit(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      TeamProjectReference project,
      int collectionIndexingUnitId)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(project.Id, "Project", this.EntityType, collectionIndexingUnitId);
      indexingUnit1.TFSEntityAttributes = (TFSEntityAttributes) new ProjectCodeTFSAttributes()
      {
        ProjectName = project.Name,
        ProjectVisibility = project.Visibility
      };
      ProjectCodeIndexingProperties indexingProperties = new ProjectCodeIndexingProperties();
      indexingProperties.Name = project.Name;
      indexingProperties.ProjectVisibility = project.Visibility;
      indexingUnit1.Properties = (IndexingProperties) indexingProperties;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 = indexingUnit1;
      return indexingUnitDataAccess.AddOrUpdateIndexingUnits(executionContext.RequestContext, new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()
      {
        indexingUnit2
      }, true).FirstOrDefault<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
    }

    internal virtual bool CheckProjectExistenceInTfs(
      ExecutionContext executionContext,
      string projectId)
    {
      return new ProjectHttpClientWrapper(executionContext, this.TraceMetadata).GetProjectInGivenState(projectId) != null;
    }

    private List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetAllIndexedGitRepos()
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = this.m_indexingUnitDataAccess.GetIndexingUnits(this.RequestContext, "Git_Repository", this.EntityType, -1);
      if (this.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() && this.ExecutionContext.IsReindexingFailedOrInProgress(this.m_dataAccessFactory, this.EntityType))
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = this.m_indexingUnitDataAccess.GetIndexingUnits(this.RequestContext, "Git_Repository", true, this.EntityType, -1);
        indexingUnits1.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits2);
      }
      return indexingUnits1;
    }

    internal abstract List<GitRepository> GetTfsRepos();

    protected IVssRequestContext RequestContext { get; set; }

    protected ExecutionContext ExecutionContext { get; set; }

    protected TraceMetaData TraceMetadata { get; set; }

    private GitHttpClientWrapper m_gitHttpClientWrapper { get; set; }
  }
}
