// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ScheduleDays
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Scope = "type", Justification = "Not all values are powers of 2.")]
  [SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames", Scope = "type", Justification = "Can represent multiple days.")]
  public enum ScheduleDays
  {
    [EnumMember(Value = "0")] None = 0,
    [EnumMember(Value = "1")] Monday = 1,
    [EnumMember(Value = "2")] Tuesday = 2,
    [EnumMember(Value = "4")] Wednesday = 4,
    [EnumMember(Value = "8")] Thursday = 8,
    [EnumMember(Value = "16")] Friday = 16, // 0x00000010
    [EnumMember(Value = "32")] Saturday = 32, // 0x00000020
    [EnumMember(Value = "64")] Sunday = 64, // 0x00000040
    [EnumMember(Value = "127")] All = 127, // 0x0000007F
  }
}
