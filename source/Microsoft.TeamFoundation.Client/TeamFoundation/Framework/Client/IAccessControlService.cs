// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.IAccessControlService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface IAccessControlService
  {
    ServiceIdentity ProvisionServiceIdentity();

    ServiceIdentity ProvisionServiceIdentity(string serviceIdentityName);

    ServiceIdentity ProvisionServiceIdentity(string serviceIdentityName, string password);

    ServiceIdentity ProvisionServiceIdentity(
      string serviceIdentityName,
      string password,
      IdentityDescriptor[] addToGroups);

    ServiceIdentity ProvisionServiceIdentity(ServiceIdentityInfo identityInfo);

    ServiceIdentity ProvisionServiceIdentity(
      ServiceIdentityInfo identityInfo,
      IdentityDescriptor[] addToGroups);

    ServiceIdentity QueryServiceIdentity(Guid serviceIdentityId);

    ServiceIdentity QueryServiceIdentity(Guid serviceIdentityId, bool includeMemberships);

    ServiceIdentity[] QueryServiceIdentities(Guid[] serviceIdentityIds, bool includeMemberships);

    ServiceIdentity QueryServiceIdentity(string serviceIdentityName);

    ServiceIdentity QueryServiceIdentity(string serviceIdentityName, bool includeMemberships);

    ServiceIdentity[] QueryServiceIdentities(string[] serviceIdentityNames, bool includeMemberships);

    void DeleteServiceIdentity(ServiceIdentity serviceIdentity);

    void DeleteServiceIdentity(Guid serviceIdentityId);
  }
}
