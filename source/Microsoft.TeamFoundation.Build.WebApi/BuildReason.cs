// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildReason
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

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
    [EnumMember] Triggered = 191, // 0x000000BF
    [EnumMember] All = 255, // 0x000000FF
    [EnumMember] ResourceTrigger = 512, // 0x00000200
  }
}
