// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities.BoardEntityProvisionProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board;
using Nest;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities
{
  public sealed class BoardEntityProvisionProvider : ProvisionerConfigAndConstantsProvider
  {
    public override IEntityType EntityType => (IEntityType) BoardEntityType.GetInstance();

    public override int IndexVersion { get; } = 1551;

    public override bool IsEnabled(IVssRequestContext requestContext) => true;

    public override void GetReservedSpaceInShards(
      ExecutionContext executionContext,
      IndexProvisionType provisionType,
      DocumentContractType contractType,
      long actualShardSizeInBytes,
      out long reservedSpace,
      out int reservedDocCount)
    {
      reservedDocCount = 0;
      reservedSpace = 0L;
    }

    public override IndexSettings GetIndexSettings(
      ExecutionContext executionContext,
      IndexProvisionType provisionType,
      DocumentContractType contractType)
    {
      ProvisionerSettings provisionerSettings = executionContext.ServiceSettings.ProvisionerSettings;
      IndexSettings boardIndexSettings = BoardIndexAttributes.GetBoardIndexSettings(executionContext, this.GetNumberOfPrimaries(executionContext.RequestContext, provisionType), provisionerSettings.Replicas, provisionerSettings.RefreshRate);
      this.AddCommonIndexSettings(boardIndexSettings);
      return boardIndexSettings;
    }

    public override ITypeMapping GetIndexMappings(
      IVssRequestContext requestContext,
      DocumentContractType contractType)
    {
      return BoardIndexAttributes.GetBoardIndexMappings(this.IndexVersion);
    }

    public override int GetNumberOfPrimaries(
      IVssRequestContext requestContext,
      IndexProvisionType indexProvisionType)
    {
      return requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BoardSharedIndexPrimaries", TeamFoundationHostType.Deployment, 1);
    }
  }
}
