// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.GitRepositoryCodeUpdateMetadataOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class GitRepositoryCodeUpdateMetadataOperation : UpdateMetadataOperation
  {
    public GitRepositoryCodeUpdateMetadataOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    internal override bool HandleOperation(
      IndexingExecutionContext executionContext,
      UpdateMetadataEventData eventData,
      StringBuilder resultMessage)
    {
      if (eventData.UpdateType != UpdateMetaDataOperationType.ClearJobYieldData)
        return false;
      GitCodeRepoIndexingProperties properties = (GitCodeRepoIndexingProperties) this.IndexingUnit.Properties;
      foreach (string key in properties.BranchIndexInfo.Keys.ToList<string>())
      {
        GitBranchIndexInfo gitBranchIndexInfo = properties.BranchIndexInfo[key];
        if (gitBranchIndexInfo != null)
        {
          gitBranchIndexInfo.BulkIndexJobYieldData = new GitBulkIndexJobYieldData();
          gitBranchIndexInfo.GitIndexJobYieldData = new GitIndexJobYieldData();
        }
      }
      properties.GitIndexJobYieldStats = new GitIndexJobYieldStats();
      this.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, this.IndexingUnit);
      resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Added operation to cleanup JobYieldData of {0}", (object) this.IndexingUnit.ToString())));
      return true;
    }
  }
}
