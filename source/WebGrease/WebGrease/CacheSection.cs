// Decompiled with JetBrains decompiler
// Type: WebGrease.CacheSection
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebGrease.Configuration;
using WebGrease.Extensions;

namespace WebGrease
{
  public class CacheSection : ICacheSection
  {
    private readonly List<CacheResult> cacheResults = new List<CacheResult>();
    private readonly IDictionary<string, CacheSourceDependency> sourceDependencies = (IDictionary<string, CacheSourceDependency>) new Dictionary<string, CacheSourceDependency>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Lazy<List<CacheSection>> childCacheSections = new Lazy<List<CacheSection>>((Func<List<CacheSection>>) (() => new List<CacheSection>()), true);
    private string cacheCategory;
    private ReadOnlyCacheSection cachedSection;
    private IWebGreaseContext context;
    private bool isUnsaved = true;
    private CacheSection parent;
    private string absolutePath;

    private CacheSection()
    {
    }

    public ICacheSection Parent => (ICacheSection) this.parent;

    public string UniqueKey { get; private set; }

    private List<CacheSection> ChildCacheSections => this.childCacheSections.Value;

    public static CacheSection Begin(
      IWebGreaseContext context,
      string cacheCategory,
      string uniqueKey,
      ICacheSection parentCacheSection = null,
      bool autoLoad = true)
    {
      CacheSection cacheSection = new CacheSection()
      {
        parent = parentCacheSection as CacheSection,
        cacheCategory = cacheCategory,
        context = context,
        UniqueKey = uniqueKey
      };
      cacheSection.absolutePath = context.Cache.GetAbsoluteCacheFilePath(cacheSection.cacheCategory, context.GetValueHash(cacheSection.UniqueKey) + ".cache.json");
      if (cacheSection.parent != null)
        cacheSection.parent.AddChildCacheSection(cacheSection);
      CacheSection.EnsureCachePath(context, cacheCategory);
      if (autoLoad)
        cacheSection.Load();
      return cacheSection;
    }

    public void Load()
    {
      try
      {
        FileInfo fileInfo = new FileInfo(this.absolutePath);
        if (!fileInfo.Exists)
          return;
        this.cachedSection = ReadOnlyCacheSection.Load(fileInfo.FullName, this.context);
        this.isUnsaved = this.cachedSection != null;
      }
      catch (PathTooLongException ex)
      {
        throw new BuildWorkflowException("Path to long: {0}".InvariantFormat((object) this.absolutePath), (Exception) ex);
      }
    }

    public T GetCacheData<T>(string id) where T : new()
    {
      if (this.cachedSection != null)
      {
        ContentItem contentItem = this.GetCachedContentItems(id, false).FirstOrDefault<ContentItem>();
        if (contentItem != null && !string.IsNullOrWhiteSpace(contentItem.Content))
          return contentItem.Content.FromJson<T>(true);
      }
      return new T();
    }

    public void SetCacheData<T>(string id, T obj) where T : new() => this.AddResult(ContentItem.FromContent(((object) obj).ToJson(true)), id, false);

    public void AddResult(ContentItem contentItem, string id, bool isEndResult)
    {
      this.isUnsaved = true;
      Safe.Lock((object) this.cacheResults, int.MaxValue, (Action) (() => this.cacheResults.Add(CacheResult.FromContentFile(this.context, this.cacheCategory, isEndResult, id, contentItem))));
    }

    public void AddSourceDependency(string file)
    {
      if (!File.Exists(file))
        throw new BuildWorkflowException("Cannot add a source dependency that does not exists on disk: {0}".InvariantFormat((object) file));
      this.AddSourceDependency(new InputSpec()
      {
        Path = file
      });
    }

    public void AddSourceDependency(
      string directory,
      string searchPattern,
      SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
      if (!Directory.Exists(directory))
        throw new BuildWorkflowException("Cannot add a source dependency that does not exists on disk: {0}".InvariantFormat((object) directory));
      this.AddSourceDependency(new InputSpec()
      {
        Path = directory,
        SearchPattern = searchPattern,
        SearchOption = searchOption
      });
    }

