// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.AgentPoolRolePermissionsName
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class AgentPoolRolePermissionsName
  {
    public static readonly IList<string> Administrator = (IList<string>) new List<string>()
    {
      TaskResources.AdministerPermissions(),
      TaskResources.Manage(),
      TaskResources.View(),
      TaskResources.Use()
    };
    public static readonly IList<string> Creator = (IList<string>) new List<string>()
    {
      TaskResources.Create(),
      TaskResources.View()
    };
    public static readonly IList<string> User = (IList<string>) new List<string>()
    {
      TaskResources.View(),
      TaskResources.Use()
    };
    public static readonly IList<string> Reader = (IList<string>) new List<string>()
    {
      TaskResources.View()
    };
  }
}
