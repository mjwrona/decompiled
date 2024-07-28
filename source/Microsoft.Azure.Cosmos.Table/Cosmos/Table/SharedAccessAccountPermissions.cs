// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.SharedAccessAccountPermissions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Table
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
