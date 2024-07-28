// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.NavigationContextLevels
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [Flags]
  [DataContract]
  public enum NavigationContextLevels
  {
    [EnumMember] None = 0,
    [EnumMember] Deployment = 1,
    [EnumMember] Application = 2,
    [EnumMember] Collection = 4,
    [EnumMember] Project = 8,
    [EnumMember] Team = 16, // 0x00000010
    [EnumMember] ApplicationAll = Team | Project | Collection | Application, // 0x0000001E
    [EnumMember] All = ApplicationAll | Deployment, // 0x0000001F
  }
}
