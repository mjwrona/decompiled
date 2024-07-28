// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TokenStorage.RegistryTokenStorageHelper
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common.TokenStorage
{
  internal static class RegistryTokenStorageHelper
  {
    private static readonly string KindValue = "Kind";
    private static readonly string UrlValue = "Url";
    private static readonly string UserNameValue = "UserName";
    private static readonly string TypeValue = "Type";
    private static readonly string TokenValue = "Token";
    private static readonly string PropertiesKeyName = "Properties";

    internal static void ThrowExceptionCouldNotCreateSubKey(string parentKey, string subkey) => throw new VssTokenStorageException("Could not write a new connection for subkey: " + parentKey + "\\" + subkey);

    internal static bool WriteNewToken(
      VssTokenKey tokenKey,
      string tokenValue,
      RegistryKey kindRegKey)
    {
      string subkey = Guid.NewGuid().ToString("N");
      using (RegistryKey subKey = kindRegKey.CreateSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree))
      {
        if (subKey == null)
          RegistryTokenStorageHelper.ThrowExceptionCouldNotCreateSubKey(kindRegKey.Name, subkey);
        RegistryTokenStorageHelper.WriteToken(tokenKey, tokenValue, subKey);
        return true;
      }
    }

    internal static void WriteToken(
      VssTokenKey tokenKey,
      string tokenValue,
      RegistryKey tokenRegKey)
    {
      RegistryTokenStorageHelper.WriteNonSecretTokenData(tokenRegKey, tokenKey);
      RegistryTokenStorageHelper.WriteSecretTokenValue(tokenRegKey, tokenValue);
    }

    internal static bool UpdateToken(
      VssToken token,
      string tokenValue,
      RegistryKey kindRegKey,
      bool updateValueOnly)
    {
      bool flag = false;
      VssTokenKey tokenKey = (VssTokenKey) token;
      string[] subKeyNames = kindRegKey.GetSubKeyNames();
      string tokenIdentity = RegistryTokenStorageHelper.CreateTokenIdentity(tokenKey);
      foreach (string name in subKeyNames)
      {
        using (RegistryKey registryKey = kindRegKey.OpenSubKey(name, true))
        {
          if (registryKey != null)
          {
            string b = Convert.ToString(registryKey.GetValue((string) null, (object) string.Empty), (IFormatProvider) CultureInfo.InvariantCulture);
            if (string.Equals(tokenIdentity, b, StringComparison.Ordinal))
            {
              if (updateValueOnly)
                RegistryTokenStorageHelper.WriteSecretTokenValue(registryKey, tokenValue);
              else
                RegistryTokenStorageHelper.WriteToken(tokenKey, tokenValue, registryKey);
              flag = true;
              break;
            }
          }
        }
      }
      if (!flag)
        flag = RegistryTokenStorageHelper.WriteNewToken(tokenKey, tokenValue, kindRegKey);
      return flag;
    }

    internal static void DeleteTokenStorage(string registryRootPath, string storageNamespace)
    {
      using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(registryRootPath, true))
        registryKey.DeleteSubKeyTree(storageNamespace);
    }

    internal static RegistryKey GetRootKey(string subkeyName)
    {
      RegistryKey rootKey = !string.IsNullOrEmpty(subkeyName) ? Registry.CurrentUser.CreateSubKey(subkeyName, RegistryKeyPermissionCheck.Default, RegistryOptions.None) : throw new ArgumentNullException(nameof (subkeyName));
      if (rootKey != null)
        return rootKey;
      RegistryTokenStorageHelper.ThrowExceptionCouldNotCreateSubKey(Registry.CurrentUser.Name, subkeyName);
      return rootKey;
    }

    internal static RegistryToken ReadTokenData(RegistryKey subkey, RegistryTokenStorage storage) => RegistryTokenStorageHelper.CreateTokenFromDataIfMatch(subkey, storage, (string) null);

    internal static string CreateTokenIdentity(VssTokenKey tokenKey) => RegistryTokenStorageHelper.CreateTokenIdentity(tokenKey.Kind, tokenKey.Resource, tokenKey.UserName, tokenKey.Type);

    internal static string CreateTokenIdentity(
      string kind,
      string resource,
      string userName,
      string type)
    {
      return kind.ToLowerInvariant() + ":" + resource + ":" + userName.ToLowerInvariant() + ":" + type.ToLowerInvariant();
    }

    internal static RegistryToken CreateTokenFromDataIfMatch(
      RegistryKey subkey,
      RegistryTokenStorage storage,
      string identityToMatch)
    {
      RegistryToken tokenFromDataIfMatch = (RegistryToken) null;
      string b = Convert.ToString(subkey.GetValue((string) null, (object) string.Empty), (IFormatProvider) CultureInfo.InvariantCulture);
      if (identityToMatch == null || string.Equals(identityToMatch, b, StringComparison.Ordinal))
      {
        string kind = Convert.ToString(subkey.GetValue(RegistryTokenStorageHelper.KindValue, (object) string.Empty), (IFormatProvider) CultureInfo.InvariantCulture);
        string resource = Convert.ToString(subkey.GetValue(RegistryTokenStorageHelper.UrlValue, (object) string.Empty), (IFormatProvider) CultureInfo.InvariantCulture);
        string userName = Convert.ToString(subkey.GetValue(RegistryTokenStorageHelper.UserNameValue, (object) string.Empty), (IFormatProvider) CultureInfo.InvariantCulture);
        string type = Convert.ToString(subkey.GetValue(RegistryTokenStorageHelper.TypeValue, (object) string.Empty), (IFormatProvider) CultureInfo.InvariantCulture);
        string tokenIdentity = RegistryTokenStorageHelper.CreateTokenIdentity(kind, resource, userName, type);
        if (identityToMatch == null || string.Equals(identityToMatch, tokenIdentity, StringComparison.Ordinal))
        {
          string token = RegistryTokenStorageHelper.UnprotectTokenValue(Convert.ToString(subkey.GetValue(RegistryTokenStorageHelper.TokenValue, (object) string.Empty), (IFormatProvider) CultureInfo.InvariantCulture));
          tokenFromDataIfMatch = new RegistryToken((VssTokenStorage) storage, kind, resource, userName, type, token);
        }
      }
      return tokenFromDataIfMatch;
    }

    internal static string ReadTokenPropertyValue(RegistryKey subkey, string name)
    {
      using (RegistryKey registryKey = subkey.OpenSubKey(RegistryTokenStorageHelper.PropertiesKeyName))
      {
        if (registryKey != null)
          return Convert.ToString(registryKey.GetValue(name, (object) null), (IFormatProvider) CultureInfo.InvariantCulture);
      }
      return (string) null;
    }

    internal static IEnumerable<string> ReadTokenPropertyNames(RegistryKey subkey)
    {
      using (RegistryKey registryKey = subkey.OpenSubKey(RegistryTokenStorageHelper.PropertiesKeyName))
      {
        if (registryKey != null)
          return (IEnumerable<string>) registryKey.GetValueNames();
      }
      return (IEnumerable<string>) null;
    }

    internal static void WriteNonSecretTokenData(RegistryKey subkey, VssTokenKey tokenKey)
    {
      string tokenIdentity = RegistryTokenStorageHelper.CreateTokenIdentity(tokenKey);
      subkey.SetValue((string) null, (object) tokenIdentity, RegistryValueKind.String);
      subkey.SetValue(RegistryTokenStorageHelper.KindValue, (object) tokenKey.Kind, RegistryValueKind.String);
      subkey.SetValue(RegistryTokenStorageHelper.UrlValue, (object) tokenKey.Resource, RegistryValueKind.String);
      subkey.SetValue(RegistryTokenStorageHelper.UserNameValue, (object) tokenKey.UserName, RegistryValueKind.String);
      subkey.SetValue(RegistryTokenStorageHelper.TypeValue, (object) tokenKey.Type, RegistryValueKind.String);
    }

    internal static bool WriteTokenPropertyValue(RegistryKey subkey, string name, string value)
    {
      using (RegistryKey subKey = subkey.CreateSubKey(RegistryTokenStorageHelper.PropertiesKeyName, RegistryKeyPermissionCheck.Default))
        subKey.SetValue(name, (object) value, RegistryValueKind.String);
      return true;
    }

    internal static void WriteSecretTokenValue(RegistryKey subkey, string tokenValue)
    {
      string str = RegistryTokenStorageHelper.ProtectTokenValue(tokenValue ?? string.Empty);
      subkey.SetValue(RegistryTokenStorageHelper.TokenValue, (object) str, RegistryValueKind.String);
    }

    internal static string ReadTokenValue(RegistryKey subkey) => Convert.ToString(subkey.GetValue(RegistryTokenStorageHelper.TokenValue, (object) string.Empty), (IFormatProvider) CultureInfo.InvariantCulture);

    internal static string ProtectTokenValue(string secret) => string.IsNullOrEmpty(secret) ? string.Empty : Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(secret), (byte[]) null, DataProtectionScope.CurrentUser));

    internal static string UnprotectTokenValue(string cipherText) => string.IsNullOrEmpty(cipherText) ? string.Empty : Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(cipherText), (byte[]) null, DataProtectionScope.CurrentUser));

    internal static RegistryKey FindTokenKey(RegistryKey root, VssToken token)
    {
      string[] subKeyNames = root.GetSubKeyNames();
      string tokenIdentity = RegistryTokenStorageHelper.CreateTokenIdentity((VssTokenKey) token);
      foreach (string name in subKeyNames)
      {
        using (RegistryKey registryKey = root.OpenSubKey(name, true))
        {
          if (registryKey != null)
          {
            foreach (string subKeyName in registryKey.GetSubKeyNames())
            {
              RegistryKey tokenKey = registryKey.OpenSubKey(subKeyName, true);
              if (tokenKey != null)
              {
                string str = tokenKey.GetValue((string) null) as string;
                if (tokenIdentity.Equals(str, StringComparison.Ordinal))
                  return tokenKey;
                tokenKey.Dispose();
              }
            }
          }
        }
      }
      return (RegistryKey) null;
    }
  }
}
