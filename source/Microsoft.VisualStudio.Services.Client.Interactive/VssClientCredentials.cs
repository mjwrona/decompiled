// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssClientCredentials
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Client.Controls;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client
{
  public class VssClientCredentials : VssCredentials
  {
    public VssClientCredentials(
      WindowsCredential windowsCredential = null,
      FederatedCredential federatedCredential = null,
      CredentialPromptType? promptType = null,
      TaskScheduler scheduler = null,
      IVssCredentialPrompt credentialPrompt = null)
      : this(windowsCredential ?? new WindowsCredential(), federatedCredential ?? VssClientCredentials.CreateDefaultFederatedCredential(true), scheduler, promptType, credentialPrompt)
    {
    }

    public VssClientCredentials(bool useDefaultCredentials)
      : this(new WindowsCredential(useDefaultCredentials), VssClientCredentials.CreateDefaultFederatedCredential(useDefaultCredentials))
    {
    }

    private VssClientCredentials(
      WindowsCredential windowsCredential,
      FederatedCredential federatedCredential,
      TaskScheduler scheduler,
      CredentialPromptType? promptType = null,
      IVssCredentialPrompt credentialPrompt = null)
      : base(windowsCredential, federatedCredential, (CredentialPromptType) ((int) promptType ?? (int) VssClientCredentials.CreateDefaultPromptType(federatedCredential)), scheduler, credentialPrompt ?? (IVssCredentialPrompt) VssCredentialPrompts.CreateDefault(windowsCredential, federatedCredential))
    {
    }

    public VssClientCredentials(
      WindowsCredential windowsCredential,
      FederatedCredential federatedCredential,
      IDialogHost dialogHost)
      : base(windowsCredential, federatedCredential, !VssCredentials.EnvironmentUserInteractive || federatedCredential is VssServiceIdentityCredential ? CredentialPromptType.DoNotPrompt : CredentialPromptType.PromptIfNeeded, (TaskScheduler) null, (IVssCredentialPrompt) VssCredentialPrompts.CreatePromptsWithHost(windowsCredential, federatedCredential, dialogHost))
    {
    }

    private VssClientCredentials(
      WindowsCredential windowsCredential,
      FederatedCredential federatedCredential,
      CredentialPromptType promptType,
      TaskScheduler scheduler = null,
      IVssCredentialPrompt credentialPrompt = null)
      : base(windowsCredential, federatedCredential ?? VssClientCredentials.CreateDefaultFederatedCredential(), promptType, scheduler, credentialPrompt ?? (IVssCredentialPrompt) VssCredentialPrompts.CreateDefault(windowsCredential, federatedCredential ?? VssClientCredentials.CreateDefaultFederatedCredential()))
    {
    }

    public static implicit operator VssClientCredentials(FederatedCredential credential) => new VssClientCredentials(new WindowsCredential(), credential, VssClientCredentials.CreateDefaultPromptType(credential));

    public static implicit operator VssClientCredentials(WindowsCredential credential)
    {
      FederatedCredential federatedCredential = VssClientCredentials.CreateDefaultFederatedCredential();
      return new VssClientCredentials(credential, federatedCredential, !VssCredentials.EnvironmentUserInteractive || federatedCredential is VssServiceIdentityCredential ? CredentialPromptType.DoNotPrompt : CredentialPromptType.PromptIfNeeded);
    }

    internal override bool TryGetValidToken(IVssCredentialPrompt prompt)
    {
      if (prompt is VssCredentialPrompts credentialPrompts && credentialPrompts.FederatedPrompt != null)
      {
        VssFederatedCredentialPrompt federatedPrompt = credentialPrompts.FederatedPrompt as VssFederatedCredentialPrompt;
        string user;
        if (federatedPrompt.Parameters != null && federatedPrompt.Parameters.TryGetValue("user", out user) && !string.IsNullOrWhiteSpace(user))
          return VssAuthenticationHelper.CheckForValidTokenByUserAndTenantAsync(federatedPrompt, user, (string) null).SyncResultConfigured<bool>();
      }
      return false;
    }

    public new static VssCredentials LoadCachedCredentials(Uri serverUrl, bool requireExactMatch) => VssClientCredentials.LoadCachedCredentials((string) null, serverUrl, requireExactMatch);

    public static VssCredentials LoadCachedCredentials(
      Uri serverUrl,
      bool requireExactMatch,
      CredentialPromptType promptType)
    {
      return VssClientCredentials.LoadCachedCredentials((string) null, serverUrl, requireExactMatch, promptType);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new static VssCredentials LoadCachedCredentials(
      string featureRegistryKeyword,
      Uri serverUrl,
      bool requireExactMatch)
    {
      return VssClientCredentials.LoadCachedCredentials(featureRegistryKeyword, serverUrl, requireExactMatch, VssCredentials.EnvironmentUserInteractive ? CredentialPromptType.PromptIfNeeded : CredentialPromptType.DoNotPrompt);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static VssCredentials LoadCachedCredentials(
      string featureRegistryKeyword,
      Uri serverUrl,
      bool requireExactMatch,
      CredentialPromptType promptType)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      bool knownUri = false;
      VssCredentials vssCredentials = VssCredentials.LoadCachedCredentialsFromRegisteredProviders(serverUrl, out knownUri);
      if (vssCredentials == null && !knownUri)
      {
        WindowsCredential windowsCredential = (WindowsCredential) null;
        FederatedCredential federatedCredential = (FederatedCredential) null;
        TfsCredentialCacheEntry cacheEntry = new CredentialsCacheManager().GetCredentials(featureRegistryKeyword, serverUrl, requireExactMatch, new bool?());
        if (cacheEntry != null && (cacheEntry.NonInteractive || promptType == CredentialPromptType.PromptIfNeeded))
        {
          switch (cacheEntry.Type)
          {
            case CachedCredentialsType.Windows:
              windowsCredential = new WindowsCredential((ICredentials) cacheEntry.Credentials);
              break;
            case CachedCredentialsType.ServiceIdentity:
              VssServiceIdentityToken initialToken = (VssServiceIdentityToken) null;
              string token = VssCredentials.ReadAuthorizationToken((IDictionary<string, string>) cacheEntry.Attributes);
              if (!string.IsNullOrEmpty(token))
                initialToken = new VssServiceIdentityToken(token);
              federatedCredential = (FederatedCredential) new VssServiceIdentityCredential(cacheEntry.Credentials.UserName, cacheEntry.Credentials.Password, initialToken);
              break;
            case CachedCredentialsType.OAuth:
              VssSigningCredentials signingCredentials = VssSigningCredentials.Create((Func<RSACryptoServiceProvider>) (() =>
              {
                RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(2048);
                cryptoServiceProvider.ImportParameters(RSAParametersExtensions.FromJsonString(cacheEntry.Credentials.Password));
                return cryptoServiceProvider;
              }));
              string clientId;
              if (!cacheEntry.Attributes.TryGetValue(CredentialsCacheConstants.OAuthClientIdKeyword, out clientId))
                throw new ArgumentNullException(CredentialsCacheConstants.OAuthClientIdKeyword);
              string str;
              if (!cacheEntry.Attributes.TryGetValue(CredentialsCacheConstants.OAuthorizationUrlKeyword, out str))
                throw new ArgumentNullException(CredentialsCacheConstants.OAuthorizationUrlKeyword);
              VssOAuthJwtBearerClientCredential clientCredential = new VssOAuthJwtBearerClientCredential(clientId, str, signingCredentials);
              federatedCredential = (FederatedCredential) new Microsoft.VisualStudio.Services.OAuth.VssOAuthCredential(new Uri(str), (VssOAuthGrant) VssOAuthGrant.ClientCredentials, (VssOAuthClientCredential) clientCredential);
              break;
          }
        }
        vssCredentials = (VssCredentials) new VssClientCredentials(windowsCredential ?? new WindowsCredential(true), federatedCredential ?? VssClientCredentials.CreateDefaultFederatedCredential(true), promptType);
      }
      return vssCredentials ?? new VssCredentials(new WindowsCredential(true), VssClientCredentials.CreateDefaultFederatedCredential(true), CredentialPromptType.DoNotPrompt);
    }

    private static FederatedCredential CreateDefaultFederatedCredential(bool useDefaultCredentials = false) => VssClientEnvironment.GetSharedConnectedUserValue<bool>("UseAadWindowsIntegrated") ? (FederatedCredential) new VssAadCredential() : (FederatedCredential) new VssFederatedCredential(useDefaultCredentials);

    private static CredentialPromptType CreateDefaultPromptType(
      FederatedCredential federatedCredential)
    {
      return !VssCredentials.EnvironmentUserInteractive || federatedCredential is VssServiceIdentityCredential ? CredentialPromptType.DoNotPrompt : CredentialPromptType.PromptIfNeeded;
    }
  }
}
