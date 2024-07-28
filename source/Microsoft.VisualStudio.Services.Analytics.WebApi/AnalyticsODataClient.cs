// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsODataClient
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [ResourceArea("{F47C4501-5E41-4A7C-B17B-19B7CEF00B91}")]
  public class AnalyticsODataClient : VssHttpClientBase
  {
    private const string ODataAPIVersion = "v4.0-preview";

    public AnalyticsODataClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AnalyticsODataClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AnalyticsODataClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AnalyticsODataClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public AnalyticsODataClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<T> GetODataAsync<T>(
      string entityType,
      Guid projectId = default (Guid),
      Uri nextLinkUri = null,
      string apply = null,
      int count = 0,
      string expand = null,
      string filter = null,
      string format = null,
      string orderby = null,
      string search = null,
      string select = null,
      int skip = 0,
      int top = 0,
      string preferHeaderValue = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AnalyticsODataClient analyticsOdataClient = this;
      HttpMethod get = HttpMethod.Get;
      List<KeyValuePair<string, string>> additionalHeaders = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(preferHeaderValue))
        additionalHeaders.Add(new KeyValuePair<string, string>("Prefer", preferHeaderValue));
      Guid locationId = new Guid("{97758D47-38CF-4AC2-A11D-C161E9B20609}");
      ApiResourceLocation location = await analyticsOdataClient.GetResourceLocationAsync(locationId, userState, cancellationToken).ConfigureAwait(false);
      object routeValues;
      if (projectId == new Guid())
      {
        routeValues = (object) new
        {
          ODataVersion = "v4.0-preview",
          EntityType = entityType
        };
      }
      else
      {
        routeValues = (object) new
        {
          Project = projectId.ToString(),
          ODataVersion = "v4.0-preview",
          EntityType = entityType
        };
        if (!location.RouteTemplate.StartsWith("{project}"))
          location.RouteTemplate = "{project}/" + location.RouteTemplate;
      }
      HttpRequestMessage requestMessage;
      if (nextLinkUri == (Uri) null)
      {
        List<KeyValuePair<string, string>> queryParameters = analyticsOdataClient.ReturnQueryParameters(apply, count, expand, filter, format, orderby, search, select, skip, top);
        requestMessage = analyticsOdataClient.CreateRequestMessage(HttpMethod.Get, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, location, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters);
        try
        {
          return await analyticsOdataClient.SendAsync<T>(requestMessage, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
          requestMessage?.Dispose();
        }
      }
      else
      {
        requestMessage = analyticsOdataClient.CreateRequestMessage(HttpMethod.Get, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, location, version: new ApiResourceVersion("5.0-preview.1"));
        try
        {
          requestMessage.RequestUri = nextLinkUri;
          return await analyticsOdataClient.SendAsync<T>(requestMessage, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
          requestMessage?.Dispose();
        }
      }
    }

    private List<KeyValuePair<string, string>> ReturnQueryParameters(
      string apply = null,
      int count = 0,
      string expand = null,
      string filter = null,
      string format = null,
      string orderby = null,
      string search = null,
      string select = null,
      int skip = 0,
      int top = 0)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(apply))
        collection.Add("$apply", apply);
      if (count != 0)
        collection.Add<int>("$count", count);
      if (!string.IsNullOrEmpty(expand))
        collection.Add("$expand", expand);
      if (!string.IsNullOrEmpty(filter))
        collection.Add("$filter", filter);
      if (!string.IsNullOrEmpty(format))
        collection.Add("$format", format);
      if (!string.IsNullOrEmpty(orderby))
        collection.Add("$orderby", orderby);
      if (!string.IsNullOrEmpty(search))
        collection.Add("$search", search);
      if (!string.IsNullOrEmpty(select))
        collection.Add("$select", select);
      if (skip != 0)
        collection.Add<int>("$skip", skip);
      if (top != 0)
        collection.Add<int>("$top", top);
      return collection;
    }
  }
}
