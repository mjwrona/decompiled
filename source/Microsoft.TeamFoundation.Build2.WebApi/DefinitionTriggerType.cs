// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

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
    [EnumMember] GatedCheckIn = 16, // 0x00000010
    [EnumMember] BatchedGatedCheckIn = 32, // 0x00000020
    [EnumMember] PullRequest = 64, // 0x00000040
    [EnumMember] BuildCompletion = 128, // 0x00000080
    [EnumMember] All = 255, // 0x000000FF
  }
}
