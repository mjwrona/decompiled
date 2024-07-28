// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.ScheduleDays
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public enum ScheduleDays
  {
    [EnumMember] None = 0,
    [EnumMember] Monday = 1,
    [EnumMember] Tuesday = 2,
    [EnumMember] Wednesday = 4,
    [EnumMember] Thursday = 8,
    [EnumMember] Friday = 16, // 0x00000010
    [EnumMember] Saturday = 32, // 0x00000020
    [EnumMember] Sunday = 64, // 0x00000040
    [EnumMember] All = 127, // 0x0000007F
  }
}
