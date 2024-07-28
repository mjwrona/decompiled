// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalHttpClientBase
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [ResourceArea("5B55A9B6-2E0F-40D7-829D-3741D2B8C4E4")]
  public abstract class ApprovalHttpClientBase : VssHttpClientBase
  {
    public ApprovalHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ApprovalHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ApprovalHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ApprovalHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ApprovalHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> GetApprovalAsync(
      string project,
      Guid approvalId,
      ApprovalDetailsExpandParameter? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("37794717-f36f-4d78-b2bf-4dc30d0cfbcd");
      object routeValues = (object) new
      {
        project = project,
        approvalId = approvalId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> GetApprovalAsync(
      Guid project,
      Guid approvalId,
      ApprovalDetailsExpandParameter? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("37794717-f36f-4d78-b2bf-4dc30d0cfbcd");
      object routeValues = (object) new
      {
        project = project,
        approvalId = approvalId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>> QueryApprovalsAsync(
      string project,
      IEnumerable<Guid> approvalIds = null,
      ApprovalDetailsExpandParameter? expand = null,
      IEnumerable<string> assignedTo = null,
      ApprovalStatus? state = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("37794717-f36f-4d78-b2bf-4dc30d0cfbcd");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (approvalIds != null && approvalIds.Any<Guid>())
        keyValuePairList.Add(nameof (approvalIds), string.Join<Guid>(",", approvalIds));
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (assignedTo != null && assignedTo.Any<string>())
        keyValuePairList.Add(nameof (assignedTo), string.Join(",", assignedTo));
      if (state.HasValue)
        keyValuePairList.Add(nameof (state), state.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>> QueryApprovalsAsync(
      Guid project,
      IEnumerable<Guid> approvalIds = null,
      ApprovalDetailsExpandParameter? expand = null,
      IEnumerable<string> assignedTo = null,
      ApprovalStatus? state = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("37794717-f36f-4d78-b2bf-4dc30d0cfbcd");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (approvalIds != null && approvalIds.Any<Guid>())
        keyValuePairList.Add(nameof (approvalIds), string.Join<Guid>(",", approvalIds));
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      if (assignedTo != null && assignedTo.Any<string>())
        keyValuePairList.Add(nameof (assignedTo), string.Join(",", assignedTo));
      if (state.HasValue)
        keyValuePairList.Add(nameof (state), state.Value.ToString());
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>> UpdateApprovalsAsync(
      string project,
      IEnumerable<ApprovalUpdateParameters> updateParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("37794717-f36f-4d78-b2bf-4dc30d0cfbcd");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ApprovalUpdateParameters>>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>> UpdateApprovalsAsync(
      Guid project,
      IEnumerable<ApprovalUpdateParameters> updateParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("37794717-f36f-4d78-b2bf-4dc30d0cfbcd");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<ApprovalUpdateParameters>>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<Microsoft.Azure.Pipelines.Approval.WebApi.Approval>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
