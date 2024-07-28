// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ProcessPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class ProcessPermissionsManager : SecurityNamespacePermissionsManager
  {
    public ProcessPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string token)
      : base(requestContext, permissionsIdentifier, token)
    {
    }

    protected override bool CanUserManageIdentities(IVssRequestContext requestContext)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.ProcessNamespaceId);
      return securityNamespace != null && securityNamespace.HasPermission(requestContext, this.Token, 8);
    }

    protected override bool CanUserViewPermissions(IVssRequestContext requestContext)
    {
      SecurityNamespacePermissionSet namespacePermissionSet;
      return this.PermissionSets.TryGetValue(FrameworkSecurity.ProcessNamespaceId, out namespacePermissionSet) && namespacePermissionSet != null && namespacePermissionSet.HasReadPermission(requestContext, this.Token);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int permissionsToDisplay;
      if (this.IsSystemProcess(requestContext))
      {
        permissionsToDisplay = 4;
      }
      else
      {
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.ProcessNamespaceId);
        permissionsToDisplay = (securityNamespace == null || !securityNamespace.HasPermission(requestContext, this.Token, 8) ? 7 : 15) & -5;
      }
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, FrameworkSecurity.ProcessNamespaceId, this.Token, permissionsToDisplay);
      permissionSets.Add(FrameworkSecurity.ProcessNamespaceId, namespacePermissionSet);
      return permissionSets;
    }

    protected override IAccessControlList GetParentAccessControlList(
      IVssRequestContext requestContext,
      string token,
      List<IdentityDescriptor> descriptors)
    {
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, FrameworkSecurity.ProcessNamespaceId, this.Token, SecurityNamespacePermissionsManager.AllPermissions);
      string parentToken = this.GetParentToken(requestContext, token);
      IVssRequestContext requestContext1 = requestContext;
      string token1 = parentToken;
      return namespacePermissionSet.GetAccessControlList(requestContext1, token1);
    }

    private string GetParentToken(IVssRequestContext requestContext, string childToken)
    {
      if (!childToken.StartsWith(PermissionNamespaces.Process))
        throw new ArgumentException(Microsoft.TeamFoundation.Server.Core.Resources.InvalidToken((object) childToken));
      if (string.Equals(childToken, PermissionNamespaces.Process, StringComparison.OrdinalIgnoreCase))
        return PermissionNamespaces.Process;
      string str = childToken;
      if (str.EndsWith(ProcessConstants.ProcessSecurityTokenSeparator))
        str = str.Remove(str.LastIndexOf(ProcessConstants.ProcessSecurityTokenSeparator));
      return str.Remove(str.LastIndexOf(ProcessConstants.ProcessSecurityTokenSeparator) + 1);
    }

    private bool IsSystemProcess(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, this.ParseProcessTemplateTypeId(this.Token)).Scope == ProcessScope.Deployment;

    private Guid ParseProcessTemplateTypeId(string token)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(token, nameof (token));
      if (!token.StartsWith(PermissionNamespaces.Process) || !token.EndsWith(ProcessConstants.ProcessSecurityTokenSeparator) || string.Equals(token, PermissionNamespaces.Process, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(Microsoft.TeamFoundation.Server.Core.Resources.InvalidToken((object) token));
      string str = token.Remove(token.LastIndexOf(ProcessConstants.ProcessSecurityTokenSeparator));
      return Guid.Parse(str.Substring(str.LastIndexOf(ProcessConstants.ProcessSecurityTokenSeparator) + 1));
    }
  }
}
