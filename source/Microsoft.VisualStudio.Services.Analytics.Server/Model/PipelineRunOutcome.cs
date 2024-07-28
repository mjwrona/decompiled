// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.PipelineRunOutcome
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;

namespace Microsoft.VisualStudio.Services.Analytics.Model
{
  public enum PipelineRunOutcome : byte
  {
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_OUTCOME_NONE", false)] None = 0,
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_OUTCOME_SUCCEED", false)] Succeed = 2,
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_OUTCOME_PARTIALLY_SUCCEEDED", false)] PartiallySucceeded = 4,
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_OUTCOME_FAILED", false)] Failed = 8,
    [LocalizedDisplayName("ENUM_TYPE_PIPELINE_RUN_OUTCOME_CANCELED", false)] Canceled = 32, // 0x20
  }
}
