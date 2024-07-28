// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.ContainerListingDetails
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;

namespace Microsoft.Azure.Storage.Blob
{
  [Flags]
  public enum ContainerListingDetails
  {
    None = 0,
    Metadata = 1,
    All = Metadata, // 0x00000001
  }
}
