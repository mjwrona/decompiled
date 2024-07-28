// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.SharedIndexProvisioner
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal sealed class SharedIndexProvisioner : IndexProvisionerBase
  {
    public SharedIndexProvisioner(
      IndexingExecutionContext indexingExecutionContext,
      IEntityType entityType)
      : base(indexingExecutionContext, IndexProvisionType.Shared, entityType)
    {
    }

    protected override IndexIdentity OnboardIndexingUnit(
      IndexingExecutionContext executionContext,
      ISearchPlatform searchPlatform,
      ProvisionerConfigAndConstantsProvider entityProvisionerProvider)
    {
      return executionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<ISharedIndexSelectorService>().SelectSharedIndexToOnboardAccount(executionContext, entityProvisionerProvider.EntityType);
    }

    protected override IndexIdentity MigrateIndexingUnit(
      IndexingExecutionContext executionContext,
      ISearchPlatform searchPlatform,
      ProvisionerConfigAndConstantsProvider entityProvisionerProvider,
      IndexIdentity indexToSkip)
    {
      return executionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<ISharedIndexSelectorService>().SelectSharedIndexToOnboardAccount(executionContext, executionContext.IndexingUnit.EntityType, indexToSkip);
    }
  }
}
