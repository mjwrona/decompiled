// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.UserFriendlyTestOutcome
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  public enum UserFriendlyTestOutcome : byte
  {
    InProgress = 0,
    Blocked = 1,
    Failed = 2,
    Passed = 3,
    Ready = 4,
    NotApplicable = 5,
    Paused = 6,
    Timeout = 7,
    Warning = 8,
    Error = 9,
    NotExecuted = 10, // 0x0A
    Inconclusive = 11, // 0x0B
    Aborted = 12, // 0x0C
    None = 13, // 0x0D
    NotImpacted = 14, // 0x0E
    MaxValue = 15, // 0x0F
    Unspecified = 15, // 0x0F
  }
}
