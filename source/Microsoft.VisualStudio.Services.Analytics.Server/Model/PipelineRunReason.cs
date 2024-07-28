// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.PipelineRunReason
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public enum PipelineRunReason : short
  {
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_NONE", false)] None = 0,
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_MANUAL", false)] Manual = 1,
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_INDIVIDUAL_CI", false)] IndividualCI = 2,
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_BATCHED_CI", false)] BatchedCI = 4,
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_SCHEDULE", false)] Schedule = 8,
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_USER_CREATED", false)] UserCreated = 32, // 0x0020
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_VALIDATE_SHELVESET", false)] ValidateShelveset = 64, // 0x0040
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_CHECK_IN_SHELVESET", false)] CheckInShelveset = 128, // 0x0080
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_PULL_REQUEST", false)] PullRequest = 256, // 0x0100
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_REASON_BUILD_COMPLETION", false)] PipelineRunCompletion = 512, // 0x0200
  }
}
