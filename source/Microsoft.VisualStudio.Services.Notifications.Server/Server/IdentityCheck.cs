// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IdentityCheck
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class IdentityCheck
  {
    private const string c_layer = "IdentityCheck";
    private const string c_notifTenantIdKey = "__notifTenantIdKey";

    public static bool IsValidVsIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity) => identity != null;

    public static bool IsProjectCollectionValidUser(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      return IdentityHelper.IsWellKnownGroup(identity.Descriptor, GroupWellKnownIdentityDescriptors.EveryoneGroup) && identity.GetProperty<Guid>("ScopeId", Guid.Empty) == requestContext.ServiceHost.InstanceId;
    }

    public static bool IsProjectCollectionValidUserMember(
      IVssRequestContext requestContext,
      IdentityService identityService,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      return identityService.IsMember(requestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, identity.Descriptor) || IdentityCheck.IsProjectCollectionValidUser(requestContext, identity);
    }

    public static bool IsValidOrgUser(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      bool flag = requestContext.ExecutionEnvironment.IsHostedDeployment;
      if (flag)
      {
        Guid orgAadTenantId = IdentityCheck.GetOrgAadTenantId(requestContext);
        flag = !orgAadTenantId.Equals(Guid.Empty);
        if (flag)
          flag = AadIdentityHelper.GetIdentityTenantId(identity.Descriptor).Equals(orgAadTenantId);
      }
      return flag;
    }

    public static bool IsValidCollectionOrOrgUser(
      IVssRequestContext requestContext,
      IdentityService identityService,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      bool flag = IdentityCheck.IsValidVsIdentity(identity);
      if (flag)
      {
        flag = IdentityCheck.IsValidOrgUser(requestContext, identity);
        if (!flag)
          flag = IdentityCheck.IsProjectCollectionValidUserMember(requestContext, identityService, identity);
      }
      return flag;
    }

    public static Guid GetOrgAadTenantId(IVssRequestContext requestContext)
    {
      Guid orgAadTenantId;
      if (!requestContext.Items.TryGetValue<Guid>("__notifTenantIdKey", out orgAadTenantId))
      {
        try
        {
          orgAadTenantId = requestContext.GetOrganizationAadTenantId();
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1002407, "Notifications", nameof (IdentityCheck), ex);
          orgAadTenantId = Guid.Empty;
        }
        requestContext.Items["__notifTenantIdKey"] = (object) orgAadTenantId;
      }
      return orgAadTenantId;
    }

    public static bool IsBindPending(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity) => requestContext.ExecutionEnvironment.IsHostedDeployment && identity?.Descriptor != (IdentityDescriptor) null && string.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.BindPendingIdentity", StringComparison.OrdinalIgnoreCase);

    public static bool IsAadGroupQuickAndDirty(IdentityDescriptor descriptor) => (object) descriptor != null && descriptor.Identifier != null && descriptor.Identifier.StartsWith(SidIdentityHelper.AadSidPrefix, StringComparison.OrdinalIgnoreCase);

    public static bool IsAadGroupQuickAndDirty(Microsoft.VisualStudio.Services.Identity.Identity identity) => IdentityCheck.IsAadGroupQuickAndDirty(identity?.Descriptor);
  }
}
