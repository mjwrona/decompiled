// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointSecurity
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class ServiceEndpointSecurity
  {
    public static readonly char NamespaceSeparator = '/';
    public static readonly Guid NamespaceId = new Guid("49B48001-CA20-4ADC-8111-5B60C903A50C");
    public static readonly string Endpoints = "endpoints";
    public static readonly string Collection = nameof (Collection);
    private const string c_layer = "ServiceEndpointSecurity";

    public void InitializeServiceEndpointSecurity(
      IVssRequestContext requestContext,
      Guid endpointId,
      Guid projectId,
      Microsoft.VisualStudio.Services.Identity.Identity serviceEndpointCreator)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IdentityDescriptor[] serviceEndPointAdmins = new IdentityDescriptor[1]
      {
        serviceEndpointCreator.Descriptor
      };
      this.AddDefaultPermissionsForServiceEndpointGroups(requestContext1, (IEnumerable<IdentityDescriptor>) serviceEndPointAdmins, ServiceEndpointSecurity.GetServiceEndPointToken(projectId.ToString("D"), endpointId.ToString("D")));
      ServiceEndpointSecurity.AddCollectionLevelAdmin(requestContext1, ServiceEndpointSecurity.GetServiceEndPointToken(ServiceEndpointSecurity.Collection, endpointId.ToString("D")), serviceEndpointCreator.Descriptor);
    }

    public void AddDefaultPermissionsForServiceEndpointGroups(
      IVssRequestContext requestContext,
      IEnumerable<IdentityDescriptor> serviceEndPointAdmins,
      string endpointToken)
    {
      List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry> aces = new List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry>();
      foreach (IdentityDescriptor serviceEndPointAdmin in serviceEndPointAdmins)
      {
        Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceEndPointAdmin, 3, 0);
        aces.Add(accessControlEntry);
      }
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ServiceEndpointSecurity.NamespaceId).Secured();
      Microsoft.TeamFoundation.Framework.Server.AccessControlList accessControlList = new Microsoft.TeamFoundation.Framework.Server.AccessControlList(endpointToken, true, (IEnumerable<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry>) aces);
      IVssRequestContext requestContext1 = requestContext;
      securityNamespace.SetAccessControlLists(requestContext1, (IEnumerable<IAccessControlList>) new List<Microsoft.TeamFoundation.Framework.Server.AccessControlList>()
      {
        accessControlList
      });
    }

    private static void AddCollectionLevelAdmin(
      IVssRequestContext requestContext,
      string serviceEndPointCollectionToken,
      IdentityDescriptor identityDescriptor)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ServiceEndpointSecurity.NamespaceId).Secured().SetAccessControlEntries(requestContext, serviceEndPointCollectionToken, (IEnumerable<IAccessControlEntry>) new List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry>()
      {
        new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identityDescriptor, 2, 0)
      }, true);
    }

    public static string GetServiceEndPointToken(string projectId, string serviceEndpointId) => ServiceEndpointSecurity.GetServiceEndPointToken(serviceEndpointId.IsNullOrEmpty<char>() ? projectId : projectId + (object) ServiceEndpointSecurity.NamespaceSeparator + serviceEndpointId);

    public static string GetServiceEndPointToken(string resourceId)
    {
      string[] strArray = resourceId.Split(ServiceEndpointSecurity.NamespaceSeparator);
      if (strArray.Length != 2 || !string.Equals(strArray[0], ServiceEndpointSecurity.Collection, StringComparison.OrdinalIgnoreCase))
        return ServiceEndpointSecurity.Endpoints + (object) ServiceEndpointSecurity.NamespaceSeparator + resourceId;
      return ServiceEndpointSecurity.Endpoints + (object) ServiceEndpointSecurity.NamespaceSeparator + ServiceEndpointSecurity.Collection + (object) ServiceEndpointSecurity.NamespaceSeparator + strArray[1];
    }

    public static string GetServiceEndPointToken(Guid projectId) => ServiceEndpointSecurity.Endpoints + (object) ServiceEndpointSecurity.NamespaceSeparator + (object) projectId;

    public void CheckPermission(
      IVssRequestContext requestContext,
      string projectId,
      string serviceEndpointId,
      int permission,
      bool alwaysAllowAdministrators,
      Func<IVssRequestContext, string> getErrorMessageFunc)
    {
      using (new MethodScope(requestContext, nameof (ServiceEndpointSecurity), nameof (CheckPermission)))
      {
        if (!this.HasPermission(requestContext, projectId, serviceEndpointId, permission, alwaysAllowAdministrators))
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          throw new AccessCheckException(userIdentity.Descriptor, userIdentity.DisplayName, ServiceEndpointSecurity.GetServiceEndPointToken(projectId, serviceEndpointId), permission, ServiceEndpointSecurity.NamespaceId, getErrorMessageFunc(requestContext));
        }
      }
    }

    public void CheckCreateEndpointPermission(
      IVssRequestContext requestContext,
      string projectId,
      string serviceEndpointId,
      bool alwaysAllowAdministrators,
      Func<IVssRequestContext, string> getErrorMessageFunc)
    {
      using (new MethodScope(requestContext, nameof (ServiceEndpointSecurity), nameof (CheckCreateEndpointPermission)))
      {
        int num = this.HasPermission(requestContext, projectId, serviceEndpointId, 4, alwaysAllowAdministrators) ? 1 : 0;
        bool flag = false;
        if (num == 0)
          flag = this.HasPermission(requestContext, projectId, serviceEndpointId, 2, alwaysAllowAdministrators);
        if (num == 0 && !flag)
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          throw new AccessCheckException(userIdentity.Descriptor, userIdentity.DisplayName, ServiceEndpointSecurity.GetServiceEndPointToken(projectId, serviceEndpointId), 4, ServiceEndpointSecurity.NamespaceId, getErrorMessageFunc(requestContext));
        }
      }
    }

    public virtual void CheckFrameworkReadPermissions(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).Secured().CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);

    public virtual bool HasFrameworkReadPermissions(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).Secured().HasPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);

    public virtual bool HasPermission(
      IVssRequestContext requestContext,
      string projectId,
      string serviceEndpointId,
      int permission,
      bool alwaysAllowAdministrators = false)
    {
      using (new MethodScope(requestContext, nameof (ServiceEndpointSecurity), nameof (HasPermission)))
        return requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ServiceEndpointSecurity.NamespaceId).HasPermission(requestContext, ServiceEndpointSecurity.GetServiceEndPointToken(projectId, serviceEndpointId), permission, alwaysAllowAdministrators);
    }

    public virtual bool IsCallerServicePrincipal(
      IVssRequestContext requestContext,
      string projectId,
      string serviceEndpointId = null,
      bool alwaysAllowAdministrators = false)
    {
      return requestContext.IsSystemContext || ServicePrincipals.IsServicePrincipalThatCanViewSeviceEndpointSecrets(requestContext);
    }

    public void CheckCallerIsServicePrincipal(
      IVssRequestContext requestContext,
      string projectId,
      string serviceEndpointId = null,
      bool alwaysAllowAdministrators = false)
    {
      using (new MethodScope(requestContext, nameof (ServiceEndpointSecurity), nameof (CheckCallerIsServicePrincipal)))
      {
        if (!this.IsCallerServicePrincipal(requestContext, projectId, serviceEndpointId, alwaysAllowAdministrators))
        {
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          throw new AccessCheckException(userIdentity.Descriptor, userIdentity.DisplayName, ServiceEndpointSecurity.GetServiceEndPointToken(projectId, serviceEndpointId), 8, ServiceEndpointSecurity.NamespaceId, ServiceEndpointSdkResources.OperationNotAllowedForNonServicePrincipal());
        }
      }
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity ProvisionProjectLevelGroups(
      IVssRequestContext requestContext,
      IdentityService identityService,
      IdentityScope projectScope,
      Guid projectId)
    {
      using (new MethodScope(requestContext, nameof (ServiceEndpointSecurity), nameof (ProvisionProjectLevelGroups)))
      {
        ArgumentUtility.CheckForNull<IdentityScope>(projectScope, nameof (projectScope));
        IVssRequestContext requestContext1 = requestContext.Elevate();
        bool flag1 = false;
        bool flag2 = false;
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ServiceEndpointSecurity.NamespaceId);
        string token = ServiceEndpointSecurity.Endpoints + (object) ServiceEndpointSecurity.NamespaceSeparator + projectId.ToString("D");
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = identityService.GetIdentity(requestContext1, projectScope.Id, TaskWellKnownIdentityDescriptors.GlobalEndpointAdministrators);
        if (identity1 == null)
        {
          identity1 = identityService.CreateGroup(requestContext1, projectId, TaskWellKnownSecurityIds.GlobalEndpointAdministratorsGroup.Value, ServiceEndpointSdkResources.EndpointAdministratorsGroup(), ServiceEndpointSdkResources.ProjectLevelEndpointAdministratorsGroupDescription());
          if (identity1 == null)
          {
            requestContext.TraceError("ServiceEndpoints", "Failed to create endpoint adiministrator group for project scope: {0} and projectId: {1}", (object) projectScope.Id, (object) projectId);
            throw new ServiceEndpointException(ServiceEndpointSdkResources.EndpontGroupCreationFailed((object) ServiceEndpointSdkResources.EndpointAdministratorsGroup()));
          }
          flag1 = true;
          Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identity1.Descriptor, 7, 0);
          securityNamespace.SetAccessControlEntries(requestContext1, token, (IEnumerable<IAccessControlEntry>) new List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry>()
          {
            accessControlEntry
          }, true);
        }
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = identityService.GetIdentity(requestContext1, projectScope.Id, TaskWellKnownIdentityDescriptors.GlobalEndpointCreators);
        if (identity2 == null)
        {
          identity2 = identityService.CreateGroup(requestContext1, projectId, TaskWellKnownSecurityIds.GlobalEndpointCreatorsGroup.Value, ServiceEndpointSdkResources.EndpointCreatorsGroup(), ServiceEndpointSdkResources.EndpointCreatorsGroupDescription());
          if (identity2 == null)
          {
            requestContext.TraceError("ServiceEndpoints", "Failed to create endpoint creators group for project scope: {0} and projectId: {1}", (object) projectScope.Id, (object) projectId);
            throw new ServiceEndpointException(ServiceEndpointSdkResources.EndpontGroupCreationFailed((object) ServiceEndpointSdkResources.EndpointCreatorsGroup()));
          }
          flag2 = true;
          Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identity2.Descriptor, 4, 0);
          securityNamespace.SetAccessControlEntries(requestContext1, token, (IEnumerable<IAccessControlEntry>) new List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry>()
          {
            accessControlEntry
          }, true);
        }
        if (flag1)
        {
          try
          {
            identityService.AddMemberToGroup(requestContext1, identity1.Descriptor, projectScope.Administrators);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(34000212, "ServiceEndpoints", "ServiceEndpoints", ex);
            throw;
          }
        }
        if (flag2)
        {
          try
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity3 = identityService.ListGroups(requestContext1, new Guid[1]
            {
              projectId
            }, false, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group =>
            {
              if (group == null)
                return false;
              string property = group.GetProperty<string>("Account", string.Empty);
              return property != null && property.Equals(ServiceEndpointSdkResources.ContributorGroup(), StringComparison.OrdinalIgnoreCase);
            }));
            identityService.AddMemberToGroup(requestContext1, identity2.Descriptor, projectScope.Administrators);
            if (identity3 != null)
              identityService.AddMemberToGroup(requestContext1, identity2.Descriptor, identity3.Descriptor);
            else
              requestContext.TraceInfo(34000213, "ServiceEndpoints", "Cannot find contributor group in project {0}.", (object) projectId);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(34000212, "ServiceEndpoints", "ServiceEndpoints", ex);
            throw;
          }
        }
        return identity1;
      }
    }

    public virtual void RemoveServiceEndpointAccessControlList(
      IVssRequestContext systemRequestContext,
      Guid endpointId,
      string scope)
    {
      string serviceEndpointId = endpointId.ToString();
      ServiceEndpointSecurity.RemoveServiceEndpointAccessControlList(systemRequestContext, serviceEndpointId, scope);
    }

    public static void RemoveServiceEndpointAccessControlList(
      IVssRequestContext systemRequestContext,
      string serviceEndpointId,
      string scope)
    {
      systemRequestContext.CheckSystemRequestContext();
      systemRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(systemRequestContext, ServiceEndpointSecurity.NamespaceId)?.RemoveAccessControlLists(systemRequestContext, (IEnumerable<string>) new string[1]
      {
        ServiceEndpointSecurity.GetServiceEndPointToken(scope, serviceEndpointId)
      }, true);
    }

    public virtual void SetInheritance(
      IVssRequestContext requestContext,
      Guid endpointId,
      string scope,
      bool inheritFlag)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, ServiceEndpointSecurity.NamespaceId)?.SetInheritFlag(requestContext, ServiceEndpointSecurity.GetServiceEndPointToken(scope, endpointId.ToString()), inheritFlag);
    }

    public static void AssignProjectLevelAdminAsCollectionAdminForCreate(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      IVssRequestContext systemContext = requestContext.Elevate();
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap = (IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<string, Microsoft.VisualStudio.Services.Identity.Identity>();
      if (endpoint.ServiceEndpointProjectReferences == null || endpoint.ServiceEndpointProjectReferences.Count<ServiceEndpointProjectReference>() <= 0)
        return;
      endpoint.ServiceEndpointProjectReferences.ForEach<ServiceEndpointProjectReference>((Action<ServiceEndpointProjectReference>) (projectReference =>
      {
        if (projectReference != null && projectReference.ProjectReference != null)
        {
          Guid id1 = projectReference.ProjectReference.Id;
          IVssRequestContext systemRequestContext = systemContext;
          Guid id2 = projectReference.ProjectReference.Id;
          string projectId = id2.ToString();
          id2 = endpoint.Id;
          string endpointId = id2.ToString();
          IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap1 = identityMap;
          ServiceEndpointSecurity.AssignCollectionLevelAccessControlList(systemRequestContext, projectId, endpointId, identityMap1);
        }
        else
          requestContext.TraceInfo("ServiceEndpoints", "Project reference is empty for endpoint {0}", (object) endpoint.Id);
      }));
    }

    public static void AssignCollectionLevelAccessControlList(
      IVssRequestContext systemRequestContext,
      string projectId,
      string endpointId,
      IDictionary<string, Microsoft.VisualStudio.Services.Identity.Identity> identityMap)
    {
      systemRequestContext.CheckSystemRequestContext();
      string resourceId = projectId + "/" + endpointId;
      string serviceEndPointToken = ServiceEndpointSecurity.GetServiceEndPointToken(resourceId);
      IVssSecurityNamespace securityNamespace = systemRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(systemRequestContext, ServiceEndpointSecurity.NamespaceId).Secured();
      IAccessControlList accessControlList = securityNamespace.QueryAccessControlLists(systemRequestContext, serviceEndPointToken, true, false).FirstOrDefault<IAccessControlList>();
      if (accessControlList == null)
        return;
      if (accessControlList.Count <= 0)
        return;
      try
      {
        IEnumerable<IAccessControlEntry> accessControlEntries = accessControlList.AccessControlEntries.Where<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (x => (3 & x.EffectiveAllow) == 3));
        IdentityService service = systemRequestContext.GetService<IdentityService>();
        List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry> accessControlEntryList = new List<Microsoft.TeamFoundation.Framework.Server.AccessControlEntry>();
        foreach (IAccessControlEntry accessControlEntry1 in accessControlEntries)
        {
          try
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity;
            if (identityMap.ContainsKey(accessControlEntry1.Descriptor.Identifier))
            {
              identity = identityMap[accessControlEntry1.Descriptor.Identifier];
            }
            else
            {
              identity = service.GetIdentity(systemRequestContext, accessControlEntry1.Descriptor);
              identityMap.Add(accessControlEntry1.Descriptor.Identifier, identity);
            }
            if (identity != null && identity.IsActive && identity.Descriptor != (IdentityDescriptor) null && !identity.Descriptor.IsSystemServicePrincipalType())
            {
              Microsoft.TeamFoundation.Framework.Server.AccessControlEntry accessControlEntry2 = new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(accessControlEntry1.Descriptor, 2, 0);
              accessControlEntryList.Add(accessControlEntry2);
              systemRequestContext.TraceInfo(34000214, "ServiceEndpoints", "Adding identity {0} of resource id {1} as collection level administrator", (object) accessControlEntry1.Descriptor, (object) resourceId);
            }
            else
              systemRequestContext.TraceError(34000214, "ServiceEndpoints", "Identity for service endpoint id {0} is not found or not active", (object) endpointId);
          }
          catch (Exception ex)
          {
            systemRequestContext.TraceError(34000214, "ServiceEndpoints", "Failed to set project level admin of service endpoint {0} as collection level administrator with error: {1}", (object) resourceId, (object) ex);
          }
        }
        securityNamespace.SetAccessControlEntries(systemRequestContext, ServiceEndpointSecurity.GetServiceEndPointToken(ServiceEndpointSecurity.Collection, endpointId), (IEnumerable<IAccessControlEntry>) accessControlEntryList, true);
        systemRequestContext.TraceInfo(34000214, "ServiceEndpoints", "Successfully set resource {0} project admins as collection level administrator", (object) resourceId);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceError(34000214, "ServiceEndpoints", "Failed to set project level admin of service endpoint {0} as collection level administrator with error: {1}", (object) resourceId, (object) ex);
        throw;
      }
    }
  }
}
