// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.IAuthorizationProvider
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.CssNodes
{
  [DefaultServiceImplementation("Microsoft.TeamFoundation.Integration.Server.AuthorizationProvider, Microsoft.TeamFoundation.Server.Core")]
  public interface IAuthorizationProvider : IVssFrameworkService
  {
    string GetObjectClass(IVssRequestContext requestContext, string objectId);

    string GetObjectId(IVssRequestContext requestContext, Guid namespaceId, string securityToken);

    IdentityDescriptor GetAdminGroupForObjectId(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string objectId);

    string GetSecurityToken(IVssRequestContext requestContext, Guid namespaceId, string objectId);

    string[] ListObjectClassActions(IVssRequestContext requestContext, string objectClassId);

    string[] ListLocalizedActionNames(string objectClassId, string[] actionId);

    void ReplaceAccessControlList(
      IVssRequestContext requestContext,
      string objectId,
      AccessControlEntry[] aces);

    void AddAccessControlEntry(
      IVssRequestContext requestContext,
      string objectId,
      AccessControlEntry ace);

    void AddAccessControlEntries(
      IVssRequestContext requestContext,
      string objectId,
      IEnumerable<AccessControlEntry> aces);

    void RemoveAccessControlEntry(
      IVssRequestContext requestContext,
      string objectId,
      AccessControlEntry ace);

    string GetChangedAccessControlEntries(IVssRequestContext requestContext, int startSequenceId);

    bool IsPermitted(
      IVssRequestContext requestContext,
      string objectId,
      string actionId,
      IdentityDescriptor descriptor);

    void EnsurePermitted(IVssRequestContext requestContext, string objectId, string actionId);

    AccessControlEntry[] ReadAccessControlList(IVssRequestContext requestContext, string objectId);

    string[] ListObjectClasses(IVssRequestContext requestContext);

    void RegisterObject(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      string objectId,
      string objectClassId,
      string projectUri,
      string parentObjectId);

    void UnregisterObject(IVssRequestContext requestContext, string objectId);

    void ResetInheritance(
      IVssRequestContext requestContext,
      string objectId,
      string parentObjectId);

    void SecurityObjectCreatedByParentId(
      IVssRequestContext requestContext,
      string parentId,
      string objectId,
      string securityToken);

    void SecurityObjectCreatedByClassId(
      IVssRequestContext requestContext,
      string classId,
      string objectId,
      string securityToken,
      Guid projectId);

    void SecurityObjectDeleted(IVssRequestContext requestContext, string objectId);

    void ClearMemoryCache(IVssRequestContext requestContext, string securityClass);
  }
}
