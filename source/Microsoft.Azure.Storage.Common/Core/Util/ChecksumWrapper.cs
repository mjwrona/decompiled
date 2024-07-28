// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.ChecksumWrapper
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class ChecksumWrapper : IDisposable
  {
    private bool disposed;

    public Crc64Wrapper CRC64 { get; set; }

    public MD5Wrapper MD5 { get; set; }

    internal ChecksumWrapper(bool calcMd5 = true, bool calcCrc64 = true)
    {
      if (calcCrc64)
        this.CRC64 = new Crc64Wrapper();
      if (!calcMd5)
        return;
      this.MD5 = new MD5Wrapper();
    }

    internal void UpdateHash(byte[] input, int offset, int count)
    {
      if (this.CRC64 != null)
        this.CRC64.UpdateHash(input, offset, count);
      if (this.MD5 == null)
        return;
      this.MD5.UpdateHash(input, offset, count);
    }

    public bool HasAny => this.MD5 != null || this.CRC64 != null;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing)
      {
        if (this.CRC64 != null)
          this.CRC64.Dispose();
        if (this.MD5 != null)
          this.MD5.Dispose();
      }
      this.disposed = true;
    }
  }
}
