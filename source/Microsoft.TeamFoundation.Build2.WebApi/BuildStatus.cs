// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildStatus
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public enum BuildStatus
  {
    [EnumMember] None = 0,
    [EnumMember] InProgress = 1,
    [EnumMember] Completed = 2,
    [EnumMember] Cancelling = 4,
    [EnumMember] Postponed = 8,
    [EnumMember] NotStarted = 32, // 0x00000020
    [EnumMember] All = 47, // 0x0000002F
  }
}
