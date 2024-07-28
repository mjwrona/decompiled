// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security.AgentPoolPermissions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security
{
  public static class AgentPoolPermissions
  {
    public const int View = 1;
    public const int Manage = 2;
    public const int Listen = 4;
    public const int AdministerPermissions = 8;
    public const int Use = 16;
    public const int Create = 32;
    public const int AllPermissions = 63;
  }
}
