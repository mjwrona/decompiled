// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git.NormalRepoGitBranchDeleter
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git
{
  public class NormalRepoGitBranchDeleter : GitBranchDeleter
  {
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
      List<string> stringList1 = this.DeleteBranches(indexingExecutionContext, branchesToDeleteList, indexingExecutionContext.RequestContext.GetCollectionID(), searchIndex, new Func<IndexingExecutionContext, IndexIdentity, IExpression, string, bool>(((GitBranchDeleter) this).PerformBulkDelete));
      List<string> stringList2 = new List<string>();
      List<string> stringList3 = stringList1 != null ? branchesToDeleteList.Except<string>((IEnumerable<string>) stringList1).ToList<string>() : throw new InvalidDataException("BranchesFailedToDelete is null");
      if (stringList3.Any<string>())
      {
        IndexingUnitChangeEvent indexingUnitChangeEvent = new IndexingUnitChangeEvent()
        {
          IndexingUnitId = indexingUnit.IndexingUnitId,
          ChangeType = "CompleteBranchDelete",
          ChangeData = (ChangeEventData) new CompleteGitBranchDeleteEventData((ExecutionContext) indexingExecutionContext)
          {
            Branches = stringList3,
            CleanUpIndexingUnitData = cleanUpIndexingUnitData
          },
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        eventHandler.HandleEvent((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Successfully queued CompleteBranchDelete for branches ({0}) ", (object) string.Join(",", (IEnumerable<string>) stringList3))));
      }
      OperationStatus operationStatus = stringList1.Any<string>() ? OperationStatus.Failed : OperationStatus.Succeeded;
      StringBuilder stringBuilder = resultMessage;
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      string str1;
      if (!stringList1.Any<string>())
        str1 = string.Empty;
      else
        str1 = FormattableString.Invariant(FormattableStringFactory.Create("Failed to delete branches ({0}) ", (object) string.Join(",", (IEnumerable<string>) stringList1)));
      string str2 = FormattableString.Invariant(FormattableStringFactory.Create("of IndexingUnit {0}", (object) indexingUnit.ToString()));
      string format = str1 + str2;
      object[] objArray = Array.Empty<object>();
      stringBuilder.AppendFormat((IFormatProvider) invariantCulture, format, objArray);
      return operationStatus;
    }

    public override List<string> PerformBranchDeletionInThisOperation(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList,
      IndexIdentity searchIndex)
    {
      return this.DeleteBranches(indexingExecutionContext, branchesToDeleteList, indexingExecutionContext.RequestContext.GetCollectionID(), searchIndex, new Func<IndexingExecutionContext, IndexIdentity, IExpression, string, bool>(((GitBranchDeleter) this).PerformBulkDelete));
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
      GitCodeRepoTFSAttributes entityAttributes = indexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
      GitCodeRepoIndexingProperties properties = indexingUnit.Properties as GitCodeRepoIndexingProperties;
      if (branchesToDeleteList != null && branchesToDeleteList.Any<string>())
      {
        if (cleanUpIndexingUnitData)
        {
          foreach (string branchesToDelete in branchesToDeleteList)
          {
            entityAttributes.Branches.Remove(branchesToDelete);
            properties.BranchIndexInfo.Remove(branchesToDelete);
            if (entityAttributes.DefaultBranch != null && entityAttributes.DefaultBranch.Equals(branchesToDelete, StringComparison.Ordinal))
              entityAttributes.DefaultBranch = (string) null;
          }
        }
        else
        {
          foreach (string branchesToDelete in branchesToDeleteList)
            properties.BranchIndexInfo[branchesToDelete] = new GitBranchIndexInfo();
        }
        indexingUnit = indexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingUnit);
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Successfully updated branches ({0}) from indexing unit {1}", (object) string.Join(",", (IEnumerable<string>) branchesToDeleteList), (object) indexingUnit.ToString())));
      }
      return OperationStatus.Succeeded;
    }
  }
}
