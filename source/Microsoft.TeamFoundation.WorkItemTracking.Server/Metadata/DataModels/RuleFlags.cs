// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.RuleFlags
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  [DebuggerDisplay("{ToDebugString()}")]
  [Flags]
  public enum RuleFlags
  {
    None = 0,
    Editable = 1,
    GrantRead = 2,
    DenyRead = 4,
    GrantWrite = 8,
    DenyWrite = 16, // 0x00000010
    GrantAdmin = 32, // 0x00000020
    DenyAdmin = 64, // 0x00000040
    Unless = 128, // 0x00000080
    FlowdownTree = 256, // 0x00000100
    Default = 512, // 0x00000200
    Suggestion = 1024, // 0x00000400
    Reverse = 2048, // 0x00000800
    IfNot = 4096, // 0x00001000
    If2Not = 524288, // 0x00080000
    SemiEditable = 1048576, // 0x00100000
    InversePerson = 2097152, // 0x00200000
    ThenNot = 4194304, // 0x00400000
    ThenLike = 8388608, // 0x00800000
    ThenLeaf = 16777216, // 0x01000000
    ThenInterior = 33554432, // 0x02000000
    ThenOneLevel = 67108864, // 0x04000000
    ThenTwoPlusLevels = 134217728, // 0x08000000
    ThenImplicitAlso = 268435456, // 0x10000000
    Helptext = 536870912, // 0x20000000
    If = 782336, // 0x000BF000
    Then = ThenTwoPlusLevels | ThenOneLevel | ThenInterior | ThenLeaf | ThenLike | ThenNot, // 0x0FC00000
    AccessMasks = Helptext | Suggestion | Default | DenyAdmin | GrantAdmin | DenyWrite | GrantWrite | DenyRead | GrantRead, // 0x2000067E
    IfAllNodes = 245760, // 0x0003C000
    ThenAllNodes = ThenTwoPlusLevels | ThenOneLevel | ThenInterior | ThenLeaf, // 0x0F000000
    DecisiveRuleFlags = 1072693246, // 0x3FEFFFFE
  }
}
