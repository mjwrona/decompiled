// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.ChecksumOptions
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  public sealed class ChecksumOptions
  {
    public bool? DisableContentMD5Validation { get; set; }

    public bool? StoreContentMD5 { get; set; }

    public bool? UseTransactionalMD5 { get; set; }

    public bool? DisableContentCRC64Validation { get; set; }

    internal bool? StoreContentCRC64
    {
      get => new bool?(false);
      set
      {
      }
    }

    public bool? UseTransactionalCRC64 { get; set; }

    internal void CopyFrom(ChecksumOptions other)
    {
      this.DisableContentMD5Validation = other.DisableContentMD5Validation;
      this.StoreContentMD5 = other.StoreContentMD5;
      this.UseTransactionalMD5 = other.UseTransactionalMD5;
      this.DisableContentCRC64Validation = other.DisableContentCRC64Validation;
      this.StoreContentCRC64 = other.StoreContentCRC64;
      this.UseTransactionalCRC64 = other.UseTransactionalCRC64;
    }
  }
}
