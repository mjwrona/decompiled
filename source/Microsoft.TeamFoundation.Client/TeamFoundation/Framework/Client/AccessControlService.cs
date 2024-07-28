// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AccessControlService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class AccessControlService : IAccessControlService
  {
    private TfsConnection m_tfs;
    private AccessControlWebService m_accessControlWebService;

    internal AccessControlService(TfsConnection tfs)
    {
      this.m_tfs = tfs;
      this.m_accessControlWebService = new AccessControlWebService(tfs);
    }

    public ServiceIdentity ProvisionServiceIdentity() => this.ProvisionServiceIdentity((ServiceIdentityInfo) null, (IdentityDescriptor[]) null);

    public ServiceIdentity ProvisionServiceIdentity(string serviceIdentityName) => this.ProvisionServiceIdentity(serviceIdentityName, (string) null);

    public ServiceIdentity ProvisionServiceIdentity(string serviceIdentityName, string password) => this.ProvisionServiceIdentity(serviceIdentityName, password, (IdentityDescriptor[]) null);

    public ServiceIdentity ProvisionServiceIdentity(
      string serviceIdentityName,
      string password,
      IdentityDescriptor[] addToGroups)
    {
      return this.ProvisionServiceIdentity(new ServiceIdentityInfo(serviceIdentityName, password), addToGroups);
    }

    public ServiceIdentity ProvisionServiceIdentity(ServiceIdentityInfo identityInfo) => this.ProvisionServiceIdentity(identityInfo, (IdentityDescriptor[]) null);

    public ServiceIdentity ProvisionServiceIdentity(
      ServiceIdentityInfo identityInfo,
      IdentityDescriptor[] addToGroups)
    {
      return this.m_accessControlWebService.ProvisionServiceIdentity(identityInfo, addToGroups);
    }

    public ServiceIdentity QueryServiceIdentity(Guid serviceIdentityId) => this.QueryServiceIdentity(serviceIdentityId, false);

    public ServiceIdentity QueryServiceIdentity(Guid serviceIdentityId, bool includeMemberships) => this.QueryServiceIdentities(new Guid[1]
    {
      serviceIdentityId
    }, includeMemberships)[0];

    public ServiceIdentity[] QueryServiceIdentities(
      Guid[] serviceIdentityIds,
      bool includeMemberships)
    {
      return this.m_accessControlWebService.QueryServiceIdentitiesById(serviceIdentityIds, includeMemberships);
    }

    public ServiceIdentity QueryServiceIdentity(string serviceIdentityName) => this.QueryServiceIdentity(serviceIdentityName, false);

    public ServiceIdentity QueryServiceIdentity(string serviceIdentityName, bool includeMemberships) => this.QueryServiceIdentities(new string[1]
    {
      serviceIdentityName
    }, includeMemberships)[0];

    public ServiceIdentity[] QueryServiceIdentities(
      string[] serviceIdentityNames,
      bool includeMemberships)
    {
      return this.m_accessControlWebService.QueryServiceIdentities(serviceIdentityNames, includeMemberships);
    }

    public void DeleteServiceIdentity(ServiceIdentity serviceIdentity)
    {
      ArgumentUtility.CheckForNull<ServiceIdentity>(serviceIdentity, nameof (serviceIdentity));
      this.DeleteServiceIdentity(serviceIdentity.Identity.TeamFoundationId);
    }

    public void DeleteServiceIdentity(Guid serviceIdentityId) => this.m_accessControlWebService.DeleteServiceIdentity(serviceIdentityId);
  }
}
