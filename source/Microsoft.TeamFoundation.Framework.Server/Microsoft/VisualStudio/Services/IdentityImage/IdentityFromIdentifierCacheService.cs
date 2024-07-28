// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityImage.IdentityFromIdentifierCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityImage
{
  public class IdentityFromIdentifierCacheService : VssMemoryCacheService<string, Microsoft.VisualStudio.Services.Identity.Identity>
  {
    private const string RegistryRoot = "/WebAccess/TfIdFromIdentifierCache";
    internal static int MaxCacheValues = 200000;
    internal static readonly TimeSpan s_cacheCleanupInterval = new TimeSpan(0, 30, 0);

    public IdentityFromIdentifierCacheService()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, new MemoryCacheConfiguration<string, Microsoft.VisualStudio.Services.Identity.Identity>().WithCleanupInterval(IdentityFromIdentifierCacheService.s_cacheCleanupInterval))
    {
      this.ExpiryInterval.Value = TimeSpan.FromHours(24.0);
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.GetService<CachedRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/WebAccess/TfIdFromIdentifierCache/...");
      this.ReadRegistrySettings(requestContext);
      base.ServiceStart(requestContext);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      base.ServiceEnd(requestContext);
      requestContext.GetService<CachedRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    private void ReadRegistrySettings(IVssRequestContext requestContext)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      RegistryEntry registryEntry1 = service.ReadEntries(requestContext, (RegistryQuery) "/WebAccess/TfIdFromIdentifierCache/ExpiryIntervalInHours").FirstOrDefault<RegistryEntry>();
      int num1 = registryEntry1 != null ? registryEntry1.GetValue<int>() : 24;
      RegistryEntry registryEntry2 = service.ReadEntries(requestContext, (RegistryQuery) "/WebAccess/TfIdFromIdentifierCache/ExpiryDelayInHours").FirstOrDefault<RegistryEntry>();
      int num2 = registryEntry2 != null ? registryEntry2.GetValue<int>() : 0;
      service.ReadEntries(requestContext, (RegistryQuery) "/WebAccess/TfIdFromIdentifierCache/ClearCache").FirstOrDefault<RegistryEntry>()?.GetValue<bool>();
      RegistryEntry registryEntry3 = service.ReadEntries(requestContext, (RegistryQuery) "/WebAccess/TfIdFromIdentifierCache/maxCacheValues").FirstOrDefault<RegistryEntry>();
      if ((registryEntry3 != null ? registryEntry3.GetValue<int>() : IdentityFromIdentifierCacheService.MaxCacheValues) != this.MaxCacheLength.Value)
        this.MaxCacheLength.Value = IdentityFromIdentifierCacheService.MaxCacheValues;
      this.ExpiryInterval.Value = TimeSpan.FromHours((double) num1);
      if (num2 <= 0)
        return;
      this.ExpiryDelay.Value = DateTime.UtcNow + TimeSpan.FromHours((double) num2);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.ReadRegistrySettings(requestContext);
    }
  }
}
