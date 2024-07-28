// Decompiled with JetBrains decompiler
// Type: WebGrease.ReadOnlyCacheSection
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebGrease.Css.Extensions;
using WebGrease.Extensions;

namespace WebGrease
{
  public class ReadOnlyCacheSection
  {
    private static readonly object LoadLock = new object();
    private readonly string absolutePath;
    private IWebGreaseContext context;
    private IEnumerable<CacheSourceDependency> sourceDependencies;
    private IEnumerable<CacheResult> cacheResults;
    private IEnumerable<string> childCacheSectionFiles;
    private IEnumerable<ReadOnlyCacheSection> childCacheSections;
    private bool disposed;
    private int referenceCount;

    private ReadOnlyCacheSection(string jsonString, IWebGreaseContext context)
    {
      this.context = context;
      JObject jobject = JObject.Parse(jsonString);
      this.sourceDependencies = jobject[nameof (sourceDependencies)].ToString().FromJson<IEnumerable<CacheSourceDependency>>(true);
      this.cacheResults = jobject[nameof (cacheResults)].ToString().FromJson<IEnumerable<CacheResult>>(true);
      this.childCacheSectionFiles = jobject["children"].AsEnumerable<JToken>().Select<JToken, string>((Func<JToken, string>) (f => (string) f));
      this.absolutePath = (string) jobject[nameof (absolutePath)];
    }

    private IEnumerable<ReadOnlyCacheSection> ChildCacheSections => this.childCacheSections ?? (this.childCacheSections = (IEnumerable<ReadOnlyCacheSection>) this.childCacheSectionFiles.Select<string, ReadOnlyCacheSection>((Func<string, ReadOnlyCacheSection>) (childCacheSectionFile => ReadOnlyCacheSection.Load(childCacheSectionFile, this.context))).ToArray<ReadOnlyCacheSection>());

    internal static ReadOnlyCacheSection Load(string fullPath, IWebGreaseContext context) => !File.Exists(fullPath) ? (ReadOnlyCacheSection) null : Safe.Lock<ReadOnlyCacheSection>(ReadOnlyCacheSection.LoadLock, (Func<ReadOnlyCacheSection>) (() =>
    {
      ReadOnlyCacheSection onlyCacheSection;
      if (!context.Cache.LoadedCacheSections.TryGetValue(fullPath, out onlyCacheSection))
      {
        onlyCacheSection = new ReadOnlyCacheSection(File.ReadAllText(fullPath), context);
        context.Cache.LoadedCacheSections.Add(fullPath, onlyCacheSection);
      }
      ++onlyCacheSection.referenceCount;
      return onlyCacheSection;
    }));

    internal IEnumerable<CacheResult> GetCacheResults(string fileCategory = null, bool endResultOnly = false) => this.cacheResults.Where<CacheResult>((Func<CacheResult, bool>) (cr =>
    {
      if (endResultOnly && !cr.EndResult)
        return false;
      return fileCategory == null || cr.FileCategory == fileCategory;
    })).Concat<CacheResult>(this.ChildCacheSections.SelectMany<ReadOnlyCacheSection, CacheResult>((Func<ReadOnlyCacheSection, IEnumerable<CacheResult>>) (css => css.GetCacheResults(fileCategory, endResultOnly))));

    internal void Dispose()
    {
      if (this.disposed)
        throw new BuildWorkflowException("Cannot dispose an object twice.");
      if (!ReadOnlyCacheSection.Unload(this.context, this.absolutePath))
        return;
      this.disposed = true;
      if (this.childCacheSections != null)
        this.childCacheSections.Where<ReadOnlyCacheSection>((Func<ReadOnlyCacheSection, bool>) (ccs => ccs != null)).ForEach<ReadOnlyCacheSection>((Action<ReadOnlyCacheSection>) (ccs => ccs.Dispose()));
      this.context = (IWebGreaseContext) null;
      this.cacheResults = (IEnumerable<CacheResult>) null;
      this.sourceDependencies = (IEnumerable<CacheSourceDependency>) null;
      this.childCacheSections = (IEnumerable<ReadOnlyCacheSection>) null;
      this.childCacheSectionFiles = (IEnumerable<string>) null;
    }

