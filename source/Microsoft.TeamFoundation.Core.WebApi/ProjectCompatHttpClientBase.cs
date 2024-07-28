// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProjectCompatHttpClientBase
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ProjectCompatHttpClientBase : VssHttpClientBase
  {
    public ProjectCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ProjectCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ProjectCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ProjectCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ProjectCompatHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public Task<IEnumerable<TeamProjectReference>> GetProjects(
      ProjectState? stateFilter,
      int? top,
      int? skip,
      object userState)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      QueryParamHelper.AddNonNullParam(keyValuePairList, nameof (stateFilter), (object) stateFilter);
      QueryParamHelper.AddNonNullParam(keyValuePairList, "$top", (object) top);
      QueryParamHelper.AddNonNullParam(keyValuePairList, "$skip", (object) skip);
      return this.GetAsync<IEnumerable<TeamProjectReference>>(CoreConstants.ProjectsLocationId, version: new ApiResourceVersion("2.0-preview.3"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState);
    }
  }
}
