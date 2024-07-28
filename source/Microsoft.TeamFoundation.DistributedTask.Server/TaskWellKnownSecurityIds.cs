// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskWellKnownSecurityIds
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskWellKnownSecurityIds
  {
    public static readonly SecurityIdentifier PoolServiceAccountsGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(8U, 2U));
    public static readonly SecurityIdentifier PoolServicePrincipalsGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(8U, 3U));
    public static readonly SecurityIdentifier QueueUsersGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(8U, 5U));
    public static readonly SecurityIdentifier QueueServicePrincipalsGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(8U, 6U));
    public static readonly SecurityIdentifier QueueCreatorsGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(8U, 7U));
    public static readonly SecurityIdentifier ServiceEndpointReadersGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(8U, 4U));
    public static readonly SecurityIdentifier GlobalEndpointAdministratorsGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(8U, 8U));
    public static readonly SecurityIdentifier GlobalEndpointCreatorsGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(8U, 9U));
    public static readonly SecurityIdentifier DeploymentGroupAdministratorsGroup = new SecurityIdentifier(SidIdentityHelper.ConstructWellKnownSid(8U, 10U));
  }
}
