// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class SubscriptionHelper
  {
    internal static void WithUserInSubscriptionTenantContext(
      IVssRequestContext requestContext,
      Guid subscriptionTenantId,
      Action<IVssRequestContext> action)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext elevatedContext = vssRequestContext.Elevate();
      if (!vssRequestContext.GetService<PlatformSubscriptionService>().GetUserTenantsFromAAD(vssRequestContext, elevatedContext).Contains<Guid>(subscriptionTenantId))
        throw new AccessCheckException("Logged user does not have access to perform this operation.");
      string addressOfIdentity = requestContext.GetService<AzureResourceHelper>().GetLoginAddressOfIdentity(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity bindPendingIdentity = IdentityHelper.GetOrCreateBindPendingIdentity(vssRequestContext, subscriptionTenantId.ToString(), addressOfIdentity, callerName: nameof (WithUserInSubscriptionTenantContext));
      IVssRequestContext userContext = vssRequestContext.CreateUserContext(bindPendingIdentity);
      action(userContext);
    }
  }
}
