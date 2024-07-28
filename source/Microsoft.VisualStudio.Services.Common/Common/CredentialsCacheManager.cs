// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CredentialsCacheManager
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CredentialsCacheManager
  {
    private CredentialsProviderRegistryHelper m_credentialsProviderRegistryHelper;

    public CredentialsCacheManager() => this.m_credentialsProviderRegistryHelper = new CredentialsProviderRegistryHelper();

    public CredentialsCacheManager(string registryRootPath, bool useWOW6432Hive)
    {
      RegistryView registryView = useWOW6432Hive ? RegistryView.Registry32 : RegistryView.Registry64;
      this.m_credentialsProviderRegistryHelper = new CredentialsProviderRegistryHelper(registryRootPath, registryView);
    }

    public TfsCredentialCacheEntry GetCredentials(Uri uri) => this.GetCredentials((string) null, uri);

    public TfsCredentialCacheEntry GetCredentials(Uri uri, bool requireExactUriMatch) => this.GetCredentials((string) null, uri, requireExactUriMatch);

    public TfsCredentialCacheEntry GetCredentials(
      Uri uri,
      bool requireExactUriMatch,
      bool? nonInteractive)
    {
      return this.GetCredentials((string) null, uri, requireExactUriMatch, nonInteractive);
    }

    public TfsCredentialCacheEntry GetCredentials(string featureRegistryKeyword, Uri uri) => this.GetCredentials(featureRegistryKeyword, uri, false);

    public TfsCredentialCacheEntry GetCredentials(
      string featureRegistryKeyword,
      Uri uri,
      bool requireExactUriMatch)
    {
      return this.GetCredentials(featureRegistryKeyword, uri, requireExactUriMatch, new bool?());
    }

    public TfsCredentialCacheEntry GetCredentials(
      string featureRegistryKeyword,
      Uri uri,
      bool requireExactUriMatch,
      bool? nonInteractive)
    {
      TfsCredentialCacheEntry credentials1 = this.GetCredentials(featureRegistryKeyword, uri.AbsoluteUri);
      if (requireExactUriMatch || CredentialsCacheManager.IsMatch(credentials1, nonInteractive))
        return credentials1;
      string leftPart = uri.GetLeftPart(UriPartial.Authority);
      string targetName = uri.AbsoluteUri;
      for (int length = targetName.LastIndexOf('/', targetName.Length - 2); length >= leftPart.Length; length = targetName.LastIndexOf('/', targetName.Length - 2))
      {
        targetName = length != leftPart.Length ? targetName.Substring(0, length) : targetName.Substring(0, length + 1);
        TfsCredentialCacheEntry credentials2 = this.GetCredentials(featureRegistryKeyword, targetName);
        if (CredentialsCacheManager.IsMatch(credentials2, nonInteractive))
          return credentials2;
      }
      return (TfsCredentialCacheEntry) null;
    }

    public bool ContainsCredentials(Uri uri) => this.GetCredentials((string) null, uri.AbsoluteUri) != null;

    public bool ContainsCredentials(string featureRegistryKeyword, Uri uri) => this.GetCredentials(featureRegistryKeyword, uri.AbsoluteUri) != null;

    private TfsCredentialCacheEntry GetCredentials(string featureRegistryKeyword, string targetName)
    {
      if (string.IsNullOrEmpty(featureRegistryKeyword))
        featureRegistryKeyword = Environment.GetEnvironmentVariable("TFS_REG_CRED");
      string targetKey = this.GetTargetKey(targetName);
      return string.IsNullOrEmpty(featureRegistryKeyword) ? CredentialsCacheManager.GetCredentialsFromStore(targetKey) : this.GetCredentialsFromRegistry(featureRegistryKeyword, targetKey);
    }

    private static TfsCredentialCacheEntry GetCredentialsFromStore(string targetName)
    {
      if (targetName == null)
        return (TfsCredentialCacheEntry) null;
      string toZero = (string) null;
      IntPtr credential = IntPtr.Zero;
      try
      {
        if (!Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CredRead(targetName, 1U, 0U, out credential))
          return (TfsCredentialCacheEntry) null;
        Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL structure1 = (Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL) Marshal.PtrToStructure(credential, typeof (Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL));
        if (string.IsNullOrEmpty(structure1.UserName))
          return (TfsCredentialCacheEntry) null;
        int len1 = structure1.CredentialBlobSize / 2;
        toZero = len1 <= 0 || !(IntPtr.Zero != structure1.CredentialBlob) ? string.Empty : Marshal.PtrToStringUni(structure1.CredentialBlob, len1);
        NetworkCredential networkCredential = CredentialsProviderHelper.GetNetworkCredential(structure1.UserName, new StringBuilder(toZero));
        TfsCredentialCacheEntry credentialsFromStore = new TfsCredentialCacheEntry(structure1.TargetName, networkCredential, structure1.Comment, CachedCredentialsType.Other, true);
        if (structure1.AttributeCount > 0)
        {
          int num = Marshal.SizeOf(typeof (Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE));
          IntPtr ptr = structure1.Attributes;
          for (int index = 0; index < structure1.AttributeCount; ++index)
          {
            Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE structure2 = (Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE) Marshal.PtrToStructure(ptr, typeof (Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE));
            int len2 = structure2.ValueSize / 2;
            if (len2 > 0 && IntPtr.Zero != structure2.Value)
              credentialsFromStore.Attributes[structure2.Keyword] = Marshal.PtrToStringUni(structure2.Value, len2);
            ptr = new IntPtr(ptr.ToInt64() + (long) num);
          }
        }
        return credentialsFromStore;
      }
      finally
      {
        if (IntPtr.Zero != credential)
        {
          IntPtr num = Marshal.OffsetOf(typeof (Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL), "CredentialBlob");
          int int32_1 = num.ToInt32();
          IntPtr address = Marshal.ReadIntPtr(credential, int32_1);
          num = Marshal.OffsetOf(typeof (Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL), "CredentialBlobSize");
          int int32_2 = num.ToInt32();
          int byteCount = Marshal.ReadInt32(credential, int32_2);
          Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.ZeroMemory(address, (uint) byteCount);
          Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CredFree(credential);
          IntPtr zero = IntPtr.Zero;
        }
        if (!string.IsNullOrEmpty(toZero))
          CredentialsProviderHelper.ZeroString(toZero);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static SecureString CreateSecureString(string str)
    {
      SecureString secureString = new SecureString();
      if (!string.IsNullOrEmpty(str))
      {
        foreach (char c in str)
          secureString.AppendChar(c);
      }
      return secureString;
    }

    private TfsCredentialCacheEntry GetCredentialsFromRegistry(
      string featureRegistryKeyword,
      string targetName)
    {
      if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(featureRegistryKeyword))
        return (TfsCredentialCacheEntry) null;
      string toZero = (string) null;
      string name = this.m_credentialsProviderRegistryHelper.BuildRegistryPath(featureRegistryKeyword, targetName);
      try
      {
        using (RegistryKey registryKey1 = this.m_credentialsProviderRegistryHelper.OpenLocalMachineHive())
        {
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey(name, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ExecuteKey))
          {
            if (registryKey2 == null)
              return (TfsCredentialCacheEntry) null;
            string username = (string) registryKey2.GetValue(CredentialsCacheConstants.UserNameKeyword, (object) null);
            toZero = this.m_credentialsProviderRegistryHelper.DecryptPassword((string) registryKey2.GetValue(CredentialsCacheConstants.UserPasswordKeyword, (object) string.Empty));
            if (string.IsNullOrEmpty(username))
              return (TfsCredentialCacheEntry) null;
            NetworkCredential networkCredential = CredentialsProviderHelper.GetNetworkCredential(username, new StringBuilder(toZero));
            TfsCredentialCacheEntry credentialsFromRegistry = new TfsCredentialCacheEntry(targetName, networkCredential, (string) null, CachedCredentialsType.Other, true);
            string[] valueNames = registryKey2.GetValueNames();
            if (valueNames.Length != 0)
            {
              for (int index = 0; index < valueNames.Length; ++index)
              {
                string str1 = valueNames[index];
                string str2 = (string) registryKey2.GetValue(str1, (object) string.Empty);
                if (!str1.Equals(CredentialsCacheConstants.UserNameKeyword, StringComparison.Ordinal) && !str1.Equals(CredentialsCacheConstants.UserPasswordKeyword, StringComparison.Ordinal))
                  credentialsFromRegistry.Attributes[str1] = str2;
              }
            }
            return credentialsFromRegistry;
          }
        }
      }
      catch
      {
        return (TfsCredentialCacheEntry) null;
      }
      finally
      {
        if (!string.IsNullOrEmpty(toZero))
          CredentialsProviderHelper.ZeroString(toZero);
      }
    }

    public int StoreCredentials(Uri uri, string userName, string password) => this.StoreCredentials((string) null, uri, userName, password);

    public int StoreCredentials(
      Uri uri,
      string userName,
      string password,
      CachedCredentialsType type,
      bool nonInteractive)
    {
      return this.StoreCredentials((string) null, uri, userName, password, type, nonInteractive);
    }

    public int StoreCredentials(
      Uri uri,
      string userName,
      SecureString password,
      CachedCredentialsType type,
      bool nonInteractive)
    {
      return this.StoreCredentials((string) null, uri, userName, password, type, nonInteractive);
    }

    public int StoreCredentials(
      Uri uri,
      string userName,
      SecureString password,
      CachedCredentialsType type,
      bool nonInteractive,
      Dictionary<string, string> additionalAttributes)
    {
      return this.StoreCredentials((string) null, uri, userName, password, type, nonInteractive, additionalAttributes);
    }

    public int StoreCredentials(
      string targetName,
      string userName,
      SecureString password,
      string comment,
      Dictionary<string, string> attributes)
    {
      return this.StoreCredentials((string) null, targetName, userName, password, comment, attributes);
    }

    public int StoreCredentials(
      string featureRegistryKeyword,
      Uri uri,
      string userName,
      string password)
    {
      return this.StoreCredentials(featureRegistryKeyword, uri, userName, password, CachedCredentialsType.Other, true);
    }

    public int StoreCredentials(
      string featureRegistryKeyword,
      Uri uri,
      string userName,
      string password,
      CachedCredentialsType type,
      bool nonInteractive)
    {
      return this.StoreCredentials(featureRegistryKeyword, uri, userName, CredentialsCacheManager.CreateSecureString(password), type, nonInteractive);
    }

    public int StoreCredentials(
      string featureRegistryKeyword,
      Uri uri,
      string userName,
      SecureString password,
      CachedCredentialsType type,
      bool nonInteractive)
    {
      Dictionary<string, string> additionalAttributes = new Dictionary<string, string>();
      return this.StoreCredentials(featureRegistryKeyword, uri, userName, password, type, nonInteractive, additionalAttributes);
    }

    public int StoreCredentials(
      string featureRegistryKeyword,
      Uri uri,
      string userName,
      SecureString password,
      CachedCredentialsType type,
      bool nonInteractive,
      Dictionary<string, string> additionalAttributes)
    {
      if (additionalAttributes == null)
        additionalAttributes = new Dictionary<string, string>();
      additionalAttributes[CredentialsCacheConstants.CredentialsTypeKeyword] = type.ToString();
      additionalAttributes[CredentialsCacheConstants.NonInteractiveKeyword] = nonInteractive.ToString();
      return this.StoreCredentials(featureRegistryKeyword, uri.AbsoluteUri, userName, password, (string) null, additionalAttributes);
    }

    public int StoreCredentials(
      string featureRegistryKeyword,
      string targetName,
      string userName,
      SecureString password,
      string comment,
      Dictionary<string, string> attributes)
    {
      if (string.IsNullOrEmpty(featureRegistryKeyword))
        featureRegistryKeyword = Environment.GetEnvironmentVariable("TFS_REG_CRED");
      string targetKey = this.GetTargetKey(targetName);
      return string.IsNullOrEmpty(featureRegistryKeyword) ? CredentialsCacheManager.StoreCredentialsToStore(1, targetKey, userName, password, comment, attributes) : this.StoreCredentialsToRegistry(featureRegistryKeyword, targetKey, userName, password, comment, attributes);
    }

    public int StoreWindowsCredentials(string host, string userName, string password) => CredentialsCacheManager.StoreCredentialsToStore(2, host, userName, CredentialsCacheManager.CreateSecureString(password), (string) null, new Dictionary<string, string>());

    private static int StoreCredentialsToStore(
      int credType,
      string targetName,
      string userName,
      SecureString password,
      string comment,
      Dictionary<string, string> attributes)
    {
      Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL credential = new Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL();
      Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE[] credentialAttributeArray = new Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE[attributes.Count];
      credential.CredentialBlob = IntPtr.Zero;
      try
      {
        credential.Flags = 0;
        credential.Type = credType;
        credential.TargetName = targetName;
        credential.Comment = comment;
        credential.CredentialBlob = Marshal.SecureStringToCoTaskMemUnicode(password);
        credential.CredentialBlobSize = password.Length * 2;
        credential.Persist = 2;
        if (attributes.Count > 0)
        {
          int num = 0;
          int cb = 0;
          foreach (KeyValuePair<string, string> attribute in attributes)
          {
            Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE structure = new Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE();
            structure.Keyword = attribute.Key;
            structure.Flags = 0;
            structure.ValueSize = attribute.Value.Length * 2;
            structure.Value = Marshal.StringToCoTaskMemUni(attribute.Value);
            checked { cb += Marshal.SizeOf<Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE>(structure); }
            credentialAttributeArray[num++] = structure;
          }
          IntPtr ptr = Marshal.AllocCoTaskMem(cb);
          credential.AttributeCount = attributes.Count;
          credential.Attributes = ptr;
          for (int index = 0; index < credentialAttributeArray.Length; ++index)
          {
            Marshal.StructureToPtr<Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE>(credentialAttributeArray[index], ptr, false);
            ptr = new IntPtr(ptr.ToInt64() + (long) Marshal.SizeOf<Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE>(credentialAttributeArray[index]));
          }
        }
        else
        {
          credential.AttributeCount = 0;
          credential.Attributes = IntPtr.Zero;
        }
        credential.TargetAlias = (string) null;
        credential.UserName = userName;
        return !Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CredWrite(ref credential, 0U) ? Marshal.GetLastWin32Error() : 0;
      }
      finally
      {
        if (IntPtr.Zero != credential.CredentialBlob)
        {
          Marshal.ZeroFreeCoTaskMemUnicode(credential.CredentialBlob);
          credential.CredentialBlob = IntPtr.Zero;
        }
        if (IntPtr.Zero != credential.Attributes)
        {
          Marshal.FreeCoTaskMem(credential.Attributes);
          credential.Attributes = IntPtr.Zero;
        }
        for (int index = 0; index < credentialAttributeArray.Length; ++index)
        {
          Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CREDENTIAL_ATTRIBUTE credentialAttribute = credentialAttributeArray[index];
          if (IntPtr.Zero != credentialAttribute.Value)
          {
            Marshal.ZeroFreeCoTaskMemUnicode(credentialAttribute.Value);
            credentialAttribute.Value = IntPtr.Zero;
          }
        }
      }
    }

    private int StoreCredentialsToRegistry(
      string featureRegistryKeyword,
      string targetName,
      string userName,
      SecureString password,
      string comment,
      Dictionary<string, string> attributes)
    {
      if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(featureRegistryKeyword))
        return 82;
      string path = this.m_credentialsProviderRegistryHelper.BuildRegistryPath(featureRegistryKeyword, targetName);
      try
      {
        using (RegistryKey hive = this.m_credentialsProviderRegistryHelper.OpenLocalMachineHive())
        {
          using (RegistryKey subKey = CredentialsCacheManager.CreateSubKey(hive, path))
          {
            foreach (string valueName in subKey.GetValueNames())
              subKey.DeleteValue(valueName);
            subKey.SetValue(CredentialsCacheConstants.UserNameKeyword, (object) userName);
            subKey.SetValue(CredentialsCacheConstants.UserPasswordKeyword, (object) this.m_credentialsProviderRegistryHelper.EncryptPassword(password));
            foreach (KeyValuePair<string, string> attribute in attributes)
              subKey.SetValue(attribute.Key, (object) attribute.Value);
            return 0;
          }
        }
      }
      catch
      {
        return 5;
      }
    }

    public bool DeleteCredentials(Uri uri) => Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CredDelete(uri.AbsoluteUri, 1U, 0U);

    public bool DeleteCredentials(string featureRegistryKeyword, Uri uri)
    {
      if (string.IsNullOrEmpty(featureRegistryKeyword) || uri == (Uri) null)
        return false;
      string targetKey = CredentialsCacheManager.GetTargetKey(uri);
      string subkey = this.m_credentialsProviderRegistryHelper.BuildRegistryPath(featureRegistryKeyword, targetKey);
      try
      {
        using (RegistryKey registryKey = this.m_credentialsProviderRegistryHelper.OpenLocalMachineHive())
          registryKey.DeleteSubKeyTree(subkey, false);
      }
      catch
      {
        return false;
      }
      return true;
    }

    public bool DeleteCredentialsHive(string featureRegistryKeyword)
    {
      if (string.IsNullOrEmpty(featureRegistryKeyword))
        return false;
      string subkey = this.m_credentialsProviderRegistryHelper.BuildRegistryPath(featureRegistryKeyword);
      try
      {
        using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
          registryKey.DeleteSubKeyTree(subkey, false);
      }
      catch
      {
        return false;
      }
      return true;
    }

    public bool DeleteWindowsCredentials(string host) => Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.CredDelete(host, 2U, 0U);

    public bool SetHiveAccessRights(string featureRegistryKeyword, string owningAccountName)
    {
      if (string.IsNullOrEmpty(featureRegistryKeyword) || string.IsNullOrEmpty(owningAccountName))
        return false;
      this.m_credentialsProviderRegistryHelper.BuildRegistryPath();
      string path = this.m_credentialsProviderRegistryHelper.BuildRegistryPath(featureRegistryKeyword);
      RegistrySecurity registrySecurity = this.m_credentialsProviderRegistryHelper.BuildSecurityDescriptor(owningAccountName);
      using (RegistryKey hive = this.m_credentialsProviderRegistryHelper.OpenLocalMachineHive())
      {
        using (RegistryKey subKey = CredentialsCacheManager.CreateSubKey(hive, path))
        {
          if (subKey == null)
            return false;
          foreach (string subKeyName in subKey.GetSubKeyNames())
          {
            using (RegistryKey registryKey = subKey.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl))
              registryKey.SetAccessControl(registrySecurity);
          }
          return true;
        }
      }
    }

    private static bool IsMatch(TfsCredentialCacheEntry entry, bool? nonInteractive)
    {
      if (entry == null)
        return false;
      if (nonInteractive.HasValue)
      {
        int num1 = entry.NonInteractive ? 1 : 0;
        bool? nullable = nonInteractive;
        int num2 = nullable.GetValueOrDefault() ? 1 : 0;
        if (!(num1 == num2 & nullable.HasValue))
          return false;
      }
      return true;
    }

    private string GetTargetKey(string targetName) => string.IsNullOrEmpty(targetName) ? (string) null : CredentialsCacheManager.GetTargetKey(new Uri(targetName));

    private static string GetTargetKey(Uri targetUrl)
    {
      if (targetUrl == (Uri) null)
        return (string) null;
      if (targetUrl.Segments.Length == 1 || !targetUrl.Segments[targetUrl.Segments.Length - 1].EndsWith("/", StringComparison.Ordinal))
        return targetUrl.AbsoluteUri;
      string absoluteUri = targetUrl.AbsoluteUri;
      return absoluteUri.Remove(absoluteUri.Length - 1);
    }

    private static RegistryKey CreateSubKey(RegistryKey hive, string path) => hive.OpenSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl) ?? hive.CreateSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, Microsoft.Win32.RegistryOptions.None);
  }
}
