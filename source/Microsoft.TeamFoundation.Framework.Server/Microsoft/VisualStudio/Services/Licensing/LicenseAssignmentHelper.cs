// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicenseAssignmentHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.GroupLicensingRule;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class LicenseAssignmentHelper
  {
    private const string Area = "LicenseAssignmentHelper";
    private const string Layer = "LicenseAssignmentHelper";
    private const string ApplyRulesToNewUsersFeature = "VisualStudio.Services.GroupLicensingRule.ApplyRulesToNewUsers";

    public static AccountEntitlement AssignLicenseToIdentity(
      IVssRequestContext requestContext,
      Guid userId,
      bool isPublicResource,
      bool determineRights)
    {
      if (userId == Guid.Empty)
        return (AccountEntitlement) null;
      requestContext.TraceDataConditionally(1446401, TraceLevel.Info, nameof (LicenseAssignmentHelper), nameof (LicenseAssignmentHelper), string.Format("Assigning available license to authenticated user {0} in service host {1} with determine rights {2}", (object) userId, (object) requestContext.ServiceHost.InstanceId, (object) determineRights), (Func<object>) (() => (object) Environment.StackTrace), nameof (AssignLicenseToIdentity));
      return !isPublicResource ? LicenseAssignmentHelper.AssignBestAvailableLicense(requestContext, userId, LicensingOrigin.OnDemandPrivateProject) : LicenseAssignmentHelper.AssignBestAvailableLicense(requestContext, userId, LicensingOrigin.OnDemandPublicProject);
    }

    public static AccountEntitlement AssignLicenseToIdentity(
      IVssRequestContext requestContext,
      Guid userId,
      bool determineRights = true)
    {
      return userId == Guid.Empty ? (AccountEntitlement) null : LicenseAssignmentHelper.AssignLicenseToIdentity(requestContext, userId, requestContext.IsPublicResourceLicense(), determineRights);
    }

    private static AccountEntitlement AssignBestAvailableLicense(
      IVssRequestContext requestContext,
      Guid userId,
      LicensingOrigin origin)
    {
      ILicensingEntitlementService service = requestContext.GetService<ILicensingEntitlementService>();
      try
      {
        return requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("VisualStudio.Services.GroupLicensingRule.ApplyRulesToNewUsers") && origin == LicensingOrigin.OnDemandPrivateProject ? requestContext.GetService<IGroupLicensingService>().ApplyGroupLicensingRulesToUser(requestContext.Elevate(), userId) : service.AssignAvailableAccountEntitlement(requestContext.Elevate(), userId, origin);
      }
      catch (ServiceOwnerNotFoundException ex) when (!requestContext.ServiceHost.IsProduction)
      {
        return new AccountEntitlement()
        {
          UserId = userId,
          License = License.Auto,
          Rights = new AccountRights(VisualStudioOnlineServiceLevel.AdvancedPlus)
        };
      }
    }
  }
}
