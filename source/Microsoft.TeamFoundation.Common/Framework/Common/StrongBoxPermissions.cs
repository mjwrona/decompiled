// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.StrongBoxPermissions
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class StrongBoxPermissions
  {
    public const int CreateDrawer = 1;
    public const int DeleteDrawer = 2;
    public const int AdministerStrongBox = 4;
    public const int AddItem = 16;
    public const int GetItem = 32;
    public const int DeleteItem = 64;
    public const int AdministerDrawer = 128;
    public const int AllStrongBox = 7;
    public const int AllDrawer = 240;
  }
}
