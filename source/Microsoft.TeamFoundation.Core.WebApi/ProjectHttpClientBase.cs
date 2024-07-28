// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProjectHttpClientBase
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("79134C72-4A58-4B42-976C-04E7115F32BF")]
  [ClientCancellationTimeout(45)]
  [ClientCircuitBreakerSettings(15, 50)]
  public abstract class ProjectHttpClientBase : ProjectCompatHttpClientBase
  {
    public ProjectHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ProjectHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ProjectHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ProjectHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ProjectHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task RemoveProjectAvatarAsync(
      string projectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("54b2a2a0-859b-4d05-827c-ec4c862f641a"), (object) new
      {
        projectId = projectId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task SetProjectAvatarAsync(
      ProjectAvatar avatarBlob,
      string projectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProjectHttpClientBase projectHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("54b2a2a0-859b-4d05-827c-ec4c862f641a");
      object obj1 = (object) new{ projectId = projectId };
      HttpContent httpContent = (HttpContent) new ObjectContent<ProjectAvatar>(avatarBlob, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      ProjectHttpClientBase projectHttpClientBase2 = projectHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await projectHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ProjectInfo>> GetProjectHistoryEntriesAsync(
      long? minRevision = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6488a877-4749-4954-82ea-7340d36be9f2");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (minRevision.HasValue)
        keyValuePairList.Add(nameof (minRevision), minRevision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<ProjectInfo>>(method, locationId, version: new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ProjectProperties>> GetProjectsPropertiesAsync(
      IEnumerable<Guid> projectIds,
      IEnumerable<string> properties = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0a3ffdfc-fe94-47a6-bb27-79bf3f762eac");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (projectIds != null)
        str = string.Join<Guid>(",", projectIds);
      keyValuePairList.Add(nameof (projectIds), str);
      if (properties != null && properties.Any<string>())
        keyValuePairList.Add(nameof (properties), string.Join(",", properties));
      return this.SendAsync<List<ProjectProperties>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<ProjectProperty>> GetProjectPropertiesAsync(
      Guid projectId,
      IEnumerable<string> keys = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4976a71a-4487-49aa-8aab-a1eda469037a");
      object routeValues = (object) new
      {
        projectId = projectId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (keys != null && keys.Any<string>())
        keyValuePairList.Add(nameof (keys), string.Join(",", keys));
      return this.SendAsync<List<ProjectProperty>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task SetProjectPropertiesAsync(
      Guid projectId,
      JsonPatchDocument patchDocument,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProjectHttpClientBase projectHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4976a71a-4487-49aa-8aab-a1eda469037a");
      object obj1 = (object) new{ projectId = projectId };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchDocument, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      ProjectHttpClientBase projectHttpClientBase2 = projectHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await projectHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
