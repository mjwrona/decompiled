// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.SecuredRegistrationManager
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Security;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class SecuredRegistrationManager : IVssFrameworkService
  {
    private TeamFoundationSecurityService m_securityService;
    private RegistrationProvider m_registrationProvider;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_securityService = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetService<TeamFoundationSecurityService>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_registrationProvider = systemRequestContext.GetService<RegistrationProvider>();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public FrameworkRegistrationEntry[] GetRegistrationEntries(
      IVssRequestContext requestContext,
      string toolId)
    {
      IVssSecurityNamespace securityNamespace1 = this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId);
      if (!securityNamespace1.HasPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1))
        throw new SecurityException(FrameworkResources.UnauthorizedAccessException((object) requestContext.DomainUserName, (object) string.Join(", ", securityNamespace1.Description.GetLocalizedActions(1))));
      FrameworkRegistrationEntry[] registrationEntries = this.m_registrationProvider.GetRegistrationEntries(requestContext, toolId);
      IVssSecurityNamespace securityNamespace2 = this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.RegistryNamespaceId);
      foreach (FrameworkRegistrationEntry registrationEntry in registrationEntries)
      {
        List<RegistrationDatabase> registrationDatabaseList = new List<RegistrationDatabase>();
        foreach (RegistrationDatabase database in registrationEntry.Databases)
        {
          if (database.SourceRegistryPath == null || securityNamespace2.HasPermission(requestContext, database.SourceRegistryPath, 1))
            registrationDatabaseList.Add(database);
        }
        registrationEntry.Databases = registrationDatabaseList.ToArray();
        List<RegistrationExtendedAttribute2> extendedAttribute2List = new List<RegistrationExtendedAttribute2>();
        foreach (RegistrationExtendedAttribute2 extendedAttribute in registrationEntry.RegistrationExtendedAttributes)
        {
          if (extendedAttribute.SourceRegistryPath == null || securityNamespace2.HasPermission(requestContext, extendedAttribute.SourceRegistryPath, 1))
            extendedAttribute2List.Add(extendedAttribute);
        }
        registrationEntry.RegistrationExtendedAttributes = extendedAttribute2List.ToArray();
        List<RegistrationServiceInterface> serviceInterfaceList = new List<RegistrationServiceInterface>();
        IVssSecurityNamespace securityNamespace3 = this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
        foreach (RegistrationServiceInterface serviceInterface in registrationEntry.ServiceInterfaces)
        {
          if (serviceInterface.ProjectUri == null || securityNamespace3.HasPermission(requestContext, securityNamespace3.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace3, serviceInterface.ProjectUri), TeamProjectPermissions.GenericRead))
            serviceInterfaceList.Add(serviceInterface);
        }
        registrationEntry.ServiceInterfaces = serviceInterfaceList.ToArray();
      }
      return registrationEntries;
    }
  }
}
