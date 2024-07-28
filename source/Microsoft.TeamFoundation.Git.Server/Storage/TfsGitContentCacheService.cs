// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Storage.TfsGitContentCacheService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Streams;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server.Storage
{
  internal sealed class TfsGitContentCacheService : 
    VssMemoryCacheService<CacheKeys.CrossHostOdbScopedObjectId, StreamBytesAndType>
  {
    private int m_maxItemBytes;
    private const int c_defaultMaxItemBytes = 262144;
    private const double c_defaultMaxSizeFraction = 0.05;
    private static readonly TimeSpan s_defaultInactivityInterval = TimeSpan.FromMinutes(10.0);

    public TfsGitContentCacheService()
      : base((IEqualityComparer<CacheKeys.CrossHostOdbScopedObjectId>) EqualityComparer<CacheKeys.CrossHostOdbScopedObjectId>.Default, new MemoryCacheConfiguration<CacheKeys.CrossHostOdbScopedObjectId, StreamBytesAndType>().WithCleanupInterval(TfsGitContentCacheService.s_defaultInactivityInterval).WithInactivityInterval(TfsGitContentCacheService.s_defaultInactivityInterval).WithMaxSize(CacheUtil.GetMemoryFraction(0.05), (ISizeProvider<CacheKeys.CrossHostOdbScopedObjectId, StreamBytesAndType>) TfsGitContentCacheService.SizeProvider.Instance))
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRC)
    {
      systemRC.CheckDeploymentRequestContext();
      base.ServiceStart(systemRC);
    }

    protected override void LoadSettings(
      IVssRequestContext requestContext,
      RegistryEntryCollection ce)
    {
      base.LoadSettings(requestContext, ce);
      this.MaxCacheSize.Value = CacheUtil.GetMemoryFractionFromRegistry(ce, this.RegistryPath, 0.05);
      this.m_maxItemBytes = ce.GetValueFromPath<int>(this.RegistryPath + "/MaxItemBytes", 262144);
    }

    internal (TimeSpan inactivityInterval, long maxCacheSize, int maxItemBytes) TEST_Settings => (this.InactivityInterval.Value, this.MaxCacheSize.Value, this.m_maxItemBytes);

    public bool TryMakeStreamCached(
      CacheKeys.CrossHostOdbScopedObjectId key,
      GitPackObjectType packType,
      ref Stream stream)
    {
      if (!this.ShouldCache(packType, stream))
        return false;
      ConcatMemoryStream.Shards shards;
      StreamBytesAndType streamBytesAndType;
      if (packType == GitPackObjectType.Blob && ConcatMemoryStream.Shards.TryShardNonLoh(stream, out shards))
      {
        streamBytesAndType = new StreamBytesAndType(shards, packType);
      }
      else
      {
        byte[] numArray = new byte[stream.Length];
        GitStreamUtil.ReadGreedy(stream, numArray, 0, numArray.Length);
        streamBytesAndType = new StreamBytesAndType(numArray, packType);
      }
      stream.Dispose();
      stream = streamBytesAndType.GetStream();
      this.MemoryCache[key] = streamBytesAndType;
      return true;
    }

    private bool ShouldCache(GitPackObjectType packType, Stream stream)
    {
      if (stream.Length > (long) this.m_maxItemBytes)
        return false;
      return packType == GitPackObjectType.Tree || packType == GitPackObjectType.Blob || packType == GitPackObjectType.Commit || packType == GitPackObjectType.Tag;
    }

    public bool TryGetContent(CacheKeys.CrossHostOdbScopedObjectId key, out StreamAndType content)
    {
      StreamBytesAndType streamBytesAndType;
      if (this.MemoryCache.TryGetValue(key, out streamBytesAndType))
      {
        content = new StreamAndType(streamBytesAndType.GetStream(), streamBytesAndType.PackType, new Sha1Id?());
        return true;
      }
      content = new StreamAndType();
      return false;
    }

    private class SizeProvider : 
      ISizeProvider<CacheKeys.CrossHostOdbScopedObjectId, StreamBytesAndType>
    {
      public static readonly TfsGitContentCacheService.SizeProvider Instance = new TfsGitContentCacheService.SizeProvider();

      public long GetSize(CacheKeys.CrossHostOdbScopedObjectId key, StreamBytesAndType value) => (long) (52 + value.Length);
    }
  }
}
