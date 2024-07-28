// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.TeamFoundationJobResult
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Framework.Common
{
  public enum TeamFoundationJobResult
  {
    None = -1, // 0xFFFFFFFF
    Succeeded = 0,
    PartiallySucceeded = 1,
    Failed = 2,
    Stopped = 3,
    Killed = 4,
    Blocked = 5,
    ExtensionNotFound = 6,
    Inactive = 7,
    Disabled = 8,
    JobInitializationError = 9,
    BlockedByUpgrade = 10, // 0x0000000A
    HostShutdown = 11, // 0x0000000B
    HostNotFound = 12, // 0x0000000C
    JobDefinitionNotFound = 13, // 0x0000000D
    Last = 14, // 0x0000000E
  }
}
