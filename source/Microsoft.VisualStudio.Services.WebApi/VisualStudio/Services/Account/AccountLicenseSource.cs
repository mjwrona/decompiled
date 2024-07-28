// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.AccountLicenseSource
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Account
{
  [DataContract]
  public enum AccountLicenseSource
  {
    [EnumMember] None = 2,
    [EnumMember] VsExpress = 10, // 0x0000000A
    [EnumMember] VsPro = 12, // 0x0000000C
    [EnumMember] VsTestPro = 14, // 0x0000000E
    [EnumMember] VsPremium = 16, // 0x00000010
    [EnumMember] VsUltimate = 18, // 0x00000012
    [EnumMember] MSDN = 38, // 0x00000026
    [EnumMember] MsdnPro = 40, // 0x00000028
    [EnumMember] MsdnTestPro = 42, // 0x0000002A
    [EnumMember] MsdnPremium = 44, // 0x0000002C
    [EnumMember] MsdnUltimate = 46, // 0x0000002E
    [EnumMember] MsdnPlatforms = 48, // 0x00000030
    [EnumMember] VSOStandard = 50, // 0x00000032
    [EnumMember] VSOAdvanced = 52, // 0x00000034
    [EnumMember] VSOProStandard = 54, // 0x00000036
    [EnumMember] Win8 = 56, // 0x00000038
    [EnumMember] Desktop = 58, // 0x0000003A
    [EnumMember] Phone = 60, // 0x0000003C
    [EnumMember] VsEarlyAdopter = 70, // 0x00000046
  }
}
