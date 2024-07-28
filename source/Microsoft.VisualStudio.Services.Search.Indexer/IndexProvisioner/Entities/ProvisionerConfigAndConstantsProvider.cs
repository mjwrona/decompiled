// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities.ProvisionerConfigAndConstantsProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Nest;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities
{
  public abstract class ProvisionerConfigAndConstantsProvider
  {
    public abstract IEntityType EntityType { get; }

    public AliasIdentity ActiveSharedIndexAlias(DocumentContractType contractType) => AliasIdentity.CreateAliasIdentity(FormattableString.Invariant(FormattableStringFactory.Create("{0}_{1}_{2}_active_shared_indices", (object) this.EntityType.Name, (object) contractType.GetMappingName(), (object) this.IndexVersion)).ToLowerInvariant());

    public abstract int IndexVersion { get; }

    public abstract void GetReservedSpaceInShards(
      ExecutionContext executionContext,
      IndexProvisionType provisionType,
      DocumentContractType contractType,
      long actualShardSizeInBytes,
      out long reservedSpace,
      out int reservedDocCount);

    public string GetDedicatedIndexName(DocumentContractType documentContractType, Guid tfsEntityId) => FormattableString.Invariant(FormattableStringFactory.Create("{0}_{1}_{2}_{3}_{4}_{5}", (object) this.EntityType.Name, (object) documentContractType.GetMappingName(), (object) this.IndexVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) "Dedicated", (object) tfsEntityId.ToString(), (object) Guid.NewGuid().ToString())).ToLowerInvariant();

    public string GetSharedIndexName(DocumentContractType documentContractType) => FormattableString.Invariant(FormattableStringFactory.Create("{0}_{1}_{2}_{3}_{4}", (object) this.EntityType.Name, (object) documentContractType.GetMappingName(), (object) this.IndexVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) "Shared", (object) Guid.NewGuid().ToString())).ToLowerInvariant();

    public IndexProvisionType GetIndexProvisionType(string indexName) => !indexName.Contains("shared") ? IndexProvisionType.Dedicated : IndexProvisionType.Shared;

    public abstract IndexSettings GetIndexSettings(
      ExecutionContext executionContext,
      IndexProvisionType provisionType,
      DocumentContractType contractType);

    protected void AddCommonIndexSettings(IndexSettings indexSettings)
    {
      indexSettings.UnassignedNodeLeftDelayedTimeout = (Time) "20m";
      indexSettings.Add("index.allocation.max_retries", (object) 1000);
    }

    public abstract ITypeMapping GetIndexMappings(
      IVssRequestContext requestContext,
      DocumentContractType contractType);

    public virtual bool IsEnabled(IVssRequestContext requestContext) => true;

    public abstract int GetNumberOfPrimaries(
      IVssRequestContext requestContext,
      IndexProvisionType indexProvisionType);
  }
}
