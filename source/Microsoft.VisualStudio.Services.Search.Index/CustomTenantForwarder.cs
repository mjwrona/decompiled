// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Index.CustomTenantForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Index, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C10BA9-319D-46E2-AA64-F18680226A42
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Index.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Search.Index
{
  public class CustomTenantForwarder : ICustomTenantForwarder
  {
    protected ISearchPlatform m_searchPlatform;
    protected IIndexMapper m_tenantIndexMapper;

    public CustomTenantForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      bool isOnPrem)
    {
      if (string.IsNullOrEmpty(searchPlatformConnectionString))
        throw new ArgumentException(nameof (searchPlatformConnectionString));
      if (string.IsNullOrEmpty(searchPlatformSettings))
        throw new ArgumentException("platformSettings");
      this.Initialize(SearchPlatformFactory.GetInstance().Create(searchPlatformConnectionString, searchPlatformSettings, isOnPrem), (IIndexMapper) new IndexMapper((IEntityType) TenantCodeEntityType.GetInstance()));
    }

    internal CustomTenantForwarder(ISearchPlatform searchPlatform, IIndexMapper tenantIndexMapper) => this.Initialize(searchPlatform, tenantIndexMapper);

    public bool ForwardRegisterTenantRequest(IVssRequestContext requestContext, CustomTenant tenant)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081380, "Query Pipeline", "Query", nameof (ForwardRegisterTenantRequest));
      try
      {
        this.Validate(tenant);
        ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, MethodBase.GetCurrentMethod().Name, 15);
        EventProcessingContext eventProcessingContext = new EventProcessingContext(requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForCustomRepoProcessing"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/BatchCountForCustomRepoProcessing"), (IIndexingUnitChangeEventSelector) new CreationTimeBasedIndexingUnitChangeEventSelector());
        ExecutionContext executionContext = new ExecutionContext(requestContext, correlationDetails, eventProcessingContext)
        {
          FaultService = (IIndexerFaultService) new IndexerV1FaultService(requestContext, (IFaultStore) new RegistryServiceFaultStore(requestContext))
        };
        this.AddTenantAliases(executionContext, tenant);
        this.UpdateTenantRegistrySetting(executionContext, tenant);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081381, "Query Pipeline", "Query", nameof (ForwardRegisterTenantRequest));
      }
      return true;
    }

    public IEnumerable<string> ForwardGetTenantCollectionNamesRequest(
      IVssRequestContext requestContext,
      string tenantName)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081380, "Query Pipeline", "Query", nameof (ForwardGetTenantCollectionNamesRequest));
      try
      {
        IEnumerable<string> collectionNamesRequest = Enumerable.Empty<string>();
        string configValue = requestContext.GetConfigValue("/Service/ALMSearch/Settings/CustomTenantCollectionNames");
        if (!string.IsNullOrWhiteSpace(configValue))
          collectionNamesRequest = (IEnumerable<string>) configValue.Split(',');
        return collectionNamesRequest;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081381, "Query Pipeline", "Query", nameof (ForwardGetTenantCollectionNamesRequest));
      }
    }

    private void AddTenantAliases(ExecutionContext executionContext, CustomTenant tenant)
    {
      CustomTenantAliasInfo customTenantAliasInfo = this.BuildTenantAliasInfo(executionContext, tenant);
      List<Alias> aliasesRequest = new List<Alias>();
      foreach (KeyValuePair<string, CustomTenantIndexInfo> index in (IEnumerable<KeyValuePair<string, CustomTenantIndexInfo>>) customTenantAliasInfo.Indices)
        aliasesRequest.Add(new Alias()
        {
          Identity = AliasIdentity.CreateAliasIdentity(customTenantAliasInfo.Name),
          IndexIdentity = IndexIdentity.CreateIndexIdentity(index.Key),
          Filters = index.Value.Filters
        });
      this.m_searchPlatform.CreateAliasPointingToMultipleIndices(executionContext, aliasesRequest);
    }

    private void UpdateTenantRegistrySetting(ExecutionContext executionContext, CustomTenant tenant)
    {
      IList<string> source = (IList<string>) new List<string>();
      foreach (IndexDetail index in tenant.Indices)
      {
        foreach (CollectionDetail collection in index.Collections)
        {
          if (!source.Contains(collection.Name))
            source.Add(collection.Name);
        }
      }
      executionContext.SetConfigValue<string>("/Service/ALMSearch/Settings/CustomTenantCollectionNames", source.Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)));
    }

    private CustomTenantAliasInfo BuildTenantAliasInfo(
      ExecutionContext executionContext,
      CustomTenant tenant)
    {
      CustomTenantAliasInfo customTenantAliasInfo = new CustomTenantAliasInfo()
      {
        Name = this.m_tenantIndexMapper.GetTenantCodeQueryAlias(tenant.Name).First<IndexInfo>().IndexName
      };
      foreach (IndexDetail index1 in tenant.Indices)
      {
        if (!customTenantAliasInfo.Indices.ContainsKey(index1.Name))
          customTenantAliasInfo.Indices.Add(index1.Name, new CustomTenantIndexInfo());
        CustomTenantIndexInfo index2 = customTenantAliasInfo.Indices[index1.Name];
        foreach (CollectionDetail collection in index1.Collections)
        {
          if (!index2.CollectionIds.Contains(collection.Id))
            index2.CollectionIds.Add(collection.Id);
        }
      }
      return customTenantAliasInfo;
    }

    private void Validate(CustomTenant tenant)
    {
      if (tenant == null)
        throw new ArgumentNullException("Tenant details should not be null", nameof (tenant));
      if (string.IsNullOrWhiteSpace(tenant.Name))
        throw new ArgumentNullException("Tenant name should not be null or contain only whitespaces", "tenant.Name");
      if (tenant.Indices == null || !tenant.Indices.Any<IndexDetail>())
        throw new ArgumentNullException("Indices details should not be null or empty", "tenant.Indices");
      foreach (IndexDetail index in tenant.Indices)
      {
        if (string.IsNullOrWhiteSpace(index.Name))
          throw new ArgumentNullException("Index name is null or empty", "tenant.Indices");
        if (index.Collections == null || !index.Collections.Any<CollectionDetail>())
          throw new ArgumentNullException("Collection details should not be null or empty", "tenant.Indices.Collections");
        foreach (CollectionDetail collection in index.Collections)
        {
          if (string.IsNullOrWhiteSpace(collection.Name) || string.IsNullOrWhiteSpace(collection.Id))
            throw new ArgumentNullException("One of the collection's name or collection Id is null or empty", "tenant.Indices.Collections");
        }
      }
    }

    private void Initialize(ISearchPlatform searchPlatform, IIndexMapper tenantIndexMapper)
    {
      this.m_searchPlatform = searchPlatform;
      this.m_tenantIndexMapper = tenantIndexMapper;
    }
  }
}
