// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.CertHelper
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CertHelper
  {
    private const string RsaSha1Oid = "1.2.840.113549.1.1.11";

    public static X509Certificate2 CreateCertificate(
      X500DistinguishedName subjectName,
      string friendlyName)
    {
      X509Certificate2 signedCertificate = CertHelper.CreateSelfSignedCertificate(CertHelper.Create2048RsaKey(), subjectName);
      signedCertificate.FriendlyName = friendlyName;
      return signedCertificate;
    }

    public static X500DistinguishedName CreateContinuousDeploymentDistinguishedName() => new X500DistinguishedName("CN=Windows Azure Tools");

    public static string CreateContinuousDeploymentCertificateFriendlyName() => string.Format("Tfs-ContinuousDeployment{0}-credentials", (object) DateTime.Now.ToString("M-d-yyyy", (IFormatProvider) CultureInfo.InvariantCulture));

    private static CngKey Create2048RsaKey()
    {
      CngKeyCreationParameters creationParameters = new CngKeyCreationParameters()
      {
        ExportPolicy = new CngExportPolicies?(CngExportPolicies.AllowExport),
        KeyCreationOptions = CngKeyCreationOptions.None,
        KeyUsage = new CngKeyUsages?(CngKeyUsages.AllUsages),
        Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider
      };
      creationParameters.Parameters.Add(new CngProperty("Length", BitConverter.GetBytes(2048), CngPropertyOptions.None));
      return CngKey.Create(new CngAlgorithm("RSA"), (string) null, creationParameters);
    }

    private static X509Certificate2 CreateSelfSignedCertificate(
      CngKey key,
      X500DistinguishedName subjectName)
    {
      using (CertHelper.SafeCertContextHandle signedCertificate1 = CertHelper.CreateSelfSignedCertificate(key, true, subjectName.RawData, CertHelper.X509CertificateCreationOptions.None, "1.2.840.113549.1.1.11", DateTime.UtcNow, DateTime.UtcNow.AddYears(1)))
      {
        X509Certificate2 signedCertificate2 = (X509Certificate2) null;
        bool success = false;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
          signedCertificate1.DangerousAddRef(ref success);
          signedCertificate2 = new X509Certificate2(signedCertificate1.DangerousGetHandle());
        }
        finally
        {
          if (success)
            signedCertificate1.DangerousRelease();
        }
        key.Dispose();
        return signedCertificate2;
      }
    }

    [SecurityCritical]
    private static CertHelper.SafeCertContextHandle CreateSelfSignedCertificate(
      CngKey key,
      bool takeOwnershipOfKey,
      byte[] subjectName,
      CertHelper.X509CertificateCreationOptions creationOptions,
      string signatureAlgorithmOid,
      DateTime startTime,
      DateTime endTime)
    {
      CertHelper.CRYPT_ALGORITHM_IDENTIFIER pSignatureAlgorithm = new CertHelper.CRYPT_ALGORITHM_IDENTIFIER()
      {
        pszObjId = signatureAlgorithmOid,
        Parameters = new CertHelper.CRYPTOAPI_BLOB()
      };
      pSignatureAlgorithm.Parameters.cbData = 0;
      pSignatureAlgorithm.Parameters.pbData = IntPtr.Zero;
      CertHelper.SYSTEMTIME pStartTime = new CertHelper.SYSTEMTIME(startTime);
      CertHelper.SYSTEMTIME pEndTime = new CertHelper.SYSTEMTIME(endTime);
      CertHelper.CERT_EXTENSIONS pExtensions = new CertHelper.CERT_EXTENSIONS();
      pExtensions.cExtension = 0;
      CertHelper.CRYPT_KEY_PROV_INFO pKeyProvInfo = new CertHelper.CRYPT_KEY_PROV_INFO();
      pKeyProvInfo.pwszContainerName = key.UniqueName;
      pKeyProvInfo.pwszProvName = key.Provider.Provider;
      pKeyProvInfo.dwProvType = 0;
      pKeyProvInfo.dwFlags = 0;
      pKeyProvInfo.cProvParam = 0;
      pKeyProvInfo.rgProvParam = IntPtr.Zero;
      pKeyProvInfo.dwKeySpec = 0;
      CertHelper.SafeCertContextHandle pCertContext = (CertHelper.SafeCertContextHandle) null;
      GCHandle gcHandle = GCHandle.Alloc((object) subjectName, GCHandleType.Pinned);
      CertHelper.CRYPTOAPI_BLOB pSubjectIssuerBlob = new CertHelper.CRYPTOAPI_BLOB();
      pSubjectIssuerBlob.cbData = subjectName.Length;
      pSubjectIssuerBlob.pbData = gcHandle.AddrOfPinnedObject();
      using (SafeNCryptKeyHandle handle = key.Handle)
      {
        pCertContext = CertHelper.CertCreateSelfSignCertificate(handle, ref pSubjectIssuerBlob, creationOptions, ref pKeyProvInfo, ref pSignatureAlgorithm, ref pStartTime, ref pEndTime, ref pExtensions);
        gcHandle.Free();
        if (pCertContext.IsInvalid)
          throw new CryptographicException(Marshal.GetLastWin32Error());
      }
      using (SafeNCryptKeyHandle handle = key.Handle)
      {
        CertHelper.CERT_KEY_CONTEXT pvData = new CertHelper.CERT_KEY_CONTEXT();
        pvData.cbSize = Marshal.SizeOf(typeof (CertHelper.CERT_KEY_CONTEXT));
        pvData.hNCryptKey = handle.DangerousGetHandle();
        pvData.dwKeySpec = CertHelper.KeySpec.NCryptKey;
        bool flag = false;
        int hr = 0;
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          CertHelper.CertificatePropertySetFlags dwFlags = CertHelper.CertificatePropertySetFlags.None;
          if (!takeOwnershipOfKey)
            dwFlags |= CertHelper.CertificatePropertySetFlags.NoCryptRelease;
          flag = CertHelper.CertSetCertificateContextProperty(pCertContext, CertHelper.CertificateProperty.KeyContext, dwFlags, ref pvData);
          hr = Marshal.GetLastWin32Error();
          if (flag & takeOwnershipOfKey)
            handle.SetHandleAsInvalid();
        }
        if (!flag)
          throw new CryptographicException(hr);
      }
      return pCertContext;
    }

    [DllImport("crypt32.dll", SetLastError = true)]
    private static extern CertHelper.SafeCertContextHandle CertCreateSelfSignCertificate(
      SafeNCryptKeyHandle hCryptProvOrNCryptKey,
      [In] ref CertHelper.CRYPTOAPI_BLOB pSubjectIssuerBlob,
      CertHelper.X509CertificateCreationOptions dwFlags,
      [In] ref CertHelper.CRYPT_KEY_PROV_INFO pKeyProvInfo,
      [In] ref CertHelper.CRYPT_ALGORITHM_IDENTIFIER pSignatureAlgorithm,
      [In] ref CertHelper.SYSTEMTIME pStartTime,
      [In] ref CertHelper.SYSTEMTIME pEndTime,
      [In] ref CertHelper.CERT_EXTENSIONS pExtensions);

    [DllImport("crypt32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CertSetCertificateContextProperty(
      CertHelper.SafeCertContextHandle pCertContext,
      CertHelper.CertificateProperty dwPropId,
      CertHelper.CertificatePropertySetFlags dwFlags,
      [In] ref CertHelper.CERT_KEY_CONTEXT pvData);

    [SecurityCritical]
    private sealed class SafeCertContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      private SafeCertContextHandle()
        : base(true)
      {
      }

      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [SuppressUnmanagedCodeSecurity]
      [DllImport("crypt32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool CertFreeCertificateContext(IntPtr pCertContext);

      protected override bool ReleaseHandle() => CertHelper.SafeCertContextHandle.CertFreeCertificateContext(this.handle);
    }

    private struct CRYPT_ALGORITHM_IDENTIFIER
    {
      [MarshalAs(UnmanagedType.LPStr)]
      internal string pszObjId;
      internal CertHelper.CRYPTOAPI_BLOB Parameters;
    }

    private struct CRYPTOAPI_BLOB
    {
      internal int cbData;
      internal IntPtr pbData;
    }

    private struct SYSTEMTIME
    {
      internal ushort wYear;
      internal ushort wMonth;
      internal ushort wDayOfWeek;
      internal ushort wDay;
      internal ushort wHour;
      internal ushort wMinute;
      internal ushort wSecond;
      internal ushort wMilliseconds;

      internal SYSTEMTIME(DateTime time)
      {
        this.wYear = (ushort) time.Year;
        this.wMonth = (ushort) time.Month;
        this.wDayOfWeek = (ushort) time.DayOfWeek;
        this.wDay = (ushort) time.Day;
        this.wHour = (ushort) time.Hour;
        this.wMinute = (ushort) time.Minute;
        this.wSecond = (ushort) time.Second;
        this.wMilliseconds = (ushort) time.Millisecond;
      }
    }

    private struct CERT_EXTENSIONS
    {
      internal int cExtension;
      internal IntPtr rgExtension;
    }

    private struct CERT_EXTENSION
    {
      [MarshalAs(UnmanagedType.LPStr)]
      internal string pszObjId;
      [MarshalAs(UnmanagedType.Bool)]
      internal bool fCritical;
      internal CertHelper.CRYPTOAPI_BLOB Value;
    }

    private struct CRYPT_KEY_PROV_INFO
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      internal string pwszContainerName;
      [MarshalAs(UnmanagedType.LPWStr)]
      internal string pwszProvName;
      internal int dwProvType;
      internal int dwFlags;
      internal int cProvParam;
      internal IntPtr rgProvParam;
      internal int dwKeySpec;
    }

    private struct CERT_KEY_CONTEXT
    {
      internal int cbSize;
      internal IntPtr hNCryptKey;
      internal CertHelper.KeySpec dwKeySpec;
    }

    private enum KeySpec
    {
      NCryptKey = -1, // 0xFFFFFFFF
    }

    [Flags]
    private enum CertificatePropertySetFlags
    {
      None = 0,
      NoCryptRelease = 1,
    }

    [Flags]
    private enum X509CertificateCreationOptions
    {
      None = 0,
    }

    private enum CertificateProperty
    {
      KeyProviderInfo = 2,
      KeyContext = 5,
    }
  }
}
