// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssCredentials
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Diagnostics;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  public class VssCredentials
  {
    private static ConcurrentDictionary<string, ICachedVssCredentialProvider> m_loadedCachedVssCredentialProviders = new ConcurrentDictionary<string, ICachedVssCredentialProvider>();
    private object m_thisLock;
    private CredentialPromptType m_promptType;
    private IssuedTokenProvider m_currentProvider;
    protected WindowsCredential m_windowsCredential;
    protected FederatedCredential m_federatedCredential;
    private IVssCredentialStorage m_credentialStorage;

    public VssCredentials()
      : this(true)
    {
    }

    public VssCredentials(bool useDefaultCredentials)
      : this(new WindowsCredential(useDefaultCredentials))
    {
    }

    public VssCredentials(WindowsCredential windowsCredential)
      : this(windowsCredential, (FederatedCredential) null)
    {
    }

    public VssCredentials(WindowsCredential windowsCredential, CredentialPromptType promptType)
      : this(windowsCredential, (FederatedCredential) null, promptType)
    {
    }

    public VssCredentials(FederatedCredential federatedCredential)
      : this(new WindowsCredential(), federatedCredential)
    {
    }

    public VssCredentials(
      WindowsCredential windowsCredential,
      FederatedCredential federatedCredential)
      : this(windowsCredential, federatedCredential, VssCredentials.EnvironmentUserInteractive ? CredentialPromptType.PromptIfNeeded : CredentialPromptType.DoNotPrompt)
    {
    }

    public VssCredentials(
      WindowsCredential windowsCredential,
      FederatedCredential federatedCredential,
      CredentialPromptType promptType)
      : this(windowsCredential, federatedCredential, promptType, (TaskScheduler) null)
    {
    }

    public VssCredentials(
      WindowsCredential windowsCredential,
      FederatedCredential federatedCredential,
      CredentialPromptType promptType,
      TaskScheduler scheduler)
      : this(windowsCredential, federatedCredential, promptType, scheduler, (IVssCredentialPrompt) null)
    {
    }

    public VssCredentials(
      WindowsCredential windowsCredential,
      FederatedCredential federatedCredential,
      CredentialPromptType promptType,
      TaskScheduler scheduler,
      IVssCredentialPrompt credentialPrompt)
    {
      this.PromptType = promptType;
      if (promptType == CredentialPromptType.PromptIfNeeded && scheduler == null)
        scheduler = TaskScheduler.Default;
      if (windowsCredential != null)
      {
        this.m_windowsCredential = windowsCredential;
        this.m_windowsCredential.Scheduler = scheduler;
        this.m_windowsCredential.Prompt = credentialPrompt;
      }
      if (federatedCredential != null)
      {
        this.m_federatedCredential = federatedCredential;
        this.m_federatedCredential.Scheduler = scheduler;
        this.m_federatedCredential.Prompt = credentialPrompt;
      }
      this.m_thisLock = new object();
    }

    public static implicit operator VssCredentials(FederatedCredential credential) => new VssCredentials(credential);

    public static implicit operator VssCredentials(WindowsCredential credential) => new VssCredentials(credential);

    public CredentialPromptType PromptType
    {
      get => this.m_promptType;
      set => this.m_promptType = value != CredentialPromptType.PromptIfNeeded || VssCredentials.EnvironmentUserInteractive ? value : throw new ArgumentException(CommonResources.CannotPromptIfNonInteractive(), nameof (PromptType));
    }

    public FederatedCredential Federated => this.m_federatedCredential;

    public WindowsCredential Windows => this.m_windowsCredential;

    public IVssCredentialStorage Storage
    {
      get => this.m_credentialStorage;
      set
      {
        this.m_credentialStorage = value;
        if (this.m_windowsCredential != null)
          this.m_windowsCredential.Storage = value;
        if (this.m_federatedCredential == null)
          return;
        this.m_federatedCredential.Storage = value;
      }
    }

    internal virtual bool TryGetValidToken(IVssCredentialPrompt prompt) => false;

    internal IssuedTokenProvider CreateTokenProvider(
      Uri serverUrl,
      IHttpResponse webResponse,
      IssuedToken failedToken)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      VssTraceActivity current = VssTraceActivity.Current;
      lock (this.m_thisLock)
      {
        IssuedTokenProvider provider = this.m_currentProvider;
        if (provider == null || !provider.IsAuthenticationChallenge(webResponse))
        {
          if (this.m_federatedCredential != null && this.m_federatedCredential.IsAuthenticationChallenge(webResponse))
          {
            if (provider != null)
              VssHttpEventSource.Log.IssuedTokenProviderRemoved(current, provider);
            this.TryGetValidToken(this.m_federatedCredential.Prompt);
            provider = this.m_federatedCredential.CreateTokenProvider(serverUrl, webResponse, failedToken);
            if (provider != null)
              VssHttpEventSource.Log.IssuedTokenProviderCreated(current, provider);
          }
          else if (this.m_windowsCredential != null && this.m_windowsCredential.IsAuthenticationChallenge(webResponse))
          {
            if (provider != null)
              VssHttpEventSource.Log.IssuedTokenProviderRemoved(current, provider);
            provider = this.m_windowsCredential.CreateTokenProvider(serverUrl, webResponse, failedToken);
            if (provider != null)
              VssHttpEventSource.Log.IssuedTokenProviderCreated(current, provider);
          }
          this.m_currentProvider = provider;
        }
        return provider;
      }
    }

    public bool TryGetTokenProvider(Uri serverUrl, out IssuedTokenProvider provider)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      lock (this.m_thisLock)
      {
        if (this.m_currentProvider == null)
        {
          if (this.m_federatedCredential != null)
            this.m_currentProvider = this.m_federatedCredential.CreateTokenProvider(serverUrl, (IHttpResponse) null, (IssuedToken) null);
          if (this.m_currentProvider == null && this.m_windowsCredential != null)
            this.m_currentProvider = this.m_windowsCredential.CreateTokenProvider(serverUrl, (IHttpResponse) null, (IssuedToken) null);
          if (this.m_currentProvider != null)
            VssHttpEventSource.Log.IssuedTokenProviderCreated(VssTraceActivity.Current, this.m_currentProvider);
        }
        provider = this.m_currentProvider;
      }
      return provider != null;
    }

    internal bool IsAuthenticationChallenge(IHttpResponse webResponse)
    {
      if (webResponse == null)
        return false;
      bool flag = false;
      if (this.m_windowsCredential != null)
        flag = this.m_windowsCredential.IsAuthenticationChallenge(webResponse);
      if (!flag && this.m_federatedCredential != null)
        flag = this.m_federatedCredential.IsAuthenticationChallenge(webResponse);
      return flag;
    }

    internal void SignOut(Uri serverUrl, Uri serviceLocation, string identityProvider)
    {
      if (this.m_currentProvider != null && this.m_currentProvider.CurrentToken != null)
      {
        if (this.m_currentProvider.Credential.Storage != null && this.m_currentProvider.TokenStorageUrl != (Uri) null)
          this.m_currentProvider.Credential.Storage.RemoveToken(this.m_currentProvider.TokenStorageUrl, this.m_currentProvider.CurrentToken);
        this.m_currentProvider.CurrentToken = (IssuedToken) null;
      }
      if (!(this.m_currentProvider is ISupportSignOut currentProvider))
        return;
      if (serviceLocation != (Uri) null)
        serviceLocation = new Uri(serviceLocation.AbsoluteUri.Replace("{mode}", nameof (SignOut)).Replace("{redirectUrl}", serverUrl.AbsoluteUri));
      currentProvider.SignOut(serviceLocation, serverUrl, identityProvider);
    }

    public static VssCredentials LoadCachedCredentials(Uri serverUrl, bool requireExactMatch) => VssCredentials.LoadCachedCredentials((string) null, serverUrl, requireExactMatch);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static VssCredentials LoadCachedCredentials(
      string featureRegistryKeyword,
      Uri serverUrl,
      bool requireExactMatch)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      bool knownUri = false;
      VssCredentials vssCredentials = VssCredentials.LoadCachedCredentialsFromRegisteredProviders(serverUrl, out knownUri);
      if (vssCredentials == null && !knownUri)
      {
        WindowsCredential windowsCredential = (WindowsCredential) null;
        FederatedCredential federatedCredential = (FederatedCredential) null;
        TfsCredentialCacheEntry credentials = new CredentialsCacheManager().GetCredentials(featureRegistryKeyword, serverUrl, requireExactMatch, new bool?());
        if (credentials != null && credentials.NonInteractive)
        {
          switch (credentials.Type)
          {
            case CachedCredentialsType.Windows:
              windowsCredential = new WindowsCredential((ICredentials) credentials.Credentials);
              break;
            case CachedCredentialsType.ServiceIdentity:
              VssServiceIdentityToken initialToken = (VssServiceIdentityToken) null;
              string token = VssCredentials.ReadAuthorizationToken((IDictionary<string, string>) credentials.Attributes);
              if (!string.IsNullOrEmpty(token))
                initialToken = new VssServiceIdentityToken(token);
              federatedCredential = (FederatedCredential) new VssServiceIdentityCredential(credentials.Credentials.UserName, credentials.Credentials.Password, initialToken);
              break;
          }
        }
        vssCredentials = new VssCredentials(windowsCredential ?? new WindowsCredential(true), federatedCredential, CredentialPromptType.DoNotPrompt);
      }
      return vssCredentials ?? new VssCredentials(new WindowsCredential(true), (FederatedCredential) null, CredentialPromptType.DoNotPrompt);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static VssCredentials LoadCachedCredentialsFromRegisteredProviders(
      Uri serverUri,
      out bool knownUri)
    {
      VssCredentials.LoadRegisteredCachedVssCredentialProviders();
      bool flag = false;
      VssCredentials vssCredentials = (VssCredentials) null;
      foreach (KeyValuePair<string, ICachedVssCredentialProvider> credentialProvider in VssCredentials.m_loadedCachedVssCredentialProviders)
      {
        bool knownUri1 = false;
        vssCredentials = credentialProvider.Value?.GetCachedCredentials(serverUri, out knownUri1);
        if (vssCredentials != null | knownUri1)
        {
          flag |= knownUri1;
          break;
        }
      }
      knownUri = flag;
      return vssCredentials;
    }

    private static void LoadRegisteredCachedVssCredentialProviders() => CredentialsProviderRegistryHelper.LoadCachedVssCredentialProviders(ref VssCredentials.m_loadedCachedVssCredentialProviders);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void WriteAuthorizationToken(string token, IDictionary<string, string> attributes)
    {
      int num = 0;
      for (int startIndex = 0; startIndex < token.Length; startIndex += 128)
      {
        attributes["AuthTokenSegment" + num.ToString()] = token.Substring(startIndex, Math.Min(128, token.Length - startIndex));
        ++num;
      }
      attributes["AuthTokenSegmentCount"] = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    protected static string ReadAuthorizationToken(IDictionary<string, string> attributes)
    {
      string s;
      if (!attributes.TryGetValue("AuthTokenSegmentCount", out s))
        return string.Empty;
      int num = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < num; ++index)
      {
        string key = "AuthTokenSegment" + index.ToString();
        string str;
        if (attributes.TryGetValue(key, out str))
          stringBuilder.Append(str);
      }
      return stringBuilder.ToString();
    }

    protected static bool EnvironmentUserInteractive => Environment.UserInteractive;
  }
}
