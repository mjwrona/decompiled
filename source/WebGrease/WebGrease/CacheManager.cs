// Decompiled with JetBrains decompiler
// Type: WebGrease.CacheManager
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
  public class CacheManager : ICacheManager
  {
    internal const string LockFileName = "webgrease.caching.lock";
    private static readonly IList<string> First = (IList<string>) new List<string>();
    private readonly IDictionary<string, ReadOnlyCacheSection> loadedCacheSections = (IDictionary<string, ReadOnlyCacheSection>) new Dictionary<string, ReadOnlyCacheSection>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly string cacheRootPath;
    private IWebGreaseContext context;
    private ICacheSection currentCacheSection;

    public CacheManager(
      WebGreaseConfiguration configuration,
      LogManager logManager,
      ICacheSection parentCacheSection)
    {
      CacheManager cacheManager = this;
      this.currentCacheSection = parentCacheSection;
      if (configuration == null)
        throw new ArgumentNullException(nameof (configuration));
      if (logManager == null)
        throw new ArgumentNullException(nameof (logManager));
      string str = configuration.CacheRootPath.AsNullIfWhiteSpace() ?? "_webgrease.cache";
      if (!Path.IsPathRooted(str))
        str = Path.Combine(configuration.SourceDirectory, str);
      this.cacheRootPath = CacheManager.GetCacheUniquePath(str, configuration.CacheUniqueKey);
      if (!Directory.Exists(this.cacheRootPath))
        Directory.CreateDirectory(this.cacheRootPath);
      Safe.Lock((object) CacheManager.First, (Action) (() =>
      {
        if (!CacheManager.First.Contains(cacheManager.cacheRootPath))
          return;
        CacheManager.First.Add(cacheManager.cacheRootPath);
        logManager.Information("Cache enabled using cache path: {0}".InvariantFormat((object) cacheManager.cacheRootPath));
      }));
    }

    public void LockedFileCacheAction(string lockFileContent, Action action)
    {
      string filePath = Path.Combine(this.RootPath, "webgrease.caching.lock");
      if (!Safe.WriteToFileStream(filePath, (Action<FileStream>) (fs =>
      {
        StreamWriter streamWriter = new StreamWriter((Stream) fs);
        streamWriter.Write(lockFileContent);
        streamWriter.Flush();
        action();
        streamWriter.Write("\r\nDone");
        streamWriter.Flush();
      })))
        throw new BuildWorkflowException("Could not get a unique lock on cache lock file: {0}".InvariantFormat((object) filePath));
    }

    private static string GetCacheUniquePath(string cacheRoot, string cacheUniqueKey)
    {
      string cachePath = (string) null;
      string uniqueKey = cacheUniqueKey ?? string.Empty;
      string filePath = Path.Combine(cacheRoot, "cachefoldermap.txt");
      if (!Safe.WriteToFileStream(filePath, (Action<FileStream>) (fs =>
      {
        Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
        Dictionary<string, string> dictionary2 = new StreamReader((Stream) fs).ReadToEnd().FromJson<Dictionary<string, string>>() ?? dictionary1;
        string path2;
        if (!dictionary2.TryGetValue(uniqueKey, out path2))
        {
          int num = 0;
          do
          {
            path2 = "CF{0}".InvariantFormat((object) ++num);
          }
          while (dictionary2.ContainsValue(path2));
          dictionary2.Add(uniqueKey, path2);
          fs.Seek(0L, SeekOrigin.Begin);
          StreamWriter streamWriter = new StreamWriter((Stream) fs);
          string json = dictionary2.ToJson();
          streamWriter.Write(json);
          streamWriter.Flush();
        }
        cachePath = Path.Combine(cacheRoot, path2);
      })))
        throw new BuildWorkflowException("Could not get a unique lock on: {0}".InvariantFormat((object) filePath));
      return !string.IsNullOrWhiteSpace(cachePath) && !(cachePath == cacheRoot) ? cachePath : throw new BuildWorkflowException("Could not find a valid cache folder in: {0}".InvariantFormat((object) cacheRoot));
    }

    public ICacheSection CurrentCacheSection => this.currentCacheSection;

    public IDictionary<string, ReadOnlyCacheSection> LoadedCacheSections => this.loadedCacheSections;

    public string RootPath => this.cacheRootPath;

    public ICacheSection BeginSection(WebGreaseSectionKey webGreaseSectionKey, bool autoLoad = true) => this.currentCacheSection = (ICacheSection) CacheSection.Begin(this.context, webGreaseSectionKey.Category, webGreaseSectionKey.Value, this.CurrentCacheSection, autoLoad);

    public void CleanUp()
    {
      DateTime utcDateTime = this.context.SessionStartTime.UtcDateTime;
      if (this.context.Configuration.CacheTimeout.TotalSeconds <= 0.0)
        return;
      DateTime expireTime = utcDateTime - this.context.Configuration.CacheTimeout;
      foreach (string path in ((IEnumerable<string>) Directory.GetFiles(this.cacheRootPath, "*.*", SearchOption.AllDirectories)).Where<string>((Func<string, bool>) (f => File.GetLastWriteTimeUtc(f) < expireTime)))
      {
        try
        {
          File.Delete(path);
        }
        catch (Exception ex)
        {
        }
      }
    }

    public void EndSection(ICacheSection cacheSection)
    {
      if (this.CurrentCacheSection != cacheSection)
        return;
      this.currentCacheSection = cacheSection.Parent;
    }

    public string GetAbsoluteCacheFilePath(string category, string fileName) => Path.Combine(this.cacheRootPath, category, fileName);

    public void SetContext(IWebGreaseContext newContext) => this.context = newContext;

    public string StoreInCache(string cacheCategory, ContentItem contentItem)
    {
      string contentHash = contentItem.GetContentHash(this.context);
      string str = Path.GetExtension(contentItem.RelativeContentPath) ?? ".txt";
      string absoluteCacheFilePath = this.GetAbsoluteCacheFilePath(cacheCategory, contentHash + str);
      contentItem.WriteTo(absoluteCacheFilePath);
      return absoluteCacheFilePath;
    }
  }
}
