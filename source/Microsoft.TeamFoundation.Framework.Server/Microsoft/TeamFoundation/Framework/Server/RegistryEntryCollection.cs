// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryEntryCollection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class RegistryEntryCollection : IEnumerable<RegistryEntry>, IEnumerable
  {
    private readonly string m_rootPath;
    private readonly Dictionary<string, RegistryEntry> m_map = new Dictionary<string, RegistryEntry>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public RegistryEntryCollection(string rootPath, IEnumerable<RegistryItem> items)
      : this(rootPath, items.Select<RegistryItem, RegistryEntry>((Func<RegistryItem, RegistryEntry>) (s => new RegistryEntry(s.Path, s.Value))))
    {
    }

    public RegistryEntryCollection(string rootPath, IEnumerable<RegistryEntry> entries)
      : this(rootPath)
    {
      if (entries == null)
        return;
      foreach (RegistryEntry entry in entries)
        this.m_map[entry.Path] = entry;
    }

    public RegistryEntryCollection(string rootPath) => this.m_rootPath = rootPath;

    public RegistryEntry this[string path]
    {
      get
      {
        string absolute = RegistryUtility.ToAbsolute(this.m_rootPath, path);
        RegistryEntry registryEntry;
        if (!this.m_map.TryGetValue(absolute, out registryEntry))
        {
          registryEntry = new RegistryEntry(absolute, (string) null);
          this.m_map[absolute] = registryEntry;
        }
        return registryEntry;
      }
    }

    public int Count => this.m_map.Count;

    public bool ContainsPath(string path) => this.m_map.ContainsKey(RegistryUtility.ToAbsolute(this.m_rootPath, path));

    public bool TryGetValue(string path, out RegistryEntry entry) => this.m_map.TryGetValue(RegistryUtility.ToAbsolute(this.m_rootPath, path), out entry);

    public T GetValueFromPath<T>(string path, T defaultValue) => this.ContainsPath(path) ? this[path].GetValue<T>(defaultValue) : defaultValue;

    public Dictionary<string, RegistryEntry>.ValueCollection.Enumerator GetEnumerator() => this.m_map.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    IEnumerator<RegistryEntry> IEnumerable<RegistryEntry>.GetEnumerator() => (IEnumerator<RegistryEntry>) this.GetEnumerator();
  }
}
