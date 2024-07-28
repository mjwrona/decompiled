// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.AbstractSearchDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public abstract class AbstractSearchDataProvider
  {
    private readonly string m_methodName = "Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.EsIndexDocumentCountDataProvider.Initialize";

    protected AbstractSearchDataProvider()
      : this(Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.SearchPlatformFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal AbstractSearchDataProvider(ISearchPlatformFactory searchPlatformFactory) => this.SearchPlatformFactory = searchPlatformFactory;

    protected internal ISearchPlatformFactory SearchPlatformFactory { get; set; }

    protected internal ISearchPlatform SearchPlatform { get; set; }

    protected internal ISearchClusterManagementService SearchClusterStateService { get; set; }

    protected ExecutionContext ExecutionContext { get; set; }

    [Info("InternalForTestPurpose")]
    internal void Initialize(ProviderContext providerContext)
    {
      ESDeploymentContext deploymentContext = (ESDeploymentContext) providerContext;
      this.SearchPlatform = this.SearchPlatformFactory.Create(deploymentContext.SearchPlatformConnectionString, deploymentContext.SearchPlatformSettings, false);
      this.SearchClusterStateService = this.SearchPlatformFactory.CreateSearchClusterManagementService(deploymentContext.SearchPlatformConnectionString, deploymentContext.SearchPlatformSettings, false);
      this.ExecutionContext = deploymentContext.RequestContext.GetExecutionContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(deploymentContext.RequestContext.ActivityId.ToString(), this.m_methodName, 0));
    }

    [Info("InternalForTestPurpose")]
    internal List<string> GetFilteredIndices(ProviderContext providerContext)
    {
      ESDeploymentContext deploymentContext = providerContext != null ? (ESDeploymentContext) providerContext : throw new ArgumentNullException(nameof (providerContext));
      IEnumerable<string> indices = deploymentContext.Indices;
      if ((indices != null ? (!indices.Any<string>() ? 1 : 0) : 1) != 0)
        return new List<string>();
      IEnumerable<string> second = this.SearchPlatform.GetIndices(this.ExecutionContext).Records.Select<CatIndicesRecord, string>((Func<CatIndicesRecord, string>) (it => it.Index));
      IEnumerable<string> source = indices.Contains<string>("_all") ? second : indices.Intersect<string>(second);
      IEntityType entityType = deploymentContext.EntityType;
      List<string> filteredIndices;
      if (entityType.Name != "All" && source.Any<string>())
      {
        filteredIndices = new List<string>();
        foreach (string indexName in source)
        {
          if (this.GetIndexEntityType(this.ExecutionContext, indexName).Equals((object) entityType))
            filteredIndices.Add(indexName);
        }
      }
      else
        filteredIndices = source.ToList<string>();
      return filteredIndices;
    }

    [Info("InternalForTestPurpose")]
    internal IEntityType GetIndexEntityType(ExecutionContext executionContext, string indexName)
    {
      List<DocumentContractType> list = this.SearchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(indexName)).GetDocumentContracts(executionContext).ToList<DocumentContractType>();
      if (!list.Any<DocumentContractType>())
        throw new SearchServiceException("Unable to fetch any contract type for the index: " + indexName);
      try
      {
        return list.First<DocumentContractType>().GetEntityTypeForContractType();
      }
      catch (Exception ex)
      {
        throw new SearchServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Can't understand the contract type {0} present in the index {1}. Exception message: {2}", (object) list.First<DocumentContractType>().ToString(), (object) indexName, (object) ex.Message));
      }
    }
  }
}
