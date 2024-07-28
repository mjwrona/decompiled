// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PropertyContainerMemoryCacheService`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  internal class PropertyContainerMemoryCacheService<T> : 
    VssMemoryCacheService<Guid, PropertyContainerCacheEntry<T>>
    where T : ICloneable, IPropertyContainer
  {
    private static readonly TimeSpan DefaultExpiryInterval = TimeSpan.FromMinutes(25.0);
    private static readonly TimeSpan CleanupTimeInMinutes = TimeSpan.FromMinutes(5.0);
    private const string c_name = "PropertyContainerMemoryCache";

    internal PropertyContainerMemoryCacheService()
      : this(PropertyContainerMemoryCacheService<T>.CleanupTimeInMinutes)
    {
    }

    internal PropertyContainerMemoryCacheService(TimeSpan cleanupInterval)
      : base(cleanupInterval)
    {
      this.ExpiryInterval.Value = PropertyContainerMemoryCacheService<T>.DefaultExpiryInterval;
    }

    protected override void ServiceStart(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
      base.ServiceStart(context);
    }

    public override string Name => "PropertyContainerMemoryCache";
  }
}
