// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProcessHttpClient
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
  public class ProcessHttpClient : VssHttpClientBase
  {
    public ProcessHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ProcessHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ProcessHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ProcessHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ProcessHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<Process> GetProcessByIdAsync(
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Process>(new HttpMethod("GET"), new Guid("93878975-88c5-4e6a-8abb-7ddd77a8a7d8"), (object) new
      {
        processId = processId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Process>> GetProcessesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Process>>(new HttpMethod("GET"), new Guid("93878975-88c5-4e6a-8abb-7ddd77a8a7d8"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
