// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.ChecksumRequested
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal struct ChecksumRequested
  {
    public static ChecksumRequested None => new ChecksumRequested();

    public ChecksumRequested(bool md5, bool crc64)
    {
      this.MD5 = md5;
      this.CRC64 = crc64;
    }

    public bool MD5 { get; }

    public bool CRC64 { get; }

    internal bool HasAny => this.MD5 || this.CRC64;

    internal void AssertInBounds(long? offset, long? count, int maxMd5 = 2147483647, int maxCrc64 = 2147483647)
    {
      if (!offset.HasValue || !this.HasAny)
        return;
      CommonUtility.AssertNotNull(nameof (count), (object) count);
      CommonUtility.AssertInBounds<long>(nameof (count), count.Value, 1L, this.MD5 ? (long) maxMd5 : (long) int.MaxValue);
      CommonUtility.AssertInBounds<long>(nameof (count), count.Value, 1L, this.CRC64 ? (long) maxCrc64 : (long) int.MaxValue);
    }
  }
}
