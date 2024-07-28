// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestOutcome
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public enum TestOutcome : byte
  {
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_UNSPECIFIED", false)] Unspecified = 0,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_NONE", false)] None = 1,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_PASSED", false)] Passed = 2,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_FAILED", false)] Failed = 3,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_INCONCLUSIVE", false)] Inconclusive = 4,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_TIMEOUT", false)] Timeout = 5,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_ABORTED", false)] Aborted = 6,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_BLOCKED", false)] Blocked = 7,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_NOT_EXECUTED", false)] NotExecuted = 8,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_WARNING", false)] Warning = 9,
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_ERROR", false)] Error = 10, // 0x0A
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_NOT_APPLICABLE", false)] NotApplicable = 11, // 0x0B
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_PAUSED", false)] Paused = 12, // 0x0C
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_IN_PROGRESS", false)] InProgress = 13, // 0x0D
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_MAX_VALUE", false)] MaxValue = 14, // 0x0E
    [LocalizedDisplayName("ENUM_TYPE_TEST_OUTCOME_NOT_IMPACTED", false)] NotImpacted = 14, // 0x0E
  }
}
