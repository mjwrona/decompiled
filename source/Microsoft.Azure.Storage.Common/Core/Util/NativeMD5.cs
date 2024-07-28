// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.NativeMD5
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal sealed class NativeMD5 : MD5
  {
    private const uint ProvRsaFull = 1;
    private const uint CryptVerifyContext = 4026531840;
    private const uint CalgMD5 = 32771;
    private const uint HashVal = 2;
    private IntPtr hashHandle;
    private IntPtr hashProv;
    private bool disposed;

    public NativeMD5()
    {
      NativeMD5.ValidateReturnCode(NativeMethods.CryptAcquireContextW(out this.hashProv, (string) null, (string) null, 1U, 4026531840U));
      this.Initialize();
    }

    ~NativeMD5() => this.Dispose(false);

    public override void Initialize()
    {
      if (this.hashHandle != IntPtr.Zero)
      {
        NativeMethods.CryptDestroyHash(this.hashHandle);
        this.hashHandle = IntPtr.Zero;
      }
      NativeMD5.ValidateReturnCode(NativeMethods.CryptCreateHash(this.hashProv, 32771U, IntPtr.Zero, 0U, out this.hashHandle));
    }

    protected override void HashCore(byte[] array, int offset, int dataLen)
    {
      GCHandle gcHandle = GCHandle.Alloc((object) array, GCHandleType.Pinned);
      try
      {
        IntPtr num = gcHandle.AddrOfPinnedObject();
        if (offset != 0)
          num = IntPtr.Add(num, offset);
        NativeMD5.ValidateReturnCode(NativeMethods.CryptHashData(this.hashHandle, num, dataLen, 0U));
      }
      finally
      {
        gcHandle.Free();
      }
    }

    protected override byte[] HashFinal()
    {
      byte[] data = new byte[16];
      int length = data.Length;
      NativeMD5.ValidateReturnCode(NativeMethods.CryptGetHashParam(this.hashHandle, 2U, data, ref length, 0U));
      return data;
    }

    protected override void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (this.hashHandle != IntPtr.Zero)
        {
          NativeMethods.CryptDestroyHash(this.hashHandle);
          this.hashHandle = IntPtr.Zero;
        }
        if (this.hashProv != IntPtr.Zero)
        {
          NativeMethods.CryptReleaseContext(this.hashProv, 0);
          this.hashProv = IntPtr.Zero;
        }
        this.disposed = true;
      }
      base.Dispose(disposing);
    }

    private static void ValidateReturnCode(bool status)
    {
      if (!status)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Crypto function failed with error code '{0}'", (object) Marshal.GetLastWin32Error()));
    }
  }
}
