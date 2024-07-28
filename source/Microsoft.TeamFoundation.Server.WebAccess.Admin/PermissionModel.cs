// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PermissionModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.AdminEngagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class PermissionModel
  {
    internal PermissionModel(SettableAction action, bool autoGrantAdmins)
    {
      this.DisplayName = action.ActionDefinition.DisplayName;
      if (autoGrantAdmins && (action.NamespaceId == AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid || action.NamespaceId == AuthorizationSecurityConstants.IterationNodeSecurityGuid || action.NamespaceId == AuthorizationSecurityConstants.NamespaceSecurityGuid || action.NamespaceId == AuthorizationSecurityConstants.ProjectSecurityGuid))
      {
        this.Allow = true;
        this.Deny = false;
        this.InheritedAllow = false;
        this.InheritedDeny = false;
      }
      else
      {
        this.Allow = action.ExplicitAllow;
        this.Deny = action.ExplicitDeny;
        this.InheritedAllow = action.InheritedAllow;
        this.InheritedDeny = action.InheritedDeny;
      }
      this.Bit = action.ActionDefinition.Bit;
      this.NamespaceId = action.NamespaceId;
      this.Token = action.Token;
      this.CanEdit = action.CanEdit;
      this.AllowedBySystem = action.AllowedBySystem;
      this.DeniedBySystem = action.DeniedBySystem;
      this.SetExplicitPermissionValue();
      this.SetEffectivePermissionValue();
    }

    public bool Allow { get; internal set; }

    public int Bit { get; internal set; }

    public bool CanEdit { get; internal set; }

    public bool Deny { get; internal set; }

    public string DisplayName { get; internal set; }

    public bool InheritedAllow { get; internal set; }

    public bool InheritedDeny { get; internal set; }

    public bool AllowedBySystem { get; internal set; }

    public bool DeniedBySystem { get; internal set; }

    public bool InheritDenyOverride
    {
      get
      {
        if (this.EffectivePermissionValue != PermissionValue.InheritedDeny)
          return false;
        return this.ExplicitPermissionValue == PermissionValue.Allow || this.ExplicitPermissionValue == PermissionValue.NotSet;
      }
    }

    public Guid NamespaceId { get; internal set; }

    public PermissionValue ExplicitPermissionValue { get; internal set; }

    public PermissionValue EffectivePermissionValue { get; internal set; }

    public string Token { get; internal set; }

    public string GetPermissionDisplayString() => PermissionModel.GetPermissionDisplayString(this.EffectivePermissionValue, this.ExplicitPermissionValue);

    public bool DisplayTrace() => this.EffectivePermissionValue == PermissionValue.InheritedDeny || this.EffectivePermissionValue == PermissionValue.InheritedAllow;

    public static string GetPermissionDisplayString(PermissionValue value)
    {
      switch (value)
      {
        case PermissionValue.NotSet:
          return AdminResources.NotSet;
        case PermissionValue.Allow:
          return AdminResources.Allow;
        case PermissionValue.Deny:
          return AdminResources.Deny;
        case PermissionValue.InheritedAllow:
          return AdminResources.InheritedAllow;
        case PermissionValue.InheritedDeny:
          return AdminResources.InheritedDeny;
        case PermissionValue.AllowedBySystem:
          return AdminResources.AllowedBySystem;
        case PermissionValue.DeniedBySystem:
          return AdminResources.DeniedBySystem;
        default:
          return AdminResources.NotSet;
      }
    }

    public static string GetPermissionDisplayString(
      PermissionValue effectiveValue,
      PermissionValue explicitValue)
    {
      return effectiveValue == PermissionValue.InheritedDeny && explicitValue == PermissionValue.Allow ? AdminServerResources.InheritedDenyOfExplicitAllow : PermissionModel.GetPermissionDisplayString(effectiveValue);
    }

    private void SetExplicitPermissionValue()
    {
      if (this.Deny)
        this.ExplicitPermissionValue = PermissionValue.Deny;
      else if (this.Allow)
        this.ExplicitPermissionValue = PermissionValue.Allow;
      else
        this.ExplicitPermissionValue = PermissionValue.NotSet;
    }

    private void SetEffectivePermissionValue()
    {
      if (this.AllowedBySystem)
        this.EffectivePermissionValue = PermissionValue.AllowedBySystem;
      else if (this.DeniedBySystem)
        this.EffectivePermissionValue = PermissionValue.DeniedBySystem;
      else if (this.InheritedAllow)
        this.EffectivePermissionValue = PermissionValue.InheritedAllow;
      else if (this.InheritedDeny)
        this.EffectivePermissionValue = PermissionValue.InheritedDeny;
      else if (this.Deny)
        this.EffectivePermissionValue = PermissionValue.Deny;
      else if (this.Allow)
        this.EffectivePermissionValue = PermissionValue.Allow;
      else
        this.EffectivePermissionValue = PermissionValue.NotSet;
    }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("permissionId", (object) this.EffectivePermissionValue);
      json.Add("explicitPermissionId", (object) this.ExplicitPermissionValue);
      json.Add("originalPermissionId", (object) this.EffectivePermissionValue);
      json.Add("permissionBit", (object) this.Bit);
      json.Add("namespaceId", (object) this.NamespaceId);
      json.Add("permissionToken", (object) this.Token);
      json.Add("canEdit", (object) this.CanEdit);
      json.Add("displayName", (object) this.DisplayName);
      json.Add("inheritDenyOverride", (object) this.InheritDenyOverride);
      json.Add("permissionDisplayString", (object) this.GetPermissionDisplayString());
      json.Add("displayTrace", (object) this.DisplayTrace());
      return json;
    }
  }
}
