// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Security.BCryptHashAlgorithm2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Server.Core.Security
{
  internal sealed class BCryptHashAlgorithm2 : IDisposable
  {
    [ThreadStatic]
    [SecurityCritical]
    private static BCryptAlgorithmHandleCache2 _algorithmCache;
    [SecurityCritical]
    private SafeBCryptAlgorithmHandle2 m_algorithmHandle;
    [SecurityCritical]
    private SafeBCryptHashHandle2 m_hashHandle;

    [SecuritySafeCritical]
    public BCryptHashAlgorithm2(CngAlgorithm algorithm, string implementation)
    {
      if (!BCryptNative2.BCryptSupported)
        throw new PlatformNotSupportedException("CNG is not supported on this platform");
      if (BCryptHashAlgorithm2._algorithmCache == null)
        BCryptHashAlgorithm2._algorithmCache = new BCryptAlgorithmHandleCache2();
      this.m_algorithmHandle = BCryptHashAlgorithm2._algorithmCache.GetCachedAlgorithmHandle(algorithm.Algorithm, implementation);
      this.Initialize();
    }

    [SecuritySafeCritical]
    public void Dispose()
    {
      if (this.m_hashHandle != null)
        this.m_hashHandle.Dispose();
      if (this.m_algorithmHandle == null)
        return;
      this.m_algorithmHandle = (SafeBCryptAlgorithmHandle2) null;
    }

    [SecuritySafeCritical]
    public void Initialize()
    {
      SafeBCryptHashHandle2 phHash = (SafeBCryptHashHandle2) null;
      IntPtr num = IntPtr.Zero;
      RuntimeHelpers.PrepareConstrainedRegions();
      try
      {
        int int32Property = BCryptNative2.GetInt32Property<SafeBCryptAlgorithmHandle2>(this.m_algorithmHandle, "ObjectLength");
        RuntimeHelpers.PrepareConstrainedRegions();
        try
        {
        }
        finally
        {
          num = Marshal.AllocCoTaskMem(int32Property);
        }
        BCryptNative2.ErrorCode hash = BCryptNative2.UnsafeNativeMethods.BCryptCreateHash(this.m_algorithmHandle, out phHash, num, int32Property, IntPtr.Zero, 0, 0);
        if (hash != BCryptNative2.ErrorCode.Success)
          throw new CryptographicException((int) hash);
      }
      finally
      {
        if (num != IntPtr.Zero)
        {
          if (phHash != null)
            phHash.HashObject = num;
          else
            Marshal.FreeCoTaskMem(num);
        }
      }
      if (this.m_hashHandle != null)
        this.m_hashHandle.Dispose();
      this.m_hashHandle = phHash;
    }

    [SecuritySafeCritical]
    public void HashCore(byte[] array, int ibStart, int cbSize)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (ibStart < 0 || ibStart > array.Length - cbSize)
        throw new ArgumentOutOfRangeException(nameof (ibStart));
      if (cbSize < 0 || cbSize > array.Length)
        throw new ArgumentOutOfRangeException(nameof (cbSize));
      using (ByteArray byteArray = new ByteArray(cbSize))
      {
        Buffer.BlockCopy((Array) array, ibStart, (Array) byteArray.Bytes, 0, cbSize);
        BCryptNative2.ErrorCode hr = BCryptNative2.UnsafeNativeMethods.BCryptHashData(this.m_hashHandle, byteArray.Bytes, cbSize, 0);
        if (hr != BCryptNative2.ErrorCode.Success)
          throw new CryptographicException((int) hr);
      }
    }

    [SecuritySafeCritical]
    public byte[] HashFinal()
    {
      byte[] pbInput = new byte[BCryptNative2.GetInt32Property<SafeBCryptHashHandle2>(this.m_hashHandle, "HashDigestLength")];
      BCryptNative2.ErrorCode hr = BCryptNative2.UnsafeNativeMethods.BCryptFinishHash(this.m_hashHandle, pbInput, pbInput.Length, 0);
      if (hr != BCryptNative2.ErrorCode.Success)
        throw new CryptographicException((int) hr);
      return pbInput;
    }

    [SecuritySafeCritical]
    public void HashStream(Stream stream)
    {
      using (ByteArray byteArray = new ByteArray(4096))
      {
        int cbSize;
        do
        {
          cbSize = stream.Read(byteArray.Bytes, 0, byteArray.Bytes.Length);
          if (cbSize > 0)
            this.HashCore(byteArray.Bytes, 0, cbSize);
        }
        while (cbSize > 0);
      }
    }
  }
}
