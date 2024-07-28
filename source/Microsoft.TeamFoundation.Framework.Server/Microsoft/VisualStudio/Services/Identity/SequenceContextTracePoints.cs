// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SequenceContextTracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Identity
{
  internal enum SequenceContextTracePoints
  {
    GetLatestSequenceContextFailed = 1765000, // 0x001AEE88
    OriginalCacheMiss = 1765001, // 0x001AEE89
    RedisCacheMiss = 1765002, // 0x001AEE8A
    UnexpectedMulticache = 1765003, // 0x001AEE8B
    InvalidatedPrimary = 1765004, // 0x001AEE8C
    InvalidatedSecondary = 1765005, // 0x001AEE8D
    OriginalCacheHit = 1765006, // 0x001AEE8E
    RedisCacheHit = 1765007, // 0x001AEE8F
    CacheComparison = 1765008, // 0x001AEE90
  }
}
