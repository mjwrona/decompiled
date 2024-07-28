// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.ScopedIndexingUnitBranchDeleteOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class ScopedIndexingUnitBranchDeleteOperation : AbstractIndexingOperation
  {
    public ScopedIndexingUnitBranchDeleteOperation(
      CoreIndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base((ExecutionContext) indexingExecutionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(1083047, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder1 = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      bool currentHostConfigValue = coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsGitBranchDeleteOperationDisabled", true);
      try
      {
        if (!currentHostConfigValue && this.ValidateCIFeatureFlags(executionContext))
        {
          GitBranchDeleteEventData changeData = (GitBranchDeleteEventData) this.IndexingUnitChangeEvent.ChangeData;
          List<string> stringList1 = new List<string>();
          List<string> stringList2 = new List<string>();
          List<string> stringList3 = new List<string>();
          GitCodeRepoTFSAttributes repoAttributes = executionContext.RepositoryIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
          GitCodeRepoIndexingProperties properties = executionContext.RepositoryIndexingUnit.Properties as GitCodeRepoIndexingProperties;
          if (changeData.Branches == null)
          {
            stringList1.AddRange((IEnumerable<string>) properties.BranchIndexInfo.Keys.ToList<string>().FindAll((Predicate<string>) (x => !repoAttributes.BranchesToIndex.Contains(x))));
          }
          else
          {
            foreach (string branch in changeData.Branches)
            {
              if (!string.IsNullOrWhiteSpace(branch))
                stringList1.Add(branch);
            }
          }
          if (stringList1.Count > 0)
          {
            List<string> stringList4 = this.DeleteBranches(executionContext, stringList1);
            List<string> stringList5 = stringList4 != null ? stringList1.Except<string>((IEnumerable<string>) stringList4).ToList<string>() : throw new InvalidDataException("BranchesFailedToDelete is null");
            operationResult.Status = stringList4.Any<string>() ? OperationStatus.Failed : OperationStatus.Succeeded;
            StringBuilder stringBuilder2 = stringBuilder1;
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            string str1;
            if (!stringList4.Any<string>())
              str1 = string.Empty;
            else
              str1 = FormattableString.Invariant(FormattableStringFactory.Create("Failed to delete branches ({0}) ", (object) string.Join(",", (IEnumerable<string>) stringList4)));
            string str2;
            if (!stringList5.Any<string>())
              str2 = string.Empty;
            else
              str2 = FormattableString.Invariant(FormattableStringFactory.Create("Successfully deleted branches ({0}) ", (object) string.Join(",", (IEnumerable<string>) stringList5)));
            string str3 = FormattableString.Invariant(FormattableStringFactory.Create("of IndexingUnit {0}", (object) this.IndexingUnit.ToString()));
            string format = str1 + str2 + str3;
            object[] objArray = Array.Empty<object>();
            stringBuilder2.AppendFormat((IFormatProvider) invariantCulture, format, objArray);
          }
          else
          {
            operationResult.Status = OperationStatus.Succeeded;
            stringBuilder1.Append(FormattableString.Invariant(FormattableStringFactory.Create("There are no branches to be deleted from IndexingUnit {0} CollectionId {1}.", (object) this.IndexingUnit.ToString(), (object) coreIndexingExecutionContext.RequestContext.GetCollectionID())));
          }
        }
        else
        {
          operationResult.Status = OperationStatus.Succeeded;
          stringBuilder1.Append(FormattableString.Invariant(FormattableStringFactory.Create("Branch Deletion Operation is disabled for {0}", (object) this.IndexingUnit.ToString())));
        }
      }
      finally
      {
        operationResult.Message = stringBuilder1.ToString();
        Tracer.TraceLeave(1083047, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual List<string> DeleteBranches(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList)
    {
      IndexIdentity index = indexingExecutionContext.GetIndex();
      ScopedIndexingUnitLevelGitBranchDeleter gitBranchDeleter = new ScopedIndexingUnitLevelGitBranchDeleter();
      return gitBranchDeleter.DeleteBranches(indexingExecutionContext, branchesToDeleteList, indexingExecutionContext.RequestContext.GetCollectionID(), index, new Func<IndexingExecutionContext, IndexIdentity, IExpression, string, bool>(((GitBranchDeleter) gitBranchDeleter).PerformBulkDelete));
    }
  }
}
