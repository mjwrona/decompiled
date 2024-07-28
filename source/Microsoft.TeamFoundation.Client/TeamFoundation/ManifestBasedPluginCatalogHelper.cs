// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ManifestBasedPluginCatalogHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation
{
  internal class ManifestBasedPluginCatalogHelper : PluginCatalogHelper
  {
    private bool includeCurrentUserPlugins;

    internal ManifestBasedPluginCatalogHelper(bool includeCurrentUserPlugins) => this.includeCurrentUserPlugins = includeCurrentUserPlugins;

    public override void FillCategoryNames(IList<string> names) => throw new NotImplementedException();

    public override void FillPluginCategory(
      string categoryName,
      bool includeDisabled,
      IList<PluginRecord> pluginRecords)
    {
      throw new NotImplementedException();
    }

    public override void Refresh() => throw new NotImplementedException();
  }
}
