// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildHttpClientCompatBase
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  public abstract class BuildHttpClientCompatBase : VssHttpClientBase
  {
    protected static readonly ApiResourceVersion s_BuildsApiVersion = new ApiResourceVersion("4.1-preview.3");
    protected static readonly ApiResourceVersion s_DefinitionsApiVersion = new ApiResourceVersion("4.1-preview.6");

    public BuildHttpClientCompatBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public BuildHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public BuildHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public BuildHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public BuildHttpClientCompatBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [Obsolete]
    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      string project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      string branchName = null,
      IEnumerable<int> buildIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (definitions != null && definitions.Any<int>())
        keyValuePairList.Add(nameof (definitions), string.Join<int>(",", definitions));
      if (queues != null && queues.Any<int>())
        keyValuePairList.Add(nameof (queues), string.Join<int>(",", queues));
      if (!string.IsNullOrEmpty(buildNumber))
        keyValuePairList.Add(nameof (buildNumber), buildNumber);
      if (minFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minFinishTime), minFinishTime.Value);
      if (maxFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxFinishTime), maxFinishTime.Value);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      if (reasonFilter.HasValue)
        keyValuePairList.Add(nameof (reasonFilter), reasonFilter.Value.ToString());
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (resultFilter.HasValue)
        keyValuePairList.Add(nameof (resultFilter), resultFilter.Value.ToString());
      if (tagFilters != null && tagFilters.Any<string>())
        keyValuePairList.Add(nameof (tagFilters), string.Join(",", tagFilters));
      if (properties != null && properties.Any<string>())
        keyValuePairList.Add(nameof (properties), string.Join(",", properties));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (maxBuildsPerDefinition.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxBuildsPerDefinition.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxBuildsPerDefinition), str);
      }
      if (deletedFilter.HasValue)
        keyValuePairList.Add(nameof (deletedFilter), deletedFilter.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (!string.IsNullOrEmpty(branchName))
        keyValuePairList.Add(nameof (branchName), branchName);
      if (buildIds != null && buildIds.Any<int>())
        keyValuePairList.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_BuildsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Guid project,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (ignoreWarnings.HasValue)
        collection.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (!string.IsNullOrEmpty(checkInTicket))
        collection.Add(nameof (checkInTicket), checkInTicket);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.4");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (ignoreWarnings.HasValue)
        collection.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (!string.IsNullOrEmpty(checkInTicket))
        collection.Add(nameof (checkInTicket), checkInTicket);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.4");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, (object) null, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string project,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (ignoreWarnings.HasValue)
        collection.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (!string.IsNullOrEmpty(checkInTicket))
        collection.Add(nameof (checkInTicket), checkInTicket);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.4");
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      Guid project,
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      string branchName = null,
      IEnumerable<int> buildIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (definitions != null && definitions.Any<int>())
        keyValuePairList.Add(nameof (definitions), string.Join<int>(",", definitions));
      if (queues != null && queues.Any<int>())
        keyValuePairList.Add(nameof (queues), string.Join<int>(",", queues));
      if (!string.IsNullOrEmpty(buildNumber))
        keyValuePairList.Add(nameof (buildNumber), buildNumber);
      if (minFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minFinishTime), minFinishTime.Value);
      if (maxFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxFinishTime), maxFinishTime.Value);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      if (reasonFilter.HasValue)
        keyValuePairList.Add(nameof (reasonFilter), reasonFilter.Value.ToString());
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (resultFilter.HasValue)
        keyValuePairList.Add(nameof (resultFilter), resultFilter.Value.ToString());
      if (tagFilters != null && tagFilters.Any<string>())
        keyValuePairList.Add(nameof (tagFilters), string.Join(",", tagFilters));
      if (properties != null && properties.Any<string>())
        keyValuePairList.Add(nameof (properties), string.Join(",", properties));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (maxBuildsPerDefinition.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxBuildsPerDefinition.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxBuildsPerDefinition), str);
      }
      if (deletedFilter.HasValue)
        keyValuePairList.Add(nameof (deletedFilter), deletedFilter.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (!string.IsNullOrEmpty(branchName))
        keyValuePairList.Add(nameof (branchName), branchName);
      if (buildIds != null && buildIds.Any<int>())
        keyValuePairList.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_BuildsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) null);
    }

    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      IEnumerable<int> definitions = null,
      IEnumerable<int> queues = null,
      string buildNumber = null,
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      BuildReason? reasonFilter = null,
      BuildStatus? statusFilter = null,
      BuildResult? resultFilter = null,
      IEnumerable<string> tagFilters = null,
      IEnumerable<string> properties = null,
      int? top = null,
      string continuationToken = null,
      int? maxBuildsPerDefinition = null,
      QueryDeletedOption? deletedFilter = null,
      BuildQueryOrder? queryOrder = null,
      string branchName = null,
      IEnumerable<int> buildIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (definitions != null && definitions.Any<int>())
        keyValuePairList.Add(nameof (definitions), string.Join<int>(",", definitions));
      if (queues != null && queues.Any<int>())
        keyValuePairList.Add(nameof (queues), string.Join<int>(",", queues));
      if (!string.IsNullOrEmpty(buildNumber))
        keyValuePairList.Add(nameof (buildNumber), buildNumber);
      if (minFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minFinishTime), minFinishTime.Value);
      if (maxFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxFinishTime), maxFinishTime.Value);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      if (reasonFilter.HasValue)
        keyValuePairList.Add(nameof (reasonFilter), reasonFilter.Value.ToString());
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (resultFilter.HasValue)
        keyValuePairList.Add(nameof (resultFilter), resultFilter.Value.ToString());
      if (tagFilters != null && tagFilters.Any<string>())
        keyValuePairList.Add(nameof (tagFilters), string.Join(",", tagFilters));
      if (properties != null && properties.Any<string>())
        keyValuePairList.Add(nameof (properties), string.Join(",", properties));
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (maxBuildsPerDefinition.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxBuildsPerDefinition.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxBuildsPerDefinition), str);
      }
      if (deletedFilter.HasValue)
        keyValuePairList.Add(nameof (deletedFilter), deletedFilter.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (!string.IsNullOrEmpty(branchName))
        keyValuePairList.Add(nameof (branchName), branchName);
      if (buildIds != null && buildIds.Any<int>())
        keyValuePairList.Add(nameof (buildIds), string.Join<int>(",", buildIds));
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, (object) null, BuildHttpClientCompatBase.s_BuildsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) null);
    }

    public virtual Task<BuildDefinition> GetDefinitionAsync(
      string project,
      int definitionId,
      int? revision = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    public virtual Task<BuildDefinition> GetDefinitionAsync(
      Guid project,
      int definitionId,
      int? revision = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new
      {
        project = project,
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    [Obsolete("Use GetDefinitionAsync(string, int) instead.")]
    public virtual Task<BuildDefinition> GetDefinitionAsync(
      int definitionId,
      int? revision = null,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new
      {
        definitionId = definitionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (revision.HasValue)
        keyValuePairList.Add(nameof (revision), revision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<BuildDefinition>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<BuildDefinition>>) null);
    }

    public virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    public virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    [Obsolete("Use GetDefinitionsAsync(string) instead.")]
    public virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, (object) null, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    protected virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeAllProperties = null,
      bool? includeLatestBuilds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      bool flag;
      if (includeAllProperties.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllProperties.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllProperties), str);
      }
      if (includeLatestBuilds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestBuilds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestBuilds), str);
      }
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    protected virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeAllProperties = null,
      bool? includeLatestBuilds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      bool flag;
      if (includeAllProperties.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllProperties.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllProperties), str);
      }
      if (includeLatestBuilds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestBuilds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestBuilds), str);
      }
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    [Obsolete("Use GetDefinitionsAsync(string) instead.")]
    protected virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeAllProperties = null,
      bool? includeLatestBuilds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      bool flag;
      if (includeAllProperties.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllProperties.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllProperties), str);
      }
      if (includeLatestBuilds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestBuilds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestBuilds), str);
      }
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, (object) null, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    protected virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeAllProperties = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      bool flag;
      if (includeAllProperties.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllProperties.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllProperties), str);
      }
      if (includeLatestBuilds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestBuilds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestBuilds), str);
      }
      if (taskIdFilter.HasValue)
        keyValuePairList.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.6"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    protected virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeAllProperties = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      bool flag;
      if (includeAllProperties.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllProperties.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllProperties), str);
      }
      if (includeLatestBuilds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestBuilds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestBuilds), str);
      }
      if (taskIdFilter.HasValue)
        keyValuePairList.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.6"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    protected virtual Task<List<BuildDefinitionReference>> GetDefinitionsAsync(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTime = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeAllProperties = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(name))
        keyValuePairList.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        keyValuePairList.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        keyValuePairList.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minMetricsTime), minMetricsTime.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        keyValuePairList.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (notBuiltAfter), notBuiltAfter.Value);
      bool flag;
      if (includeAllProperties.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllProperties.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllProperties), str);
      }
      if (includeLatestBuilds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeLatestBuilds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestBuilds), str);
      }
      if (taskIdFilter.HasValue)
        keyValuePairList.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      return this.SendAsync<List<BuildDefinitionReference>>(method, locationId, (object) null, new ApiResourceVersion("5.0-preview.6"), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinitionReference>>>) null);
    }

    public virtual Task<IPagedList<BuildDefinitionReference>> GetDefinitionsAsync2(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(false));
      return this.SendAsync<IPagedList<BuildDefinitionReference>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinitionReference>>>(this.GetPagedList<BuildDefinitionReference>));
    }

    public virtual Task<IPagedList<BuildDefinitionReference>> GetDefinitionsAsync2(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(false));
      return this.SendAsync<IPagedList<BuildDefinitionReference>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinitionReference>>>(this.GetPagedList<BuildDefinitionReference>));
    }

    [Obsolete("Use GetDefinitionsAsync2(string) instead.")]
    public virtual Task<IPagedList<BuildDefinitionReference>> GetDefinitionsAsync2(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(false));
      return this.SendAsync<IPagedList<BuildDefinitionReference>>(method, locationId, version: BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinitionReference>>>(this.GetPagedList<BuildDefinitionReference>));
    }

    public virtual Task<List<BuildDefinition>> GetFullDefinitionsAsync(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true));
      return this.SendAsync<List<BuildDefinition>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinition>>>) null);
    }

    public virtual Task<List<BuildDefinition>> GetFullDefinitionsAsync(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true));
      return this.SendAsync<List<BuildDefinition>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinition>>>) null);
    }

    [Obsolete("Use GetFullDefinitionsAsync(string) instead.")]
    public virtual Task<List<BuildDefinition>> GetFullDefinitionsAsync(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true));
      return this.SendAsync<List<BuildDefinition>>(method, locationId, (object) null, BuildHttpClientCompatBase.s_DefinitionsApiVersion, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<BuildDefinition>>>) null);
    }

    public virtual Task<IPagedList<BuildDefinition>> GetFullDefinitionsAsync2(
      string project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true));
      return this.SendAsync<IPagedList<BuildDefinition>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinition>>>(this.GetPagedList<BuildDefinition>));
    }

    public virtual Task<IPagedList<BuildDefinition>> GetFullDefinitionsAsync2(
      Guid project,
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true));
      return this.SendAsync<IPagedList<BuildDefinition>>(method, locationId, routeValues, BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinition>>>(this.GetPagedList<BuildDefinition>));
    }

    [Obsolete("Use GetFullDefinitionsAsync2(string) instead.")]
    public virtual Task<IPagedList<BuildDefinition>> GetFullDefinitionsAsync2(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> definitionsQueryParams = this.GetDefinitionsQueryParams(name, repositoryId, repositoryType, queryOrder, top, continuationToken, minMetricsTimeInUtc, definitionIds, path, builtAfter, notBuiltAfter, new bool?(true));
      return this.SendAsync<IPagedList<BuildDefinition>>(method, locationId, version: BuildHttpClientCompatBase.s_DefinitionsApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) definitionsQueryParams, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedList<BuildDefinition>>>(this.GetPagedList<BuildDefinition>));
    }

    [Obsolete("Use UpdateBuildAsync(Build, bool, object, CancellationToken) instead.")]
    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ buildId = buildId };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    [Obsolete("Use UpdateBuildAsync(Build, bool, object, CancellationToken) instead.")]
    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    [Obsolete("Use UpdateBuildAsync(Build, bool, object, CancellationToken) instead.")]
    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(5.0, 4);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<List<string>> ListBranchesAsync(
      string project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e05d4403-9b81-4244-8763-20fde28d1976");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<List<string>> ListBranchesAsync(
      Guid project,
      string providerName,
      Guid? serviceEndpointId = null,
      string repository = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e05d4403-9b81-4244-8763-20fde28d1976");
      object routeValues = (object) new
      {
        project = project,
        providerName = providerName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (serviceEndpointId.HasValue)
        keyValuePairList.Add(nameof (serviceEndpointId), serviceEndpointId.Value.ToString());
      if (repository != null)
        keyValuePairList.Add(nameof (repository), repository);
      return this.SendAsync<List<string>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 1), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<string>>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string project,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      int? sourceBuildId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (ignoreWarnings.HasValue)
        collection.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (checkInTicket != null)
        collection.Add(nameof (checkInTicket), checkInTicket);
      if (sourceBuildId.HasValue)
        collection.Add(nameof (sourceBuildId), sourceBuildId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(6.0, 5);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Guid project,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      int? sourceBuildId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (ignoreWarnings.HasValue)
        collection.Add(nameof (ignoreWarnings), ignoreWarnings.Value.ToString());
      if (checkInTicket != null)
        collection.Add(nameof (checkInTicket), checkInTicket);
      if (sourceBuildId.HasValue)
        collection.Add(nameof (sourceBuildId), sourceBuildId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(6.0, 5);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2, (Func<HttpResponseMessage, CancellationToken, Task<Microsoft.TeamFoundation.Build.WebApi.Build>>) null);
    }

    [Obsolete]
    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> QueueBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      bool? ignoreWarnings = null,
      string checkInTicket = null,
      int? sourceBuildId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.Build>(build, nameof (build));
      ArgumentUtility.CheckForNull<TeamProjectReference>(build.Project, "build.Project");
      return this.QueueBuildAsync(build, build.Project.Id, ignoreWarnings, checkInTicket, sourceBuildId, userState, cancellationToken);
    }

    private protected List<KeyValuePair<string, string>> GetDefinitionsQueryParams(
      string name = null,
      string repositoryId = null,
      string repositoryType = null,
      DefinitionQueryOrder? queryOrder = null,
      int? top = null,
      string continuationToken = null,
      DateTime? minMetricsTimeInUtc = null,
      IEnumerable<int> definitionIds = null,
      string path = null,
      DateTime? builtAfter = null,
      DateTime? notBuiltAfter = null,
      bool? includeAllProperties = null,
      bool? includeLatestBuilds = null,
      Guid? taskIdFilter = null,
      int? processType = null,
      string yamlFilename = null)
    {
      List<KeyValuePair<string, string>> definitionsQueryParams = new List<KeyValuePair<string, string>>();
      definitionsQueryParams.Add("type", "Build");
      if (!string.IsNullOrEmpty(name))
        definitionsQueryParams.Add(nameof (name), name);
      if (!string.IsNullOrEmpty(repositoryId))
        definitionsQueryParams.Add(nameof (repositoryId), repositoryId);
      if (!string.IsNullOrEmpty(repositoryType))
        definitionsQueryParams.Add(nameof (repositoryType), repositoryType);
      if (queryOrder.HasValue)
        definitionsQueryParams.Add(nameof (queryOrder), queryOrder.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = definitionsQueryParams;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(continuationToken))
        definitionsQueryParams.Add(nameof (continuationToken), continuationToken);
      if (minMetricsTimeInUtc.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) definitionsQueryParams, nameof (minMetricsTimeInUtc), minMetricsTimeInUtc.Value);
      if (definitionIds != null && definitionIds.Any<int>())
        definitionsQueryParams.Add(nameof (definitionIds), string.Join<int>(",", definitionIds));
      if (!string.IsNullOrEmpty(path))
        definitionsQueryParams.Add(nameof (path), path);
      if (builtAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) definitionsQueryParams, nameof (builtAfter), builtAfter.Value);
      if (notBuiltAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) definitionsQueryParams, nameof (notBuiltAfter), notBuiltAfter.Value);
      bool flag;
      if (includeAllProperties.GetValueOrDefault())
      {
        List<KeyValuePair<string, string>> collection = definitionsQueryParams;
        flag = includeAllProperties.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllProperties), str);
      }
      if (includeLatestBuilds.GetValueOrDefault())
      {
        List<KeyValuePair<string, string>> collection = definitionsQueryParams;
        flag = includeLatestBuilds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeLatestBuilds), str);
      }
      if (taskIdFilter.HasValue)
        definitionsQueryParams.Add(nameof (taskIdFilter), taskIdFilter.Value.ToString());
      if (processType.HasValue)
      {
        List<KeyValuePair<string, string>> collection = definitionsQueryParams;
        num = processType.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (processType), str);
      }
      if (!string.IsNullOrEmpty(yamlFilename))
        definitionsQueryParams.Add(nameof (yamlFilename), yamlFilename);
      return definitionsQueryParams;
    }

    private protected async Task<IPagedList<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      BuildHttpClientCompatBase clientCompatBase = this;
      string continuationToken = clientCompatBase.GetContinuationToken(responseMessage);
      IPagedList<T> pagedList = (IPagedList<T>) new PagedList<T>((IEnumerable<T>) await clientCompatBase.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
      continuationToken = (string) null;
      return pagedList;
    }

    private protected Task<T> SendAsync<T>(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      return this.SendAsync<T>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, processResponse);
    }

    private protected async Task<T> SendAsync<T>(
      HttpMethod method,
      IEnumerable<KeyValuePair<string, string>> additionalHeaders,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      BuildHttpClientCompatBase clientCompatBase = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await clientCompatBase.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await clientCompatBase.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    private protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      BuildHttpClientCompatBase clientCompatBase = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) clientCompatBase).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await clientCompatBase.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }

    private protected string GetContinuationToken(HttpResponseMessage responseMessage)
    {
      string continuationToken = (string) null;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (responseMessage.Headers.TryGetValues("x-ms-continuationtoken", out values))
        continuationToken = values.FirstOrDefault<string>();
      return continuationToken;
    }
  }
}
