// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Client.TestOutcome
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.TestManagement.Client
{
  [GenerateAllConstants(null)]
  public enum TestOutcome
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
    Error = 10, // 0x0000000A
    NotApplicable = 11, // 0x0000000B
    Paused = 12, // 0x0000000C
    InProgress = 13, // 0x0000000D
    MaxValue = 14, // 0x0000000E
    NotImpacted = 14, // 0x0000000E
  }
}
