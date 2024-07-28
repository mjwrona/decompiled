// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityFeatureFlagConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class IdentityFeatureFlagConstants
  {
    public const string EnableIdentityNavigation = "VisualStudio.Services.Framework.EnableIdentityNavigation";
    public const string VirtualizedDeploymentGroupStore = "VisualStudio.Services.Identity.VirtualizedDeploymentGroupStore";
    public const string UseUserRequestContext = "AzureDevOps.Services.RequestContext.UseUserRequestContext";
    public const string DisableCommonTenantForLiveTenant = "VisualStudio.Services.Identity.DisableCommonTenantForLiveTenant";
    public const string EnableAfdIpValidation = "VisualStudio.Services.Identity.EnableAfdIpValidation";
    public const string AfdIpValidatorVerifyOnlyMode = "VisualStudio.Services.Identity.AfdIpValidationVerifyOnlyMode";
    public const string EnableArrForwardingIpValidation = "VisualStudio.Services.Identity.EnableArrForwardingIpValidation";
    public const string EnableIpSpoofingPrevention = "VisualStudio.Services.Identity.EnableIpSpoofingPrevention";
    public const string EnableAadScopes = "Identity.OAuth2.EnableAadScopesAndRoles.M214";
    public const string UseArrServiceForS2SToken = "VisualStudio.Services.Identity.UseArrServiceForS2SToken";
  }
}
