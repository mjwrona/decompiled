// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ISecurityChangedEventHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [InheritedExport]
  public interface ISecurityChangedEventHandler
  {
    bool ValidInHostContext(IVssRequestContext requestContext, TeamFoundationHostType hostType);

    Guid NamespaceId { get; }

    bool SetInheritFlag(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string token,
      bool inherit);

    bool RenameToken(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string existingToken,
      string newToken,
      bool copy);

    bool RemovePermissions(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string token,
      IEnumerable<IdentityDescriptor> identities);

    bool RemoveAccessControlLists(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      IEnumerable<string> securityTokens,
      bool recurse);

    bool RemovePermissions(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string token,
      IdentityDescriptor descriptor,
      int permissionsToRemove);

    bool SetPermissions(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      string token,
      IEnumerable<IAccessControlEntry> entries,
      bool merge,
      bool throwOnInvalidIdentity,
      bool rootNewIdentities);

    bool SetAccessControlLists(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      IEnumerable<IAccessControlList> lists,
      bool throwOnInvalidIdentity,
      bool rootNewIdentities);
  }
}
