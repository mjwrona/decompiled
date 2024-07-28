// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.WikiRepositoryUpdateFieldOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class WikiRepositoryUpdateFieldOperation : RepositoryUpdateFieldOperation
  {
    public WikiRepositoryUpdateFieldOperation(
      CoreIndexingExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    protected internal override AbstractSearchDocumentContract GetPartialUpdatedDocument(
      IndexingExecutionContext executionContext,
      string indexingUnitType,
      DocumentContractType contractType)
    {
      WikiContract partialUpdatedDocument = new WikiContract();
      switch (indexingUnitType)
      {
        case "Collection":
          partialUpdatedDocument.CollectionName = executionContext.CollectionName;
          break;
        case "Project":
          partialUpdatedDocument.ProjectName = executionContext.ProjectName;
          partialUpdatedDocument.ProjectVisibility = (executionContext.ProjectIndexingUnit.Properties as ProjectCodeIndexingProperties).ProjectVisibility.ToString();
          break;
        case "Git_Repository":
          partialUpdatedDocument.RepoName = executionContext.RepositoryName;
          break;
      }
      return (AbstractSearchDocumentContract) partialUpdatedDocument;
    }
  }
}