    internal bool CanBeRestoredFromCache()
    {
      foreach (ReadOnlyCacheSection onlyCacheSection in ((IEnumerable<ReadOnlyCacheSection>) new ReadOnlyCacheSection[1]
      {
        this
      }).Concat<ReadOnlyCacheSection>(this.SafeAllRecursiveChildSections()))
      {
        if (onlyCacheSection == null || onlyCacheSection.cacheResults.Any<CacheResult>((Func<CacheResult, bool>) (cr => cr == null || !File.Exists(cr.CachedFilePath))) || onlyCacheSection.HasChangedSourceDependencies())
          return false;
      }
      return true;
    }

    internal bool CanBeSkipped()
    {
      IEnumerable<ReadOnlyCacheSection> onlyCacheSections = ((IEnumerable<ReadOnlyCacheSection>) new ReadOnlyCacheSection[1]
      {
        this
      }).Concat<ReadOnlyCacheSection>(this.SafeAllRecursiveChildSections());
      List<ReadOnlyCacheSection> onlyCacheSectionList = new List<ReadOnlyCacheSection>();
      bool flag = false;
      foreach (ReadOnlyCacheSection onlyCacheSection in onlyCacheSections)
      {
        if (onlyCacheSection == null)
          return false;
        CacheResult[] array = onlyCacheSection.cacheResults.Where<CacheResult>((Func<CacheResult, bool>) (cr => cr.EndResult)).ToArray<CacheResult>();
        if (((IEnumerable<CacheResult>) array).Any<CacheResult>())
        {
          flag = true;
          if (((IEnumerable<CacheResult>) array).Any<CacheResult>((Func<CacheResult, bool>) (cr => ReadOnlyCacheSection.HasCachedEndResultThatChanged(this.context, cr))))
            return false;
        }
        if (onlyCacheSection.sourceDependencies.Any<CacheSourceDependency>((Func<CacheSourceDependency, bool>) (sd => sd == null || sd.HasChanged(this.context))))
          return false;
        onlyCacheSectionList.Add(onlyCacheSection);
      }
      if (flag)
      {
        onlyCacheSectionList.ForEach((Action<ReadOnlyCacheSection>) (scc => scc.Touch()));
        this.Touch();
      }
      return flag;
    }

    private static bool Unload(IWebGreaseContext context, string fullPath) => Safe.Lock<bool>(ReadOnlyCacheSection.LoadLock, (Func<bool>) (() =>
    {
      ReadOnlyCacheSection onlyCacheSection;
      if (context.Cache.LoadedCacheSections.TryGetValue(fullPath, out onlyCacheSection))
      {
        --onlyCacheSection.referenceCount;
        if (onlyCacheSection.referenceCount != 0)
          return false;
        context.Cache.LoadedCacheSections.Remove(fullPath);
      }
      return true;
    }));

    private static bool HasCachedEndResultThatChanged(IWebGreaseContext context, CacheResult r)
    {
      if (r == null)
        return true;
      string str = Path.Combine(context.Configuration.DestinationDirectory, r.RelativeHashedContentPath ?? r.RelativeContentPath);
      return !File.Exists(str) || !r.ContentHash.Equals(context.GetFileHash(str));
    }

    private void Touch()
    {
      this.context.Touch(this.absolutePath);
      this.cacheResults.ForEach<CacheResult>((Action<CacheResult>) (cr => this.context.Touch(cr.CachedFilePath)));
    }

    private bool HasChangedSourceDependencies() => this.sourceDependencies.Any<CacheSourceDependency>((Func<CacheSourceDependency, bool>) (sd => sd == null || sd.HasChanged(this.context)));

    private IEnumerable<ReadOnlyCacheSection> SafeAllRecursiveChildSections() => this.ChildCacheSections.Concat<ReadOnlyCacheSection>(this.ChildCacheSections.SelectMany<ReadOnlyCacheSection, ReadOnlyCacheSection>((Func<ReadOnlyCacheSection, IEnumerable<ReadOnlyCacheSection>>) (css => css?.SafeAllRecursiveChildSections())));
  }
}
