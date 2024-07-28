// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  [Flags]
  public enum RuleFlags2
  {
    None = 0,
    ThenConstLargetext = 16, // 0x00000010
    ThenImplicitEmpty = 32, // 0x00000020
    ThenImplicitUnchanged = 64, // 0x00000040
    FlowaroundTree = 256, // 0x00000100
    DecisiveRuleFlags = 375, // 0x00000177
  }
}
