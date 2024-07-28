// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Extension.ExtensionStateFlags
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Extension
{
  [DataContract]
  [Flags]
  public enum ExtensionStateFlags
  {
    [EnumMember] None = 0,
    [EnumMember] Disabled = 1,
    [EnumMember] BuiltIn = 2,
    [EnumMember] MultiVersion = 4,
    [EnumMember] UnInstalled = 8,
    [EnumMember] VersionCheckError = 16, // 0x00000010
    [EnumMember] Trusted = 32, // 0x00000020
    [EnumMember] Error = 64, // 0x00000040
    [EnumMember] NeedsReauthorization = 128, // 0x00000080
    [EnumMember] AutoUpgradeError = 256, // 0x00000100
    [EnumMember] Warning = 512, // 0x00000200
  }
}
