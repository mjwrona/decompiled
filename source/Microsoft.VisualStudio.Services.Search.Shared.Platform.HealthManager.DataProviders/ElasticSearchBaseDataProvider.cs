// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.ElasticSearchBaseDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public class ElasticSearchBaseDataProvider : IDataProvider
  {
    private const string QueryConstantTerms = "terms";
    private const string QueryConstantBool = "bool";
    private const string QueryConstantMust = "must";
    private const string QueryConstantTerm = "term";
    private const string QueryConstantFilter = "filter";
    private const string QueryConstantQuery = "query";
    private const string TraceArea = "Health Manager";
    private const string TraceLayer = "Job";
    private readonly string m_abstractQueryTemplate = "{{\r\n                                    \"{0}\" : {{\r\n                                        \"{1}\" : {2}\r\n                                    }}\r\n                                }}";
    private readonly List<DocumentContractType> m_supportedContractTypes = new List<DocumentContractType>()
    {
      DocumentContractType.DedupeFileContractV3,
      DocumentContractType.DedupeFileContractV4,
      DocumentContractType.DedupeFileContractV5,
      DocumentContractType.SourceNoDedupeFileContractV3,
      DocumentContractType.SourceNoDedupeFileContractV4,
      DocumentContractType.SourceNoDedupeFileContractV5,
      DocumentContractType.ProjectContract,
      DocumentContractType.RepositoryContract,
      DocumentContractType.WorkItemContract,
      DocumentContractType.WikiContract,
      DocumentContractType.PackageVersionContract
    };
    private DocumentContractType m_documentContractType;
    private IElasticClient m_elasticClient;
    private List<string> m_queryIndices;
    private string m_rawFilterString;
    private string m_rawQueryString;

    [Info("InternalForTestPurpose")]
    internal ElasticSearchBaseDataProvider(IElasticClient elasticClient) => this.m_elasticClient = elasticClient;

    public ElasticSearchBaseDataProvider()
    {
    }

    public List<HealthData> GetData(ProviderContext providerContext)
    {
      ESContext esContext = (ESContext) providerContext;
      this.InstantiateESClient(esContext.QueryConnectionString);
      this.m_documentContractType = esContext.ContractType;
      if (esContext.Indices == null || esContext.Indices.Count <= 0)
        throw new SearchServiceException("No valid index/indices present to fire the search query to ES");
      this.m_queryIndices = esContext.Indices;
      if (!this.m_supportedContractTypes.Contains(this.m_documentContractType))
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported document contract [{0}] encountered.", (object) this.m_documentContractType.ToString())));
      this.ConstructRawQueryString(esContext.SearchText);
      this.ConstructRawFilterString(esContext.SearchFilters);
      return new List<HealthData>()
      {
        (HealthData) new ESTermQueryData((object) this.GetSearchQueryResponse(), DataType.ESTermQuerydata)
      };
    }

    private ESResponseObject GetSearchQueryResponse()
    {
      switch (this.m_documentContractType)
      {
        case DocumentContractType.WorkItemContract:
          return this.Query<WorkItemContract>();
        case DocumentContractType.SourceNoDedupeFileContractV3:
          return this.Query<SourceNoDedupeFileContractV3>();
        case DocumentContractType.DedupeFileContractV3:
          return this.Query<DedupeFileContractV3>();
        case DocumentContractType.SourceNoDedupeFileContractV4:
          return this.Query<SourceNoDedupeFileContractV4>();
        case DocumentContractType.DedupeFileContractV4:
          return this.Query<DedupeFileContractV4>();
        case DocumentContractType.SourceNoDedupeFileContractV5:
          return this.Query<SourceNoDedupeFileContractV5>();
        case DocumentContractType.DedupeFileContractV5:
          return this.Query<DedupeFileContractV5>();
        default:
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported contract type {0}", (object) this.m_documentContractType)));
      }
    }

    private ESResponseObject Query<T>() where T : class
    {
      SearchDescriptor<T> searchDescriptor = new SearchDescriptor<T>().Skip(new int?(0)).Take(new int?(1000)).Index((Indices) Indices.Index((IEnumerable<string>) this.m_queryIndices)).Query(new Func<QueryContainerDescriptor<T>, QueryContainer>(this.BoolQuery<T>));
      ISearchResponse<T> searchResponse = this.m_elasticClient.Search<T>((Func<SearchDescriptor<T>, ISearchRequest>) (s => (ISearchRequest) searchDescriptor));
      ESResponseObject esResponseObject = new ESResponseObject();
      esResponseObject.SearchHitsCount = (int) searchResponse.Total;
      if (searchResponse != null && searchResponse.Hits != null)
      {
        List<Tuple<string, string>> tupleList = new List<Tuple<string, string>>();
        foreach (IHit<T> hit in (IEnumerable<IHit<T>>) searchResponse.Hits)
        {
          if (!string.IsNullOrEmpty(hit.Routing) && !string.IsNullOrEmpty(hit.Id))
            tupleList.Add(Tuple.Create<string, string>(hit.Id, hit.Routing));
          else
            Tracer.TraceVerbose(1083056, "Health Manager", "Job", "Encountered null/empty routingId or docId and hence skipping the same");
        }
        esResponseObject.HitData = tupleList;
      }
      return esResponseObject;
    }

    private QueryContainer BoolQuery<T>(QueryContainerDescriptor<T> queryDescriptor) where T : class => queryDescriptor.Raw(this.CreateBoolQueryString());

    private void InstantiateESClient(string elasticSearchConnectionString)
    {
      if (this.m_elasticClient != null)
        return;
      this.m_elasticClient = !string.IsNullOrWhiteSpace(elasticSearchConnectionString) ? (IElasticClient) new ElasticClient((IConnectionSettingsValues) new ConnectionSettings((IConnectionPool) new StaticConnectionPool(((IEnumerable<string>) elasticSearchConnectionString.Split(';')).Select<string, Uri>((Func<string, Uri>) (u => new Uri(u.Trim()))))).DisableDirectStreaming()) : throw new ArgumentException("ESBaseDataProvider Ctor: connectionString cannot be null or empty");
    }

    [Info("InternalForTestPurpose")]
    internal void ConstructRawQueryString(List<Tuple<string, string>> searchText)
    {
      List<string> values = new List<string>();
      if (searchText == null || !searchText.Any<Tuple<string, string>>())
        return;
      foreach (Tuple<string, string> tuple in searchText)
        values.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_abstractQueryTemplate, (object) "term", (object) tuple.Item1, (object) JsonConvert.SerializeObject((object) tuple.Item2)));
      this.m_rawQueryString = string.Join(",", (IEnumerable<string>) values);
    }

    [Info("InternalForTestPurpose")]
    internal void ConstructRawFilterString(Dictionary<string, List<string>> queryFilters)
    {
      List<string> values = new List<string>();
      if (queryFilters == null || queryFilters.Count <= 0)
        return;
      foreach (KeyValuePair<string, List<string>> queryFilter in queryFilters)
        values.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_abstractQueryTemplate, (object) "terms", (object) queryFilter.Key, (object) JsonConvert.SerializeObject((object) queryFilter.Value)));
      this.m_rawFilterString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                     \"{0}\" : {{\r\n                        \"{1}\" : [\r\n                            {2}\r\n                        ]\r\n                      }}\r\n                    }}", (object) "bool", (object) "must", (object) string.Join(",", (IEnumerable<string>) values));
    }

    [Info("InternalForTestPurpose")]
    internal string CreateBoolQueryString()
    {
      if (!string.IsNullOrWhiteSpace(this.m_rawQueryString) && !string.IsNullOrWhiteSpace(this.m_rawFilterString))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                              \"{1}\": [{2}],\r\n                              \"{3}\": {4}\r\n                            }}\r\n                    }}", (object) "bool", (object) "must", (object) this.m_rawQueryString, (object) "filter", (object) this.m_rawFilterString);
      if (!string.IsNullOrWhiteSpace(this.m_rawQueryString))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                              \"{1}\": [{2}]\r\n                            }}\r\n                    }}", (object) "bool", (object) "must", (object) this.m_rawQueryString);
      return !string.IsNullOrWhiteSpace(this.m_rawFilterString) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                              \"{1}\": {2}\r\n                            }}\r\n                    }}", (object) "bool", (object) "filter", (object) this.m_rawFilterString) : string.Empty;
    }
  }
}
