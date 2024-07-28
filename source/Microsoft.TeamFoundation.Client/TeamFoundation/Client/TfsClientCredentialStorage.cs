// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsClientCredentialStorage
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.TokenStorage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("This class is deprecated and will be removed in a future release. See VssClientCredentialStorage instead.", false)]
  public class TfsClientCredentialStorage
  {
    private VssTokenStorage m_tokenStorage;
    private Dictionary<string, VssTokenKey> m_tokenKeyMap = new Dictionary<string, VssTokenKey>();

    public TfsClientCredentialStorage()
      : this("VisualStudio")
    {
    }

    internal TfsClientCredentialStorage(string tokenNamespace) => this.m_tokenStorage = VssTokenStorageFactory.GetTokenStorageNamespace(tokenNamespace);

    public IssuedToken RetrieveToken(Uri serverUrl, VssCredentialsType credentialType)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      try
      {
        VssToken vssToken = this.m_tokenStorage.Retrieve(this.BuildTokenKey(serverUrl, credentialType));
        IssuedToken issuedToken = (IssuedToken) null;
        if (vssToken != null && !string.IsNullOrWhiteSpace(vssToken.TokenValue))
        {
          issuedToken = TfsClientCredentialStorage.GetTokenFromString(credentialType, vssToken.TokenValue);
          issuedToken.FromStorage = true;
          Guid result;
          Guid.TryParse(vssToken.GetProperty("UserId") ?? string.Empty, out result);
          issuedToken.UserId = result;
          issuedToken.UserName = vssToken.GetProperty("UserName") ?? string.Empty;
        }
        if (issuedToken == null)
        {
          VssToken token = TfsClientCredentialStorage.RetrieveConnectedUserToken();
          if (token != null)
          {
            Guid result = Guid.Empty;
            if (vssToken != null)
              Guid.TryParse(vssToken.GetProperty("UserId") ?? string.Empty, out result);
            if (token.Type.Equals(credentialType.ToString(), StringComparison.OrdinalIgnoreCase) && (result == Guid.Empty || CredentialStorageUtility.IsUserDataMatched(token, result)))
            {
              issuedToken = TfsClientCredentialStorage.GetTokenFromString(credentialType, token.TokenValue);
              issuedToken.FromStorage = false;
            }
          }
        }
        return issuedToken;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        if (!(ex is NullReferenceException))
        {
          ArgumentException argumentException = ex as ArgumentException;
        }
        return (IssuedToken) null;
      }
    }

    public void StoreToken(Uri serverUrl, IssuedToken token, bool matchUserInformation)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      ArgumentUtility.CheckForNull<IssuedToken>(token, nameof (token));
      try
      {
        VssTokenKey tokenKey = this.BuildTokenKey(serverUrl, token.CredentialType);
        if (matchUserInformation && token.UserId != Guid.Empty)
        {
          VssToken token1 = (VssToken) null;
          try
          {
            token1 = this.m_tokenStorage.Retrieve(tokenKey);
          }
          catch (Exception ex)
          {
            TeamFoundationTrace.TraceException(ex);
          }
          if (token1 != null && !CredentialStorageUtility.IsUserDataMatched(token1, token.UserId))
            throw new TeamFoundationInvalidAuthenticationException(Microsoft.TeamFoundation.Client.Internal.ClientResources.CannotAuthenticateAsAnotherUser((object) (token1.GetProperty("UserName") ?? string.Empty), (object) token.UserName), TeamFoundationAuthenticationError.UserMismatched);
        }
        string tokenAsString = TfsClientCredentialStorage.GetTokenAsString(token);
        if (string.IsNullOrEmpty(tokenAsString))
          return;
        VssToken vssToken = this.m_tokenStorage.Add(tokenKey, tokenAsString);
        if (token.UserId != Guid.Empty)
          vssToken.SetProperty("UserId", token.UserId.ToString("D"));
        if (string.IsNullOrWhiteSpace(token.UserName))
          return;
        vssToken.SetProperty("UserName", token.UserName);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        if (!(ex is NullReferenceException))
        {
          ArgumentException argumentException = ex as ArgumentException;
        }
        if (!(ex is TeamFoundationInvalidAuthenticationException))
          return;
        throw;
      }
    }

    public void RemoveToken(Uri serverUrl)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      try
      {
        string leftPart = serverUrl.GetLeftPart(UriPartial.Authority);
        foreach (VssToken tokenKey in this.m_tokenStorage.RetrieveAll("VssApp"))
        {
          if (tokenKey.Resource.Equals(leftPart, StringComparison.OrdinalIgnoreCase) && tokenKey.UserName.Equals("VssUser", StringComparison.OrdinalIgnoreCase))
          {
            this.m_tokenStorage.Remove((VssTokenKey) tokenKey);
            break;
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        if (ex is NullReferenceException)
          return;
        ArgumentException argumentException = ex as ArgumentException;
      }
    }

    public bool RemoveTokenValue(Uri serverUrl, IssuedToken token)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      ArgumentUtility.CheckForNull<IssuedToken>(token, nameof (token));
      bool flag = false;
      try
      {
        VssToken vssToken = this.m_tokenStorage.Retrieve(this.BuildTokenKey(serverUrl, token.CredentialType));
        if (vssToken != null)
        {
          if (!string.IsNullOrEmpty(vssToken.TokenValue))
          {
            if (string.Equals(TfsClientCredentialStorage.GetTokenAsString(token), vssToken.TokenValue, StringComparison.Ordinal))
              flag = vssToken.RemoveTokenValue();
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        if (!(ex is NullReferenceException))
        {
          ArgumentException argumentException = ex as ArgumentException;
        }
      }
      return flag;
    }

    public void RemoveTokenValuesByUser(Guid userId)
    {
      try
      {
        if (!(userId != Guid.Empty))
          return;
        foreach (VssToken token in this.m_tokenStorage.RetrieveAll("VssApp"))
        {
          if (!string.IsNullOrEmpty(token.TokenValue) && CredentialStorageUtility.IsUserDataMatched(token, userId))
            token.RemoveTokenValue();
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        if (ex is NullReferenceException)
          return;
        ArgumentException argumentException = ex as ArgumentException;
      }
    }

    public string GetTokenProperty(Uri serverUrl, string propertyName) => this.GetTokenProperty(new Uri[1]
    {
      serverUrl
    }, propertyName)[0];

    public string[] GetTokenProperty(Uri[] serverUrls, string propertyName)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) serverUrls, nameof (serverUrls));
      ArgumentUtility.CheckForNull<string>(propertyName, nameof (propertyName));
      string[] tokenProperty = new string[serverUrls.Length];
      try
      {
        IEnumerable<VssToken> vssTokens = this.m_tokenStorage.RetrieveAll("VssApp");
        for (int index = 0; index < serverUrls.Length; ++index)
        {
          string leftPart = serverUrls[index].GetLeftPart(UriPartial.Authority);
          foreach (VssToken vssToken in vssTokens)
          {
            if (vssToken.Resource.Equals(leftPart, StringComparison.OrdinalIgnoreCase) && vssToken.UserName.Equals("VssUser", StringComparison.OrdinalIgnoreCase))
            {
              tokenProperty[index] = vssToken.GetProperty(propertyName);
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        if (!(ex is NullReferenceException))
        {
          ArgumentException argumentException = ex as ArgumentException;
        }
      }
      return tokenProperty;
    }

    private VssTokenKey BuildTokenKey(Uri serverUrl, VssCredentialsType credentialType)
    {
      string leftPart = serverUrl.GetLeftPart(UriPartial.Authority);
      VssTokenKey vssTokenKey;
      lock (this.m_tokenKeyMap)
      {
        if (!this.m_tokenKeyMap.TryGetValue(leftPart, out vssTokenKey))
        {
          vssTokenKey = new VssTokenKey("VssApp", leftPart, "VssUser", credentialType.ToString());
          this.m_tokenKeyMap[leftPart] = vssTokenKey;
        }
      }
      return vssTokenKey;
    }

    public static VssToken RetrieveConnectedUserToken()
    {
      Uri result = (Uri) null;
      if (!string.IsNullOrWhiteSpace(UIHost.ConnectedUserContext.ServerUri))
        Uri.TryCreate(UIHost.ConnectedUserContext.ServerUri, UriKind.Absolute, out result);
      try
      {
        IEnumerable<VssToken> source = VssTokenStorageFactory.GetTokenStorageNamespace(UIHost.ConnectedUserContext.Namespace).RetrieveAll(UIHost.ConnectedUserContext.TokenKind);
        if (result != (Uri) null)
        {
          string leftPart = result.GetLeftPart(UriPartial.Authority);
          foreach (VssToken vssToken in source)
          {
            if (vssToken.Resource.Equals(leftPart, StringComparison.OrdinalIgnoreCase))
              return vssToken;
          }
        }
        return source.FirstOrDefault<VssToken>();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        if (!(ex is NullReferenceException))
        {
          ArgumentException argumentException = ex as ArgumentException;
        }
        return (VssToken) null;
      }
    }

    private static IssuedToken GetTokenFromString(
      VssCredentialsType credentialType,
      string tokenValue)
    {
      switch (credentialType)
      {
        case VssCredentialsType.Windows:
          Dictionary<string, string> dictionaryFromString1 = CredentialStorageUtility.GetDictionaryFromString(tokenValue);
          string domain1;
          dictionaryFromString1.TryGetValue("Domain", out domain1);
          string userName1;
          dictionaryFromString1.TryGetValue("UserName", out userName1);
          string password1;
          dictionaryFromString1.TryGetValue("Password", out password1);
          return !string.IsNullOrEmpty(domain1) ? (IssuedToken) new WindowsToken((ICredentials) new NetworkCredential(userName1, password1, domain1)) : (IssuedToken) new WindowsToken((ICredentials) new NetworkCredential(userName1, password1));
        case VssCredentialsType.Federated:
          Dictionary<string, string> dictionaryFromString2 = CredentialStorageUtility.GetDictionaryFromString(tokenValue);
          CookieCollection cookies = new CookieCollection();
          foreach (KeyValuePair<string, string> keyValuePair in dictionaryFromString2)
            cookies.Add(new Cookie(keyValuePair.Key, keyValuePair.Value));
          return (IssuedToken) new CookieToken(cookies);
        case VssCredentialsType.Basic:
          Dictionary<string, string> dictionaryFromString3 = CredentialStorageUtility.GetDictionaryFromString(tokenValue);
          string domain2;
          dictionaryFromString3.TryGetValue("Domain", out domain2);
          string userName2;
          dictionaryFromString3.TryGetValue("UserName", out userName2);
          string password2;
          dictionaryFromString3.TryGetValue("Password", out password2);
          return !string.IsNullOrEmpty(domain2) ? (IssuedToken) new BasicAuthToken((ICredentials) new NetworkCredential(userName2, password2, domain2)) : (IssuedToken) new BasicAuthToken((ICredentials) new NetworkCredential(userName2, password2));
        case VssCredentialsType.ServiceIdentity:
          return (IssuedToken) new SimpleWebToken(tokenValue);
        case VssCredentialsType.OAuth:
          return (IssuedToken) new OAuthTokenContainer(new OAuthToken(tokenValue, OAuthTokenType.AccessToken), (OAuthToken) null);
        default:
          throw new NotSupportedException();
      }
    }

    private static string GetTokenAsString(IssuedToken token)
    {
      if (token.CredentialType == VssCredentialsType.Windows)
      {
        if (!(((WindowsToken) token).Credentials is NetworkCredential credentials) || string.IsNullOrEmpty(credentials.Password))
          return (string) null;
        Dictionary<string, string> keyValueCollection = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        if (!string.IsNullOrEmpty(credentials.Domain))
          keyValueCollection["Domain"] = credentials.Domain;
        keyValueCollection["UserName"] = credentials.UserName;
        keyValueCollection["Password"] = credentials.Password;
        return CredentialStorageUtility.GetStringFromDictionary(keyValueCollection);
      }
      if (token.CredentialType == VssCredentialsType.Federated)
        return CredentialStorageUtility.GetStringFromDictionary(((CookieToken) token).CookieCollection.Cast<Cookie>().ToDictionary<Cookie, string, string>((Func<Cookie, string>) (cookie => cookie.Name), (Func<Cookie, string>) (cookie => cookie.Value)));
      if (token.CredentialType == VssCredentialsType.Basic)
      {
        if (!(((BasicAuthToken) token).Credentials is NetworkCredential credentials) || string.IsNullOrEmpty(credentials.Password))
          return (string) null;
        Dictionary<string, string> keyValueCollection = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        if (!string.IsNullOrEmpty(credentials.Domain))
          keyValueCollection["Domain"] = credentials.Domain;
        keyValueCollection["UserName"] = credentials.UserName;
        keyValueCollection["Password"] = credentials.Password;
        return CredentialStorageUtility.GetStringFromDictionary(keyValueCollection);
      }
      if (token.CredentialType == VssCredentialsType.ServiceIdentity)
        return ((SimpleWebToken) token).Token;
      if (token.CredentialType == VssCredentialsType.OAuth)
        return ((OAuthTokenContainer) token).AccessToken.Token;
      throw new NotSupportedException();
    }
  }
}
