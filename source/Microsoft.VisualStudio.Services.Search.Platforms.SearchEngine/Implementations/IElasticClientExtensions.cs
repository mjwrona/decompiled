// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.IElasticClientExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Nest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations
{
  internal static class IElasticClientExtensions
  {
    private static readonly List<Type> s_nestExceptions = new List<Type>()
    {
      typeof (ElasticsearchClientException),
      typeof (UnexpectedElasticsearchClientException),
      typeof (WebException)
    };

    public static void WrapAndThrowException(
      this IElasticClient elasticClient,
      Exception e,
      IndexIdentity indexIdentity = null)
    {
      if (indexIdentity != null && indexIdentity.Name != null && elasticClient.Indices.Exists((Indices) Indices.Index(indexIdentity.Name)).Exists)
      {
        HealthStatus healthStatus = IElasticClientExtensions.GetHealthStatus(elasticClient, indexIdentity);
        if (healthStatus.Equals((object) HealthStatus.Yellow))
          throw new SearchPlatformException("The state of the Elastic Search Cluster is yellow.", e);
        if (healthStatus.Equals((object) HealthStatus.Red))
          throw new SearchPlatformException("The state of the Elastic Search Cluster is red.", e);
      }
      if (e is AggregateException)
      {
        AggregateException aggregateException = (AggregateException) e;
        ReadOnlyCollection<Exception> innerExceptions = aggregateException.InnerExceptions;
        if (innerExceptions != null && innerExceptions.All<Exception>((Func<Exception, bool>) (ex => IElasticClientExtensions.s_nestExceptions.Exists((Predicate<Type>) (ee => ee.IsInstanceOfType((object) ex))))))
          throw new SearchPlatformException("A non-generic exception was through from the Elastic Search NEST client.", (Exception) aggregateException.Flatten());
      }
      else if (IElasticClientExtensions.s_nestExceptions.Exists((Predicate<Type>) (ee => ee.IsInstanceOfType((object) e))))
        throw new SearchPlatformException("A non-generic exception was through from the Elastic Search NEST client.", e);
      ExceptionDispatchInfo.Capture(e).Throw();
    }

    public static void ThrowOnInvalidOrFailedResponse(
      this IResponse response,
      bool isNotFoundAllowed = false)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      string empty = string.Empty;
      SearchServiceErrorCode errorCode = SearchServiceErrorCode.Unknown;
      List<SearchPlatformException.ErrorItem> itemsWithErrors = new List<SearchPlatformException.ErrorItem>();
      bool flag;
      try
      {
        flag = response.IsValid;
        if (!flag & isNotFoundAllowed)
        {
          int? httpStatusCode = response.ApiCall.HttpStatusCode;
          int num = 404;
          if (httpStatusCode.GetValueOrDefault() == num & httpStatusCode.HasValue)
            flag = response.ApiCall.Success && response.ServerError == null;
        }
      }
      catch (Exception ex)
      {
        Tracer.TraceException(1080700, "Search Engine", "Search Engine", ex);
        flag = false;
      }
      if (!flag)
      {
        if (response.OriginalException != null && response.OriginalException.Message.IndexOf("Unable to connect to the remote server", StringComparison.OrdinalIgnoreCase) >= 0)
          throw new SearchPlatformException(SearchWebApiResources.ElasticsearchUnavailableErrorMessage, response.OriginalException, SearchServiceErrorCode.ElasticSearch_Connection_Error);
        empty += FormattableString.Invariant(FormattableStringFactory.Create("HTTP Status Code: [{0}]{1}", (object) response.ApiCall.HttpStatusCode, (object) Environment.NewLine));
        if (response.ApiCall.OriginalException != null)
        {
          empty += FormattableString.Invariant(FormattableStringFactory.Create("ORIGINAL_EXCEPTION: [{0}]{1}", (object) response.ApiCall.OriginalException, (object) Environment.NewLine));
          errorCode = SearchServiceErrorCode.ElasticSearch_InvalidResponse_Error;
        }
        if (response.ServerError != null)
        {
          empty += FormattableString.Invariant(FormattableStringFactory.Create("SERVER_ERROR: [{0}]{1}", (object) response.ServerError.Error, (object) Environment.NewLine));
          errorCode = SearchServiceErrorCode.ElasticSearch_Server_Error;
        }
        if (response is BulkResponse bulkResponse)
        {
          if (bulkResponse.ItemsWithErrors != null && bulkResponse.ItemsWithErrors.Any<BulkResponseItemBase>())
          {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (BulkResponseItemBase itemsWithError in bulkResponse.ItemsWithErrors)
            {
              string str = FormattableString.Invariant(FormattableStringFactory.Create("\t{0} returned {1} _index: {2} _type: {3} _version: {4} _id: {5} error: {6}{7}", (object) itemsWithError.Operation, (object) itemsWithError.Status, (object) itemsWithError.Index, (object) itemsWithError.Type, (object) itemsWithError.Version, (object) itemsWithError.Id, (object) itemsWithError.Error, (object) Environment.NewLine));
              stringBuilder.Append(str);
              itemsWithErrors.Add(new SearchPlatformException.ErrorItem(itemsWithError.Index, itemsWithError.Type, itemsWithError.Id, itemsWithError.Version, itemsWithError.Status));
            }
            empty += FormattableString.Invariant(FormattableStringFactory.Create("BULK_API_ERROR: [{0}]{1}", (object) stringBuilder, (object) Environment.NewLine));
            errorCode = SearchServiceErrorCode.ElasticSearch_ItemsResponse_Error;
          }
          else
          {
            string str = string.Empty;
            byte[] responseBodyInBytes = response.ApiCall.ResponseBodyInBytes;
            if (responseBodyInBytes != null && responseBodyInBytes.Length != 0)
              str = FormattableString.Invariant(FormattableStringFactory.Create("RAW_RESPONSE: [{0}]{1}", (object) Encoding.UTF8.GetString(responseBodyInBytes), (object) Environment.NewLine));
            empty += FormattableString.Invariant(FormattableStringFactory.Create("RESPONSE: [{0}]. {1}", (object) response, (object) str));
            errorCode = SearchServiceErrorCode.ElasticSearch_InvalidResponse_Error;
          }
        }
      }
      if (!string.IsNullOrWhiteSpace(empty))
        throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("ES Exception: [{0}]", (object) empty)), errorCode, (IEnumerable<SearchPlatformException.ErrorItem>) itemsWithErrors);
    }

    private static HealthStatus GetHealthStatus(
      IElasticClient elasticClient,
      IndexIdentity indexIdentity)
    {
      ClusterHealthResponse response = elasticClient.Cluster.Health((Indices) indexIdentity.Name, (Func<ClusterHealthDescriptor, IClusterHealthRequest>) (i => (IClusterHealthRequest) i.Index((Indices) indexIdentity.Name)));
      response.ThrowOnInvalidOrFailedResponse();
      switch (response.Status)
      {
        case Health.Green:
          return HealthStatus.Green;
        case Health.Yellow:
          return HealthStatus.Yellow;
        case Health.Red:
          return HealthStatus.Red;
        default:
          return HealthStatus.Unknown;
      }
    }
  }
}
