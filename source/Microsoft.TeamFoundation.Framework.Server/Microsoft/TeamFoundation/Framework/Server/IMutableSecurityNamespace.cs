// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IMutableSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IMutableSecurityNamespace
  {
    IEnumerable<IAccessControlEntry> SetAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IAccessControlEntry> accessControlEntries,
      bool merge,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true);

    void SetAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<IAccessControlList> accessControlLists,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true);

    void SetInheritFlag(IVssRequestContext requestContext, string token, bool inherit);

    bool RemoveAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IdentityDescriptor> descriptors);

    bool RemoveAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      bool recurse);

    void RenameToken(
      IVssRequestContext requestContext,
      string existingToken,
      string newToken,
      bool copy);

    void RenameTokens(IVssRequestContext requestContext, IEnumerable<TokenRename> renameTokens);
  }
}
