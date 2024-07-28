// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PluginCatalogImpl
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation
{
  public class PluginCatalogImpl : PluginCatalog
  {
    private List<PluginCatalogHelper> catalogHelpers = new List<PluginCatalogHelper>();

    public override string[] CategoryNames
    {
      get
      {
        List<string> names = new List<string>();
        foreach (PluginCatalogHelper catalogHelper in this.catalogHelpers)
          catalogHelper.FillCategoryNames((IList<string>) names);
        return names.ToArray();
      }
    }

    public override PluginCategory this[string categoryName]
    {
      get
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(categoryName, nameof (categoryName));
        string categoryName1 = categoryName.Trim();
        List<PluginRecord> pluginRecords = new List<PluginRecord>();
        foreach (PluginCatalogHelper catalogHelper in this.catalogHelpers)
          catalogHelper.FillPluginCategory(categoryName1, this.includeDisabled, (IList<PluginRecord>) pluginRecords);
        return new PluginCategory(categoryName1, pluginRecords);
      }
    }

    public override void Refresh()
    {
      foreach (PluginCatalogHelper catalogHelper in this.catalogHelpers)
        catalogHelper.Refresh();
    }

    public PluginCatalogImpl(PluginCatalogHelper catalogHelper)
    {
      if (catalogHelper == null)
        throw new ArgumentNullException(nameof (catalogHelper));
      this.catalogHelpers.Add(catalogHelper);
    }

    public PluginCatalogImpl(PluginCatalogHelper[] catalogHelpers)
    {
      if (catalogHelpers == null)
        throw new ArgumentNullException(nameof (catalogHelpers));
      foreach (PluginCatalogHelper catalogHelper in catalogHelpers)
        this.catalogHelpers.Add(catalogHelper);
    }

    public PluginCatalogImpl(PluginCatalog pluginCatalog, PluginCatalogHelper[] catalogHelpers)
    {
      if (pluginCatalog == null)
        throw new ArgumentNullException(nameof (pluginCatalog));
      if (catalogHelpers == null)
        throw new ArgumentNullException(nameof (catalogHelpers));
      if (!(pluginCatalog is PluginCatalogImpl))
        throw new ArgumentException(ClientResources.ArgumentNotSupportedClass((object) nameof (pluginCatalog)));
      this.catalogHelpers.AddRange((IEnumerable<PluginCatalogHelper>) (pluginCatalog as PluginCatalogImpl).catalogHelpers);
      foreach (PluginCatalogHelper catalogHelper in catalogHelpers)
        this.catalogHelpers.Add(catalogHelper);
    }
  }
}
