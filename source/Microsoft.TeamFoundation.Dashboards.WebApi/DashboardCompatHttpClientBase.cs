// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardCompatHttpClientBase
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ResourceArea("31C84E0A-3ECE-48FD-A29D-100849AF99BA")]
  public class DashboardCompatHttpClientBase : VssHttpClientBase
  {
    public DashboardCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DashboardCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DashboardCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DashboardCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DashboardCompatHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<DashboardGroup> GetDashboardsAsync(
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
      return this.SendAsync<DashboardGroup>(method, locationId, routeValues, new ApiResourceVersion(6.0, 2), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
