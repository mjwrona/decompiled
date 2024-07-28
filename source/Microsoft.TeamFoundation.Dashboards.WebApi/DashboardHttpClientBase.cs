// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardHttpClientBase
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [ResourceArea("31C84E0A-3ECE-48FD-A29D-100849AF99BA")]
  public abstract class DashboardHttpClientBase : DashboardCompatHttpClientBase
  {
    public DashboardHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DashboardHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DashboardHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DashboardHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DashboardHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<CopyDashboardResponse> CopyDashboardAsync(
      CopyDashboardOptions copyDashboardOptions,
      TeamContext teamContext,
      Guid dashboardId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid1 = new Guid("454b3e51-2e6e-48d4-ad81-978154089351");
      string str3 = str2;
      Guid guid2 = dashboardId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid2
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CopyDashboardOptions>(copyDashboardOptions, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<CopyDashboardResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Dashboard> CreateDashboardAsync(
      Dashboard dashboard,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("454b3e51-2e6e-48d4-ad81-978154089351");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Dashboard>(dashboard, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Dashboard>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteDashboardAsync(
      TeamContext teamContext,
      Guid dashboardId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DashboardHttpClientBase dashboardHttpClientBase = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("454b3e51-2e6e-48d4-ad81-978154089351");
      string str3 = str2;
      Guid guid = dashboardId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid
      };
      using (await dashboardHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Dashboard> GetDashboardAsync(
      TeamContext teamContext,
      Guid dashboardId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("454b3e51-2e6e-48d4-ad81-978154089351");
      string str3 = str2;
      Guid guid = dashboardId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid
      };
      return this.SendAsync<Dashboard>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Dashboard>> GetDashboardsByProjectAsync(
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("454b3e51-2e6e-48d4-ad81-978154089351");
      string str3 = str2;
      object routeValues = (object) new
      {
        project = str1,
        team = str3
      };
      return this.SendAsync<List<Dashboard>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Dashboard> ReplaceDashboardAsync(
      Dashboard dashboard,
      TeamContext teamContext,
      Guid dashboardId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid1 = new Guid("454b3e51-2e6e-48d4-ad81-978154089351");
      string str3 = str2;
      Guid guid2 = dashboardId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid2
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Dashboard>(dashboard, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Dashboard>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DashboardGroup> ReplaceDashboardsAsync(
      DashboardGroup group,
      TeamContext teamContext,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("454b3e51-2e6e-48d4-ad81-978154089351");
      string str3 = str2;
      object obj1 = (object) new
      {
        project = str1,
        team = str3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<DashboardGroup>(group, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DashboardGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Widget> CreateWidgetAsync(
      Widget widget,
      TeamContext teamContext,
      Guid dashboardId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid1 = new Guid("bdcff53a-8355-4172-a00a-40497ea23afc");
      string str3 = str2;
      Guid guid2 = dashboardId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid2
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Widget>(widget, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Widget>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Dashboard> DeleteWidgetAsync(
      TeamContext teamContext,
      Guid dashboardId,
      Guid widgetId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("bdcff53a-8355-4172-a00a-40497ea23afc");
      string str3 = str2;
      Guid guid1 = dashboardId;
      Guid guid2 = widgetId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid1,
        widgetId = guid2
      };
      return this.SendAsync<Dashboard>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Widget> GetWidgetAsync(
      TeamContext teamContext,
      Guid dashboardId,
      Guid widgetId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bdcff53a-8355-4172-a00a-40497ea23afc");
      string str3 = str2;
      Guid guid1 = dashboardId;
      Guid guid2 = widgetId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid1,
        widgetId = guid2
      };
      return this.SendAsync<Widget>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task<WidgetsVersionedList> GetWidgetsAsync(
      TeamContext teamContext,
      Guid dashboardId,
      string eTag = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DashboardHttpClientBase dashboardHttpClientBase = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bdcff53a-8355-4172-a00a-40497ea23afc");
      string str3 = str2;
      Guid guid = dashboardId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(eTag))
        keyValuePairList.Add("ETag", eTag);
      WidgetsVersionedList widgetsAsync;
      using (HttpRequestMessage requestMessage = await dashboardHttpClientBase.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WidgetsVersionedList returnObject = new WidgetsVersionedList();
        using (HttpResponseMessage response = await dashboardHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = dashboardHttpClientBase.GetHeaderValue(response, "ETag");
          WidgetsVersionedList widgetsVersionedList = returnObject;
          widgetsVersionedList.Widgets = (IEnumerable<Widget>) await dashboardHttpClientBase.ReadContentAsAsync<List<Widget>>(response, cancellationToken).ConfigureAwait(false);
          widgetsVersionedList = (WidgetsVersionedList) null;
        }
        widgetsAsync = returnObject;
      }
      return widgetsAsync;
    }

    public virtual Task<Widget> ReplaceWidgetAsync(
      Widget widget,
      TeamContext teamContext,
      Guid dashboardId,
      Guid widgetId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid1 = new Guid("bdcff53a-8355-4172-a00a-40497ea23afc");
      string str3 = str2;
      Guid guid2 = dashboardId;
      Guid guid3 = widgetId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid2,
        widgetId = guid3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Widget>(widget, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Widget>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task<WidgetsVersionedList> ReplaceWidgetsAsync(
      IEnumerable<Widget> widgets,
      TeamContext teamContext,
      Guid dashboardId,
      string eTag = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DashboardHttpClientBase dashboardHttpClientBase = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("bdcff53a-8355-4172-a00a-40497ea23afc");
      string str3 = str2;
      Guid guid = dashboardId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid
      };
      HttpContent content = (HttpContent) new ObjectContent<IEnumerable<Widget>>(widgets, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(eTag))
        keyValuePairList.Add("ETag", eTag);
      WidgetsVersionedList widgetsVersionedList1;
      using (HttpRequestMessage requestMessage = await dashboardHttpClientBase.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), content, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WidgetsVersionedList returnObject = new WidgetsVersionedList();
        using (HttpResponseMessage response = await dashboardHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = dashboardHttpClientBase.GetHeaderValue(response, "ETag");
          WidgetsVersionedList widgetsVersionedList = returnObject;
          widgetsVersionedList.Widgets = (IEnumerable<Widget>) await dashboardHttpClientBase.ReadContentAsAsync<List<Widget>>(response, cancellationToken).ConfigureAwait(false);
          widgetsVersionedList = (WidgetsVersionedList) null;
        }
        widgetsVersionedList1 = returnObject;
      }
      return widgetsVersionedList1;
    }

    public virtual Task<Widget> UpdateWidgetAsync(
      Widget widget,
      TeamContext teamContext,
      Guid dashboardId,
      Guid widgetId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid1 = new Guid("bdcff53a-8355-4172-a00a-40497ea23afc");
      string str3 = str2;
      Guid guid2 = dashboardId;
      Guid guid3 = widgetId;
      object obj1 = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid2,
        widgetId = guid3
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Widget>(widget, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid1;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Widget>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task<WidgetsVersionedList> UpdateWidgetsAsync(
      IEnumerable<Widget> widgets,
      TeamContext teamContext,
      Guid dashboardId,
      string eTag = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DashboardHttpClientBase dashboardHttpClientBase = this;
      if (teamContext == null)
        throw new ArgumentNullException(nameof (teamContext));
      string str1 = teamContext.ProjectId.HasValue ? teamContext.ProjectId.ToString() : teamContext.Project;
      string str2 = teamContext.TeamId.HasValue ? teamContext.TeamId.ToString() : teamContext.Team;
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("bdcff53a-8355-4172-a00a-40497ea23afc");
      string str3 = str2;
      Guid guid = dashboardId;
      object routeValues = (object) new
      {
        project = str1,
        team = str3,
        dashboardId = guid
      };
      HttpContent content = (HttpContent) new ObjectContent<IEnumerable<Widget>>(widgets, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(eTag))
        keyValuePairList.Add("ETag", eTag);
      WidgetsVersionedList widgetsVersionedList1;
      using (HttpRequestMessage requestMessage = await dashboardHttpClientBase.CreateRequestMessageAsync(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, locationId, routeValues, new ApiResourceVersion("7.2-preview.2"), content, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        WidgetsVersionedList returnObject = new WidgetsVersionedList();
        using (HttpResponseMessage response = await dashboardHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ETag = dashboardHttpClientBase.GetHeaderValue(response, "ETag");
          WidgetsVersionedList widgetsVersionedList = returnObject;
          widgetsVersionedList.Widgets = (IEnumerable<Widget>) await dashboardHttpClientBase.ReadContentAsAsync<List<Widget>>(response, cancellationToken).ConfigureAwait(false);
          widgetsVersionedList = (WidgetsVersionedList) null;
        }
        widgetsVersionedList1 = returnObject;
      }
      return widgetsVersionedList1;
    }

    public virtual Task<WidgetMetadataResponse> GetWidgetMetadataAsync(
      string contributionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WidgetMetadataResponse>(new HttpMethod("GET"), new Guid("6b3628d3-e96f-4fc7-b176-50240b03b515"), (object) new
      {
        contributionId = contributionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WidgetMetadataResponse> GetWidgetMetadataAsync(
      string project,
      string contributionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WidgetMetadataResponse>(new HttpMethod("GET"), new Guid("6b3628d3-e96f-4fc7-b176-50240b03b515"), (object) new
      {
        project = project,
        contributionId = contributionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WidgetMetadataResponse> GetWidgetMetadataAsync(
      Guid project,
      string contributionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WidgetMetadataResponse>(new HttpMethod("GET"), new Guid("6b3628d3-e96f-4fc7-b176-50240b03b515"), (object) new
      {
        project = project,
        contributionId = contributionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WidgetTypesResponse> GetWidgetTypesAsync(
      string project,
      WidgetScope scope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6b3628d3-e96f-4fc7-b176-50240b03b515");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$scope", scope.ToString());
      return this.SendAsync<WidgetTypesResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WidgetTypesResponse> GetWidgetTypesAsync(
      Guid project,
      WidgetScope scope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6b3628d3-e96f-4fc7-b176-50240b03b515");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$scope", scope.ToString());
      return this.SendAsync<WidgetTypesResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WidgetTypesResponse> GetWidgetTypesAsync(
      WidgetScope scope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6b3628d3-e96f-4fc7-b176-50240b03b515");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add("$scope", scope.ToString());
      return this.SendAsync<WidgetTypesResponse>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
