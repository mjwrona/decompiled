// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public enum DefinitionTriggerType
  {
    [EnumMember] None = 1,
    [EnumMember] ContinuousIntegration = 2,
    [EnumMember] BatchedContinuousIntegration = 4,
    [EnumMember] Schedule = 8,
    [EnumMember] ScheduleForced = 16, // 0x00000010
    [EnumMember] GatedCheckIn = 32, // 0x00000020
    [EnumMember] BatchedGatedCheckIn = 64, // 0x00000040
    [EnumMember] All = 127, // 0x0000007F
  }
}
