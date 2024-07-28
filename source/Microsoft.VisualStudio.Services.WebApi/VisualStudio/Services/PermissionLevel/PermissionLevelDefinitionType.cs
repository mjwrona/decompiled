// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelDefinitionType
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  [DataContract]
  [Flags]
  public enum PermissionLevelDefinitionType
  {
    [EnumMember(Value = "system")] SystemDefined = 1,
    [EnumMember(Value = "restrictedVisibility")] RestrictedVisibility = 8,
    [EnumMember(Value = "all")] All = RestrictedVisibility | SystemDefined, // 0x00000009
  }
}
