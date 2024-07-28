// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityMembershipHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class IdentityMembershipHelper
  {
    private const string c_returnInScopeMembershipsForOrgLevelRGMCalls = "VisualStudio.Services.Identity.RGM.ReturnInScopeMembershipsForOrgLevelCalls";
    private const string c_enableGetScopedGroupChanges = "VisualStudio.Services.Identity.EnableGetScopedGroupChanges";

    public static bool ShouldReturnInScopeMemberships(IVssRequestContext context) => context != null && !context.ExecutionEnvironment.IsOnPremisesDeployment && context.ServiceHost.IsOnly(TeamFoundationHostType.Application) && context.IsFeatureEnabled("VisualStudio.Services.Identity.RGM.ReturnInScopeMembershipsForOrgLevelCalls");

    public static bool ShouldGetScopedGroupChanges(IVssRequestContext context) => context != null && !context.ExecutionEnvironment.IsOnPremisesDeployment && context.IsFeatureEnabled("VisualStudio.Services.Identity.EnableGetScopedGroupChanges");
  }
}
