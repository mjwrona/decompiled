// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.ExtensionPerCategory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  public class ExtensionPerCategory
  {
    [JsonProperty("cn")]
    public string CategoryName { get; set; }

    [JsonProperty("e")]
    public List<BaseExtensionItem> Extensions { get; set; }

    [JsonProperty("hme")]
    public bool HasMoreExtensions { get; set; }

    [JsonProperty("sml")]
    public string SeeMoreLink { get; set; }

    internal ExtensionPerCategory(ExtensionPerCategory epc)
    {
      this.CategoryName = epc.CategoryName;
      this.HasMoreExtensions = epc.HasMoreExtensions;
      this.SeeMoreLink = epc.SeeMoreLink;
      if (epc.Extensions == null)
        return;
      this.Extensions = new List<BaseExtensionItem>();
      this.Extensions.AddRange((IEnumerable<BaseExtensionItem>) epc.Extensions);
    }

    public ExtensionPerCategory()
    {
    }
  }
}
