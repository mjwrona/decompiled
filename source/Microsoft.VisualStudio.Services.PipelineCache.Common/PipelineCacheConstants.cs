// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PipelineCache.Common.PipelineCacheConstants
// Assembly: Microsoft.VisualStudio.Services.PipelineCache.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E063C74A-FCE9-47BF-84C0-7143B7075032
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PipelineCache.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.PipelineCache.Common
{
  public static class PipelineCacheConstants
  {
    public const int RetentionDays = 7;
    public const string CacheScopesClaimKey = "pcc";
    public static readonly Guid PipelineCacheTaskId = Guid.Parse("D53CCAB4-555E-4494-9D06-11DB043FB4A9");
  }
}
