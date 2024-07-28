// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ClientCertificateManager
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ClientCertificateManager
  {
    private Dictionary<string, X509Certificate2> m_allClientCertificates = new Dictionary<string, X509Certificate2>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private X509Certificate2Collection m_clientCertificates = new X509Certificate2Collection();
    private bool m_refreshNeeded = true;
    private ReaderWriterLock m_rwLock = new ReaderWriterLock();
    private object m_refreshLock = new object();
    private static ClientCertificateManager s_instance = (ClientCertificateManager) null;
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

    public void ApplyCertificatesToWebRequest(HttpWebRequest request)
    {
      if (Uri.UriSchemeHttps != request.RequestUri.Scheme)
        return;
      this.RefreshIfNeeded();
      this.m_rwLock.AcquireReaderLock(-1);
      try
      {
        ClientCertificateManager.ApplyCertificatesToWebRequest(request, this.m_clientCertificates);
      }
      finally
      {
        this.m_rwLock.ReleaseReaderLock();
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

    public static ClientCertificateManager Instance
    {
      get
      {
        if (ClientCertificateManager.s_instance == null)
        {
          lock (ClientCertificateManager.s_lock)
          {
            if (ClientCertificateManager.s_instance == null)
              ClientCertificateManager.s_instance = new ClientCertificateManager();
          }
        }
        return ClientCertificateManager.s_instance;
      }
    }

    public static void ApplyCertificatesToWebRequest(
      HttpWebRequest request,
      X509Certificate2Collection certificates)
    {
      if (certificates == null || request == null || certificates.Count <= 0 || !(Uri.UriSchemeHttps == request.RequestUri.Scheme))
        return;
      request.ClientCertificates.AddRange((X509CertificateCollection) certificates);
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
          if (ClientCertificateManager.CertificateSupportsClientAuth(current))
            authCertificates.Add(current);
        }
      }
      catch (Exception ex)
      {
        UIHost.WriteError(LogCategory.General, ClientResources.ClientCertificateErrorReadingStore((object) ex.Message));
      }
      finally
      {
        x509Store?.Close();
      }
      return authCertificates;
    }

    public static bool? ReadDisableClientCertificates(StoreLocation machineOrUserLevel) => ClientCertificateManager.GetBooleanValue(ClientCertificateManager.s_disableClientCertificatesName, machineOrUserLevel);

    public static bool ReadEffectiveDisableClientCertificates()
    {
      bool? booleanValue1 = ClientCertificateManager.GetBooleanValue(ClientCertificateManager.s_disableClientCertificatesName, StoreLocation.LocalMachine);
      bool? booleanValue2 = ClientCertificateManager.GetBooleanValue(ClientCertificateManager.s_disableClientCertificatesName, StoreLocation.CurrentUser);
      bool? nullable1 = booleanValue2;
      bool flag1 = true;
      if (!(nullable1.GetValueOrDefault() == flag1 & nullable1.HasValue))
      {
        bool? nullable2 = booleanValue1;
        bool flag2 = true;
        if (nullable2.GetValueOrDefault() == flag2 & nullable2.HasValue)
        {
          bool? nullable3 = booleanValue2;
          bool flag3 = false;
          if (!(nullable3.GetValueOrDefault() == flag3 & nullable3.HasValue))
            goto label_3;
        }
        return false;
      }
label_3:
      return true;
    }

    public static string[] ReadEffectiveSpecifiedCertificateThumbprints()
    {
      List<string> stringList = new List<string>();
      stringList.AddRange((IEnumerable<string>) ClientCertificateManager.SplitDelimitedString(Environment.GetEnvironmentVariable(ClientCertificateManager.s_certEnvironmentConfigVar)));
      stringList.AddRange((IEnumerable<string>) ClientCertificateManager.SplitDelimitedString(ConfigurationManager.AppSettings[ClientCertificateManager.s_certEnvironmentConfigVar]));
      stringList.AddRange((IEnumerable<string>) ClientCertificateManager.ReadSpecifiedCertificateThumbprints(StoreLocation.CurrentUser));
      stringList.AddRange((IEnumerable<string>) ClientCertificateManager.ReadSpecifiedCertificateThumbprints(StoreLocation.LocalMachine));
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
      string stringValue = ClientCertificateManager.GetStringValue(ClientCertificateManager.s_thumbprintsName, machineOrUserLevel);
      return stringValue == null ? Array.Empty<string>() : stringValue.Split(ClientCertificateManager.s_thumbprintDelimiters, StringSplitOptions.RemoveEmptyEntries);
    }

    public static void WriteDisableClientCertificates(
      bool disableClientCertificates,
      StoreLocation machineOrUserLevel)
    {
      ClientCertificateManager.WriteStringValue(ClientCertificateManager.s_disableClientCertificatesName, disableClientCertificates ? bool.TrueString : (string) null, machineOrUserLevel);
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
              stringBuilder.Append(ClientCertificateManager.s_thumbprintDelimiters[0]);
            stringBuilder.Append(thumbprint);
          }
        }
        str = stringBuilder.ToString();
      }
      ClientCertificateManager.WriteStringValue(ClientCertificateManager.s_thumbprintsName, str, machineOrUserLevel);
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
        if (ClientCertificateManager.ReadEffectiveDisableClientCertificates())
          return;
        X509Certificate2Enumerator enumerator1 = ClientCertificateManager.GetClientAuthCertificates(StoreLocation.LocalMachine).GetEnumerator();
        while (enumerator1.MoveNext())
        {
          X509Certificate2 current = enumerator1.Current;
          this.m_allClientCertificates[current.Thumbprint] = current;
        }
        X509Certificate2Enumerator enumerator2 = ClientCertificateManager.GetClientAuthCertificates(StoreLocation.CurrentUser).GetEnumerator();
        while (enumerator2.MoveNext())
        {
          X509Certificate2 current = enumerator2.Current;
          this.m_allClientCertificates[current.Thumbprint] = current;
        }
        string[] strArray = ClientCertificateManager.ReadEffectiveSpecifiedCertificateThumbprints();
        if (strArray.Length == 0)
        {
          foreach (X509Certificate2 x509Certificate2 in this.m_allClientCertificates.Values)
          {
            if (ClientCertificateManager.IsPrivateKeyAvailable(x509Certificate2, out int _))
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

    private static string[] SplitDelimitedString(string delimitedString) => delimitedString == null ? Array.Empty<string>() : delimitedString.Split(ClientCertificateManager.s_thumbprintDelimiters, StringSplitOptions.RemoveEmptyEntries);

    private X509Certificate2 GetClientCertificateByThumbprint(string thumbprint)
    {
      if (thumbprint == null)
        throw new ArgumentNullException(nameof (thumbprint));
      string message = (string) null;
      X509Certificate2 toCheck;
      if (this.m_allClientCertificates.TryGetValue(thumbprint, out toCheck))
      {
        int hr;
        if (ClientCertificateManager.IsPrivateKeyAvailable(toCheck, out hr))
          return toCheck;
        if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SCARD_E_NO_SMARTCARD == hr)
          message = ClientResources.SmartCardMissing((object) thumbprint);
        else if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.NTE_BAD_KEYSET == hr)
          message = ClientResources.ClientCertificateNoPermission((object) thumbprint);
      }
      if (message == null)
        message = ClientResources.ClientCertificateMissing((object) thumbprint);
      UIHost.WriteWarning(LogCategory.General, message);
      return (X509Certificate2) null;
    }

    private static bool CertificateSupportsClientAuth(X509Certificate2 certificate)
    {
      if (certificate == null)
        return false;
      bool flag = true;
      foreach (X509Extension extension in certificate.Extensions)
      {
        if (extension.Oid.Value == ClientCertificateManager.OID_EXTENDED_KEY_USAGE)
        {
          flag = false;
          foreach (Oid enhancedKeyUsage in ((X509EnhancedKeyUsageExtension) extension).EnhancedKeyUsages)
          {
            if (enhancedKeyUsage.Value.Equals(ClientCertificateManager.OID_PKIX_KP_CLIENT_AUTH) || enhancedKeyUsage.Value.Equals(ClientCertificateManager.OID_EXTENDED_KEY_USAGE_ANY))
              flag = true;
          }
        }
      }
      return flag;
    }

    private static bool IsPrivateKeyAvailable(X509Certificate2 toCheck, out int hr)
    {
      bool pfCallerFreeProvOrNCryptKey = true;
      uint dwKeySpec = 0;
      bool flag = false;
      hr = 0;
      try
      {
      }
      finally
      {
        SafeNCryptKeyHandle phCryptProvOrNCryptKey;
        if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.CryptAcquireCertificatePrivateKey(toCheck.Handle, Microsoft.TeamFoundation.Common.Internal.NativeMethods.CRYPT_ACQUIRE_SILENT_FLAG, IntPtr.Zero, out phCryptProvOrNCryptKey, out dwKeySpec, out pfCallerFreeProvOrNCryptKey))
        {
          hr = Marshal.GetLastWin32Error();
          if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.NTE_SILENT_CONTEXT == hr)
            flag = true;
        }
        else
        {
          if (pfCallerFreeProvOrNCryptKey)
            phCryptProvOrNCryptKey.Dispose();
          else
            phCryptProvOrNCryptKey.SetHandleAsInvalid();
          flag = true;
        }
      }
      return flag;
    }

    private static bool? GetBooleanValue(string name, StoreLocation machineOrUserLevel)
    {
      string stringValue = ClientCertificateManager.GetStringValue(name, machineOrUserLevel);
      if (stringValue != null)
      {
        if (string.Equals(stringValue, bool.TrueString, StringComparison.OrdinalIgnoreCase))
          return new bool?(true);
        if (string.Equals(stringValue, bool.FalseString, StringComparison.OrdinalIgnoreCase))
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
            registryKey1 = UIHost.TryGetUserRegistryRoot();
            break;
          case StoreLocation.LocalMachine:
            registryKey1 = UIHost.TryGetApplicationRegistryRoot();
            break;
        }
        if (registryKey1 == null)
          return (string) null;
        registryKey2 = registryKey1.OpenSubKey(ClientCertificateManager.s_clientCertsKeyName);
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
            registryKey1 = UIHost.UserRegistryRoot;
            break;
          case StoreLocation.LocalMachine:
            registryKey1 = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\VisualStudio\\17.0", true);
            break;
        }
        registryKey2 = registryKey1.OpenSubKey(ClientCertificateManager.s_clientCertsKeyName, true) ?? registryKey1.CreateSubKey(ClientCertificateManager.s_clientCertsKeyName);
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
  }
}
