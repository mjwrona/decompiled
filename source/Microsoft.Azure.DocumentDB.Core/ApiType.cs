// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ApiType
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents
{
  [Flags]
  internal enum ApiType
  {
    None = 0,
    MongoDB = 1,
    Gremlin = 2,
    Cassandra = 4,
    Table = 8,
    Sql = 16, // 0x00000010
    Etcd = 32, // 0x00000020
  }
}
