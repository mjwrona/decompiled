// Decompiled with JetBrains decompiler
// Type: WebGrease.NullCacheManager
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace WebGrease
{
  internal class NullCacheManager : ICacheManager
  {
    internal static readonly ICacheSection EmptyCacheSection = (ICacheSection) new NullCacheSection();
    private static readonly Dictionary<string, ReadOnlyCacheSection> EmptyReadOnlyCacheSections = new Dictionary<string, ReadOnlyCacheSection>();

    public ICacheSection CurrentCacheSection => NullCacheManager.EmptyCacheSection;

    public IDictionary<string, ReadOnlyCacheSection> LoadedCacheSections => (IDictionary<string, ReadOnlyCacheSection>) NullCacheManager.EmptyReadOnlyCacheSections;

    public string RootPath => (string) null;

    public ICacheSection BeginSection(WebGreaseSectionKey webGreaseSectionKey, bool autoLoad = true) => NullCacheManager.EmptyCacheSection;

    public void CleanUp()
    {
    }

    public void EndSection(ICacheSection cacheSection)
    {
    }

    public string GetAbsoluteCacheFilePath(string category, string fileName) => (string) null;

    public void SetContext(IWebGreaseContext newContext)
    {
    }

    public string StoreInCache(string cacheCategory, ContentItem contentItem) => (string) null;

    public void LockedFileCacheAction(string lockFileContent, Action action) => action();
  }
}
