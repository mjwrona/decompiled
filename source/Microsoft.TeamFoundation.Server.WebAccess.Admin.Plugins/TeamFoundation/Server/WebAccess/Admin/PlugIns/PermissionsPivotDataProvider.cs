// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.PermissionsPivotDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.AdminEngagement.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class PermissionsPivotDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.PermissionsPivot";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      try
      {
        string subjectDescriptor = (string) null;
        Guid permissionSetId = new Guid();
        string permissionSetToken = (string) null;
        string projectName;
        PermissionsHelper.GetSecurityPermissionsParams(requestContext, providerContext, out subjectDescriptor, out permissionSetId, out permissionSetToken, out projectName);
        string empty = string.Empty;
        if (providerContext.Properties.ContainsKey("accountName") && providerContext.Properties["accountName"] != null)
          empty = providerContext.Properties["accountName"].ToString();
        ArgumentUtility.CheckStringForNullOrEmpty(subjectDescriptor, "descriptor");
        ArgumentUtility.CheckForEmptyGuid(permissionSetId, "permissionSetId");
        IdentityDescriptor identityDescriptor = SubjectDescriptor.FromString(subjectDescriptor).ToIdentityDescriptor(requestContext);
        if (identityDescriptor == (IdentityDescriptor) null && !string.IsNullOrWhiteSpace(empty))
          identityDescriptor = IdentityHelper.GetOrCreateBindPendingIdentity(requestContext, PermissionsPivotDataProvider.GetUserDomain(requestContext), empty, callerName: nameof (GetData)).Descriptor;
        ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, "identityDescriptor");
        requestContext.Items["RequestContextItemKeyForStakeholderLicenseCheckBypass"] = (object) true;
        SecurityNamespacePermissionsManager manager = SecurityNamespacePermissionsManagerFactory.CreateManager(requestContext, permissionSetId, permissionSetToken, projectName);
        PermissionsModel permissionsModel = new PermissionsModel(requestContext, identityDescriptor, manager);
        List<SubjectPermission> permissions = new List<SubjectPermission>();
        permissionsModel.Permissions.ForEach((Action<PermissionModel>) (o => permissions.Add(o.ToClientPermissionModel())));
        return (object) new SubjectPermissions()
        {
          subjectPermissions = permissions,
          identityDescriptor = identityDescriptor,
          CanEditPermissions = permissionsModel.CanEditPermissions,
          UserHasReadAccess = manager.UserHasReadAccess,
          IsProjectScope = object.Equals((object) permissionSetId, (object) NamespacePermissionSetConstants.ProjectLevel)
        };
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050065, TraceLevel.Error, "OrgSettingsPermissionsPivot", "DataProvider", ex.Message);
        return (object) null;
      }
      finally
      {
        requestContext.Items.Remove("RequestContextItemKeyForStakeholderLicenseCheckBypass");
      }
    }

    private static string GetUserDomain(IVssRequestContext tfsRequestContext)
    {
      Guid organizationAadTenantId = tfsRequestContext.GetOrganizationAadTenantId();
      return !organizationAadTenantId.Equals(Guid.Empty) ? organizationAadTenantId.ToString() : "Windows Live ID";
    }
  }
}
