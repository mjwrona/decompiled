// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.ConfigExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  public static class ConfigExtensions
  {
    public static T QueryByCtx<T>(
      this IConfigQueryable<T> config,
      IVssRequestContext requestContext)
    {
      Query query = Query.ByCtx(requestContext);
      return config.Query(requestContext, in query);
    }

    public static T QueryByTenantId<T>(
      this IConfigQueryable<T> config,
      IVssRequestContext requestContext,
      Guid tenantId)
    {
      Query query = Query.ByTenantId(tenantId);
      return config.Query(requestContext, in query);
    }

    public static T QueryByHostId<T>(
      this IConfigQueryable<T> config,
      IVssRequestContext requestContext,
      Guid hostId)
    {
      Query query = Query.ByHostId(hostId);
      return config.Query(requestContext, in query);
    }

    public static T QueryById<T>(
      this IConfigQueryable<T> config,
      IVssRequestContext requestContext,
      Guid vsid)
    {
      Query query = Query.ById(vsid);
      return config.Query(requestContext, in query);
    }

    public static T QueryByIds<T>(
      this IConfigQueryable<T> config,
      IVssRequestContext requestContext,
      Guid vsid,
      Guid hostId)
    {
      Query query = Query.ByIds(vsid, hostId);
      return config.Query(requestContext, in query);
    }
  }
}
