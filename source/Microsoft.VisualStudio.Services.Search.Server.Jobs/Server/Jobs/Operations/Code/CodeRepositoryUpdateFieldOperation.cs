// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CodeRepositoryUpdateFieldOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CodeRepositoryUpdateFieldOperation : RepositoryUpdateFieldOperation
  {
    public CodeRepositoryUpdateFieldOperation(
      CoreIndexingExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    protected override IndexInfo SelectIndexInfo(
      IndexingExecutionContext executionContext,
      IEnumerable<IndexInfo> indexInfos)
    {
      return indexInfos.Count<IndexInfo>() > 1 && executionContext.IndexingUnit.IsLargeRepository(executionContext.RequestContext) ? indexInfos.Where<IndexInfo>((Func<IndexInfo, bool>) (i => i.EntityName.Equals(executionContext.IndexingUnit.GetRepositoryNameFromTFSAttributes(), StringComparison.OrdinalIgnoreCase))).First<IndexInfo>() : base.SelectIndexInfo(executionContext, indexInfos);
    }

    protected internal override AbstractSearchDocumentContract GetPartialUpdatedDocument(
      IndexingExecutionContext executionContext,
      string indexingUnitType,
      DocumentContractType contractType)
    {
      CodeFileContract codeContract = CodeFileContract.CreateCodeContract(contractType, executionContext.ProvisioningContext.SearchPlatform);
      if (contractType.IsDedupeFileContract())
        ((DedupeFileContractBase) codeContract).Paths = (FileGroup) null;
      switch (indexingUnitType)
      {
        case "Collection":
          codeContract.CollectionName = executionContext.CollectionName.NormalizeString();
          codeContract.CollectionNameOriginal = executionContext.CollectionName;
          break;
        case "Project":
          if (contractType.IsNoPayloadContract())
          {
            codeContract.ProjectName = executionContext.ProjectName;
          }
          else
          {
            codeContract.ProjectName = executionContext.ProjectName.NormalizeString();
            codeContract.ProjectNameOriginal = executionContext.ProjectName;
          }
          if ("TFVC_Repository" == this.IndexingUnit.IndexingUnitType)
          {
            codeContract.RepoName = "$/" + executionContext.ProjectName.NormalizeString();
            codeContract.RepoNameOriginal = "$/" + executionContext.ProjectName;
            break;
          }
          break;
        case "Git_Repository":
          if (contractType.IsNoPayloadContract())
          {
            codeContract.RepoName = executionContext.RepositoryName;
            break;
          }
          codeContract.RepoName = executionContext.RepositoryName.NormalizeString();
          codeContract.RepoNameOriginal = executionContext.RepositoryName;
          break;
      }
      return (AbstractSearchDocumentContract) codeContract;
    }
  }
}
