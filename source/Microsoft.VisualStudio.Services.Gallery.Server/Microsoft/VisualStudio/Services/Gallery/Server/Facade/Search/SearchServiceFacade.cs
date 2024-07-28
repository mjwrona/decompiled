// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.Search.SearchServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.Search
{
  public sealed class SearchServiceFacade : ISearchServiceFacade
  {
    private const string traceLayer = "LogClientTraceForSearchExtension";
    private const string area = "gallery";
    private readonly ITracerFacade tracer;
    private readonly SearchServiceClient searchServiceClient;

    public SearchServiceFacade(HttpClient httpClient, ITracerFacade tracer)
    {
      this.tracer = tracer ?? throw new ArgumentNullException(nameof (tracer));
      this.searchServiceClient = new SearchServiceClient(httpClient ?? throw new ArgumentNullException(nameof (httpClient)));
    }

    public ExtensionQueryResult GetSearchResults(
      ExtensionQuery extensionQuery,
      string requestUri,
      IVssRequestContext requestContext,
      bool isRequestFromChinaRegion)
    {
      Uri requestUri1;
      try
      {
        requestUri1 = new Uri(requestUri);
      }
      catch (Exception ex) when (ex is ArgumentNullException || ex is UriFormatException)
      {
        this.tracer.TraceException(12062082, "LogClientTraceForSearchExtension", ex);
        throw;
      }
      ExtensionQueryResult results = (ExtensionQueryResult) null;
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        results = this.GetSearchResultsAsync(extensionQuery, requestUri1, isRequestFromChinaRegion).GetAwaiter().GetResult();
        return results;
      }
      finally
      {
        stopwatch.Stop();
        this.PublishTelemetryForSearch(extensionQuery, results, stopwatch, requestContext, isRequestFromChinaRegion);
      }
    }

    public async Task<ExtensionQueryResult> GetSearchResultsAsync(
      ExtensionQuery extensionQuery,
      Uri requestUri,
      bool isRequestFromChinaRegion)
    {
      ExtensionQueryResult searchResultsAsync;
      try
      {
        searchResultsAsync = await this.searchServiceClient.GetSearchResultsAsync(extensionQuery, requestUri, isRequestFromChinaRegion).ConfigureAwait(false);
      }
      catch (ArgumentNullException ex)
      {
        this.tracer.TraceException(12062082, "LogClientTraceForSearchExtension", (Exception) ex);
        if (ex.ParamName == nameof (extensionQuery))
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        throw ex;
      }
      catch (SearchServiceClientException ex)
      {
        this.tracer.TraceException(ex.TracePoint, "LogClientTraceForSearchExtension", (Exception) ex);
        throw ex;
      }
      return searchResultsAsync;
    }

    private void PublishTelemetryForSearch(
      ExtensionQuery extensionQuery,
      ExtensionQueryResult results,
      Stopwatch stopwatch,
      IVssRequestContext requestContext,
      bool isRequestFromChinaRegion)
    {
      string json = this.GetJson<ExtensionQuery>(extensionQuery);
      ClientTraceService service = requestContext.GetService<ClientTraceService>();
      try
      {
        ClientTraceData properties = new ClientTraceData();
        properties.Add(CustomerIntelligenceProperty.Action, (object) "Searched");
        properties.Add("ExtensionQuery", (object) json);
        properties.Add("SearchDuration", (object) stopwatch.ElapsedMilliseconds);
        properties.Add("ResultCount", (object) results?.Results[0]?.Extensions?.Count);
        properties.Add("NewSearchServiceCalled", (object) true);
        properties.Add("IsRequestFromChinaRegion", (object) isRequestFromChinaRegion);
        service.Publish(requestContext, "gallery", "Extension", properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062047, "gallery", "LogClientTraceForSearchExtension", ex);
      }
    }

    private string GetJson<T>(T value)
    {
      string json = string.Empty;
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
      };
      try
      {
        json = JsonConvert.SerializeObject((object) value, settings);
      }
      catch (Exception ex)
      {
        this.tracer.TraceException(12062047, "LogClientTraceForSearchExtension", ex);
      }
      return json;
    }
  }
}
