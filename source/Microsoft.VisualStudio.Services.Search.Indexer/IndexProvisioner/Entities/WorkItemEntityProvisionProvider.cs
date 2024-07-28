// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities.WorkItemEntityProvisionProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Nest;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities
{
  public sealed class WorkItemEntityProvisionProvider : ProvisionerConfigAndConstantsProvider
  {
    public override IEntityType EntityType => (IEntityType) WorkItemEntityType.GetInstance();

    public override int IndexVersion { get; } = 1490;

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
      int localeId = executionContext.RequestContext.GetLocaleId();
      IndexSettings itemIndexSettings = WorkItemIndexAttributes.GetWorkItemIndexSettings(executionContext.RequestContext, this.GetNumberOfPrimaries(executionContext.RequestContext, provisionType), provisionerSettings.Replicas, provisionerSettings.RefreshRate, localeId);
      this.AddCommonIndexSettings(itemIndexSettings);
      return itemIndexSettings;
    }

    public override ITypeMapping GetIndexMappings(
      IVssRequestContext requestContext,
      DocumentContractType contractType)
    {
      int localeId = requestContext.GetLocaleId();
      return WorkItemIndexAttributes.GetWorkItemIndexMappings(requestContext, this.IndexVersion, localeId);
    }

    public override int GetNumberOfPrimaries(
      IVssRequestContext requestContext,
      IndexProvisionType indexProvisionType)
    {
      if (indexProvisionType == IndexProvisionType.Dedicated)
        requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/DedicatedWorkItemIndexPrimaries");
      return requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemSharedIndexPrimaries", TeamFoundationHostType.Deployment, 10);
    }
  }
}
