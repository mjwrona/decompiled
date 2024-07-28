// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.GitPackFeature
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  [Flags]
  internal enum GitPackFeature
  {
    None = 0,
    MultiAck = 1,
    ThinPack = 2,
    SideBand = 4,
    SideBand64K = 8,
    OfsDelta = 16, // 0x00000010
    Shallow = 32, // 0x00000020
    NoProgress = 64, // 0x00000040
    IncludeTag = 128, // 0x00000080
    MultiAckDetailed = 256, // 0x00000100
    NoDone = 512, // 0x00000200
    ReportStatus = 1024, // 0x00000400
    DeleteRefs = 2048, // 0x00000800
    Quiet = 4096, // 0x00001000
  }
}
