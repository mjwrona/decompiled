// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StorageSizeBasedCacheCleanup
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StorageSizeBasedCacheCleanup : CacheCleanup
  {
    public StorageSizeBasedCacheCleanup(FileCacheService fileCache)
      : base(fileCache)
    {
    }

    protected override long GetCacheLimit()
    {
      long overallCacheSize = this.m_fileCache.Statistics.OverallCacheSize;
      return this.m_fileCache.ValidateCacheLimit((long) this.m_fileCache.Configuration.CacheLimit * 1024L * 1024L);
    }
  }
}
