// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ISecurityNamespaceBackingStore
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface ISecurityNamespaceBackingStore
  {
    Guid AclStoreId { get; }

    IQuerySecurityDataResult QuerySecurityData(
      IVssRequestContext requestContext,
      long oldSequenceId = -1);

    SecurityBackingStoreChangedEventHandler WeakSubscribeToPushInvalidations(
      IVssRequestContext requestContext,
      SecurityBackingStoreChangedEventHandler eventHandler);

    bool SupportsPollingInvalidation { get; }

    TokenStoreSequenceId PollForSequenceId(IVssRequestContext requestContext);

    TokenStoreSequenceId SetPermissions(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IAccessControlEntry> permissions,
      bool merge,
      bool throwOnInvalidIdentity);

    TokenStoreSequenceId SetAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<IAccessControlList> acls,
      bool throwOnInvalidIdentity);

    TokenStoreSequenceId SetInheritFlag(
      IVssRequestContext requestContext,
      string token,
      bool inheritFlag);

    TokenStoreSequenceId RemovePermissions(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<Guid> identityIds);

    TokenStoreSequenceId RemoveAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      bool recurse);

    TokenStoreSequenceId RenameTokens(
      IVssRequestContext requestContext,
      IEnumerable<TokenRename> renames);
  }
}
