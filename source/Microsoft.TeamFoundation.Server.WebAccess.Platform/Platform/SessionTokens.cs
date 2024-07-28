// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.SessionTokens
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  internal class SessionTokens
  {
    public Dictionary<Guid, UserTokens> Users { get; set; }

    public WebSessionTokenValue GetNamedToken(Guid userId, string tokenName)
    {
      UserTokens userTokens = (UserTokens) null;
      if (this.Users == null || !this.Users.TryGetValue(userId, out userTokens) || userTokens == null)
        return (WebSessionTokenValue) null;
      WebSessionTokenValue sessionTokenValue;
      return userTokens.NamedTokens == null || !userTokens.NamedTokens.TryGetValue(tokenName, out sessionTokenValue) || sessionTokenValue == null ? (WebSessionTokenValue) null : sessionTokenValue;
    }

    public WebSessionTokenValue GetToken(
      Guid userId,
      Guid appId,
      Guid accountId,
      DelegatedAppTokenType tokenType)
    {
      UserTokens userTokens = (UserTokens) null;
      if (this.Users == null || !this.Users.TryGetValue(userId, out userTokens) || userTokens == null)
        return (WebSessionTokenValue) null;
      AppTokens appTokens = (AppTokens) null;
      if (userTokens.Apps == null || !userTokens.Apps.TryGetValue(appId, out appTokens) || appTokens == null)
        return (WebSessionTokenValue) null;
      AccountTokens accountTokens = (AccountTokens) null;
      if (appTokens.Accounts == null || !appTokens.Accounts.TryGetValue(accountId, out accountTokens) || accountTokens == null)
        return (WebSessionTokenValue) null;
      WebSessionTokenValue token = (WebSessionTokenValue) null;
      if (accountTokens.Types == null || !accountTokens.Types.TryGetValue(tokenType, out token) || token == null)
        return (WebSessionTokenValue) null;
      if (!(token.ValidTo < DateTime.UtcNow))
        return token;
      userTokens.Apps.Remove(appId);
      return (WebSessionTokenValue) null;
    }

    public void SetNamedToken(Guid userId, string tokenName, WebSessionTokenValue cookieValue)
    {
      UserTokens userTokens = (UserTokens) null;
      if (this.Users == null || !this.Users.TryGetValue(userId, out userTokens) || userTokens == null)
      {
        if (this.Users == null)
          this.Users = new Dictionary<Guid, UserTokens>();
        userTokens = new UserTokens();
        this.Users[userId] = userTokens;
      }
      if (userTokens.NamedTokens == null)
        userTokens.NamedTokens = new Dictionary<string, WebSessionTokenValue>();
      userTokens.NamedTokens[tokenName] = cookieValue;
    }

    public void SetToken(
      Guid userId,
      Guid appId,
      Guid accountId,
      DelegatedAppTokenType tokenType,
      WebSessionTokenValue cookieValue)
    {
      UserTokens userTokens = (UserTokens) null;
      if (this.Users == null || !this.Users.TryGetValue(userId, out userTokens) || userTokens == null)
      {
        if (this.Users == null)
          this.Users = new Dictionary<Guid, UserTokens>();
        userTokens = new UserTokens();
        this.Users[userId] = userTokens;
      }
      AppTokens appTokens = (AppTokens) null;
      if (userTokens.Apps == null || !userTokens.Apps.TryGetValue(appId, out appTokens) || appTokens == null)
      {
        if (userTokens.Apps == null)
          userTokens.Apps = new Dictionary<Guid, AppTokens>();
        appTokens = new AppTokens();
        userTokens.Apps[appId] = appTokens;
      }
      AccountTokens accountTokens = (AccountTokens) null;
      if (appTokens.Accounts == null || !appTokens.Accounts.TryGetValue(accountId, out accountTokens) || accountTokens == null)
      {
        if (appTokens.Accounts == null)
          appTokens.Accounts = new Dictionary<Guid, AccountTokens>();
        accountTokens = new AccountTokens();
        appTokens.Accounts[accountId] = accountTokens;
      }
      if (accountTokens.Types == null)
        accountTokens.Types = new Dictionary<DelegatedAppTokenType, WebSessionTokenValue>();
      accountTokens.Types[tokenType] = cookieValue;
    }
  }
}
