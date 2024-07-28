// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceMigrationDriverFactory
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class ResourceMigrationDriverFactory
  {
    public static IResourceMigrationDriver CreateDriver(
      IVssRequestContext requestContext,
      Guid targetInstanceType)
    {
      if (requestContext.IsSpsService())
      {
        if (targetInstanceType == CommerceServiceInstanceTypes.Commerce)
          return (IResourceMigrationDriver) new SpsToCommerceOfferSubscriptionMigrationDriver(requestContext);
      }
      else if (requestContext.IsCommerceService())
        throw new NotImplementedException("ResourceMigrationDrive is not implemented for Commerce Service.");
      return (IResourceMigrationDriver) null;
    }
  }
}
