// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework.MigrationStateCacheOrgProtocolServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Caching;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework
{
  public class MigrationStateCacheOrgProtocolServiceFacade : 
    ICache<IProtocol, EtagValue<IEnumerable<MigrationEntry>>>,
    IInvalidatableCache<IProtocol>
  {
    private readonly IVssRequestContext requestContext;
    private readonly PackagingMigrationStatesPerProtocolCacheService migrationStatesPerProtocolCacheService;

    public MigrationStateCacheOrgProtocolServiceFacade(IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
      this.migrationStatesPerProtocolCacheService = this.requestContext.GetService<PackagingMigrationStatesPerProtocolCacheService>();
    }

    public bool TryGet(IProtocol key, out EtagValue<IEnumerable<MigrationEntry>> val) => this.migrationStatesPerProtocolCacheService.TryGet(this.requestContext, key, out val);

    public bool Has(IProtocol key) => this.migrationStatesPerProtocolCacheService.HasValue(this.requestContext, key);

    public bool Set(IProtocol key, EtagValue<IEnumerable<MigrationEntry>> val) => this.migrationStatesPerProtocolCacheService.SetValue(this.requestContext, key, val);

    public void Invalidate(IProtocol key) => this.migrationStatesPerProtocolCacheService.Invalidate(this.requestContext, key);
  }
}
