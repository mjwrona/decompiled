// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssClientCertificateManager
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VssClientCertificateManager : IVssClientCertificateManager
  {
    private Dictionary<string, X509Certificate2> m_allClientCertificates = new Dictionary<string, X509Certificate2>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private X509Certificate2Collection m_clientCertificates = new X509Certificate2Collection();
    private bool m_refreshNeeded = true;
    private ReaderWriterLock m_rwLock = new ReaderWriterLock();
    private object m_refreshLock = new object();
    private static VssClientCertificateManager s_instance = (VssClientCertificateManager) null;
    private static object s_lock = new object();
    public static readonly string OID_EXTENDED_KEY_USAGE = "2.5.29.37";
    public static readonly string OID_PKIX_KP_CLIENT_AUTH = "1.3.6.1.5.5.7.3.2";
    public static readonly string OID_EXTENDED_KEY_USAGE_ANY = "2.5.29.37.0";
    private static readonly string s_certEnvironmentConfigVar = "VstsClientCertificate";
    private static readonly string s_clientCertsKeyName = "TeamFoundation\\ClientCertificates";
    private static readonly string s_disableClientCertificatesName = "DisableClientCertificates";
    private static readonly string s_thumbprintsName = "Thumbprints";
    private static readonly char[] s_thumbprintDelimiters = new char[2]
    {
      ';',
      ','
    };

    public void Invalidate()
    {
      this.m_rwLock.AcquireWriterLock(-1);
      try
      {
        this.m_refreshNeeded = true;
      }
      finally
      {
        this.m_rwLock.ReleaseWriterLock();
      }
    }

    public X509Certificate2Collection ClientCertificates
    {
      get
      {
        this.RefreshIfNeeded();
        X509Certificate2Collection clientCertificates = new X509Certificate2Collection();
        this.m_rwLock.AcquireReaderLock(-1);
        try
        {
          clientCertificates.AddRange(this.m_clientCertificates);
        }
        finally
        {
          this.m_rwLock.ReleaseReaderLock();
        }
        return clientCertificates;
      }
    }

    public static VssClientCertificateManager Instance
    {
      get
      {
        if (VssClientCertificateManager.s_instance == null)
        {
          lock (VssClientCertificateManager.s_lock)
          {
            if (VssClientCertificateManager.s_instance == null)
              VssClientCertificateManager.s_instance = new VssClientCertificateManager();
          }
        }
        return VssClientCertificateManager.s_instance;
      }
    }

    public static X509Certificate2Collection GetClientAuthCertificates(StoreLocation storeLocation)
    {
      X509Store x509Store = (X509Store) null;
      X509Certificate2Collection authCertificates = new X509Certificate2Collection();
      try
      {
        x509Store = new X509Store(StoreName.My, storeLocation);
        x509Store.Open(OpenFlags.OpenExistingOnly);
        X509Certificate2Enumerator enumerator = x509Store.Certificates.Find(X509FindType.FindByTimeValid, (object) DateTime.Now, true).GetEnumerator();
        while (enumerator.MoveNext())
        {
          X509Certificate2 current = enumerator.Current;
          if (VssClientCertificateManager.CertificateSupportsClientAuth(current))
            authCertificates.Add(current);
        }
      }
      catch (Exception ex)
      {
      }
      finally
      {
        x509Store?.Close();
      }
      return authCertificates;
    }

    public static bool ReadEffectiveDisableClientCertificates()
    {
      bool? registryBooleanValue1 = VssClientCertificateManager.GetRegistryBooleanValue(VssClientCertificateManager.s_disableClientCertificatesName, StoreLocation.LocalMachine);
      bool? registryBooleanValue2 = VssClientCertificateManager.GetRegistryBooleanValue(VssClientCertificateManager.s_disableClientCertificatesName, StoreLocation.CurrentUser);
      bool? settingsBooleanValue = VssClientCertificateManager.GetAppSettingsBooleanValue(VssClientCertificateManager.s_disableClientCertificatesName);
      bool? nullable1 = settingsBooleanValue;
      bool flag1 = true;
      if (!(nullable1.GetValueOrDefault() == flag1 & nullable1.HasValue))
      {
        bool? nullable2 = registryBooleanValue2;
        bool flag2 = true;
        if (nullable2.GetValueOrDefault() == flag2 & nullable2.HasValue)
        {
          bool? nullable3 = settingsBooleanValue;
          bool flag3 = false;
          if (!(nullable3.GetValueOrDefault() == flag3 & nullable3.HasValue))
            goto label_6;
        }
        bool? nullable4 = registryBooleanValue1;
        bool flag4 = true;
        if (nullable4.GetValueOrDefault() == flag4 & nullable4.HasValue)
        {
          bool? nullable5 = registryBooleanValue2;
          bool flag5 = false;
          if (!(nullable5.GetValueOrDefault() == flag5 & nullable5.HasValue))
          {
            bool? nullable6 = settingsBooleanValue;
            bool flag6 = false;
            if (!(nullable6.GetValueOrDefault() == flag6 & nullable6.HasValue))
              goto label_6;
          }
        }
        return false;
      }
label_6:
      return true;
    }

    public static string[] ReadEffectiveSpecifiedCertificateThumbprints()
    {
      List<string> stringList = new List<string>();
      stringList.AddRange((IEnumerable<string>) VssClientCertificateManager.SplitDelimitedString(Environment.GetEnvironmentVariable(VssClientCertificateManager.s_certEnvironmentConfigVar)));
      try
      {
        stringList.AddRange((IEnumerable<string>) VssClientCertificateManager.SplitDelimitedString(ConfigurationManager.AppSettings[VssClientCertificateManager.s_certEnvironmentConfigVar]));
      }
      catch (ConfigurationErrorsException ex)
      {
      }
      stringList.AddRange((IEnumerable<string>) VssClientCertificateManager.ReadSpecifiedCertificateThumbprints(StoreLocation.CurrentUser));
      stringList.AddRange((IEnumerable<string>) VssClientCertificateManager.ReadSpecifiedCertificateThumbprints(StoreLocation.LocalMachine));
      stringList.Sort((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = stringList.Count - 1; index >= 1; --index)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(stringList[index], stringList[index - 1]))
          stringList.RemoveAt(index);
      }
      return stringList.ToArray();
    }

    public static string[] ReadSpecifiedCertificateThumbprints(StoreLocation machineOrUserLevel)
    {
      string stringValue = VssClientCertificateManager.GetStringValue(VssClientCertificateManager.s_thumbprintsName, machineOrUserLevel);
      return stringValue == null ? Array.Empty<string>() : stringValue.Split(VssClientCertificateManager.s_thumbprintDelimiters, StringSplitOptions.RemoveEmptyEntries);
    }

    public static void WriteDisableClientCertificates(
      bool disableClientCertificates,
      StoreLocation machineOrUserLevel)
    {
      VssClientCertificateManager.WriteStringValue(VssClientCertificateManager.s_disableClientCertificatesName, disableClientCertificates ? bool.TrueString : (string) null, machineOrUserLevel);
    }

    public static void WriteSpecifiedCertificateThumbprints(
      string[] thumbprints,
      StoreLocation machineOrUserLevel)
    {
      Dictionary<string, bool> dictionary = new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string str;
      if (thumbprints == null || thumbprints.Length == 0)
      {
        str = (string) null;
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder(Math.Max(thumbprints.Length * 17 - 1, 0));
        foreach (string thumbprint in thumbprints)
        {
          if (!dictionary.ContainsKey(thumbprint))
          {
            dictionary[thumbprint] = true;
            if (stringBuilder.Length > 0)
              stringBuilder.Append(VssClientCertificateManager.s_thumbprintDelimiters[0]);
            stringBuilder.Append(thumbprint);
          }
        }
        str = stringBuilder.ToString();
      }
      VssClientCertificateManager.WriteStringValue(VssClientCertificateManager.s_thumbprintsName, str, machineOrUserLevel);
    }

    private void RefreshIfNeeded()
    {
      if (!this.m_refreshNeeded)
        return;
      lock (this.m_refreshLock)
      {
        if (!this.m_refreshNeeded)
          return;
        this.Refresh();
      }
    }

    private void Refresh()
    {
      this.m_rwLock.AcquireWriterLock(-1);
      try
      {
        this.m_allClientCertificates.Clear();
        this.m_clientCertificates.Clear();
        if (VssClientCertificateManager.ReadEffectiveDisableClientCertificates())
          return;
        X509Certificate2Enumerator enumerator1 = VssClientCertificateManager.GetClientAuthCertificates(StoreLocation.LocalMachine).GetEnumerator();
        while (enumerator1.MoveNext())
        {
          X509Certificate2 current = enumerator1.Current;
          this.m_allClientCertificates[current.Thumbprint] = current;
        }
        X509Certificate2Enumerator enumerator2 = VssClientCertificateManager.GetClientAuthCertificates(StoreLocation.CurrentUser).GetEnumerator();
        while (enumerator2.MoveNext())
        {
          X509Certificate2 current = enumerator2.Current;
          this.m_allClientCertificates[current.Thumbprint] = current;
        }
        string[] strArray = VssClientCertificateManager.ReadEffectiveSpecifiedCertificateThumbprints();
        if (strArray.Length == 0)
        {
          foreach (X509Certificate2 x509Certificate2 in this.m_allClientCertificates.Values)
          {
            if (VssClientCertificateManager.IsPrivateKeyAvailable(x509Certificate2, out int _))
              this.m_clientCertificates.Add(x509Certificate2);
          }
        }
        else
        {
          foreach (string thumbprint in strArray)
          {
            X509Certificate2 certificateByThumbprint = this.GetClientCertificateByThumbprint(thumbprint);
            if (certificateByThumbprint != null)
              this.m_clientCertificates.Add(certificateByThumbprint);
          }
        }
      }
      finally
      {
        this.m_refreshNeeded = false;
        this.m_rwLock.ReleaseWriterLock();
      }
    }

    private static string[] SplitDelimitedString(string delimitedString) => delimitedString == null ? Array.Empty<string>() : delimitedString.Split(VssClientCertificateManager.s_thumbprintDelimiters, StringSplitOptions.RemoveEmptyEntries);

    private X509Certificate2 GetClientCertificateByThumbprint(string thumbprint)
    {
      if (thumbprint == null)
        throw new ArgumentNullException(nameof (thumbprint));
      string str = (string) null;
      X509Certificate2 toCheck;
      if (this.m_allClientCertificates.TryGetValue(thumbprint, out toCheck))
      {
        int hr;
        if (VssClientCertificateManager.IsPrivateKeyAvailable(toCheck, out hr))
          return toCheck;
        if (VssClientCertificateManager.NativeMethods.SCARD_E_NO_SMARTCARD == hr)
          str = WebApiResources.SmartCardMissing((object) thumbprint);
        else if (VssClientCertificateManager.NativeMethods.NTE_BAD_KEYSET == hr)
          str = WebApiResources.ClientCertificateNoPermission((object) thumbprint);
      }
      if (str == null)
        WebApiResources.ClientCertificateMissing((object) thumbprint);
      return (X509Certificate2) null;
    }

    private static bool CertificateSupportsClientAuth(X509Certificate2 certificate)
    {
      if (certificate == null)
        return false;
      bool flag = true;
      foreach (X509Extension extension in certificate.Extensions)
      {
        if (extension.Oid.Value == VssClientCertificateManager.OID_EXTENDED_KEY_USAGE)
        {
          flag = false;
          foreach (Oid enhancedKeyUsage in ((X509EnhancedKeyUsageExtension) extension).EnhancedKeyUsages)
          {
            if (enhancedKeyUsage.Value.Equals(VssClientCertificateManager.OID_PKIX_KP_CLIENT_AUTH) || enhancedKeyUsage.Value.Equals(VssClientCertificateManager.OID_EXTENDED_KEY_USAGE_ANY))
              flag = true;
          }
        }
      }
      return flag;
    }

    private static bool IsPrivateKeyAvailable(X509Certificate2 toCheck, out int hr)
    {
      bool pfCallerFreeProvOrNCryptKey = true;
      IntPtr phCryptProvOrNCryptKey = IntPtr.Zero;
      uint dwKeySpec = 0;
      bool flag = false;
      hr = 0;
      try
      {
      }
      finally
      {
        if (!VssClientCertificateManager.NativeMethods.CryptAcquireCertificatePrivateKey(toCheck.Handle, VssClientCertificateManager.NativeMethods.CRYPT_ACQUIRE_SILENT_FLAG, IntPtr.Zero, out phCryptProvOrNCryptKey, out dwKeySpec, out pfCallerFreeProvOrNCryptKey))
        {
          hr = Marshal.GetLastWin32Error();
          if (VssClientCertificateManager.NativeMethods.NTE_SILENT_CONTEXT == hr)
            flag = true;
        }
        else
        {
          if (pfCallerFreeProvOrNCryptKey && IntPtr.Zero != phCryptProvOrNCryptKey)
            VssClientCertificateManager.NativeMethods.CryptReleaseContext(phCryptProvOrNCryptKey, 0);
          flag = true;
        }
      }
      return flag;
    }

    private static bool? GetRegistryBooleanValue(string name, StoreLocation machineOrUserLevel) => VssClientCertificateManager.ParseBoolString(VssClientCertificateManager.GetStringValue(name, machineOrUserLevel));

    private static bool? GetAppSettingsBooleanValue(string appSettingKey)
    {
      string appSetting;
      try
      {
        appSetting = ConfigurationManager.AppSettings[appSettingKey];
      }
      catch (ConfigurationErrorsException ex)
      {
        return new bool?();
      }
      return VssClientCertificateManager.ParseBoolString(appSetting);
    }

    private static bool? ParseBoolString(string boolString)
    {
      if (boolString != null)
      {
        if (string.Equals(boolString, bool.TrueString, StringComparison.OrdinalIgnoreCase))
          return new bool?(true);
        if (string.Equals(boolString, bool.FalseString, StringComparison.OrdinalIgnoreCase))
          return new bool?(false);
      }
      return new bool?();
    }

    private static string GetStringValue(string name, StoreLocation machineOrUserLevel)
    {
      RegistryKey registryKey1 = (RegistryKey) null;
      RegistryKey registryKey2 = (RegistryKey) null;
      try
      {
        switch (machineOrUserLevel)
        {
          case StoreLocation.CurrentUser:
            registryKey1 = VssClientEnvironment.TryGetUserRegistryRoot();
            break;
          case StoreLocation.LocalMachine:
            registryKey1 = VssClientEnvironment.TryGetApplicationRegistryRoot();
            break;
        }
        if (registryKey1 == null)
          return (string) null;
        registryKey2 = registryKey1.OpenSubKey(VssClientCertificateManager.s_clientCertsKeyName);
        return registryKey2 == null ? (string) null : registryKey2.GetValue(name) as string;
      }
      catch
      {
      }
      finally
      {
        registryKey1?.Dispose();
        registryKey2?.Dispose();
      }
      return (string) null;
    }

    private static void WriteStringValue(
      string name,
      string value,
      StoreLocation machineOrUserLevel)
    {
      RegistryKey registryKey1 = (RegistryKey) null;
      RegistryKey registryKey2 = (RegistryKey) null;
      try
      {
        switch (machineOrUserLevel)
        {
          case StoreLocation.CurrentUser:
            registryKey1 = VssClientEnvironment.TryGetUserRegistryRoot();
            break;
          case StoreLocation.LocalMachine:
            registryKey1 = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\VisualStudio\\19.0", true);
            break;
        }
        registryKey2 = registryKey1.OpenSubKey(VssClientCertificateManager.s_clientCertsKeyName, true) ?? registryKey1.CreateSubKey(VssClientCertificateManager.s_clientCertsKeyName);
        if (value == null)
          registryKey2.DeleteValue(name, false);
        else
          registryKey2.SetValue(name, (object) value);
      }
      finally
      {
        registryKey1?.Dispose();
        registryKey2?.Dispose();
      }
    }

    private class NativeMethods
    {
      public static readonly int CRYPT_ACQUIRE_SILENT_FLAG = 64;
      public static readonly int NTE_BAD_KEYSET = -2146893802;
      public static readonly int NTE_SILENT_CONTEXT = -2146893790;
      public static readonly int SCARD_E_NO_SMARTCARD = -2146435060;

      [DllImport("crypt32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CryptAcquireCertificatePrivateKey(
        IntPtr pCert,
        int dwFlags,
        IntPtr pvReserved,
        out IntPtr phCryptProvOrNCryptKey,
        out uint dwKeySpec,
        [MarshalAs(UnmanagedType.Bool)] out bool pfCallerFreeProvOrNCryptKey);

      [DllImport("advapi32.dll", SetLastError = true)]
      public static extern int CryptReleaseContext(IntPtr hProv, int dwFlags);
    }
  }
}
