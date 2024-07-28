// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Enums.QueryScopingCacheStatus
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common.Enums
{
  public enum QueryScopingCacheStatus
  {
    Undefined,
    CacheDisabled,
    CacheCreationRunning,
    CacheNotNeeded,
    Cached,
    CacheInvalidated,
    CacheCreationNotPossible,
    CacheUpdationGoingOn,
    CacheCreationFailed,
    CacheCreationPossible,
  }
}
