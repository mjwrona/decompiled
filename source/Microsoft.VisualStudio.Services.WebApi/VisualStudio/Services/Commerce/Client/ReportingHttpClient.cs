// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Client.ReportingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.Client
{
  [ResourceArea("365D9DCD-4492-4AE3-B5BA-AD0FF4AB74B3")]
  public class ReportingHttpClient : VssHttpClientBase
  {
    protected static readonly Version previewApiVersion = new Version(3, 2);
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
      using (new VssHttpClientBase.OperationScope("Commerce", "GetReportingEvents"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (startTime), startTime.ToString("u"));
        collection.Add(nameof (endTime), endTime.ToString("u"));
        if (!string.IsNullOrEmpty(filter))
          collection.Add(nameof (filter), filter);
        ReportingHttpClient reportingHttpClient2 = reportingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid reportingEventLocationId = CommerceResourceIds.ReportingEventLocationId;
        var routeValues = new
        {
          viewName = viewName,
          resourceName = resourceName
        };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(ReportingHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        commerceEvents = (IEnumerable<ICommerceEvent>) await reportingHttpClient2.SendAsync<IEnumerable<CommerceEvent>>(get, reportingEventLocationId, (object) routeValues, version, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken1).ConfigureAwait(false);
      }
      return commerceEvents;
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) ReportingHttpClient.s_translatedExceptions;
  }
}
