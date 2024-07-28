// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CodeIndexingStatusProvider
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public class CodeIndexingStatusProvider : IndexingStatusProvider
  {
    [StaticSafe]
    private static readonly HashSet<string> s_indexingOperations = new HashSet<string>()
    {
      "CrawlMetadata",
      "BeginBulkIndex",
      "CompleteBulkIndex",
      "BeginEntityRename",
      "BeginProjectRename",
      "CustomGitRepositoryBulkIndex",
      "Patch"
    };
    [StaticSafe]
    private static readonly HashSet<int> s_indexingJobTrigger = new HashSet<int>()
    {
      1,
      4,
      33,
      20
    };
    private readonly IDataAccessFactory m_dataAccessFactory;
    [StaticSafe]
    private static readonly List<IndexingUnitChangeEventState> changeEventStateList = new List<IndexingUnitChangeEventState>()
    {
      IndexingUnitChangeEventState.Pending,
      IndexingUnitChangeEventState.InProgress,
      IndexingUnitChangeEventState.Queued
    };

    internal CodeIndexingStatusProvider(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory)
    {
      this.m_dataAccessFactory = dataAccessFactory;
    }

    protected override IEntityType EntityType => (IEntityType) CodeEntityType.GetInstance();

    public override ISet<string> GetSupportedOperations() => (ISet<string>) CodeIndexingStatusProvider.s_indexingOperations;

    public override ISet<int> GetSupportedJobTriggers() => (ISet<int>) CodeIndexingStatusProvider.s_indexingJobTrigger;

    protected override CollectionIndexingStatus? GetIndexingStatus(
      IVssRequestContext requestContext,
      int trigger,
      out IndexingStatusDetails indexingStatusDetails)
    {
      indexingStatusDetails = (IndexingStatusDetails) null;
      switch (trigger)
      {
        case 1:
          return new CollectionIndexingStatus?(CollectionIndexingStatus.Onboarding);
        case 4:
        case 33:
          return new CollectionIndexingStatus?(requestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() ? CollectionIndexingStatus.NotIndexing : CollectionIndexingStatus.Reindexing);
        case 20:
          indexingStatusDetails = (IndexingStatusDetails) this.GetBranchesCurrentlyIndexing(requestContext);
          return new CollectionIndexingStatus?(CollectionIndexingStatus.BranchIndexing);
        default:
          return new CollectionIndexingStatus?();
      }
    }

    internal CodeIndexingStatusDetails GetBranchesCurrentlyIndexing(
      IVssRequestContext collectionRequestContext)
    {
      IIndexingUnitChangeEventDataAccess changeEventDataAccess = this.m_dataAccessFactory.GetIndexingUnitChangeEventDataAccess();
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      IVssRequestContext requestContext = collectionRequestContext;
      List<string> list1 = CodeIndexingStatusProvider.s_indexingOperations.ToList<string>();
      List<int> list2 = CodeIndexingStatusProvider.s_indexingJobTrigger.ToList<int>();
      List<IndexingUnitChangeEventState> changeEventStateList = CodeIndexingStatusProvider.changeEventStateList;
      List<Tuple<IEntityType, int, string, int, Microsoft.VisualStudio.Services.Search.Common.ChangeEventData>> unitChangeEvents = changeEventDataAccess.GetIndexingUnitChangeEvents(requestContext, list1, list2, changeEventStateList);
      CodeIndexingStatusDetails currentlyIndexing = new CodeIndexingStatusDetails();
      foreach (Tuple<IEntityType, int, string, int, Microsoft.VisualStudio.Services.Search.Common.ChangeEventData> tuple in unitChangeEvents)
      {
        int indexingUnitId = tuple.Item2;
        List<string> beingBulkIndexed = this.GetBranchesBeingBulkIndexed(tuple.Item5);
        if (beingBulkIndexed != null && beingBulkIndexed.Count<string>() > 0)
        {
          IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(collectionRequestContext, indexingUnitId);
          if (!indexingUnit.IsShadow || !collectionRequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled())
          {
            string fromTfsAttributes = indexingUnit.GetRepositoryNameFromTFSAttributes();
            string projectIuElseNull = indexingUnitDataAccess.GetIndexingUnit(collectionRequestContext, indexingUnit.ParentUnitId).GetProjectNameFromTFSAttributesIfProjectIUElseNull();
            GitCodeRepoTFSAttributes entityAttributes = indexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
            currentlyIndexing.AddToBranchesCurrentlyBeingIndexed(projectIuElseNull, fromTfsAttributes, beingBulkIndexed);
            currentlyIndexing.AddToDefaultBranches(projectIuElseNull, fromTfsAttributes, entityAttributes.DefaultBranch);
          }
        }
      }
      return currentlyIndexing;
    }

    internal virtual List<string> GetBranchesBeingBulkIndexed(Microsoft.VisualStudio.Services.Search.Common.ChangeEventData changeData)
    {
      List<string> stringList = new List<string>();
      if (changeData == null)
        return new List<string>();
      if (object.Equals((object) changeData.GetType(), (object) typeof (GitBranchAddedEventData)))
        stringList = ((GitBranchAddedEventData) changeData).BranchesToBeBulkIndexed;
      else if (object.Equals((object) changeData.GetType(), (object) typeof (GitRepositoryBIEventData)))
        stringList = ((GitRepositoryBIEventData) changeData).BranchesToBeBulkIndexed;
      else if (object.Equals((object) changeData.GetType(), (object) typeof (GitRepositoryCustomBulkIndexingEventData)))
        stringList = ((GitRepositoryCustomBulkIndexingEventData) changeData).BranchesToIndex;
      return stringList ?? new List<string>();
    }
  }
}
