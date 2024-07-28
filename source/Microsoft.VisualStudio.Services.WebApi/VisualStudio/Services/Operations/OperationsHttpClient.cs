// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Operations.OperationsHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Operations
{
  public class OperationsHttpClient : OperationsHttpClientBase
  {
    public OperationsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public OperationsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public OperationsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public OperationsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public OperationsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<Operation> GetOperation(
      Guid id,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetOperationAsync(id, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Operation> GetOperationAsync(
      OperationReference operationReference,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetOperationAsync(operationReference.Id, new Guid?(operationReference.PluginId), userState, cancellationToken);
    }
  }
}
