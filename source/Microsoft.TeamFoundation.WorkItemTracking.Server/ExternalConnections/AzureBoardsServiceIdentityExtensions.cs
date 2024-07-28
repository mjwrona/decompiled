// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.AzureBoardsServiceIdentityExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal static class AzureBoardsServiceIdentityExtensions
  {
    private const string FrameworkIdentityRole = "Boards";
    private const string FrameworkIdentityName = "Azure Boards";

    public static Microsoft.VisualStudio.Services.Identity.Identity EnsureServiceIdentity(
      this IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(918030, "ExternalEvents", nameof (AzureBoardsServiceIdentityExtensions), nameof (EnsureServiceIdentity));
      string identifier = requestContext.ServiceHost.InstanceId.ToString("D");
      IVssRequestContext vssRequestContext1 = requestContext.Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = IdentityHelper.GetFrameworkIdentity(vssRequestContext1, FrameworkIdentityType.ServiceIdentity, "Boards", identifier);
      if (frameworkIdentity == null)
      {
        requestContext.Trace(918032, TraceLevel.Info, "ExternalEvents", nameof (AzureBoardsServiceIdentityExtensions), "Creating service identity");
        IVssRequestContext vssRequestContext2 = IdentityHelper.GetRequestContextForFrameworkIdentity(requestContext).Elevate();
        frameworkIdentity = vssRequestContext2.GetService<IdentityService>().CreateFrameworkIdentity(vssRequestContext2, FrameworkIdentityType.ServiceIdentity, "Boards", identifier, "Azure Boards");
        AzureBoardsServiceIdentityExtensions.EnsureServiceIdentityPermissions(vssRequestContext1, (IVssIdentity) frameworkIdentity);
      }
      if (!frameworkIdentity.IsActive)
      {
        requestContext.Trace(918033, TraceLevel.Info, "ExternalEvents", nameof (AzureBoardsServiceIdentityExtensions), "Service identity was not active, adding to the security service group");
        IdentityService service = vssRequestContext1.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(vssRequestContext1, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          GroupWellKnownIdentityDescriptors.SecurityServiceGroup
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
          service.AddMemberToGroup(vssRequestContext1, identity.Descriptor, frameworkIdentity);
      }
      requestContext.TraceLeave(918031, "ExternalEvents", nameof (AzureBoardsServiceIdentityExtensions), nameof (EnsureServiceIdentity));
      return frameworkIdentity;
    }

    public static void EnsureServiceIdentityPermissions(
      IVssRequestContext requestContext,
      IVssIdentity serviceIdentity)
    {
      requestContext.TraceEnter(918040, "ExternalEvents", nameof (AzureBoardsServiceIdentityExtensions), nameof (EnsureServiceIdentityPermissions));
      AzureBoardsServiceIdentityExtensions.EnsurePermissions(requestContext, serviceIdentity.Descriptor, "$PROJECT:", FrameworkSecurity.TeamProjectNamespaceId, TeamProjectPermissions.GenericRead | TeamProjectPermissions.BypassRules);
      AzureBoardsServiceIdentityExtensions.EnsurePermissions(requestContext, serviceIdentity.Descriptor, "vstfs", AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid, 49);
      requestContext.TraceLeave(918041, "ExternalEvents", nameof (AzureBoardsServiceIdentityExtensions), nameof (EnsureServiceIdentityPermissions));
    }

    private static void EnsurePermissions(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      string token,
      Guid namespaceId,
      int allow)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId).EnsurePermissions(requestContext, token, identityDescriptor, allow, 0, false);
    }
  }
}
