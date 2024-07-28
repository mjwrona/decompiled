// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.Internal.OnPremNameResolutionCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.NameResolution.Server.Internal
{
  internal class OnPremNameResolutionCache : INameResolutionCache
  {
    private IReadOnlyDictionary<NameResolutionKey, NameResolutionEntry> m_entryCache;

    public OnPremNameResolutionCache(IEnumerable<NameResolutionEntry> entries)
    {
      Dictionary<NameResolutionKey, NameResolutionEntry> dictionary = new Dictionary<NameResolutionKey, NameResolutionEntry>((IEqualityComparer<NameResolutionKey>) NameResolutionKeyComparer.Instance);
      if (entries != null)
      {
        foreach (NameResolutionEntry entry in entries)
          dictionary.Add(new NameResolutionKey(entry.Namespace, entry.Name), entry);
      }
      this.m_entryCache = (IReadOnlyDictionary<NameResolutionKey, NameResolutionEntry>) dictionary;
    }

    public bool IncrementalUpdatesAllowed => false;

    public NameResolutionEntry Get(
      IVssRequestContext requestContext,
      string @namespace,
      string name)
    {
      NameResolutionEntry nameResolutionEntry;
      return !this.m_entryCache.TryGetValue(new NameResolutionKey(@namespace, name), out nameResolutionEntry) ? (NameResolutionEntry) null : nameResolutionEntry;
    }

    public void Set(IVssRequestContext requestContext, NameResolutionEntry entry) => throw new InvalidOperationException();

    public void Remove(IVssRequestContext requestContext, NameResolutionEntry entry) => throw new InvalidOperationException();
  }
}
