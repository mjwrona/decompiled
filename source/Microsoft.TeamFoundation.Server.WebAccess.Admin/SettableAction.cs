// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.SettableAction
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SettableAction : IComparable
  {
    internal SettableAction(
      Guid namespaceId,
      ActionDefinition action,
      string token,
      IAccessControlEntry accessControlEntry,
      bool canEdit,
      bool allowedBySystem = false,
      bool deniedBySystem = false)
    {
      this.ActionDefinition = action;
      this.AccessControlEntry = accessControlEntry;
      this.Token = token;
      this.CanEdit = canEdit;
      this.NamespaceId = namespaceId;
      this.AllowedBySystem = allowedBySystem;
      this.DeniedBySystem = deniedBySystem;
    }

    internal IAccessControlEntry AccessControlEntry { get; private set; }

    internal ActionDefinition ActionDefinition { get; set; }

    internal bool ExplicitAllow => (this.AccessControlEntry.Allow & this.ActionDefinition.Bit) == this.ActionDefinition.Bit;

    internal bool CanEdit { get; set; }

    internal bool ExplicitDeny => (this.AccessControlEntry.Deny & this.ActionDefinition.Bit) == this.ActionDefinition.Bit;

    internal bool InheritedAllow => this.AccessControlEntry.IncludesExtendedInfo && !this.IsExplicitPermissionSet() && (this.AccessControlEntry.EffectiveAllow & this.ActionDefinition.Bit) == this.ActionDefinition.Bit && (this.AccessControlEntry.Allow & this.ActionDefinition.Bit) != this.ActionDefinition.Bit;

    internal bool InheritedDeny => this.AccessControlEntry.IncludesExtendedInfo && (this.AccessControlEntry.EffectiveDeny & this.ActionDefinition.Bit) == this.ActionDefinition.Bit && (this.AccessControlEntry.Deny & this.ActionDefinition.Bit) != this.ActionDefinition.Bit;

    internal bool AllowedBySystem { get; private set; }

    internal bool DeniedBySystem { get; private set; }

    internal Guid NamespaceId { get; private set; }

    internal string Token { get; private set; }

    public int CompareTo(object obj) => obj is SettableAction ? TFStringComparer.AclPermissionEntry.Compare(this.ActionDefinition.DisplayName, ((SettableAction) obj).ActionDefinition.DisplayName) : -1;

    private bool IsExplicitPermissionSet() => (this.AccessControlEntry.Deny & this.ActionDefinition.Bit) == this.ActionDefinition.Bit || (this.AccessControlEntry.Allow & this.ActionDefinition.Bit) == this.ActionDefinition.Bit;
  }
}
