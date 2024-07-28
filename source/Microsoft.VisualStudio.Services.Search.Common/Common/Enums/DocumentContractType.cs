// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Enums.DocumentContractType
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Enums
{
  [DataContract]
  public enum DocumentContractType
  {
    [EnumMember] Unsupported = 0,
    [EnumMember] ProjectContract = 2,
    [EnumMember] SourceNoDedupeFileContractV2 = 6,
    [EnumMember] WorkItemContract = 7,
    [EnumMember] RepositoryContract = 9,
    [EnumMember] SourceNoDedupeFileContractV3 = 10, // 0x0000000A
    [EnumMember] DedupeFileContractV3 = 11, // 0x0000000B
    [EnumMember] WikiContract = 12, // 0x0000000C
    [EnumMember] PackageVersionContract = 13, // 0x0000000D
    [EnumMember] SourceNoDedupeFileContractV4 = 15, // 0x0000000F
    [EnumMember] BoardContract = 16, // 0x00000010
    [EnumMember] DedupeFileContractV4 = 17, // 0x00000011
    [EnumMember] SettingContract = 18, // 0x00000012
    [EnumMember] SourceNoDedupeFileContractV5 = 19, // 0x00000013
    [EnumMember] DedupeFileContractV5 = 20, // 0x00000014
  }
}
