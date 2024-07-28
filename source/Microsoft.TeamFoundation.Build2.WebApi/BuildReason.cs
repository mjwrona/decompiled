// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildReason
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public enum BuildReason
  {
    [EnumMember] None = 0,
    [EnumMember] Manual = 1,
    [EnumMember] IndividualCI = 2,
    [EnumMember] BatchedCI = 4,
    [EnumMember] Schedule = 8,
    [EnumMember] ScheduleForced = 16, // 0x00000010
    [EnumMember] UserCreated = 32, // 0x00000020
    [EnumMember] ValidateShelveset = 64, // 0x00000040
    [EnumMember] CheckInShelveset = 128, // 0x00000080
    [EnumMember] PullRequest = 256, // 0x00000100
    [EnumMember] BuildCompletion = 512, // 0x00000200
    [EnumMember] ResourceTrigger = 1024, // 0x00000400
    [EnumMember] Triggered = 1967, // 0x000007AF
    [EnumMember] All = 2031, // 0x000007EF
  }
}