    public void AddSourceDependency(InputSpec inputSpec)
    {
      this.isUnsaved = true;
      string key = inputSpec.ToJson(true);
      Safe.UniqueKeyLock(key, int.MaxValue, (Action) (() =>
      {
        if (this.sourceDependencies.ContainsKey(key))
          return;
        this.sourceDependencies.Add(key, CacheSourceDependency.Create(this.context, new InputSpec()
        {
          IsOptional = inputSpec.IsOptional,
          Path = inputSpec.Path,
          SearchOption = inputSpec.SearchOption,
          SearchPattern = inputSpec.SearchPattern
        }));
      }));
    }

    public bool CanBeRestoredFromCache() => this.cachedSection != null && this.cachedSection.CanBeRestoredFromCache();

    public bool CanBeSkipped() => this.cachedSection != null && this.cachedSection.CanBeSkipped();

    public void EndSection()
    {
      this.context.Cache.EndSection((ICacheSection) this);
      this.Dispose();
    }

    public IEnumerable<CacheResult> GetCacheResults(string fileCategory = null, bool endResultOnly = false) => this.cachedSection.GetCacheResults(fileCategory, endResultOnly);

    public ContentItem GetCachedContentItem(string fileCategory)
    {
      CacheResult cacheResult = this.GetCacheResults(fileCategory).FirstOrDefault<CacheResult>();
      return cacheResult == null ? (ContentItem) null : ContentItem.FromCacheResult(cacheResult);
    }

    public ContentItem GetCachedContentItem(
      string fileCategory,
      string relativeDestinationFile,
      string relativeHashedDestinationFile = null,
      IEnumerable<ResourcePivotKey> contentPivots = null)
    {
      return ContentItem.FromCacheResult(this.GetCacheResults(fileCategory).FirstOrDefault<CacheResult>(), relativeDestinationFile, relativeHashedDestinationFile, contentPivots != null ? contentPivots.ToArray<ResourcePivotKey>() : (ResourcePivotKey[]) null);
    }

    public IEnumerable<ContentItem> GetCachedContentItems(string fileCategory, bool endResultOnly = false) => this.GetCacheResults(fileCategory, endResultOnly).Select<CacheResult, ContentItem>((Func<CacheResult, ContentItem>) (crf => ContentItem.FromCacheResult(crf)));

    public void Save()
    {
      if (!this.isUnsaved)
        return;
      this.isUnsaved = false;
      FileInfo fileInfo = new FileInfo(this.absolutePath);
      if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
        fileInfo.Directory.Create();
      File.WriteAllText(this.absolutePath, CacheSection.ToReadOnlyCacheSectionJson(this));
      this.Touch();
    }

    private static void EnsureCachePath(IWebGreaseContext context, string cacheCategory)
    {
      string absoluteCacheFilePath = context.Cache.GetAbsoluteCacheFilePath(cacheCategory, string.Empty);
      if (Directory.Exists(absoluteCacheFilePath))
        return;
      Directory.CreateDirectory(absoluteCacheFilePath);
    }

    private static string ToReadOnlyCacheSectionJson(CacheSection cacheSection) => new
    {
      sourceDependencies = cacheSection.sourceDependencies.Values,
      cacheResults = cacheSection.cacheResults,
      children = cacheSection.ChildCacheSections.Select<CacheSection, string>((Func<CacheSection, string>) (ccs => ccs.absolutePath)),
      absolutePath = cacheSection.absolutePath
    }.ToJson();

    private void AddChildCacheSection(CacheSection cacheSection) => Safe.Lock((object) this.ChildCacheSections, (Action) (() => this.ChildCacheSections.Add(cacheSection)));

    private void Dispose()
    {
      if (this.cachedSection != null)
        this.cachedSection.Dispose();
      this.context = (IWebGreaseContext) null;
      this.parent = (CacheSection) null;
      this.sourceDependencies.Clear();
      this.ChildCacheSections.Clear();
      this.childCacheSections = (Lazy<List<CacheSection>>) null;
      this.cacheResults.Clear();
    }

    private void Touch()
    {
      this.context.Touch(this.absolutePath);
      this.cacheResults.ForEach((Action<CacheResult>) (cr => this.context.Touch(cr.CachedFilePath)));
    }
  }
}
