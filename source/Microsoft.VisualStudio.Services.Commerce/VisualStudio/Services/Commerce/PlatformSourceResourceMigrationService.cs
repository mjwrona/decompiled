// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformSourceResourceMigrationService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class PlatformSourceResourceMigrationService : IVssFrameworkService
  {
    private const string Area = "Commerce";
    private const string Layer = "PlatformSourceResourceMigrationService";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      systemRequestContext.CheckHostedDeployment();
    }

    public IEnumerable<SubscriptionResourceUsage> GetResourceUsages(
      IVssRequestContext collectionContext,
      Guid updatedByIdentifier)
    {
      try
      {
        collectionContext.TraceEnter(0, "Commerce", nameof (PlatformSourceResourceMigrationService), nameof (GetResourceUsages));
        collectionContext.CheckProjectCollectionRequestContext();
        List<OfferSubscriptionInternal> list;
        using (CommerceMeteringComponent component = collectionContext.CreateComponent<CommerceMeteringComponent>())
          list = component.GetMeteredResources(new int?(), new byte?()).ToList<OfferSubscriptionInternal>();
        return (IEnumerable<SubscriptionResourceUsage>) list.Select<OfferSubscriptionInternal, SubscriptionResourceUsage>((Func<OfferSubscriptionInternal, SubscriptionResourceUsage>) (x => x.ToSubscriptionResourceUsage(updatedByIdentifier))).ToList<SubscriptionResourceUsage>();
      }
      finally
      {
        collectionContext.TraceLeave(0, "Commerce", nameof (PlatformSourceResourceMigrationService), nameof (GetResourceUsages));
      }
    }
  }
}
