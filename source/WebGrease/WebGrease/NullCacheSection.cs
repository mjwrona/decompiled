// Decompiled with JetBrains decompiler
// Type: WebGrease.NullCacheSection
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.IO;
using WebGrease.Configuration;

namespace WebGrease
{
  public class NullCacheSection : ICacheSection
  {
    private static readonly IEnumerable<ContentItem> EmptyContentItems = (IEnumerable<ContentItem>) new ContentItem[0];

    public NullCacheSection() => this.UniqueKey = string.Empty;

    public ICacheSection Parent => NullCacheManager.EmptyCacheSection;

    public string UniqueKey { get; private set; }

    public void AddResult(ContentItem contentItem, string id, bool isEndResult)
    {
    }

    public void AddSourceDependency(string file)
    {
    }

    public void AddSourceDependency(
      string directory,
      string searchPattern,
      SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
    }

    public void AddSourceDependency(InputSpec inputSpec)
    {
    }

    public bool CanBeRestoredFromCache() => false;

    public bool CanBeSkipped() => false;

    public void EndSection()
    {
    }

    public ContentItem GetCachedContentItem(string fileCategory) => (ContentItem) null;

    public IEnumerable<ContentItem> GetCachedContentItems(string fileCategory, bool endResultOnly = false) => NullCacheSection.EmptyContentItems;

    public ContentItem GetCachedContentItem(
      string fileCategory,
      string relativeDestinationFile,
      string relativeHashedDestinationFile = null,
      IEnumerable<ResourcePivotKey> contentPivots = null)
    {
      return (ContentItem) null;
    }

    public void Load()
    {
    }

    public T GetCacheData<T>(string id) where T : new() => default (T);

    public void SetCacheData<T>(string id, T obj) where T : new()
    {
    }

    public void Save()
    {
    }
  }
}
