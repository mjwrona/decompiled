// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.RuleValueFrom
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public enum RuleValueFrom
  {
    Value = 1,
    CurrentValue = 2,
    OriginalValue = 4,
    OtherFieldCurrentValue = 8,
    OtherFieldOriginalValue = 16, // 0x00000010
    CurrentUser = 32, // 0x00000020
    Clock = 64, // 0x00000040
  }
}
