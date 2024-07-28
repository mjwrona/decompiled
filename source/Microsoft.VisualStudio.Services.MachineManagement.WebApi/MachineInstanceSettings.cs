// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineInstanceSettings
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class MachineInstanceSettings
  {
    private static Lazy<Regex> s_dcRegex = new Lazy<Regex>((Func<Regex>) (() => new Regex("^DC=(?<settings>(.+))$", RegexOptions.IgnoreCase | RegexOptions.Compiled)));
    private static Lazy<DataContractSerializer> s_serializer = new Lazy<DataContractSerializer>((Func<DataContractSerializer>) (() => new DataContractSerializer(typeof (IDictionary<string, object>))));

    public static void DeleteSettingsCertificateFromStore()
    {
      X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
      try
      {
        x509Store.Open(OpenFlags.ReadWrite);
        X509Certificate2Collection certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectName, (object) "MS.VSS.MachineManagement.Settings", false);
        if (certificate2Collection.Count <= 0)
          return;
        X509Certificate2Enumerator enumerator = certificate2Collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
          X509Certificate2 current = enumerator.Current;
          x509Store.Remove(current);
        }
      }
      finally
      {
        x509Store.Close();
      }
    }

    public static void SaveToCertificateStore(IDictionary<string, object> settings)
    {
      X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
      try
      {
        x509Store.Open(OpenFlags.ReadWrite);
        X509Certificate2 certificate = MachineInstanceSettings.SaveAsCertificate(settings);
        x509Store.Add(certificate);
        X509Certificate2Collection certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectName, (object) "MS.VSS.MachineManagement.Settings", false);
        if (certificate2Collection.Count <= 0)
          return;
        X509Certificate2Enumerator enumerator = certificate2Collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
          X509Certificate2 current = enumerator.Current;
          if (!current.Thumbprint.Equals(certificate.Thumbprint, StringComparison.OrdinalIgnoreCase))
            x509Store.Remove(current);
        }
      }
      finally
      {
        x509Store.Close();
      }
    }

    public static IDictionary<string, object> ReadFromCertificateStore()
    {
      X509Certificate2 certificate = (X509Certificate2) null;
      X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
      try
      {
        x509Store.Open(OpenFlags.ReadOnly);
        X509Certificate2Collection certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectName, (object) "MS.VSS.MachineManagement.Settings", false);
        certificate = certificate2Collection.Count != 0 ? certificate2Collection[0] : throw new FileNotFoundException("The required certificate was not found in the local machine store");
      }
      finally
      {
        x509Store.Close();
      }
      return MachineInstanceSettings.ReadFromCertificate(certificate);
    }

    public static IDictionary<string, object> ReadFromCertificate(X509Certificate2 certificate)
    {
      byte[] buffer = (byte[]) null;
      string str = certificate.SubjectName.Format(true);
      char[] separator = new char[2]{ '\r', '\n' };
      foreach (string input in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        Match match = MachineInstanceSettings.s_dcRegex.Value.Match(input);
        if (match.Success)
        {
          buffer = Convert.FromBase64String(match.Groups["settings"].Value.Trim('"'));
          break;
        }
      }
      if (buffer == null || buffer.Length == 0)
        return (IDictionary<string, object>) new Dictionary<string, object>();
      using (XmlDictionaryReader binaryReader = XmlDictionaryReader.CreateBinaryReader(buffer, XmlDictionaryReaderQuotas.Max))
        return (IDictionary<string, object>) MachineInstanceSettings.s_serializer.Value.ReadObject(binaryReader);
    }

    public static X509Certificate2 SaveAsCertificate(IDictionary<string, object> settings) => MachineInstanceSettings.SaveAsCertificate(settings, TimeSpan.FromDays(365.0));

    public static X509Certificate2 SaveAsCertificate(
      IDictionary<string, object> settings,
      TimeSpan expiresIn)
    {
      MachineInstanceSettings.SystemTime systemTime1 = MachineInstanceSettings.ToSystemTime(DateTime.UtcNow);
      MachineInstanceSettings.SystemTime systemTime2 = MachineInstanceSettings.ToSystemTime(DateTime.UtcNow.Add(expiresIn));
      string container = Guid.NewGuid().ToString();
      GCHandle gcHandle = new GCHandle();
      IntPtr providerContext = IntPtr.Zero;
      IntPtr cryptKeyHandle = IntPtr.Zero;
      IntPtr certificateContext = IntPtr.Zero;
      IntPtr certificateStoreHandle = IntPtr.Zero;
      IntPtr storeContextPtr = IntPtr.Zero;
      RuntimeHelpers.PrepareConstrainedRegions();
      byte[] numArray = (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter((Stream) memoryStream))
        {
          MachineInstanceSettings.s_serializer.Value.WriteObject(binaryWriter, (object) settings);
          binaryWriter.Flush();
          numArray = new byte[(int) memoryStream.Length];
          Array.Copy((Array) memoryStream.GetBuffer(), 0, (Array) numArray, 0, (int) memoryStream.Length);
        }
      }
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DC=\"{0}\"", (object) Convert.ToBase64String(numArray)) + "," + "CN=MS.VSS.MachineManagement.Settings";
      byte[] rawData;
      try
      {
        MachineInstanceSettings.Check(MachineInstanceSettings.NativeMethods.CryptAcquireContextW(out providerContext, container, (string) null, 1, 8));
        MachineInstanceSettings.Check(MachineInstanceSettings.NativeMethods.CryptGenKey(providerContext, 1, 1, out cryptKeyHandle));
        int encodedLength = 0;
        gcHandle = GCHandle.Alloc((object) str, GCHandleType.Pinned);
        IntPtr errorString;
        if (!MachineInstanceSettings.NativeMethods.CertStrToNameW(65537, gcHandle.AddrOfPinnedObject(), 3, IntPtr.Zero, (byte[]) null, ref encodedLength, out errorString))
        {
          Marshal.GetLastWin32Error();
          throw new ArgumentException(Marshal.PtrToStringUni(errorString));
        }
        byte[] encoded = new byte[encodedLength];
        if (!MachineInstanceSettings.NativeMethods.CertStrToNameW(65537, gcHandle.AddrOfPinnedObject(), 3, IntPtr.Zero, encoded, ref encodedLength, out errorString))
          throw new ArgumentException(Marshal.PtrToStringUni(errorString));
        gcHandle.Free();
        gcHandle = GCHandle.Alloc((object) encoded, GCHandleType.Pinned);
        MachineInstanceSettings.CryptoApiBlob subjectIssuerBlob = new MachineInstanceSettings.CryptoApiBlob(encoded.Length, gcHandle.AddrOfPinnedObject());
        MachineInstanceSettings.CryptKeyProviderInformation providerInformation = new MachineInstanceSettings.CryptKeyProviderInformation()
        {
          ContainerName = container,
          ProviderType = 1,
          KeySpec = 1
        };
        certificateContext = MachineInstanceSettings.NativeMethods.CertCreateSelfSignCertificate(providerContext, ref subjectIssuerBlob, 0, ref providerInformation, IntPtr.Zero, ref systemTime1, ref systemTime2, IntPtr.Zero);
        MachineInstanceSettings.Check(certificateContext != IntPtr.Zero);
        gcHandle.Free();
        certificateStoreHandle = MachineInstanceSettings.NativeMethods.CertOpenStore("Memory", 0, IntPtr.Zero, 8192, IntPtr.Zero);
        MachineInstanceSettings.Check(certificateStoreHandle != IntPtr.Zero);
        MachineInstanceSettings.Check(MachineInstanceSettings.NativeMethods.CertAddCertificateContextToStore(certificateStoreHandle, certificateContext, 1, out storeContextPtr));
        MachineInstanceSettings.NativeMethods.CertSetCertificateContextProperty(storeContextPtr, 2, 0, ref providerInformation);
        MachineInstanceSettings.CryptoApiBlob pfxBlob = new MachineInstanceSettings.CryptoApiBlob();
        MachineInstanceSettings.Check(MachineInstanceSettings.NativeMethods.PFXExportCertStoreEx(certificateStoreHandle, ref pfxBlob, IntPtr.Zero, IntPtr.Zero, 7));
        rawData = new byte[pfxBlob.DataLength];
        gcHandle = GCHandle.Alloc((object) rawData, GCHandleType.Pinned);
        pfxBlob.Data = gcHandle.AddrOfPinnedObject();
        MachineInstanceSettings.Check(MachineInstanceSettings.NativeMethods.PFXExportCertStoreEx(certificateStoreHandle, ref pfxBlob, IntPtr.Zero, IntPtr.Zero, 7));
        gcHandle.Free();
      }
      finally
      {
        if (gcHandle.IsAllocated)
          gcHandle.Free();
        if (certificateContext != IntPtr.Zero)
          MachineInstanceSettings.NativeMethods.CertFreeCertificateContext(certificateContext);
        if (storeContextPtr != IntPtr.Zero)
          MachineInstanceSettings.NativeMethods.CertFreeCertificateContext(storeContextPtr);
        if (certificateStoreHandle != IntPtr.Zero)
          MachineInstanceSettings.NativeMethods.CertCloseStore(certificateStoreHandle, 0);
        if (cryptKeyHandle != IntPtr.Zero)
          MachineInstanceSettings.NativeMethods.CryptDestroyKey(cryptKeyHandle);
        if (providerContext != IntPtr.Zero)
        {
          MachineInstanceSettings.NativeMethods.CryptReleaseContext(providerContext, 0);
          MachineInstanceSettings.NativeMethods.CryptAcquireContextW(out providerContext, container, (string) null, 1, 16);
        }
      }
      return new X509Certificate2(rawData, string.Empty, X509KeyStorageFlags.Exportable);
    }

    private static MachineInstanceSettings.SystemTime ToSystemTime(DateTime dateTime)
    {
      long fileTime = dateTime.ToFileTime();
      MachineInstanceSettings.SystemTime systemTime;
      MachineInstanceSettings.Check(MachineInstanceSettings.NativeMethods.FileTimeToSystemTime(ref fileTime, out systemTime));
      return systemTime;
    }

    private static void Check(bool nativeCallSucceeded)
    {
      if (nativeCallSucceeded)
        return;
      Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
    }

    private struct SystemTime
    {
      public short Year;
      public short Month;
      public short DayOfWeek;
      public short Day;
      public short Hour;
      public short Minute;
      public short Second;
      public short Milliseconds;
    }

    private struct CryptoApiBlob
    {
      public int DataLength;
      public IntPtr Data;

      public CryptoApiBlob(int dataLength, IntPtr data)
      {
        this.DataLength = dataLength;
        this.Data = data;
      }
    }

    private struct CryptKeyProviderInformation
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string ContainerName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string ProviderName;
      public int ProviderType;
      public int Flags;
      public int ProviderParameterCount;
      public IntPtr ProviderParameters;
      public int KeySpec;
    }

    private static class NativeMethods
    {
      [DllImport("kernel32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool FileTimeToSystemTime(
        [In] ref long fileTime,
        out MachineInstanceSettings.SystemTime systemTime);

      [DllImport("AdvApi32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CryptAcquireContextW(
        out IntPtr providerContext,
        [MarshalAs(UnmanagedType.LPWStr)] string container,
        [MarshalAs(UnmanagedType.LPWStr)] string provider,
        int providerType,
        int flags);

      [DllImport("AdvApi32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CryptReleaseContext(IntPtr providerContext, int flags);

      [DllImport("AdvApi32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CryptGenKey(
        IntPtr providerContext,
        int algorithmId,
        int flags,
        out IntPtr cryptKeyHandle);

      [DllImport("AdvApi32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CryptDestroyKey(IntPtr cryptKeyHandle);

      [DllImport("Crypt32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CertStrToNameW(
        int certificateEncodingType,
        IntPtr x500,
        int strType,
        IntPtr reserved,
        [MarshalAs(UnmanagedType.LPArray), Out] byte[] encoded,
        ref int encodedLength,
        out IntPtr errorString);

      [DllImport("Crypt32.dll", SetLastError = true)]
      public static extern IntPtr CertCreateSelfSignCertificate(
        IntPtr providerHandle,
        [In] ref MachineInstanceSettings.CryptoApiBlob subjectIssuerBlob,
        int flags,
        [In] ref MachineInstanceSettings.CryptKeyProviderInformation keyProviderInformation,
        IntPtr signatureAlgorithm,
        [In] ref MachineInstanceSettings.SystemTime startTime,
        [In] ref MachineInstanceSettings.SystemTime endTime,
        IntPtr extensions);

      [DllImport("Crypt32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CertFreeCertificateContext(IntPtr certificateContext);

      [DllImport("Crypt32.dll", SetLastError = true)]
      public static extern IntPtr CertOpenStore(
        [MarshalAs(UnmanagedType.LPStr)] string storeProvider,
        int messageAndCertificateEncodingType,
        IntPtr cryptProvHandle,
        int flags,
        IntPtr parameters);

      [DllImport("Crypt32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CertCloseStore(IntPtr certificateStoreHandle, int flags);

      [DllImport("Crypt32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CertAddCertificateContextToStore(
        IntPtr certificateStoreHandle,
        IntPtr certificateContext,
        int addDisposition,
        out IntPtr storeContextPtr);

      [DllImport("Crypt32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CertSetCertificateContextProperty(
        IntPtr certificateContext,
        int propertyId,
        int flags,
        [In] ref MachineInstanceSettings.CryptKeyProviderInformation data);

      [DllImport("Crypt32.dll", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool PFXExportCertStoreEx(
        IntPtr certificateStoreHandle,
        ref MachineInstanceSettings.CryptoApiBlob pfxBlob,
        IntPtr password,
        IntPtr reserved,
        int flags);
    }
  }
}
