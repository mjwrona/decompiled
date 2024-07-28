// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.Crc64Wrapper
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class Crc64Wrapper : IDisposable
  {
    private ulong uCRC;

    internal Crc64Wrapper()
    {
    }

    internal void UpdateHash(byte[] input, int offset, int count)
    {
      if (offset != 0)
        throw new NotImplementedException("non-zero offset for Crc64Wrapper update not supported");
      if (count <= 0)
        return;
      this.uCRC = Crc64.ComputeSlicedSafe(input, count, this.uCRC);
    }

    internal string ComputeHash() => Convert.ToBase64String(BitConverter.GetBytes(this.uCRC));

    public void Dispose()
    {
    }
  }
}
