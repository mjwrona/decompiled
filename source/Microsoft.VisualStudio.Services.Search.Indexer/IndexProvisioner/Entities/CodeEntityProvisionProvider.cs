// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities.CodeEntityProvisionProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Nest;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities
{
  public sealed class CodeEntityProvisionProvider : ProvisionerConfigAndConstantsProvider
  {
    public override IEntityType EntityType => (IEntityType) CodeEntityType.GetInstance();

    public override int IndexVersion { get; } = 1421;

    public override void GetReservedSpaceInShards(
      ExecutionContext executionContext,
      IndexProvisionType provisionType,
      DocumentContractType contractType,
      long actualShardSizeInBytes,
      out long reservedSpace,
      out int reservedDocCount)
    {
      if (provisionType == IndexProvisionType.Shared)
      {
        long currentHostConfigValue1 = executionContext.RequestContext.GetCurrentHostConfigValue<long>("/Service/ALMSearch/Settings/Routing/Code/ReservedSpaceForShards", true, 10L);
        long currentHostConfigValue2 = executionContext.RequestContext.GetCurrentHostConfigValue<long>("/Service/ALMSearch/Settings/Routing/Code/MaxShardSizeInBytes", true, 42949672960L);
        if (actualShardSizeInBytes < currentHostConfigValue2 - currentHostConfigValue1 * 1024L * 1024L * 1024L)
        {
          reservedSpace = currentHostConfigValue1 * 1024L * 1024L * 1024L;
          reservedDocCount = contractType.GetEstimatedDocumentCountFromEstimatedSize(executionContext.RequestContext, reservedSpace);
        }
        else if (actualShardSizeInBytes < currentHostConfigValue2)
        {
          reservedSpace = currentHostConfigValue2 - actualShardSizeInBytes;
          reservedDocCount = contractType.GetEstimatedDocumentCountFromEstimatedSize(executionContext.RequestContext, reservedSpace);
        }
        else
        {
          reservedSpace = 0L;
          reservedDocCount = 0;
        }
      }
      else
      {
        reservedDocCount = 0;
        reservedSpace = 0L;
      }
    }

    public override IndexSettings GetIndexSettings(
      ExecutionContext executionContext,
      IndexProvisionType provisionType,
      DocumentContractType contractType)
    {
      ProvisionerSettings provisionerSettings = executionContext.ServiceSettings.ProvisionerSettings;
      IndexSettings codeIndexSettings = CodeFileContract.GetContractInstance(contractType).GetCodeIndexSettings(executionContext, this.GetNumberOfPrimaries(executionContext.RequestContext, provisionType), provisionerSettings.Replicas, provisionerSettings.RefreshRate);
      this.AddCommonIndexSettings(codeIndexSettings);
      return codeIndexSettings;
    }

    public override ITypeMapping GetIndexMappings(
      IVssRequestContext requestContext,
      DocumentContractType contractType)
    {
      return CodeFileContract.GetCodeIndexMappings(requestContext, contractType, this.IndexVersion);
    }

    public override int GetNumberOfPrimaries(
      IVssRequestContext requestContext,
      IndexProvisionType indexProvisionType)
    {
      return indexProvisionType == IndexProvisionType.Dedicated ? requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/DedicatedIndexPrimaries") : requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CodeSharedIndexPrimaries", TeamFoundationHostType.Deployment, 12);
    }
  }
}
