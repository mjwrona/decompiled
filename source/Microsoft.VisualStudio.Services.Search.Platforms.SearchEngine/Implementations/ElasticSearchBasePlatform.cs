// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearchBasePlatform
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations
{
  internal abstract class ElasticSearchBasePlatform
  {
    internal readonly IElasticClient ElasticClient;
    private const char SearchConnectionStringAddressSeparator = ';';
    private readonly Action<SearchShardsResponse> m_validateSearchShardsResponse = ElasticSearchBasePlatform.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse ?? (ElasticSearchBasePlatform.\u003C\u003EO.\u003C0\u003E__AnalyzeResponse = new Action<SearchShardsResponse>(ElasticSearchBasePlatform.AnalyzeResponse));

    internal ElasticSearchBasePlatform(
      string elasticSearchConnectionString,
      string platformSettings,
      bool isOnPrem)
    {
      this.ElasticClient = (IElasticClient) new Nest.ElasticClient((IConnectionSettingsValues) this.GetElasticSearchConnectionSettings(elasticSearchConnectionString, platformSettings, isOnPrem));
    }

    internal ElasticSearchBasePlatform(IElasticClient elasticClient) => this.ElasticClient = elasticClient;

    public CatResponse<CatIndicesRecord> GetIndices(
      ExecutionContext executionContext,
      List<string> indices = null)
    {
      CatResponse<CatIndicesRecord> indices1 = new ElasticSearchClientWrapper(executionContext, this.ElasticClient).GetIndices(indices);
      int? httpStatusCode = indices1.ApiCall.HttpStatusCode;
      int num = 404;
      return httpStatusCode.GetValueOrDefault() == num & httpStatusCode.HasValue ? (CatResponse<CatIndicesRecord>) null : indices1;
    }

    private Dictionary<string, string> GetElasticSearchPlatformSettings(string platformSettings)
    {
      Dictionary<string, string> platformSettings1;
      try
      {
        platformSettings1 = ((IEnumerable<string>) platformSettings.Split(',')).Select<string, string[]>((Func<string, string[]>) (item => item.Split(new char[1]
        {
          '='
        }, 2))).ToDictionary<string[], string, string>((Func<string[], string>) (item => item[0]), (Func<string[], string>) (item => item[1]));
      }
      catch (Exception ex)
      {
        Tracer.TraceException(1082251, "Search Engine", "Search Engine", ex);
        platformSettings1 = new Dictionary<string, string>();
      }
      if (!platformSettings1.ContainsKey("ConnectionTimeout"))
        platformSettings1.Add("ConnectionTimeout", 60.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return platformSettings1;
    }

    internal ConnectionSettings GetElasticSearchConnectionSettings(
      string elasticSearchConnectionString,
      string platformSettings,
      bool isOnPrem)
    {
      if (string.IsNullOrWhiteSpace(elasticSearchConnectionString))
        throw new ArgumentException("ESPlatform Ctor: connectionString cannot be null or empty");
      Dictionary<string, string> platformSettings1 = this.GetElasticSearchPlatformSettings(platformSettings);
      double result;
      if (!double.TryParse(platformSettings1["ConnectionTimeout"], out result))
        result = 60.0;
      double num = result * 1000.0;
      ConnectionSettings connectionSettings = new ConnectionSettings(new Uri(elasticSearchConnectionString)).DisableDirectStreaming().RequestTimeout(TimeSpan.FromMilliseconds(num));
      if (isOnPrem)
        connectionSettings.DisablePing();
      string username;
      string s;
      if (!platformSettings1.TryGetValue("User", out username) || !platformSettings1.TryGetValue("Password", out s))
        throw new SearchPlatformException("Authentication credentials not found.");
      string password = Encoding.UTF8.GetString(Convert.FromBase64String(s));
      connectionSettings.BasicAuthentication(username, password);
      return connectionSettings;
    }

    public int GetShardId(ExecutionContext executionContext, string indexName, string routingId)
    {
      Tracer.TraceEnter(1082301, "Search Engine", "Search Engine", nameof (GetShardId));
      try
      {
        ElasticSearchClientWrapper elasticSearchClientWrapper = new ElasticSearchClientWrapper(executionContext, this.ElasticClient);
        SearchShardsResponse response = GenericInvoker.Instance.InvokeWithFaultCheck<SearchShardsResponse>((Func<SearchShardsResponse>) (() => elasticSearchClientWrapper.SearchShards(new SearchShardsRequest((Indices) Indices.Index(indexName))
        {
          Routing = (Routing) routingId
        })), executionContext.FaultService, 2, 1000, new TraceMetaData(1082293, "Search Engine", "Search Engine"), this.m_validateSearchShardsResponse);
        response.ThrowOnInvalidOrFailedResponse();
        return response.Shards.FirstOrDefault<IReadOnlyCollection<SearchShard>>().FirstOrDefault<SearchShard>().Shard;
      }
      catch (Exception ex)
      {
        throw new SearchPlatformException("Shard id should be present in the response", ex);
      }
      finally
      {
        Tracer.TraceLeave(1082293, "Search Engine", "Search Engine", nameof (GetShardId));
      }
    }

    public static IEnumerable<IndexName> GetIndices(IEnumerable<IndexInfo> indexInfo) => indexInfo == null ? (IEnumerable<IndexName>) null : indexInfo.Select<IndexInfo, IndexName>((Func<IndexInfo, IndexName>) (i => (IndexName) i.IndexName));

    protected static void AnalyzeResponse(IResponse resp) => resp.ThrowOnInvalidOrFailedResponse();

    protected virtual T GenericESInvoker<T>(
      ExecutionContext executionContext,
      Func<T> function,
      int tracePoint)
      where T : IResponse
    {
      T obj = GenericInvoker.Instance.InvokeWithFaultCheck<T>(function, executionContext.FaultService, 2, 1000, new TraceMetaData(tracePoint, "Search Engine", "Search Engine"), (Action<T>) (response => response.ThrowOnInvalidOrFailedResponse()));
      return obj.IsValid ? obj : throw new SearchPlatformException(obj.OriginalException?.Message ?? string.Empty, obj.OriginalException?.InnerException, SearchServiceErrorCode.ElasticsearchClusterStateServiceError);
    }

    protected virtual T InvokeFunctionWithSearchPlatformExceptionWrapper<T>(
      Func<T> function,
      SearchServiceErrorCode errorCode)
    {
      try
      {
        return function();
      }
      catch (Exception ex)
      {
        throw new SearchPlatformException(ex.Message ?? string.Empty, ex.InnerException, errorCode);
      }
    }
  }
}
