// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.FeatureFlags
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  public static class FeatureFlags
  {
    public const string EnableTenantPolicyService = "VisualStudio.Services.TenantPolicy.EnableTenantPolicyService";
    public const string PrivatePreview = "VisualStudio.Services.TenantPolicy.EnablePrivatePreview";
    public const string PublicPreview = "VisualStudio.Services.TenantPolicy.EnablePublicPreview";
    public const string EnableOrganizationCreationRestrictionPolicy = "VisualStudio.Services.TenantPolicy.EnableOrganizationCreationRestrictionPolicy";
    public const string RestrictTenantPolicyAPIToTenantMembers = "VisualStudio.Services.TenantPolicy.RestrictTenantPolicyAPIToTenantMembers";
    public const string RestrictGlobalPersonalAccessTokenPolicy = "VisualStudio.Services.TenantPolicy.RestrictGlobalPersonalAccessTokenPolicy";
    public const string RestrictFullScopePersonalAccessTokenPolicy = "VisualStudio.Services.TenantPolicy.RestrictFullScopePersonalAccessTokenPolicy";
    public const string RestrictPersonalAccessTokenLifespanPolicy = "VisualStudio.Services.TenantPolicy.RestrictPersonalAccessTokenLifespanPolicy";
    public const string EnableLeakedPersonalAccessTokenAutoRevocationPolicy = "VisualStudio.Services.TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocationPolicy";
    public const string RestrictGlobalPersonalAccessTokenPolicyPrivatePreview = "VisualStudio.Services.TenantPolicy.PrivatePreview.RestrictGlobalPersonalAccessTokenPolicy";
    public const string RestrictFullScopePersonalAccessTokenPolicyPrivatePreview = "VisualStudio.Services.TenantPolicy.PrivatePreview.RestrictFullScopePersonalAccessTokenPolicy";
    public const string RestrictPersonalAccessTokenLifespanPolicyPrivatePreview = "VisualStudio.Services.TenantPolicy.PrivatePreview.RestrictPersonalAccessTokenLifespanPolicy";
  }
}
