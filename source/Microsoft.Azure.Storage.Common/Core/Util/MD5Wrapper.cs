// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.MD5Wrapper
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Security.Cryptography;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class MD5Wrapper : IDisposable
  {
    private readonly bool version1MD5 = CloudStorageAccount.UseV1MD5;
    private MD5 hash;

    internal MD5Wrapper() => this.hash = this.version1MD5 ? MD5.Create() : (MD5) new NativeMD5();

    internal void UpdateHash(byte[] input, int offset, int count)
    {
      if (count <= 0)
        return;
      this.hash.TransformBlock(input, offset, count, (byte[]) null, 0);
    }

    internal string ComputeHash()
    {
      this.hash.TransformFinalBlock(new byte[0], 0, 0);
      return Convert.ToBase64String(this.hash.Hash);
    }

    public void Dispose()
    {
      if (this.hash == null)
        return;
      this.hash.Dispose();
      this.hash = (MD5) null;
    }
  }
}
