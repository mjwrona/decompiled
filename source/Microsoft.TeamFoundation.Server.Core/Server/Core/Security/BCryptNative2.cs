// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Security.BCryptNative2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;

namespace Microsoft.TeamFoundation.Server.Core.Security
{
  internal static class BCryptNative2
  {
    private static volatile bool s_haveBcryptSupported;
    private static volatile bool s_bcryptSupported;

    internal static bool BCryptSupported
    {
      [SecuritySafeCritical] get
      {
        if (!BCryptNative2.s_haveBcryptSupported)
        {
          using (BCryptNative2.SafeLibraryHandle2 safeLibraryHandle2 = BCryptNative2.LoadLibraryEx("bcrypt", IntPtr.Zero, 0))
          {
            BCryptNative2.s_bcryptSupported = !safeLibraryHandle2.IsInvalid;
            BCryptNative2.s_haveBcryptSupported = true;
          }
        }
        return BCryptNative2.s_bcryptSupported;
      }
    }

    [SecurityCritical]
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern BCryptNative2.SafeLibraryHandle2 LoadLibraryEx(
      string libFilename,
      IntPtr reserved,
      int flags);

    [SecurityCritical]
    internal static int GetInt32Property<T>(T algorithm, string property) where T : SafeHandle => BitConverter.ToInt32(BCryptNative2.GetProperty<T>(algorithm, property), 0);

    [SecurityCritical]
    internal static byte[] GetProperty<T>(T algorithm, string property) where T : SafeHandle
    {
      BCryptNative2.BCryptPropertyGetter<T> bcryptPropertyGetter = (BCryptNative2.BCryptPropertyGetter<T>) null;
      if (typeof (T) == typeof (SafeBCryptAlgorithmHandle2))
        bcryptPropertyGetter = new BCryptNative2.BCryptPropertyGetter<SafeBCryptAlgorithmHandle2>(BCryptNative2.UnsafeNativeMethods.BCryptGetAlgorithmProperty) as BCryptNative2.BCryptPropertyGetter<T>;
      else if (typeof (T) == typeof (SafeBCryptHashHandle2))
        bcryptPropertyGetter = new BCryptNative2.BCryptPropertyGetter<SafeBCryptHashHandle2>(BCryptNative2.UnsafeNativeMethods.BCryptGetHashProperty) as BCryptNative2.BCryptPropertyGetter<T>;
      int pcbResult = 0;
      BCryptNative2.ErrorCode hr1 = bcryptPropertyGetter(algorithm, property, (byte[]) null, 0, ref pcbResult, 0);
      switch (hr1)
      {
        case BCryptNative2.ErrorCode.BufferToSmall:
        case BCryptNative2.ErrorCode.Success:
          byte[] pbOutput = new byte[pcbResult];
          BCryptNative2.ErrorCode hr2 = bcryptPropertyGetter(algorithm, property, pbOutput, pbOutput.Length, ref pcbResult, 0);
          if (hr2 != BCryptNative2.ErrorCode.Success)
            throw new CryptographicException((int) hr2);
          return pbOutput;
        default:
          throw new CryptographicException((int) hr1);
      }
    }

    [SecurityCritical]
    internal static SafeBCryptAlgorithmHandle2 OpenAlgorithm(
      string algorithm,
      string implementation)
    {
      SafeBCryptAlgorithmHandle2 phAlgorithm = (SafeBCryptAlgorithmHandle2) null;
      BCryptNative2.ErrorCode hr = BCryptNative2.UnsafeNativeMethods.BCryptOpenAlgorithmProvider(out phAlgorithm, algorithm, implementation, 0);
      if (hr != BCryptNative2.ErrorCode.Success)
        throw new CryptographicException((int) hr);
      return phAlgorithm;
    }

    [SecurityCritical(SecurityCriticalScope.Everything)]
    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    internal sealed class SafeLibraryHandle2 : SafeHandleZeroOrMinusOneIsInvalid
    {
      internal SafeLibraryHandle2()
        : base(true)
      {
      }

      protected override bool ReleaseHandle() => BCryptNative2.SafeLibraryHandle2.FreeLibrary(this.handle);

      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [SecurityCritical]
      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool FreeLibrary(IntPtr hModule);
    }

    internal enum ErrorCode
    {
      BufferToSmall = -1073741789, // 0xC0000023
      ObjectNameNotFound = -1073741772, // 0xC0000034
      Success = 0,
    }

    internal static class HashPropertyName2
    {
      public const string HashLength = "HashDigestLength";
    }

    internal static class ProviderName2
    {
      public const string MicrosoftPrimitiveProvider = "Microsoft Primitive Provider";
    }

    internal static class ObjectPropertyName
    {
      public const string ObjectLength = "ObjectLength";
    }

    [SecurityCritical(SecurityCriticalScope.Everything)]
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
      [DllImport("bcrypt.dll", CharSet = CharSet.Unicode)]
      internal static extern BCryptNative2.ErrorCode BCryptCreateHash(
        SafeBCryptAlgorithmHandle2 hAlgorithm,
        out SafeBCryptHashHandle2 phHash,
        IntPtr pbHashObject,
        int cbHashObject,
        IntPtr pbSecret,
        int cbSecret,
        int dwFlags);

      [DllImport("bcrypt.dll", EntryPoint = "BCryptGetProperty", CharSet = CharSet.Unicode)]
      internal static extern BCryptNative2.ErrorCode BCryptGetAlgorithmProperty(
        SafeBCryptAlgorithmHandle2 hObject,
        string pszProperty,
        [MarshalAs(UnmanagedType.LPArray), In, Out] byte[] pbOutput,
        int cbOutput,
        [In, Out] ref int pcbResult,
        int flags);

      [DllImport("bcrypt.dll", EntryPoint = "BCryptGetProperty", CharSet = CharSet.Unicode)]
      internal static extern BCryptNative2.ErrorCode BCryptGetHashProperty(
        SafeBCryptHashHandle2 hObject,
        string pszProperty,
        [MarshalAs(UnmanagedType.LPArray), In, Out] byte[] pbOutput,
        int cbOutput,
        [In, Out] ref int pcbResult,
        int flags);

      [DllImport("bcrypt.dll")]
      internal static extern BCryptNative2.ErrorCode BCryptFinishHash(
        SafeBCryptHashHandle2 hHash,
        [MarshalAs(UnmanagedType.LPArray), Out] byte[] pbInput,
        int cbInput,
        int dwFlags);

      [DllImport("bcrypt.dll")]
      internal static extern BCryptNative2.ErrorCode BCryptHashData(
        SafeBCryptHashHandle2 hHash,
        [MarshalAs(UnmanagedType.LPArray), In] byte[] pbInput,
        int cbInput,
        int dwFlags);

      [DllImport("bcrypt.dll", CharSet = CharSet.Unicode)]
      internal static extern BCryptNative2.ErrorCode BCryptOpenAlgorithmProvider(
        out SafeBCryptAlgorithmHandle2 phAlgorithm,
        string pszAlgId,
        string pszImplementation,
        int dwFlags);
    }

    [SecurityCritical(SecurityCriticalScope.Everything)]
    private delegate BCryptNative2.ErrorCode BCryptPropertyGetter<T>(
      T hObject,
      string pszProperty,
      byte[] pbOutput,
      int cbOutput,
      ref int pcbResult,
      int dwFlags)
      where T : SafeHandle;
  }
}
