// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.PolicyPermissionValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  public class PolicyPermissionValidator
  {
    private const string c_area = "TenantPolicy";
    private const string c_layer = "PolicyPermissionValidator";

    public virtual void CheckPermission(
      IVssRequestContext context,
      string policyName,
      Microsoft.VisualStudio.Services.Identity.Identity identity = null,
      Guid? tenantId = null,
      TenantPolicyPermissionType permissionType = TenantPolicyPermissionType.Write)
    {
      Guid spGuid;
      if ((!ServicePrincipals.IsServicePrincipal(context, context.UserContext) || !ServicePrincipals.TryParse(context.UserContext, out spGuid, out Guid _) || !(spGuid == Constants.GenevaServiceGuid) && !(spGuid == Constants.TokenServiceGuid) || permissionType != TenantPolicyPermissionType.Read) && !context.Items.TryGetValue("TenantPolicy.CheckPermission.Bypass", out object _) && !this.HasPermission(context, policyName, identity, tenantId, permissionType))
        throw new TenantPolicyPermissionValidationFailedException(FrameworkResources.TenantPolicyPermissionValidationFailed((object) identity?.Id, (object) policyName));
    }

    public virtual bool HasPermission(
      IVssRequestContext context,
      string policyName,
      Microsoft.VisualStudio.Services.Identity.Identity identity = null,
      Guid? tenantId = null,
      TenantPolicyPermissionType permissionType = TenantPolicyPermissionType.Write)
    {
      identity = identity == null ? context.GetUserIdentity() : identity;
      Guid tenantId1 = tenantId.HasValue ? tenantId.Value : AadIdentityHelper.ExtractTenantId((IReadOnlyVssIdentity) identity);
      return this.HasPermission(context, policyName, identity, identity.GetAadObjectId(), tenantId1, permissionType);
    }

    public virtual bool HasPermission(
      IVssRequestContext context,
      string policyName,
      Guid identityObjectId,
      Guid targetTenantId,
      TenantPolicyPermissionType permissionType = TenantPolicyPermissionType.Write)
    {
      return this.HasPermission(context, policyName, (Microsoft.VisualStudio.Services.Identity.Identity) null, identityObjectId, targetTenantId, permissionType);
    }

    private bool HasPermission(
      IVssRequestContext context,
      string policyName,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid identityObjectId,
      Guid tenantId,
      TenantPolicyPermissionType permissionType = TenantPolicyPermissionType.Write)
    {
      using (IDisposableReadOnlyList<ITenantPolicyPermissionExtension> extensions = context.GetExtensions<ITenantPolicyPermissionExtension>())
      {
        if (extensions.IsNullOrEmpty<ITenantPolicyPermissionExtension>())
        {
          context.TraceAlways(8524000, TraceLevel.Error, "TenantPolicy", nameof (PolicyPermissionValidator), "Expecting at least one ITenantPolicyPermissionExtension to evaluate permissions, but found zero.");
          throw new TenantPolicyPermissionValidatorNotFoundException();
        }
        foreach (ITenantPolicyPermissionExtension permissionExtension in (IEnumerable<ITenantPolicyPermissionExtension>) extensions)
        {
          ITenantPolicyPermissionExtension policyPermissionExtension = permissionExtension;
          if (policyPermissionExtension.HasPermission(context, policyName, identity, identityObjectId, tenantId, permissionType))
          {
            context.TraceConditionally(8524002, TraceLevel.Info, "TenantPolicy", nameof (PolicyPermissionValidator), (Func<string>) (() => "TenantPolicyPermission validation passed for " + policyPermissionExtension.GetType().Name));
            return true;
          }
          context.Trace(8524004, TraceLevel.Info, "TenantPolicy", nameof (PolicyPermissionValidator), "TenantPolicyPermission validation failed for " + policyPermissionExtension.GetType().Name);
        }
        return false;
      }
    }
  }
}
