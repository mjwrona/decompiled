// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.ReportingHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.WebApi
{
  [ResourceArea("C890B7C4-5CF6-4280-91AC-331E439B8119")]
  public class ReportingHttpClient : VssHttpClientBase
  {
    internal static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "ReportingViewNotSupportedException",
        typeof (ReportingViewNotSupportedException)
      },
      {
        "ReportingViewInvalidFilterException",
        typeof (ReportingViewInvalidFilterException)
      }
    };

    public ReportingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ReportingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ReportingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ReportingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ReportingHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<IEnumerable<ICommerceEvent>> GetCommerceEvents(
      string viewName,
      string resourceName,
      DateTime startTime,
      DateTime endTime,
      string filter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReportingHttpClient reportingHttpClient1 = this;
      IEnumerable<ICommerceEvent> commerceEvents;
      using (new VssHttpClientBase.OperationScope("ReportingEvents", "GetReportingEvents"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (startTime), startTime.ToUniversalTime().ToString("u"));
        collection.Add(nameof (endTime), endTime.ToUniversalTime().ToString("u"));
        if (!string.IsNullOrEmpty(filter))
          collection.Add(nameof (filter), filter);
        ReportingHttpClient reportingHttpClient2 = reportingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid eventsLocationId = CommerceServiceResourceIds.ReportingEventsLocationId;
        var routeValues = new
        {
          viewName = viewName,
          resourceName = resourceName
        };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        commerceEvents = (IEnumerable<ICommerceEvent>) await reportingHttpClient2.SendAsync<IEnumerable<CommerceEvent>>(get, eventsLocationId, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return commerceEvents;
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) ReportingHttpClient.s_translatedExceptions;
  }
}
