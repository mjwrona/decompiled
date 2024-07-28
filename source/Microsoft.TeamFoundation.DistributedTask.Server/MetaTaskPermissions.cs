// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MetaTaskPermissions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class MetaTaskPermissions
  {
    public const int Administer = 1;
    public const int Edit = 2;
    public const int Delete = 4;
    public static readonly int AllPermissions = 7;
  }
}
