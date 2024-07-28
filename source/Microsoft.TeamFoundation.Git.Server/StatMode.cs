// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.StatMode
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  [Flags]
  public enum StatMode
  {
    S_IFMT = 61440, // 0x0000F000
    S_IFIFO = 4096, // 0x00001000
    S_IFCHR = 8192, // 0x00002000
    S_IFDIR = 16384, // 0x00004000
    S_IFBLK = S_IFDIR | S_IFCHR, // 0x00006000
    S_IFREG = 32768, // 0x00008000
    S_IFLNK = S_IFREG | S_IFCHR, // 0x0000A000
    S_IFSOCK = S_IFREG | S_IFDIR, // 0x0000C000
    S_644 = 420, // 0x000001A4
    S_755 = 493, // 0x000001ED
    S_777 = 511, // 0x000001FF
  }
}
