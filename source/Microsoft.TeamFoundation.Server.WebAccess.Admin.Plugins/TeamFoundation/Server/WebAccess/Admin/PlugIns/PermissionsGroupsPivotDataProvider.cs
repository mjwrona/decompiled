// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.PermissionsGroupsPivotDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.AdminEngagement.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class PermissionsGroupsPivotDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.GroupsPivot";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      string subjectDescriptor = (string) null;
      TeamFoundationIdentity groupIdentity = (TeamFoundationIdentity) null;
      PermissionsGroupsPivotDataProvider.GetPermissionsGroupsPivotParams(providerContext, out subjectDescriptor);
      if (string.IsNullOrEmpty(subjectDescriptor))
      {
        requestContext.Trace(10050062, TraceLevel.Error, "OrgSettingsGroupsPivot", "DataProvider", "Descriptor is null.");
        return (object) null;
      }
      IdentityDescriptor identityDescriptor = SubjectDescriptor.FromString(subjectDescriptor).ToIdentityDescriptor(requestContext);
      if (identityDescriptor == (IdentityDescriptor) null)
      {
        requestContext.Trace(10050057, TraceLevel.Error, "OrgSettingsGroupsPivot", "DataProvider", "Unable to resolve identity descriptor for subject descriptor");
        return (object) null;
      }
      try
      {
        TeamFoundationIdentity[] source = requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, new IdentityDescriptor[1]
        {
          identityDescriptor
        });
        groupIdentity = ((IEnumerable<TeamFoundationIdentity>) source).Any<TeamFoundationIdentity>() ? source[0] : (TeamFoundationIdentity) null;
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050063, TraceLevel.Error, "OrgSettingsGroupsPivot", "DataProvider", ex.Message);
      }
      return (object) PermissionsGroupsPivotDataProvider.HasGroupActionPermissions(requestContext, identityDescriptor, groupIdentity);
    }

    private static GroupPermissions HasGroupActionPermissions(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      TeamFoundationIdentity groupIdentity,
      bool alwaysAllowAdministrators = true)
    {
      if (groupIdentity == null)
        return (GroupPermissions) null;
      try
      {
        SecurityHttpClient client = requestContext.GetClient<SecurityHttpClient>(ServiceInstanceTypes.TFS);
        string securityToken = IdentityUtil.CreateSecurityToken(groupIdentity);
        if (SidIdentityHelper.IsWellKnownSid(identityDescriptor.Identifier))
          return new GroupPermissions()
          {
            IsEditAllowed = false,
            IsDeleteAllowed = false,
            IsManageMembershipAllowed = client.HasPermissionAsync(FrameworkSecurity.IdentitiesNamespaceId, securityToken, 8, alwaysAllowAdministrators).Result
          };
        return new GroupPermissions()
        {
          IsEditAllowed = client.HasPermissionAsync(FrameworkSecurity.IdentitiesNamespaceId, securityToken, 2, alwaysAllowAdministrators).Result && client.HasPermissionAsync(FrameworkSecurity.IdentitiesNamespaceId, securityToken, 8, alwaysAllowAdministrators).Result,
          IsDeleteAllowed = client.HasPermissionAsync(FrameworkSecurity.IdentitiesNamespaceId, securityToken, 4, alwaysAllowAdministrators).Result,
          IsManageMembershipAllowed = client.HasPermissionAsync(FrameworkSecurity.IdentitiesNamespaceId, securityToken, 8, alwaysAllowAdministrators).Result
        };
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050064, TraceLevel.Error, "OrgSettingsGroupsPivot", "DataProvider", ex.Message);
        return (GroupPermissions) null;
      }
    }

    private static void GetPermissionsGroupsPivotParams(
      DataProviderContext providerContext,
      out string subjectDescriptor)
    {
      subjectDescriptor = (string) null;
      if (!providerContext.Properties.ContainsKey(nameof (subjectDescriptor)) || providerContext.Properties[nameof (subjectDescriptor)] == null)
        return;
      subjectDescriptor = providerContext.Properties[nameof (subjectDescriptor)].ToString();
    }
  }
}
