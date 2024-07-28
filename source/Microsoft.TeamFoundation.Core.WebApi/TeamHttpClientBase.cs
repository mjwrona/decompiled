// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TeamHttpClientBase
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("79134C72-4A58-4B42-976C-04E7115F32BF")]
  public abstract class TeamHttpClientBase : TeamCompatHttpClientBase
  {
    public TeamHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TeamHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TeamHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TeamHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TeamHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<CategorizedWebApiTeams> GetProjectTeamsByCategoryAsync(
      string projectId,
      bool? expandIdentity = null,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff");
      object routeValues = (object) new
      {
        projectId = projectId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expandIdentity.HasValue)
        keyValuePairList.Add("$expandIdentity", expandIdentity.Value.ToString());
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
      return this.SendAsync<CategorizedWebApiTeams>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<TeamMember>> GetTeamMembersWithExtendedPropertiesAsync(
      string projectId,
      string teamId,
      int? top = null,
      int? skip = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
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
      return this.SendAsync<List<TeamMember>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WebApiTeam>> GetAllTeamsAsync(
      bool? mine = null,
      int? top = null,
      int? skip = null,
      bool? expandIdentity = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7a4d9ee9-3433-4347-b47a-7a80f1cf307e");
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (mine.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = mine.Value;
        string str = flag.ToString();
        collection.Add("$mine", str);
      }
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
      if (expandIdentity.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = expandIdentity.Value;
        string str = flag.ToString();
        collection.Add("$expandIdentity", str);
      }
      return this.SendAsync<List<WebApiTeam>>(method, locationId, version: new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WebApiTeam> CreateTeamAsync(
      WebApiTeam team,
      string projectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d30a3dd1-f8ba-442a-b86a-bd0c0c383e59");
      object obj1 = (object) new{ projectId = projectId };
      HttpContent httpContent = (HttpContent) new ObjectContent<WebApiTeam>(team, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WebApiTeam>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteTeamAsync(
      string projectId,
      string teamId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d30a3dd1-f8ba-442a-b86a-bd0c0c383e59"), (object) new
      {
        projectId = projectId,
        teamId = teamId
      }, new ApiResourceVersion(7.2, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<WebApiTeam> GetTeamAsync(
      string projectId,
      string teamId,
      bool? expandIdentity = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d30a3dd1-f8ba-442a-b86a-bd0c0c383e59");
      object routeValues = (object) new
      {
        projectId = projectId,
        teamId = teamId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expandIdentity.HasValue)
        keyValuePairList.Add("$expandIdentity", expandIdentity.Value.ToString());
      return this.SendAsync<WebApiTeam>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<WebApiTeam>> GetTeamsAsync(
      string projectId,
      bool? mine = null,
      int? top = null,
      int? skip = null,
      bool? expandIdentity = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d30a3dd1-f8ba-442a-b86a-bd0c0c383e59");
      object routeValues = (object) new
      {
        projectId = projectId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (mine.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = mine.Value;
        string str = flag.ToString();
        collection.Add("$mine", str);
      }
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
      if (expandIdentity.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = expandIdentity.Value;
        string str = flag.ToString();
        collection.Add("$expandIdentity", str);
      }
      return this.SendAsync<List<WebApiTeam>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<WebApiTeam> UpdateTeamAsync(
      WebApiTeam teamData,
      string projectId,
      string teamId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d30a3dd1-f8ba-442a-b86a-bd0c0c383e59");
      object obj1 = (object) new
      {
        projectId = projectId,
        teamId = teamId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WebApiTeam>(teamData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 3);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WebApiTeam>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
