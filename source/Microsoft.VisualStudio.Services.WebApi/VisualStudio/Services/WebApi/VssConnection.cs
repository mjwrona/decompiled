// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssConnection
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.WebApi.Location;
using Microsoft.VisualStudio.Services.WebApi.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class VssConnection : IVssConnection, IDisposable
  {
    private bool m_isDisposed;
    private object m_disposeLock = new object();
    private IVssServerDataProvider m_serverDataProvider;
    private VssConnection m_parentConnection;
    private object m_parentConnectionLock = new object();
    private readonly Uri m_baseUrl;
    private readonly HttpMessageHandler m_pipeline;
    private readonly VssHttpMessageHandler m_innerHandler;
    private readonly IEnumerable<DelegatingHandler> m_delegatingHandlers;
    private readonly bool m_allowUnattributedClients;
    private readonly ConcurrentDictionary<VssConnection.ClientCacheKey, AsyncLock> m_loadingTypes = new ConcurrentDictionary<VssConnection.ClientCacheKey, AsyncLock>(VssConnection.ClientCacheKey.Comparer);
    private readonly ConcurrentDictionary<VssConnection.ClientCacheKey, object> m_cachedTypes = new ConcurrentDictionary<VssConnection.ClientCacheKey, object>(VssConnection.ClientCacheKey.Comparer);
    private readonly ConcurrentDictionary<string, Type> m_extensibleServiceTypes = new ConcurrentDictionary<string, Type>();

    public VssConnection(Uri baseUrl, VssCredentials credentials)
      : this(baseUrl, credentials, (VssHttpRequestSettings) VssClientHttpRequestSettings.Default.Clone())
    {
    }

    public VssConnection(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : this(baseUrl, new VssHttpMessageHandler(credentials, settings), (IEnumerable<DelegatingHandler>) null)
    {
    }

    public VssConnection(
      Uri baseUrl,
      VssHttpMessageHandler innerHandler,
      IEnumerable<DelegatingHandler> delegatingHandlers)
      : this(baseUrl, innerHandler, delegatingHandlers, true)
    {
    }

    private VssConnection(
      Uri baseUrl,
      VssHttpMessageHandler innerHandler,
      IEnumerable<DelegatingHandler> delegatingHandlers,
      bool allowUnattributedClients)
    {
      ArgumentUtility.CheckForNull<Uri>(baseUrl, nameof (baseUrl));
      ArgumentUtility.CheckForNull<VssHttpMessageHandler>(innerHandler, nameof (innerHandler));
      this.m_delegatingHandlers = delegatingHandlers = delegatingHandlers ?? Enumerable.Empty<DelegatingHandler>();
      this.m_baseUrl = baseUrl;
      this.m_innerHandler = innerHandler;
      this.m_allowUnattributedClients = allowUnattributedClients;
      if (this.Settings.MaxRetryRequest > 0)
        delegatingHandlers = delegatingHandlers.Concat<DelegatingHandler>((IEnumerable<DelegatingHandler>) new DelegatingHandler[1]
        {
          (DelegatingHandler) new VssHttpRetryMessageHandler(this.Settings.MaxRetryRequest)
        });
      this.m_pipeline = !delegatingHandlers.Any<DelegatingHandler>() ? (HttpMessageHandler) this.m_innerHandler : HttpClientFactory.CreatePipeline((HttpMessageHandler) this.m_innerHandler, delegatingHandlers);
      this.m_serverDataProvider = (IVssServerDataProvider) new VssServerDataProvider(this, this.m_pipeline, this.m_baseUrl.AbsoluteUri);
      if (innerHandler.Credentials == null)
        return;
      if (innerHandler.Credentials.Federated != null)
        innerHandler.Credentials.Federated.TokenStorageUrl = baseUrl;
      if (innerHandler.Credentials.Windows == null)
        return;
      innerHandler.Credentials.Windows.TokenStorageUrl = baseUrl;
    }

    public Task ConnectAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ConnectAsync(VssConnectMode.Automatic, (IDictionary<string, string>) null, cancellationToken);

    public Task ConnectAsync(VssConnectMode connectMode, CancellationToken cancellationToken = default (CancellationToken)) => this.ConnectAsync(connectMode, (IDictionary<string, string>) null, cancellationToken);

    public Task ConnectAsync(
      VssConnectMode connectMode,
      IDictionary<string, string> parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.CheckForDisposed();
      if (this.Credentials.Federated != null && this.Credentials.Federated.Prompt != null)
      {
        parameters = parameters == null ? (IDictionary<string, string>) new Dictionary<string, string>() : (IDictionary<string, string>) new Dictionary<string, string>(parameters);
        IVssCredentialPrompt credentialPrompt = !(this.Credentials.Federated.Prompt is IVssCredentialPrompts prompt) || prompt.FederatedPrompt == null ? this.Credentials.Federated.Prompt : prompt.FederatedPrompt;
        parameters["vssConnectionMode"] = connectMode.ToString();
        credentialPrompt.Parameters = parameters;
      }
      return this.ServerDataProvider.ConnectAsync(ConnectOptions.None, cancellationToken);
    }

    public void Disconnect()
    {
      try
      {
        if (!this.HasAuthenticated)
          return;
        this.m_innerHandler.Credentials.SignOut(this.Uri, (Uri) null, (string) null);
      }
      finally
      {
        this.ServerDataProvider.DisconnectAsync().SyncResult();
      }
    }

    public T GetService<T>() where T : IVssClientService => (T) this.GetClientServiceImplAsync(typeof (T), Guid.Empty, new Func<Type, Guid, CancellationToken, Task<object>>(this.GetServiceInstanceAsync)).SyncResult<object>();

    public async Task<T> GetServiceAsync<T>(CancellationToken cancellationToken = default (CancellationToken)) where T : IVssClientService
    {
      VssConnection vssConnection = this;
      return (T) await vssConnection.GetClientServiceImplAsync(typeof (T), Guid.Empty, new Func<Type, Guid, CancellationToken, Task<object>>(vssConnection.GetServiceInstanceAsync), cancellationToken).ConfigureAwait(false);
    }

    public T GetClient<T>() where T : IVssHttpClient => this.GetClientAsync<T>(new CancellationToken()).SyncResult<T>();

    public T GetClient<T>(Guid serviceIdentifier) where T : IVssHttpClient => this.GetClientAsync<T>(serviceIdentifier, new CancellationToken()).SyncResult<T>();

    public T GetClient<T>(CancellationToken cancellationToken) where T : IVssHttpClient => this.GetClientAsync<T>(cancellationToken).SyncResult<T>();

    public T GetClient<T>(Guid serviceIdentifier, CancellationToken cancellationToken) where T : IVssHttpClient => this.GetClientAsync<T>(serviceIdentifier, cancellationToken).SyncResult<T>();

    public async Task<T> GetClientAsync<T>(CancellationToken cancellationToken = default (CancellationToken)) where T : IVssHttpClient
    {
      VssConnection vssConnection = this;
      vssConnection.CheckForDisposed();
      Type type = typeof (T);
      Guid serviceIdentifier = vssConnection.GetServiceIdentifier(type);
      if (serviceIdentifier == Guid.Empty && !vssConnection.m_allowUnattributedClients)
        throw new CannotGetUnattributedClientException(type);
      return (T) await vssConnection.GetClientServiceImplAsync(typeof (T), serviceIdentifier, new Func<Type, Guid, CancellationToken, Task<object>>(vssConnection.GetClientInstanceAsync), cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> GetClientAsync<T>(
      Guid serviceIdentifier,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : IVssHttpClient
    {
      VssConnection vssConnection = this;
      return (T) await vssConnection.GetClientServiceImplAsync(typeof (T), serviceIdentifier, new Func<Type, Guid, CancellationToken, Task<object>>(vssConnection.GetClientInstanceAsync), cancellationToken).ConfigureAwait(false);
    }

    public static async Task<VssConnection> CreateAsync(
      string organizationName,
      VssCredentials credentials,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return new VssConnection(await VssConnectionHelper.GetOrganizationUrlAsync(organizationName, cancellationToken).ConfigureAwait(false), credentials);
    }

    public static async Task<VssConnection> CreateAsync(
      string organizationName,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return new VssConnection(await VssConnectionHelper.GetOrganizationUrlAsync(organizationName, cancellationToken).ConfigureAwait(false), credentials, settings);
    }

    public static async Task<VssConnection> CreateAsync(
      string organizationName,
      VssHttpMessageHandler innerHandler,
      IEnumerable<DelegatingHandler> delegatingHandler,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return new VssConnection(await VssConnectionHelper.GetOrganizationUrlAsync(organizationName, cancellationToken).ConfigureAwait(false), innerHandler, delegatingHandler);
    }

    public object GetClient(Type clientType)
    {
      Type type = typeof (IVssHttpClient);
      return type.GetTypeInfo().IsAssignableFrom(clientType.GetTypeInfo()) ? this.GetClientServiceImplAsync(clientType, this.GetServiceIdentifier(clientType), new Func<Type, Guid, CancellationToken, Task<object>>(this.GetClientInstanceAsync)).SyncResult<object>() : throw new ArgumentException(type.FullName);
    }

    private async Task<object> GetClientServiceImplAsync(
      Type requestedType,
      Guid serviceIdentifier,
      Func<Type, Guid, CancellationToken, Task<object>> getInstanceAsync,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.CheckForDisposed();
      object obj = (object) null;
      Type managedType = this.GetExtensibleType(requestedType);
      VssConnection.ClientCacheKey cacheKey = new VssConnection.ClientCacheKey(managedType, serviceIdentifier);
      if (!this.m_cachedTypes.TryGetValue(cacheKey, out obj) || obj is IVssHttpClient vssHttpClient1 && vssHttpClient1.IsDisposed())
      {
        IDisposable disposable = await this.m_loadingTypes.GetOrAdd(cacheKey, (Func<VssConnection.ClientCacheKey, AsyncLock>) (t => new AsyncLock())).LockAsync(cancellationToken).ConfigureAwait(false);
        try
        {
          if (this.m_cachedTypes.TryGetValue(cacheKey, out obj))
          {
            if (obj is IVssHttpClient vssHttpClient)
            {
              if (!vssHttpClient.IsDisposed())
                goto label_10;
            }
            else
              goto label_10;
          }
          obj = await getInstanceAsync(managedType, serviceIdentifier, cancellationToken).ConfigureAwait(false);
          this.m_cachedTypes[cacheKey] = obj;
          this.m_loadingTypes.TryRemove(cacheKey, out AsyncLock _);
        }
        finally
        {
          disposable?.Dispose();
        }
label_10:
        disposable = (IDisposable) null;
      }
      object serviceImplAsync = obj;
      managedType = (Type) null;
      cacheKey = new VssConnection.ClientCacheKey();
      return serviceImplAsync;
    }

    private Task<object> GetClientInstanceAsync(
      Type managedType,
      Guid serviceIdentifier,
      CancellationToken cancellationToken)
    {
      return this.GetClientInstanceAsync(managedType, serviceIdentifier, cancellationToken, (VssHttpRequestSettings) null, (DelegatingHandler[]) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal Task<object> GetClientInstanceAsync(
      Type managedType,
      CancellationToken cancellationToken,
      VssHttpRequestSettings settings,
      DelegatingHandler[] handlers)
    {
      return this.GetClientInstanceAsync(managedType, this.GetServiceIdentifier(managedType), cancellationToken, settings, handlers);
    }

    private async Task<object> GetClientInstanceAsync(
      Type managedType,
      Guid serviceIdentifier,
      CancellationToken cancellationToken,
      VssHttpRequestSettings settings,
      DelegatingHandler[] handlers)
    {
      this.CheckForDisposed();
      ILocationDataProvider locationData = await (await this.GetServiceAsync<ILocationService>(cancellationToken).ConfigureAwait(false)).GetLocationDataAsync(serviceIdentifier, cancellationToken).ConfigureAwait(false);
      if (locationData == null)
        throw new VssServiceException(WebApiResources.ServerDataProviderNotFound((object) serviceIdentifier));
      Uri uri = new Uri(await locationData.LocationForCurrentConnectionAsync("LocationService2", LocationServiceConstants.SelfReferenceIdentifier, cancellationToken).ConfigureAwait(false));
      IVssHttpClient toReturn;
      if (settings != null)
        toReturn = (IVssHttpClient) Activator.CreateInstance(managedType, (object) uri, (object) this.Credentials, (object) settings, (object) handlers);
      else
        toReturn = (IVssHttpClient) Activator.CreateInstance(managedType, (object) uri, (object) this.m_pipeline, (object) false);
      toReturn.SetResourceLocations(await locationData.GetResourceLocationsAsync(cancellationToken).ConfigureAwait(false));
      object clientInstanceAsync = (object) toReturn;
      locationData = (ILocationDataProvider) null;
      toReturn = (IVssHttpClient) null;
      return clientInstanceAsync;
    }

    private Guid GetServiceIdentifier(Type requestedType)
    {
      ResourceAreaAttribute[] customAttributes = (ResourceAreaAttribute[]) requestedType.GetTypeInfo().GetCustomAttributes<ResourceAreaAttribute>(true);
      return customAttributes.Length != 0 ? customAttributes[0].AreaId : Guid.Empty;
    }

    private Task<object> GetServiceInstanceAsync(
      Type managedType,
      Guid serviceIdentifier,
      CancellationToken cancellationToken)
    {
      this.CheckForDisposed();
      IVssClientService instance;
      try
      {
        instance = (IVssClientService) Activator.CreateInstance(managedType);
      }
      catch (MissingMemberException ex)
      {
        throw new ArgumentException(WebApiResources.GetServiceArgumentError((object) managedType), (Exception) ex);
      }
      instance.Initialize(this);
      return Task.FromResult<object>((object) instance);
    }

    private Type GetExtensibleType(Type managedType)
    {
      if (!managedType.GetTypeInfo().IsAbstract && !managedType.GetTypeInfo().IsInterface)
        return managedType;
      Type type = (Type) null;
      if (!this.m_extensibleServiceTypes.TryGetValue(managedType.Name, out type))
      {
        VssClientServiceImplementationAttribute[] customAttributes = (VssClientServiceImplementationAttribute[]) managedType.GetTypeInfo().GetCustomAttributes<VssClientServiceImplementationAttribute>(true);
        if (customAttributes.Length != 0)
        {
          if (customAttributes[0].Type != (Type) null)
          {
            type = customAttributes[0].Type;
            this.m_extensibleServiceTypes[managedType.Name] = type;
          }
          else if (!string.IsNullOrEmpty(customAttributes[0].TypeName))
          {
            type = Type.GetType(customAttributes[0].TypeName);
            if (type != (Type) null)
              this.m_extensibleServiceTypes[managedType.Name] = type;
          }
        }
      }
      if (type == (Type) null)
        throw new ExtensibleServiceTypeNotRegisteredException(managedType);
      return managedType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? type : throw new ExtensibleServiceTypeNotValidException(managedType, type);
    }

    internal void RegisterExtensibleType(string typeName, Type type)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(typeName, nameof (typeName));
      ArgumentUtility.CheckForNull<Type>(type, nameof (type));
      this.m_extensibleServiceTypes[typeName] = type;
    }

    internal void RegisterClientServiceInstance(Type type, object instance)
    {
      ArgumentUtility.CheckForNull<Type>(type, nameof (type));
      ArgumentUtility.CheckForNull<object>(instance, nameof (instance));
      this.CheckForDisposed();
      Type type1 = type.GetTypeInfo().IsAssignableFrom(instance.GetType().GetTypeInfo()) ? instance.GetType() : throw new ArgumentException("Object is not an instance of the specified type.");
      VssConnection.ClientCacheKey key = new VssConnection.ClientCacheKey(type1, this.GetServiceIdentifier(type));
      this.RegisterExtensibleType(type.Name, type1);
      this.m_cachedTypes[key] = instance;
    }

    public void Dispose()
    {
      if (this.m_isDisposed)
        return;
      lock (this.m_disposeLock)
      {
        if (this.m_isDisposed)
          return;
        this.m_isDisposed = true;
        foreach (IDisposable disposable in this.m_cachedTypes.Values.Where<object>((Func<object, bool>) (v => v is IDisposable)).Select<object, IDisposable>((Func<object, IDisposable>) (v => v as IDisposable)))
          disposable.Dispose();
        this.m_cachedTypes.Clear();
        this.Disconnect();
        if (this.m_parentConnection == null)
          return;
        this.m_parentConnection.Dispose();
        this.m_parentConnection = (VssConnection) null;
      }
    }

    private void CheckForDisposed()
    {
      if (this.m_isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    public bool IsDisposed() => this.m_isDisposed;

    public Uri Uri => this.m_baseUrl;

    public VssHttpMessageHandler InnerHandler => this.m_innerHandler;

    public IEnumerable<DelegatingHandler> DelegatingHandlers => this.m_delegatingHandlers;

    public VssCredentials Credentials => this.m_innerHandler.Credentials;

    public VssClientHttpRequestSettings Settings => (VssClientHttpRequestSettings) this.m_innerHandler.Settings;

    public Guid ServerId => this.ServerDataProvider.GetInstanceIdAsync().SyncResult<Guid>();

    public Guid ServerType => this.ServerDataProvider.GetInstanceTypeAsync().SyncResult<Guid>();

    public Microsoft.VisualStudio.Services.Identity.Identity AuthorizedIdentity => this.ServerDataProvider.GetAuthorizedIdentityAsync().SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Microsoft.VisualStudio.Services.Identity.Identity AuthenticatedIdentity => this.ServerDataProvider.GetAuthenticatedIdentityAsync().SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>();

    public bool HasAuthenticated => this.ServerDataProvider.HasConnected;

    public IVssConnection ParentConnection
    {
      get
      {
        this.CheckForDisposed();
        if (this.m_parentConnection == null)
        {
          lock (this.m_parentConnectionLock)
          {
            string uriString = this.GetService<ILocationService>().GetLocationData(Guid.Empty).LocationForCurrentConnection("LocationService2", LocationServiceConstants.ApplicationIdentifier);
            if (string.IsNullOrEmpty(uriString))
              throw new VssServiceException(WebApiResources.ServerDataProviderNotFound((object) LocationServiceConstants.ApplicationIdentifier));
            this.m_parentConnection = new VssConnection(new Uri(uriString), new VssHttpMessageHandler(this.Credentials, (VssHttpRequestSettings) VssClientHttpRequestSettings.Default.Clone()), (IEnumerable<DelegatingHandler>) null, false);
          }
        }
        return (IVssConnection) this.m_parentConnection;
      }
    }

    internal IVssServerDataProvider ServerDataProvider
    {
      get => this.m_serverDataProvider;
      set => this.m_serverDataProvider = value;
    }

    private struct ClientCacheKey
    {
      public readonly Type Type;
      public readonly Guid ServiceIdentifier;
      public static readonly IEqualityComparer<VssConnection.ClientCacheKey> Comparer = (IEqualityComparer<VssConnection.ClientCacheKey>) new VssConnection.ClientCacheKey.ClientCacheKeyComparer();

      public ClientCacheKey(Type type, Guid serviceIdentifier)
      {
        this.Type = type;
        this.ServiceIdentifier = serviceIdentifier;
      }

      private class ClientCacheKeyComparer : IEqualityComparer<VssConnection.ClientCacheKey>
      {
        public bool Equals(VssConnection.ClientCacheKey x, VssConnection.ClientCacheKey y) => x.Type.Equals(y.Type) && x.ServiceIdentifier.Equals(y.ServiceIdentifier);

        public int GetHashCode(VssConnection.ClientCacheKey obj) => obj.Type.GetHashCode() ^ obj.ServiceIdentifier.GetHashCode();
      }
    }
  }
}
