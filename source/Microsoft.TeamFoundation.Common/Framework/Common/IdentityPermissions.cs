// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.IdentityPermissions
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class IdentityPermissions
  {
    public const int Read = 1;
    public const int Write = 2;
    public const int Delete = 4;
    public const int ManageMembership = 8;
    public const int CreateScope = 16;
    public const int RestoreScope = 32;
    public const int ForceDelete = 64;
    public const int AllPermissions = 31;
  }
}
