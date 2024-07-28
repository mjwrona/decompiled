// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.VersionControlChangeType
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [Flags]
  [DataContract]
  public enum VersionControlChangeType
  {
    [EnumMember] None = 0,
    [EnumMember] Add = 1,
    [EnumMember] Edit = 2,
    [EnumMember] Encoding = 4,
    [EnumMember] Rename = 8,
    [EnumMember] Delete = 16, // 0x00000010
    [EnumMember] Undelete = 32, // 0x00000020
    [EnumMember] Branch = 64, // 0x00000040
    [EnumMember] Merge = 128, // 0x00000080
    [EnumMember] Lock = 256, // 0x00000100
    [EnumMember] Rollback = 512, // 0x00000200
    [EnumMember] SourceRename = 1024, // 0x00000400
    [EnumMember] TargetRename = 2048, // 0x00000800
    [EnumMember] Property = 4096, // 0x00001000
    [EnumMember] All = Property | TargetRename | SourceRename | Rollback | Lock | Merge | Branch | Undelete | Delete | Rename | Encoding | Edit | Add, // 0x00001FFF
  }
}
