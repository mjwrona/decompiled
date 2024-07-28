// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Client.PropertyCacheHttpClient
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

namespace Microsoft.VisualStudio.Services.Identity.Client
{
  [ResourceArea("0B808CEB-EF49-4C5E-9483-600A4ECF1224")]
  public class PropertyCacheHttpClient : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();
    private static readonly ApiResourceVersion s_currentApiVersion = new ApiResourceVersion(1.0);

    public PropertyCacheHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PropertyCacheHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PropertyCacheHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PropertyCacheHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PropertyCacheHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<string> Cache<T>(
      T value,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PropertyCacheHttpClient propertyCacheHttpClient = this;
      string str;
      using (new VssHttpClientBase.OperationScope(nameof (Cache), nameof (Cache)))
      {
        HttpContent content = (HttpContent) new ObjectContent<T>(value, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        str = await propertyCacheHttpClient.SendAsync<string>(HttpMethod.Put, PropertyCacheResourceIds.PropertyCache, version: PropertyCacheHttpClient.s_currentApiVersion, content: content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return str;
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) PropertyCacheHttpClient.s_translatedExceptions;
  }
}
