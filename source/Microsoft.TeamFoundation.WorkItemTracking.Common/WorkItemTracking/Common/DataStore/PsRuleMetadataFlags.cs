// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore.PsRuleMetadataFlags
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum PsRuleMetadataFlags
  {
    RuleFlagNone = 0,
    RuleFlagEditable = 1,
    RuleFlagGrantRead = 2,
    RuleFlagDenyRead = 4,
    RuleFlagGrantWrite = 8,
    RuleFlagDenyWrite = 16, // 0x00000010
    RuleFlagGrantAdmin = 32, // 0x00000020
    RuleFlagDenyAdmin = 64, // 0x00000040
    RuleFlagUnless = 128, // 0x00000080
    RuleFlagFlowdownTree = 256, // 0x00000100
    RuleFlagDefault = 512, // 0x00000200
    RuleFlagSuggestion = 1024, // 0x00000400
    RuleFlagReverse = 2048, // 0x00000800
    RuleFlagIfNot = 4096, // 0x00001000
    RuleFlagIf2Not = 524288, // 0x00080000
    RuleFlagSemiEditable = 1048576, // 0x00100000
    RuleFlagInversePerson = 2097152, // 0x00200000
    RuleFlagThenNot = 4194304, // 0x00400000
    RuleFlagThenLike = 8388608, // 0x00800000
    RuleFlagThenLeaf = 16777216, // 0x01000000
    RuleFlagThenInterior = 33554432, // 0x02000000
    RuleFlagThenOneLevel = 67108864, // 0x04000000
    RuleFlagThenTwoPlusLevels = 134217728, // 0x08000000
    RuleFlagThenImplicitAlso = 268435456, // 0x10000000
    RuleFlagHelptext = 536870912, // 0x20000000
    RuleFlagIf = 782336, // 0x000BF000
    RuleFlagThen = RuleFlagThenTwoPlusLevels | RuleFlagThenOneLevel | RuleFlagThenInterior | RuleFlagThenLeaf | RuleFlagThenLike | RuleFlagThenNot, // 0x0FC00000
    RuleFlagAccessMasks = RuleFlagHelptext | RuleFlagSuggestion | RuleFlagDefault | RuleFlagDenyAdmin | RuleFlagGrantAdmin | RuleFlagDenyWrite | RuleFlagGrantWrite | RuleFlagDenyRead | RuleFlagGrantRead, // 0x2000067E
    RuleFlagIfAllNodes = 245760, // 0x0003C000
    RuleFlagThenAllNodes = RuleFlagThenTwoPlusLevels | RuleFlagThenOneLevel | RuleFlagThenInterior | RuleFlagThenLeaf, // 0x0F000000
    RuleFlagDecisiveRuleFlags = 1072693246, // 0x3FEFFFFE
  }
}
