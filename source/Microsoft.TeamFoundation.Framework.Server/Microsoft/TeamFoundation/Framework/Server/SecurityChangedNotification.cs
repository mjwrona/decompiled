// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityChangedNotification
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SecurityChangedNotification
  {
    public SecurityChangedNotification(
      Guid namespaceId,
      string token,
      IdentityDescriptor descriptor,
      int permissionsToRemove)
    {
      this.SecurityChangeType = SecurityChangeType.RemoveExplicitPermissions;
      this.NamespaceId = namespaceId;
      this.Token = token;
      this.Descriptor = descriptor;
      this.ExplicitPermissionsToRemove = permissionsToRemove;
    }

    public SecurityChangedNotification(Guid namespaceId, List<string> tokens, bool recurse)
    {
      this.SecurityChangeType = SecurityChangeType.RemoveAccessControlLists;
      this.NamespaceId = namespaceId;
      this.Tokens = tokens;
      this.Recurse = recurse;
    }

    public SecurityChangedNotification(
      Guid namespaceId,
      string token,
      List<IdentityDescriptor> identities)
    {
      this.SecurityChangeType = SecurityChangeType.RemoveAccessControlEntries;
      this.NamespaceId = namespaceId;
      this.Token = token;
      this.RemoveAceDescriptors = identities;
    }

    public SecurityChangedNotification(
      Guid namespaceId,
      string existingToken,
      string newToken,
      bool copy)
    {
      this.SecurityChangeType = SecurityChangeType.RenameToken;
      this.NamespaceId = namespaceId;
      this.RenameTokenSource = existingToken;
      this.RenameTokenDestination = newToken;
      this.RenameWillCopy = copy;
    }

    public SecurityChangedNotification(Guid namespaceId, IEnumerable<TokenRename> renameTokens)
    {
      this.SecurityChangeType = SecurityChangeType.RenameTokens;
      this.NamespaceId = namespaceId;
      this.RenameTokens = renameTokens;
    }

    public SecurityChangedNotification(Guid namespaceId, string token, bool inherit)
    {
      this.SecurityChangeType = SecurityChangeType.SetInheritFlag;
      this.NamespaceId = namespaceId;
      this.Token = token;
      this.InheritPermissions = inherit;
    }

    public SecurityChangedNotification(
      Guid namespaceId,
      List<IAccessControlList> accessControlLists,
      bool throwOnInvalidIdentity,
      bool rootNewIdentities)
    {
      this.SecurityChangeType = SecurityChangeType.SetAccessControlLists;
      this.NamespaceId = namespaceId;
      this.AccessControlLists = accessControlLists;
      this.ThrowOnInvalidIdentity = throwOnInvalidIdentity;
      this.RootNewIdentities = rootNewIdentities;
    }

    public SecurityChangedNotification(
      Guid namespaceId,
      string token,
      List<IAccessControlEntry> permissions,
      bool merge,
      bool throwOnInvalidIdentity,
      bool rootNewIdentities)
    {
      this.SecurityChangeType = SecurityChangeType.SetAccessControlEntries;
      this.NamespaceId = namespaceId;
      this.Token = token;
      this.Permissions = permissions;
      this.Merge = merge;
      this.ThrowOnInvalidIdentity = throwOnInvalidIdentity;
      this.RootNewIdentities = rootNewIdentities;
    }

    public SecurityChangeType SecurityChangeType { get; private set; }

    public Guid NamespaceId { get; private set; }

    public string Token { get; set; }

    public List<IAccessControlEntry> Permissions { get; set; }

    public bool Merge { get; set; }

    public List<IAccessControlList> AccessControlLists { get; set; }

    public bool InheritPermissions { get; set; }

    public IEnumerable<TokenRename> RenameTokens { get; set; }

    public string RenameTokenSource { get; set; }

    public string RenameTokenDestination { get; set; }

    public bool RenameWillCopy { get; set; }

    public List<IdentityDescriptor> RemoveAceDescriptors { get; set; }

    public List<string> Tokens { get; set; }

    public bool Recurse { get; set; }

    public IdentityDescriptor Descriptor { get; set; }

    public int ExplicitPermissionsToRemove { get; set; }

    public bool ThrowOnInvalidIdentity { get; set; }

    public bool RootNewIdentities { get; set; }
  }
}
