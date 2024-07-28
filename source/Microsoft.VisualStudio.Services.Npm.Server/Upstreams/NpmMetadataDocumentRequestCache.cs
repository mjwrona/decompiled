// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Upstreams.NpmMetadataDocumentRequestCache
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Upstreams
{
  public class NpmMetadataDocumentRequestCache : 
    ICache<NpmPackageName, MetadataDocument<INpmMetadataEntry>>,
    IInvalidatableCache<NpmPackageName>
  {
    private readonly Dictionary<NpmPackageName, MetadataDocument<INpmMetadataEntry>> cache;
    private readonly Queue<NpmPackageName> cacheQueue;
    private readonly int maxEntries;

    public NpmMetadataDocumentRequestCache(int maxEntries)
    {
      this.cache = new Dictionary<NpmPackageName, MetadataDocument<INpmMetadataEntry>>();
      this.cacheQueue = new Queue<NpmPackageName>(maxEntries);
      this.maxEntries = maxEntries > 0 ? maxEntries : throw new ArgumentOutOfRangeException(nameof (maxEntries));
    }

    public bool Has(NpmPackageName key) => this.cache.ContainsKey(key);

    public void Invalidate(NpmPackageName key)
    {
      if (!this.cache.ContainsKey(key))
        return;
      this.cache.Remove(key);
    }

    public bool Set(NpmPackageName key, MetadataDocument<INpmMetadataEntry> val)
    {
      if (this.Has(key))
      {
        this.cache[key] = val;
        return true;
      }
      this.cache.Add(key, val);
      this.cacheQueue.Enqueue(key);
      if (this.cacheQueue.Count > this.maxEntries)
        this.EvictEntry();
      return true;
    }

    private void EvictEntry()
    {
      NpmPackageName key = this.cacheQueue.Dequeue();
      if (!this.cache.ContainsKey(key))
        return;
      this.cache.Remove(key);
    }

    public bool TryGet(NpmPackageName key, out MetadataDocument<INpmMetadataEntry> val) => this.cache.TryGetValue(key, out val);
  }
}
