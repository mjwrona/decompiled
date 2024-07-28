// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Query
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  public readonly struct Query
  {
    public Guid Id { get; }

    public Guid CollectionHostId { get; }

    public Guid TenantId { get; }

    public static Query Any() => new Query(Guid.Empty, Guid.Empty, Guid.Empty);

    public static Query ById(Guid id) => new Query(id, Guid.Empty, Guid.Empty);

    public static Query ByHostId(Guid id) => new Query(Guid.Empty, id, Guid.Empty);

    public static Query ByIds(Guid id, Guid hostId) => new Query(id, hostId, Guid.Empty);

    public static Query ByTenantId(Guid tenantId) => new Query(Guid.Empty, Guid.Empty, tenantId);

    public static Query ByCtx(IVssRequestContext ctx)
    {
      Guid hostId = Guid.Empty;
      Guid tenantId = ctx.GetTenantId();
      if (!ctx.ServiceHost.Is(TeamFoundationHostType.Deployment))
        hostId = ctx.ServiceHost.InstanceId;
      return new Query(ctx.GetUserId(), hostId, tenantId);
    }

    public Query(Guid id, Guid hostId, Guid tenantId)
    {
      this.Id = id;
      this.CollectionHostId = hostId;
      this.TenantId = tenantId;
    }
  }
}
