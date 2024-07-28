// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildStatus
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public enum BuildStatus
  {
    [EnumMember] None = 0,
    [EnumMember] InProgress = 1,
    [EnumMember] Succeeded = 2,
    [EnumMember] PartiallySucceeded = 4,
    [EnumMember] Failed = 8,
    [EnumMember] Stopped = 16, // 0x00000010
    [EnumMember] NotStarted = 32, // 0x00000020
    [EnumMember] All = 63, // 0x0000003F
  }
}
