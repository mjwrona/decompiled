// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.TenantPolicyRolloutHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  public static class TenantPolicyRolloutHelper
  {
    private const string c_area = "TenantPolicy";
    private const string c_layer = "TenantPolicyRolloutHelper";
    private const string c_privatePreviewTenants = "/Configuration/TenantPolicy/Rollout/PrivatePreviewTenants";
    private const string c_restrictGlobalPatPolicyPrivatePreviewTenants = "/Configuration/TenantPolicy/Rollout/RestrictGlobalPatPrivatePreviewTenants";
    private const string c_restrictFullScopePatPolicyPrivatePreviewTenants = "/Configuration/TenantPolicy/Rollout/RestrictFullScopePatPrivatePreviewTenants";
    private const string c_restrictPatLifespanPolicyPrivatePreviewTenants = "/Configuration/TenantPolicy/Rollout/RestrictPatLifespanPrivatePreviewTenants";
    private static PolicyConfigRolloutDriver policyConfigRolloutDriver = new PolicyConfigRolloutDriver();

    public static bool IsInPreview(IVssRequestContext requestContext, Guid tenantId) => TenantPolicyRolloutHelper.IsPublicPreviewEnabled(requestContext) || TenantPolicyRolloutHelper.IsInPrivatePreview(requestContext, tenantId);

    public static bool IsPublicPreviewEnabled(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.Services.TenantPolicy.EnablePublicPreview");

    public static bool IsInPrivatePreview(IVssRequestContext requestContext, Guid tenantId) => TenantPolicyRolloutHelper.IsPrivatePreviewEnabled(requestContext) && TenantPolicyRolloutHelper.IsInPrivatePreviewTenantList(requestContext, tenantId, "/Configuration/TenantPolicy/Rollout/PrivatePreviewTenants");

    public static bool IsPrivatePreviewEnabled(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.Services.TenantPolicy.EnablePrivatePreview");

    public static bool IsPolicyFeatureAvailable(
      IVssRequestContext requestContext,
      string policyName,
      Guid tenantId)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      return TenantPolicyRolloutHelper.IsPolicyPublicFeatureFlagEnabled(requestContext1, policyName) || TenantPolicyRolloutHelper.IsPolicyPrivateFeatureFlagEnabled(requestContext1, policyName) && (TenantPolicyRolloutHelper.IsInPrivatePreviewTenantList(requestContext1, tenantId, TenantPolicyRolloutHelper.GetPolicyPreviewRegistryKey(policyName)) || TenantPolicyRolloutHelper.policyConfigRolloutDriver.IsPolicyEnabledForTenant(requestContext1, policyName, tenantId));
    }

    private static bool IsPolicyPublicFeatureFlagEnabled(
      IVssRequestContext requestContext,
      string policyName)
    {
      string empty = string.Empty;
      string featureName;
      switch (policyName)
      {
        case "TenantPolicy.RestrictFullScopePersonalAccessToken":
          featureName = "VisualStudio.Services.TenantPolicy.RestrictFullScopePersonalAccessTokenPolicy";
          break;
        case "TenantPolicy.RestrictGlobalPersonalAccessToken":
          featureName = "VisualStudio.Services.TenantPolicy.RestrictGlobalPersonalAccessTokenPolicy";
          break;
        case "TenantPolicy.RestrictPersonalAccessTokenLifespan":
          featureName = "VisualStudio.Services.TenantPolicy.RestrictPersonalAccessTokenLifespanPolicy";
          break;
        case "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation":
          featureName = "VisualStudio.Services.TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocationPolicy";
          break;
        default:
          featureName = "VisualStudio.Services.TenantPolicy.EnablePublicPreview";
          break;
      }
      return requestContext.IsFeatureEnabled(featureName);
    }

    private static bool IsPolicyPrivateFeatureFlagEnabled(
      IVssRequestContext requestContext,
      string policyName)
    {
      string empty = string.Empty;
      string featureName;
      switch (policyName)
      {
        case "TenantPolicy.RestrictFullScopePersonalAccessToken":
          featureName = "VisualStudio.Services.TenantPolicy.PrivatePreview.RestrictFullScopePersonalAccessTokenPolicy";
          break;
        case "TenantPolicy.RestrictGlobalPersonalAccessToken":
          featureName = "VisualStudio.Services.TenantPolicy.PrivatePreview.RestrictGlobalPersonalAccessTokenPolicy";
          break;
        case "TenantPolicy.RestrictPersonalAccessTokenLifespan":
          featureName = "VisualStudio.Services.TenantPolicy.PrivatePreview.RestrictPersonalAccessTokenLifespanPolicy";
          break;
        default:
          featureName = "VisualStudio.Services.TenantPolicy.EnablePrivatePreview";
          break;
      }
      return requestContext.IsFeatureEnabled(featureName);
    }

    private static string GetPolicyPreviewRegistryKey(string policyName)
    {
      switch (policyName)
      {
        case "TenantPolicy.OrganizationCreationRestriction":
          return "/Configuration/TenantPolicy/Rollout/PrivatePreviewTenants";
        case "TenantPolicy.RestrictFullScopePersonalAccessToken":
          return "/Configuration/TenantPolicy/Rollout/RestrictFullScopePatPrivatePreviewTenants";
        case "TenantPolicy.RestrictGlobalPersonalAccessToken":
          return "/Configuration/TenantPolicy/Rollout/RestrictGlobalPatPrivatePreviewTenants";
        case "TenantPolicy.RestrictPersonalAccessTokenLifespan":
          return "/Configuration/TenantPolicy/Rollout/RestrictPatLifespanPrivatePreviewTenants";
        default:
          return string.Empty;
      }
    }

    private static bool IsInPrivatePreviewTenantList(
      IVssRequestContext requestContext,
      Guid tenantId,
      string previewTenantRegistryKey)
    {
      if (string.IsNullOrWhiteSpace(previewTenantRegistryKey) || tenantId == Guid.Empty)
        return false;
      string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) previewTenantRegistryKey, (string) null);
      ISet<Guid> guidSet;
      return !str.IsNullOrEmpty<char>() && JsonUtilities.TryDeserialize<ISet<Guid>>(str, out guidSet) && guidSet.Contains(tenantId);
    }
  }
}
