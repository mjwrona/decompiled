// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.CodeGitRepoResetBranchInfoOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class CodeGitRepoResetBranchInfoOperation : AbstractIndexingOperation
  {
    public CodeGitRepoResetBranchInfoOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(CoreIndexingExecutionContext executionContext)
    {
      Tracer.TraceEnter(1080636, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        List<string> branches = ((GitCodeRepoTFSAttributes) this.IndexingUnit.TFSEntityAttributes).Branches;
        string defaultBranch = ((GitCodeRepoTFSAttributes) this.IndexingUnit.TFSEntityAttributes).DefaultBranch;
        Dictionary<string, GitBranchIndexInfo> branchIndexInfo = ((GitCodeRepoIndexingProperties) this.IndexingUnit.Properties).BranchIndexInfo;
        if (branches != null)
        {
          branches.RemoveAll((Predicate<string>) (branch => branch == defaultBranch || string.IsNullOrWhiteSpace(branch)));
          string message = FormattableString.Invariant(FormattableStringFactory.Create("Deleting the branches ({0}) from Repo {1} having defaultBranch : {2}.", (object) string.Join(",", (IEnumerable<string>) branches), (object) this.IndexingUnit.ToString(), (object) defaultBranch));
          if (branchIndexInfo != null)
          {
            Tracer.TraceInfo(1080636, "Indexing Pipeline", "IndexingOperation", message);
            foreach (string key in branches)
            {
              if (branchIndexInfo.ContainsKey(key))
                branchIndexInfo.Remove(key);
            }
          }
          branches.Clear();
          this.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, this.IndexingUnit);
          stringBuilder.Append(message);
        }
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Tracer.TraceLeave(1080636, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }
  }
}
