// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git.GitBranchDeleter
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git
{
  public abstract class GitBranchDeleter
  {
    protected const int s_tracePoint = 1080624;
    protected const string s_traceArea = "Indexing Pipeline";
    protected const string s_traceLayer = "IndexingOperation";

    public bool PerformBulkDelete(
      IndexingExecutionContext executionContext,
      IndexIdentity searchIndex,
      IExpression query,
      string repositoryId)
    {
      IndexUpdaterParams indexUpdaterParams = new IndexUpdaterParams()
      {
        ContractType = executionContext.ProvisioningContext.ContractType,
        IndexSubScope = new IndexSubScope()
        {
          AccountId = executionContext.RequestContext.GetOrganizationID().ToString(),
          CollectionId = executionContext.RequestContext.GetCollectionID().ToString()
        },
        IndexIdentity = searchIndex
      };
      return IndexerFactory.CreateIndexUpdater(executionContext, indexUpdaterParams, executionContext.ProvisioningContext.SearchPlatform, executionContext.GetRouteLevel()).DeleteDocumentsByQuery(query, true).Success;
    }

    public virtual IExpression GetQueryExpression(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList)
    {
      if (branchesToDeleteList.Count<string>() <= 0)
        return (IExpression) new EmptyExpression();
      return (IExpression) new AndExpression((IEnumerable<IExpression>) new List<IExpression>()
      {
        (IExpression) new TermExpression("repositoryId", Operator.Equals, indexingExecutionContext.RepositoryId.ToString()),
        (IExpression) new TermsExpression(AbstractSearchDocumentContract.GetBranchNameOriginalFieldName(indexingExecutionContext.ProvisioningContext.ContractType), Operator.In, branchesToDeleteList.Select<string, string>((Func<string, string>) (s => CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", s))))
      });
    }

    public abstract OperationStatus PerformBranchDeletion(
      IndexingExecutionContext indexingExecutionContext,
      IIndexingUnitChangeEventHandler eventHandler,
      List<string> branchesToDeleteList,
      IndexIdentity searchIndex,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      bool cleanUpIndexingUnitData,
      string leaseId,
      StringBuilder resultMessage);

    public abstract List<string> PerformBranchDeletionInThisOperation(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList,
      IndexIdentity searchIndex);

    public abstract OperationStatus CompleteBranchDeletion(
      IndexingExecutionContext indexingExecutionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      List<string> branchesToDeleteList,
      IndexIdentity searchIndex,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      bool cleanUpIndexingUnitData,
      StringBuilder resultMessage);

    public virtual List<string> DeleteBranches(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList,
      Guid collectionId,
      IndexIdentity searchIndex,
      Func<IndexingExecutionContext, IndexIdentity, IExpression, string, bool> bulkDelete)
    {
      List<string> stringList = new List<string>();
      if (!indexingExecutionContext.RepositoryId.HasValue)
        throw new ArgumentException("RepositoryId in execution context is null", nameof (indexingExecutionContext));
      if (indexingExecutionContext.ProjectName == null)
        throw new ArgumentException("ProjectName in execution context is null", nameof (indexingExecutionContext));
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Index : '{0}' for Repository {1} and Branch '{2}'", (object) searchIndex, (object) indexingExecutionContext.RepositoryId, (object) string.Join(",", (IEnumerable<string>) branchesToDeleteList));
      bool flag = false;
      try
      {
        this.DeleteFailedItems(indexingExecutionContext, branchesToDeleteList);
        IExpression queryExpression = this.GetQueryExpression(indexingExecutionContext, branchesToDeleteList);
        if (queryExpression is EmptyExpression)
          flag = true;
        else if (bulkDelete(indexingExecutionContext, searchIndex, queryExpression, indexingExecutionContext.RepositoryId.ToString()))
        {
          Tracer.TraceInfo(1080624, "Indexing Pipeline", "IndexingOperation", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deletion of all documents from {0} succeeded.", (object) str));
          flag = true;
        }
        else
        {
          Tracer.TraceError(1080624, "Indexing Pipeline", "IndexingOperation", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Document deletion failed from {0}.", (object) str));
          indexingExecutionContext.Log.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Document deletion failed from {0}.", (object) str));
        }
      }
      catch (Exception ex)
      {
        Tracer.TraceError(1080624, "Indexing Pipeline", "IndexingOperation", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deletion of documents from {0} failed with exception: {1}", (object) str, (object) ex));
        indexingExecutionContext.Log.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deletion of documents from {0} failed with exception: {1}", (object) str, (object) ex));
      }
      finally
      {
        if (!flag)
          stringList.AddRange((IEnumerable<string>) branchesToDeleteList);
      }
      return stringList;
    }

    internal virtual void DeleteFailedItems(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branches)
    {
      int thresholdForMaxDocs = indexingExecutionContext.ServiceSettings.JobSettings.PatchInMemoryThresholdForMaxDocs;
      using (List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>.Enumerator enumerator = this.GetIndexingUnitsForFailedItems(indexingExecutionContext).GetEnumerator())
      {
label_21:
        while (enumerator.MoveNext())
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit current = enumerator.Current;
          long startingId = 0;
          IItemLevelFailureDataAccess failureDataAccess = indexingExecutionContext.ItemLevelFailureDataAccess;
          while (true)
          {
            List<ItemLevelFailureRecord> list = failureDataAccess.GetFailedItems(indexingExecutionContext.RequestContext, current, thresholdForMaxDocs, startingId).ToList<ItemLevelFailureRecord>();
            if (list.Any<ItemLevelFailureRecord>())
            {
              List<ItemLevelFailureRecord> successfullyIndexedRecords = new List<ItemLevelFailureRecord>();
              foreach (ItemLevelFailureRecord levelFailureRecord1 in list)
              {
                Branches source = new Branches();
                foreach (string branch in branches)
                {
                  if ((levelFailureRecord1.Metadata as FileFailureMetadata).Branches.Contains(branch))
                    source.Add(branch);
                }
                if (source.Any<string>())
                {
                  ItemLevelFailureRecord levelFailureRecord2 = new ItemLevelFailureRecord()
                  {
                    Id = levelFailureRecord1.Id,
                    Item = levelFailureRecord1.Item,
                    Metadata = (FailureMetadata) new FileFailureMetadata()
                    {
                      Branches = source
                    }
                  };
                  successfullyIndexedRecords.Add(levelFailureRecord2);
                }
                if (startingId < levelFailureRecord1.Id)
                  startingId = levelFailureRecord1.Id;
              }
              failureDataAccess.RemoveSuccessfullyIndexedItemsFromFailedRecords(indexingExecutionContext.RequestContext, current, (IEnumerable<ItemLevelFailureRecord>) successfullyIndexedRecords);
            }
            if (list.Count >= thresholdForMaxDocs)
              ++startingId;
            else
              goto label_21;
          }
        }
      }
    }

    protected internal virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetIndexingUnitsForFailedItems(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>()
      {
        indexingExecutionContext.IndexingUnit
      };
    }
  }
}
