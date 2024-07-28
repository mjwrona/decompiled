// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Authentication.VsoCredentialHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Client.AccountManagement;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common.Authentication.CredentialProvider;
using Microsoft.VisualStudio.Services.Content.Common.Operations;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common.Authentication
{
  public class VsoCredentialHelper
  {
    internal IAadErrorHandlingPolicy AadErrorPolicy;
    public readonly Guid CorrelationId;
    internal InMemoryLog RequiredLog;
    private Action<string> optionalTrace;

    public VsoCredentialHelper(IAppTraceSource tracer, Guid correlationId = default (Guid))
      : this((Action<string>) (msg => tracer.Verbose(msg)), correlationId)
    {
    }

    public VsoCredentialHelper(Action<string> verboseLog, Guid correlationId = default (Guid))
      : this(correlationId)
    {
      this.optionalTrace = verboseLog != null ? verboseLog : throw new ArgumentNullException(nameof (verboseLog));
    }

    public VsoCredentialHelper(Guid correlationId = default (Guid))
    {
      if (correlationId == new Guid())
        correlationId = Guid.NewGuid();
      this.CorrelationId = correlationId;
      this.RequiredLog = new InMemoryLog();
      this.AadErrorPolicy = (IAadErrorHandlingPolicy) new AadErrorHandlingPolicy(5, TimeSpan.FromSeconds(2.0), new LogCallback(this.Log));
    }

    private void Log(Microsoft.Identity.Client.LogLevel level, string message, bool containsPii)
    {
      this.RequiredLog.Log(new InMemoryLogMessage(message));
      if (this.optionalTrace == null)
        return;
      this.optionalTrace(message);
    }

    private void Log(string message) => this.Log(Microsoft.Identity.Client.LogLevel.Verbose, message, false);

    private AadAcquireTokenException CreateAcquireTokenException(Exception lastException)
    {
      StringBuilder stringBuilder = new StringBuilder("Failed to authenticate via AAD due to an AcquireToken exception. ");
      stringBuilder.AppendLine("Log of MSAL Tracing and retries:");
      foreach (InMemoryLogMessage memoryLogMessage in this.RequiredLog.GetSnapshot().ToArray<InMemoryLogMessage>())
        stringBuilder.AppendLine(memoryLogMessage.DisplayMessage);
      if (lastException != null)
        stringBuilder.AppendLine("AcquireToken exception: " + lastException.ToString());
      return new AadAcquireTokenException(stringBuilder.ToString(), lastException);
    }

    public VssCredentials GetCredentials(
      Uri serviceUri,
      bool useAad,
      SecureString pat,
      Prompt prompt,
      string aadCredentialTokenFile = null,
      LocalTokenCacheArgs localTokenCacheArgs = null)
    {
      byte[] existingAadTokenCacheBytes = (byte[]) null;
      if (!string.IsNullOrWhiteSpace(aadCredentialTokenFile))
      {
        this.Log("Reading existing AAD credentialTokenFile: " + aadCredentialTokenFile);
        existingAadTokenCacheBytes = System.IO.File.ReadAllBytes(aadCredentialTokenFile);
      }
      return this.GetCredentials(serviceUri, useAad, existingAadTokenCacheBytes, pat, prompt, localTokenCacheArgs);
    }

    [Obsolete("Please provide the PAT as a SecureString via the other GetCredentials override")]
    public VssCredentials GetCredentials(
      Uri serviceUri,
      bool useAad,
      byte[] existingAadTokenCacheBytes,
      string pat)
    {
      SecureString pat1 = SecureStringFactory.Create(pat);
      return this.GetCredentials(serviceUri, useAad, existingAadTokenCacheBytes, pat1, Prompt.Never);
    }

    public VssCredentials GetCredentials(
      Uri serviceUri,
      bool useAad,
      byte[] existingAadTokenCacheBytes,
      SecureString pat,
      Prompt prompt,
      LocalTokenCacheArgs localTokenCacheArgs = null)
    {
      return this.GetCredentialsAsync(serviceUri, useAad, existingAadTokenCacheBytes, pat, prompt, localTokenCacheArgs).GetAwaiter().GetResult();
    }

    public Task<VssCredentials> GetCredentialsAsync(
      Uri serviceUri,
      bool useAad,
      string aadCredentialTokenFile,
      SecureString pat,
      Prompt prompt,
      LocalTokenCacheArgs localTokenCacheArgs = null)
    {
      byte[] existingAadTokenCacheBytes = (byte[]) null;
      if (!string.IsNullOrWhiteSpace(aadCredentialTokenFile))
      {
        this.Log("Reading existing AAD credentialTokenFile: " + aadCredentialTokenFile);
        existingAadTokenCacheBytes = System.IO.File.ReadAllBytes(aadCredentialTokenFile);
      }
      return this.GetCredentialsAsync(serviceUri, useAad, existingAadTokenCacheBytes, pat, prompt, localTokenCacheArgs);
    }

    public async Task<VssCredentials> GetCredentialsAsync(
      Uri serviceUri,
      bool useAad,
      byte[] existingAadTokenCacheBytes,
      SecureString pat,
      Prompt prompt,
      LocalTokenCacheArgs localTokenCacheArgs = null)
    {
      VsoCredentialHelper credentialHelper = this;
      if (serviceUri == (Uri) null)
        throw new ArgumentNullException(nameof (serviceUri));
      if (useAad && pat != null)
        throw new ArgumentException("AAD and PAT authentication cannot be used at the same time");
      if (!useAad && existingAadTokenCacheBytes != null)
        throw new ArgumentException("useAAD is false, but existingAADTokenCacheBytes was specified", nameof (useAad));
      VssCredentials credentialsAsync = (VssCredentials) null;
      ConfiguredTaskAwaitable<VssCredentials> configuredTaskAwaitable;
      if (existingAadTokenCacheBytes == null && pat == null && (prompt == Prompt.NoPrompt || prompt == Prompt.Never))
      {
        configuredTaskAwaitable = credentialHelper.GetCredentialsFromProviderAsync(serviceUri).ConfigureAwait(false);
        credentialsAsync = await configuredTaskAwaitable;
        if (credentialsAsync != null)
          credentialHelper.Log("Authenticating with credential from CredentialProviderManager");
      }
      if (credentialsAsync == null)
      {
        VsoAadAuthority authority = new VsoAadAuthority(new Action<string>(credentialHelper.Log));
        if (existingAadTokenCacheBytes != null)
        {
          configuredTaskAwaitable = credentialHelper.GetAADCredentialsAsync(existingAadTokenCacheBytes, authority, prompt).ConfigureAwait(false);
          credentialsAsync = await configuredTaskAwaitable;
        }
        else if (pat != null)
          credentialsAsync = credentialHelper.GetPATCredentials(pat);
        else if (useAad)
        {
          if (localTokenCacheArgs != null && (prompt == Prompt.NoPrompt || prompt == Prompt.Never))
          {
            credentialsAsync = credentialHelper.GetCachedPATCredentials(serviceUri, prompt, localTokenCacheArgs);
          }
          else
          {
            configuredTaskAwaitable = credentialHelper.GetAADCredentialsAsync(authority, prompt).ConfigureAwait(false);
            credentialsAsync = await configuredTaskAwaitable;
          }
        }
        else
        {
          // ISSUE: reference to a compiler-generated method
          credentialsAsync = await Task.Run<VssCredentials>(new Func<VssCredentials>(credentialHelper.\u003CGetCredentialsAsync\u003Eb__14_0));
        }
      }
      credentialHelper.Log("Credentials acquired");
      return credentialsAsync;
    }

    public virtual VssCredentials GetAADCredentials(
      byte[] encryptedSerializedAadTokenCacheBytes,
      VsoAadAuthority authority,
      Prompt prompt)
    {
      return VsoCredentialHelper.SyncResultOnThreadPool<VssCredentials>((Func<Task<VssCredentials>>) (() => this.GetAADCredentialsAsync(encryptedSerializedAadTokenCacheBytes, authority, prompt)));
    }

    public virtual VssCredentials GetAADCredentials(VsoAadAuthority authority, Prompt prompt) => VsoCredentialHelper.SyncResultOnThreadPool<VssCredentials>((Func<Task<VssCredentials>>) (() => this.GetAADCredentialsAsync((byte[]) null, authority, prompt)));

    public virtual Task<VssCredentials> GetAADCredentialsAsync(
      VsoAadAuthority authority,
      Prompt prompt)
    {
      return this.GetAADCredentialsAsync((byte[]) null, authority, prompt);
    }

    public virtual async Task<VssCredentials> GetAADCredentialsAsync(
      byte[] encryptedSerializedAadTokenCacheBytes,
      VsoAadAuthority authority,
      Prompt prompt)
    {
      try
      {
        this.AadErrorPolicy.StartAadLogging();
        if (encryptedSerializedAadTokenCacheBytes == null || encryptedSerializedAadTokenCacheBytes.Length == 0)
        {
          this.Log("Authenticating with new AAD token");
          return (VssCredentials) (FederatedCredential) await this.AcquireTokenInternalAsync(authority, prompt).ConfigureAwait(false);
        }
        this.Log("Authenticating with existing AAD token");
        return (VssCredentials) (FederatedCredential) await this.AcquireTokenInternalAsync(encryptedSerializedAadTokenCacheBytes, authority, prompt).ConfigureAwait(false);
      }
      finally
      {
        this.AadErrorPolicy.StopAadLogging();
      }
    }

    public virtual VssCredentials GetPATCredentials(SecureString pat)
    {
      this.Log("Authenticating with PAT");
      return (VssCredentials) (FederatedCredential) new VssBasicCredential((ICredentials) new NetworkCredential("PAT", pat));
    }

    internal virtual VssCredentials GetPromptCredentials()
    {
      this.Log("Authenticating with VSS client via cache or prompt");
      VssClientCredentials promptCredentials = new VssClientCredentials();
      promptCredentials.Storage = (IVssCredentialStorage) new VssClientCredentialStorage();
      return (VssCredentials) promptCredentials;
    }

    private Task<VssCredentials> GetCredentialsFromProviderAsync(Uri serviceUri) => this.CreateProviderManager(new Action<string>(this.Log)).GetCredentialsAsync(serviceUri, false, false);

    internal virtual CredentialProviderManager CreateProviderManager(Action<string> log) => new CredentialProviderManager(log);

    internal virtual VssCredentials GetCachedPATCredentials(
      Uri serviceUri,
      Prompt prompt,
      LocalTokenCacheArgs localTokenCacheArgs)
    {
      VsoAadAuthority authority = new VsoAadAuthority(new Action<string>(this.Log));
      Uri targetBaseUri = new Uri(serviceUri.GetLeftPart(UriPartial.Authority));
      Uri credName = new Uri(string.Format("{0}:{1}", (object) localTokenCacheArgs.AppName, (object) targetBaseUri));
      CredentialsCacheManager credCacheManger = new CredentialsCacheManager();
      NetworkCredential credentialsCacheManager1 = this.GetCredentialsFromCredentialsCacheManager(credCacheManger, credName);
      VssCredentials creds;
      if (credentialsCacheManager1?.SecurePassword == null || localTokenCacheArgs.InvalidateCachedToken)
      {
        creds = this.GetAADCredentials(authority, prompt);
        int errorCode;
        if (this.IsCredentialManagerAvailable(credCacheManger, localTokenCacheArgs, out errorCode))
          TaskSafety.SyncResultOnThreadPool((Func<Task>) (async () =>
          {
            SessionToken sessionToken = (SessionToken) null;
            string scope = localTokenCacheArgs.Scope;
            string displayName = credName.ToString() + " on " + Environment.MachineName;
            try
            {
              Guid serviceInstanceId = await this.GetServiceInstanceIdAsync(targetBaseUri, creds).ConfigureAwait(false);
              string newValue = targetBaseUri.Host.Split('.')[0];
              UriBuilder uriBuilder = new UriBuilder(AccountManager.VsoEndpoint);
              uriBuilder.Host = uriBuilder.Host.Replace("app", newValue);
              sessionToken = await this.CreateSessionToken(new TokenHttpClient(uriBuilder.Uri, creds), scope, displayName, serviceInstanceId);
              this.Log(string.Format("Created a PAT to store on the local machine with expiration '{0}'.", (object) sessionToken?.ValidTo));
            }
            catch (Exception ex)
            {
              this.Log("Failed to create a '" + scope + "' scoped PAT with display name: '" + displayName + "' to store on the local machine.");
              this.Log(ex.ToString());
            }
            if (sessionToken?.Token == null)
            {
              sessionToken = (SessionToken) null;
              scope = (string) null;
              displayName = (string) null;
            }
            else
            {
              int credentialsCacheManager2 = this.SetCredentialsToCredentialsCacheManager(credCacheManger, credName, "PAT", sessionToken.Token);
              if (credentialsCacheManager2 == 0)
              {
                this.Log("Stored the PAT on the local machine.");
                sessionToken = (SessionToken) null;
                scope = (string) null;
                displayName = (string) null;
              }
              else
              {
                this.Log(string.Format("Failed to store the PAT on the local machine (ErrorCode: {0}).", (object) credentialsCacheManager2));
                sessionToken = (SessionToken) null;
                scope = (string) null;
                displayName = (string) null;
              }
            }
          }));
        else
          this.Log(string.Format("Windows Credential Manager is not available (ErrorCode: {0}).", (object) errorCode));
      }
      else
      {
        this.Log(string.Format("Found cached PAT in Windows Credential Manager for {0}", (object) credName));
        creds = this.GetPATCredentials(credentialsCacheManager1.SecurePassword);
      }
      return creds;
    }

    public virtual Task<SessionToken> CreateSessionToken(
      TokenHttpClient authClient,
      string scope,
      string displayName,
      Guid serviceInstanceId)
    {
      TokenHttpClient tokenHttpClient = authClient;
      SessionToken sessionToken = new SessionToken();
      sessionToken.Scope = scope;
      sessionToken.DisplayName = displayName;
      sessionToken.TargetAccounts = (IList<Guid>) new List<Guid>()
      {
        serviceInstanceId
      };
      SessionTokenType? tokenType = new SessionTokenType?(SessionTokenType.Compact);
      bool? isPublic = new bool?();
      bool? isRequestedByTfsPatWebUI = new bool?();
      CancellationToken cancellationToken = new CancellationToken();
      return tokenHttpClient.CreateSessionTokenAsync(sessionToken, tokenType, isPublic, isRequestedByTfsPatWebUI, cancellationToken: cancellationToken);
    }

    public virtual NetworkCredential GetCredentialsFromCredentialsCacheManager(
      CredentialsCacheManager credCacheManger,
      Uri credName)
    {
      return credCacheManger.GetCredentials(credName)?.Credentials;
    }

    public virtual bool DeleteCredentialsFromCredentialsCacheManager(
      CredentialsCacheManager credCacheManger,
      Uri credName)
    {
      return credCacheManger.DeleteCredentials(credName);
    }

    public virtual int SetCredentialsToCredentialsCacheManager(
      CredentialsCacheManager credCacheManger,
      Uri credName,
      string username,
      string password)
    {
      return credCacheManger.StoreCredentials(credName, username, password);
    }

    public virtual async Task<Guid> GetServiceInstanceIdAsync(
      Uri targetUri,
      VssCredentials credential)
    {
      Guid instanceId;
      using (LocationHttpClient client = new LocationHttpClient(targetUri, credential))
        instanceId = (await client.GetConnectionDataAsync(ConnectOptions.None, 0L)).InstanceId;
      return instanceId;
    }

    public virtual bool IsCredentialManagerAvailable(
      CredentialsCacheManager credCacheManger,
      LocalTokenCacheArgs localTokenCacheArgs,
      out int errorCode)
    {
      Uri credName = new Uri(localTokenCacheArgs.AppName + ":Test");
      errorCode = this.SetCredentialsToCredentialsCacheManager(credCacheManger, credName, "Test", "Test");
      if (errorCode == 0)
        this.DeleteCredentialsFromCredentialsCacheManager(credCacheManger, credName);
      return errorCode == 0;
    }

    private Task<VssAadCredential> AcquireTokenInternalAsync(
      byte[] credBytes,
      VsoAadAuthority authority,
      Prompt prompt)
    {
      return this.AcquireTokenInternalAsync(authority, (Func<ITokenCache, bool>) (tokenCache =>
      {
        VsoCredentialHelper.DeserializeData(credBytes, tokenCache);
        return true;
      }), prompt);
    }

    private static void DeserializeData(byte[] credBytes, ITokenCache tokenCache) => tokenCache.SetBeforeAccess((TokenCacheCallback) (args =>
    {
      byte[] msalV3State = ProtectedData.Unprotect(credBytes, (byte[]) null, DataProtectionScope.CurrentUser);
      args.TokenCache.DeserializeMsalV3(msalV3State);
    }));

    private Task<VssAadCredential> AcquireTokenInternalAsync(
      VsoAadAuthority authority,
      Prompt prompt)
    {
      return this.AcquireTokenInternalAsync(authority, (Func<ITokenCache, bool>) (tokenCache => false), prompt);
    }

    private async Task<VssAadCredential> AcquireTokenInternalAsync(
      VsoAadAuthority authority,
      Func<ITokenCache, bool> updatedTokenCacheForSilentAuth,
      Prompt prompt)
    {
      ITestableAuthenticationContext authCtx = this.CreateAuthenticationContext(authority);
      string methodName = "AcquireTokenAsync with VssAadCredential for IntegratedAuth";
      if (updatedTokenCacheForSilentAuth != null && updatedTokenCacheForSilentAuth(authCtx.TokenCache))
        methodName = "AcquireTokenSilentAsync with updated TokenCache";
      Func<string[], Task<AuthenticationResult>> tokenAcquisitionDelegate;
      try
      {
        try
        {
          tokenAcquisitionDelegate = (Func<string[], Task<AuthenticationResult>>) (scopes => authCtx.AcquireTokenSilentAsync(scopes, new Guid?(this.CorrelationId)));
          await this.AcquireToken(tokenAcquisitionDelegate).ConfigureAwait(false);
        }
        catch
        {
          VssAadCredential userCredential = new VssAadCredential();
          tokenAcquisitionDelegate = (Func<string[], Task<AuthenticationResult>>) (scopes => authCtx.AcquireTokenAsync(scopes, userCredential.Username, userCredential.Password, new Guid?(this.CorrelationId)));
          await this.AcquireToken(tokenAcquisitionDelegate).ConfigureAwait(false);
        }
      }
      catch (AadAcquireTokenException ex) when (this.IsInteractionRequired((Exception) ex))
      {
        if (prompt != Prompt.Never && !this.IsEnvironmentUserInteractive())
        {
          this.Log(string.Format("Ignoring {0}.{1} because Environment.UserInteractive is false.", (object) "Prompt", (object) prompt));
          prompt = Prompt.Never;
        }
        this.Log(string.Format("{0} failed. Calling AcquireTokenAsync with Prompt.{1}.", (object) methodName, (object) prompt));
        tokenAcquisitionDelegate = (Func<string[], Task<AuthenticationResult>>) (scopes => authCtx.AcquireTokenAsync(scopes, prompt, new Guid?(this.CorrelationId)));
        await this.AcquireToken(tokenAcquisitionDelegate).ConfigureAwait(false);
      }
      VssAadCredential vssAadCredential = new VssAadCredential(new VssAadToken((Func<string[], AuthenticationResult>) (scopes => tokenAcquisitionDelegate(scopes).SyncResultConfigured<AuthenticationResult>())));
      methodName = (string) null;
      return vssAadCredential;
    }

    private Task AcquireToken(
      Func<string[], Task<AuthenticationResult>> tokenDelegate)
    {
      return (Task) this.AcquireTokenWithRetryAsync((Func<Task<AuthenticationResult>>) (() => tokenDelegate(VsoAadConstants.DefaultScopes)));
    }

    protected virtual bool IsEnvironmentUserInteractive() => Environment.UserInteractive;

    private ITestableAuthenticationContext CreateAuthenticationContext(VsoAadAuthority authority)
    {
      string authority1 = authority.Authority;
      this.Log("Using AAD Authority: " + authority1);
      return this.CreateAuthenticationContext(authority1);
    }

    internal virtual ITestableAuthenticationContext CreateAuthenticationContext(string authorityUri)
    {
      PublicClientApplicationBuilder applicationBuilder = PublicClientApplicationBuilder.Create("872cd9fa-d31f-45e0-9eab-6e460a02d1f1").WithAuthority(authorityUri);
      LogCallback loggingCallback = (LogCallback) ((level, message, containsPii) => this.Log(level, message, containsPii));
      bool? nullable = new bool?(false);
      Microsoft.Identity.Client.LogLevel? logLevel = new Microsoft.Identity.Client.LogLevel?();
      bool? enablePiiLogging = nullable;
      bool? enableDefaultPlatformLogging = new bool?();
      return (ITestableAuthenticationContext) new TestableAuthenticationContext(BrokerExtension.WithBroker(applicationBuilder.WithLogging(loggingCallback, logLevel, enablePiiLogging, enableDefaultPlatformLogging).WithRedirectUri(VssAadSettings.NativeClientRedirectUri.AbsoluteUri), new BrokerOptions(BrokerOptions.OperatingSystems.Windows)
      {
        Title = "Azure DevOps",
        ListOperatingSystemAccounts = false
      }));
    }

    private async Task<AuthenticationResult> AcquireTokenWithRetryAsync(
      Func<Task<AuthenticationResult>> getAuthResult)
    {
      int attemptIndex = 0;
      AuthenticationResult authResult = (AuthenticationResult) null;
      Exception lastException = (Exception) null;
      for (; authResult == null && attemptIndex < this.AadErrorPolicy.AadRetryCount + 1; ++attemptIndex)
      {
        try
        {
          authResult = await getAuthResult();
          if (authResult == null)
            this.Log("AuthenticationResult was null.");
        }
        catch (Exception ex)
        {
          lastException = ex;
          StringBuilder stringBuilder = new StringBuilder("Authentication failed. ");
          if (ex is MsalException msalException)
            stringBuilder.Append("MsalException.ErrorCode=" + msalException.ErrorCode + " ");
          if (msalException is MsalServiceException serviceException)
            stringBuilder.Append(string.Format("MsalServiceException.StatusCode={0} ", (object) serviceException.StatusCode));
          stringBuilder.Append(ex.ToString());
          this.Log(stringBuilder.ToString());
          if (this.ShouldNotRetryGetAuthResultException(ex))
            break;
        }
        if (attemptIndex < this.AadErrorPolicy.AadRetryCount && authResult == null)
        {
          this.Log(string.Format("Retry {0} of {1} in {2}s...", (object) (attemptIndex + 1), (object) this.AadErrorPolicy.AadRetryCount, (object) this.AadErrorPolicy.AadRetryDelay.TotalSeconds));
          await Task.Delay(this.AadErrorPolicy.AadRetryDelay);
        }
      }
      AuthenticationResult authenticationResult = authResult != null ? authResult : throw this.CreateAcquireTokenException(lastException);
      authResult = (AuthenticationResult) null;
      lastException = (Exception) null;
      return authenticationResult;
    }

    private bool ShouldNotRetryGetAuthResultException(Exception e)
    {
      if (e is AadAcquireTokenException)
        e = e.InnerException;
      if (this.IsInteractionRequired(e))
        return true;
      return e is MsalClientException msalClientException && msalClientException.ErrorCode == "authentication_canceled";
    }

    private bool IsInteractionRequired(Exception e)
    {
      if (e is AadAcquireTokenException)
        e = e.InnerException;
      if (e is MsalUiRequiredException)
        return true;
      if (!(e is MsalClientException msalClientException))
        return false;
      return msalClientException.ErrorCode == "failed_to_acquire_token_silently_from_broker" || msalClientException.ErrorCode == "activity_required" || msalClientException.ErrorCode == "failed_to_refresh_token" || msalClientException.ErrorCode == "invalid_grant" || msalClientException.ErrorCode == "no_account_for_login_hint" || msalClientException.ErrorCode == "no_prompt_failed" || msalClientException.ErrorCode == "get_user_name_failed" || msalClientException.ErrorCode == "federated_service_returned_error" || msalClientException.ErrorCode == "interaction_required";
    }

    private static T SyncResultOnThreadPool<T>(Func<Task<T>> taskFunc) => Task.Run<T>(taskFunc).GetAwaiter().GetResult();
  }
}
