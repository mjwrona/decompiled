// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GdprSyncMigrationHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class GdprSyncMigrationHelper
  {
    public static List<GdprHistoricalIdentity> GetGdprClaimsIdentities(
      IVssRequestContext requestContext)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      List<GdprHistoricalIdentity> historicalIdentityList = new List<GdprHistoricalIdentity>();
      using (IdentityManagementComponent component = context.CreateComponent<IdentityManagementComponent>())
        return component.FetchGdprClaimsIdentities().Select<IdentityManagementComponent.GdprClaimsIdentity, GdprHistoricalIdentity>((Func<IdentityManagementComponent.GdprClaimsIdentity, GdprHistoricalIdentity>) (ci => new GdprHistoricalIdentity(ci.Id, ci.Descriptor))).ToList<GdprHistoricalIdentity>();
    }
  }
}
