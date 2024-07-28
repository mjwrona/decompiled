// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CommonLicenseCheckHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public static class CommonLicenseCheckHelper
  {
    public static bool IsStakeholder(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        AccountEntitlement accountEntitlement = requestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlement(requestContext, userIdentity.Id);
        return accountEntitlement != (AccountEntitlement) null && accountEntitlement.Rights != null && accountEntitlement.Rights.Level == VisualStudioOnlineServiceLevel.Stakeholder;
      }
      bool? nullable = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetExtension<IStakeholderLicenseAdapter>(ExtensionLifetime.Service).HasStakeholderRights(requestContext, requestContext.RootContext.UserContext);
      return nullable.HasValue && nullable.HasValue && nullable.Value;
    }
  }
}
