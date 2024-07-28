// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Licensing.VssRequestContextLicensingExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Diagnostics;

namespace Microsoft.Azure.DevOps.Licensing
{
  public static class VssRequestContextLicensingExtensions
  {
    private const string s_area = "VssRequestContextLicensingExtensions";
    private const string s_layer = "BusinessLogic";

    public static AccountLicenseType GetRUCompatibleAccountLicenseType(
      this IVssRequestContext requestContext,
      bool cacheOnly = false)
    {
      AccountLicenseType accountLicenseType;
      try
      {
        Guid userId = requestContext.GetUserId();
        IVssRequestContext rootContext = requestContext.RootContext;
        bool flag1 = rootContext?.ServiceHost == null;
        bool flag2 = rootContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
        bool flag3 = rootContext.ServiceHost.Is(TeamFoundationHostType.Application);
        bool flag4 = requestContext.IsAnonymous();
        bool flag5 = requestContext.IsPublicUser();
        bool flag6 = ServicePrincipals.IsServicePrincipal(requestContext, requestContext.GetAuthenticatedDescriptor());
        if (flag1 | flag2 | flag3 | flag4 | flag5 | flag6)
        {
          requestContext.Trace(1039111, TraceLevel.Info, nameof (VssRequestContextLicensingExtensions), "BusinessLogic", "Skipping license check: UserId={0}, isServiceHostNull={1}, isDeployment={2}, isEnterprise={3}, isAnonymous={4}, isPublicUser={5}, isServicePrincipal={6}", (object) userId, (object) flag1, (object) flag2, (object) flag3, (object) flag4, (object) flag5, (object) flag6);
          return AccountLicenseType.None;
        }
        AccountEntitlement accountEntitlement1 = requestContext.GetAccountEntitlement(userId);
        accountLicenseType = (object) accountEntitlement1 != null ? accountEntitlement1.ToAccessLevel(requestContext).AccountLicenseType : AccountLicenseType.None;
        if (cacheOnly || accountLicenseType != AccountLicenseType.None)
          return accountLicenseType;
        requestContext.Trace(1039113, TraceLevel.Info, nameof (VssRequestContextLicensingExtensions), "BusinessLogic", "Making license check: UserId={0}, isServiceHostNull={1}, isDeployment={2}, isEnterprise={3}, isAnonymous={4}, isPublicUser={5}, isServicePrincipal={6}", (object) userId, (object) flag1, (object) flag2, (object) flag3, (object) flag4, (object) flag5, (object) flag6);
        AccountEntitlement accountEntitlement2 = requestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlement(requestContext, userId);
        accountLicenseType = (object) accountEntitlement2 != null ? accountEntitlement2.ToAccessLevel(requestContext).AccountLicenseType : AccountLicenseType.None;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1039110, nameof (VssRequestContextLicensingExtensions), "BusinessLogic", ex);
        throw;
      }
      return accountLicenseType;
    }
  }
}
