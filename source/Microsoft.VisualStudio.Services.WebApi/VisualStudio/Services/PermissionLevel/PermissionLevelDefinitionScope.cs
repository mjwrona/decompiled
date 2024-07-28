// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelDefinitionScope
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  [DataContract]
  public enum PermissionLevelDefinitionScope
  {
    [EnumMember(Value = "deployment")] Deployment = 1,
    [EnumMember(Value = "enterprise")] Enterprise = 10, // 0x0000000A
    [EnumMember(Value = "organization")] Organization = 20, // 0x00000014
    [EnumMember(Value = "project")] Project = 30, // 0x0000001E
  }
}
