// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.IVssConnection
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public interface IVssConnection : IDisposable
  {
    Microsoft.VisualStudio.Services.Identity.Identity AuthenticatedIdentity { get; }

    Microsoft.VisualStudio.Services.Identity.Identity AuthorizedIdentity { get; }

    VssCredentials Credentials { get; }

    IEnumerable<DelegatingHandler> DelegatingHandlers { get; }

    bool HasAuthenticated { get; }

    VssHttpMessageHandler InnerHandler { get; }

    IVssConnection ParentConnection { get; }

    Guid ServerId { get; }

    Guid ServerType { get; }

    VssClientHttpRequestSettings Settings { get; }

    Uri Uri { get; }

    Task ConnectAsync(CancellationToken cancellationToken = default (CancellationToken));

    Task ConnectAsync(VssConnectMode connectMode, CancellationToken cancellationToken = default (CancellationToken));

    Task ConnectAsync(
      VssConnectMode connectMode,
      IDictionary<string, string> parameters,
      CancellationToken cancellationToken = default (CancellationToken));

    void Disconnect();

    object GetClient(Type clientType);

    T GetClient<T>() where T : IVssHttpClient;

    T GetClient<T>(CancellationToken cancellationToken) where T : IVssHttpClient;

    T GetClient<T>(Guid serviceIdentifier) where T : IVssHttpClient;

    T GetClient<T>(Guid serviceIdentifier, CancellationToken cancellationToken) where T : IVssHttpClient;

    Task<T> GetClientAsync<T>(CancellationToken cancellationToken = default (CancellationToken)) where T : IVssHttpClient;

    Task<T> GetClientAsync<T>(Guid serviceIdentifier, CancellationToken cancellationToken = default (CancellationToken)) where T : IVssHttpClient;

    T GetService<T>() where T : IVssClientService;

    Task<T> GetServiceAsync<T>(CancellationToken cancellationToken = default (CancellationToken)) where T : IVssClientService;

    bool IsDisposed();
  }
}
