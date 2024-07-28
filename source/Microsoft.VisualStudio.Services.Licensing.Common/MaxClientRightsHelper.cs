// Decompiled with JetBrains decompiler
// Type: MaxClientRightsHelper
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public static class MaxClientRightsHelper
{
  public const string TestManagerExtensionId = "ms.vss-testmanager-web";
  private static string c_area = "UserMapping";
  private static string c_layer = nameof (MaxClientRightsHelper);

  public static VisualStudioLevel ComputeMaxLevelFromVSTSLicense(
    IDictionary<Guid, AccountEntitlement> userToEntitlementMap,
    Guid userId)
  {
    AccountEntitlement entitlement;
    userToEntitlementMap.TryGetValue(userId, out entitlement);
    return MaxClientRightsHelper.ComputeMaxLevelFromVSTSLicense(entitlement);
  }

  public static VisualStudioLevel ComputeMaxLevelFromVSTSLicense(
    IVssRequestContext requestContext,
    Guid userId)
  {
    return MaxClientRightsHelper.ComputeMaxLevelFromVSTSLicense(requestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlements(requestContext, (IList<Guid>) new Guid[1]
    {
      userId
    }, true).FirstOrDefault<AccountEntitlement>());
  }

  public static VisualStudioLevel ComputeMaxLevelFromVSTSExtensions(
    IDictionary<Guid, IList<ExtensionSource>> userToExtensionsMap,
    Guid userId)
  {
    IList<ExtensionSource> extensions;
    userToExtensionsMap.TryGetValue(userId, out extensions);
    return MaxClientRightsHelper.ComputeMaxLevelFromVSTSExtensions(extensions);
  }

  public static VisualStudioLevel ComputeMaxLevelFromVSTSExtensions(
    IVssRequestContext requestContext,
    Guid userId)
  {
    IDictionary<Guid, IList<ExtensionSource>> extensionsAssignedToUsers = requestContext.GetService<IExtensionEntitlementService>().GetExtensionsAssignedToUsers(requestContext, (IList<Guid>) new Guid[1]
    {
      userId
    });
    IList<ExtensionSource> extensions;
    if (!extensionsAssignedToUsers.TryGetValue(userId, out extensions))
    {
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        userId
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
        requestContext.Trace(2154314, TraceLevel.Warning, MaxClientRightsHelper.c_area, MaxClientRightsHelper.c_layer, string.Format("Failed to resolve identity with id {0}", (object) userId));
      else
        extensionsAssignedToUsers.TryGetValue(readIdentity.Id, out extensions);
    }
    return MaxClientRightsHelper.ComputeMaxLevelFromVSTSExtensions(extensions);
  }

  private static VisualStudioLevel ComputeMaxLevelFromVSTSExtensions(
    IList<ExtensionSource> extensions)
  {
    return extensions != null && extensions.Any<ExtensionSource>((Func<ExtensionSource, bool>) (x => x != null && string.Equals(x.ExtensionGalleryId, "ms.vss-testmanager-web", StringComparison.InvariantCultureIgnoreCase))) ? VisualStudioLevel.TestManager : VisualStudioLevel.None;
  }

  private static VisualStudioLevel ComputeMaxLevelFromVSTSLicense(AccountEntitlement entitlement)
  {
    if (entitlement != (AccountEntitlement) null && entitlement.License != (License) null)
    {
      if (AccountLicense.Professional.Equals(entitlement.License))
        return VisualStudioLevel.Professional;
      if (AccountLicense.Advanced.Equals(entitlement.License))
        return VisualStudioLevel.TestManager;
    }
    return VisualStudioLevel.None;
  }
}
