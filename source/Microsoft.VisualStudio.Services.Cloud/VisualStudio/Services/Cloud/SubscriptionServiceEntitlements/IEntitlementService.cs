// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements.IEntitlementService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements
{
  [DefaultServiceImplementation(typeof (EntitlementService))]
  public interface IEntitlementService : IVssFrameworkService
  {
    Task<List<Ev4Entitlement>> GetMyEntitlementsAsync(
      IVssRequestContext requestContext,
      string site,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
