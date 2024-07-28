// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Client.UserCompatHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Users.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class UserCompatHttpClientBase : VssHttpClientBase
  {
    public UserCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public UserCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public UserCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public UserCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public UserCompatHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    protected virtual Task<User> GetUserAsync(
      string descriptor,
      bool? createIfNotExists,
      SubjectDescriptor? knownDescriptor,
      object userState,
      CancellationToken cancellationToken)
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("61117502-a055-422c-9122-b56e6643ed02");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (createIfNotExists.HasValue)
        keyValuePairList.Add("X-VSS-FaultInUser", createIfNotExists.Value.ToString());
      if (knownDescriptor.HasValue)
        keyValuePairList.Add("X-VSS-KnownDescriptor", knownDescriptor.Value.ToString());
      return this.SendAsync<User>(method, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, locationId, routeValues, new ApiResourceVersion(5.0, 2), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
