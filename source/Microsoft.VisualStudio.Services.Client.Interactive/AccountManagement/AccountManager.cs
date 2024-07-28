// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountManager
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Client.AccountManagement.Logging;
using Microsoft.VisualStudio.Services.Client.Controls;
using Microsoft.VisualStudio.Services.Client.Keychain.VSProvider;
using Microsoft.VisualStudio.Services.Common.TokenStorage;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public class AccountManager : IAccountManager
  {
    private static Lazy<IAccountManager> accountManagerInstance = new Lazy<IAccountManager>((Func<IAccountManager>) (() => (IAccountManager) new AccountManager((IAccountStore) null)));
    private VssTokenStorage VssTokenStorageInstance;
    private IAccountCache AccountCacheInstance;
    private VSAccountProvider vsAccountProvider;
    private readonly IVSAccountProviderShim vsAccountProviderShim;
    private static object syncLock = new object();
    private string instanceName;
    private static Uri ResolvedVsoEndpoint;
    internal static Uri DefaultVsoEndpoint = new Uri("https://go.microsoft.com/fwlink/?LinkID=415837");
    private static AggregateLogger logger = new AggregateLogger();

    internal static ILogger Logger => (ILogger) AccountManager.logger;

    public static void AddLogger(ILogger logger) => AccountManager.logger.Add(logger);

    public static void RemoveLogger(ILogger logger) => AccountManager.logger.Remove(logger);

    public AccountManager(IAccountStore store, string instanceName = null)
    {
      this.Store = store ?? (IAccountStore) new RegistryAccountStore(instanceName);
      this.instanceName = instanceName;
      VssFederatedCredentialPrompt.InteractiveAuthentication += new Action<InteractiveAuthenticationEvent>(this.SignInTelemetryEventHandler);
    }

    internal AccountManager(
      IAccountStore store,
      string instanceName,
      IVSAccountProviderShim vsAccountProviderShim)
    {
      this.Store = store ?? (IAccountStore) new RegistryAccountStore(instanceName);
      this.instanceName = instanceName;
      this.vsAccountProviderShim = vsAccountProviderShim;
      VssFederatedCredentialPrompt.InteractiveAuthentication += new Action<InteractiveAuthenticationEvent>(this.SignInTelemetryEventHandler);
    }

    public static IAccountManager DefaultInstance => AccountManager.accountManagerInstance.Value;

    public IAccountStore Store { get; private set; }

    public static Uri VsoEndpoint
    {
      get
      {
        if (AccountManager.GetVSOEndpointCallback != null)
          AccountManager.ResolvedVsoEndpoint = AccountManager.GetVSOEndpointCallback();
        else if (AccountManager.ResolvedVsoEndpoint == (Uri) null)
          AccountManager.ResolvedVsoEndpoint = AccountManager.GetVsoEndpoint();
        return AccountManager.ResolvedVsoEndpoint;
      }
      set => AccountManager.ResolvedVsoEndpoint = value;
    }

    public static Func<Uri> GetVSOEndpointCallback { get; set; }

    public T GetCache<T>() where T : class
    {
      lock (AccountManager.syncLock)
      {
        Type t = typeof (T);
        return (this.TryGetVssTokenStorage(t) ?? this.TryGetAccountManagerCache(t)) as T;
      }
    }

    private object TryGetAccountManagerCache(Type t)
    {
      if (!t.IsSubclassOf(typeof (IAccountCache)) && t != typeof (IAccountCache))
        return (object) null;
      this.SetApplicationCore();
      return (object) this.AccountCacheInstance;
    }

    private void SetApplicationCore()
    {
      if (this.AccountCacheInstance != null)
        return;
      AccountCache accountCache = new AccountCache((IAccountCacheConfiguration) new AccountCacheConfiguration(), (IAadProviderConfiguration) new AadProviderConfiguration(), VssAadSettings.ApplicationTenant);
      lock (AccountManager.syncLock)
      {
        if (this.AccountCacheInstance != null)
          return;
        this.SetAccountManagerAccountCacheInstance((IAccountCache) accountCache);
      }
    }

    private object TryGetVssTokenStorage(Type t)
    {
      if (!t.IsSubclassOf(typeof (VssTokenStorage)) && t != typeof (VssTokenStorage))
        return (object) null;
      if (this.VssTokenStorageInstance == null)
      {
        lock (AccountManager.syncLock)
        {
          if (this.VssTokenStorageInstance == null)
            this.SetVssTokenStorageInstance(VssTokenStorageFactory.GetTokenStorageNamespace("VisualStudio"));
        }
      }
      return (object) this.VssTokenStorageInstance;
    }

    public IAccountProvider GetAccountProvider(Guid accountProviderId)
    {
      if (!(accountProviderId == VSAccountProvider.AccountProviderIdentifier))
        return (IAccountProvider) null;
      if (this.vsAccountProvider == null)
      {
        VSAccountProvider vsAccountProvider = new VSAccountProvider(this.instanceName, this.vsAccountProviderShim);
        vsAccountProvider.Initialize(this.Store);
        this.SetApplicationCore();
        lock (AccountManager.syncLock)
        {
          if (this.vsAccountProvider == null)
          {
            this.vsAccountProvider = vsAccountProvider;
            this.vsAccountProvider.SetAccountCache(this.AccountCacheInstance);
          }
        }
      }
      return (IAccountProvider) this.vsAccountProvider;
    }

    public static void SetDefaultInstanceWithName(
      string instanceName,
      IAccountStore store = null,
      IVSAccountProviderShim vsAccountProviderShim = null)
    {
      AccountManager.SetDefaultInstance((IAccountManager) new AccountManager(store, instanceName, vsAccountProviderShim));
    }

    internal static void SetDefaultInstance(IAccountManager manager) => AccountManager.accountManagerInstance = new Lazy<IAccountManager>((Func<IAccountManager>) (() => manager));

    internal void SetVssTokenStorageInstance(VssTokenStorage store) => this.VssTokenStorageInstance = store;

    internal void SetAccountManagerAccountCacheInstance(IAccountCache accountCache) => this.AccountCacheInstance = accountCache;

    internal static Uri GetVsoEndpoint()
    {
      Uri uri = (Uri) null;
      try
      {
        RegistryKey sharedRegistryRoot = VssClientEnvironment.TryGetUserSharedRegistryRoot();
        if (sharedRegistryRoot != null)
        {
          using (sharedRegistryRoot)
          {
            using (RegistryKey registryKey = sharedRegistryRoot.OpenSubKey("ConnectedUser"))
            {
              if (registryKey != null)
              {
                if (registryKey.GetValue("uri", (object) null) is string uriName)
                  uri = AccountManagementUtilities.CheckUri(uriName);
              }
            }
          }
        }
      }
      catch
      {
      }
      Uri vsoEndPoint = uri;
      if ((object) vsoEndPoint == null)
        vsoEndPoint = AccountManager.DefaultVsoEndpoint;
      return AccountManagementUtilities.ResolveFWLinkIfRequired(vsoEndPoint);
    }

    internal void SignInTelemetryEventHandler(InteractiveAuthenticationEvent e) => AccountManager.logger.LogEvent("Prompt", (IDictionary<string, object>) new Dictionary<string, object>()
    {
      {
        "AuthType",
        e.IsReauthenticating ? (object) "Reauth" : (object) "Create"
      },
      {
        "Source",
        e.ConnectMode == VssConnectMode.Resource ? (object) "TFS" : (object) "non-TFS"
      }
    });
  }
}
