// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.TestResultState
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public enum TestResultState : byte
  {
    [LocalizedDisplayName("ENUM_TYPE_TEST_RESULT_STATE_UNSPECIFIED", false)] Unspecified = 0,
    [LocalizedDisplayName("ENUM_TYPE_TEST_RESULT_STATE_PENDING", false)] Pending = 1,
    [LocalizedDisplayName("ENUM_TYPE_TEST_RESULT_STATE_QUEUED", false)] Queued = 2,
    [LocalizedDisplayName("ENUM_TYPE_TEST_RESULT_STATE_IN_PROGRESS", false)] InProgress = 3,
    [LocalizedDisplayName("ENUM_TYPE_TEST_RESULT_STATE_PAUSED", false)] Paused = 4,
    [LocalizedDisplayName("ENUM_TYPE_TEST_RESULT_STATE_COMPLETED", false)] Completed = 5,
    [LocalizedDisplayName("ENUM_TYPE_TEST_RESULT_STATE_MAX_VALUE", false)] MaxValue = 5,
  }
}
