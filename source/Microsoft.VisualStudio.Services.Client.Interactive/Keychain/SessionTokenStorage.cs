// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Keychain.SessionTokenStorage
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Client.AccountManagement;
using Microsoft.VisualStudio.Services.Client.AccountManagement.Logging;
using Microsoft.VisualStudio.Services.Client.Keychain.Logging;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.TokenStorage;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.Keychain
{
  public class SessionTokenStorage
  {
    private readonly ILogger logger;
    private readonly VssTokenStorage tokenStorage;
    internal const string SessionTokenKind = "SessionToken";
    internal const string DateToRefreshSessionTokenPropertyName = "DateToRefreshSessionToken";
    internal const string SessionTokenValidToPropertyName = "ValidTo";
    internal const string SessionTokenValidFromPropertyName = "ValidFrom";
    internal const string TokenType = "Basic";
    internal const string SessionTokenUserIdPropertyName = "UserId";
    internal const string RefreshedOnPropertyName = "RefreshedOn";
    private const int maxRetries = 3;
    private const int retryDelay = 1;

    internal SessionTokenStorage(
      ILogger logger,
      VssTokenStorage tokenStorage,
      SessionTokenStorage.GetSessionTokenFromVSO getSessionTokenFromServer)
    {
      if (tokenStorage == null)
        tokenStorage = AccountManager.DefaultInstance.GetCache<VssTokenStorage>();
      this.tokenStorage = tokenStorage;
      this.logger = logger ?? AccountManager.Logger;
      this.GetSessionTokenFromServer = getSessionTokenFromServer ?? new SessionTokenStorage.GetSessionTokenFromVSO(this.RetreiveAndStoreSessionTokensFromVSO);
    }

    public SessionTokenStorage(ILogger logger = null, VssTokenStorage tokenStorage = null)
      : this(logger, tokenStorage, (SessionTokenStorage.GetSessionTokenFromVSO) null)
    {
    }

    internal SessionTokenStorage.GetSessionTokenFromVSO GetSessionTokenFromServer { get; set; }

    public void ClearSessionTokensForAccount(Account account = null)
    {
      IEnumerable<VssToken> source = this.tokenStorage.RetrieveAll("SessionToken");
      if (account != null)
        source = source.Where<VssToken>((Func<VssToken, bool>) (token => string.Equals(token.Resource, account.UniqueId, StringComparison.OrdinalIgnoreCase)));
      foreach (VssTokenKey tokenKey in source)
        this.tokenStorage.Remove(tokenKey);
    }

    public IEnumerable<SessionToken> GetAllSessionTokens(Account account = null)
    {
      List<SessionToken> allSessionTokens = new List<SessionToken>();
      foreach (VssToken token in account != null ? this.tokenStorage.RetrieveAll("SessionToken").Where<VssToken>((Func<VssToken, bool>) (token => string.Equals(token.Resource, account.UniqueId, StringComparison.OrdinalIgnoreCase))) : this.tokenStorage.RetrieveAll("SessionToken"))
      {
        SessionToken sessionToken = new SessionToken();
        string property1 = token.GetProperty("ValidTo");
        if (!string.IsNullOrEmpty(property1))
          sessionToken.ValidTo = DateTime.FromFileTimeUtc(Convert.ToInt64(property1));
        string property2 = token.GetProperty("ValidFrom");
        if (!string.IsNullOrEmpty(property1))
          sessionToken.ValidFrom = DateTime.FromFileTimeUtc(Convert.ToInt64(property2));
        string property3 = token.GetProperty("DateToRefreshSessionToken");
        if (!string.IsNullOrEmpty(property1))
          sessionToken.DateToRefresh = DateTime.FromFileTimeUtc(Convert.ToInt64(property3));
        string property4 = token.GetProperty("RefreshedOn");
        if (!string.IsNullOrEmpty(property1))
          sessionToken.RefreshedOn = DateTime.FromFileTimeUtc(Convert.ToInt64(property4));
        sessionToken.UserId = token.GetProperty("UserId");
        token.RefreshTokenValue();
        SessionTokenPair tokenPair = SessionTokenPair.GetTokenPair(token.TokenValue);
        sessionToken.HasCompact = !string.IsNullOrEmpty(tokenPair.GetToken(SessionTokenType.Compact));
        SessionTokenDescriptor sessionTokenDescriptor = SessionTokenDescriptor.FromKey(token);
        sessionToken.Scope = sessionTokenDescriptor.Scope;
        sessionToken.TargetAccounts = (IEnumerable<Guid>) sessionTokenDescriptor.TargetAccounts;
        allSessionTokens.Add(sessionToken);
      }
      return (IEnumerable<SessionToken>) allSessionTokens;
    }

    public async Task<string> GetSessionTokenFromAccountAsync(
      Account account,
      SessionTokenDescriptor tokenParameter,
      bool forceRefresh = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SessionTokenStorage sessionTokenStorage1 = this;
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      ArgumentUtility.CheckForNull<SessionTokenDescriptor>(tokenParameter, nameof (tokenParameter));
      SessionTokenStorage sessionTokenStorage2 = sessionTokenStorage1;
      Account account1 = account;
      List<SessionTokenDescriptor> tokenParameters = new List<SessionTokenDescriptor>();
      tokenParameters.Add(tokenParameter);
      int num = forceRefresh ? 1 : 0;
      CancellationToken cancellationToken1 = cancellationToken;
      IDictionary<SessionTokenDescriptor, string> accountImplAsync = await sessionTokenStorage2.GetSessionTokenFromAccountImplAsync(account1, (IEnumerable<SessionTokenDescriptor>) tokenParameters, num != 0, cancellationToken1);
      return accountImplAsync == null || !accountImplAsync.Any<KeyValuePair<SessionTokenDescriptor, string>>() ? (string) null : accountImplAsync.FirstOrDefault<KeyValuePair<SessionTokenDescriptor, string>>().Value;
    }

    internal bool DoesSessionTokenRequireRefresh(
      DateTime now,
      VssToken token,
      string lifeTime,
      out bool expired)
    {
      ArgumentUtility.CheckForNull<VssToken>(token, nameof (token));
      expired = false;
      string property1 = token.GetProperty("DateToRefreshSessionToken");
      string property2 = token.GetProperty("ValidTo");
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(property1, "DateToRefreshSessionToken");
        ArgumentUtility.CheckStringForNullOrEmpty(property2, "ValidTo");
        DateTime dateTime1 = DateTime.FromFileTimeUtc(Convert.ToInt64(property1));
        DateTime dateTime2 = DateTime.FromFileTimeUtc(Convert.ToInt64(property2));
        if (dateTime2 <= now)
        {
          expired = true;
          DiagnosticsLogging.LogInformationEvent(this.logger, "SessionTokenRequiredRefresh", string.Format("Token '{0}' is expired because the ValidToDate in FileTimeUtc was '{1}' and the current date is '{2}'", (object) token.Resource, (object) dateTime2.ToFileTimeUtc(), (object) now.ToFileTimeUtc()));
          return true;
        }
        if (!string.IsNullOrEmpty(lifeTime) && dateTime2 <= now.AddHours(double.Parse(lifeTime)))
        {
          expired = true;
          DiagnosticsLogging.LogInformationEvent(this.logger, "SessionTokenRequiredRefresh", string.Format("Token '{0}' is still valid but it doesn't have requested lifetime so it is considered as expired. Requested lifetime is '{1}', the ValidToDate in FileTimeUtc was '{2}' and the current date is '{3}'", (object) token.Resource, (object) lifeTime, (object) dateTime2.ToFileTimeUtc(), (object) now.ToFileTimeUtc()));
          return true;
        }
        if (dateTime1 > now)
          return false;
        DiagnosticsLogging.LogInformationEvent(this.logger, "SessionTokenRequiredRefresh", string.Format("Require a refresh for token '{0}' because the refresh date in FileTimeUtc was '{1}'", (object) token.Resource, (object) dateTime1.ToFileTimeUtc()));
      }
      catch (Exception ex)
      {
        DiagnosticsLogging.LogInformationEvent(this.logger, "SessionTokenRequiredRefresh", string.Format("There was a problem accessing the session token '{0}', a refresh is required. '{1}'", (object) token.Resource, (object) ex.Message), exception: ex);
      }
      return true;
    }

    internal async Task RefreshSessionTokensForAccountAsync(
      Account account,
      bool forceRefresh = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      IEnumerable<VssToken> source = this.tokenStorage.RetrieveAll("SessionToken");
      if (source == null)
        return;
      IEnumerable<VssToken> vssTokens = source.Where<VssToken>((Func<VssToken, bool>) (token => string.Equals(token.Resource, account.UniqueId, StringComparison.OrdinalIgnoreCase)));
      List<SessionTokenDescriptor> tokenParameters = new List<SessionTokenDescriptor>();
      foreach (VssToken token in vssTokens)
      {
        token.RefreshTokenValue();
        if (!string.IsNullOrEmpty(SessionTokenPair.GetTokenPair(token.TokenValue).GetToken(SessionTokenType.Compact)))
          tokenParameters.Add(SessionTokenDescriptor.FromKey(token, SessionTokenType.Compact));
        else
          tokenParameters.Add(SessionTokenDescriptor.FromKey(token));
      }
      if (tokenParameters.Count <= 0)
        return;
      IDictionary<SessionTokenDescriptor, string> accountImplAsync = await this.GetSessionTokenFromAccountImplAsync(account, (IEnumerable<SessionTokenDescriptor>) tokenParameters, forceRefresh, cancellationToken);
    }

    internal async Task<IDictionary<SessionTokenDescriptor, string>> GetSessionTokenFromAccountImplAsync(
      Account account,
      IEnumerable<SessionTokenDescriptor> tokenParameters,
      bool forceRefresh,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      if (!object.Equals((object) VSAccountProvider.AccountProviderIdentifier, (object) account.ProviderId))
      {
        DiagnosticsLogging.LogInformationEvent(this.logger, "SessionTokenRequiredRefresh", string.Format("Only VSAccountProvider accounts can get session tokens. The expected provider identifier was '{0}' but the account '{1}' had '{2}'", (object) VSAccountProvider.AccountProviderIdentifier, (object) account.UniqueId, (object) account.ProviderId));
        return (IDictionary<SessionTokenDescriptor, string>) null;
      }
      IDictionary<SessionTokenDescriptor, string> tokensToReturn = (IDictionary<SessionTokenDescriptor, string>) new Dictionary<SessionTokenDescriptor, string>();
      HashSet<SessionTokenDescriptor> tokensNeedingRefresh = new HashSet<SessionTokenDescriptor>();
      this.GetScopesFromTokenStorage(account, tokenParameters, forceRefresh, tokensToReturn, tokensNeedingRefresh);
      cancellationToken.ThrowIfCancellationRequested();
      if (tokensNeedingRefresh.Count > 0)
        await this.GetSessionTokenFromServer(account, tokensToReturn, tokensNeedingRefresh, cancellationToken);
      return tokensToReturn;
    }

    private async Task RetreiveAndStoreSessionTokensFromVSO(
      Account account,
      IDictionary<SessionTokenDescriptor, string> tokensToReturn,
      HashSet<SessionTokenDescriptor> tokensNeedingRefresh,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TenantInformation homeTenantInfo = (AccountManager.DefaultInstance.GetAccountProvider(VSAccountProvider.AccountProviderIdentifier) as VSAccountProvider).GetHomeTenantInfo((AccountKey) account);
      string tenantId;
      string uniqueId;
      IAccountCacheItem result;
      IAccountCache accountCache;
      if (homeTenantInfo == null)
      {
        tenantId = (string) null;
        uniqueId = (string) null;
        result = (IAccountCacheItem) null;
        accountCache = (IAccountCache) null;
      }
      else
      {
        tenantId = homeTenantInfo.TenantId;
        uniqueId = ((IEnumerable<string>) homeTenantInfo.UniqueIds).FirstOrDefault<string>((Func<string, bool>) (id => !string.IsNullOrWhiteSpace(id)));
        if (string.IsNullOrWhiteSpace(uniqueId))
        {
          tenantId = (string) null;
          uniqueId = (string) null;
          result = (IAccountCacheItem) null;
          accountCache = (IAccountCache) null;
        }
        else
        {
          result = (IAccountCacheItem) null;
          accountCache = AccountManager.DefaultInstance.GetCache<IAccountCache>();
          for (int i = 1; i <= 3; ++i)
          {
            try
            {
              result = await accountCache.AcquireTokenSilentAsync(VssAadSettings.DefaultScopes, uniqueId, tenantId);
              break;
            }
            catch (Exception ex)
            {
            }
            if (i != 3)
              await Task.Delay(TimeSpan.FromSeconds(1.0));
          }
          Uri vsoEndpoint = AccountManager.VsoEndpoint;
          if (result == null)
          {
            tenantId = (string) null;
            uniqueId = (string) null;
            result = (IAccountCacheItem) null;
            accountCache = (IAccountCache) null;
          }
          else
          {
            VssCredentials credentials = (VssCredentials) (FederatedCredential) new VssAadCredential(new VssAadToken(result.InnerResult.TokenType, result.AccessToken));
            TokenHttpClient tokenHttpClient = (TokenHttpClient) null;
            try
            {
              tokenHttpClient = await new VssConnection(vsoEndpoint, credentials).GetClientAsync<TokenHttpClient>(cancellationToken);
            }
            catch (Exception ex)
            {
              DiagnosticsLogging.LogInformationEvent(this.logger, nameof (RetreiveAndStoreSessionTokensFromVSO), string.Format("The account '{0}' was not refreshed", (object) account.UniqueId), exception: ex);
            }
            if (tokenHttpClient != null)
            {
              foreach (SessionTokenDescriptor tokenParameter in tokensNeedingRefresh)
              {
                string scopeToUse = tokenParameter.Scope;
                if (string.IsNullOrEmpty(scopeToUse))
                  scopeToUse = (string) null;
                try
                {
                  SessionTokenType tokenType = tokenParameter.TokenType;
                  DateTime? nullable = new DateTime?();
                  if (!string.IsNullOrEmpty(tokenParameter.LifeTime))
                    nullable = new DateTime?(DateTime.Now.AddHours(double.Parse(tokenParameter.LifeTime)));
                  TokenHttpClient tokenHttpClient1 = tokenHttpClient;
                  Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionToken sessionToken = new Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionToken();
                  sessionToken.Scope = scopeToUse;
                  sessionToken.TargetAccounts = tokenParameter.TargetAccounts;
                  sessionToken.ValidTo = nullable.GetValueOrDefault();
                  SessionTokenType? tokenType1 = new SessionTokenType?(tokenType);
                  CancellationToken cancellationToken1 = cancellationToken;
                  bool? isPublic = new bool?();
                  bool? isRequestedByTfsPatWebUI = new bool?();
                  CancellationToken cancellationToken2 = cancellationToken1;
                  Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionToken sessionTokenAsync = await tokenHttpClient1.CreateSessionTokenAsync(sessionToken, tokenType1, isPublic, isRequestedByTfsPatWebUI, cancellationToken: cancellationToken2);
                  if (sessionTokenAsync != null)
                  {
                    if (!string.IsNullOrEmpty(sessionTokenAsync.Token))
                    {
                      switch (tokenType)
                      {
                        case SessionTokenType.SelfDescribing:
                          VssTokenKey tokenKey = new VssTokenKey("SessionToken", account.UniqueId, tokenParameter.ToKey(), "Basic");
                          if (sessionTokenAsync.ValidTo <= DateTime.UtcNow)
                          {
                            DiagnosticsLogging.LogErrorEvent(this.logger, nameof (RetreiveAndStoreSessionTokensFromVSO), string.Format("The scope '{0}' for account '{1}' was refreshed but the token from the server was expired.", (object) scopeToUse, (object) account.UniqueId), (IDictionary<string, object>) SessionTokenStorage.GetMetadataFromSessionToken(sessionTokenAsync));
                            this.tokenStorage.Remove(tokenKey);
                            break;
                          }
                          SessionTokenPair sessionTokenPair = tokenType != SessionTokenType.Compact ? new SessionTokenPair(sessionTokenAsync.Token, (string) null) : new SessionTokenPair(sessionTokenAsync.AlternateToken, sessionTokenAsync.Token);
                          string tokenValue = sessionTokenPair.Serialize();
                          VssToken vssToken = this.tokenStorage.Add(tokenKey, tokenValue);
                          DateTime refreshSessionToken = SessionTokenStorage.CalculateDateToRefreshSessionToken(sessionTokenAsync.ValidFrom, sessionTokenAsync.ValidTo);
                          DateTime dateTime = DateTime.UtcNow;
                          vssToken.SetProperty("RefreshedOn", Convert.ToString(dateTime.ToFileTimeUtc(), (IFormatProvider) CultureInfo.InvariantCulture));
                          dateTime = sessionTokenAsync.ValidTo;
                          vssToken.SetProperty("ValidTo", Convert.ToString(dateTime.ToFileTimeUtc(), (IFormatProvider) CultureInfo.InvariantCulture));
                          dateTime = sessionTokenAsync.ValidFrom;
                          vssToken.SetProperty("ValidFrom", Convert.ToString(dateTime.ToFileTimeUtc(), (IFormatProvider) CultureInfo.InvariantCulture));
                          vssToken.SetProperty("DateToRefreshSessionToken", Convert.ToString(refreshSessionToken.ToFileTimeUtc(), (IFormatProvider) CultureInfo.InvariantCulture));
                          vssToken.SetProperty("UserId", VSAccountProvider.FormatGuid(sessionTokenAsync.UserId));
                          tokensToReturn[tokenParameter] = sessionTokenPair.GetToken(tokenType);
                          DiagnosticsLogging.LogInformationEvent(this.logger, nameof (RetreiveAndStoreSessionTokensFromVSO), string.Format("The scope '{0}' for account '{1}' was refreshed", (object) scopeToUse, (object) account.UniqueId), (IDictionary<string, object>) SessionTokenStorage.GetMetadataFromSessionToken(sessionTokenAsync));
                          break;
                        case SessionTokenType.Compact:
                          if (string.IsNullOrEmpty(sessionTokenAsync.AlternateToken))
                            break;
                          goto case SessionTokenType.SelfDescribing;
                      }
                    }
                  }
                }
                catch (Exception ex)
                {
                  DiagnosticsLogging.LogInformationEvent(this.logger, nameof (RetreiveAndStoreSessionTokensFromVSO), string.Format("The scope '{0}' for account '{1}' was not refreshed", (object) scopeToUse, (object) account.UniqueId), exception: ex);
                }
                scopeToUse = (string) null;
              }
            }
            tokenHttpClient = (TokenHttpClient) null;
            tenantId = (string) null;
            uniqueId = (string) null;
            result = (IAccountCacheItem) null;
            accountCache = (IAccountCache) null;
          }
        }
      }
    }

    private static Dictionary<string, object> GetMetadataFromSessionToken(Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionToken token) => new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "ClientId",
        (object) token.ClientId
      },
      {
        "Scope",
        (object) token.Scope
      },
      {
        "UserId",
        (object) token.UserId
      },
      {
        "ValidTo",
        (object) token.ValidTo
      },
      {
        "ValidFrom",
        (object) token.ValidFrom
      },
      {
        "AccessId",
        (object) token.AccessId
      }
    };

    internal void GetScopesFromTokenStorage(
      Account account,
      IEnumerable<SessionTokenDescriptor> tokenParameters,
      bool forceRefresh,
      IDictionary<SessionTokenDescriptor, string> tokensToReturn,
      HashSet<SessionTokenDescriptor> tokensNeedingRefresh)
    {
      ArgumentUtility.CheckForNull<Account>(account, nameof (account));
      ArgumentUtility.CheckForNull<IEnumerable<SessionTokenDescriptor>>(tokenParameters, nameof (tokenParameters));
      ArgumentUtility.CheckForNull<IDictionary<SessionTokenDescriptor, string>>(tokensToReturn, nameof (tokensToReturn));
      ArgumentUtility.CheckForNull<HashSet<SessionTokenDescriptor>>(tokensNeedingRefresh, nameof (tokensNeedingRefresh));
      foreach (SessionTokenDescriptor tokenParameter in tokenParameters)
      {
        if (forceRefresh)
        {
          tokensNeedingRefresh.Add(tokenParameter);
        }
        else
        {
          bool needsRefresh;
          string tokenValue;
          if (this.IsValidToken(this.tokenStorage.Retrieve(new VssTokenKey("SessionToken", account.UniqueId, tokenParameter.ToKey(), "Basic")), tokenParameter, out needsRefresh, out tokenValue))
          {
            tokensToReturn.Add(tokenParameter, tokenValue);
            if (needsRefresh)
              tokensNeedingRefresh.Add(tokenParameter);
          }
          else
            tokensNeedingRefresh.Add(tokenParameter);
        }
      }
    }

    private bool IsValidToken(
      VssToken token,
      SessionTokenDescriptor tokenParameter,
      out bool needsRefresh,
      out string tokenValue)
    {
      needsRefresh = false;
      tokenValue = string.Empty;
      if (token != null)
      {
        bool expired = false;
        if (this.DoesSessionTokenRequireRefresh(DateTime.UtcNow, token, tokenParameter.LifeTime, out expired))
          needsRefresh = true;
        if (!expired)
        {
          token.RefreshTokenValue();
          SessionTokenPair tokenPair = SessionTokenPair.GetTokenPair(token.TokenValue);
          tokenValue = tokenPair.GetToken(tokenParameter.TokenType);
          if (!string.IsNullOrEmpty(tokenValue))
            return true;
          needsRefresh = true;
        }
      }
      else
        needsRefresh = true;
      return false;
    }

    internal static DateTime CalculateDateToRefreshSessionToken(
      DateTime validFrom,
      DateTime validTo)
    {
      int num = Math.Max(1, (int) Math.Ceiling((double) validTo.Subtract(validFrom).Days * 0.2));
      DateTime dateTime = validFrom.AddDays((double) num);
      return dateTime >= validTo ? validTo : dateTime;
    }

    internal delegate Task GetSessionTokenFromVSO(
      Account account,
      IDictionary<SessionTokenDescriptor, string> scopesAndTokenValues,
      HashSet<SessionTokenDescriptor> tokensNeedingRefresh,
      CancellationToken cancellationToken);
  }
}
