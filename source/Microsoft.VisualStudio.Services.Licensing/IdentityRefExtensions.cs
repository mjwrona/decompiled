// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.IdentityRefExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class IdentityRefExtensions
  {
    public static AccountEntitlement WithIdentityRef(
      this AccountEntitlement accountEntitlement,
      IVssRequestContext requestContext)
    {
      if (accountEntitlement == (AccountEntitlement) null)
        return (AccountEntitlement) null;
      return ((IEnumerable<AccountEntitlement>) new AccountEntitlement[1]
      {
        accountEntitlement
      }).WithIdentityRefs(requestContext).Single<AccountEntitlement>();
    }

    public static IReadOnlyCollection<AccountEntitlement> WithIdentityRefs(
      this IEnumerable<AccountEntitlement> accountEntitlements,
      IVssRequestContext requestContext)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      List<AccountEntitlement> list1 = accountEntitlements != null ? accountEntitlements.Where<AccountEntitlement>((Func<AccountEntitlement, bool>) (x => x != (AccountEntitlement) null)).ToList<AccountEntitlement>() : (List<AccountEntitlement>) null;
      if (!list1.Any<AccountEntitlement>())
        return (IReadOnlyCollection<AccountEntitlement>) new List<AccountEntitlement>();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> second = service.ReadIdentities(requestContext, (IList<Guid>) list1.Select<AccountEntitlement, Guid>((Func<AccountEntitlement, Guid>) (x => x.UserId)).ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
      List<AccountEntitlement> list2 = list1.Zip<AccountEntitlement, Microsoft.VisualStudio.Services.Identity.Identity, AccountEntitlement>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) second, (Func<AccountEntitlement, Microsoft.VisualStudio.Services.Identity.Identity, AccountEntitlement>) ((entitlement, identity) =>
      {
        if (identity != null)
        {
          IdentityRef identityRef = identity.ToIdentityRef(requestContext, false);
          identityRef.ImageUrl = LicensingEntitlementUtil.GetImageResourceUrl(requestContext, identity.Id);
          identityRef.Url = IdentityHelper.GetIdentityResourceUriString(requestContext, identity.Id);
          entitlement.UserId = Guid.Parse(identityRef.Id);
          entitlement.User = identityRef;
        }
        return entitlement;
      })).ToList<AccountEntitlement>();
      foreach (AccountEntitlement accountEntitlement in list2.Where<AccountEntitlement>((Func<AccountEntitlement, bool>) (x => x.User == null)))
        requestContext.Trace(1039471, TraceLevel.Warning, "Licensing", nameof (IdentityRefExtensions), string.Format("Could not find entitlement by user ID {0}", (object) accountEntitlement.UserId));
      return (IReadOnlyCollection<AccountEntitlement>) list2.Where<AccountEntitlement>((Func<AccountEntitlement, bool>) (x => x.User != null)).ToList<AccountEntitlement>();
    }
  }
}
