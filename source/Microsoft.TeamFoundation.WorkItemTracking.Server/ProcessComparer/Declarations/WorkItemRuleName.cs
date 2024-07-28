// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WorkItemRuleName
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public enum WorkItemRuleName
  {
    Block = 100, // 0x00000064
    FieldRules = 110, // 0x0000006E
    WhenWas = 300, // 0x0000012C
    WhenWasNot = 350, // 0x0000015E
    When = 400, // 0x00000190
    WhenNot = 450, // 0x000001C2
    WhenChanged = 700, // 0x000002BC
    WhenNotChanged = 750, // 0x000002EE
    Copy = 800, // 0x00000320
    Default = 900, // 0x00000384
    ServerDefault = 1000, // 0x000003E8
    ReadOnly = 1100, // 0x0000044C
    Empty = 1200, // 0x000004B0
    Required = 2100, // 0x00000834
    Frozen = 2200, // 0x00000898
    CannotLoseValue = 2300, // 0x000008FC
    NotSameAs = 2400, // 0x00000960
    ValidUser = 2500, // 0x000009C4
    AllowExistingValue = 2600, // 0x00000A28
    Match = 2700, // 0x00000A8C
    AllowedValues = 3000, // 0x00000BB8
    SuggestedValues = 3100, // 0x00000C1C
    ProhibitedValues = 3200, // 0x00000C80
    HelpText = 20000, // 0x00004E20
    OnState = 20001, // 0x00004E21
    OnTransition = 20002, // 0x00004E22
    OnReason = 20003, // 0x00004E23
  }
}
