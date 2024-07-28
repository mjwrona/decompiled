// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ContentValidationHttpClient
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("contentValidation")]
  public class ContentValidationHttpClient : VssHttpClientBase
  {
    public ContentValidationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ContentValidationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ContentValidationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ContentValidationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ContentValidationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task UpdateTakedownStatusAsync(
      Guid sourceProjectId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContentValidationHttpClient validationHttpClient = this;
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("7ae2f97a-5cca-4a0a-ac90-81dd689f26f5");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (sourceProjectId), sourceProjectId.ToString());
      using (await validationHttpClient.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
