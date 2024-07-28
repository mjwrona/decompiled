// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.WellKnownGroupsInfo
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class WellKnownGroupsInfo
  {
    public readonly string ValidUsersGroupName;
    public readonly string ValidUsersGroupDescription;
    public readonly string AdministratorsGroupName;
    public readonly string AdministratorsGroupDescription;
    public readonly string ServiceAccountsGroupName;
    public readonly string ServiceAccountsGroupDescription;

    public WellKnownGroupsInfo(IVssRequestContext requestContext)
    {
      bool flag = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection);
      bool hostedDeployment = requestContext.ExecutionEnvironment.IsHostedDeployment;
      this.ValidUsersGroupName = flag ? FrameworkResources.ProjectCollectionValidUsers() : (hostedDeployment ? FrameworkResources.OrganizationValidUsersGroupName() : FrameworkResources.ValidUsersGroupName());
      this.ValidUsersGroupDescription = flag ? FrameworkResources.ProjectCollectionValidUsersDescription() : (hostedDeployment ? FrameworkResources.OrganizationValidUsersGroupDescription() : FrameworkResources.ValidUsersGroupDescription());
      this.AdministratorsGroupName = flag ? FrameworkResources.ProjectCollectionAdministrators() : (hostedDeployment ? FrameworkResources.OrganizationAdministratorsGroupName() : FrameworkResources.AdministratorsGroupName());
      this.AdministratorsGroupDescription = flag ? FrameworkResources.ProjectCollectionAdministratorsDescription() : (hostedDeployment ? FrameworkResources.OrganizationAdministratorsGroupDescription() : FrameworkResources.AdministratorsGroupDescription());
      this.ServiceAccountsGroupName = flag ? FrameworkResources.ProjectCollectionServiceAccounts() : (hostedDeployment ? FrameworkResources.OrganizationServiceAccountsGroupName() : FrameworkResources.ServiceGroupName());
      this.ServiceAccountsGroupDescription = flag ? FrameworkResources.ProjectCollectionServiceAccountsDescription() : (hostedDeployment ? FrameworkResources.OrganizationServiceAccountsGroupDescription() : FrameworkResources.ServiceGroupDescription());
    }
  }
}
