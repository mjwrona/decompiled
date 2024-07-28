// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git.ScopedIndexingUnitLevelGitBranchDeleter
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git
{
  public class ScopedIndexingUnitLevelGitBranchDeleter : GitBranchDeleter
  {
    public override IExpression GetQueryExpression(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList)
    {
      string str = indexingExecutionContext.IndexingUnit.GetScopePathFromTFSAttributesIfScopedIUElseNull();
      int num = str.IndexOf("/", StringComparison.Ordinal);
      if (num >= 0)
        str = str.Substring(num + 1);
      if (branchesToDeleteList.Count<string>() <= 0 || string.IsNullOrWhiteSpace(str))
        return (IExpression) new EmptyExpression();
      return (IExpression) new AndExpression((IEnumerable<IExpression>) new List<IExpression>()
      {
        (IExpression) new TermExpression("repositoryId", Operator.Equals, indexingExecutionContext.RepositoryId.ToString()),
        (IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Operator.In, (IEnumerable<string>) new List<string>()
        {
          str
        }),
        (IExpression) new TermsExpression(AbstractSearchDocumentContract.GetBranchNameOriginalFieldName(indexingExecutionContext.ProvisioningContext.ContractType), Operator.In, branchesToDeleteList.Select<string, string>((Func<string, string>) (s => CustomUtils.GetBranchNameWithoutPrefix("refs/heads/", s))))
      });
    }

    public override List<string> DeleteBranches(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList,
      Guid collectionId,
      IndexIdentity searchIndex,
      Func<IndexingExecutionContext, IndexIdentity, IExpression, string, bool> bulkDelete)
    {
      List<string> second = new List<string>();
      if (this.IsFeatureFlagEnabled(indexingExecutionContext))
      {
        this.DeleteFailedItems(indexingExecutionContext, branchesToDeleteList);
        if (!this.DeleteDataFromTempAndFileMetadataStore(indexingExecutionContext, branchesToDeleteList))
          second.AddRange((IEnumerable<string>) branchesToDeleteList);
      }
      else
      {
        second = base.DeleteBranches(indexingExecutionContext, branchesToDeleteList, collectionId, searchIndex, bulkDelete);
        if (second != null)
        {
          List<string> list = branchesToDeleteList.Except<string>((IEnumerable<string>) second).ToList<string>();
          if (list != null && list.Any<string>() && !this.DeleteDataFromTempAndFileMetadataStore(indexingExecutionContext, list))
            second.AddRange((IEnumerable<string>) list);
        }
      }
      return second;
    }

    internal virtual bool DeleteDataFromTempAndFileMetadataStore(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleted)
    {
      return new TempAndFileMetaDataStoreDeleter().DeleteTempAndFileMetaDataStoreRecords(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit, indexingExecutionContext.IndexingUnitDataAccess, branchesToDeleted);
    }

    private bool IsFeatureFlagEnabled(IndexingExecutionContext indexingExecutionContext) => indexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.Code.DisableESCleanUp");

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
      throw new NotImplementedException();
    }

    public override List<string> PerformBranchDeletionInThisOperation(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList,
      IndexIdentity searchIndex)
    {
      throw new NotImplementedException();
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
      throw new NotImplementedException();
    }
  }
}
