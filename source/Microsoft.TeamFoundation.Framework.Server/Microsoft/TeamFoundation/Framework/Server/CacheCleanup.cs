// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CacheCleanup
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class CacheCleanup
  {
    protected FileCacheService m_fileCache;
    private const string c_Area = "FileCache";
    private const string c_Layer = "Cleanup";

    protected CacheCleanup(FileCacheService fileCache) => this.m_fileCache = fileCache;

    internal void CleanCache(IVssRequestContext requestContext, long cacheLimit)
    {
      long currentCacheSize = this.m_fileCache.Statistics.OverallCacheSize;
      if (currentCacheSize <= 0L || !this.m_fileCache.TryGetCacheCleanupMutex(requestContext))
        return;
      long spaceToFreeUp = this.ComputeSpaceToFreeUp(currentCacheSize, cacheLimit);
      requestContext.Trace(12080, TraceLevel.Verbose, "FileCache", "Cleanup", FrameworkResources.StartingCacheCleanup((object) currentCacheSize.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture), (object) cacheLimit.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture), (object) spaceToFreeUp.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture)));
      IVssDeploymentServiceHost deploymentServiceHost = requestContext.ServiceHost.DeploymentServiceHost;
      ThreadPool.QueueUserWorkItem((WaitCallback) (foo =>
      {
        using (IVssRequestContext systemContext = deploymentServiceHost.CreateSystemContext())
        {
          try
          {
            this.m_fileCache.DeleteCacheItems.Delete(systemContext, spaceToFreeUp);
            long overallCacheSize = this.m_fileCache.Statistics.OverallCacheSize;
            if (overallCacheSize < cacheLimit)
              return;
            systemContext.TraceAlways(12081, TraceLevel.Warning, "FileCache", "Cleanup", FrameworkResources.StartingCacheCleanup((object) currentCacheSize.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture), (object) cacheLimit.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture), (object) spaceToFreeUp.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture)));
            this.m_fileCache.DeleteCacheItems.Delete(systemContext, this.ComputeSpaceToFreeUp(overallCacheSize, cacheLimit), true);
          }
          finally
          {
            this.m_fileCache.ReleaseCacheCleanupMutex(systemContext);
          }
        }
      }));
    }

    private long ComputeSpaceToFreeUp(long currentCacheSize, long cacheLimit)
    {
      long spaceToFreeUp = 0;
      if (cacheLimit != -1L)
      {
        long num = cacheLimit - cacheLimit * (long) this.m_fileCache.Configuration.CacheDeletionPercent / 100L;
        if (currentCacheSize > num)
          spaceToFreeUp = currentCacheSize - num;
      }
      else
        spaceToFreeUp = currentCacheSize * (long) this.m_fileCache.Configuration.CacheDeletionPercent / 100L;
      return spaceToFreeUp;
    }

    internal void CheckCacheLimit(IVssRequestContext requestContext)
    {
      long cacheLimit = this.GetCacheLimit();
      if (this.m_fileCache.Statistics.OverallCacheSize < cacheLimit)
        return;
      this.CleanCache(requestContext, cacheLimit);
    }

    protected abstract long GetCacheLimit();
  }
}
