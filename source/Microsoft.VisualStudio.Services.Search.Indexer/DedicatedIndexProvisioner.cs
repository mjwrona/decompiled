// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.DedicatedIndexProvisioner
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using System;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal abstract class DedicatedIndexProvisioner : IndexProvisionerBase
  {
    public DedicatedIndexProvisioner(
      IndexingExecutionContext indexingExecutionContext,
      IndexProvisionType provisionerType,
      IEntityType entityType)
      : base(indexingExecutionContext, provisionerType, entityType)
    {
    }

    protected IndexIdentity GetMigrateIndexIdentity(
      IndexingExecutionContext indexingExecutionContext,
      ISearchPlatform searchPlatform,
      ProvisionerConfigAndConstantsProvider entityProvisionProvider,
      Guid entityId)
    {
      DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
      IndexIdentity indexIdentity;
      do
      {
        indexIdentity = IndexIdentity.CreateIndexIdentity(entityProvisionProvider.GetDedicatedIndexName(contractType, entityId));
      }
      while (searchPlatform.IndexExists((ExecutionContext) indexingExecutionContext, indexIdentity));
      return indexIdentity;
    }
  }
}
