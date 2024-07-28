// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TeamCompatHttpClientBase
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  public abstract class TeamCompatHttpClientBase : VssHttpClientBase
  {
    public TeamCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TeamCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TeamCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TeamCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TeamCompatHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [Obsolete("Use GetTeamMembersWithExtendedPropertiesAsync method", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<IEnumerable<IdentityRef>> GetTeamMembersAsync(
      string projectId,
      string teamId,
      int? top,
      int? skip,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("294c494c-2600-4d7e-b76c-3dd50c3c95be");
      object routeValues = (object) new
      {
        projectId = projectId,
        teamId = teamId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<IEnumerable<IdentityRef>>(method, locationId, routeValues, new ApiResourceVersion("4.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WebApiTeam>> GetTeamsAsync(
      string projectId,
      int? top,
      int? skip,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d30a3dd1-f8ba-442a-b86a-bd0c0c383e59");
      object routeValues = (object) new
      {
        projectId = projectId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<WebApiTeam>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WebApiTeam>> GetAllTeamsAsync(
      bool? mine,
      int? top,
      int? skip,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7a4d9ee9-3433-4347-b47a-7a80f1cf307e");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (mine.HasValue)
        keyValuePairList.Add("$mine", mine.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<WebApiTeam>>(method, locationId, version: new ApiResourceVersion(4.1, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<WebApiTeam> GetTeamAsync(
      string projectId,
      string teamId,
      object userState,
      CancellationToken cancellationToken)
    {
      return this.SendAsync<WebApiTeam>(new HttpMethod("GET"), new Guid("d30a3dd1-f8ba-442a-b86a-bd0c0c383e59"), (object) new
      {
        projectId = projectId,
        teamId = teamId
      }, new ApiResourceVersion(4.1, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<WebApiTeam>> GetTeamsAsync(
      string projectId,
      bool? mine,
      int? top,
      int? skip,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d30a3dd1-f8ba-442a-b86a-bd0c0c383e59");
      object routeValues = (object) new
      {
        projectId = projectId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (mine.HasValue)
        keyValuePairList.Add("$mine", mine.Value.ToString());
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      return this.SendAsync<List<WebApiTeam>>(method, locationId, routeValues, new ApiResourceVersion(4.1, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
