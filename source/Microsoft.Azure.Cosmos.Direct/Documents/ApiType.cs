// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ApiType
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
    GremlinV2 = 64, // 0x00000040
  }
}
