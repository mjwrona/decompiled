// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git.LargeRepoAccountGitBranchDeleter
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git
{
  public class LargeRepoAccountGitBranchDeleter : GitBranchDeleter
  {
    private List<IndexingUnit> m_scopedIndexingUnits;

    public LargeRepoAccountGitBranchDeleter(IndexingExecutionContext indexingExecutionContext) => this.m_scopedIndexingUnits = indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit.IndexingUnitId, -1);

    public override OperationStatus PerformBranchDeletion(
      IndexingExecutionContext indexingExecutionContext,
      IIndexingUnitChangeEventHandler eventHandler,
      List<string> branchesToDeleteList,
      IndexIdentity searchIndex,
      IndexingUnit indexingUnit,
      bool cleanUpIndexingUnitData,
      string leaseId,
      StringBuilder resultMessage)
    {
      IList<IndexingUnitChangeEvent> prereqChangeEvents = (IList<IndexingUnitChangeEvent>) new List<IndexingUnitChangeEvent>();
      foreach (IndexingUnit scopedIndexingUnit in this.m_scopedIndexingUnits)
      {
        IndexingUnitChangeEvent indexingUnitChangeEvent1 = new IndexingUnitChangeEvent(leaseId)
        {
          IndexingUnitId = scopedIndexingUnit.IndexingUnitId,
          ChangeType = "BranchDelete",
          ChangeData = (ChangeEventData) new GitBranchDeleteEventData((ExecutionContext) indexingExecutionContext)
          {
            Branches = branchesToDeleteList.ToHashSet<string>(),
            CleanUpIndexingUnitData = cleanUpIndexingUnitData
          },
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        IndexingUnitChangeEvent indexingUnitChangeEvent2 = eventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent1);
        prereqChangeEvents.Add(indexingUnitChangeEvent2);
      }
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Successfully queued scoped indexing unit level operation for branches ({0}),", (object) string.Join(",", (IEnumerable<string>) branchesToDeleteList))));
      IndexingUnitChangeEventPrerequisites fromChangeEvents = this.GetPrerequisitesFromChangeEvents(prereqChangeEvents);
      IndexingUnitChangeEvent indexingUnitChangeEvent = new IndexingUnitChangeEvent(leaseId)
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeType = "CompleteBranchDelete",
        ChangeData = (ChangeEventData) new CompleteGitBranchDeleteEventData((ExecutionContext) indexingExecutionContext)
        {
          Branches = branchesToDeleteList,
          CleanUpIndexingUnitData = cleanUpIndexingUnitData
        },
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = fromChangeEvents
      };
      eventHandler.HandleEvent((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Successfully queued CompleteBranchDelete for branches ({0}) for indexing unit {1}", (object) string.Join(",", (IEnumerable<string>) branchesToDeleteList), (object) indexingUnit.ToString())));
      return OperationStatus.Succeeded;
    }

    public override List<string> PerformBranchDeletionInThisOperation(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList,
      IndexIdentity searchIndex)
    {
      List<string> second = this.DeleteBranches(indexingExecutionContext, branchesToDeleteList, indexingExecutionContext.RequestContext.GetCollectionID(), searchIndex, new Func<IndexingExecutionContext, IndexIdentity, IExpression, string, bool>(((GitBranchDeleter) this).PerformBulkDelete));
      if (second != null)
      {
        List<string> list = branchesToDeleteList.Except<string>((IEnumerable<string>) second).ToList<string>();
        if (list != null && list.Any<string>())
        {
          TempAndFileMetaDataStoreDeleter tempAndFileMetaDataStoreDeleter = new TempAndFileMetaDataStoreDeleter();
          foreach (IndexingUnit scopedIndexingUnit in this.m_scopedIndexingUnits)
          {
            if (!this.DeleteDataFromTempAndFileMetadataStore(tempAndFileMetaDataStoreDeleter, indexingExecutionContext, scopedIndexingUnit, list))
            {
              second.AddRange((IEnumerable<string>) list);
              break;
            }
          }
        }
      }
      return second;
    }

    internal virtual bool DeleteDataFromTempAndFileMetadataStore(
      TempAndFileMetaDataStoreDeleter tempAndFileMetaDataStoreDeleter,
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit,
      List<string> branchesToDeleted)
    {
      return tempAndFileMetaDataStoreDeleter.DeleteTempAndFileMetaDataStoreRecords(indexingExecutionContext.RequestContext, indexingUnit, indexingExecutionContext.IndexingUnitDataAccess, branchesToDeleted);
    }

    public override OperationStatus CompleteBranchDeletion(
      IndexingExecutionContext indexingExecutionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      List<string> branchesToDeleteList,
      IndexIdentity searchIndex,
      IndexingUnit indexingUnit,
      bool cleanUpIndexingUnitData,
      StringBuilder resultMessage)
    {
      bool isESCleanUpDisabled = this.IsFeatureFlagEnabled(indexingExecutionContext);
      List<string> stringList1;
      if (isESCleanUpDisabled)
      {
        stringList1 = branchesToDeleteList;
      }
      else
      {
        List<string> second = this.DeleteBranches(indexingExecutionContext, branchesToDeleteList, indexingExecutionContext.RequestContext.GetCollectionID(), searchIndex, new Func<IndexingExecutionContext, IndexIdentity, IExpression, string, bool>(((GitBranchDeleter) this).PerformBulkDelete));
        stringList1 = branchesToDeleteList.Except<string>((IEnumerable<string>) second).ToList<string>();
      }
      List<string> stringList2 = new List<string>();
      foreach (string key in stringList1)
      {
        bool flag = false;
        foreach (IndexingUnit scopedIndexingUnit in this.m_scopedIndexingUnits)
        {
          if ((scopedIndexingUnit.Properties as ScopedGitRepositoryIndexingProperties).BranchIndexInfo.ContainsKey(key))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          stringList2.Add(key);
      }
      if (stringList2.Any<string>())
      {
        this.UpdateBranchInfoInRepositoryIndexingUnit(stringList2, indexingUnit, cleanUpIndexingUnitData, isESCleanUpDisabled);
        indexingUnit = indexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingUnit);
      }
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Total branchesToDelete ({0}) out of which successfully updated branches ({1}) from indexing unit {2}", (object) string.Join(",", (IEnumerable<string>) branchesToDeleteList), (object) string.Join(",", (IEnumerable<string>) stringList2), (object) indexingUnit.ToString())));
      return OperationStatus.Succeeded;
    }

    private void UpdateBranchInfoInRepositoryIndexingUnit(
      List<string> branchesToBeDeletedFromIndexingUnit,
      IndexingUnit indexingUnit,
      bool cleanUpIndexingUnitData,
      bool isESCleanUpDisabled)
    {
      GitCodeRepoTFSAttributes entityAttributes = indexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
      GitCodeRepoIndexingProperties properties = indexingUnit.Properties as GitCodeRepoIndexingProperties;
      if (isESCleanUpDisabled && properties.BranchesToClean == null)
        properties.BranchesToClean = new List<string>();
      foreach (string key in branchesToBeDeletedFromIndexingUnit)
      {
        if (isESCleanUpDisabled && !properties.BranchesToClean.Contains(key))
          properties.BranchesToClean.Add(key);
        if (cleanUpIndexingUnitData)
        {
          entityAttributes.Branches.Remove(key);
          properties.BranchIndexInfo.Remove(key);
          if (entityAttributes.DefaultBranch != null && entityAttributes.DefaultBranch.Equals(key, StringComparison.Ordinal))
            entityAttributes.DefaultBranch = (string) null;
        }
        else
          properties.BranchIndexInfo[key] = new GitBranchIndexInfo();
      }
    }

    private IndexingUnitChangeEventPrerequisites GetPrerequisitesFromChangeEvents(
      IList<IndexingUnitChangeEvent> prereqChangeEvents)
    {
      IndexingUnitChangeEventPrerequisites fromChangeEvents = new IndexingUnitChangeEventPrerequisites();
      foreach (IndexingUnitChangeEvent prereqChangeEvent in (IEnumerable<IndexingUnitChangeEvent>) prereqChangeEvents)
        fromChangeEvents.Add(new IndexingUnitChangeEventPrerequisitesFilter()
        {
          Id = prereqChangeEvent.Id,
          Operator = IndexingUnitChangeEventFilterOperator.Contains,
          PossibleStates = new List<IndexingUnitChangeEventState>()
          {
            IndexingUnitChangeEventState.Succeeded,
            IndexingUnitChangeEventState.Failed
          }
        });
      return fromChangeEvents;
    }

    private bool IsFeatureFlagEnabled(IndexingExecutionContext indexingExecutionContext) => indexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.Code.DisableESCleanUp");
  }
}
