// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildReason
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public enum BuildReason
  {
    None = 0,
    Manual = 1,
    IndividualCI = 2,
    BatchedCI = 4,
    Schedule = 8,
    UserCreated = 32, // 0x00000020
    ValidateShelveset = 64, // 0x00000040
    CheckInShelveset = 128, // 0x00000080
    PullRequest = 256, // 0x00000100
    BuildCompletion = 512, // 0x00000200
    ResourceTrigger = 1024, // 0x00000400
    Triggered = 1967, // 0x000007AF
    All = 2031, // 0x000007EF
  }
}
