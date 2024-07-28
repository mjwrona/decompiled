// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PermissionsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class PermissionsModel
  {
    private List<PermissionModel> m_permissions = new List<PermissionModel>();
    private List<string> m_licenseErrors = new List<string>();
    private List<string> m_aadErrors = new List<string>();

    internal PermissionsModel(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      SecurityNamespacePermissionsManager manager)
    {
      IList<SettableAction> permissions = manager.GetPermissions(requestContext, descriptor);
      bool adminPermissions = manager.CanEditAdminPermissions;
      TeamFoundationIdentity foundationIdentity = requestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(requestContext, descriptor, MembershipQuery.None, ReadIdentityOptions.None);
      bool flag1 = false;
      bool flag2 = false;
      if (foundationIdentity != null)
      {
        if (foundationIdentity.IsContainer)
        {
          flag1 = foundationIdentity.GetAttribute("SpecialType", string.Empty).Equals("AdministrativeApplicationGroup", StringComparison.OrdinalIgnoreCase);
          flag2 = requestContext.ExecutionEnvironment.IsHostedDeployment && AadIdentityHelper.IsAadGroup(foundationIdentity.Descriptor);
        }
        this.TeamFoundationId = foundationIdentity.TeamFoundationId;
      }
      this.IsAdminGroup = flag1;
      this.IsAadGroup = flag2;
      this.CanEditAdminPermissions = adminPermissions;
      bool autoGrantAdmins = flag1 && !adminPermissions;
      this.IsAbleToEditAtLeastOnePermission = false;
      bool flag3 = requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") || requestContext.IsFeatureEnabled("WorkItemTracking.Server.MoveWorkItems");
      foreach (SettableAction action in (IEnumerable<SettableAction>) permissions)
      {
        if (flag3 || !(action.NamespaceId == FrameworkSecurity.TeamProjectNamespaceId) || action.ActionDefinition.Bit != TeamProjectPermissions.WorkItemMove)
        {
          if (autoGrantAdmins)
            action.CanEdit = false;
          else if (!this.IsAbleToEditAtLeastOnePermission)
            this.IsAbleToEditAtLeastOnePermission = action.CanEdit;
          this.Permissions.Add(new PermissionModel(action, autoGrantAdmins));
        }
      }
      this.Descriptor = descriptor;
    }

    public List<PermissionModel> Permissions => this.m_permissions;

    public bool AreExplicitEditablePermissionsSet
    {
      get
      {
        foreach (PermissionModel permission in this.Permissions)
        {
          if (permission.CanEdit && (permission.Allow || permission.Deny))
            return true;
        }
        return false;
      }
    }

    public bool IsEligibleForRemove
    {
      get
      {
        bool flag = true;
        foreach (PermissionModel permission in this.Permissions)
        {
          if (permission.InheritedAllow || permission.InheritedDeny || permission.InheritDenyOverride)
            return false;
          flag = flag && permission.CanEdit;
        }
        return this.Permissions.Count > 0 & flag;
      }
    }

    public bool CanEditAtLeastOnePermission
    {
      get
      {
        foreach (PermissionModel permission in this.Permissions)
        {
          if (permission.CanEdit)
            return true;
        }
        return false;
      }
    }

    public bool IsAbleToEditAtLeastOnePermission { get; private set; }

    public bool IsAdminGroup { get; private set; }

    public bool CanEditAdminPermissions { get; private set; }

    public bool CanEditPermissions => !this.AutoGrantCurrentIdentity && this.CanEditAtLeastOnePermission;

    public Guid TeamFoundationId { get; private set; }

    public bool AutoGrantCurrentIdentity => this.IsAdminGroup && !this.CanEditAdminPermissions;

    public IdentityDescriptor Descriptor { get; private set; }

    public List<string> LicenseErrors => this.m_licenseErrors;

    public List<string> AADErrors => this.m_aadErrors;

    public bool IsAadGroup { get; private set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["currentTeamFoundationId"] = (object) this.TeamFoundationId;
      json["descriptorIdentityType"] = (object) this.Descriptor.IdentityType;
      json["descriptorIdentifier"] = (object) this.Descriptor.Identifier;
      json["isAbleToEditAtLeastOnePermission"] = (object) this.IsAbleToEditAtLeastOnePermission;
      json["canEditPermissions"] = (object) this.CanEditPermissions;
      json["areExplicitPermissionsSet"] = (object) this.AreExplicitEditablePermissionsSet;
      json["isEligibleForRemove"] = (object) this.IsEligibleForRemove;
      json["autoGrantCurrentIdentity"] = (object) this.AutoGrantCurrentIdentity;
      json["permissions"] = (object) this.Permissions.Select<PermissionModel, JsObject>((Func<PermissionModel, JsObject>) (p => p.ToJson()));
      json["licenseErrors"] = (object) this.LicenseErrors;
      json["aadErrors"] = (object) this.AADErrors;
      json["isAadGroup"] = (object) this.IsAadGroup;
      return json;
    }
  }
}
