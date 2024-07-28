// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlAzureResource
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal enum SqlAzureResource
  {
    PhysicalDatabaseSpace = 0,
    PhysicalLogSpace = 2,
    LogWriteIODelay = 4,
    DataReadIODelay = 6,
    CPU = 8,
    DatabaseSize = 10, // 0x0000000A
    Internal = 12, // 0x0000000C
    SqlWorkerThreads = 14, // 0x0000000E
  }
}
