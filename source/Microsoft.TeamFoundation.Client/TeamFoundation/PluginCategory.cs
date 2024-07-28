// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PluginCategory
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation
{
  public class PluginCategory
  {
    private string name;
    private List<PluginRecord> pluginRecords;

    public string Name => this.name;

    public PluginRecord[] PluginRecords => this.pluginRecords.ToArray();

    public PluginRecord this[string pluginId]
    {
      get
      {
        string y = pluginId != null ? pluginId.Trim() : throw new ArgumentNullException(nameof (pluginId));
        if (y.Length == 0)
          throw new ArgumentException((string) null, nameof (pluginId));
        foreach (PluginRecord pluginRecord in this.PluginRecords)
        {
          if (VssStringComparer.PlugInId.Equals(pluginRecord.Id, y))
            return pluginRecord;
        }
        return (PluginRecord) null;
      }
    }

    internal PluginCategory(string categoryName, List<PluginRecord> pluginRecords)
    {
      this.name = categoryName;
      this.pluginRecords = pluginRecords;
    }
  }
}
