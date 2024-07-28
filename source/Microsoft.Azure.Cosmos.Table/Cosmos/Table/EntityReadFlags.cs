// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.EntityReadFlags
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Table
{
  [Flags]
  internal enum EntityReadFlags
  {
    PartitionKey = 1,
    RowKey = 2,
    Timestamp = 4,
    Etag = 8,
    Properties = 16, // 0x00000010
    All = Properties | Etag | Timestamp | RowKey | PartitionKey, // 0x0000001F
  }
}
