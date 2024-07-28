// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsRuleMetadataFlags2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum PsRuleMetadataFlags2
  {
    RuleFlag2None = 0,
    RuleFlag2ThenConstLargetext = 16, // 0x00000010
    RuleFlag2ThenImplicitEmpty = 32, // 0x00000020
    RuleFlag2ThenImplicitUnchanged = 64, // 0x00000040
    RuleFlag2FlowaroundTree = 256, // 0x00000100
    RuleFlagDecisiveRuleFlags2 = 375, // 0x00000177
  }
}
