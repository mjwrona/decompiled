// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.StrongBoxExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class StrongBoxExtensions
  {
    public static Guid GetOrCreateDrawer(
      this ITeamFoundationStrongBoxService strongBoxService,
      IVssRequestContext requestContext,
      string drawerName)
    {
      Guid drawer = strongBoxService.UnlockDrawer(requestContext, drawerName, false);
      if (drawer == Guid.Empty)
      {
        try
        {
          drawer = strongBoxService.CreateDrawer(requestContext, drawerName);
        }
        catch (StrongBoxDrawerExistsException ex)
        {
          drawer = strongBoxService.UnlockDrawer(requestContext, drawerName, false);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "Service", ex);
          throw;
        }
      }
      return drawer;
    }
  }
}
