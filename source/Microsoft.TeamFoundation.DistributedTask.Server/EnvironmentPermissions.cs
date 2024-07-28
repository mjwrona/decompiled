// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.EnvironmentPermissions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class EnvironmentPermissions
  {
    public const int View = 1;
    public const int Manage = 2;
    public const int ManageHistory = 4;
    public const int AdministerPermissions = 8;
    public const int Use = 16;
    public const int Create = 32;
    public const int AllPermissions = 63;
  }
}
