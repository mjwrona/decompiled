// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TaggingSecurityHelper
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TaggingSecurityHelper
  {
    public static void SetTeamProjectTaggingPermissions(
      IVssRequestContext requestContext,
      string projectUri,
      bool grantCreateToEveryone = false)
    {
      Guid projectId = ProjectInfo.GetProjectId(projectUri);
      IdentityScope scope = requestContext.GetService<IdentityService>().GetScope(requestContext, projectId);
      TaggingSecurityHelper.SetTeamProjectTaggingPermissions(requestContext, projectId, scope.Administrators, scope.Id, grantCreateToEveryone);
    }

    private static void SetTeamProjectTaggingPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IdentityDescriptor administrators,
      Guid projectScopeId,
      bool grantCreateToEveryone = false)
    {
      string securityToken = TaggingService.GetSecurityToken(new Guid?(projectId));
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TaggingNamespaceId);
      IdentityDescriptor descriptor = IdentityDomain.MapFromWellKnownIdentifier(projectScopeId, GroupWellKnownIdentityDescriptors.EveryoneGroup);
      IVssRequestContext requestContext1 = requestContext;
      string token = securityToken;
      AccessControlEntry[] accessControlEntryArray = new AccessControlEntry[2]
      {
        new AccessControlEntry(administrators, TaggingPermissions.AllPermissions, 0),
        new AccessControlEntry(descriptor, TaggingPermissions.Enumerate | (grantCreateToEveryone ? TaggingPermissions.Create : 0), 0)
      };
      securityNamespace.SetAccessControlEntries(requestContext1, token, (IEnumerable<IAccessControlEntry>) accessControlEntryArray, false);
    }
  }
}
