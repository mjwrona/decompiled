// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.VssClientCredentialStorage
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.TokenStorage;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client
{
  public class VssClientCredentialStorage : IVssCredentialStorage
  {
    private string m_tokenKind;
    private VssTokenStorage m_tokenStorage;
    private Dictionary<string, VssTokenKey> m_tokenKeyMap = new Dictionary<string, VssTokenKey>();

    public VssClientCredentialStorage(string storageKind = "VssApp", string storageNamespace = "VisualStudio")
      : this(storageKind, VssTokenStorageFactory.GetTokenStorageNamespace(storageNamespace))
    {
    }

    public VssClientCredentialStorage(string storageKind, VssTokenStorage tokenStorage)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(storageKind, nameof (storageKind));
      ArgumentUtility.CheckForNull<VssTokenStorage>(tokenStorage, nameof (tokenStorage));
      this.m_tokenKind = storageKind;
      this.m_tokenStorage = tokenStorage;
    }

    protected VssTokenStorage TokenStorage => this.m_tokenStorage;

    protected string TokenKind => this.m_tokenKind;

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
        if (!(ex is NullReferenceException))
        {
          ArgumentException argumentException = ex as ArgumentException;
        }
      }
      return tokenProperty;
    }

    public virtual IssuedToken RetrieveToken(Uri serverUrl, VssCredentialsType credentialsType)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      try
      {
        VssToken vssToken = this.TokenStorage.Retrieve(this.BuildTokenKey(serverUrl, credentialsType));
        IssuedToken issuedToken = (IssuedToken) null;
        if (vssToken != null && !string.IsNullOrWhiteSpace(vssToken.TokenValue))
        {
          issuedToken = CredentialStorageUtility.GetTokenFromString(credentialsType, vssToken.TokenValue);
          issuedToken.FromStorage = true;
          Guid result;
          Guid.TryParse(vssToken.GetProperty("UserId") ?? string.Empty, out result);
          issuedToken.UserId = result;
          issuedToken.UserName = vssToken.GetProperty("UserName") ?? string.Empty;
          IEnumerable<string> propertyNames = vssToken.GetPropertyNames();
          if (propertyNames != null)
          {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string str in propertyNames)
              dictionary[str] = vssToken.GetProperty(str);
            issuedToken.Properties = (IDictionary<string, string>) dictionary;
          }
        }
        return issuedToken;
      }
      catch (Exception ex)
      {
        if (!(ex is NullReferenceException))
        {
          ArgumentException argumentException = ex as ArgumentException;
        }
        return (IssuedToken) null;
      }
    }

    public virtual void StoreToken(Uri serverUrl, IssuedToken token)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      ArgumentUtility.CheckForNull<IssuedToken>(token, nameof (token));
      try
      {
        string tokenAsString = CredentialStorageUtility.GetTokenAsString(token);
        if (string.IsNullOrWhiteSpace(tokenAsString))
          return;
        VssTokenKey tokenKey = this.BuildTokenKey(serverUrl, token.CredentialType);
        if (token.UserId != Guid.Empty)
        {
          try
          {
            VssToken token1 = this.m_tokenStorage.Retrieve(tokenKey);
            if (token1 != null)
            {
              if (!CredentialStorageUtility.IsUserDataMatched(token1, token.UserId))
                this.m_tokenStorage.Remove(tokenKey);
            }
          }
          catch (Exception ex)
          {
          }
        }
        VssToken vssToken = this.TokenStorage.Add(tokenKey, tokenAsString);
        if (token.UserId != Guid.Empty)
          vssToken.SetProperty("UserId", token.UserId.ToString("D"));
        if (!string.IsNullOrWhiteSpace(token.UserName))
          vssToken.SetProperty("UserName", token.UserName);
        if (token.Properties == null || token.Properties.Count <= 0)
          return;
        foreach (string key in (IEnumerable<string>) token.Properties.Keys)
          vssToken.SetProperty(key, token.Properties[key]);
      }
      catch (Exception ex)
      {
        if (!(ex is NullReferenceException))
        {
          ArgumentException argumentException = ex as ArgumentException;
        }
        if (!(ex is VssAuthenticationException))
          return;
        throw;
      }
    }

    public void RemoveToken(Uri serverUrl)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      try
      {
        string str = this.ConvertUriForStorage(serverUrl);
        foreach (VssToken tokenKey in this.m_tokenStorage.RetrieveAll("VssApp"))
        {
          if (tokenKey.Resource.Equals(str, StringComparison.OrdinalIgnoreCase) && tokenKey.UserName.Equals("VssUser", StringComparison.OrdinalIgnoreCase))
          {
            this.m_tokenStorage.Remove((VssTokenKey) tokenKey);
            break;
          }
        }
      }
      catch (Exception ex)
      {
        if (ex is NullReferenceException)
          return;
        ArgumentException argumentException = ex as ArgumentException;
      }
    }

    public virtual void RemoveToken(Uri serverUrl, IssuedToken token)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      ArgumentUtility.CheckForNull<IssuedToken>(token, nameof (token));
      try
      {
        string tokenAsString = CredentialStorageUtility.GetTokenAsString(token);
        if (string.IsNullOrWhiteSpace(tokenAsString))
          return;
        VssTokenKey tokenKey = this.BuildTokenKey(serverUrl, token.CredentialType);
        VssToken vssToken = this.TokenStorage.Retrieve(tokenKey);
        if (vssToken == null || !string.IsNullOrWhiteSpace(vssToken.TokenValue) && !string.Equals(tokenAsString, vssToken.TokenValue, StringComparison.Ordinal))
          return;
        this.TokenStorage.Remove(tokenKey);
      }
      catch (Exception ex)
      {
        if (ex is NullReferenceException)
          return;
        ArgumentException argumentException = ex as ArgumentException;
      }
    }

    public virtual bool RemoveTokenValue(Uri serverUrl, IssuedToken token)
    {
      ArgumentUtility.CheckForNull<Uri>(serverUrl, nameof (serverUrl));
      ArgumentUtility.CheckForNull<IssuedToken>(token, nameof (token));
      bool flag = false;
      try
      {
        string tokenAsString = CredentialStorageUtility.GetTokenAsString(token);
        if (!string.IsNullOrWhiteSpace(tokenAsString))
        {
          VssToken vssToken = this.m_tokenStorage.Retrieve(this.BuildTokenKey(serverUrl, token.CredentialType));
          if (vssToken != null)
          {
            if (!string.IsNullOrEmpty(vssToken.TokenValue))
            {
              if (string.Equals(tokenAsString, vssToken.TokenValue, StringComparison.Ordinal))
                flag = vssToken.RemoveTokenValue();
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (!(ex is NullReferenceException))
        {
          ArgumentException argumentException = ex as ArgumentException;
        }
      }
      return flag;
    }

    public void RemoveTokenValues(Guid userId)
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
        if (ex is NullReferenceException)
          return;
        ArgumentException argumentException = ex as ArgumentException;
      }
    }

    protected virtual VssTokenKey BuildTokenKey(Uri serverUrl, VssCredentialsType credentialType)
    {
      string str = this.ConvertUriForStorage(serverUrl);
      VssTokenKey vssTokenKey;
      lock (this.m_tokenKeyMap)
      {
        if (!this.m_tokenKeyMap.TryGetValue(str, out vssTokenKey))
        {
          vssTokenKey = new VssTokenKey(this.m_tokenKind, str, "VssUser", credentialType.ToString());
          this.m_tokenKeyMap[str] = vssTokenKey;
        }
      }
      return vssTokenKey;
    }

    private string ConvertUriForStorage(Uri serverUri)
    {
      if (serverUri.Host.EndsWith(".visualstudio.com", StringComparison.OrdinalIgnoreCase))
        return serverUri.GetLeftPart(UriPartial.Authority);
      return serverUri.AbsoluteUri.TrimEnd('/');
    }
  }
}
