// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Client.UserPrivateHttpClientBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Users.Client
{
  [ResourceArea("970AA69F-E316-4D78-B7B0-B7137E47A22C")]
  public abstract class UserPrivateHttpClientBase : VssHttpClientBase
  {
    public UserPrivateHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public UserPrivateHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public UserPrivateHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public UserPrivateHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public UserPrivateHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual async Task DeletePrivateAttributeAsync(
      string descriptor,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("bde78236-6d43-4487-9fa0-1fafe5357d54"), (object) new
      {
        descriptor = descriptor,
        attributeName = attributeName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual Task<UserAttribute> GetPrivateAttributeAsync(
      string descriptor,
      string attributeName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UserAttribute>(new HttpMethod("GET"), new Guid("bde78236-6d43-4487-9fa0-1fafe5357d54"), (object) new
      {
        descriptor = descriptor,
        attributeName = attributeName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual Task<List<UserAttribute>> QueryPrivateAttributesAsync(
      string descriptor,
      string queryPattern = null,
      DateTimeOffset? modifiedAfter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bde78236-6d43-4487-9fa0-1fafe5357d54");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queryPattern != null)
        keyValuePairList.Add(nameof (queryPattern), queryPattern);
      if (modifiedAfter.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (modifiedAfter), modifiedAfter.Value);
      return this.SendAsync<List<UserAttribute>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual Task<List<UserAttribute>> SetPrivateAttributesAsync(
      string descriptor,
      IEnumerable<SetUserAttributeParameters> attributeParametersList,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("bde78236-6d43-4487-9fa0-1fafe5357d54");
      object obj1 = (object) new{ descriptor = descriptor };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<SetUserAttributeParameters>>(attributeParametersList, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<UserAttribute>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
