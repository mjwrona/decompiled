// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ScheduleDays
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public enum ScheduleDays
  {
    None = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 4,
    Thursday = 8,
    Friday = 16, // 0x00000010
    Saturday = 32, // 0x00000020
    Sunday = 64, // 0x00000040
    All = 127, // 0x0000007F
  }
}
