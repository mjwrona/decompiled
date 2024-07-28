// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.AgentPoolRoles
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class AgentPoolRoles
  {
    public const int GlobalAdministrator = 59;
    public const int Administrator = 27;
    public const int Creator = 33;
    public const int User = 17;
    internal const int ServiceAccount = 21;
    public const int Reader = 1;
    private const string c_agentPoolAdminRole = "AgentPoolAdmin";
    private const string c_agentPoolServiceRole = "AgentPool";

    internal static bool IsAgentPoolServiceIdentity(IdentityDescriptor identityDescriptor)
    {
      string role;
      return IdentityHelper.TryParseFrameworkServiceIdentityDescriptor(identityDescriptor, out Guid _, out role, out string _) && ("AgentPoolAdmin".Equals(role, StringComparison.OrdinalIgnoreCase) || "AgentPool".Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    internal static string ToString(AgentPoolServiceAccountRoles role)
    {
      if (role == AgentPoolServiceAccountRoles.AgentPoolService)
        return "AgentPool";
      return role == AgentPoolServiceAccountRoles.AgentPoolAdmin ? "AgentPoolAdmin" : (string) null;
    }
  }
}
