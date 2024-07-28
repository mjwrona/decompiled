// Decompiled with JetBrains decompiler
// Type: WebGrease.WebGreaseSectionKey
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WebGrease.Configuration;
using WebGrease.Extensions;

namespace WebGrease
{
  public class WebGreaseSectionKey
  {
    private const string CacheSectionFileVersionKey = "1.0.11";
    private const string Delimiter = "|";

    public WebGreaseSectionKey(
      IWebGreaseContext context,
      string category,
      ContentItem cacheVarByContentItem,
      object cacheVarBySetting,
      IFileSet cacheVarByFileSet,
      string uniqueKey = null)
    {
      this.Category = category;
      this.Value = uniqueKey;
      if (!string.IsNullOrWhiteSpace(uniqueKey))
        return;
      List<CacheVaryByFile> source = new List<CacheVaryByFile>();
      List<string> second = new List<string>();
      if (cacheVarByContentItem != null)
      {
        source.Add(CacheVaryByFile.FromFile(context, cacheVarByContentItem));
        second.Add(cacheVarByContentItem.ResourcePivotKeys.ToJson());
      }
      if (cacheVarByFileSet != null)
        second.Add(cacheVarByFileSet.ToJson());
      if (context.Configuration.Overrides != null)
        second.Add(context.Configuration.Overrides.UniqueKey);
      second.Add(cacheVarBySetting.ToJson(true));
      this.Value = "1.0.11|" + category + "|" + string.Join("|", source.Select<CacheVaryByFile, string>((Func<CacheVaryByFile, string>) (vbf => vbf.Hash)).Concat<string>((IEnumerable<string>) second));
    }

    public string Category { get; private set; }

    public string Value { get; private set; }
  }
}
