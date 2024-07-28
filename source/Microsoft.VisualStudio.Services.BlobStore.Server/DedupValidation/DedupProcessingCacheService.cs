// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation.DedupProcessingCacheService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DedupValidation
{
  public class DedupProcessingCacheService : 
    VssCacheService,
    IVssFrameworkService,
    IDedupProcessingCache,
    ISizeProvider<DedupIdentifier, IDedupInfo>,
    ISizeProvider<Tuple<DedupIdentifier, DedupIdentifier>, byte>
  {
    private string m_name;
    private VssMemoryCacheList<DedupIdentifier, IDedupInfo> infoCache;
    private VssMemoryCacheList<Tuple<DedupIdentifier, DedupIdentifier>, byte> validationCache;
    private const int MaxInfoEntries = 25000000;
    private const int MaxPairEntries = 25000000;
    private const long MaxSize = 2684354560;
    private const int InfoKeyValueSize = 58;
    private const int PairKeyValueSize = 17;
    private const int PairValidated = 1;

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      this.infoCache = new VssMemoryCacheList<DedupIdentifier, IDedupInfo>((IVssCachePerformanceProvider) this, 25000000, 2684354560L, (ISizeProvider<DedupIdentifier, IDedupInfo>) this);
      this.validationCache = new VssMemoryCacheList<Tuple<DedupIdentifier, DedupIdentifier>, byte>((IVssCachePerformanceProvider) this, 25000000, 2684354560L, (ISizeProvider<Tuple<DedupIdentifier, DedupIdentifier>, byte>) this);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      this.Reset();
      base.ServiceEnd(requestContext);
    }

    public override string Name
    {
      get
      {
        if (this.m_name == null)
          this.m_name = "DPC-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        return this.m_name;
      }
    }

    public void AddValidatedParentChild(DedupIdentifier parent, DedupIdentifier child)
    {
      if (parent == (DedupIdentifier) null)
        return;
      this.validationCache.Add(new Tuple<DedupIdentifier, DedupIdentifier>(parent, child), (byte) 1, true);
    }

    public bool IsParentChildValidated(DedupIdentifier parent, DedupIdentifier child)
    {
      if (parent == (DedupIdentifier) null)
        return false;
      byte num = 0;
      if (!this.validationCache.TryGetValue(new Tuple<DedupIdentifier, DedupIdentifier>(parent, child), out num))
        return false;
      if (num == (byte) 1)
        return true;
      throw new Exception(string.Format("The validation cache for parent/child par {0}/{1} contains an invalid value: {2}", (object) parent, (object) child, (object) num));
    }

    public IDedupInfo GetDedupInfo(VssRequestPump.Processor processor, DedupIdentifier dedupId)
    {
      IDedupInfo dedupInfo;
      this.infoCache.TryGetValue(dedupId, out dedupInfo);
      return dedupInfo;
    }

    public void SetDedupInfo(
      VssRequestPump.Processor processor,
      DedupIdentifier dedupId,
      IDedupInfo info)
    {
      this.infoCache.Add(dedupId, info, true);
    }

    public long GetSize(DedupIdentifier key, IDedupInfo value) => 58;

    public long GetSize(
      Tuple<DedupIdentifier, DedupIdentifier> parentAndChild,
      byte exists)
    {
      return 17;
    }

    public void Reset()
    {
      this.infoCache.Clear();
      this.validationCache.Clear();
    }
  }
}
