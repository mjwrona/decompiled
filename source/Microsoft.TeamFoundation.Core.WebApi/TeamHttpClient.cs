// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TeamHttpClient
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("79134C72-4A58-4B42-976C-04E7115F32BF")]
  public class TeamHttpClient : TeamHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();

    public TeamHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TeamHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TeamHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TeamHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TeamHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [Obsolete("Use GetTeamsAsync method", false)]
    public Task<IEnumerable<WebApiTeam>> GetTeams(
      string projectId,
      int? top = null,
      int? skip = null,
      object userState = null)
    {
      return this.GetTeamsAsyncWrapper(projectId, top, skip, userState);
    }

    [Obsolete("Use GetTeamAsync method", false)]
    public Task<WebApiTeam> GetTeam(string projectId, string teamId, object userState = null) => this.GetTeamAsync(projectId, teamId, userState: userState);

    [Obsolete("Use GetTeamMembersAsync method", false)]
    public Task<IEnumerable<IdentityRef>> GetTeamMembers(
      string projectId,
      string teamId,
      int? top = null,
      int? skip = null,
      object userState = null)
    {
      return this.GetTeamMembersAsync(projectId, teamId, top, skip, userState, new CancellationToken());
    }

    private async Task<IEnumerable<WebApiTeam>> GetTeamsAsyncWrapper(
      string projectId,
      int? top = null,
      int? skip = null,
      object userState = null)
    {
      return (IEnumerable<WebApiTeam>) await this.GetTeamsAsync(projectId, new bool?(false), top, skip, new bool?(false), userState).ConfigureAwait(false);
    }
  }
}
