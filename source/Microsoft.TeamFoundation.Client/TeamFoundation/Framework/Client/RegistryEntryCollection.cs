// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistryEntryCollection
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class RegistryEntryCollection : IEnumerable<RegistryEntry>, IEnumerable
  {
    private string m_rootPath;
    private IDictionary<string, RegistryEntry> m_map;

    public RegistryEntryCollection(string rootPath)
      : this(rootPath, (ICollection<RegistryEntry>) null)
    {
    }

    internal RegistryEntryCollection(string rootPath, ICollection<RegistryEntry> entries)
    {
      this.m_rootPath = rootPath;
      if (entries == null)
        entries = (ICollection<RegistryEntry>) Array.Empty<RegistryEntry>();
      this.m_map = (IDictionary<string, RegistryEntry>) new Dictionary<string, RegistryEntry>(entries.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry entry in (IEnumerable<RegistryEntry>) entries)
        this.m_map.Add(entry.Path, entry);
    }

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

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_map.Values.GetEnumerator();

    IEnumerator<RegistryEntry> IEnumerable<RegistryEntry>.GetEnumerator() => this.m_map.Values.GetEnumerator();
  }
}
