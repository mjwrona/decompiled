// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.SharedAccessBlobPermissions
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;

namespace Microsoft.Azure.Storage.Blob
{
  [Flags]
  public enum SharedAccessBlobPermissions
  {
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    List = 8,
    Add = 16, // 0x00000010
    Create = 32, // 0x00000020
  }
}
