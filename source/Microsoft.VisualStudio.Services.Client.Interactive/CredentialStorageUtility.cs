// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.CredentialStorageUtility
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.TokenStorage;
using Microsoft.VisualStudio.Services.OAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;

namespace Microsoft.VisualStudio.Services.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CredentialStorageUtility
  {
    public static VssClientCredentials GetCredentials(VssToken token)
    {
      VssCredentialsType credentialsType = CredentialStorageUtility.ToCredentialsType(token.Type);
      IssuedToken tokenFromString = CredentialStorageUtility.GetTokenFromString(credentialsType, token.TokenValue);
      WindowsCredential windowsCredential = (WindowsCredential) null;
      FederatedCredential federatedCredential = (FederatedCredential) null;
      switch (credentialsType)
      {
        case VssCredentialsType.Windows:
          windowsCredential = new WindowsCredential((WindowsToken) tokenFromString);
          break;
        case VssCredentialsType.Federated:
          federatedCredential = (FederatedCredential) new VssFederatedCredential(initialToken: (VssFederatedToken) tokenFromString);
          break;
        case VssCredentialsType.Basic:
          federatedCredential = (FederatedCredential) new VssBasicCredential((VssBasicToken) tokenFromString);
          break;
        case VssCredentialsType.ServiceIdentity:
          federatedCredential = (FederatedCredential) new VssServiceIdentityCredential((VssServiceIdentityToken) tokenFromString);
          break;
        case VssCredentialsType.OAuth:
          federatedCredential = (FederatedCredential) new VssOAuthAccessTokenCredential((VssOAuthAccessToken) tokenFromString);
          break;
        default:
          throw new NotSupportedException();
      }
      return new VssClientCredentials(windowsCredential ?? new WindowsCredential(), federatedCredential ?? (FederatedCredential) new VssFederatedCredential());
    }

    public static IssuedToken GetTokenFromString(
      VssCredentialsType credentialsType,
      string tokenValue)
    {
      switch (credentialsType)
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
          return (IssuedToken) new VssFederatedToken(cookies);
        case VssCredentialsType.Basic:
          Dictionary<string, string> dictionaryFromString3 = CredentialStorageUtility.GetDictionaryFromString(tokenValue);
          string domain2;
          dictionaryFromString3.TryGetValue("Domain", out domain2);
          string userName2;
          dictionaryFromString3.TryGetValue("UserName", out userName2);
          string password2;
          dictionaryFromString3.TryGetValue("Password", out password2);
          return !string.IsNullOrEmpty(domain2) ? (IssuedToken) new VssBasicToken((ICredentials) new NetworkCredential(userName2, password2, domain2)) : (IssuedToken) new VssBasicToken((ICredentials) new NetworkCredential(userName2, password2));
        case VssCredentialsType.ServiceIdentity:
          return (IssuedToken) new VssServiceIdentityToken(tokenValue);
        case VssCredentialsType.OAuth:
          return (IssuedToken) new VssOAuthAccessToken(tokenValue);
        default:
          throw new NotSupportedException();
      }
    }

    public static string GetTokenAsString(IssuedToken token)
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
        return CredentialStorageUtility.GetStringFromDictionary(((VssFederatedToken) token).CookieCollection.Cast<Cookie>().ToDictionary<Cookie, string, string>((Func<Cookie, string>) (cookie => cookie.Name), (Func<Cookie, string>) (cookie => cookie.Value)));
      if (token.CredentialType == VssCredentialsType.Basic)
      {
        if (!(((VssBasicToken) token).Credentials is NetworkCredential credentials) || string.IsNullOrEmpty(credentials.Password))
          return (string) null;
        Dictionary<string, string> keyValueCollection = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
        if (!string.IsNullOrEmpty(credentials.Domain))
          keyValueCollection["Domain"] = credentials.Domain;
        keyValueCollection["UserName"] = credentials.UserName;
        keyValueCollection["Password"] = credentials.Password;
        return CredentialStorageUtility.GetStringFromDictionary(keyValueCollection);
      }
      if (token.CredentialType == VssCredentialsType.ServiceIdentity)
        return ((VssServiceIdentityToken) token).Token;
      return token.CredentialType == VssCredentialsType.OAuth ? ((VssOAuthAccessToken) token).Value : throw new NotSupportedException();
    }

    public static Dictionary<string, string> GetDictionaryFromString(string tokenValue)
    {
      Dictionary<string, string> dictionaryFromString = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
      string str1 = tokenValue;
      char[] chArray = new char[1]{ ';' };
      foreach (string str2 in str1.Split(chArray))
      {
        char[] separator = new char[1]{ '=' };
        string[] strArray = str2.Split(separator, 2);
        if (strArray.Length == 2)
          dictionaryFromString.Add(strArray[0], strArray[1]);
      }
      return dictionaryFromString;
    }

    public static string GetStringFromDictionary(Dictionary<string, string> keyValueCollection)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      foreach (KeyValuePair<string, string> keyValue in keyValueCollection)
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder();
        else
          stringBuilder.Append(';');
        stringBuilder.Append(keyValue.Key);
        stringBuilder.Append('=');
        stringBuilder.Append(keyValue.Value);
      }
      return stringBuilder?.ToString();
    }

    public static bool IsUserDataMatched(VssToken token, Guid userId)
    {
      Guid result;
      Guid.TryParse(token.GetProperty("UserId") ?? string.Empty, out result);
      return result == Guid.Empty || result == userId;
    }

    private static VssCredentialsType ToCredentialsType(string type)
    {
      if (string.Equals(type, "Federated", StringComparison.OrdinalIgnoreCase))
        return VssCredentialsType.Federated;
      if (string.Equals(type, "ServiceIdentity", StringComparison.OrdinalIgnoreCase))
        return VssCredentialsType.ServiceIdentity;
      if (string.Equals(type, "Basic", StringComparison.OrdinalIgnoreCase))
        return VssCredentialsType.Basic;
      if (string.Equals(type, "OAuth", StringComparison.OrdinalIgnoreCase))
        return VssCredentialsType.OAuth;
      if (string.Equals(type, "S2S", StringComparison.OrdinalIgnoreCase))
        return VssCredentialsType.S2S;
      if (string.Equals(type, "Windows", StringComparison.OrdinalIgnoreCase))
        return VssCredentialsType.Windows;
      throw new NotSupportedException();
    }
  }
}
