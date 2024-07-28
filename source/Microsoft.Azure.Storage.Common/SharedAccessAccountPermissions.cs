// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.SharedAccessAccountPermissions
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;

namespace Microsoft.Azure.Storage
{
  [Flags]
  public enum SharedAccessAccountPermissions
  {
    None = 0,
    Read = 1,
    Add = 2,
    Create = 4,
    Update = 8,
    ProcessMessages = 16, // 0x00000010
    Write = 32, // 0x00000020
    Delete = 64, // 0x00000040
    List = 128, // 0x00000080
  }
}
