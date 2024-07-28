// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionCategory
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class ExtensionCategory
  {
    public string CategoryName { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ParentCategoryName { get; set; }

    public List<string> AssociatedProducts { get; set; }

    internal int MigratedId { get; set; }

    internal int ParentId { get; set; }

    internal ExtensionCategory Parent { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Language { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int CategoryId { get; set; }

    public List<CategoryLanguageTitle> LanguageTitles { get; set; }

    internal ExtensionCategory ShallowCopy() => (ExtensionCategory) this.MemberwiseClone();

    public string GetCategoryTitleForLanguage(string language)
    {
      if (this.LanguageTitles != null && this.LanguageTitles.Count > 0)
      {
        foreach (CategoryLanguageTitle languageTitle in this.LanguageTitles)
        {
          if (languageTitle.Lang != null && languageTitle.Lang.Equals(language, StringComparison.OrdinalIgnoreCase))
            return languageTitle.Title;
        }
      }
      return (string) null;
    }
  }
}
