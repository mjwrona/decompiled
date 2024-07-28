// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.Outcome
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  public enum Outcome : byte
  {
    Unspecified = 0,
    None = 1,
    Passed = 2,
    Failed = 3,
    Inconclusive = 4,
    Timeout = 5,
    Aborted = 6,
    Blocked = 7,
    NotExecuted = 8,
    Warning = 9,
    Error = 10, // 0x0A
    NotApplicable = 11, // 0x0B
    Paused = 12, // 0x0C
    InProgress = 13, // 0x0D
    MaxValue = 14, // 0x0E
    NotImpacted = 14, // 0x0E
  }
}
