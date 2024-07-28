// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.DailyBillingHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class DailyBillingHelper
  {
    private const string DailyOrganizationsRegistryPath = "/Service/Commerce/DailyBilling/Organization/";

    public static IEnumerable<Guid> GetDailyOrganizations(
      IVssRequestContext requestContext,
      DateTime today)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Service/Commerce/DailyBilling/Organization/*").Where<RegistryEntry>((Func<RegistryEntry, bool>) (x => DateTime.Parse(x.Value) < today)).Select<RegistryEntry, Guid>((Func<RegistryEntry, Guid>) (x => Guid.Parse(x.Name)));
    }

    public static void SetOrganizationBillingDate(
      IVssRequestContext requestContext,
      Guid organizationId,
      DateTime today)
    {
      IVssRegistryService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
      RegistryEntry registryEntry = service.ReadEntries(requestContext, (RegistryQuery) string.Format("{0}{1}", (object) "/Service/Commerce/DailyBilling/Organization/", (object) organizationId)).SingleOrDefault<RegistryEntry>();
      if (registryEntry == null)
        return;
      registryEntry.SetValue<DateTime>(today);
      service.WriteEntries(requestContext, (IEnumerable<RegistryEntry>) new List<RegistryEntry>()
      {
        registryEntry
      });
    }

    public static bool IsDailyBillingEnabledOrganization(
      IVssRequestContext requestContext,
      Guid organizationId)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) string.Format("{0}{1}", (object) "/Service/Commerce/DailyBilling/Organization/", (object) organizationId)).SingleOrDefault<RegistryEntry>() != null;
    }
  }
}
