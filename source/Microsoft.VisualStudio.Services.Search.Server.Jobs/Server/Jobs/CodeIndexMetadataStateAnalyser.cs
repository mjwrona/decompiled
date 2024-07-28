// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.CodeIndexMetadataStateAnalyser
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class CodeIndexMetadataStateAnalyser : IndexMetadataStateAnalyser
  {
    private ProjectHttpClientWrapper m_projectHttpClientWrapper;
    private TfvcHttpClientWrapper m_tfvcHttpClientWrapper;

    public CodeIndexMetadataStateAnalyser()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler())
    {
    }

    public CodeIndexMetadataStateAnalyser(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(dataAccessFactory, indexingUnitChangeEventHandler)
    {
    }

    public CodeIndexMetadataStateAnalyser(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      ProjectHttpClientWrapper projectHttpClientWrapper,
      TfvcHttpClientWrapper tfvcHttpClientWrapper)
      : base(dataAccessFactory, indexingUnitChangeEventHandler)
    {
      this.m_projectHttpClientWrapper = projectHttpClientWrapper;
      this.m_tfvcHttpClientWrapper = tfvcHttpClientWrapper;
    }

    protected override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionCodeFinalizeHelper();

    protected override TraceMetaData TraceMetadata => new TraceMetaData(1080638, "Indexing Pipeline", "IndexingOperation");

    public virtual int ProcessMissingTfvcRepos(IndexingExecutionContext indexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetadata, nameof (ProcessMissingTfvcRepos));
      try
      {
        bool flag = indexingExecutionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() && indexingExecutionContext.IsReindexingFailedOrInProgress(this.DataAccessFactory, (IEntityType) CodeEntityType.GetInstance());
        IEnumerable<TeamProject> withCapabilities = this.GetProjectHttpClientWrapper((ExecutionContext) indexingExecutionContext).GetTeamProjectsWithCapabilities();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source1 = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "Project", (IEntityType) CodeEntityType.GetInstance(), -1) ?? new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source2 = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "TFVC_Repository", (IEntityType) CodeEntityType.GetInstance(), -1) ?? new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source3 = flag ? this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "TFVC_Repository", true, (IEntityType) CodeEntityType.GetInstance(), -1) ?? new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() : new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        IDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> idToTfvcProjectIndexingUnit = (IDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) source1.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x));
        IDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary1 = (IDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) source2.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x));
        IDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary2 = (IDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) source3.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x));
        List<TeamProject> source4 = new List<TeamProject>();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> collection = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        foreach (TeamProject teamProject in withCapabilities)
        {
          switch (teamProject.GetSupportedVersionControlType())
          {
            case VersionControlType.TFVC:
            case VersionControlType.GitAndTFVC:
              Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1;
              if (idToTfvcProjectIndexingUnit.TryGetValue(teamProject.Id, out indexingUnit1))
              {
                Microsoft.VisualStudio.Services.Search.Common.IndexingUnit codeIndexingUnit;
                if (dictionary1.TryGetValue(teamProject.Id, out codeIndexingUnit))
                {
                  if (codeIndexingUnit.Properties.IndexIndices == null || !codeIndexingUnit.Properties.IndexIndices.Any<IndexInfo>())
                    collection.Add(codeIndexingUnit);
                  Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
                  if (flag && dictionary2.TryGetValue(teamProject.Id, out indexingUnit2) && (indexingUnit2.Properties.IndexIndices == null || !indexingUnit2.Properties.IndexIndices.Any<IndexInfo>()))
                  {
                    collection.Add(indexingUnit2);
                    continue;
                  }
                  continue;
                }
                codeIndexingUnit = teamProject.ToTfvcRepoCodeIndexingUnit(indexingUnit1.IndexingUnitId);
                indexingUnitList.Add(codeIndexingUnit);
                if (flag)
                {
                  indexingUnitList.Add(teamProject.ToTfvcRepoCodeIndexingUnit(indexingUnit1.IndexingUnitId, true));
                  continue;
                }
                continue;
              }
              source4.Add(teamProject);
              continue;
            default:
              continue;
          }
        }
        if (source4.Count <= 0 && indexingUnitList.Count <= 0 && collection.Count <= 0)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, "No missing TFVC Projects/TFVC Repos/TFVC Repos with No Index Info found.");
          return 0;
        }
        if (source4.Count > 0)
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Adding {0} new TFVC Projects with Ids '{1}'", (object) source4.Count, (object) string.Join<Guid>(",", source4.Select<TeamProject, Guid>((Func<TeamProject, Guid>) (p => p.Id)))));
        int collectionIndexingUnitId = source1.Count > 0 ? source1[0].ParentUnitId : this.IndexingUnitDataAccess.GetIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.RequestContext.GetCollectionID(), "Collection", (IEntityType) CodeEntityType.GetInstance()).IndexingUnitId;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list1 = source4.Select<TeamProject, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<TeamProject, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (project => project.ToProjectCodeIndexingUnit(collectionIndexingUnitId))).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        if (list1.Count > 0)
        {
          List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> source5 = this.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(indexingExecutionContext.RequestContext, list1, true);
          idToTfvcProjectIndexingUnit.AddRange<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, IDictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>((IEnumerable<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>>) source5.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x)));
          indexingUnitList.AddRange(source4.Select<TeamProject, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<TeamProject, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (project => project.ToTfvcRepoCodeIndexingUnit(idToTfvcProjectIndexingUnit[project.Id].IndexingUnitId))));
          if (flag)
            indexingUnitList.AddRange(source4.Select<TeamProject, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<TeamProject, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (project => project.ToTfvcRepoCodeIndexingUnit(idToTfvcProjectIndexingUnit[project.Id].IndexingUnitId, true))));
        }
        if (indexingUnitList.Count > 0)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Adding {0} new TFVC Repository Indexing Units", (object) indexingUnitList.Count)));
          indexingUnitList = this.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitList, true);
        }
        indexingUnitList.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) collection);
        int count = indexingUnitList.Count;
        if (indexingUnitList.Count > 0)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Creating RoutingAssignment Operation for {0} TFVC Repository Indexing Units ", (object) indexingUnitList.Count)));
          TfvcHttpClientWrapper tfvcHttpClientWrapper = this.GetTfvcHttpClientWrapper((ExecutionContext) indexingExecutionContext);
          IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> list2 = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitList.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (x => !tfvcHttpClientWrapper.IsEmpty(indexingExecutionContext.RequestContext, indexingExecutionContext.ProvisioningContext.ContractType, x))).Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) (tfvcRepo => new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
          {
            IndexingUnitId = tfvcRepo.IndexingUnitId,
            ChangeData = new ChangeEventData((ExecutionContext) indexingExecutionContext),
            ChangeType = "AssignRouting",
            State = IndexingUnitChangeEventState.Pending,
            AttemptCount = (byte) 0
          })).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
          count = list2.Count;
          if (list2.Count > 0)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Process Missing TfvcRepos successfully completed. Added {0} events to assign routing.", (object) string.Join(",", this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, list2).Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, string>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent, string>) (x => x.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)))))));
        }
        return count;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetadata, nameof (ProcessMissingTfvcRepos));
      }
    }

    public virtual int ProcessMissingProjectRenameAndDeleteNotifications(
      ExecutionContext executionContext)
    {
      bool entityCrudOperationsFeatureEnabled = executionContext.RequestContext.IsFeatureEnabled("Search.Server.Code.CrudOperations");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.ProcessMissingEntityRenameAndDeleteNotifications(executionContext, "Project", (IEntityType) CodeEntityType.GetInstance(), CodeIndexMetadataStateAnalyser.\u003C\u003EO.\u003C0\u003E__HasCodeProjectChanged ?? (CodeIndexMetadataStateAnalyser.\u003C\u003EO.\u003C0\u003E__HasCodeProjectChanged = new Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamProjectReference, bool, bool>(CodeProjectUtils.HasCodeProjectChanged)), (IDictionary<Guid, TeamProjectReference>) this.GetProjectHttpClientWrapper(executionContext).GetProjects().ToDictionary<TeamProjectReference, Guid, TeamProjectReference>((Func<TeamProjectReference, Guid>) (x => x.Id), (Func<TeamProjectReference, TeamProjectReference>) (x => x)), entityCrudOperationsFeatureEnabled);
    }

    public virtual int ProcessMissingChangesetsInCollection(ExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetadata, nameof (ProcessMissingChangesetsInCollection));
      try
      {
        int num = 0;
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> allTfvcRepos = this.GetAllTfvcRepos(executionContext);
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.IndexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, "Project", (IEntityType) CodeEntityType.GetInstance(), -1);
        FriendlyDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> friendlyDictionary = new FriendlyDictionary<int, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnits)
          friendlyDictionary.Add(indexingUnit.IndexingUnitId, indexingUnit);
        this.RemoveDeletedTfvcRepos(allTfvcRepos, executionContext);
        List<string> changeTypeList = new List<string>()
        {
          "BeginBulkIndex",
          "UpdateIndex"
        };
        List<IndexingUnitChangeEventState> stateList = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Pending,
          IndexingUnitChangeEventState.Queued,
          IndexingUnitChangeEventState.InProgress,
          IndexingUnitChangeEventState.FailedAndRetry
        };
        IEnumerable<IndexingUnitChangeEventDetails> pendingIndexingOps = this.IndexingUnitChangeEventDataAccess.GetIndexingUnitChangeEvents(executionContext.RequestContext, changeTypeList, stateList, -1);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} TFVC repos will not be processed for missing changeset notification as those have pending indexing operations", (object) allTfvcRepos.RemoveAll((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (repo => pendingIndexingOps.Any<IndexingUnitChangeEventDetails>((Func<IndexingUnitChangeEventDetails, bool>) (op => op.IndexingUnitChangeEvent.IndexingUnitId == repo.IndexingUnitId))))));
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit tfvcRepo in allTfvcRepos)
        {
          try
          {
            if (!tfvcRepo.Properties.IsDisabled)
            {
              Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
              friendlyDictionary.TryGetValue(tfvcRepo.ParentUnitId, out indexingUnit);
              if (indexingUnit != null)
              {
                string projectName = indexingUnit.TFSEntityAttributes is ProjectCodeTFSAttributes entityAttributes ? entityAttributes.ProjectName : (string) null;
                Guid tfsEntityId = indexingUnit.TFSEntityId;
                if (!string.IsNullOrWhiteSpace(projectName))
                {
                  if (this.ProcessMissingChangesetsInTfvcRepo(executionContext, tfvcRepo, projectName, tfsEntityId))
                    ++num;
                }
                else
                  Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Project name for tfvc project with indexing unit id: {0} is null or empty.", (object) tfsEntityId));
              }
              else
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Project indexing unit for tfvc repo with indexing unit id: {0} is not found.", (object) tfvcRepo.TFSEntityId));
            }
          }
          catch (Exception ex)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Process missing change set failed for tfvc repo id '{0}' IndexingUnitID '{1}' isShadow '{2}' with exception : {3}", (object) tfvcRepo.TFSEntityId, (object) tfvcRepo.IndexingUnitId, (object) tfvcRepo.IsShadow, (object) ex.ToString()));
          }
        }
        return num;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetadata, nameof (ProcessMissingChangesetsInCollection));
      }
    }

    public virtual bool ProcessMissingChangesetsInTfvcRepo(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit tfvcRepo,
      string projectName,
      Guid projectId)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetadata, nameof (ProcessMissingChangesetsInTfvcRepo));
      try
      {
        TeamProject projectInGivenState = this.GetProjectHttpClientWrapper(executionContext).GetProjectInGivenState(projectId.ToString());
        if (projectInGivenState == null)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find the project (id: '{0}') in Tfs.", (object) projectId.ToString()));
          return false;
        }
        string str1 = "$/" + projectInGivenState.Name;
        string str2 = "$/" + projectName;
        if (str1 != str2)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Mismatch in scope path. Latest scope path is '{0}', but IndexingUnit has the scope path as '{1}'", (object) str1, (object) str2));
          return false;
        }
        TfvcCodeRepoIndexingProperties properties = tfvcRepo.Properties as TfvcCodeRepoIndexingProperties;
        TfvcHttpClientWrapper httpClientWrapper = this.GetTfvcHttpClientWrapper(executionContext);
        List<TfvcChangesetRef> indexedChangesets = httpClientWrapper.GetUnIndexedChangesets(tfvcRepo.TFSEntityId.ToString(), str1, properties.LastIndexedChangeSetId);
        CustomerIntelligenceData processingDelayData = this.GetCIProcessingDelayData(tfvcRepo.TFSEntityId, str1, properties, httpClientWrapper);
        if (processingDelayData != null)
          executionContext.ExecutionTracerContext.PublishCi(this.TraceMetadata.TraceArea, this.TraceMetadata.TraceLayer, processingDelayData);
        if (!indexedChangesets.Any<TfvcChangesetRef>())
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No missing changeset found for Tfvc Repo {0}", (object) tfvcRepo.ToString()));
          return false;
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Processing {0} missing changesets found for Tfvc Repo {1}", (object) indexedChangesets.Count, (object) tfvcRepo.ToString()));
        TfvcChangesetRef tfvcChangesetRef = indexedChangesets.OrderBy<TfvcChangesetRef, int>((Func<TfvcChangesetRef, int>) (c => c.ChangesetId)).Last<TfvcChangesetRef>();
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          IndexingUnitId = tfvcRepo.IndexingUnitId,
          ChangeType = "UpdateIndex",
          ChangeData = (ChangeEventData) new TFVCCodeContinuousIndexEventData(executionContext)
          {
            ChangesetId = tfvcChangesetRef.ChangesetId
          },
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        this.IndexingUnitChangeEventHandler.HandleEvent(executionContext, indexingUnitChangeEvent);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UpdateIndex operation created with changesetId {0} for Tfvc Repo {1}", (object) tfvcChangesetRef.ChangesetId, (object) tfvcRepo.ToString()));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetadata, nameof (ProcessMissingChangesetsInTfvcRepo));
      }
      return true;
    }

    private CustomerIntelligenceData GetCIProcessingDelayData(
      Guid tfvcRepoId,
      string currentScopePath,
      TfvcCodeRepoIndexingProperties repoIndexingProperties,
      TfvcHttpClientWrapper tfvcHttpClientWrapper)
    {
      try
      {
        CustomerIntelligenceData processingDelayData = new CustomerIntelligenceData();
        TfvcChangesetRef changeset = (TfvcChangesetRef) tfvcHttpClientWrapper.GetChangeset(tfvcRepoId.ToString(), repoIndexingProperties.LastIndexedChangeSetId);
        List<TfvcChangesetRef> indexedChangesets = tfvcHttpClientWrapper.GetUnIndexedChangesets(tfvcRepoId.ToString(), currentScopePath, repoIndexingProperties.LastIndexedChangeSetId);
        double num = 0.0;
        TfvcChangesetRef tfvcChangesetRef1 = changeset;
        if (indexedChangesets.Any<TfvcChangesetRef>())
        {
          List<TfvcChangesetRef> list = indexedChangesets.OrderBy<TfvcChangesetRef, int>((Func<TfvcChangesetRef, int>) (c => c.ChangesetId)).ToList<TfvcChangesetRef>();
          TfvcChangesetRef tfvcChangesetRef2 = list.Any<TfvcChangesetRef>() ? list.First<TfvcChangesetRef>() : changeset;
          tfvcChangesetRef1 = list.Any<TfvcChangesetRef>() ? list.Last<TfvcChangesetRef>() : changeset;
          num = DateTime.UtcNow.Subtract(tfvcChangesetRef2.CreatedDate).TotalMilliseconds;
          processingDelayData.Add("FirstUnprocessedPushId", (double) tfvcChangesetRef2.ChangesetId);
          processingDelayData.Add("FirstUnprocessedPushTime", (object) tfvcChangesetRef2.CreatedDate);
        }
        processingDelayData.Add("CIProcessingDelaySource", "CodeIndexMetadataStateAnalyser_Tfvc");
        processingDelayData.Add("RepositoryId", tfvcRepoId.ToString());
        processingDelayData.Add("ScopePath", currentScopePath);
        processingDelayData.Add("LatestPushId", (double) tfvcChangesetRef1.ChangesetId);
        processingDelayData.Add("LatestPushTime", (object) tfvcChangesetRef1.CreatedDate);
        processingDelayData.Add("LastProcessedPushId", (double) changeset.ChangesetId);
        processingDelayData.Add("LastProcessedPushTime", (object) changeset.CreatedDate);
        processingDelayData.Add("CIProcessingDelayInMiliseconds", num);
        return processingDelayData;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TraceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetCIDelayData failed for repoId = '{0}', ScopePath = '{1}' with exception : {2}", (object) tfvcRepoId.ToString(), (object) currentScopePath, (object) ex.ToString()));
      }
      return (CustomerIntelligenceData) null;
    }

    internal virtual void RemoveDeletedTfvcRepos(
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> tfvcRepos,
      ExecutionContext executionContext)
    {
      IEnumerable<Guid> repoIds = this.GetProjectHttpClientWrapper(executionContext).GetTeamProjectsWithCapabilities().Select<TeamProject, Guid>((Func<TeamProject, Guid>) (project => project.Id));
      tfvcRepos.RemoveAll((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (tfvcRepo => !repoIds.Contains<Guid>(tfvcRepo.TFSEntityId)));
    }

    internal virtual ProjectHttpClientWrapper GetProjectHttpClientWrapper(
      ExecutionContext executionContext)
    {
      if (this.m_projectHttpClientWrapper == null)
        this.m_projectHttpClientWrapper = new ProjectHttpClientWrapper(executionContext, this.TraceMetadata);
      return this.m_projectHttpClientWrapper;
    }

    internal TfvcHttpClientWrapper GetTfvcHttpClientWrapper(ExecutionContext executionContext)
    {
      if (this.m_tfvcHttpClientWrapper == null)
        this.m_tfvcHttpClientWrapper = new TfvcHttpClientWrapper(executionContext, this.TraceMetadata);
      return this.m_tfvcHttpClientWrapper;
    }

    protected virtual int ProcessMissingEntityRenameAndDeleteNotifications(
      ExecutionContext executionContext,
      string indexingUnitType,
      IEntityType entityType,
      Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamProjectReference, bool, bool> hasEntityModified,
      IDictionary<Guid, TeamProjectReference> tfsEntityMap,
      bool entityCrudOperationsFeatureEnabled)
    {
      if (!entityCrudOperationsFeatureEnabled)
        return 0;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.IndexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, indexingUnitType, entityType, -1);
      int num = 0;
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit in indexingUnits)
      {
        TeamProjectReference tfsProject;
        tfsEntityMap.TryGetValue(entityIndexingUnit.TFSEntityId, out tfsProject);
        if (entityCrudOperationsFeatureEnabled && tfsProject == null && !executionContext.RequestContext.IsProjectSoftDeleted(entityIndexingUnit.TFSEntityId.ToString()))
          num += this.CreateEntityDeleteOperationIfRequired(executionContext, entityIndexingUnit);
        if (entityCrudOperationsFeatureEnabled && tfsProject != null)
          num += this.CreateEntityRenameOperationIfRequired(executionContext, entityIndexingUnit, tfsProject, hasEntityModified, true);
      }
      return num;
    }

    private List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetAllTfvcRepos(
      ExecutionContext indexingExecutionContext)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> allTfvcRepos = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "TFVC_Repository", (IEntityType) CodeEntityType.GetInstance(), -1) ?? new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (indexingExecutionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() && indexingExecutionContext.IsReindexingFailedOrInProgress(this.DataAccessFactory, (IEntityType) CodeEntityType.GetInstance()))
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> collection = this.IndexingUnitDataAccess.GetIndexingUnits(indexingExecutionContext.RequestContext, "TFVC_Repository", true, (IEntityType) CodeEntityType.GetInstance(), -1) ?? new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
        allTfvcRepos.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) collection);
      }
      return allTfvcRepos;
    }
  }
}
