// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.MigrationStateCacheServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Caching;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public class MigrationStateCacheServiceFacade : 
    ICache<string, MigrationState>,
    IInvalidatableCache<string>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IMigrationStateCacheService packageCacheService;
    private readonly ITracerService tracerService;

    public MigrationStateCacheServiceFacade(
      IVssRequestContext requestContext,
      ITracerService tracerService)
    {
      this.requestContext = requestContext;
      this.packageCacheService = this.requestContext.GetService<IMigrationStateCacheService>();
      this.tracerService = tracerService;
    }

    public bool TryGet(string key, out MigrationState val)
    {
      using (this.tracerService.Enter((object) this, nameof (TryGet)))
        return this.packageCacheService.TryGet(this.requestContext, key, out val);
    }

    public bool Has(string key)
    {
      using (this.tracerService.Enter((object) this, nameof (Has)))
        return this.packageCacheService.HasValue(this.requestContext, key);
    }

    public bool Set(string key, MigrationState val)
    {
      using (this.tracerService.Enter((object) this, nameof (Set)))
        return this.packageCacheService.SetValue(this.requestContext, key, val);
    }

    public void Invalidate(string key)
    {
      using (this.tracerService.Enter((object) this, nameof (Invalidate)))
        this.packageCacheService.Invalidate(this.requestContext, key);
    }
  }
}
