// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Settings.WebApi.SettingsHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Settings.WebApi
{
  public class SettingsHttpClient : VssHttpClientBase
  {
    public SettingsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SettingsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public SettingsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SettingsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public SettingsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<Dictionary<string, object>> GetEntriesAsync(
      string userScope,
      string key = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Dictionary<string, object>>(new HttpMethod("GET"), new Guid("cd006711-163d-4cd4-a597-b05bad2556ff"), (object) new
      {
        userScope = userScope,
        key = key
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RemoveEntriesAsync(
      string userScope,
      string key,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("cd006711-163d-4cd4-a597-b05bad2556ff"), (object) new
      {
        userScope = userScope,
        key = key
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task SetEntriesAsync(
      IDictionary<string, object> entries,
      string userScope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SettingsHttpClient settingsHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("cd006711-163d-4cd4-a597-b05bad2556ff");
      object obj1 = (object) new{ userScope = userScope };
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, object>>(entries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      SettingsHttpClient settingsHttpClient2 = settingsHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await settingsHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<Dictionary<string, object>> GetEntriesForScopeAsync(
      string userScope,
      string scopeName,
      string scopeValue,
      string key = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Dictionary<string, object>>(new HttpMethod("GET"), new Guid("4cbaafaf-e8af-4570-98d1-79ee99c56327"), (object) new
      {
        userScope = userScope,
        scopeName = scopeName,
        scopeValue = scopeValue,
        key = key
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RemoveEntriesForScopeAsync(
      string userScope,
      string scopeName,
      string scopeValue,
      string key,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4cbaafaf-e8af-4570-98d1-79ee99c56327"), (object) new
      {
        userScope = userScope,
        scopeName = scopeName,
        scopeValue = scopeValue,
        key = key
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task SetEntriesForScopeAsync(
      IDictionary<string, object> entries,
      string userScope,
      string scopeName,
      string scopeValue,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SettingsHttpClient settingsHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4cbaafaf-e8af-4570-98d1-79ee99c56327");
      object obj1 = (object) new
      {
        userScope = userScope,
        scopeName = scopeName,
        scopeValue = scopeValue
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IDictionary<string, object>>(entries, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      SettingsHttpClient settingsHttpClient2 = settingsHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await settingsHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
