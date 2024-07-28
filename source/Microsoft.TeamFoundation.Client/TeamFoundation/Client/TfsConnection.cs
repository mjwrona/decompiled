// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsConnection
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.Internal.Performance;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace Microsoft.TeamFoundation.Client
{
  public abstract class TfsConnection : IServiceProvider, IDisposable
  {
    private string m_displayName;
    private Uri m_uri;
    private string m_fullyQualifiedUrl;
    private TimeZone m_timeZone;
    private CultureInfo m_culture;
    private CultureInfo m_uiCulture;
    private IdentityDescriptor m_identityToImpersonate;
    private Guid m_sessionId = Guid.NewGuid();
    private VssCredentials m_clientCredentials;
    private string m_locationSerivceRelativePath;
    private object m_lockToken = new object();
    private object m_lock = new object();
    private bool m_isOffline;
    private VssConnection m_vssConnection;
    private FrameworkServerDataProvider m_serverDataProvider;
    private TFProxyServer m_proxyServer;
    private static string s_overriddenSettingsDirectory;
    private static string s_applicationName;
    private ThreadSafeServiceContainer m_serviceContainer = new ThreadSafeServiceContainer();
    private Dictionary<Type, object> m_loadingServices = new Dictionary<Type, object>();
    private static long s_requestId;
    [ThreadStatic]
    private static string t_operationName;
    private static readonly string s_unauthorizedAccessExceptionErrorCode = "TF50309";
    private const string c_cacheSettingsKey = "Services\\CacheSettings";
    private const string c_settingClientCacheTimeToLive = "ClientCacheTimeToLive";
    private static int? s_clientCacheTimeToLive;
    private static bool s_checkedClientCacheTimeToLive;

    internal TfsConnection(Uri uri, string locationServiceRelativePath)
      : this(uri, locationServiceRelativePath, (IdentityDescriptor) null, (ITfsRequestChannelFactory) null)
    {
    }

    internal TfsConnection(
      Uri uri,
      string locationServiceRelativePath,
      IdentityDescriptor identityToImpersonate,
      ITfsRequestChannelFactory channelFactory)
      : this(uri, TfsConnection.LoadFromCache(uri), identityToImpersonate, locationServiceRelativePath, channelFactory)
    {
    }

    [Obsolete("This constructor is obsolete and will be removed in a future version. See TfsTeamProjectCollection(Uri uri, VssCredentials credentials) instead", false)]
    internal TfsConnection(Uri uri, ICredentials credentials, string locationServiceRelativePath)
      : this(uri, TfsConnection.LoadFromCache(uri, credentials), (IdentityDescriptor) null, locationServiceRelativePath, (ITfsRequestChannelFactory) null)
    {
    }

    internal TfsConnection(
      Uri uri,
      VssCredentials credentials,
      IdentityDescriptor identityToImpersonate,
      string locationServiceRelativePath,
      ITfsRequestChannelFactory channelFactory)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      ArgumentUtility.CheckForNull<VssCredentials>(credentials, nameof (credentials));
      ArgumentUtility.CheckStringForNullOrEmpty(locationServiceRelativePath, nameof (locationServiceRelativePath));
      this.m_uri = uri;
      if (VssStringComparer.ServerUrl.EndsWith(this.m_uri.AbsolutePath, locationServiceRelativePath))
        this.m_uri = new Uri(this.m_uri.AbsoluteUri.Substring(0, this.m_uri.AbsoluteUri.Length - locationServiceRelativePath.Length));
      this.m_timeZone = TimeZone.CurrentTimeZone;
      this.m_culture = CultureInfo.CurrentCulture;
      this.m_uiCulture = CultureInfo.CurrentUICulture;
      this.m_identityToImpersonate = identityToImpersonate;
      this.m_locationSerivceRelativePath = locationServiceRelativePath;
      this.m_fullyQualifiedUrl = TFCommonUtil.CombinePaths(this.m_uri.AbsoluteUri, locationServiceRelativePath);
      this.ChannelFactory = channelFactory;
      this.SetClientCredentials(credentials);
    }

    [Obsolete("This constructor is obsolete and will be removed in a future version. See TfsTeamProjectCollection(Uri uri, VssCredentials credentials) instead", false)]
    internal TfsConnection(
      Uri uri,
      ICredentials credentials,
      ICredentialsProvider credentialsProvider,
      IdentityDescriptor identityToImpersonate,
      string locationServiceRelativePath,
      ITfsRequestChannelFactory channelFactory)
      : this(uri, TfsConnection.LoadFromCache(uri, credentials), identityToImpersonate, locationServiceRelativePath, channelFactory)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ITfsRequestChannelFactory ChannelFactory { get; private set; }

    public VssCredentials ClientCredentials
    {
      get => this.m_clientCredentials;
      [EditorBrowsable(EditorBrowsableState.Never)] set
      {
        ArgumentUtility.CheckForNull<VssCredentials>(value, nameof (value));
        if (this.HasAuthenticated)
          throw new InvalidOperationException();
        this.SetClientCredentials(value);
      }
    }

    private void SetClientCredentials(VssCredentials credentials)
    {
      this.m_clientCredentials = credentials;
      if (this.m_clientCredentials?.Federated != null)
        this.m_clientCredentials.Federated.TokenStorageUrl = this.m_uri;
      if (this.m_clientCredentials?.Windows == null)
        return;
      this.m_clientCredentials.Windows.TokenStorageUrl = this.m_uri;
    }

    public virtual Guid InstanceId => this.ServerDataProvider.InstanceId;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Guid CachedInstanceId => this.ServerDataProvider.CachedInstanceId;

    public Uri Uri => this.m_uri;

    public TimeZone TimeZone
    {
      get => this.m_timeZone;
      set => this.m_timeZone = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public ServerCapabilities ServerCapabilities => this.ServerDataProvider.ServerCapabilities;

    public virtual bool IsHostedServer => (this.ServerCapabilities & ServerCapabilities.Hosted) == ServerCapabilities.Hosted;

    public abstract CatalogNode CatalogNode { get; }

    public virtual string Name
    {
      get
      {
        if (this.m_displayName == null)
          this.m_displayName = this.m_uri.ToString();
        return this.m_displayName;
      }
    }

    public static string ApplicationName
    {
      get
      {
        if (string.IsNullOrEmpty(TfsConnection.s_applicationName))
          TfsConnection.s_applicationName = TfsRequestSettings.Default.UserAgent;
        return TfsConnection.s_applicationName;
      }
      set => TfsConnection.s_applicationName = value;
    }

    public static string OperationName
    {
      get => TfsConnection.t_operationName == null ? string.Empty : TfsConnection.t_operationName;
      set => TfsConnection.t_operationName = value;
    }

    public static event TfsConnectionWebServiceCallEventHandler WebServiceCallBegin;

    public static event TfsConnectionWebServiceCallEventHandler WebServiceCallEnd;

    public static long TotalRequestCount => Interlocked.Read(ref TfsConnection.s_requestId);

    private static VssCredentials LoadFromCache(Uri serverUrl, ICredentials credentials = null)
    {
      CredentialPromptType promptType = Environment.UserInteractive ? CredentialPromptType.PromptIfNeeded : CredentialPromptType.DoNotPrompt;
      VssCredentials vssCredentials = VssClientCredentials.LoadCachedCredentials(serverUrl, false, promptType);
      if (credentials != null)
        vssCredentials.Windows.Credentials = credentials;
      return vssCredentials;
    }

    internal static long OnBeginWebRequest() => Interlocked.Increment(ref TfsConnection.s_requestId);

    internal static void OnWebServiceCallBegin(
      TfsConnection originatingTfsConnection,
      long requestId,
      string methodName)
    {
      TfsConnection.OnWebServiceCallBegin(originatingTfsConnection, requestId, methodName, string.Empty, (HttpWebRequest) null);
    }

    internal static void OnWebServiceCallBegin(
      TfsConnection originatingTfsConnection,
      long requestId,
      string methodName,
      string componentName,
      HttpWebRequest request)
    {
      TfsConnectionWebServiceCallEventHandler serviceCallBegin = TfsConnection.WebServiceCallBegin;
      if (serviceCallBegin != null)
      {
        TfsConnectionWebServiceCallEventArgs e = new TfsConnectionWebServiceCallEventArgs(originatingTfsConnection, requestId, methodName, componentName, request != null ? request.ContentLength : 0L);
        serviceCallBegin((object) null, e);
      }
      CodeMarkers.Instance.CodeMarkerEx(9600, (ulong) requestId);
    }

    internal static void OnWebServiceCallEnd(
      TfsConnection originatingTfsConnection,
      long requestId,
      string methodName)
    {
      TfsConnection.OnWebServiceCallEnd(originatingTfsConnection, requestId, methodName, string.Empty, (HttpWebResponse) null);
    }

    internal static void OnWebServiceCallEnd(
      TfsConnection originatingTfsConnection,
      long requestId,
      string methodName,
      string componentName,
      HttpWebResponse response)
    {
      CodeMarkers.Instance.CodeMarkerEx(9601, (ulong) requestId);
      TfsConnectionWebServiceCallEventHandler webServiceCallEnd = TfsConnection.WebServiceCallEnd;
      if (webServiceCallEnd == null)
        return;
      TfsConnectionWebServiceCallEventArgs e = new TfsConnectionWebServiceCallEventArgs(originatingTfsConnection, requestId, methodName, componentName, response != null ? response.ContentLength : 0L);
      webServiceCallEnd((object) null, e);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IdentityDescriptor IdentityToImpersonate => this.m_identityToImpersonate;

    public ICredentials Credentials
    {
      get => this.ClientCredentials.Windows.Credentials;
      [EditorBrowsable(EditorBrowsableState.Never)] set
      {
        ArgumentUtility.CheckForNull<ICredentials>(value, nameof (value));
        this.ClientCredentials.Windows.Credentials = value;
      }
    }

    [Obsolete("This property is obsolete and will be removed in a future release. See VssCredentialPrompts instead.", false)]
    internal ICredentialsProvider CredentialsProvider
    {
      get => (ICredentialsProvider) null;
      set
      {
      }
    }

    public CultureInfo Culture
    {
      get => this.m_culture;
      set => this.m_culture = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public CultureInfo UICulture
    {
      get => this.m_uiCulture;
      set => this.m_uiCulture = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public Guid SessionId => this.m_sessionId;

    public static string ClientCacheDirectory => Path.Combine(TfsConnection.ClientSettingsDirectory, "Cache");

    public static string ClientVolatileCacheDirectory => Path.Combine(TfsConnection.ClientCacheDirectory, "Volatile");

    public static string ClientConfigurationDirectory => Path.Combine(TfsConnection.ClientSettingsDirectory, "Configuration");

    private static string SafeGetFolderPath(Environment.SpecialFolder specialFolder)
    {
      try
      {
        return Environment.GetFolderPath(specialFolder);
      }
      catch (ArgumentException ex)
      {
        return (string) null;
      }
    }

    public static string ClientSettingsDirectory
    {
      get
      {
        if (!string.IsNullOrEmpty(TfsConnection.s_overriddenSettingsDirectory))
          return TfsConnection.s_overriddenSettingsDirectory;
        string path2 = "Microsoft\\Azure DevOps";
        string folderPath = TfsConnection.SafeGetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrEmpty(folderPath))
        {
          folderPath = TfsConnection.SafeGetFolderPath(Environment.SpecialFolder.ApplicationData);
          if (string.IsNullOrEmpty(folderPath))
          {
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            path2 = "DevOps";
          }
        }
        return Path.Combine(Path.Combine(folderPath, path2), "11.0");
      }
      set => TfsConnection.s_overriddenSettingsDirectory = value;
    }

    public string ClientCacheDirectoryForInstance => this.ServerDataProvider.ClientCacheDirectoryForInstance;

    public string ClientVolatileCacheDirectoryForInstance => this.ServerDataProvider.ClientVolatileCacheDirectoryForInstance;

    public string ClientCacheDirectoryForUser => this.ServerDataProvider.ClientCacheDirectoryForUser;

    internal static int? ClientCacheTimeToLive
    {
      get
      {
        if (!TfsConnection.s_clientCacheTimeToLive.HasValue && !TfsConnection.s_checkedClientCacheTimeToLive)
        {
          TfsConnection.s_checkedClientCacheTimeToLive = true;
          RegistryKey registryKey = (RegistryKey) null;
          using (RegistryKey userRegistryRoot = TeamFoundationEnvironment.TryGetUserRegistryRoot())
          {
            if (userRegistryRoot != null)
              registryKey = userRegistryRoot.OpenSubKey("Services\\CacheSettings");
          }
          if (registryKey == null)
          {
            using (RegistryKey applicationRegistryRoot = TeamFoundationEnvironment.TryGetApplicationRegistryRoot())
            {
              if (applicationRegistryRoot != null)
                registryKey = applicationRegistryRoot.OpenSubKey("Services\\CacheSettings");
            }
          }
          if (registryKey != null && registryKey.GetValue(nameof (ClientCacheTimeToLive)) != null && registryKey.GetValueKind(nameof (ClientCacheTimeToLive)) == RegistryValueKind.DWord)
            TfsConnection.s_clientCacheTimeToLive = new int?(Math.Max(1, (int) registryKey.GetValue(nameof (ClientCacheTimeToLive))));
        }
        return TfsConnection.s_clientCacheTimeToLive;
      }
      set => TfsConnection.s_clientCacheTimeToLive = value;
    }

    public void GetAuthenticatedIdentity(out TeamFoundationIdentity identity) => identity = this.ServerDataProvider.AuthenticatedIdentity;

    public TeamFoundationIdentity AuthorizedIdentity => this.ServerDataProvider.AuthorizedIdentity;

    public bool HasAuthenticated => this.m_serverDataProvider != null && this.ServerDataProvider.HasAuthenticated;

    public override string ToString() => this.Name;

    public void EnsureAuthenticated()
    {
      if (this.HasAuthenticated)
        return;
      this.ServerDataProvider.EnsureAuthenticated();
    }

    public void Authenticate() => this.ServerDataProvider.Authenticate();

    public void Connect(Microsoft.TeamFoundation.Framework.Common.ConnectOptions connectOptions) => this.ServerDataProvider.Connect(connectOptions);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Disconnect()
    {
      try
      {
        if (this.HasAuthenticated && this.IsHostedServer)
        {
          string uriString = this.ServerDataProvider.LocationForCurrentConnection("SignOutWeb", FrameworkServiceIdentifiers.SignOutWeb);
          if (!string.IsNullOrEmpty(uriString))
          {
            string attribute = this.ServerDataProvider.AuthenticatedIdentity?.GetAttribute("Domain", (string) null);
            this.ClientCredentials.SignOut(this.Uri, new Uri(uriString, UriKind.Absolute), attribute);
          }
        }
        if (this.m_serviceContainer == null)
          return;
        this.m_serviceContainer.FlushServices();
      }
      finally
      {
        this.ServerDataProvider.Disconnect();
      }
    }

    public virtual T GetService<T>() => (T) this.GetService(typeof (T));

    public virtual object GetService(Type serviceType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      object serviceInstance = this.m_serviceContainer != null ? this.m_serviceContainer.GetService(serviceType) : throw new ObjectDisposedException(this.GetType().FullName);
      if (serviceInstance == null)
      {
        object obj = (object) null;
        lock (this.m_loadingServices)
        {
          if (!this.m_loadingServices.TryGetValue(serviceType, out obj))
          {
            obj = new object();
            this.m_loadingServices[serviceType] = obj;
          }
        }
        lock (obj)
        {
          serviceInstance = this.m_serviceContainer.GetService(serviceType);
          if (serviceInstance == null)
          {
            serviceInstance = this.GetServiceInstance(serviceType, serviceInstance);
            if (serviceInstance != null)
            {
              try
              {
                this.m_serviceContainer.AddService(serviceType, serviceInstance);
                lock (this.m_loadingServices)
                  this.m_loadingServices.Remove(serviceType);
              }
              catch (ArgumentException ex)
              {
                if (serviceInstance is IDisposable disposable)
                  disposable.Dispose();
                serviceInstance = this.m_serviceContainer.GetService(serviceType);
              }
            }
          }
        }
      }
      return serviceInstance;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void FlushServices()
    {
      if (this.m_serviceContainer == null)
        throw new ObjectDisposedException(this.GetType().FullName);
      this.m_serviceContainer.FlushServices();
    }

    public virtual T GetClient<T>() where T : IVssHttpClient => this.ValidateAndPrepareGetClient<T>().GetClient<T>();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public T GetClient<T>(VssHttpRequestSettings settings) where T : IVssHttpClient => this.ValidateAndPrepareGetClient<T>(settings).GetClient<T>();

    private VssConnection ValidateAndPrepareGetClient<T>(VssHttpRequestSettings requestSettings = null)
    {
      Type type = typeof (IVssHttpClient);
      if (!type.IsAssignableFrom(typeof (T)))
        throw new ArgumentException(type.FullName);
      this.EnsureAuthenticated();
      VssConnection client = this.m_vssConnection;
      if (requestSettings != null)
        client = this.CreateVssConnection(requestSettings);
      else if (client == null)
        client = this.m_vssConnection = this.CreateVssConnection((VssHttpRequestSettings) null);
      return client;
    }

    protected virtual VssConnection CreateVssConnection() => this.CreateVssConnection((VssHttpRequestSettings) null);

    private VssConnection CreateVssConnection(VssHttpRequestSettings requestSettings)
    {
      if (requestSettings == null)
        settings = VssClientHttpRequestSettings.Default.Clone();
      else if (!(requestSettings is VssClientHttpRequestSettings settings))
      {
        settings = VssClientHttpRequestSettings.Default.Clone();
        settings.CompressionEnabled = requestSettings.CompressionEnabled;
        settings.ExpectContinue = requestSettings.ExpectContinue;
        settings.BypassProxyOnLocal = requestSettings.BypassProxyOnLocal;
        settings.SendTimeout = requestSettings.SendTimeout;
        settings.SuppressFedAuthRedirects = requestSettings.SuppressFedAuthRedirects;
        settings.UserAgent = requestSettings.UserAgent;
        settings.SessionId = requestSettings.SessionId;
        settings.AgentId = requestSettings.AgentId;
        settings.ClientCertificateManager = requestSettings.ClientCertificateManager;
        settings.AcceptLanguages.Clear();
        foreach (CultureInfo acceptLanguage in (IEnumerable<CultureInfo>) requestSettings.AcceptLanguages)
          settings.AcceptLanguages.Add(acceptLanguage);
      }
      Uri baseUrl = this.Uri;
      if (!this.ServerCapabilities.HasFlag((System.Enum) ServerCapabilities.Hosted) && this is TfsTeamProjectCollection)
      {
        IServerDataProvider serverDataProvider = this.ServerDataProvider;
        AccessMapping clientAccessMapping = serverDataProvider.ClientAccessMapping;
        if (clientAccessMapping != null && string.IsNullOrEmpty(clientAccessMapping.VirtualDirectory))
        {
          AccessMapping defaultAccessMapping = serverDataProvider.DefaultAccessMapping;
          baseUrl = new Uri(LocationHelper.GetRootServerUrl(serverDataProvider.LocationForAccessMapping("LocationService", LocationServiceConstants.SelfReferenceLocationServiceIdentifier, defaultAccessMapping ?? clientAccessMapping)));
        }
      }
      return new VssConnection(baseUrl, this.ClientCredentials, (VssHttpRequestSettings) settings);
    }

    public event CredentialsChangedEventHandler CredentialsChanged;

    internal void FireCredentialsChanged(ICredentials currentCredentials)
    {
      CredentialsChangedEventHandler credentialsChanged = this.CredentialsChanged;
      if (credentialsChanged == null)
        return;
      credentialsChanged((object) this, new CredentialsChangedEventArgs(currentCredentials));
    }

    internal string GetLocationServiceRelativePath() => this.m_locationSerivceRelativePath;

    protected static Uri GetFullyQualifiedUriForName(
      string name,
      string locationServiceRelativePath,
      Func<string, Uri> getRegisteredUri)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      string uriString = name.Replace("\\", "/");
      if (name.EndsWith(locationServiceRelativePath, StringComparison.OrdinalIgnoreCase))
        return new Uri(name);
      Uri qualifiedUriForName = (Uri) null;
      if (uriString.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || uriString.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
      {
        qualifiedUriForName = new Uri(uriString);
      }
      else
      {
        Uri uri = getRegisteredUri(name);
        if (uri != (Uri) null)
          qualifiedUriForName = uri;
      }
      if (qualifiedUriForName == (Uri) null)
        throw new TeamFoundationInvalidServerNameException(Microsoft.TeamFoundation.Client.Internal.ClientResources.ServerNameNotValid((object) name));
      if (!VssStringComparer.ServerUrl.EndsWith(qualifiedUriForName.AbsoluteUri, locationServiceRelativePath))
        qualifiedUriForName = new Uri(TFCommonUtil.CombinePaths(qualifiedUriForName.AbsoluteUri, locationServiceRelativePath));
      return qualifiedUriForName;
    }

    protected virtual object GetServiceInstance(Type serviceType, object serviceInstance)
    {
      if (serviceType == typeof (ITeamFoundationRegistry) || serviceType == typeof (IIdentityManagementService) || serviceType == typeof (IIdentityManagementService2) || serviceType == typeof (ITeamFoundationJobService) || serviceType == typeof (IPropertyService) || serviceType == typeof (IEventService) || serviceType == typeof (ISecurityService) || serviceType == typeof (ILocationService) || serviceType == typeof (ILocationServiceInternal) || serviceType == typeof (IAccessControlService))
        serviceInstance = this.CreateInternalProxy(serviceType);
      else if (!serviceType.IsInterface)
        return this.CreateServiceInstance(serviceType.Assembly, serviceType.FullName);
      return serviceInstance;
    }

    protected object CreateServiceInstance(Assembly assembly, string fullName)
    {
      object instance = (object) null;
      object serviceInstance = (object) null;
      try
      {
        instance = assembly.CreateInstance(fullName, false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder) null, (object[]) null, (CultureInfo) null, (object[]) null);
      }
      catch (Exception ex)
      {
      }
      if (instance != null)
        serviceInstance = this.InitializeTeamFoundationObject(fullName, instance);
      return serviceInstance;
    }

    protected virtual object InitializeTeamFoundationObject(string fullName, object instance)
    {
      if (instance is ITfsConnectionObject connectionObject)
        connectionObject.Initialize(this);
      return (object) connectionObject;
    }

    protected virtual object CreateInternalProxy(Type serviceType)
    {
      if (serviceType == typeof (ITeamFoundationRegistry))
        return (object) new TeamFoundationRegistry(this);
      if (serviceType == typeof (IIdentityManagementService))
        return (object) new IdentityManagementService(this);
      if (serviceType == typeof (IIdentityManagementService2))
        return (object) new IdentityManagementService2(this);
      if (serviceType == typeof (ITeamFoundationJobService))
        return (object) new TeamFoundationJobService(this);
      if (serviceType == typeof (IPropertyService))
        return (object) new TeamFoundationPropertyService(this);
      if (serviceType == typeof (IEventService))
        return (object) new TeamFoundationEventService(this);
      if (serviceType == typeof (ISecurityService))
        return (object) new SecurityService(this);
      if (serviceType == typeof (ILocationService) || serviceType == typeof (ILocationServiceInternal))
        return (object) this.ServerDataProvider;
      return serviceType == typeof (IAccessControlService) ? (object) new AccessControlService(this) : (object) null;
    }

    internal void RefreshProxySettings() => this.m_proxyServer = TFProxyServerFactory.GetProxyServer();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TFProxyServer ProxyServer
    {
      get
      {
        if (this.m_proxyServer == null)
          this.RefreshProxySettings();
        return this.m_proxyServer;
      }
    }

    public bool ConnectivityFailureOnLastWebServiceCall
    {
      get => this.m_isOffline;
      internal set
      {
        if (this.m_isOffline == value)
          return;
        bool flag = false;
        lock (this.m_lock)
        {
          if (this.m_isOffline != value)
          {
            this.m_isOffline = value;
            flag = true;
          }
        }
        if (!flag)
          return;
        ThreadPool.QueueUserWorkItem((WaitCallback) (state =>
        {
          try
          {
            this.OnConnectivityFailureStatusChanged(value);
          }
          catch
          {
          }
        }));
      }
    }

    public event ConnectivityFailureStatusChangedEventHandler ConnectivityFailureStatusChanged;

    protected virtual void OnConnectivityFailureStatusChanged(bool newConnectivityFailureStatus)
    {
      ConnectivityFailureStatusChangedEventHandler failureStatusChanged = this.ConnectivityFailureStatusChanged;
      if (failureStatusChanged == null)
        return;
      failureStatusChanged((object) this, new ConnectivityFailureStatusChangedEventArgs(newConnectivityFailureStatus));
    }

    public override bool Equals(object obj) => obj is TfsConnection tfsConnection && VssStringComparer.ServerUrl.Equals(this.m_fullyQualifiedUrl, tfsConnection.m_fullyQualifiedUrl);

    public override int GetHashCode() => this.m_fullyQualifiedUrl.GetHashCode();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Disposed => this.m_serviceContainer == null;

    public virtual void Dispose()
    {
      GC.SuppressFinalize((object) this);
      if (this.m_serviceContainer == null)
        return;
      this.m_serviceContainer.Dispose();
      this.m_serviceContainer = (ThreadSafeServiceContainer) null;
    }

    protected Guid CatalogResourceId => this.ServerDataProvider.CatalogResourceId;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IServerDataProvider ServerDataProvider
    {
      get
      {
        this.EnsureProviderConnected();
        return (IServerDataProvider) this.m_serverDataProvider;
      }
    }

    internal void ClearInMemoryCache()
    {
      this.m_serverDataProvider = (FrameworkServerDataProvider) null;
      this.m_serviceContainer.RemoveService(typeof (ILocationService));
    }

    internal void ReactToPossibleServerUpdate(int locationServiceLastChangeId) => this.m_serverDataProvider.ReactToPossibleServerUpdate(locationServiceLastChangeId);

    private void EnsureProviderConnected()
    {
      if (this.m_serverDataProvider != null)
        return;
      try
      {
        lock (this.m_lockToken)
        {
          if (this.m_serverDataProvider != null)
            return;
          this.m_serverDataProvider = new FrameworkServerDataProvider(this, this.m_fullyQualifiedUrl);
          if (this.m_serverDataProvider.LocalCacheAvailable)
            return;
          this.m_serverDataProvider.Connect(Microsoft.TeamFoundation.Framework.Common.ConnectOptions.IncludeServices);
        }
      }
      catch (Exception ex)
      {
        this.m_serverDataProvider = (FrameworkServerDataProvider) null;
        if (ex.InnerException is WebException innerException && innerException.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.NotFound)
          throw new TeamFoundationServiceUnavailableException(Microsoft.TeamFoundation.Client.Internal.ClientResources.ConnectToTfs_AddServer_UnableToConnect_WithTechnicalInfo((object) this.Uri.ToString(), (object) this.Uri.ToString(), (object) innerException.Message), (Exception) innerException);
        throw;
      }
    }

    private static void ThrowAuthorizationException(Exception e)
    {
      switch (e)
      {
        case TeamFoundationServerUnauthorizedException _:
        case AccessCheckException _:
          throw e;
        default:
          throw new TeamFoundationServerUnauthorizedException(TFCommonResources.UnauthorizedUnknownServer(), e);
      }
    }
  }
}
