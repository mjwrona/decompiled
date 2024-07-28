// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySearchFilter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  public enum IdentitySearchFilter
  {
    [EnumMember] AccountName = 0,
    [EnumMember] DisplayName = 1,
    [EnumMember] AdministratorsGroup = 2,
    [EnumMember] Identifier = 3,
    [EnumMember] MailAddress = 4,
    [EnumMember] General = 5,
    [EnumMember] Alias = 6,
    [EnumMember] DirectoryAlias = 8,
    [Obsolete("Deprecating TeamGroupName, use LocalGroupName instead and filter out non teams groups from the result"), EnumMember] TeamGroupName = 9,
    [EnumMember] LocalGroupName = 10, // 0x0000000A
  }
}
