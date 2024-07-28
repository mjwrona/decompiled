// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients.ReleaseCompatHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ResourceArea("efc2f575-36ef-48e9-b672-0c6fb4a48ac5")]
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Compat is a well known shortening for comaptibility and using this name is the recommended approach")]
  public class ReleaseCompatHttpClientBase : VssHttpClientBase
  {
    public ReleaseCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ReleaseCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ReleaseCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ReleaseCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ReleaseCompatHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseDefinitionsAsync(project, searchText = null, expand = null, artifactType = null, artifactSourceId = null, top = null, continuationToken = null, queryOrder = null, path = null, isExactNameMatch = null, tagFilter = null, propertyFilters = null, definitionIdFilter = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      string project,
      string searchText,
      string artifactType,
      string artifactSourceId,
      ReleaseDefinitionExpands? expand,
      int? top,
      string continuationToken,
      ReleaseDefinitionQueryOrder? queryOrder,
      string path,
      bool? isExactNameMatch,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(searchText))
        keyValuePairList.Add(nameof (searchText), searchText);
      if (!string.IsNullOrEmpty(artifactType))
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (!string.IsNullOrEmpty(artifactSourceId))
        keyValuePairList.Add(nameof (artifactSourceId), artifactSourceId);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (isExactNameMatch.HasValue)
        keyValuePairList.Add(nameof (isExactNameMatch), isExactNameMatch.Value.ToString());
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseDefinitionsAsync(project, searchText = null, expand = null, artifactType = null, artifactSourceId = null, top = null, continuationToken = null, queryOrder = null, path = null, isExactNameMatch = null, tagFilter = null, propertyFilters = null, definitionIdFilter = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      Guid project,
      string searchText,
      string artifactType,
      string artifactSourceId,
      ReleaseDefinitionExpands? expand,
      int? top,
      string continuationToken,
      ReleaseDefinitionQueryOrder? queryOrder,
      string path,
      bool? isExactNameMatch,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(searchText))
        keyValuePairList.Add(nameof (searchText), searchText);
      if (!string.IsNullOrEmpty(artifactType))
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (!string.IsNullOrEmpty(artifactSourceId))
        keyValuePairList.Add(nameof (artifactSourceId), artifactSourceId);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (isExactNameMatch.HasValue)
        keyValuePairList.Add(nameof (isExactNameMatch), isExactNameMatch.Value.ToString());
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseDefinitionsAsync(project, searchText = null, expand = null, artifactType = null, artifactSourceId = null, top = null, continuationToken = null, queryOrder = null, path = null, isExactNameMatch = null, tagFilter = null, propertyFilters = null, definitionIdFilter = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      string project,
      string searchText,
      ReleaseDefinitionExpands? expand,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(searchText))
        keyValuePairList.Add(nameof (searchText), searchText);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseDefinitionsAsync(project, searchText = null, expand = null, artifactType = null, artifactSourceId = null, top = null, continuationToken = null, queryOrder = null, path = null, isExactNameMatch = null, tagFilter = null, propertyFilters = null, definitionIdFilter = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      Guid project,
      string searchText,
      ReleaseDefinitionExpands? expand,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(searchText))
        keyValuePairList.Add(nameof (searchText), searchText);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use TaskHttpClientBase.GetQueuedPlanGroupsAsync(Guid scopeIdentifier, string hubName, PlanGroupStatusFilter? statusFilter = null, int? count = null, object userState = null, CancellationToken cancellationToken = default(CancellationToken) instead", false)]
    public virtual Task<List<QueuedReleaseData>> GetQueuedReleasesAsync(
      string projectId,
      int? releaseId,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cf6fc7ba-4ad9-403b-86e6-e372cd3b2327");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      if (releaseId.HasValue)
        keyValuePairList.Add(nameof (releaseId), releaseId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<QueuedReleaseData>>(method, locationId, version: new ApiResourceVersion("3.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseDefinitionEnvironmentTemplate>> ListDefinitionEnvironmentTemplatesAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseDefinitionEnvironmentTemplate>>(new HttpMethod("GET"), new Guid("6b03b696-824e-4479-8eb2-6644a51aba89"), (object) new
      {
        project = project
      }, new ApiResourceVersion("4.1-preview.2"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ReleaseDefinitionEnvironmentTemplate>> ListDefinitionEnvironmentTemplatesAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ReleaseDefinitionEnvironmentTemplate>>(new HttpMethod("GET"), new Guid("6b03b696-824e-4479-8eb2-6644a51aba89"), (object) new
      {
        project = project
      }, new ApiResourceVersion("4.1-preview.2"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Release>> GetReleasesAsync(
      string project,
      int? definitionId,
      int? definitionEnvironmentId,
      string searchText,
      string createdBy,
      ReleaseStatus? statusFilter,
      int? environmentStatusFilter,
      DateTime? minCreatedTime,
      DateTime? maxCreatedTime,
      ReleaseQueryOrder? queryOrder,
      int? top,
      int? continuationToken,
      ReleaseExpands? expand,
      string artifactTypeId,
      string sourceId,
      string artifactVersionId,
      string sourceBranchFilter,
      bool? isDeleted,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (!string.IsNullOrEmpty(searchText))
        keyValuePairList.Add(nameof (searchText), searchText);
      if (!string.IsNullOrEmpty(createdBy))
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (environmentStatusFilter.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = environmentStatusFilter.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (environmentStatusFilter), str);
      }
      if (minCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minCreatedTime), minCreatedTime.Value);
      if (maxCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCreatedTime), maxCreatedTime.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (!string.IsNullOrEmpty(artifactTypeId))
        keyValuePairList.Add(nameof (artifactTypeId), artifactTypeId);
      if (!string.IsNullOrEmpty(sourceId))
        keyValuePairList.Add(nameof (sourceId), sourceId);
      if (!string.IsNullOrEmpty(artifactVersionId))
        keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      if (!string.IsNullOrEmpty(sourceBranchFilter))
        keyValuePairList.Add(nameof (sourceBranchFilter), sourceBranchFilter);
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      return this.SendAsync<List<Release>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Release>> GetReleasesAsync(
      Guid project,
      int? definitionId,
      int? definitionEnvironmentId,
      string searchText,
      string createdBy,
      ReleaseStatus? statusFilter,
      int? environmentStatusFilter,
      DateTime? minCreatedTime,
      DateTime? maxCreatedTime,
      ReleaseQueryOrder? queryOrder,
      int? top,
      int? continuationToken,
      ReleaseExpands? expand,
      string artifactTypeId,
      string sourceId,
      string artifactVersionId,
      string sourceBranchFilter,
      bool? isDeleted,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (!string.IsNullOrEmpty(searchText))
        keyValuePairList.Add(nameof (searchText), searchText);
      if (!string.IsNullOrEmpty(createdBy))
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (environmentStatusFilter.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = environmentStatusFilter.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (environmentStatusFilter), str);
      }
      if (minCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minCreatedTime), minCreatedTime.Value);
      if (maxCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCreatedTime), maxCreatedTime.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (!string.IsNullOrEmpty(artifactTypeId))
        keyValuePairList.Add(nameof (artifactTypeId), artifactTypeId);
      if (!string.IsNullOrEmpty(sourceId))
        keyValuePairList.Add(nameof (sourceId), sourceId);
      if (!string.IsNullOrEmpty(artifactVersionId))
        keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      if (!string.IsNullOrEmpty(sourceBranchFilter))
        keyValuePairList.Add(nameof (sourceBranchFilter), sourceBranchFilter);
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      return this.SendAsync<List<Release>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<Release>> GetReleasesAsync(
      int? definitionId,
      int? definitionEnvironmentId,
      string searchText,
      string createdBy,
      ReleaseStatus? statusFilter,
      int? environmentStatusFilter,
      DateTime? minCreatedTime,
      DateTime? maxCreatedTime,
      ReleaseQueryOrder? queryOrder,
      int? top,
      int? continuationToken,
      ReleaseExpands? expand,
      string artifactTypeId,
      string sourceId,
      string artifactVersionId,
      string sourceBranchFilter,
      bool? isDeleted,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (!string.IsNullOrEmpty(searchText))
        keyValuePairList.Add(nameof (searchText), searchText);
      if (!string.IsNullOrEmpty(createdBy))
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (environmentStatusFilter.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = environmentStatusFilter.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (environmentStatusFilter), str);
      }
      if (minCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minCreatedTime), minCreatedTime.Value);
      if (maxCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCreatedTime), maxCreatedTime.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (!string.IsNullOrEmpty(artifactTypeId))
        keyValuePairList.Add(nameof (artifactTypeId), artifactTypeId);
      if (!string.IsNullOrEmpty(sourceId))
        keyValuePairList.Add(nameof (sourceId), sourceId);
      if (!string.IsNullOrEmpty(artifactVersionId))
        keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      if (!string.IsNullOrEmpty(sourceBranchFilter))
        keyValuePairList.Add(nameof (sourceBranchFilter), sourceBranchFilter);
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      return this.SendAsync<List<Release>>(method, locationId, version: new ApiResourceVersion("3.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleasesAsync(project, definitionId = null, definitionEnvironmentId = null, searchText = null, createdBy = null, statusFilter = null, environmentStatusFilter = null, minCreatedTime = null, maxCreatedTime = null, queryOrder = null, top = null, continuationToken = null, expand = null, artifactTypeId = null, sourceId = null, artifactVersionId = null, sourceBranchFilter = null, isDeleted = null, tagFilter = null, propertyFilters = null, releaseIdFilter = null, path = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<Release>> GetReleasesAsync(
      string project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string searchText = null,
      string createdBy = null,
      ReleaseStatus? statusFilter = null,
      int? environmentStatusFilter = null,
      DateTime? minCreatedTime = null,
      DateTime? maxCreatedTime = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseExpands? expand = null,
      string artifactTypeId = null,
      string sourceId = null,
      string artifactVersionId = null,
      string sourceBranchFilter = null,
      bool? isDeleted = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<int> releaseIdFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (createdBy != null)
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (environmentStatusFilter.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = environmentStatusFilter.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (environmentStatusFilter), str);
      }
      if (minCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minCreatedTime), minCreatedTime.Value);
      if (maxCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCreatedTime), maxCreatedTime.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactTypeId != null)
        keyValuePairList.Add(nameof (artifactTypeId), artifactTypeId);
      if (sourceId != null)
        keyValuePairList.Add(nameof (sourceId), sourceId);
      if (artifactVersionId != null)
        keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      if (sourceBranchFilter != null)
        keyValuePairList.Add(nameof (sourceBranchFilter), sourceBranchFilter);
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdFilter), string.Join<int>(",", releaseIdFilter));
      return this.SendAsync<List<Release>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleasesAsync(project, definitionId = null, definitionEnvironmentId = null, searchText = null, createdBy = null, statusFilter = null, environmentStatusFilter = null, minCreatedTime = null, maxCreatedTime = null, queryOrder = null, top = null, continuationToken = null, expand = null, artifactTypeId = null, sourceId = null, artifactVersionId = null, sourceBranchFilter = null, isDeleted = null, tagFilter = null, propertyFilters = null, releaseIdFilter = null, path = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<Release>> GetReleasesAsync(
      Guid project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string searchText = null,
      string createdBy = null,
      ReleaseStatus? statusFilter = null,
      int? environmentStatusFilter = null,
      DateTime? minCreatedTime = null,
      DateTime? maxCreatedTime = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseExpands? expand = null,
      string artifactTypeId = null,
      string sourceId = null,
      string artifactVersionId = null,
      string sourceBranchFilter = null,
      bool? isDeleted = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<int> releaseIdFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (createdBy != null)
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (environmentStatusFilter.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = environmentStatusFilter.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (environmentStatusFilter), str);
      }
      if (minCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minCreatedTime), minCreatedTime.Value);
      if (maxCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCreatedTime), maxCreatedTime.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactTypeId != null)
        keyValuePairList.Add(nameof (artifactTypeId), artifactTypeId);
      if (sourceId != null)
        keyValuePairList.Add(nameof (sourceId), sourceId);
      if (artifactVersionId != null)
        keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      if (sourceBranchFilter != null)
        keyValuePairList.Add(nameof (sourceBranchFilter), sourceBranchFilter);
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdFilter), string.Join<int>(",", releaseIdFilter));
      return this.SendAsync<List<Release>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleasesAsync(definitionId = null, definitionEnvironmentId = null, searchText = null, createdBy = null, statusFilter = null, environmentStatusFilter = null, minCreatedTime = null, maxCreatedTime = null, queryOrder = null, top = null, continuationToken = null, expand = null, artifactTypeId = null, sourceId = null, artifactVersionId = null, sourceBranchFilter = null, isDeleted = null, tagFilter = null, propertyFilters = null, releaseIdFilter = null, path = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<Release>> GetReleasesAsync(
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string searchText = null,
      string createdBy = null,
      ReleaseStatus? statusFilter = null,
      int? environmentStatusFilter = null,
      DateTime? minCreatedTime = null,
      DateTime? maxCreatedTime = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseExpands? expand = null,
      string artifactTypeId = null,
      string sourceId = null,
      string artifactVersionId = null,
      string sourceBranchFilter = null,
      bool? isDeleted = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<int> releaseIdFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (createdBy != null)
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (environmentStatusFilter.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = environmentStatusFilter.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (environmentStatusFilter), str);
      }
      if (minCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minCreatedTime), minCreatedTime.Value);
      if (maxCreatedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxCreatedTime), maxCreatedTime.Value);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactTypeId != null)
        keyValuePairList.Add(nameof (artifactTypeId), artifactTypeId);
      if (sourceId != null)
        keyValuePairList.Add(nameof (sourceId), sourceId);
      if (artifactVersionId != null)
        keyValuePairList.Add(nameof (artifactVersionId), artifactVersionId);
      if (sourceBranchFilter != null)
        keyValuePairList.Add(nameof (sourceBranchFilter), sourceBranchFilter);
      if (isDeleted.HasValue)
        keyValuePairList.Add(nameof (isDeleted), isDeleted.Value.ToString());
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdFilter), string.Join<int>(",", releaseIdFilter));
      return this.SendAsync<List<Release>>(method, locationId, version: new ApiResourceVersion(5.1, 8), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<Release> GetReleaseAsync(
      string project,
      int releaseId,
      bool? includeAllApprovals,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeAllApprovals.HasValue)
        keyValuePairList.Add(nameof (includeAllApprovals), includeAllApprovals.Value.ToString());
      return this.SendAsync<Release>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<Release> GetReleaseAsync(
      Guid project,
      int releaseId,
      bool? includeAllApprovals,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeAllApprovals.HasValue)
        keyValuePairList.Add(nameof (includeAllApprovals), includeAllApprovals.Value.ToString());
      return this.SendAsync<Release>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinition> GetReleaseDefinitionAsync(
      string project,
      int definitionId,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<ReleaseDefinition>(new HttpMethod("GET"), new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion("3.2-preview.3"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<ReleaseDefinition> GetReleaseDefinitionAsync(
      Guid project,
      int definitionId,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<ReleaseDefinition>(new HttpMethod("GET"), new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion("3.2-preview.3"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetDeploymentsAsync(project, definitionId = null, definitionEnvironmentId = null, createdBy = null, minModifiedTime = null, maxModifiedTime = null, deploymentStatus = null, operationStatus = null, latestAttemptsOnly = null, queryOrder = null, top = null, continuationToken = null, createdFor = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<Deployment>> GetDeploymentsAsync(
      string project,
      int? definitionId,
      int? definitionEnvironmentId,
      string createdBy,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime,
      DeploymentStatus? deploymentStatus,
      DeploymentOperationStatus? operationStatus,
      bool? latestAttemptsOnly,
      ReleaseQueryOrder? queryOrder,
      int? top,
      int? continuationToken,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b005ef73-cddc-448e-9ba2-5193bf36b19f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (!string.IsNullOrEmpty(createdBy))
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (minModifiedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minModifiedTime), minModifiedTime.Value);
      if (maxModifiedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxModifiedTime), maxModifiedTime.Value);
      if (deploymentStatus.HasValue)
        keyValuePairList.Add(nameof (deploymentStatus), deploymentStatus.Value.ToString());
      if (operationStatus.HasValue)
        keyValuePairList.Add(nameof (operationStatus), operationStatus.Value.ToString());
      if (latestAttemptsOnly.HasValue)
        keyValuePairList.Add(nameof (latestAttemptsOnly), latestAttemptsOnly.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      return this.SendAsync<List<Deployment>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetDeploymentsAsync(project, definitionId = null, definitionEnvironmentId = null, createdBy = null, minModifiedTime = null, maxModifiedTime = null, deploymentStatus = null, operationStatus = null, latestAttemptsOnly = null, queryOrder = null, top = null, continuationToken = null, createdFor = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<Deployment>> GetDeploymentsAsync(
      Guid project,
      int? definitionId,
      int? definitionEnvironmentId,
      string createdBy,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime,
      DeploymentStatus? deploymentStatus,
      DeploymentOperationStatus? operationStatus,
      bool? latestAttemptsOnly,
      ReleaseQueryOrder? queryOrder,
      int? top,
      int? continuationToken,
      object userState,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b005ef73-cddc-448e-9ba2-5193bf36b19f");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      int num;
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (definitionEnvironmentId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionEnvironmentId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionEnvironmentId), str);
      }
      if (!string.IsNullOrEmpty(createdBy))
        keyValuePairList.Add(nameof (createdBy), createdBy);
      if (minModifiedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minModifiedTime), minModifiedTime.Value);
      if (maxModifiedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxModifiedTime), maxModifiedTime.Value);
      if (deploymentStatus.HasValue)
        keyValuePairList.Add(nameof (deploymentStatus), deploymentStatus.Value.ToString());
      if (operationStatus.HasValue)
        keyValuePairList.Add(nameof (operationStatus), operationStatus.Value.ToString());
      if (latestAttemptsOnly.HasValue)
        keyValuePairList.Add(nameof (latestAttemptsOnly), latestAttemptsOnly.Value.ToString());
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      return this.SendAsync<List<Deployment>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseDefinitionsAsync(project, searchText = null, expand = null, artifactType = null, artifactSourceId = null, top = null, continuationToken = null, queryOrder = null, path = null, isExactNameMatch = null, tagFilter = null, propertyFilters = null, definitionIdFilter = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      string project,
      string searchText,
      ReleaseDefinitionExpands? expand,
      string artifactType,
      string artifactSourceId,
      int? top,
      string continuationToken,
      ReleaseDefinitionQueryOrder? queryOrder,
      string path,
      bool? isExactNameMatch,
      IEnumerable<string> tagFilter,
      IEnumerable<string> propertyFilters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(searchText))
        keyValuePairList.Add(nameof (searchText), searchText);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (!string.IsNullOrEmpty(artifactType))
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (!string.IsNullOrEmpty(artifactSourceId))
        keyValuePairList.Add(nameof (artifactSourceId), artifactSourceId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (isExactNameMatch.HasValue)
        keyValuePairList.Add(nameof (isExactNameMatch), isExactNameMatch.Value.ToString());
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseDefinitionsAsync(project, searchText = null, expand = null, artifactType = null, artifactSourceId = null, top = null, continuationToken = null, queryOrder = null, path = null, isExactNameMatch = null, tagFilter = null, propertyFilters = null, definitionIdFilter = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      Guid project,
      string searchText,
      ReleaseDefinitionExpands? expand,
      string artifactType,
      string artifactSourceId,
      int? top,
      string continuationToken,
      ReleaseDefinitionQueryOrder? queryOrder,
      string path,
      bool? isExactNameMatch,
      IEnumerable<string> tagFilter,
      IEnumerable<string> propertyFilters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(searchText))
        keyValuePairList.Add(nameof (searchText), searchText);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (!string.IsNullOrEmpty(artifactType))
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (!string.IsNullOrEmpty(artifactSourceId))
        keyValuePairList.Add(nameof (artifactSourceId), artifactSourceId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!string.IsNullOrEmpty(continuationToken))
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (isExactNameMatch.HasValue)
        keyValuePairList.Add(nameof (isExactNameMatch), isExactNameMatch.Value.ToString());
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseDefinitionsAsync(project, searchText = null, expand = null, artifactType = null, artifactSourceId = null, top = null, continuationToken = null, queryOrder = null, path = null, isExactNameMatch = null, tagFilter = null, propertyFilters = null, definitionIdFilter = null, isDeleted = null, searchOnPath = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      string project,
      string searchText = null,
      ReleaseDefinitionExpands? expand = null,
      string artifactType = null,
      string artifactSourceId = null,
      int? top = null,
      string continuationToken = null,
      ReleaseDefinitionQueryOrder? queryOrder = null,
      string path = null,
      bool? isExactNameMatch = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<string> definitionIdFilter = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactType != null)
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (artifactSourceId != null)
        keyValuePairList.Add(nameof (artifactSourceId), artifactSourceId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      bool flag;
      if (isExactNameMatch.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isExactNameMatch.Value;
        string str = flag.ToString();
        collection.Add(nameof (isExactNameMatch), str);
      }
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (definitionIdFilter != null && definitionIdFilter.Any<string>())
        keyValuePairList.Add(nameof (definitionIdFilter), string.Join(",", definitionIdFilter));
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseDefinitionsAsync(project, searchText = null, expand = null, artifactType = null, artifactSourceId = null, top = null, continuationToken = null, queryOrder = null, path = null, isExactNameMatch = null, tagFilter = null, propertyFilters = null, definitionIdFilter = null, isDeleted = null, searchOnPath = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync(
      Guid project,
      string searchText = null,
      ReleaseDefinitionExpands? expand = null,
      string artifactType = null,
      string artifactSourceId = null,
      int? top = null,
      string continuationToken = null,
      ReleaseDefinitionQueryOrder? queryOrder = null,
      string path = null,
      bool? isExactNameMatch = null,
      IEnumerable<string> tagFilter = null,
      IEnumerable<string> propertyFilters = null,
      IEnumerable<string> definitionIdFilter = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (searchText != null)
        keyValuePairList.Add(nameof (searchText), searchText);
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (artifactType != null)
        keyValuePairList.Add(nameof (artifactType), artifactType);
      if (artifactSourceId != null)
        keyValuePairList.Add(nameof (artifactSourceId), artifactSourceId);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (path != null)
        keyValuePairList.Add(nameof (path), path);
      bool flag;
      if (isExactNameMatch.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isExactNameMatch.Value;
        string str = flag.ToString();
        collection.Add(nameof (isExactNameMatch), str);
      }
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (definitionIdFilter != null && definitionIdFilter.Any<string>())
        keyValuePairList.Add(nameof (definitionIdFilter), string.Join(",", definitionIdFilter));
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      return this.SendAsync<List<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion(5.1, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetDefinitionEnvironmentsAsync(taskGroupId = null, propertyFilters = null) instead.", false)]
    public virtual Task<List<DefinitionEnvironmentReference>> GetDefinitionEnvironmentsAsync(
      string project,
      Guid? taskGroupId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("12b5d21a-f54c-430e-a8c1-7515d196890e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (taskGroupId.HasValue)
        keyValuePairList.Add(nameof (taskGroupId), taskGroupId.Value.ToString());
      return this.SendAsync<List<DefinitionEnvironmentReference>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetDefinitionEnvironmentsAsync(taskGroupId = null, propertyFilters = null) instead.", false)]
    public virtual Task<List<DefinitionEnvironmentReference>> GetDefinitionEnvironmentsAsync(
      Guid project,
      Guid? taskGroupId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("12b5d21a-f54c-430e-a8c1-7515d196890e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (taskGroupId.HasValue)
        keyValuePairList.Add(nameof (taskGroupId), taskGroupId.Value.ToString());
      return this.SendAsync<List<DefinitionEnvironmentReference>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteReleaseDefinitionAsync(
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion("4.0-preview.3"), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteReleaseDefinitionAsync(
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d8f96f24-8ea7-4cb6-baab-2df8fc515665"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion("4.0-preview.3"), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseAsync(project, releaseId, approvalFilters = null, propertyFilters = null) instead. Use ApprovalFilters.Manual | ApprovalFilters.Automated if you were using includeAllApprovals as false, and ApprovalFilters. All if you were using it as true", false)]
    public virtual Task<Release> GetReleaseAsync(
      string project,
      int releaseId,
      bool includeAllApprovals,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      ApprovalFilters approvalFilters = includeAllApprovals ? ApprovalFilters.All : ApprovalFilters.ManualApprovals | ApprovalFilters.AutomatedApprovals;
      keyValuePairList.Add("approvalFilters", approvalFilters.ToString());
      keyValuePairList.Add(nameof (includeAllApprovals), includeAllApprovals.ToString());
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<Release>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.6"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseAsync(project, releaseId, approvalFilters = null, propertyFilters = null) instead. Use ApprovalFilters.Manual | ApprovalFilters.Automated if you were using includeAllApprovals as false, and ApprovalFilters. All if you were using it as true", false)]
    public virtual Task<Release> GetReleaseAsync(
      Guid project,
      int releaseId,
      bool includeAllApprovals,
      IEnumerable<string> propertyFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      ApprovalFilters approvalFilters = includeAllApprovals ? ApprovalFilters.All : ApprovalFilters.ManualApprovals | ApprovalFilters.AutomatedApprovals;
      keyValuePairList.Add("approvalFilters", approvalFilters.ToString());
      keyValuePairList.Add(nameof (includeAllApprovals), includeAllApprovals.ToString());
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<Release>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.6"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseAsync(project, releaseId, approvalFilters = null, propertyFilters = null, expand = null) instead.", false)]
    public virtual Task<Release> GetReleaseAsync(
      string project,
      int releaseId,
      ApprovalFilters? approvalFilters,
      IEnumerable<string> propertyFilters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (approvalFilters.HasValue)
        keyValuePairList.Add(nameof (approvalFilters), approvalFilters.Value.ToString());
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<Release>(method, locationId, routeValues, new ApiResourceVersion("4.2-preview.6"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetReleaseAsync(project, releaseId, approvalFilters = null, propertyFilters = null, expand = null) instead.", false)]
    public virtual Task<Release> GetReleaseAsync(
      Guid project,
      int releaseId,
      ApprovalFilters? approvalFilters,
      IEnumerable<string> propertyFilters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a166fde7-27ad-408e-ba75-703c2cc9d500");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (approvalFilters.HasValue)
        keyValuePairList.Add(nameof (approvalFilters), approvalFilters.Value.ToString());
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      return this.SendAsync<Release>(method, locationId, routeValues, new ApiResourceVersion("4.2-preview.6"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetTaskLogAsync(project, releaseId, environmentId, releaseDeployPhaseId, taskId, startLine, endLine, userState, cancellationToken) instead.", false)]
    public virtual async Task<Stream> GetTaskLogAsync(
      string project,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int taskId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("17c91af7-09fd-4256-bff1-c24ee4f73bc0");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        releaseDeployPhaseId = releaseDeployPhaseId,
        taskId = taskId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetTaskLogAsync(project, releaseId, environmentId, releaseDeployPhaseId, taskId, startLine, endLine, userState, cancellationToken) instead.", false)]
    public virtual async Task<Stream> GetTaskLogAsync(
      Guid project,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int taskId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ReleaseCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("17c91af7-09fd-4256-bff1-c24ee4f73bc0");
      object routeValues = (object) new
      {
        project = project,
        releaseId = releaseId,
        environmentId = environmentId,
        releaseDeployPhaseId = releaseDeployPhaseId,
        taskId = taskId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.2"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }
  }
}
