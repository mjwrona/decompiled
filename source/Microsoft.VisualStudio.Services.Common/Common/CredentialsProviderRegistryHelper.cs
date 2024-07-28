// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CredentialsProviderRegistryHelper
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  internal class CredentialsProviderRegistryHelper
  {
    private readonly RegistryView m_registryView = RegistryView.Registry64;
    private readonly string m_accountsRoot = "SOFTWARE\\Microsoft\\TeamFoundationServer\\14.0\\HostedServiceAccounts";

    internal CredentialsProviderRegistryHelper()
    {
    }

    internal CredentialsProviderRegistryHelper(string registryRootPath, RegistryView registryView)
    {
      this.m_accountsRoot = registryRootPath;
      this.m_registryView = registryView;
    }

    public RegistryKey OpenLocalMachineHive() => RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, this.RegistryView);

    internal static void ClearTestEnvironment(string testRoot)
    {
      try
      {
        using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
          registryKey.DeleteSubKeyTree(testRoot, false);
      }
      catch (Exception ex)
      {
      }
    }

    internal static void LoadCachedVssCredentialProviders(
      ref ConcurrentDictionary<string, ICachedVssCredentialProvider> providers)
    {
      try
      {
        string name = Path.Combine("Software\\Microsoft\\VisualStudio", "19.0", "TeamFoundation", CredentialsCacheConstants.RegisteredProviderKeyName);
        using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default))
        {
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey(name, RegistryKeyPermissionCheck.ReadSubTree))
          {
            string[] valueNames = registryKey2?.GetValueNames();
            if (valueNames == null || valueNames.Length == 0)
              return;
            foreach (string str in valueNames)
            {
              if (!providers.ContainsKey(str))
              {
                string assemblyFile = (string) registryKey2.GetValue(str);
                if (!string.IsNullOrEmpty(assemblyFile))
                {
                  try
                  {
                    foreach (Type type in ((IEnumerable<Type>) Assembly.LoadFrom(assemblyFile).GetTypes()).Where<Type>((Func<Type, bool>) (x => x.IsClass && !x.IsAbstract && typeof (ICachedVssCredentialProvider).IsAssignableFrom(x))))
                    {
                      try
                      {
                        ICachedVssCredentialProvider instance = (ICachedVssCredentialProvider) Activator.CreateInstance(type);
                        providers.TryAdd(str, instance);
                      }
                      catch
                      {
                      }
                    }
                  }
                  catch
                  {
                  }
                }
              }
            }
          }
        }
      }
      catch
      {
      }
    }

    public RegistrySecurity BuildSecurityDescriptor(string owningAccountName)
    {
      RegistrySecurity registrySecurity = new RegistrySecurity();
      registrySecurity.AddAccessRule(new RegistryAccessRule((IdentityReference) new NTAccount(owningAccountName), RegistryRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
      registrySecurity.AddAccessRule(new RegistryAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, (SecurityIdentifier) null), RegistryRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
      return registrySecurity;
    }

    public string BuildRegistryPath(string featureRegistryKeyword, string targetName) => this.BuildRegistryPath(featureRegistryKeyword) + "\\" + targetName;

    public string BuildRegistryPath(string featureRegistryKeyword) => this.m_accountsRoot + "\\" + featureRegistryKeyword;

    public string BuildRegistryPath() => this.m_accountsRoot;

    public string EncryptPassword(SecureString password)
    {
      IntPtr num = IntPtr.Zero;
      byte[] numArray1 = (byte[]) null;
      byte[] numArray2 = (byte[]) null;
      int length = password.Length * 2;
      try
      {
        num = Marshal.SecureStringToCoTaskMemUnicode(password);
        numArray1 = new byte[length];
        Marshal.Copy(num, numArray1, 0, length);
        numArray2 = ProtectedData.Protect(numArray1, (byte[]) null, DataProtectionScope.LocalMachine);
        return Convert.ToBase64String(numArray2);
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.ZeroFreeCoTaskMemUnicode(num);
        CredentialsProviderRegistryHelper.ZeroArray(numArray1);
        CredentialsProviderRegistryHelper.ZeroArray(numArray2);
      }
    }

    public string DecryptPassword(string cipherText)
    {
      byte[] numArray1 = (byte[]) null;
      byte[] numArray2 = (byte[]) null;
      try
      {
        numArray1 = Convert.FromBase64String(cipherText);
        numArray2 = ProtectedData.Unprotect(numArray1, (byte[]) null, DataProtectionScope.LocalMachine);
        return Encoding.Unicode.GetString(numArray2);
      }
      finally
      {
        CredentialsProviderRegistryHelper.ZeroArray(numArray1);
        CredentialsProviderRegistryHelper.ZeroArray(numArray2);
      }
    }

    private static void ZeroArray(byte[] b)
    {
      if (b == null)
        return;
      Array.Clear((Array) b, 0, b.Length);
    }

    public RegistryView RegistryView => this.m_registryView;
  }
}
