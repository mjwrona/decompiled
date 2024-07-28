// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskWellKnownIdentityDescriptors
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskWellKnownIdentityDescriptors
  {
    public static readonly IdentityDescriptor PoolServiceAccountsGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.PoolServiceAccountsGroup);
    public static readonly IdentityDescriptor PoolServicePrincipalsGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.PoolServicePrincipalsGroup);
    public static readonly IdentityDescriptor QueueUsersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.QueueUsersGroup);
    public static readonly IdentityDescriptor QueueServicePrincipalsGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.QueueServicePrincipalsGroup);
    public static readonly IdentityDescriptor QueueCreatorsGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.QueueCreatorsGroup);
    public static readonly IdentityDescriptor ServiceEndpointReadersGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.ServiceEndpointReadersGroup);
    public static readonly IdentityDescriptor GlobalEndpointAdministrators = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.GlobalEndpointAdministratorsGroup);
    public static readonly IdentityDescriptor GlobalEndpointCreators = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.GlobalEndpointCreatorsGroup);
    public static readonly IdentityDescriptor DeploymentGroupAdministratorsGroup = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.DeploymentGroupAdministratorsGroup);
  }
}
