// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionPropertyType
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  [Flags]
  public enum ContributionPropertyType
  {
    [EnumMember] Unknown = 0,
    [EnumMember] String = 1,
    [EnumMember] Uri = 2,
    [EnumMember] Guid = 4,
    [EnumMember] Boolean = 8,
    [EnumMember] Integer = 16, // 0x00000010
    [EnumMember] Double = 32, // 0x00000020
    [EnumMember] DateTime = 64, // 0x00000040
    [EnumMember] Dictionary = 128, // 0x00000080
    [EnumMember] Array = 256, // 0x00000100
    [EnumMember] Object = 512, // 0x00000200
  }
}
