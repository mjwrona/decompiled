// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TeamFoundationRequestContextExtensions
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal static class TeamFoundationRequestContextExtensions
  {
    private static string s_verifiedWorkspacesKey = "Microsoft.TeamFoundation.VersionControl.VerifiedWorkspaces";

    public static bool IsRequestingUser(this IVssRequestContext requestContext, string user)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      Guid result;
      return userIdentity != null && !string.IsNullOrEmpty(user) && (Guid.TryParse(user, out result) && object.Equals((object) userIdentity.Id, (object) result) || string.Equals(userIdentity.DisplayName, user) || string.Equals(requestContext.DomainUserName, user));
    }

    public static string GetRequestingUserDisplayName(this IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return userIdentity != null ? userIdentity.DisplayName : requestContext.DomainUserName;
    }

    public static string GetRequestingUserUniqueName(this IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return userIdentity != null ? userIdentity.Id.ToString() : requestContext.DomainUserName;
    }

    public static HashSet<WorkspaceInternal> GetVerifiedWorkspaces(
      this IVssRequestContext requestContext)
    {
      object verifiedWorkspaces;
      if (!requestContext.Items.TryGetValue(TeamFoundationRequestContextExtensions.s_verifiedWorkspacesKey, out verifiedWorkspaces))
      {
        verifiedWorkspaces = (object) new HashSet<WorkspaceInternal>();
        requestContext.Items[TeamFoundationRequestContextExtensions.s_verifiedWorkspacesKey] = verifiedWorkspaces;
      }
      return (HashSet<WorkspaceInternal>) verifiedWorkspaces;
    }

    public static VersionControlRequestContext GetVersionControlRequestContext(
      this IVssRequestContext requestContext)
    {
      object controlRequestContext;
      if (!requestContext.Items.TryGetValue("VersionControlRequestContext", out controlRequestContext))
      {
        controlRequestContext = (object) new VersionControlRequestContext(requestContext, requestContext.GetService<TeamFoundationVersionControlService>());
        requestContext.Items["VersionControlRequestContext"] = controlRequestContext;
      }
      return (VersionControlRequestContext) controlRequestContext;
    }
  }
}
