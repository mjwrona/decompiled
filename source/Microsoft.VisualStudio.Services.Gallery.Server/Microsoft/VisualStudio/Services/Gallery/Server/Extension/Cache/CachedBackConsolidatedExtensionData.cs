// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.CachedBackConsolidatedExtensionData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  public class CachedBackConsolidatedExtensionData
  {
    public CachedBackConsolidatedExtensionData() => this.VsixIdToExtensionMap = (IDictionary<string, BackConsolidationMappingEntry>) new Dictionary<string, BackConsolidationMappingEntry>();

    public IDictionary<string, BackConsolidationMappingEntry> VsixIdToExtensionMap { get; private set; }

    public virtual void AddNewExtensionMappingInCache(
      IReadOnlyDictionary<string, BackConsolidationMappingEntry> entries)
    {
      if (entries == null)
        return;
      foreach (KeyValuePair<string, BackConsolidationMappingEntry> entry in (IEnumerable<KeyValuePair<string, BackConsolidationMappingEntry>>) entries)
        this.VsixIdToExtensionMap.TryAdd<string, BackConsolidationMappingEntry>(entry.Key, entry.Value);
    }
  }
}
