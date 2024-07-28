// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SecureStringHMACSHA256Helper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class SecureStringHMACSHA256Helper : IComputeHash, IDisposable
  {
    private const uint SHA256HashOutputSizeInBytes = 32;
    private readonly SecureString key;
    private readonly int keyLength;
    private IntPtr algorithmHandle;

    public SecureStringHMACSHA256Helper(SecureString base64EncodedKey)
    {
      this.key = base64EncodedKey;
      this.keyLength = base64EncodedKey.Length;
      this.algorithmHandle = IntPtr.Zero;
      int error = SecureStringHMACSHA256Helper.NativeMethods.BCryptOpenAlgorithmProvider(out this.algorithmHandle, "SHA256", IntPtr.Zero, 8U);
      if (error != 0)
        throw new Win32Exception(error, "BCryptOpenAlgorithmProvider");
    }

    public SecureString Key => this.key;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~SecureStringHMACSHA256Helper() => this.Dispose(false);

    private void Dispose(bool disposing)
    {
      if (!(this.algorithmHandle != IntPtr.Zero))
        return;
      int num = SecureStringHMACSHA256Helper.NativeMethods.BCryptCloseAlgorithmProvider(this.algorithmHandle, 0U);
      if (num != 0)
        DefaultTrace.TraceError("Failed to close algorithm provider: {0}", (object) num);
      this.algorithmHandle = IntPtr.Zero;
    }

    public byte[] ComputeHash(ArraySegment<byte> bytesToHash)
    {
      IntPtr hashHandle = IntPtr.Zero;
      try
      {
        this.InitializeBCryptHash(this.key, this.keyLength, out hashHandle);
        this.AddData(hashHandle, bytesToHash);
        return this.FinishHash(hashHandle);
      }
      finally
      {
        if (hashHandle != IntPtr.Zero)
        {
          SecureStringHMACSHA256Helper.NativeMethods.BCryptDestroyHash(hashHandle);
          IntPtr zero = IntPtr.Zero;
        }
      }
    }

    private void AddData(IntPtr hashHandle, ArraySegment<byte> dataStream)
    {
      GCHandle gcHandle = GCHandle.Alloc((object) dataStream.Array, GCHandleType.Pinned);
      try
      {
        int error = SecureStringHMACSHA256Helper.NativeMethods.BCryptHashData(hashHandle, gcHandle.AddrOfPinnedObject(), (uint) dataStream.Count, 0U);
        if (error != 0)
          throw new Win32Exception(error, "BCryptHashData");
      }
      finally
      {
        gcHandle.Free();
      }
    }

    private byte[] FinishHash(IntPtr hashHandle)
    {
      byte[] numArray = new byte[32];
      GCHandle gcHandle = GCHandle.Alloc((object) numArray, GCHandleType.Pinned);
      try
      {
        int error = SecureStringHMACSHA256Helper.NativeMethods.BCryptFinishHash(hashHandle, gcHandle.AddrOfPinnedObject(), (uint) numArray.Length, 0U);
        if (error != 0)
          throw new Win32Exception(error, "BCryptFinishData");
      }
      finally
      {
        gcHandle.Free();
      }
      return numArray;
    }

    private void InitializeBCryptHash(
      SecureString base64EncodedPassword,
      int base64EncodedPasswordLength,
      out IntPtr hashHandle)
    {
      IntPtr bytes = IntPtr.Zero;
      uint bytesLength = 0;
      try
      {
        Base64Helper.SecureStringToNativeBytes(base64EncodedPassword, base64EncodedPasswordLength, out bytes, out bytesLength);
        int hash = SecureStringHMACSHA256Helper.NativeMethods.BCryptCreateHash(this.algorithmHandle, out hashHandle, IntPtr.Zero, 0U, bytes, bytesLength, 0U);
        if (hash != 0)
          throw new Win32Exception(hash, "BCryptCreateHash");
      }
      finally
      {
        if (bytes != IntPtr.Zero)
        {
          for (int ofs = 0; ofs < (int) bytesLength; ++ofs)
            Marshal.WriteByte(bytes, ofs, (byte) 0);
          Marshal.FreeCoTaskMem(bytes);
          IntPtr zero = IntPtr.Zero;
        }
      }
    }

    private static class NativeMethods
    {
      public const string BCRYPT_SHA256_ALGORITHM = "SHA256";
      public const uint BCRYPT_ALG_HANDLE_HMAC_FLAG = 8;

      [DllImport("Bcrypt.dll", CharSet = CharSet.Unicode)]
      public static extern int BCryptOpenAlgorithmProvider(
        out IntPtr algorithmHandle,
        string algorithmId,
        IntPtr implementation,
        uint flags);

      [DllImport("Bcrypt.dll", CharSet = CharSet.Unicode)]
      public static extern int BCryptCloseAlgorithmProvider(IntPtr algorithmHandle, uint flags);

      [DllImport("Bcrypt.dll", CharSet = CharSet.Unicode)]
      public static extern int BCryptCreateHash(
        IntPtr algorithmHandle,
        out IntPtr hashHandle,
        IntPtr workingSpace,
        uint workingSpaceSize,
        IntPtr keyBytes,
        uint keyBytesLength,
        uint flags);

      [DllImport("Bcrypt.dll", CharSet = CharSet.Unicode)]
      public static extern int BCryptDestroyHash(IntPtr hashHandle);

      [DllImport("Bcrypt.dll", CharSet = CharSet.Unicode)]
      public static extern int BCryptHashData(
        IntPtr hashHandle,
        IntPtr bytes,
        uint byteLength,
        uint flags);

      [DllImport("Bcrypt.dll", CharSet = CharSet.Unicode)]
      public static extern int BCryptFinishHash(
        IntPtr hashHandle,
        IntPtr byteOutputLocation,
        uint byteOutputLocationSize,
        uint flags);
    }
  }
}
