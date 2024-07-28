// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationRolloutHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class OrganizationRolloutHelper
  {
    private const string c_privatePreviewTenants = "/Configuration/Organization/Rollout/PrivatePreviewTenants";
    private const string c_orgSearchPrivatePreviewTenants = "/Configuration/OrganizationSearch/Rollout/PrivatePreviewTenants";

    public static bool IsInPreview(IVssRequestContext requestContext) => OrganizationRolloutHelper.IsPublicPreviewEnabled(requestContext) || OrganizationRolloutHelper.IsInPrivatePreview(requestContext);

    public static bool IsPublicPreviewEnabled(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.Services.Organization.PublicPreview");

    public static bool IsInPrivatePreview(IVssRequestContext requestContext) => OrganizationRolloutHelper.IsPrivatePreviewEnabled(requestContext) && OrganizationRolloutHelper.IsPrivatePreviewTenant(requestContext);

    public static bool IsPrivatePreviewTenant(IVssRequestContext requestContext) => OrganizationRolloutHelper.IsPreviewTenant(requestContext, "/Configuration/Organization/Rollout/PrivatePreviewTenants");

    public static bool IsPrivatePreviewEnabled(IVssRequestContext requestContext) => requestContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.Services.Organization.PrivatePreview");

    private static bool IsPreviewTenant(
      IVssRequestContext requestContext,
      string previewTenantRegistryKey)
    {
      Guid guid = requestContext.GetOrganizationAadTenantId();
      if (guid == Guid.Empty)
        guid = AadIdentityHelper.ExtractTenantId((IReadOnlyVssIdentity) requestContext.GetUserIdentity());
      if (guid == Guid.Empty)
        return false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string str = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) previewTenantRegistryKey, (string) null);
      Guid[] source;
      return !str.IsNullOrEmpty<char>() && JsonUtilities.TryDeserialize<Guid[]>(str, out source) && ((IEnumerable<Guid>) source).Contains<Guid>(guid);
    }
  }
}
