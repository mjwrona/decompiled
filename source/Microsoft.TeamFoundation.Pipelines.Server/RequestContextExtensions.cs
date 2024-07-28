// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.RequestContextExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class RequestContextExtensions
  {
    private const string c_layer = "RequestContextExtensions";

    public static void RunAsUser(
      this IVssRequestContext requestContext,
      Guid hostId,
      Guid teamProjectId,
      string role,
      Action<IVssRequestContext> action)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ArgumentUtility.CheckForEmptyGuid(teamProjectId, nameof (teamProjectId));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        requestContext.TraceError(0, nameof (RequestContextExtensions), string.Format("Expected a collection-level request context but got {0} instead.", (object) requestContext.ServiceHost.HostType));
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      }
      requestContext.TraceInfo(0, nameof (RequestContextExtensions), string.Format("RunAsUser called with hostId: {0}, teamProjectId: {1}, role: {2}", (object) hostId, (object) teamProjectId, (object) role));
      Microsoft.VisualStudio.Services.Identity.Identity pipelinesServiceIdentity = requestContext.FindPipelinesServiceIdentity(teamProjectId, role);
      using (IVssRequestContext vssRequestContext = requestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(requestContext, hostId, pipelinesServiceIdentity))
        action(vssRequestContext);
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      this IVssRequestContext requestContext,
      string userId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(userId, nameof (userId));
      Guid identifier = new Guid(userId);
      return requestContext.GetService<IdentityService>().GetIdentity(requestContext, identifier);
    }

    public static string GetSecretCredentialName(
      this IVssRequestContext requestContext,
      string drawerName,
      string secretKey)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(drawerName, nameof (drawerName));
      ArgumentUtility.CheckStringForNullOrEmpty(secretKey, nameof (secretKey));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      return vssRequestContext.GetService<ITeamFoundationStrongBoxService>().GetItemInfo(vssRequestContext, drawerName, secretKey, true).CredentialName;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity FindPipelinesServiceIdentity(
      this IVssRequestContext requestContext,
      Guid projectId,
      string role)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(role, nameof (role));
      Guid guid = projectId;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = IdentityHelper.GetFrameworkIdentity(vssRequestContext, FrameworkIdentityType.ServiceIdentity, role, guid.ToString("D"));
      if (frameworkIdentity == null || !frameworkIdentity.IsActive)
        throw new IdentityNotFoundException(PipelinesResources.ExceptionIdentityNotFound((object) guid, (object) role));
      vssRequestContext.TraceInfo(0, nameof (RequestContextExtensions), string.Format("Found service identity with ID {0}  and additional identity info: {1}", (object) frameworkIdentity.Id, (object) frameworkIdentity.ToString()));
      return frameworkIdentity;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity ProvisionPipelinesServiceIdentity(
      this IVssRequestContext requestContext,
      Guid identifier,
      string role,
      string identityName,
      bool grantProjectPermissions = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
      IVssRequestContext vssRequestContext1 = requestContext.Elevate();
      IVssRequestContext vssRequestContext2 = IdentityHelper.GetRequestContextForFrameworkIdentity(requestContext).Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = IdentityHelper.GetFrameworkIdentity(vssRequestContext1, FrameworkIdentityType.ServiceIdentity, role, identifier.ToString("D"));
      vssRequestContext1.TraceInfo(0, nameof (RequestContextExtensions), string.Format("ProvisionPipelinesServiceIdentity called with identifier: {0}, role: {1}, identityName: {2}", (object) identifier, (object) role, (object) identityName));
      if (frameworkIdentity != null)
      {
        vssRequestContext1.TraceInfo(0, nameof (RequestContextExtensions), string.Format("Found service identity with ID {0} and additional identity info: {1}", (object) frameworkIdentity.Id, (object) frameworkIdentity.ToString()));
        RequestContextExtensions.ActivateIdentity(vssRequestContext1, frameworkIdentity);
        if (!frameworkIdentity.DisplayName.Equals(identityName, StringComparison.Ordinal))
        {
          string customDisplayName = frameworkIdentity.CustomDisplayName;
          frameworkIdentity.CustomDisplayName = identityName;
          try
          {
            if (vssRequestContext2.GetService<IdentityService>().UpdateIdentities(vssRequestContext2, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
            {
              frameworkIdentity
            }))
              vssRequestContext1.TraceInfo(0, nameof (RequestContextExtensions), "Successfully renamed project service identity from '" + customDisplayName + "' to '" + identityName + "'");
            else
              vssRequestContext1.TraceInfo(0, nameof (RequestContextExtensions), "Received a non-successful response while renaming project service identtiy from '" + customDisplayName + "' to '" + identityName + "'");
          }
          catch (Exception ex)
          {
            vssRequestContext1.TraceError(0, nameof (RequestContextExtensions), "Failed to update project service identity from '" + customDisplayName + "' to '" + identityName + "':\n" + ex.ToString());
          }
        }
      }
      else
      {
        vssRequestContext1.TraceInfo(0, nameof (RequestContextExtensions), string.Format("Service identity with ID {0} was not found. Provisioning a new service identity.", (object) identifier));
        frameworkIdentity = vssRequestContext2.GetService<IdentityService>().CreateFrameworkIdentity(vssRequestContext2, FrameworkIdentityType.ServiceIdentity, role, identifier.ToString("D"), identityName);
        if (frameworkIdentity != null)
        {
          vssRequestContext1.TraceInfo(0, nameof (RequestContextExtensions), string.Format("Successfully provisioned service identity {0} with VSID {1}", (object) frameworkIdentity.DisplayName, (object) frameworkIdentity.Id));
          RequestContextExtensions.ActivateIdentity(vssRequestContext1, frameworkIdentity);
        }
      }
      if (frameworkIdentity != null & grantProjectPermissions)
        vssRequestContext1.SetServiceIdentityPermissions(identifier, frameworkIdentity);
      return frameworkIdentity;
    }

    public static void SetProjectCreationPermission(
      this IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity)
    {
      RequestContextExtensions.SetAceIfNeeded(requestContext, serviceIdentity.Descriptor, FrameworkSecurity.TeamProjectCollectionNamespaceToken, FrameworkSecurity.TeamProjectCollectionNamespaceId, TeamProjectCollectionPermissions.CreateProjects);
    }

    private static void ActivateIdentity(
      IVssRequestContext elevatedSystemContext,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity)
    {
      IdentityService service = elevatedSystemContext.GetService<IdentityService>();
      if (serviceIdentity.IsActive)
        return;
      elevatedSystemContext.TraceAlways(0, TraceLevel.Info, TracePoints.Area, nameof (RequestContextExtensions), string.Format("Service identity with ID {0} is not active in scope {1}", (object) serviceIdentity.Id, (object) elevatedSystemContext.ServiceHost.InstanceId));
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.GetIdentity(elevatedSystemContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup);
      if (identity == null)
        return;
      if (service.AddMemberToGroup(elevatedSystemContext, identity.Descriptor, serviceIdentity))
        elevatedSystemContext.TraceAlways(0, TraceLevel.Info, TracePoints.Area, nameof (RequestContextExtensions), string.Format("Service identity with ID {0} has been successfully added to security service group {1}", (object) serviceIdentity.Id, (object) identity.Id));
      else
        elevatedSystemContext.TraceAlways(0, TraceLevel.Info, TracePoints.Area, nameof (RequestContextExtensions), string.Format("Service identity with ID {0} is marked inactive but was not added to security service group {1}", (object) serviceIdentity.Id, (object) identity.Id));
    }

    internal static void SetServiceIdentityPermissions(
      this IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      ProjectInfo project = service.GetProject(requestContext, projectId);
      RequestContextExtensions.SetAceIfNeeded(requestContext, serviceIdentity.Descriptor, TaggingService.GetSecurityToken(new Guid?(projectId)), FrameworkSecurity.TaggingNamespaceId, TaggingPermissions.AllPermissions);
      RequestContextExtensions.SetAceIfNeeded(requestContext, serviceIdentity.Descriptor, service.GetSecurityToken(requestContext, project.Uri), FrameworkSecurity.TeamProjectNamespaceId, TeamProjectPermissions.GenericRead | TeamProjectPermissions.ViewTestResults);
      RequestContextExtensions.SetAceIfNeeded(requestContext, serviceIdentity.Descriptor, ServiceEndpointSecurity.GetServiceEndPointToken(projectId.ToString("D"), (string) null), ServiceEndpointSecurity.NamespaceId, 1);
      RequestContextExtensions.SetAceIfNeeded(requestContext, serviceIdentity.Descriptor, projectId.ToString("D"), BuildSecurity.BuildNamespaceId, BuildPermissions.QueueBuilds | BuildPermissions.ViewBuildDefinition | BuildPermissions.ViewBuilds | BuildPermissions.UpdateBuildInformation | BuildPermissions.DeleteBuilds);
    }

    private static void SetAceIfNeeded(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      string token,
      Guid namespaceId,
      int allow)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceId);
      if (securityNamespace == null)
        return;
      AccessControlEntry accessControlEntry1 = new AccessControlEntry(identityDescriptor, allow, 0);
      IAccessControlEntry accessControlEntry2 = securityNamespace.GetAccessControlEntry(requestContext, token, identityDescriptor);
      if (accessControlEntry2 == null || accessControlEntry1.Allow == accessControlEntry2.Allow)
        return;
      accessControlEntry1.Allow |= accessControlEntry2.Allow;
      securityNamespace.SetAccessControlEntry(requestContext, token, (IAccessControlEntry) accessControlEntry1, false);
    }

    private static IAccessControlEntry GetAccessControlEntry(
      this IVssSecurityNamespace securityNamespace,
      IVssRequestContext requestContext,
      string token,
      IdentityDescriptor descriptor)
    {
      return securityNamespace.QueryAccessControlList(requestContext, token, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }, false)?.QueryAccessControlEntry(descriptor);
    }
  }
}
