// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobListingDetails
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;

namespace Microsoft.Azure.Storage.Blob
{
  [Flags]
  public enum BlobListingDetails
  {
    None = 0,
    Snapshots = 1,
    Metadata = 2,
    UncommittedBlobs = 4,
    Copy = 8,
    Deleted = 16, // 0x00000010
    All = Deleted | Copy | UncommittedBlobs | Metadata | Snapshots, // 0x0000001F
  }
}
