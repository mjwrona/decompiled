// Decompiled with JetBrains decompiler
// Type: WebGrease.ICacheSection
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.IO;
using WebGrease.Configuration;

namespace WebGrease
{
  public interface ICacheSection
  {
    ICacheSection Parent { get; }

    string UniqueKey { get; }

    void EndSection();

    bool CanBeRestoredFromCache();

    void AddResult(ContentItem contentItem, string id, bool isEndResult = false);

    void AddSourceDependency(string file);

    void AddSourceDependency(string directory, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

    void AddSourceDependency(InputSpec inputSpec);

    void Save();

    bool CanBeSkipped();

    ContentItem GetCachedContentItem(string fileCategory);

    IEnumerable<ContentItem> GetCachedContentItems(string fileCategory, bool endResultOnly = false);

    T GetCacheData<T>(string id) where T : new();

    void SetCacheData<T>(string id, T obj) where T : new();

    ContentItem GetCachedContentItem(
      string fileCategory,
      string relativeDestinationFile,
      string relativeHashedDestinationFile = null,
      IEnumerable<ResourcePivotKey> contentPivots = null);

    void Load();
  }
}
