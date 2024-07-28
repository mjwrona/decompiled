// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Caching.PackagingMigrationStateCacheService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Caching
{
  public class PackagingMigrationStateCacheService : 
    PackagingMemoryCacheService<string, MigrationState>,
    IMigrationStateCacheService,
    IVssFrameworkService
  {
    private const int MaxElementsDefault = 10000;

    public PackagingMigrationStateCacheService()
      : base(MemoryCacheConfiguration<string, MigrationState>.Default.WithExpiryInterval(TimeSpan.FromHours(24.0)).WithCleanupInterval(TimeSpan.FromHours(12.0)).WithMaxElements(10000))
    {
    }

    protected override Guid PackagingInvalidationNotificationGuid => new Guid("1DB63E22-E0DA-4E20-A615-2C20673EA539");

    public override void Invalidate(IVssRequestContext requestContext, string key)
    {
      if (!this.IsEnabled(requestContext))
        return;
      base.Invalidate(requestContext, key);
    }

    public override bool TryGet(
      IVssRequestContext requestContext,
      string key,
      out MigrationState state)
    {
      if (this.IsEnabled(requestContext))
        return base.TryGet(requestContext, key, out state);
      state = (MigrationState) null;
      return false;
    }

    public override bool SetValue(
      IVssRequestContext requestContext,
      string key,
      MigrationState state)
    {
      return this.IsEnabled(requestContext) && base.SetValue(requestContext, key, state);
    }

    public override bool HasValue(IVssRequestContext requestContext, string key) => this.IsEnabled(requestContext) && base.HasValue(requestContext, key);

    private bool IsEnabled(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection);
  }
}
