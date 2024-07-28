// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AadGuestUserAccessHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Organization;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public static class AadGuestUserAccessHelper
  {
    internal static Policy<bool> GetPolicy(IVssRequestContext requestContext) => requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext.Elevate(), "Policy.DisallowAadGuestUserAccess", AadGuestUserAccessHelper.DisallowAadGuestUserAccessPolicyDefaultValue);

    public static bool IsAccessEnabled(IVssRequestContext requestContext) => !AadGuestUserAccessHelper.GetPolicy(requestContext).EffectiveValue;

    public static bool DisallowAadGuestUserAccessPolicyDefaultValue => false;

    public static void DisableAccess(IVssRequestContext requestContext) => requestContext.GetService<IOrganizationPolicyService>().SetPolicyValue<bool>(requestContext, "Policy.DisallowAadGuestUserAccess", true);

    public static void EnableAccess(IVssRequestContext requestContext) => requestContext.GetService<IOrganizationPolicyService>().SetPolicyValue<bool>(requestContext, "Policy.DisallowAadGuestUserAccess", false);
  }
}
