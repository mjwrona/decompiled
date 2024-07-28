// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.PluginModuleLookup
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class PluginModuleLookup
  {
    private ConcurrentDictionary<string, HashSet<string>> m_dictionary = new ConcurrentDictionary<string, HashSet<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public IEnumerable<string> GetPluginsForModule(string module)
    {
      HashSet<string> stringSet;
      return this.m_dictionary.TryGetValue(module, out stringSet) ? (IEnumerable<string>) stringSet : Enumerable.Empty<string>();
    }

    public void AddPlugin(string module, string plugin) => this.m_dictionary.AddOrUpdate(module, (Func<string, HashSet<string>>) (m => new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      plugin
    }), (Func<string, HashSet<string>, HashSet<string>>) ((m, plugins) =>
    {
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      stringSet.Add(plugin);
      stringSet.UnionWith((IEnumerable<string>) plugins);
      return stringSet;
    }));
  }
}
