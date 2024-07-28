// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner.EntitySharedIndexSelector
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Indexer.IndexProvisioner.SharedIndexProvisioner
{
  internal class EntitySharedIndexSelector
  {
    [StaticSafe]
    private static readonly Random s_rand = new Random();
    private readonly object m_lock = new object();
    protected readonly ISearchPlatform SearchPlatform;
    private readonly ISearchClusterManagementService m_searchClusterManagementService;
    private readonly ElasticsearchFeedbackProcessor m_elasticsearchFeedbackProcessor;
    private readonly IDataAccessFactory m_dataAccessFactory;
    private readonly int m_activeIndicesCount;
    private readonly ProvisionerConfigAndConstantsProvider m_provisionProvider;
    private readonly CreateSearchIndexHelper m_createSearchIndexHelper;
    private int m_indexNumberForNextAccountOnboarding;
    private readonly IDictionary<DocumentContractType, ISet<string>> m_activeIndices;

    [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Temporarily suppressing as the violation is due to virtual calls from DataAccessFactory APIs. Need more refactoring across projects.")]
    public EntitySharedIndexSelector(
      ExecutionContext executionContext,
      ProvisionerConfigAndConstantsProvider provider,
      ISearchPlatform searchPlatform,
      ISearchClusterManagementService searchClusterManagementService,
      IDataAccessFactory dataAccessFactory,
      CreateSearchIndexHelper createSearchIndexHelper)
    {
      this.m_provisionProvider = provider;
      this.SearchPlatform = searchPlatform;
      this.m_searchClusterManagementService = searchClusterManagementService;
      this.m_elasticsearchFeedbackProcessor = new ElasticsearchFeedbackProcessor();
      this.m_dataAccessFactory = dataAccessFactory;
      this.m_createSearchIndexHelper = createSearchIndexHelper;
      this.m_activeIndices = (IDictionary<DocumentContractType, ISet<string>>) new FriendlyDictionary<DocumentContractType, ISet<string>>();
      foreach (DocumentContractType supportedContractType in provider.EntityType.GetSupportedContractTypes())
        this.m_activeIndices.Add(supportedContractType, (ISet<string>) new HashSet<string>());
      this.m_activeIndicesCount = executionContext.ServiceSettings.ProvisionerSettings.ActiveIndicesCount;
      lock (this.m_lock)
        this.m_indexNumberForNextAccountOnboarding = EntitySharedIndexSelector.s_rand.Next(this.m_activeIndicesCount);
      this.InitializeIndices(executionContext);
    }

    public IndexIdentity SelectSharedIndexToOnboardAccount(
      IndexingExecutionContext indexingExecutionContext,
      IndexIdentity indexToSkip)
    {
      DocumentContractType contractType = indexingExecutionContext.ProvisioningContext.ContractType;
      if (!this.ValidateSharedAlias((ExecutionContext) indexingExecutionContext, contractType))
        this.InitializeIndices((ExecutionContext) indexingExecutionContext);
      IndexIdentity onboardAccount = this.GetNextIndexIdentity(contractType);
      if (indexToSkip != null)
      {
        int num = 0;
        while (indexToSkip.Equals((object) onboardAccount))
        {
          onboardAccount = this.GetNextIndexIdentity(contractType);
          if (++num > this.m_activeIndicesCount * 3)
            break;
        }
      }
      if (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/AlwaysCreateNewSharedIndex", true) || this.IsSharedIndexFull(indexingExecutionContext, onboardAccount) || onboardAccount.Equals((object) indexToSkip))
        onboardAccount = this.CreateNewActiveIndex((ExecutionContext) indexingExecutionContext, onboardAccount, contractType);
      int accountsOnboarded = this.GetNumberOfAccountsOnboarded(indexingExecutionContext, onboardAccount);
      indexingExecutionContext.ExecutionTracerContext.PublishCi("Indexing Pipeline", "Indexing Pipeline", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["IndexName"] = (object) onboardAccount.Name,
        ["NumberOfAccounts"] = (object) accountsOnboarded
      });
      return onboardAccount;
    }

    internal virtual bool IsSharedIndexFull(
      IndexingExecutionContext indexingExecutionContext,
      IndexIdentity sharedIndexIdentity)
    {
      if (indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>(indexingExecutionContext.IndexingUnit.EntityType.GetESFeedbackLoopRegKey(), true))
      {
        try
        {
          long indexSizeInBytes = this.m_searchClusterManagementService.GetIndexSizeInBytes((ExecutionContext) indexingExecutionContext, sharedIndexIdentity.Name);
          int numPrimaryShards = this.SearchPlatform.GetIndex(sharedIndexIdentity).GetSettings().NumberOfShards.Value;
          if (this.m_elasticsearchFeedbackProcessor.IsIndexOversized(indexingExecutionContext.RequestContext, indexSizeInBytes, indexingExecutionContext.IndexingUnit.EntityType, numPrimaryShards))
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Indexer", "IndexOverSized", (object) sharedIndexIdentity.Name, true);
            return true;
          }
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1083032, "Indexing Pipeline", "Indexer", ex);
        }
      }
      return this.GetNumberOfAccountsOnboarded(indexingExecutionContext, sharedIndexIdentity) >= indexingExecutionContext.ServiceSettings.ProvisionerSettings.MaxAccountsPerIndex;
    }

    [Info("InternalForTestPurpose")]
    internal virtual IndexIdentity GetNextIndexIdentity(DocumentContractType documentContractType) => IndexIdentity.CreateIndexIdentity(this.m_activeIndices[documentContractType].ElementAt<string>(this.IncrementAndGetIndexNumberForOnboarding()));

    [Info("InternalForTestPurpose")]
    internal ISet<string> GetActiveIndices(DocumentContractType documentContractType) => this.m_activeIndices[documentContractType];

    [Info("InternalForTestPurpose")]
    internal void InitializeIndices(ExecutionContext executionContext)
    {
      foreach (DocumentContractType supportedContractType in this.m_provisionProvider.EntityType.GetSupportedContractTypes())
      {
        if (!this.ValidateSharedAlias(executionContext, supportedContractType))
          this.CreateActiveIndices(executionContext, supportedContractType);
      }
    }

    private int GetNumberOfAccountsOnboarded(
      IndexingExecutionContext indexingExecutionContext,
      IndexIdentity indexIdentity)
    {
      int accountsOnboarded = -1;
      try
      {
        accountsOnboarded = this.SearchPlatform.GetIndex(indexIdentity).GetNumberOfCollections(indexingExecutionContext.ProvisioningContext.ContractType);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082247, "Indexing Pipeline", "Indexer", ex);
      }
      return accountsOnboarded;
    }

    private IndexIdentity CreateNewActiveIndex(
      ExecutionContext executionContext,
      IndexIdentity sharedIndexIdentity,
      DocumentContractType contractType)
    {
      IndexIdentity newSharedIndexIdentity = this.CreateSharedIndexAndRecordShardDetails(executionContext, contractType);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080201, "Indexing Pipeline", "Indexer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SharedIndexProvisioner::InitializeIndex: Created Index: {0}", (object) newSharedIndexIdentity.Name));
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        GenericInvoker.Instance.InvokeWithFaultCheck<object>((Func<object>) (() =>
        {
          EntitySharedIndexSelector.SwapSharedIndexAliases(executionContext, this.SearchPlatform, sharedIndexIdentity, newSharedIndexIdentity, this.m_provisionProvider.ActiveSharedIndexAlias(contractType));
          return (object) null;
        }), executionContext.FaultService, 2, 1000, new TraceMetaData(1080217, "Indexing Pipeline", "Indexer"));
      }
      catch (Exception ex)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "New shared index {0} created. Swapping the sharedindex alias failed", (object) newSharedIndexIdentity.Name);
        try
        {
          this.SearchPlatform.DeleteIndex(executionContext, newSharedIndexIdentity);
        }
        catch
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080218, "Indexing Pipeline", "Indexer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SwapAlias failed. Attempted to delete the newly created shared index {0}. Failed to delete the same", (object) newSharedIndexIdentity.Name));
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "New shared index {0} created. Swapping the sharedindex alias failed. Attempt to delete the newly created shared index also failed", (object) newSharedIndexIdentity.Name);
        }
        TeamFoundationEventLog.Default.Log(stringBuilder.ToString(), SearchEventId.NewSharedIndexCreationFailed, EventLogEntryType.Warning);
        throw new IndexProvisionException("IndexProvision failed: ", ex);
      }
      this.m_activeIndices[contractType].Add(newSharedIndexIdentity.Name);
      this.m_activeIndices[contractType].Remove(sharedIndexIdentity.Name);
      return newSharedIndexIdentity;
    }

    private int IncrementAndGetIndexNumberForOnboarding()
    {
      lock (this.m_lock)
      {
        int accountOnboarding = this.m_indexNumberForNextAccountOnboarding;
        this.m_indexNumberForNextAccountOnboarding = (this.m_indexNumberForNextAccountOnboarding + 1) % this.m_activeIndicesCount;
        return accountOnboarding;
      }
    }

    private void CreateActiveIndices(
      ExecutionContext executionContext,
      DocumentContractType contractType)
    {
      this.m_activeIndices[contractType] = (ISet<string>) new HashSet<string>();
      for (int index = 0; index < this.m_activeIndicesCount; ++index)
      {
        IndexIdentity recordShardDetails = this.CreateSharedIndexAndRecordShardDetails(executionContext, contractType);
        this.m_activeIndices[contractType].Add(recordShardDetails.Name);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080201, "Indexing Pipeline", "Indexer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SharedIndexProvisioner::InitializeIndex: Created Index: {0}", (object) recordShardDetails.Name));
      }
      if (this.SearchPlatform.AliasExists(executionContext, ElasticSearchPlatformSettings.AllIndices, this.m_provisionProvider.ActiveSharedIndexAlias(contractType)))
        return;
      foreach (string indexIdentity in (IEnumerable<string>) this.m_activeIndices[contractType])
        this.SearchPlatform.CreateAliasPointingToMultipleIndices(executionContext, new List<Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias>()
        {
          new Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias(indexIdentity, this.m_provisionProvider.ActiveSharedIndexAlias(contractType).Name, (string) null, (string) null, (IDictionary<string, List<string>>) null)
        });
    }

    private bool ValidateSharedAlias(
      ExecutionContext executionContext,
      DocumentContractType contractType)
    {
      AliasIdentity aliasIdentity = this.m_provisionProvider.ActiveSharedIndexAlias(contractType);
      GetAliasResponse aliases = this.SearchPlatform.GetAliases(executionContext, aliasIdentity);
      if (aliases == null)
        return false;
      foreach (IndexName key in aliases.Indices.Keys)
      {
        if (!this.SearchPlatform.IndexExists(executionContext, IndexIdentity.CreateIndexIdentity(key.Name)))
          throw new IndexProvisionException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Alias: {0} index: {1} doesn't exist", (object) aliasIdentity, (object) key));
        foreach (DocumentContractType documentContractType in this.GetDocumentContractTypes(executionContext, key.Name))
          this.m_activeIndices[documentContractType].Add(key.Name);
      }
      ISet<string> activeIndex = this.m_activeIndices[contractType];
      int count = activeIndex.Count;
      if (count < this.m_activeIndicesCount)
      {
        for (int index = count; index < this.m_activeIndicesCount; ++index)
        {
          IndexIdentity recordShardDetails = this.CreateSharedIndexAndRecordShardDetails(executionContext, contractType);
          Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias alias = new Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias(recordShardDetails.Name, aliasIdentity.Name, (string) null, (string) null, (IDictionary<string, List<string>>) null);
          this.SearchPlatform.CreateAliasPointingToMultipleIndices(executionContext, new List<Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias>()
          {
            alias
          });
          activeIndex.Add(recordShardDetails.Name);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080201, "Indexing Pipeline", "Indexer", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SharedIndexProvisioner::InitializeIndex: Created Index: {0}", (object) recordShardDetails.Name));
        }
      }
      return true;
    }

    private IndexIdentity CreateSharedIndexAndRecordShardDetails(
      ExecutionContext executionContext,
      DocumentContractType documentContractType)
    {
      IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(this.m_provisionProvider.GetSharedIndexName(documentContractType));
      this.m_createSearchIndexHelper.CreateIndex(executionContext, indexIdentity, this.SearchPlatform, this.m_searchClusterManagementService, this.m_dataAccessFactory, IndexProvisionType.Shared, documentContractType, this.m_provisionProvider);
      return indexIdentity;
    }

    internal virtual IEnumerable<DocumentContractType> GetDocumentContractTypes(
      ExecutionContext executionContext,
      string indexName)
    {
      return this.SearchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(indexName)).GetDocumentContracts(executionContext);
    }

    private static void SwapSharedIndexAliases(
      ExecutionContext executionContext,
      ISearchPlatform searchPlatform,
      IndexIdentity oldSharedIndexIdentity,
      IndexIdentity newSharedIndexIdentity,
      AliasIdentity activeIndexAlias)
    {
      List<Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias> aliasAddDescriptors = new List<Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias>();
      List<Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias> aliasRemoveDescriptors = new List<Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias>();
      StringBuilder stringBuilder = new StringBuilder();
      aliasAddDescriptors.Add(new Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias()
      {
        Identity = activeIndexAlias,
        IndexIdentity = newSharedIndexIdentity
      });
      aliasRemoveDescriptors.Add(new Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Alias()
      {
        Identity = activeIndexAlias,
        IndexIdentity = oldSharedIndexIdentity
      });
      if (!searchPlatform.SwapAlias(executionContext, aliasAddDescriptors, aliasRemoveDescriptors).IsValid)
      {
        stringBuilder.Clear();
        stringBuilder.Append((object) IndexProvisionType.Shared);
        stringBuilder.Append("Provisioner SwapAlias Failed to update aliasRequest");
        throw new IndexProvisionException(stringBuilder.ToString());
      }
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080203, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("Alias Details - Added alias {0} for index:{1} Removed for index {2} ", (object) activeIndexAlias, (object) newSharedIndexIdentity.Name, (object) oldSharedIndexIdentity.Name)));
    }
  }
}
