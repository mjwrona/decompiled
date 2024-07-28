// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TokenStorage.RegistryTokenStorage
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Common.TokenStorage
{
  internal class RegistryTokenStorage : VssTokenStorage
  {
    private readonly string m_subkeyName;

    public RegistryTokenStorage(string subkeyName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(subkeyName, nameof (subkeyName));
      this.m_subkeyName = subkeyName;
    }

    internal static void DeleteTokenStorage(string registryRootPath, string storageNamespace) => RegistryTokenStorageHelper.DeleteTokenStorage(registryRootPath, storageNamespace);

    public override IEnumerable<VssToken> RetrieveAll(string kind)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(kind, nameof (kind));
      List<VssToken> vssTokenList = new List<VssToken>();
      using (RegistryKey rootKey = this.GetRootKey())
      {
        foreach (string subKeyName1 in rootKey.GetSubKeyNames())
        {
          if (string.Equals(kind, subKeyName1, StringComparison.Ordinal))
          {
            using (RegistryKey registryKey = rootKey.OpenSubKey(subKeyName1, false))
            {
              if (registryKey != null)
              {
                foreach (string subKeyName2 in registryKey.GetSubKeyNames())
                {
                  RegistryKey subkey = registryKey.OpenSubKey(subKeyName2, false);
                  if (subkey != null)
                  {
                    using (subkey)
                      vssTokenList.Add((VssToken) RegistryTokenStorageHelper.ReadTokenData(subkey, this));
                  }
                }
              }
            }
          }
        }
      }
      return (IEnumerable<VssToken>) vssTokenList;
    }

    public override string RetrieveTokenSecret(VssToken token)
    {
      using (RegistryKey rootKey = this.GetRootKey())
      {
        using (RegistryKey tokenKey = RegistryTokenStorageHelper.FindTokenKey(rootKey, token))
        {
          if (tokenKey != null)
          {
            string cipherText = RegistryTokenStorageHelper.ReadTokenValue(tokenKey);
            try
            {
              return RegistryTokenStorageHelper.UnprotectTokenValue(cipherText);
            }
            catch (CryptographicException ex)
            {
              return (string) null;
            }
          }
        }
      }
      return (string) null;
    }

    public override bool SetTokenSecret(VssToken token, string tokenValue) => this.UpdateToken(token, tokenValue, true) != null;

    public override bool RemoveTokenSecret(VssToken token) => this.UpdateToken(token, string.Empty, true) != null;

    public override string GetProperty(VssToken token, string name)
    {
      using (RegistryKey rootKey = this.GetRootKey())
      {
        using (RegistryKey tokenKey = RegistryTokenStorageHelper.FindTokenKey(rootKey, token))
          return tokenKey != null ? RegistryTokenStorageHelper.ReadTokenPropertyValue(tokenKey, name) : (string) null;
      }
    }

    public override IEnumerable<string> GetPropertyNames(VssToken token)
    {
      using (RegistryKey rootKey = this.GetRootKey())
      {
        using (RegistryKey tokenKey = RegistryTokenStorageHelper.FindTokenKey(rootKey, token))
          return tokenKey != null ? RegistryTokenStorageHelper.ReadTokenPropertyNames(tokenKey) : (IEnumerable<string>) null;
      }
    }

    public override bool SetProperty(VssToken token, string name, string value)
    {
      using (RegistryKey rootKey = this.GetRootKey())
      {
        using (RegistryKey tokenKey = RegistryTokenStorageHelper.FindTokenKey(rootKey, token))
          return tokenKey != null && RegistryTokenStorageHelper.WriteTokenPropertyValue(tokenKey, name, value);
      }
    }

    public override bool RemoveAll()
    {
      try
      {
        using (RegistryKey rootKey = this.GetRootKey())
        {
          foreach (string subKeyName in rootKey.GetSubKeyNames())
            rootKey.DeleteSubKeyTree(subKeyName, false);
        }
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    protected override VssToken RetrieveToken(VssTokenKey tokenKey)
    {
      VssToken vssToken = (VssToken) null;
      string tokenIdentity = RegistryTokenStorageHelper.CreateTokenIdentity(tokenKey);
      using (RegistryKey rootKey = this.GetRootKey())
      {
        RegistryKey registryKey = rootKey.OpenSubKey(tokenKey.Kind, false);
        if (registryKey != null)
        {
          using (registryKey)
          {
            foreach (string subKeyName in registryKey.GetSubKeyNames())
            {
              vssToken = (VssToken) RegistryTokenStorageHelper.CreateTokenFromDataIfMatch(registryKey.OpenSubKey(subKeyName, false), this, tokenIdentity);
              if (vssToken != null)
                break;
            }
          }
        }
      }
      return vssToken;
    }

    protected override VssToken AddToken(VssTokenKey tokenKey, string tokenValue) => this.UpdateToken((VssToken) new RegistryToken((VssTokenStorage) this, tokenKey, string.Empty), tokenValue, false);

    protected override bool RemoveToken(VssTokenKey tokenKey)
    {
      string tokenIdentity = RegistryTokenStorageHelper.CreateTokenIdentity(tokenKey);
      bool flag = false;
      using (RegistryKey rootKey = this.GetRootKey())
      {
        int num = -1;
        using (RegistryKey registryKey1 = rootKey.OpenSubKey(tokenKey.Kind, true))
        {
          if (registryKey1 != null)
          {
            foreach (string subKeyName in registryKey1.GetSubKeyNames())
            {
              string str = (string) null;
              using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName, true))
              {
                if (registryKey2 != null)
                  str = registryKey2.GetValue((string) null) as string;
                else
                  continue;
              }
              if (tokenIdentity.Equals(str, StringComparison.Ordinal))
              {
                registryKey1.DeleteSubKeyTree(subKeyName, false);
                flag = true;
              }
            }
            num = registryKey1.SubKeyCount;
          }
          if (num == 0)
            rootKey.DeleteSubKeyTree(tokenKey.Kind, false);
        }
      }
      return flag;
    }

    private VssToken UpdateToken(VssToken token, string tokenValue, bool updateValueOnly)
    {
      using (RegistryKey rootKey = this.GetRootKey())
      {
        using (RegistryKey subKey = rootKey.CreateSubKey(token.Kind, RegistryKeyPermissionCheck.Default, RegistryOptions.None))
        {
          if (subKey == null)
            RegistryTokenStorageHelper.ThrowExceptionCouldNotCreateSubKey(rootKey.Name, token.Kind);
          return RegistryTokenStorageHelper.UpdateToken(token, tokenValue, subKey, updateValueOnly) ? (VssToken) new RegistryToken((VssTokenStorage) this, (VssTokenKey) token, tokenValue) : (VssToken) null;
        }
      }
    }

    private RegistryKey GetRootKey() => RegistryTokenStorageHelper.GetRootKey(this.m_subkeyName);
  }
}
