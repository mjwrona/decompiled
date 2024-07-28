// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionQueryOptions
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [Flags]
  public enum ContributionQueryOptions
  {
    [EnumMember(Value = "none")] None = 0,
    [EnumMember(Value = "includeSelf")] IncludeSelf = 16, // 0x00000010
    [EnumMember(Value = "includeChildren")] IncludeChildren = 32, // 0x00000020
    [EnumMember(Value = "includeSubTree")] IncludeSubTree = 96, // 0x00000060
    [EnumMember(Value = "includeAll")] IncludeAll = IncludeSubTree | IncludeSelf, // 0x00000070
    [EnumMember(Value = "ignoreConstraints")] IgnoreConstraints = 256, // 0x00000100
  }
}
