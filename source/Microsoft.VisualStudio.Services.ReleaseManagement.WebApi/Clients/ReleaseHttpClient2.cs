// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients.ReleaseHttpClient2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients
{
  public class ReleaseHttpClient2 : ReleaseHttpClient
  {
    public ReleaseHttpClient2(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ReleaseHttpClient2(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ReleaseHttpClient2(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ReleaseHttpClient2(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ReleaseHttpClient2(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<IPagedCollection<ReleaseApproval>> GetApprovalsAsync2(
      string project,
      string assignedToFilter = null,
      ApprovalStatus? statusFilter = null,
      IEnumerable<int> releaseIdsFilter = null,
      ApprovalType? typeFilter = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseQueryOrder? queryOrder = null,
      bool? includeMyGroupApprovals = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b47c6458-e73b-47cb-a770-4df1e8813a91");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(assignedToFilter))
        keyValuePairList.Add(nameof (assignedToFilter), assignedToFilter);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (releaseIdsFilter != null && releaseIdsFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdsFilter), string.Join<int>(",", releaseIdsFilter));
      if (typeFilter.HasValue)
        keyValuePairList.Add(nameof (typeFilter), typeFilter.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (includeMyGroupApprovals.HasValue)
        keyValuePairList.Add(nameof (includeMyGroupApprovals), includeMyGroupApprovals.Value.ToString());
      return this.SendAsync<IPagedCollection<ReleaseApproval>>(method, locationId, routeValues, new ApiResourceVersion("3.0-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<ReleaseApproval>>>(this.GetPagedList<ReleaseApproval>));
    }

    public virtual Task<IPagedCollection<ReleaseApproval>> GetApprovalsAsync2(
      Guid project,
      string assignedToFilter = null,
      ApprovalStatus? statusFilter = null,
      IEnumerable<int> releaseIdsFilter = null,
      ApprovalType? typeFilter = null,
      int? top = null,
      int? continuationToken = null,
      ReleaseQueryOrder? queryOrder = null,
      bool? includeMyGroupApprovals = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("b47c6458-e73b-47cb-a770-4df1e8813a91");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(assignedToFilter))
        keyValuePairList.Add(nameof (assignedToFilter), assignedToFilter);
      if (statusFilter.HasValue)
        keyValuePairList.Add(nameof (statusFilter), statusFilter.Value.ToString());
      if (releaseIdsFilter != null && releaseIdsFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdsFilter), string.Join<int>(",", releaseIdsFilter));
      if (typeFilter.HasValue)
        keyValuePairList.Add(nameof (typeFilter), typeFilter.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (top), str);
      }
      if (continuationToken.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = continuationToken.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (continuationToken), str);
      }
      if (queryOrder.HasValue)
        keyValuePairList.Add(nameof (queryOrder), queryOrder.Value.ToString());
      if (includeMyGroupApprovals.HasValue)
        keyValuePairList.Add(nameof (includeMyGroupApprovals), includeMyGroupApprovals.Value.ToString());
      return this.SendAsync<IPagedCollection<ReleaseApproval>>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.2"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<ReleaseApproval>>>(this.GetPagedList<ReleaseApproval>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetDeploymentsAsync2(project, definitionId = null, definitionEnvironmentId = null, createdBy = null, minModifiedTime = null, maxModifiedTime = null, deploymentStatus = null, operationStatus = null, latestAttemptsOnly = null, queryOrder = null, top = null, continuationToken = null, createdFor = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public Task<IPagedCollection<Deployment>> GetDeploymentsAsync2(
      string project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string createdBy = null,
      DateTime? minModifiedTime = null,
      DateTime? maxModifiedTime = null,
      DeploymentStatus? deploymentStatus = null,
      DeploymentOperationStatus? operationStatus = null,
      bool? latestAttemptsOnly = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      object userState = null,
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
      return this.SendAsync<IPagedCollection<Deployment>>(method, locationId, routeValues, new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<Deployment>>>(this.GetPagedList<Deployment>));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This REST API is deprecated and will be removed in a future release. Use GetDeploymentsAsync2(project, definitionId = null, definitionEnvironmentId = null, createdBy = null, minModifiedTime = null, maxModifiedTime = null, deploymentStatus = null, operationStatus = null, latestAttemptsOnly = null, queryOrder = null, top = null, continuationToken = null, createdFor = null, userState = null, cancellationToken = default(CancellationToken)) instead.", false)]
    public virtual Task<IPagedCollection<Deployment>> GetDeploymentsAsync2(
      Guid project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string createdBy = null,
      DateTime? minModifiedTime = null,
      DateTime? maxModifiedTime = null,
      DeploymentStatus? deploymentStatus = null,
      DeploymentOperationStatus? operationStatus = null,
      bool? latestAttemptsOnly = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      object userState = null,
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
      return this.SendAsync<IPagedCollection<Deployment>>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<Deployment>>>(this.GetPagedList<Deployment>));
    }

    public Task<IPagedCollection<Deployment>> GetDeploymentsAsync2(
      string project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string createdBy = null,
      DateTime? minModifiedTime = null,
      DateTime? maxModifiedTime = null,
      DeploymentStatus? deploymentStatus = null,
      DeploymentOperationStatus? operationStatus = null,
      bool? latestAttemptsOnly = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      string createdFor = null,
      DateTime? minStartedTime = null,
      DateTime? maxStartedTime = null,
      string sourceBranch = null,
      object userState = null,
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
      if (createdFor != null)
        keyValuePairList.Add(nameof (createdFor), createdFor);
      if (minStartedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minStartedTime), minStartedTime.Value);
      if (maxStartedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxStartedTime), maxStartedTime.Value);
      if (sourceBranch != null)
        keyValuePairList.Add(nameof (sourceBranch), sourceBranch);
      return this.SendAsync<IPagedCollection<Deployment>>(method, locationId, routeValues, new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<Deployment>>>(this.GetPagedList<Deployment>));
    }

    public virtual Task<IPagedCollection<Deployment>> GetDeploymentsAsync2(
      Guid project,
      int? definitionId = null,
      int? definitionEnvironmentId = null,
      string createdBy = null,
      DateTime? minModifiedTime = null,
      DateTime? maxModifiedTime = null,
      DeploymentStatus? deploymentStatus = null,
      DeploymentOperationStatus? operationStatus = null,
      bool? latestAttemptsOnly = null,
      ReleaseQueryOrder? queryOrder = null,
      int? top = null,
      int? continuationToken = null,
      string createdFor = null,
      DateTime? minStartedTime = null,
      DateTime? maxStartedTime = null,
      string sourceBranch = null,
      object userState = null,
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
      if (createdFor != null)
        keyValuePairList.Add(nameof (createdFor), createdFor);
      if (minStartedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minStartedTime), minStartedTime.Value);
      if (maxStartedTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (maxStartedTime), maxStartedTime.Value);
      if (sourceBranch != null)
        keyValuePairList.Add(nameof (sourceBranch), sourceBranch);
      return this.SendAsync<IPagedCollection<Deployment>>(method, locationId, routeValues, new ApiResourceVersion("3.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<Deployment>>>(this.GetPagedList<Deployment>));
    }

    public Task<IPagedCollection<Release>> GetReleasesAsync2(
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
      return this.SendAsync<IPagedCollection<Release>>(method, locationId, routeValues, new ApiResourceVersion("3.0-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<Release>>>(this.GetPagedList<Release>));
    }

    public virtual Task<IPagedCollection<Release>> GetReleasesAsync2(
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
      string path = null,
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
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdFilter), string.Join<int>(",", releaseIdFilter));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      return this.SendAsync<IPagedCollection<Release>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<Release>>>(this.GetPagedList<Release>));
    }

    public virtual Task<IPagedCollection<Release>> GetReleasesAsync2(
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
      string path = null,
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
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdFilter), string.Join<int>(",", releaseIdFilter));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      return this.SendAsync<IPagedCollection<Release>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<Release>>>(this.GetPagedList<Release>));
    }

    public virtual Task<IPagedCollection<Release>> GetReleasesAsync2(
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
      string path = null,
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
      if (tagFilter != null && tagFilter.Any<string>())
        keyValuePairList.Add(nameof (tagFilter), string.Join(",", tagFilter));
      if (propertyFilters != null && propertyFilters.Any<string>())
        keyValuePairList.Add(nameof (propertyFilters), string.Join(",", propertyFilters));
      if (releaseIdFilter != null && releaseIdFilter.Any<int>())
        keyValuePairList.Add(nameof (releaseIdFilter), string.Join<int>(",", releaseIdFilter));
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      return this.SendAsync<IPagedCollection<Release>>(method, locationId, version: new ApiResourceVersion("3.2-preview.4"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<Release>>>(this.GetPagedList<Release>));
    }

    public virtual Task<IPagedCollection<ReleaseDefinition>> GetReleaseDefinitionsAsync2(
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
      bool? searchTextContainsFolderName = null,
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
      if (searchTextContainsFolderName.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = searchTextContainsFolderName.Value;
        string str = flag.ToString();
        collection.Add(nameof (searchTextContainsFolderName), str);
      }
      return this.SendAsync<IPagedCollection<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<ReleaseDefinition>>>(this.GetPagedList<ReleaseDefinition>));
    }

    public virtual Task<IPagedCollection<ReleaseDefinition>> GetReleaseDefinitionsAsync2(
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
      bool? searchTextContainsFolderName = null,
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
      if (searchTextContainsFolderName.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = searchTextContainsFolderName.Value;
        string str = flag.ToString();
        collection.Add(nameof (searchTextContainsFolderName), str);
      }
      return this.SendAsync<IPagedCollection<ReleaseDefinition>>(method, locationId, routeValues, new ApiResourceVersion("3.2-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken, processResponse: new Func<HttpResponseMessage, CancellationToken, Task<IPagedCollection<ReleaseDefinition>>>(this.GetPagedList<ReleaseDefinition>));
    }

    public virtual Task<List<ReleaseDefinition>> GetReleaseDefinitionsAsync3(
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
      List<KeyValuePair<string, string>> additionalHeaders = null,
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
      return this.SendAsync<List<ReleaseDefinition>>(method, (IEnumerable<KeyValuePair<string, string>>) additionalHeaders, locationId, routeValues, new ApiResourceVersion(5.0, 3), (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken, (Func<HttpResponseMessage, CancellationToken, Task<List<ReleaseDefinition>>>) null);
    }

    protected async Task<IPagedCollection<T>> GetPagedList<T>(
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      ReleaseHttpClient2 releaseHttpClient2 = this;
      string continuationToken = releaseHttpClient2.GetContinuationToken(responseMessage);
      IPagedCollection<T> pagedList = (IPagedCollection<T>) new PageableCollection<T>(await releaseHttpClient2.ReadContentAsAsync<List<T>>(responseMessage, cancellationToken).ConfigureAwait(false), continuationToken);
      continuationToken = (string) null;
      return pagedList;
    }

    protected string GetContinuationToken(HttpResponseMessage responseMessage)
    {
      if (responseMessage == null || responseMessage.Headers == null)
        return (string) null;
      string continuationToken = (string) null;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (responseMessage.Headers.TryGetValues("x-ms-continuationtoken", out values))
        continuationToken = values.FirstOrDefault<string>();
      return continuationToken;
    }

    protected async Task<T> SendAsync<T>(
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
      return await this.SendAsync<T>(method, (IEnumerable<KeyValuePair<string, string>>) null, locationId, routeValues, version, content, queryParameters, userState, cancellationToken, processResponse).ConfigureAwait(false);
    }

    protected async Task<T> SendAsync<T>(
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
      ReleaseHttpClient2 releaseHttpClient2 = this;
      T obj;
      using (VssTraceActivity.GetOrCreate().EnterCorrelationScope())
      {
        using (HttpRequestMessage requestMessage = await releaseHttpClient2.CreateRequestMessageAsync(method, additionalHeaders, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false))
          obj = await releaseHttpClient2.SendAsync<T>(requestMessage, userState, cancellationToken, processResponse).ConfigureAwait(false);
      }
      return obj;
    }

    protected async Task<T> SendAsync<T>(
      HttpRequestMessage message,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken),
      Func<HttpResponseMessage, CancellationToken, Task<T>> processResponse = null)
    {
      ReleaseHttpClient2 releaseHttpClient2 = this;
      if (processResponse == null)
        processResponse = new Func<HttpResponseMessage, CancellationToken, Task<T>>(((VssHttpClientBase) releaseHttpClient2).ReadContentAsAsync<T>);
      T obj;
      using (HttpResponseMessage response = await releaseHttpClient2.SendAsync(message, userState, cancellationToken).ConfigureAwait(false))
        obj = await processResponse(response, cancellationToken).ConfigureAwait(false);
      return obj;
    }
  }
}
