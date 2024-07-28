// Decompiled with JetBrains decompiler
// Type: WebGrease.WebGreaseSection
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WebGrease.Configuration;

namespace WebGrease
{
  public class WebGreaseSection : IWebGreaseSection, ICachableWebGreaseSection
  {
    private static readonly ConcurrentDictionary<string, object> SectionLocks = new ConcurrentDictionary<string, object>();
    private readonly bool isGroup;
    private readonly IWebGreaseContext context;
    private readonly string[] idParts;
    private object cacheVarBySetting;
    private bool cacheIsSkipable;
    private bool cacheInfiniteWaitForLock;
    private ContentItem cacheVarByContentItem;
    private IFileSet cacheVarByFileSet;
    private Func<ICacheSection, bool> restoreFromCacheAction;
    private Action<ICacheSection> whenSkippedAction;

    private WebGreaseSection(IWebGreaseContext context, string[] idParts, bool isGroup)
    {
      this.context = context;
      this.idParts = idParts;
      this.isGroup = isGroup;
    }

    public static IWebGreaseSection Create(
      IWebGreaseContext context,
      string[] idParts,
      bool isGroup)
    {
      return (IWebGreaseSection) new WebGreaseSection(context, idParts, isGroup);
    }

    public void Execute(Action action)
    {
      this.context.Measure.Start(this.isGroup, this.idParts);
      try
      {
        action();
      }
      finally
      {
        this.context.Measure.End(this.isGroup, this.idParts);
      }
    }

    public T Execute<T>(Func<T> action)
    {
      this.context.Measure.Start(this.isGroup, this.idParts);
      try
      {
        return action();
      }
      finally
      {
        this.context.Measure.End(this.isGroup, this.idParts);
      }
    }

    public ICachableWebGreaseSection MakeCachable(
      object varBySettings,
      bool isSkipable = false,
      bool infiniteWaitForLock = false)
    {
      this.MakeCachable((IFileSet) null, varBySettings, isSkipable, infiniteWaitForLock);
      return (ICachableWebGreaseSection) this;
    }

    public ICachableWebGreaseSection MakeCachable(
      ContentItem varByContentItem,
      object varBySettings = null,
      bool isSkipable = false,
      bool infiniteWaitForLock = false)
    {
      this.cacheVarByContentItem = varByContentItem;
      this.cacheVarBySetting = varBySettings;
      this.cacheIsSkipable = isSkipable;
      this.cacheInfiniteWaitForLock = infiniteWaitForLock;
      return (ICachableWebGreaseSection) this;
    }

    public ICachableWebGreaseSection MakeCachable(
      IFileSet varByFileSet,
      object varBySettings = null,
      bool isSkipable = false,
      bool infiniteWaitForLock = false)
    {
      this.cacheVarByFileSet = varByFileSet;
      this.cacheVarBySetting = varBySettings;
      this.cacheIsSkipable = isSkipable;
      this.cacheInfiniteWaitForLock = infiniteWaitForLock;
      return (ICachableWebGreaseSection) this;
    }

    public ICachableWebGreaseSection RestoreFromCacheAction(Func<ICacheSection, bool> action)
    {
      this.restoreFromCacheAction = action;
      return (ICachableWebGreaseSection) this;
    }

    public ICachableWebGreaseSection WhenSkipped(Action<ICacheSection> action)
    {
      this.whenSkippedAction = action;
      return (ICachableWebGreaseSection) this;
    }

    public bool Execute(Func<ICacheSection, bool> cachableSectionAction)
    {
      WebGreaseSectionKey webGreaseSectionKey = new WebGreaseSectionKey(this.context, WebGreaseContext.ToStringId((IEnumerable<string>) this.idParts), this.cacheVarByContentItem, this.cacheVarBySetting, this.cacheVarByFileSet);
      return Safe.Lock<bool>(WebGreaseSection.SectionLocks.GetOrAdd(webGreaseSectionKey.Value, new object()), this.cacheInfiniteWaitForLock ? int.MaxValue : 5000, (Func<bool>) (() =>
      {
        bool errorHasOccurred = false;
        EventHandler eventHandler = (EventHandler) ((param0, param1) => errorHasOccurred = true);
        this.context.Log.ErrorOccurred += eventHandler;
        ICacheSection cacheSection = this.context.Cache.BeginSection(webGreaseSectionKey);
        try
        {
          if (this.context.TemporaryIgnore(this.cacheVarByFileSet, this.cacheVarByContentItem) && !errorHasOccurred)
          {
            cacheSection.Save();
            return true;
          }
          cacheSection.Load();
          if (this.cacheIsSkipable && cacheSection.CanBeSkipped())
          {
            if (this.whenSkippedAction != null)
              this.whenSkippedAction(cacheSection);
            if (!errorHasOccurred)
              return true;
          }
          if (this.restoreFromCacheAction != null && cacheSection.CanBeRestoredFromCache() && this.restoreFromCacheAction(cacheSection) && !errorHasOccurred)
            return true;
          this.context.Measure.Start(this.isGroup, this.idParts);
          try
          {
            if (!cachableSectionAction(cacheSection) || errorHasOccurred)
              return false;
            cacheSection.Save();
            return true;
          }
          finally
          {
            this.context.Measure.End(this.isGroup, this.idParts);
          }
        }
        finally
        {
          this.context.Log.ErrorOccurred -= eventHandler;
          cacheSection.EndSection();
        }
      }));
    }
  }
}
